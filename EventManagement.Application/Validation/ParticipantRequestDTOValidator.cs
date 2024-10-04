using EventManagement.Application.DTO.Request;
using FluentValidation;


public class ParticipantRequestDTOValidator : AbstractValidator<ParticipantRequestDTO>
{
    public ParticipantRequestDTOValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Now).WithMessage("Birth date must be in the past.");
    }
}