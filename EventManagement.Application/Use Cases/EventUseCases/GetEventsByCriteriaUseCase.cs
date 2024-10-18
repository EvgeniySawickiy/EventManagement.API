using EventManagement.Core.Entity;
using EventManagement.Core.Interfaces.Repositories;

namespace EventManagement.Application.Use_Cases.EventUseCases
{
    public class GetEventsByCriteriaUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetEventsByCriteriaUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Event> ExecuteAsync(DateTime? date, string location, string category)
        {
            var query = _unitOfWork.Events.Query();

            if (date.HasValue)
            {
                query = query.Where(e => e.EventDate.Date == date.Value.Date);
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(e => e.Location.Contains(location));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.Category.ToLower() == category.ToLower());
            }

            return query.ToList();
        }
    }
}
