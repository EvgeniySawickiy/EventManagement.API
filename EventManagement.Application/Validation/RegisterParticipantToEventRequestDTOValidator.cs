using EventManagement.Application.DTO.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Application.Validation
{
    public class RegisterParticipantToEventRequestDTOValidator : AbstractValidator<RegisterParticipantToEventRequestDTO>
    {
        public RegisterParticipantToEventRequestDTOValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("EventId is required.");
        }
    }
}