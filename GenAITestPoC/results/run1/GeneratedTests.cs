using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;
using MyApi.Services;
using Xunit;

namespace MyApi.Tests.Generated
{
    public class UserServiceTests
    {
        private readonly AppDbContext _context;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(options);
            _userService = new UserService(_context);
        }

        [Fact]
        public async Task Register_UserDoesNotExist_ShouldReturnSuccess()
        {
            var request = new RegisterRequest("test@example.com", "Password1", 25);
            var result = await _userService.Register(request);

            Assert.True(result.Ok);
            Assert.NotNull(result.UserId);
            Assert.Null(result.Error);
            Assert.Equal(201, result.Status);
        }

        [Fact]
        public async Task Register_UserAlreadyExists_ShouldReturnConflict()
        {
            var request = new RegisterRequest("test@example.com", "Password1", 25);
            await _userService.Register(request); // First registration

            var result = await _userService.Register(request); // Trying to register again

            Assert.False(result.Ok);
            Assert.Equal("Email already exists", result.Error);
            Assert.Equal(409, result.Status);
        }

        [Fact]
        public async Task Login_CredentialsAreValid_ShouldReturnSuccess()
        {
            var registerRequest = new RegisterRequest("test@example.com", "Password1", 25);
            await _userService.Register(registerRequest); // Register user before login

            var loginRequest = new LoginRequest("test@example.com", "Password1");
            var result = await _userService.Login(loginRequest);

            Assert.True(result.Ok);
            Assert.Null(result.Error);
            Assert.Equal(200, result.Status);
        }

        [Fact]
        public async Task Login_CredentialsAreInvalid_ShouldReturnUnauthorized()
        {
            var registerRequest = new RegisterRequest("test@example.com", "Password1", 25);
            await _userService.Register(registerRequest); // Register user before failing login

            var loginRequest = new LoginRequest("test@example.com", "WrongPassword");
            var result = await _userService.Login(loginRequest);

            Assert.False(result.Ok);
            Assert.Equal("Invalid credentials", result.Error);
            Assert.Equal(401, result.Status);
        }

        [Fact]
        public async Task Login_UserDoesNotExist_ShouldReturnUnauthorized()
        {
            var loginRequest = new LoginRequest("nonexistent@example.com", "Password1");
            var result = await _userService.Login(loginRequest);

            Assert.False(result.Ok);
            Assert.Equal("Invalid credentials", result.Error);
            Assert.Equal(401, result.Status);
        }
    }
}
