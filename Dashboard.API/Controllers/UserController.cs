using Dashboard.DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        // Метод для видалення користувача:
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Користувача не знайдено");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok($"Користувач {user.Email} успішно видалений");
            }

            return BadRequest(result.Errors.First().Description);
        }

        // Метод для редагування користувача:
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAsync(string id, [FromBody] User updatedUser)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("Користувача не знайдено");
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.UserName = updatedUser.UserName;
            user.NormalizedEmail = updatedUser.Email.ToUpper();
            user.NormalizedUserName = updatedUser.UserName.ToUpper();

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok($"Користувач {user.Email} успішно оновлений");
            }

            return BadRequest(result.Errors.First().Description);
        }

        // Метод для отримання всіх ролей:
        [HttpGet("AllRoles")]
        public async Task<IActionResult> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }
    }
}
