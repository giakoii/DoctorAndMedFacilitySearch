using AutoMapper;
using BusinessLogic.ViewModels;
using DataAccessObject;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public class UserService : BaseService<User, int, VwUser>, IUserService
{
    private readonly IRoleService _roleService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="roleService"></param>
    /// <param name="mapper"></param>
    public UserService(IBaseRepository<User, int, VwUser> repository, IRoleService roleService, IMapper mapper) : base(repository)
    {
        _roleService = roleService;
        _mapper = mapper;
    }

    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="registerViewModel"></param>
    /// <returns></returns>
    public bool Register(RegisterViewModel registerViewModel)
    {
        return _repository.ExecuteInTransaction( () =>
        {
            // Check if user already exists
            var userExist = _repository
                .Find(x => x.Email == registerViewModel.Email, false)
                .FirstOrDefault();

            if (userExist != null)
            {
                return false;
            }

            // Get role
            var role =  _roleService.GetById(registerViewModel.RoleId);
            if (role == null)
            {
                return false;
            }

            // Add new user
            var newUser = new User
            {
                FullName = registerViewModel.FullName,
                Email = registerViewModel.Email,
                RoleId = registerViewModel.RoleId,
            };
            _repository.Add(newUser);
            SaveChanges(registerViewModel.Email);
            return true;
        });
    }
}