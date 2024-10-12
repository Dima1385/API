using Dashboard.BLL.Services.AccountService;
using Dashboard.BLL.Validators;
using Dashboard.DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace Dashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            IAccountService accountService,
            IWebHostEnvironment hostingEnvironment,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _accountService = accountService;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync([FromBody] SignInVM model)
        {
            var validator = new SignInValidator();
            var validation = await validator.ValidateAsync(model);

            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors);
            }

            var response = await _accountService.SignInAsync(model);
            return GetResult(response);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpVM model)
        {
            SignUpValidator validator = new SignUpValidator();
            var validation = await validator.ValidateAsync(model);

            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors);
            }

            var response = await _accountService.SignUpAsync(model);
            return GetResult(response);
        }

        [HttpGet("emailconfirm")]
        public async Task<IActionResult> EmailConfirmAsync(string u, string t)
        {
            if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(t))
            {
                return NotFound();
            }

            var response = await _accountService.EmailConfirmAsync(u, t);
            return GetResult(response);
        }

        // Метод для зміни зображення користувача
        [HttpPost("user/{userId}/upload-image")]
        public async Task<IActionResult> UploadImage(Guid userId, IFormFile newImage)
        {
            var user = await _accountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Видалення старого зображення, якщо воно існує
            if (!string.IsNullOrEmpty(user.ImagePath))
            {
                var oldImagePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", user.ImagePath);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Збереження нового зображення
            if (newImage != null && newImage.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(newImage.FileName);
                var newPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", newFileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await newImage.CopyToAsync(stream);
                }

                user.ImagePath = newFileName;
                await _accountService.UpdateUserAsync(user);
            }

            return Ok("Image updated successfully");
        }

        // Метод для додавання ролі користувачу
        [HttpPost("user/{userId}/add-role")]
        public async Task<IActionResult> AddRole(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest("Role does not exist");
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok($"Role {role} added to user");
        }

        // Метод для видалення ролі у користувача
        [HttpPost("user/{userId}/remove-role")]
        public async Task<IActionResult> RemoveRole(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return BadRequest("Role does not exist");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok($"Role {role} removed from user");
        }
    }
}
