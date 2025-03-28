using Application.ViewModels;
using AutoMapper;
using DataAccessObject;
using DataAccessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class UserService : BaseService<User, int, VwUser>, IUserService
{
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="mapper"></param>
    public UserService(IBaseRepository<User, int, VwUser> repository, IMapper mapper) : base(repository)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Login
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public UserViewModel? Login(string email, string password)
    {
        var user = _repository.Find(x => x.Email == email).FirstOrDefault();
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        return new UserViewModel
        {
            Email = user.Email,
            FullName = user.FullName,
            RoleId = user.RoleId
        };
    }
    
    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="registerViewModel"></param>
    /// <returns></returns>
    public async Task<bool> Register(RegisterViewModel registerViewModel)
    {
        return await _repository.ExecuteInTransactionAsync(async () =>
        {
            // Check if user already exists
            var userExist = await _repository
                .Find(x => x.Email == registerViewModel.Email || x.PhoneNumber == registerViewModel.PhoneNumber)
                .FirstOrDefaultAsync();
        
            if (userExist != null)
            {
                return false;
            }
            
            // Add new user
            var newUser = new User
            {
                FullName = registerViewModel.FullName,
                Email = registerViewModel.Email,
                PhoneNumber = registerViewModel.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerViewModel.Password),
                RoleId = (byte) ConstantEnum.Role.Patient,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };
            await _repository.AddAsync(newUser);
            SaveChanges(registerViewModel.Email);
            return true;
        });
    }

}