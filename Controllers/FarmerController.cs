using AgriEnergyConnect.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AgriEnergyConnect.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;

namespace AgriEnergyConnect.Controllers
{
    // Controller for managing farmers, accessible only to users in the "Employee" role
    [Authorize(Roles = "Employee")]
    public class FarmerController : Controller
    {
        private readonly AgriConnectContext _context;
        private readonly UserManager<DefaultUser> _userManager;

        // Constructor to initialize required services and repositories
        public FarmerController(UserManager<DefaultUser> userManager, AgriConnectContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Displays a list of all users in the "Farmer" role
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var farmers = await _userManager.GetUsersInRoleAsync("Farmer");
            var identityFarmers = farmers.Cast<IdentityUser>().ToList();
            return View(identityFarmers);
        }

        // Displays details of products owned by a specific farmer, with optional search and date filters
        [HttpGet]
        public async Task<IActionResult> Details(string? id, string searchString, DateOnly? minDate, DateOnly? maxDate)
        {
            // Check if the Id parameter is null
            if (id == null)
            {
                return NotFound();
            }

            // Query to retrieve products for the specified user
            var productsQuery = _context.Products.Where(p => p.UserId == id);

            // Apply search filter if searchString is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString) || p.Category.Contains(searchString));
            }

            // Apply date filters if minDate and/or maxDate are provided
            if (minDate != null)
            {
                productsQuery = productsQuery.Where(p => p.ProdDate >= minDate);
            }

            if (maxDate != null)
            {
                productsQuery = productsQuery.Where(p => p.ProdDate <= maxDate);
            }

            // Execute the query and retrieve the list of products
            var products = await productsQuery.ToListAsync();

            // Check if no products were found
            if (products == null || !products.Any())
            {
                // If no products found, set a message in ViewBag and return a view
                ViewBag.Message = "No products found matching the criteria.";
                return View();
            }

            // Return the view with the list of products
            return View(products);
        }
    }
}
