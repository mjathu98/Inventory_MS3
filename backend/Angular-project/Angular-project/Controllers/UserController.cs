using Angular_project.context;
using Angular_project.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Angular_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;

        public UserController(AppDbContext appDbContext)
        {
            this._authContext = appDbContext;
        }

        // Authenticate the user
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserLogin userLogin)
        {
            if (userLogin == null)
            {
                return BadRequest(new { Message = "Invalid request" });
            }

            var user = await _authContext.Users
                .FirstOrDefaultAsync(x => x.UserName == userLogin.UserName);

            if (user == null)
            {
                return NotFound(new { Message = "User Not Found!" });
            }

            // Compare plain text passwords
            if (user.Password != userLogin.Password)
            {
                return Unauthorized(new { Message = "Invalid password!" });
            }

            return Ok(new { Message = "Login Success!" });
        }

        // Register the user
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegister userRegister)
        {
            if (userRegister == null)
            {
                return BadRequest(new { Message = "Invalid request" });
            }

            // Check if the username already exists
            var existingUser = await _authContext.Users
                .FirstOrDefaultAsync(x => x.UserName == userRegister.UserName);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "Username already exists!" });
            }

            // Create the user entity
            var user = new User
            {
                FirstName = userRegister.FirstName,
                LastName = userRegister.LastName,
                Email = userRegister.Email,
                UserName = userRegister.UserName,
                Role = "User", // Default role, you can modify this based on your requirements
                Password = userRegister.Password // Directly assign plain text password
            };

            // Add the user to the database
            await _authContext.Users.AddAsync(user);
            await _authContext.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully!" });
        }


    }
}

