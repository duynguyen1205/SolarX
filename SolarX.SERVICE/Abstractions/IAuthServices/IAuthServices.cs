using SolarX.SERVICE.Services.AuthServices;
using SolarX.SERVICE.Services.Base;

namespace SolarX.SERVICE.Abstractions.IAuthServices;

public interface IAuthServices
{
    Task<Result<string>> Login(RequestModel.LoginRequest request);
    Task<Result> AdminCreateAccount(RequestModel.RegisterRequest request);
    string CreatAccount(string password);
}