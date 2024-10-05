using Dashboard.DAL.Models.Identity;
using Dashboard.DAL.Repositories.RoleRepository;
using Dashboard.DAL.ViewModels;


namespace Dashboard.BLL.Services.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public RoleService(IMapper mapper, IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<ServiceResponse> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();    
            var rolesVM = _mapper.Map<IEnumerable<RoleVM>>(roles);
            return ServiceResponse.OkResponse(rolesVM);
        }

        public async Task<ServiceResponse> GetRoleByIdAsync(string id)
        {
            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
            {
                return ServiceResponse.BadRequestResponse("Роль не знайдена");
            }

            var roleVM = _mapper.Map<RoleVM>(role);
            return ServiceResponse.OkResponse(roleVM);
        }

        public async Task<ServiceResponse> CreateRoleAsync(RoleVM roleVM)
        {
            var role = _mapper.Map<Role>(roleVM);
            await _roleRepository.CreateRoleAsync(role);
            return ServiceResponse.OkResponse("Роль успішно створена");
        }

        public async Task<ServiceResponse> UpdateRoleAsync(RoleVM roleVM)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleVM.Id);
            if (role == null)
            {
                return ServiceResponse.BadRequestResponse("Роль не знайдена");
            }

            _mapper.Map(roleVM, role);
            await _roleRepository.UpdateRoleAsync(role);
            return ServiceResponse.OkResponse("Роль успішно оновлена");
        }

        public async Task<ServiceResponse> DeleteRoleAsync(string id)
        {
            var role = await _roleRepository.GetRoleByIdAsync(id);
            if (role == null)
            {
                return ServiceResponse.BadRequestResponse("Роль не знайдена");
            }

            await _roleRepository.DeleteRoleAsync(role);
            return ServiceResponse.OkResponse("Роль успішно видалена");
        }
    }
}
