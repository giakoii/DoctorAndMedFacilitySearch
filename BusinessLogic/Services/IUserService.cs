using Application.ViewModels;
using DataAccessObject.Models;

namespace BusinessLogic.Services;

public interface IUserService : IBaseService<User, int, VwUser>
{
    
    UserViewModel? Login(string email, string password);
    
    Task<bool> Register(RegisterViewModel registerViewModel);
}