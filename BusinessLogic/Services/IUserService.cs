using BusinessLogic.ViewModels;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public interface IUserService : IBaseService<User, int, VwUser>
{
    bool Register(RegisterViewModel registerViewModel);
}