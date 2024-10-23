using AutoMapper.Execution;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Application.Use_Cases.ParticipantUseCases
{
    public class RemoveParticipantFromEventUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveParticipantFromEventUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(Guid eventId, Guid userId)
        {
            var participant =  await _unitOfWork.Participants.Query().FirstOrDefaultAsync(p => p.UserId == userId);
            if (participant == null)
            {
                throw new Exception("This member does not exist");
            }
            await _unitOfWork.EventParticipant.DeleteAsync(new EventParticipant()
                {
                    EventId = eventId,
                    ParticipantId = participant.Id
                });
                await _unitOfWork.SaveChangesAsync();

            
        }
    }
}
