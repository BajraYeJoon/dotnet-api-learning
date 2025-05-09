using Core.DTOs;
using FluentValidation;

namespace Core.Validators
{
  public class UserDtoValidator : AbstractValidator<BaseUserDto>
  {
    public UserDtoValidator()
    {
      RuleFor(x => x.Username)
          .NotEmpty().WithMessage("Username is required")
          .MinimumLength(3).WithMessage("Username must be at least 3 characters");

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
