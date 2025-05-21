using Core.DTOs;
using FluentValidation;

namespace Core.Validators
{
    public class CreateBlockDtoValidator : AbstractValidator<CreateBlockDto>
    {
        public CreateBlockDtoValidator()
        {
            RuleFor(x => x.BlockName)
                .NotEmpty().WithMessage("Block Name is required.")
                .Length(2, 50).WithMessage("Block name must be between 2 and 50 characters")

                .Must(name => name.Trim() == name).WithMessage("Block name must not start or end with spaces")
                ;

            RuleFor(x => x.PropertyType)
                .IsInEnum()
                .WithMessage("PropertyType must be a valid value (Apartment, Housing, or Mixed).");

            RuleFor(x => x.ManagerId)
               .Must(x => x is null || x != Guid.Empty)
               .WithMessage("Must be a valid manager Id");
        }
    }

    public class AssignManagerValidator : AbstractValidator<AssignManagerDto>
    {
        public AssignManagerValidator()
        {
            // Manager ID must be a valid non-empty GUID
            RuleFor(x => x.ManagerId)
                .NotEmpty().WithMessage("Manager ID is required")
                .Must(x => x != Guid.Empty)
                .WithMessage("Manager ID must be a valid ID");
        }
    }

}