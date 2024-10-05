using Dashboard.DAL.ViewModels;
using System.Threading.Tasks;

namespace Dashboard.BLL.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse> UpdateAsync(UserUpdateVM model);
        Task<ServiceResponse> GetByIdAsync(string id);
        Task<ServiceResponse> DeleteAsync(string id);
        Task<ServiceResponse> GetAllAsync();
    }
}
