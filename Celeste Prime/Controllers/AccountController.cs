using CelestePrime.Data;
using CelestePrime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CelestePrime.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Show Login Page
        public IActionResult Login()
        {
            return View();
        }

        // Show Register Page
        public IActionResult Register()
        {
            return View();
        }

        // Handle Registration (Fixed Duplicate Issue)
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
            }

            if (_context.Users.Any(u => u.Email == user.Email || u.Username == user.Username))
            {
                ModelState.AddModelError("", "Email or Username already exists.");
            }

            if (string.IsNullOrEmpty(user.Role))
            {
                ModelState.AddModelError("Role", "Role selection is required.");
            }

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("ModelState Error: " + error.ErrorMessage);
                }
                return View(user);
            }

            // Hash the password before saving
            user.PasswordHash = HashPassword(user.Password);
            user.Password = string.Empty; // Security measure: clear raw password

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // Handle Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null)
            {
                _logger.LogError($"User not found: {model.Username}");
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            if (!VerifyPassword(model.Password, user.PasswordHash))
            {
                _logger.LogError($"Password verification failed for user: {model.Username}");
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            _logger.LogInformation($"User {model.Username} logged in successfully.");

            // Redirect based on role
            return user.Role switch
            {
                "Admin" => RedirectToAction("Index", "AdminDashboard"),
                "Homeowner" => RedirectToAction("Index", "HomeownerDashboard"),
                "SubdivisionStaff" => RedirectToAction("Index", "StaffDashboard"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        // Password Hashing
        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32);

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        // Verify Password
        private bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash) || !storedHash.Contains(":"))
            {
                _logger.LogError("Invalid password hash format");
                return false;
            }

            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32);

            return hash.SequenceEqual(storedHashBytes);
        }
    }
}
