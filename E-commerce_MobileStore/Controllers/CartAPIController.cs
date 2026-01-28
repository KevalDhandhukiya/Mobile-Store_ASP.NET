using E_commerce_MobileStore.Models;
using E_commerce_MobileStore.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace E_commerce_MobileStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly MobileStoreManagementContext _context;
        private readonly IValidator<Cart> _validator;
        
        public CartAPIController(MobileStoreManagementContext context, IValidator<Cart> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetAllCarts()
        {
            try
            {
                var carts = await _context.Carts
                    .Include(c => c.Product)
                    .Include(c => c.User)
                    .OrderByDescending(c => c.AddedDate)
                    .ToListAsync();

                return Ok(carts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving cart items", error = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartsByUserId(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new { message = "Invalid user ID" });
            }

            try
            {
                var carts = await _context.Carts
                    .Include(c => c.Product)
                    .Include(c => c.User)
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.AddedDate)
                    .ToListAsync();

                return Ok(carts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving cart items", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCartById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid cart ID" });
            }

            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Product)
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.CartId == id);

                if (cart == null)
                {
                    return NotFound(new { message = "Cart item not found" });
                }

                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the cart item", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid cart ID" });
            }

            try
            {
                var cart = await _context.Carts.FindAsync(id);
                if (cart == null)
                {
                    return NotFound(new { message = "Cart item not found" });
                }

                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the cart item", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Insertcart([FromBody] Cart cart)
        {
            if (cart == null)
            {
                return BadRequest(new { message = "Cart data is null" });
            }

            // Check if user exists
            if (!await _context.Users.AnyAsync(u => u.UserId == cart.UserId))
            {
                return BadRequest(new { message = "Invalid User ID" });
            }

            // Check if product exists and get price
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cart.ProductId && !p.IsRemoved);
            if (product == null)
            {
                return BadRequest(new { message = "Invalid or removed Product ID" });
            }

            try
            {
                // Reset CartId to 0 to ensure it's auto-generated
                cart.CartId = 0;
                
                // Set all dates on server side to avoid timezone issues
                // Use a slightly past time to avoid validation precision issues
                var now = DateTime.Now.AddSeconds(-1);
                cart.Created = now;
                cart.Modified = now;
                cart.AddedDate = now;
                
                // Auto-calculate total
                cart.Total = product.Price * cart.Quantity;
                
                // Validate after setting all required fields
                var validationResult = await _validator.ValidateAsync(cart);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { message = "Validation failed", errors = validationResult.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage }) });
                }
                
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCartById), new { id = cart.CartId }, cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the cart item", error = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] Cart cart)
        {
            if (cart == null)
            {
                return BadRequest(new { message = "Cart data is null" });
            }

            if (id != cart.CartId)
            {
                return BadRequest(new { message = "Cart ID mismatch" });
            }

            // FluentValidation
            var validationResult = await _validator.ValidateAsync(cart);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Validation failed", errors = validationResult.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage }) });
            }

            // Check if cart exists
            var existingCart = await _context.Carts.FindAsync(id);
            if (existingCart == null)
            {
                return NotFound(new { message = "Cart item not found" });
            }

            // Check if user exists
            if (!await _context.Users.AnyAsync(u => u.UserId == cart.UserId))
            {
                return BadRequest(new { message = "Invalid User ID" });
            }

            // Check if product exists
            if (!await _context.Products.AnyAsync(p => p.ProductId == cart.ProductId && !p.IsRemoved))
            {
                return BadRequest(new { message = "Invalid or removed Product ID" });
            }

            try
            {
                // Preserve original creation date and added date
                cart.Created = existingCart.Created;
                cart.AddedDate = existingCart.AddedDate;
                cart.Modified = DateTime.Now;
                
                _context.Entry(existingCart).CurrentValues.SetValues(cart);
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CartExists(id))
                    return NotFound(new { message = "Cart item not found" });
                else
                    return StatusCode(409, new { message = "Cart item was modified by another user" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the cart item", error = ex.Message });
            }
        }
        
        private async Task<bool> CartExists(int id)
        {
            return await _context.Carts.AnyAsync(e => e.CartId == id);
        }
    }
}
