using E_commerce_MobileStore.Models;
using E_commerce_MobileStore.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_MobileStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly MobileStoreManagementContext _context;
        private readonly IValidator<Product> _validator;

        public ProductAPIController(MobileStoreManagementContext context, IValidator<Product> validator)
        {
            _context = context;
            _validator = validator;
        }

        // ✅ Get all products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => !p.IsRemoved)
                    .Include(p => p.Category)
                    .Include(p => p.User)
                    .OrderBy(p => p.ModelName)
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving products", error = ex.Message });
            }
        }

        // ✅ Get product by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid product ID" });

            try
            {
                var product = await _context.Products
                    .Include(p => p.User)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(c => c.ProductId == id);

                if (product == null)
                    return NotFound(new { message = "Product not found" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the product", error = ex.Message });
            }
        }

        // ✅ Delete or Soft Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid product ID" });

            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return NotFound(new { message = "Product not found" });

                var hasCartItems = await _context.Carts.AnyAsync(c => c.ProductId == id);
                if (hasCartItems)
                {
                    product.IsRemoved = true;
                    product.Modified = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return NoContent();
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the product", error = ex.Message });
            }
        }

        // ✅ Insert product
        [HttpPost]
        public async Task<IActionResult> InsertProduct([FromBody] Product product)
        {
            var validationResult = await _validator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage })
                });
            }

            if (!await _context.Users.AnyAsync(u => u.UserId == product.UserId))
                return BadRequest(new { message = "Invalid User ID" });

            if (!await _context.Categories.AnyAsync(c => c.CategoryId == product.CategoryId))
                return BadRequest(new { message = "Invalid Category ID" });

            if (await _context.Products.AnyAsync(p => p.ModelName == product.ModelName && p.CompanyName == product.CompanyName && !p.IsRemoved))
                return Conflict(new { message = "Product with same model name and company already exists" });

            try
            {
                product.ProductId = 0;
                product.Created = DateTime.Now;
                product.Modified = DateTime.Now;
                product.IsRemoved = false;

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the product", error = ex.Message });
            }
        }

        // ✅ Update product (fixed mapping issue)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid product ID" });

            if (id != product.ProductId)
                return BadRequest(new { message = "Product ID mismatch" });

            var validationResult = await _validator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage })
                });
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
                return NotFound(new { message = "Product not found" });

            if (!await _context.Users.AnyAsync(u => u.UserId == product.UserId))
                return BadRequest(new { message = "Invalid User ID" });

            if (!await _context.Categories.AnyAsync(c => c.CategoryId == product.CategoryId))
                return BadRequest(new { message = "Invalid Category ID" });

            if (await _context.Products.AnyAsync(p =>
                    p.ModelName == product.ModelName &&
                    p.CompanyName == product.CompanyName &&
                    p.ProductId != id &&
                    !p.IsRemoved))
            {
                return Conflict(new { message = "Product with same model name and company already exists" });
            }

            try
            {
                // 🔹 Safely update scalar properties only
                existingProduct.ModelName = product.ModelName;
                existingProduct.CompanyName = product.CompanyName;
                existingProduct.LaunchDate = product.LaunchDate;
                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Stock = product.Stock;
                existingProduct.Image1 = product.Image1;
                existingProduct.Image2 = product.Image2;
                existingProduct.Image3 = product.Image3;
                existingProduct.Image4 = product.Image4;
                existingProduct.SpecificationId = product.SpecificationId;
                existingProduct.UserId = product.UserId;

                // Preserve Created date
                existingProduct.Modified = DateTime.Now;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(409, new { message = "Product was modified by another user" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the product", error = ex.Message });
            }
        }
    }
}
