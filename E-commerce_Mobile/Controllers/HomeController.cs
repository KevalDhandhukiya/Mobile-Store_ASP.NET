using Microsoft.AspNetCore.Mvc;
using E_commerce_Mobile.Models;

namespace E_commerce_Mobile.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult TestSession()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userName = HttpContext.Session.GetString("UserName");
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            
            ViewBag.UserId = userId ?? "Not set";
            ViewBag.UserName = userName ?? "Not set";
            ViewBag.IsAdmin = isAdmin ?? "Not set";
            
            return View();
        }
    }
}
