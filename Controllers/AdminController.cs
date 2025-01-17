using AgriEnergyConnect.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AgriEnergyConnect.Controllers
{
    // Controller for administrative tasks, accessible to admin users
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<DefaultUser> _userManager;

        // Constructor to initialize RoleManager and UserManager
        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<DefaultUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Displays the index page for admin tasks
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Displays the form for adding a new farmer
        [HttpGet]
        public IActionResult AddFarmer()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> AddFarmer()
        //{
        //    return
        //}

    }
}
