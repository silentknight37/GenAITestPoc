using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;
using System.Security.Cryptography;
using System.Text;

namespace MyApi.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;
        public UserService(AppDbContext db) => _db = db;

        public async Task<(bool Ok, int? UserId, string? Error, int Status)> Register(RegisterRequest req)
        {
            var exists = await _db.Users.AnyAsync(u => u.Email == req.Email);
            if (exists) return (false, null, "Email already exists", 409);

            var user = new UserEntity
            {
                Email = req.Email,
                Age = req.Age,
                PasswordHash = Hash(req.Password),
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return (true, user.Id, null, 201);
        }

        public async Task<(bool Ok, string? Error, int Status)> Login(LoginRequest req)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user == null) return (false, "Invalid credentials", 401);

            if (user.PasswordHash != Hash(req.Password))
                return (false, "Invalid credentials", 401);

            return (true, null, 200);
        }

        private static string Hash(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
    }
}
