//using AgriEnergyConnect.Data;
//using AgriEnergyConnect.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.WebUtilities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Text;
//using System.Text.Encodings.Web;
//using System.Threading.Tasks;

//namespace AgriEnergyConnect.Controllers
//{
//    public class RequestsController : Controller
//    {
//        private readonly AgriConnectContext _context;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly UserManager<DefaultUser> _userManager;
//        private readonly IEmailSender _emailSender;
//        private readonly ILogger<RequestsController> _logger;
//        private readonly IUserStore<DefaultUser> _userStore;
//        private readonly IUserEmailStore<DefaultUser>  _emailStore;

//        public RequestsController(
//    AgriConnectContext context,
//    RoleManager<IdentityRole> roleManager,
//    UserManager<DefaultUser> userManager,
//    IUserStore<DefaultUser> userStore,
//    IUserEmailStore<DefaultUser> emailStore,
//    IEmailSender emailSender,
//    ILogger<RequestsController> logger
//)
//        {
//            _context = context;
//            _roleManager = roleManager;
//            _userManager = userManager;
//            _userStore = userStore;
//            _emailStore = emailStore; // Ensure this cast is valid
//            _emailSender = emailSender;
//            _logger = logger;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var requests = await _context.RegistrationRequests.ToListAsync();
//            return View(requests);
//        }

//        public async Task<IActionResult> Approve(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var request = await _context.RegistrationRequests.FirstOrDefaultAsync(r => r.Id == id);
//            if (request == null)
//            {
//                return NotFound();
//            }

//            var user = new DefaultUser
//            {
//                UserName = $"{request.FirstName}{request.LastName}",
//                Email = request.Email,
//                FirstName = request.FirstName,
//                LastName = request.LastName,
//                PhoneNumber = request.PhoneNumber,
//                Address = request.PhysicalAddress,
//                City = request.City
//            };

//            var password = GeneratePassword();
//            var result = await _userManager.CreateAsync(user, password);

//            if (result.Succeeded)
//            {
//                if (!await _roleManager.RoleExistsAsync("Farmer"))
//                {
//                    await _roleManager.CreateAsync(new IdentityRole("Farmer"));
//                }

//                var roleAssignmentResult = await _userManager.AddToRoleAsync(user, "Farmer");
//                if (!roleAssignmentResult.Succeeded)
//                {
//                    _logger.LogError("Failed to assign role to user.");
//                    TempData["ErrorMessage"] = "Failed to assign role to user.";
//                    return RedirectToAction("Index");
//                }

//                // Generate email confirmation token
//                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
//                var callbackUrl = Url.Page(
//                    "/Account/ConfirmEmail",
//                    pageHandler: null,
//                    values: new { area = "Identity", userId = user.Id, code = code },
//                    protocol: Request.Scheme);

//                await _emailSender.SendEmailAsync(user.Email, "Account Approved",
//                    $"Your account has been approved. You can now log in using the following password: {password}. " +
//                    $"Please confirm your email by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

//                _context.RegistrationRequests.Remove(request);
//                await _context.SaveChangesAsync();

//                _logger.LogInformation("User created and approved successfully.");
//                return RedirectToAction("Index");
//            }

//            _logger.LogError("Failed to create user account.");
//            TempData["ErrorMessage"] = "Failed to create user account.";
//            return RedirectToAction("Index");
//        }


//        private string GeneratePassword()
//        {
//            const int length = 12;
//            const string lower = "abcdefghijklmnopqrstuvwxyz";
//            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
//            const string digits = "1234567890";
//            const string nonAlpha = "!@#$%^&*()";
//            const string allChars = lower + upper + digits + nonAlpha;

//            StringBuilder res = new StringBuilder();
//            Random rnd = new Random();

//            res.Append(lower[rnd.Next(lower.Length)]);
//            res.Append(upper[rnd.Next(upper.Length)]);
//            res.Append(digits[rnd.Next(digits.Length)]);
//            res.Append(nonAlpha[rnd.Next(nonAlpha.Length)]);

//            for (int i = res.Length; i < length; i++)
//            {
//                res.Append(allChars[rnd.Next(allChars.Length)]);
//            }

//            return new string(res.ToString().OrderBy(c => rnd.Next()).ToArray());
//        }
//    }
//}