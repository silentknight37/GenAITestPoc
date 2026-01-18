using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MyApi.Models;
using MyApi.Services;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _svc;
        private readonly IValidator<RegisterRequest> _validator;

        public UsersController(UserService svc, IValidator<RegisterRequest> validator)
        {
            _svc = svc;
            _validator = validator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var validation = await _validator.ValidateAsync(req);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var result = await _svc.Register(req);
            if (!result.Ok) return StatusCode(result.Status, result.Error);

            return StatusCode(201, new { userId = result.UserId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var result = await _svc.Login(req);
            if (!result.Ok) return StatusCode(result.Status, result.Error);
            return Ok(new { token = "demo-token" });
        }
    }
}
