using E_commerce_MobileStore.Models;
using E_commerce_MobileStore.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_commerce_MobileStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        private readonly MobileStoreManagementContext _context;
        private readonly IValidator<User> _validator;
        private readonly IConfiguration _configuration;
        
        public UserAPIController(MobileStoreManagementContext context, IValidator<User> validator, IConfiguration configuration)
        {
            _context = context;
            _validator = validator;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .OrderBy(u => u.UserName)
                    .ToListAsync();
                
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(c => c.UserId == id);

                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Check if user has related records
                var hasCategories = await _context.Categories.AnyAsync(c => c.UserId == id);
                var hasProducts = await _context.Products.AnyAsync(p => p.UserId == id);
                var hasCarts = await _context.Carts.AnyAsync(c => c.UserId == id);
                var hasOrders = await _context.Orders.AnyAsync(o => o.UserId == id);

                if (hasCategories || hasProducts || hasCarts || hasOrders)
                {
                    return BadRequest(new { message = "Cannot delete user with existing categories, products, cart items, or orders" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the user", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertUser([FromBody] User user)
        {
            // FluentValidation
            var validationResult = await _validator.ValidateAsync(user);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Validation failed", errors = validationResult.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage }) });
            }

            // Check for duplicate username
            if (await _context.Users.AnyAsync(u => u.UserName == user.UserName))
            {
                return Conflict(new { message = "Username already exists" });
            }

            // Check for duplicate email if provided
            if (!string.IsNullOrEmpty(user.Email) && await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return Conflict(new { message = "Email already exists" });
            }

            try
            {
                // Reset UserId to 0 to ensure it's auto-generated
                user.UserId = 0;
                user.Created = DateTime.Now;
                user.Modified = DateTime.Now;
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the user", error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "User data is null" });
            }
            
            if (id != user.UserId)
            {
                return BadRequest(new { message = "User ID mismatch" });
            }

            // Check if user exists
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Check for duplicate username (excluding current user)
            if (await _context.Users.AnyAsync(u => u.UserName == user.UserName && u.UserId != id))
            {
                return Conflict(new { message = "Username already exists" });
            }

            // Check for duplicate email if provided (excluding current user)
            if (!string.IsNullOrEmpty(user.Email) && await _context.Users.AnyAsync(u => u.Email == user.Email && u.UserId != id))
            {
                return Conflict(new { message = "Email already exists" });
            }

            try
            {
                // Preserve original creation date
                user.Created = existingUser.Created;
                user.Modified = DateTime.Now;
                
                _context.Entry(existingUser).CurrentValues.SetValues(user);
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(id))
                    return NotFound(new { message = "User not found" });
                else
                    return StatusCode(409, new { message = "User was modified by another user" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the user", error = ex.Message });
            }
        }
        
        // A. Generate Token with Role & Expiry from appsettings.json
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),  
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim("IsActive", user.IsActive.ToString())
            };
            
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:TokenExpiryMinutes"])),
                signingCredentials: credentials);
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        // B. Admin only role restriction
        [HttpGet("admin-only")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminOnlyContent()
        {
            var context = HttpContext;
            var claims = context.User.Claims.ToDictionary(x => x.Type, x => x.Value);
            
            // This will require an authenticated user with role restriction
            return Ok(new { message = "Welcome Authenticated Admin User!", claims });
        }
        
        // C. Generic with specific roles
        [HttpGet("roles-demo")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetRolesDemo()
        {
            return Ok(new { message = "This is admin area, or admin required." });
        }
        
        // D. Allow anonymous access (non-admin controller action)
        [HttpGet("anonymous")]
        [AllowAnonymous] 
        public IActionResult GetAnonymousData()
        {
            return Ok(new { message = "This is public area, no login required." });
        }
        
        // E. Login API
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null)
                return BadRequest("Invalid username or password");
                
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == loginRequest.UserName && u.Password == loginRequest.Password);
                
            if (user == null)
                return Unauthorized("Invalid username or password");
                
            if (!user.IsActive)
                return Unauthorized("Account is inactive");
                
            var token = GenerateJwtToken(user);
            
            return Ok(new
            {
                Data = user,
                User = new { user.UserName, user.Email, user.IsAdmin },
                Token = token
            });
        }
        
        private async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.UserId == id);
        }
    }
    
    public class LoginRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
