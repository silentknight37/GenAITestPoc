using FluentValidation;
using MyApi.Models;
namespace MyApi.Validation
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Age).GreaterThanOrEqualTo(18);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter")
                .Matches("[0-9]").WithMessage("Password must contain a number");
        }
    }
}
