using E_commerce_MobileStore.Models;
using E_commerce_MobileStore.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace E_commerce_MobileStore.Controllers
{
    [Route("order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly MobileStoreManagementContext _context;
        private readonly IValidator<Order> _validator;
        
        public OrderAPIController(MobileStoreManagementContext context, IValidator<Order> validator)
        {
            _context = context;
            _validator = validator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.User)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving orders", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid order ID" });
            }

            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(c => c.OrderId == id);

                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the order", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid order ID" });
            }

            try
            {
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound(new { message = "Order not found" });
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the order", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertOrder([FromBody] Order order)
        {
            // FluentValidation
            var validationResult = await _validator.ValidateAsync(order);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Validation failed", errors = validationResult.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage }) });
            }

            // Check if user exists
            if (!await _context.Users.AnyAsync(u => u.UserId == order.UserId))
            {
                return BadRequest(new { message = "Invalid User ID" });
            }

            try
            {
                // Reset OrderId to 0 to ensure it's auto-generated
                order.OrderId = 0;
                order.Created = DateTime.Now;
                order.Modified = DateTime.Now;
                order.OrderDate = DateTime.Now;
                
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest(new { message = "Order ID mismatch" });
            }

            // FluentValidation
            var validationResult = await _validator.ValidateAsync(order);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { message = "Validation failed", errors = validationResult.Errors.Select(x => new { field = x.PropertyName, error = x.ErrorMessage }) });
            }

            // Check if order exists
            var existingOrder = await _context.Orders.FindAsync(id);
            if (existingOrder == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            // Check if user exists
            if (!await _context.Users.AnyAsync(u => u.UserId == order.UserId))
            {
                return BadRequest(new { message = "Invalid User ID" });
            }

            try
            {
                // Preserve original creation date and order date
                order.Created = existingOrder.Created;
                order.OrderDate = existingOrder.OrderDate;
                order.Modified = DateTime.Now;
                
                _context.Entry(existingOrder).CurrentValues.SetValues(order);
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrderExists(id))
                    return NotFound(new { message = "Order not found" });
                else
                    return StatusCode(409, new { message = "Order was modified by another user" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the order", error = ex.Message });
            }
        }
        
        private async Task<bool> OrderExists(int id)
        {
            return await _context.Orders.AnyAsync(e => e.OrderId == id);
        }
    }
}
