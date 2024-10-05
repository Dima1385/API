using System.Collections.Generic;
using System.Threading.Tasks;
using Dashboard.DAL.Models.Identity;

namespace Dashboard.DAL.Repositories.RoleRepository
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> GetRoleByIdAsync(string id);
        Task CreateRoleAsync(Role role);
        Task UpdateRoleAsync(Role role);
        Task DeleteRoleAsync(Role role);
    }
}
