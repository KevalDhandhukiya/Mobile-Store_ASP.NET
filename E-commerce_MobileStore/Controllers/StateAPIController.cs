using E_commerce_MobileStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_MobileStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateAPIController : ControllerBase
    {
        private readonly MobileStoreManagementContext _context;

        public StateAPIController(MobileStoreManagementContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<State>>> GetAllStates()
        {
            try
            {
                var states = await _context.States
                    .Include(s => s.Country)
                    .OrderBy(s => s.StateName)
                    .ToListAsync();

                return Ok(states);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving states", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<State>> GetStateById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid state ID" });
            }

            try
            {
                var state = await _context.States
                    .Include(s => s.Country)
                    .FirstOrDefaultAsync(s => s.StateId == id);

                if (state == null)
                {
                    return NotFound(new { message = "State not found" });
                }

                return Ok(state);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the state", error = ex.Message });
            }
        }

        // Create (POST)
        [HttpPost]
        public async Task<ActionResult<State>> CreateState([FromBody] State state)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                state.CreateDate = DateTime.Now;
                state.ModifiedDate = DateTime.Now;

                _context.States.Add(state);
                await _context.SaveChangesAsync();

                return Ok(state);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the state", error = ex.Message });
            }
        }

        // Update (PUT)
        [HttpPut("{id}")]
        public async Task<ActionResult<State>> UpdateState(int id, [FromBody] State updatedState)
        {
            if (id != updatedState.StateId)
            {
                return BadRequest(new { message = "State ID mismatch" });
            }

            try
            {
                var existingState = await _context.States.FindAsync(id);
                if (existingState == null)
                    return NotFound("State not found");

                existingState.StateName = updatedState.StateName;
                existingState.StateCode = updatedState.StateCode;
                existingState.CountryId = updatedState.CountryId;
                existingState.FilePath = updatedState.FilePath;
                existingState.ModifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return Ok(existingState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the state", error = ex.Message });
            }
        }

        // Delete (DELETE)
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteState(int id)
        {
            try
            {
                var state = await _context.States.FindAsync(id);
                if (state == null)
                    return NotFound(new { message = "State not found" });

                _context.States.Remove(state);
                await _context.SaveChangesAsync();

                return Ok(new { message = "State deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the state", error = ex.Message });
            }
        }
    }
}
