using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;

namespace EventManagement.Application.Use_Cases.ParticipantUseCases
{
    public class GetParticipantByIdUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetParticipantByIdUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Participant> ExecuteAsync(Guid id)
        {
            return await _unitOfWork.Participants.GetParticipantByIdAsync(id);
        }
    }
}
