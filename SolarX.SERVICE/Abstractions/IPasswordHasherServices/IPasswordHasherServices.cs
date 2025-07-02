namespace SolarX.SERVICE.Abstractions.IPasswordHasherServices;

public interface IPasswordHasherServices
{
    public string HashPassword(string password);
    
    public bool VerifyPassword(string password, string hashedPassword);
}