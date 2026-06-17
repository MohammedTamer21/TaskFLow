using Microsoft.AspNetCore.Mvc;
using TaskFlow.Models;
using System.Security.Cryptography;
using System.Text;

namespace TaskFlow.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext context;

        public AccountController(AppDbContext ctx)
        {
            context = ctx;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, string password)
        {
            user.PasswordHash = HashPassword(password);
            user.CreatedAt = DateTime.Now;
            context.Users.Add(user);
            context.SaveChanges();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            string passwordHash = HashPassword(password);
            var user = context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("Index", "Task");
            }
            ViewBag.Error = "Invalid login";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = context.Users.Find(userId);
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = context.Users.Find(userId);
            if (user.PasswordHash != HashPassword(currentPassword))
            {
                TempData["Error"] = "Current password is incorrect";
                return RedirectToAction("Profile");
            }

            user.PasswordHash = HashPassword(newPassword);
            context.SaveChanges();

            TempData["Success"] = "Password changed successfully";
            return RedirectToAction("Profile");
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}