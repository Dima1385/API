using Dashboard.DAL.ViewModels;
using System.Threading.Tasks;

namespace Dashboard.BLL.Services.RoleService
{
    public interface IRoleService
    {
        Task<ServiceResponse> GetAllRolesAsync();
        Task<ServiceResponse> GetRoleByIdAsync(string id);
        Task<ServiceResponse> CreateRoleAsync(RoleVM roleVM);
        Task<ServiceResponse> UpdateRoleAsync(RoleVM roleVM);
        Task<ServiceResponse> DeleteRoleAsync(string id);
    }
}
