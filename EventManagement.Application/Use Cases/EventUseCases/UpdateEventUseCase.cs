﻿using EventManagement.Application.Exceptions;
using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;
using EventManagement.Core.Interfaces.Services;


namespace EventManagement.Application.Use_Cases.EventUseCases
{
    public class UpdateEventUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public UpdateEventUseCase(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task ExecuteAsync(Guid eventId, Event eventEntity)
        {
            var existingEvent = await _unitOfWork.Events.GetEventByIdAsync(eventId);
            if (existingEvent == null)
            {
                throw new NotFoundException("Event not found");
            }

            bool isDateChanged = existingEvent.EventDate != eventEntity.EventDate;
            bool isLocationChanged = existingEvent.Location != eventEntity.Location;

            eventEntity.Id = eventId;
            await _unitOfWork.Events.UpdateEventAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();

            if (isDateChanged || isLocationChanged)
            {
                await _notificationService.NotifyParticipantsAsync(eventId, isDateChanged, isLocationChanged);
            }
        }
    }
}