using Dashboard.BLL.Services.AccountService;
using Dashboard.BLL.Services.MailService;
using Dashboard.BLL.Validators;
using Dashboard.DAL.Models.Identity; 
using Dashboard.DAL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        private readonly UserManager<User> _userManager; 
        private readonly IMailService _mailService; 

        public AccountController(IAccountService accountService, UserManager<User> userManager, IMailService mailService)
        {
            _accountService = accountService;
            _userManager = userManager;
            _mailService = mailService;
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { token }, Request.Scheme);

            await _mailService.SendEmailAsync(model.Email, "Reset Password",
                $"Please reset your password by clicking <a href='{callbackUrl}'>here</a>.");

            return Ok("Password reset link has been sent to your email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("User not found");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest("Error while resetting the password");

            return Ok("Password has been reset successfully.");
        }
    }
}
