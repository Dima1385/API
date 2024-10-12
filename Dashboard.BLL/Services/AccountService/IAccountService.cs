using Dashboard.DAL.Models.Identity;
using Dashboard.DAL.ViewModels;

namespace Dashboard.BLL.Services.AccountService
{
    public interface IAccountService
    {
        Task<ServiceResponse> SignInAsync(SignInVM model);
        Task<ServiceResponse> SignUpAsync(SignUpVM model);
        Task<ServiceResponse> EmailConfirmAsync(string id, string token);

        Task<User> GetUserByIdAsync(Guid userId);
        Task<bool> UpdateUserAsync(User user);
    }
}
