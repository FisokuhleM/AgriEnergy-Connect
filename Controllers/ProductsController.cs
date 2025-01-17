using AgriEnergyConnect.Data;
using AgriEnergyConnect.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgriEnergyConnect.Controllers
{
    // Controller for managing products, accessible only to users in the "Farmer" role
    [Authorize(Roles = "Farmer")]
    public class ProductsController : Controller
    {
        private readonly AgriConnectContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<DefaultUser> _userManager;
        private readonly INotyfService _notyf;

        // Constructor to initialize required services and repositories
        public ProductsController(AgriConnectContext context, IWebHostEnvironment environment,
                                  UserManager<DefaultUser> userManager, INotyfService notyf)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
            _notyf = notyf ?? throw new ArgumentNullException(nameof(notyf));
        }

        // Displays a list of products owned by the current user
        // GET: Products
        public async Task<IActionResult> Index()
        {
            // Get the current user's ID
            var userId = _userManager.GetUserId(User);

            // Retrieve products that belong to the current user
            var products = await _context.Products
                .Where(p => p.UserId == userId)
                .ToListAsync();

            return View(products);
        }

        // Allows both "Farmer" and "Employee" roles to view product details
        // GET: Products/Details/5
        [Authorize(Roles = "Farmer,Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            // Check if the 'id' parameter is null
            if (id == null)
            {
                return NotFound();
            }

            // Find the product with the given 'id'
            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // Displays the form for creating a new product
        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // Handles form submission for creating a new product
        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Category,ProdDate,ImageFile")] Product product)
        {
            // Get current user's ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            product.UserId = userId;

            // Process uploaded image file, if provided
            string uniqueFileName = ProcessUploadedFile(product);
            product.imageURL = "/images/" + uniqueFileName;

            // Add new product to database and save changes
            _context.Add(product);
            await _context.SaveChangesAsync();

            // Display success notification using toast notification service
            _notyf.Success("Product has been added successfully");

            // Redirect to the index page after successful product creation
            return RedirectToAction("Index");
        }

        // Checks if a product with the specified ID exists in the database
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        // Processes the uploaded file and returns a unique file name
        private string ProcessUploadedFile(Product product)
        {
            string uniqueFileName = null;

            // Check if an image file was uploaded
            if (product.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + product.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Copy uploaded file to the specified file path
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    product.ImageFile.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
