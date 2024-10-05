using Dashboard.DAL.Models.Identity;
using System.Collections.Generic;

namespace Dashboard.BLL.Services.TokenService
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, IList<string> roles);
    }
}
