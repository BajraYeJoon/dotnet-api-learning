using Core.DTOs;
using FluentValidation;

namespace Core.Validators
{
  public class UserDtoValidator : AbstractValidator<BaseUserDto>
  {
    public UserDtoValidator()
    {
      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email is required")
          .EmailAddress().WithMessage("Invalid email address");

      RuleFor(x => x.Password)
          .NotEmpty().WithMessage("Password is required")
          .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
  }

  public class SignUpDtoValidator : AbstractValidator<SignUpDto>
  {
    public SignUpDtoValidator()
    {
      Include(new UserDtoValidator());
    }
  }

  public class LoginDtoValidator : AbstractValidator<LoginDto>
  {
    public LoginDtoValidator()
    {
      Include(new UserDtoValidator());
    }
  }
}
