using Domain.Entities;

namespace Domain.Repositories {
    public interface IMemberRepository {
        Task<Member> GetByIdAsync(Guid memberId, CancellationToken cancellationToken);
    }
}
