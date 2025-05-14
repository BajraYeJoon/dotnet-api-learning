using Core.DTOs;
using FluentValidation;

namespace Core.Validators
{
    public class CreateManagerDtoValidator : AbstractValidator<CreateManagerDto>
    {
        public CreateManagerDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50)
                .WithMessage("Username must be between 3 and 50 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email address");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(50)
                .WithMessage("Password must be between 8 and 50 characters");
        }
    }
}