using Dashboard.BLL.Validators;
using Dashboard.DAL.Models.Identity;
using Dashboard.DAL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager) 
        {
            _userManager = userManager;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpVM model)
        {
            SignUpValidator validator = new SignUpValidator();
            var validation = await validator.ValidateAsync(model);

            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors);
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                NormalizedEmail = model.Email.ToUpper(),
                NormalizedUserName = model.UserName.ToUpper()
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if(result.Succeeded)
            {
                return Ok($"Користувач {model.Email} успішно зареєстрований");
            }
            else
            {
                return BadRequest(result.Errors.First().Description);
            }
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignInAsync([FromBody] SignInVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Неправильні вхідні дані");
            }

            var user = await _userManager.FindByEmailAsync(model.Email)
                        ?? await _userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                return BadRequest("Користувач з такими даними не знайдений");
            }

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                return BadRequest("Невірний пароль");
            }

            // Якщо необхідно виконати додаткову логіку, наприклад, додати токен
            return Ok($"Користувач {model.Email} успішно увійшов");
        }

    }
}
