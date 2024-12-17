using EIUSmartWarehouse.Models.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EIUSmartWarehouse.Controllers
{
    public class LoginController : Controller
    {
        private readonly DBContext _DBContext;
        public LoginController(DBContext context)
        {
            _DBContext = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string uName, string uPass, string mode)
        {
            if (mode == "login")
            {
                // Hash the password (SHA-256)
                string hashedPassword = HashPassword(uPass);

                // Check if it's admin login
                if (uName == "admin" && hashedPassword == HashPassword("123456"))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, uName),
                        new Claim(ClaimTypes.Role, "Admin")  // Add the "Admin" role claim
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Optional: makes the cookie persistent
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30) // Set expiration time for cookie
                    };
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    HttpContext.Session.SetString("LoggedInUser", uName);
                    return RedirectToAction("HomeAdmin", "Admin");
                }

                // Check if it's staff login
                var staff = _DBContext.Staff.FirstOrDefault(s => s.StaffID == uName && s.Password == hashedPassword);
                if (staff != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, staff.StaffName),
                        new Claim(ClaimTypes.Role, "Staff")  // Staff role
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    };
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    HttpContext.Session.SetString("LoggedInUser", staff.StaffName);
                    return RedirectToAction("HomeStaff", "Staff");
                }

                // Check if it's customer login
                var customer = _DBContext.Customer.FirstOrDefault(c => c.CustomerCode == uName && c.Password == hashedPassword);
                if (customer != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, customer.CustomerName),
                        new Claim(ClaimTypes.Role, "Customer")  // Customer role
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    };
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    HttpContext.Session.SetString("LoggedInUser", customer.CustomerName);
                    return RedirectToAction("HomeCustomer", "Customer");
                }

                // If none match
                TempData["Message"] = "Incorrect Username or Password.";
                return View();
            }
            else if (mode == "logout")
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }

            return View();
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var byteVal in bytes)
                {
                    builder.Append(byteVal.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
