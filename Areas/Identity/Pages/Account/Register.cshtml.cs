// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using AgriEnergyConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace AgriEnergyConnect.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<DefaultUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<DefaultUser> _userManager;
        private readonly IUserStore<DefaultUser> _userStore;
        private readonly IUserEmailStore<DefaultUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly INotyfService _notyf;

        public RegisterModel(
            UserManager<DefaultUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<DefaultUser> userStore,
            SignInManager<DefaultUser> signInManager,
            ILogger<RegisterModel> logger,
            INotyfService notyf,
            IEmailSender emailSender)
        {

            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _notyf = notyf;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Physical Address")]
            public string PhysicalAddress { get; set; }
            [Required]
            [Display(Name = "City")]
            public string City { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                //var user = CreateUser();

                var user = new DefaultUser
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Email = Input.Email,
                    Address = Input.PhysicalAddress,
                    City = Input.City,

                };

                await _userStore.SetUserNameAsync(user, $"{Input.FirstName}{Input.LastName}", CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // Generate a random password
                var password = GeneratePassword();
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    //Send Login Credentials to the User's email address
                    await _emailSender.SendEmailAsync(Input.Email, "Agri-Energy Connect Registration",
                        $"Your account has been registered. Please verify your email by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>." +
                        $"\nCredentials:\n\n" +
                        $"Username:{user.FirstName}{user.LastName}\n" +
                        $"Password:{password}");

                    //Assign the user to the farmer role
                    var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "Farmer");
                    if (!roleAssignmentResult.Succeeded)
                    {
                        foreach (var error in roleAssignmentResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        _notyf.Success("User Successfully Registered");
                        return RedirectToAction("Index", "Farmer");
                        
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private DefaultUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<DefaultUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(DefaultUser)}'. " +
                    $"Ensure that '{nameof(DefaultUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<DefaultUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<DefaultUser>)_userStore;
        }

        private string GeneratePassword()
        {
            const int length = 12;
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "1234567890";
            const string nonAlpha = "!@#$%^&*()";
            const string allChars = lower + upper + digits + nonAlpha;

            StringBuilder res = new StringBuilder();
            Random rnd = new Random();

            // Ensure at least one character from each category
            res.Append(lower[rnd.Next(lower.Length)]);
            res.Append(upper[rnd.Next(upper.Length)]);
            res.Append(digits[rnd.Next(digits.Length)]);
            res.Append(nonAlpha[rnd.Next(nonAlpha.Length)]);

            // Fill the remaining length with random characters
            for (int i = res.Length; i < length; i++)
            {
                res.Append(allChars[rnd.Next(allChars.Length)]);
            }

            // Shuffle the result to mix the characters
            return new string(res.ToString().OrderBy(c => rnd.Next()).ToArray());
        }
    }
}