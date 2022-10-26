using Domain.Entities;

namespace Domain.Repositories {
    public interface IMemberRepository {
        void Add(object member);
        Task<Member> GetByIdAsync(Guid memberId, CancellationToken cancellationToken);
    }
}
