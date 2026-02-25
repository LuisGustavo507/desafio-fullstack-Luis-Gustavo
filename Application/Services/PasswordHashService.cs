using Application.Interfaces;
using BCrypt.Net;

namespace Application.Services
{
    public class PasswordHashService : IPasswordHashService
    {
        public string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }

        public bool VerifyPassword(string password, string hash)
        {
            bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
            return isValid;
        }
    }
}