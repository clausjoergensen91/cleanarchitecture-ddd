using Domain.Entities;

namespace Domain.Repositories {
    public interface IGatheringRepository {
        Task<Gathering> GetByIdWithCreatorAsync(Guid gatheringId, CancellationToken cancellationToken);
    }
}
