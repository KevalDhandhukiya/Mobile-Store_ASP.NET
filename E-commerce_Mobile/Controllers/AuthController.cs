using E_commerce_Mobile.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace E_commerce_Mobile.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Error = "Please enter username and password.";
                return View(model);
            }

            // Hard-coded check for testing
            if ((model.UserName == "keval" && model.Password == "keval@123") ||
                (model.UserName == "string" && model.Password == "string"))
            {
                // Set session
                HttpContext.Session.SetString("UserId", "13");
                HttpContext.Session.SetString("UserName", model.UserName);
                HttpContext.Session.SetString("IsAdmin", "true");
                
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Invalid username or password.";
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();
            
            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }

    public class LoginModel
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }

    public class LoginResponseModel
    {
        public UserModel? Data { get; set; }
        public UserInfo? User { get; set; }
        public string? Token { get; set; }
    }

    public class UserInfo
    {
        public string UserName { get; set; } = null!;
        public string? Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
