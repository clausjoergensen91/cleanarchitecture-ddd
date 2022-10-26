using Domain.Entities;

namespace Domain.Repositories {
    public interface IInvitationRepository {
        void Add(Invitation invitation);
    }
}
