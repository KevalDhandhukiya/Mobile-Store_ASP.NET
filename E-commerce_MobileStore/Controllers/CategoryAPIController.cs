using E_commerce_MobileStore.Models;
using E_commerce_MobileStore.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace E_commerce_MobileStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryAPIController : ControllerBase
    {
        private readonly MobileStoreManagementContext _context;
        private readonly IValidator<Category> _validator;
        
        public CategoryAPIController(MobileStoreManagementContext context, IValidator<Category> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.User)
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync();
                
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving categories", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid category ID" });
            }

            try
            {
                var category = await _context.Categories
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.CategoryId == id);

                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the category", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid category ID" });
            }

            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new { message = "Category not found" });
                }

                // Check if category has related products
                var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id);
                if (hasProducts)
                {
                    return BadRequest(new { message = "Cannot delete category with existing products" });
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the category", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertCategory([FromBody] Category category)
        {
            // FluentValidation
            var validationResult = await _validator.ValidateAsync(category);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Validation failed", errors = validationResult.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage }) });
            }

            // Check if user exists
            if (!await _context.Users.AnyAsync(u => u.UserId == category.UserId))
            {
                return BadRequest(new { message = "Invalid User ID" });
            }

            // Check for duplicate category name for the same user
            if (await _context.Categories.AnyAsync(c => c.CategoryName == category.CategoryName && c.UserId == category.UserId))
            {
                return Conflict(new { message = "Category name already exists for this user" });
            }

            try
            {
                // Reset CategoryId to 0 to ensure it's auto-generated
                category.CategoryId = 0;
                category.Created = DateTime.Now;
                category.Modified = DateTime.Now;
                
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId }, category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the category", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest(new { message = "Category ID mismatch" });
            }

            // FluentValidation
            var validationResult = await _validator.ValidateAsync(category);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Validation failed", errors = validationResult.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage }) });
            }

            // Check if category exists
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            // Check if user exists
            if (!await _context.Users.AnyAsync(u => u.UserId == category.UserId))
            {
                return BadRequest(new { message = "Invalid User ID" });
            }

            // Check for duplicate category name for the same user (excluding current category)
            if (await _context.Categories.AnyAsync(c => c.CategoryName == category.CategoryName && c.UserId == category.UserId && c.CategoryId != id))
            {
                return Conflict(new { message = "Category name already exists for this user" });
            }

            try
            {
                // Preserve original creation date
                category.Created = existingCategory.Created;
                category.Modified = DateTime.Now;
                
                _context.Entry(existingCategory).CurrentValues.SetValues(category);
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CategoryExists(id))
                    return NotFound(new { message = "Category not found" });
                else
                    return StatusCode(409, new { message = "Category was modified by another user" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the category", error = ex.Message });
            }
        }
        
        private async Task<bool> CategoryExists(int id)
        {
            return await _context.Categories.AnyAsync(e => e.CategoryId == id);
        }
    }
}
