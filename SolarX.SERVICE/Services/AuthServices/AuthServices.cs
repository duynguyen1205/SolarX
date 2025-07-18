using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY.Abstractions;
using SolarX.REPOSITORY.Entity;
using SolarX.REPOSITORY.Enum;
using SolarX.SERVICE.Abstractions.IAuthServices;
using SolarX.SERVICE.Abstractions.IJwtServices;
using SolarX.SERVICE.Abstractions.IPasswordHasherServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Services.AuthServices;

public class AuthServices : IAuthServices
{
    private readonly IBaseRepository<User, Guid> _userRepository;
    private readonly IJwtServices _jwtServices;
    private readonly IPasswordHasherServices _passwordHasherServices;

    public AuthServices(IBaseRepository<User, Guid> userRepository, IPasswordHasherServices passwordHasherServices,
        IJwtServices jwtServices)
    {
        _userRepository = userRepository;
        _passwordHasherServices = passwordHasherServices;
        _jwtServices = jwtServices;
    }

    public async Task<Result<string>> Login(RequestModel.LoginRequest request)
    {
        var isUserExist = await _userRepository.GetAllWithQuery(x => x.Email == request.Email)
            .Include(x => x.Agency)
            .FirstOrDefaultAsync();
        if (isUserExist == null)
        {
            return Result<string>.CreateResult("UserName doesn't exist", 400, "");
        }

        var isPasswordValid = _passwordHasherServices.VerifyPassword(request.Password, isUserExist.Password);
        if (!isPasswordValid)
        {
            return Result<string>.CreateResult("Invalid password", 400, "Invalid password");
        }

        var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        var claims = new List<Claim>
        {
            new("UserId", isUserExist.Id.ToString()),
            new("Role", isUserExist.Role.ToString()),
            new("Name", isUserExist.FullName),
            new("Slug", isUserExist.Agency.Slug),
            new("AgencyId", isUserExist.AgencyId.ToString()),
            new(ClaimTypes.Role, isUserExist.Role.ToString()),
            new(ClaimTypes.Expired,
                TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow.AddMinutes(120), vietnamTimeZone).ToString())
        };
        var token = _jwtServices.GenerateAccessToken(claims);
        return Result<string>.CreateResult("Login Successfully", 200, token);
    }


    public async Task<Result> AdminCreateAccount(RequestModel.RegisterRequest request, bool isAgencyAdmin)
    {
        var isUserExist = await _userRepository.GetAllWithQuery(x => x.Email == request.Email)
            .FirstOrDefaultAsync();

        if (isUserExist != null)
        {

            if (!isUserExist.IsDeleted)
            {
                return Result.CreateResult("Email already exist", 400);
            }


            var hashPassword = _passwordHasherServices.HashPassword(request.Password);
            isUserExist.PhoneNumber = request.PhoneNumber;
            isUserExist.FullName = request.FirstName;
            isUserExist.Password = hashPassword;
            isUserExist.Role = isAgencyAdmin ? Role.AgencyAdmin : Role.SystemStaff;
            isUserExist.IsDeleted = false;
            isUserExist.AgencyId = request.AgencyId;

            _userRepository.UpdateEntity(isUserExist);
            return Result.CreateResult("Account recreated successfully", 201);
        }


        var newHashPassword = _passwordHasherServices.HashPassword(request.Password);
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            FullName = request.LastName,
            Password = newHashPassword,
            Role = isAgencyAdmin ? Role.AgencyAdmin : Role.SystemStaff,
            AgencyId = request.AgencyId
        };
        _userRepository.AddEntity(newUser);
        return Result.CreateResult("Create user successfully", 201);
    }

    public async Task<Result> AgencyAdminCreateAccount(Guid agencyId,RequestModel.RegisterAgencyRequest request)
    {
        var isUserExist = await _userRepository.GetAllWithQuery(x => x.Email == request.Email)
            .FirstOrDefaultAsync();

        if (isUserExist != null)
        {

            if (!isUserExist.IsDeleted)
            {
                return Result.CreateResult("Email already exist", 400);
            }


            var hashPassword = _passwordHasherServices.HashPassword(request.Password);
            isUserExist.PhoneNumber = request.PhoneNumber;
            isUserExist.FullName = request.FirstName;
            isUserExist.Password = hashPassword;
            isUserExist.Role = Role.AgencyStaff;
            isUserExist.IsDeleted = false;
            isUserExist.AgencyId = agencyId;

            _userRepository.UpdateEntity(isUserExist);
            return Result.CreateResult("Account recreated successfully", 201);
        }


        var newHashPassword = _passwordHasherServices.HashPassword(request.Password);
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            FullName = request.LastName,
            Password = newHashPassword,
            Role = Role.AgencyStaff,
            AgencyId = agencyId
        };
        _userRepository.AddEntity(newUser);
        return Result.CreateResult("Create user successfully", 201);
    }

    public string CreatAccount(string password)
    {
        var hashPassword = _passwordHasherServices.HashPassword(password);
        return hashPassword;
    }
}