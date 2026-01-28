using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_MobileStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // This will require an authenticated user for all actions inside this controller
    public class AuthorizeController : ControllerBase
    {
        [HttpGet("GetAuthorizedData")]
        public IActionResult GetAuthorizedData()
        {
            // This will require an authenticated user (no role restriction)
            return Ok(new { message = "Welcome Authenticated User!" });
        }

        [HttpGet("GetAdminData")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminData()
        {
            // Override with specific roles
            return Ok(new { message = "This is admin area, or admin required." });
        }

        [HttpGet("GetPublicData")] 
        [AllowAnonymous]
        public IActionResult GetPublicData()
        {
            return Ok(new { message = "This is public area, no login required." });
        }
    }
}
