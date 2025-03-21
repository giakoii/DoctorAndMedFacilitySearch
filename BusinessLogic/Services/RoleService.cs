using DataAccessObject;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public class RoleService : BaseService<Role, int, VwRole>, IRoleService
{
    public RoleService(IBaseRepository<Role, int, VwRole> repository) : base(repository)
    {
    }
}