using Domain.Entities;

namespace Domain.Repositories {
    public interface IAttendeeRepository {
        void Add(Attendee attendee);
    }
}
