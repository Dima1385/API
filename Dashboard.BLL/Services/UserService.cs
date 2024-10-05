using Dashboard.DAL;
using Dashboard.DAL.Models.Identity;
using Dashboard.DAL.Repositories.UserRepository;
using Dashboard.DAL.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Dashboard.BLL.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public UserService(IUserRepository userRepository, UserManager<User> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<ServiceResponse> UpdateAsync(UserUpdateVM model)
        {
            var user = await _userRepository.GetByIdAsync(model.Id) as User;

            if (user == null)
            {
                return ServiceResponse.BadRequestResponse("Користувача не знайдено");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any() && model.Role != roles.First())
            {
                await _userManager.RemoveFromRoleAsync(user, roles.First());
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return ServiceResponse.BadRequestResponse(result.Errors.First().Description);
            }

            return ServiceResponse.OkResponse("Дані користувача успішно оновлено");
        }

        public async Task<ServiceResponse> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id) as User;
            if (user == null)
            {
                return ServiceResponse.BadRequestResponse("Користувача не знайдено");
            }

            var userVM = new UserVM
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                Role = user.UserRoles.Any() ? user.UserRoles.First().Role.Name : Settings.UserRole
            };

            return ServiceResponse.OkResponse("Користувача знайдено", userVM);
        }

        public async Task<ServiceResponse> DeleteAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id) as User;

            if (user == null)
            {
                return ServiceResponse.BadRequestResponse("Користувача не знайдено");
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return ServiceResponse.BadRequestResponse(result.Errors.First().Description);
            }

            return ServiceResponse.OkResponse("Користувача успішно видалено");
        }

        public async Task<ServiceResponse> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userList = users.Select(user => new UserVM
            {
                Id = (user as User).Id,
                Email = (user as User).Email,
                FirstName = (user as User).FirstName,
                LastName = (user as User).LastName,
                PhoneNumber = (user as User).PhoneNumber,
                UserName = (user as User).UserName,
                Role = (user as User).UserRoles.Any() ? (user as User).UserRoles.First().Role.Name : Settings.UserRole
            }).ToList();

            return ServiceResponse.OkResponse("Список користувачів", userList);
        }
    }
}
