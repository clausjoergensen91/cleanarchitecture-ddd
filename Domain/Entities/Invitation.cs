using Domain.Enums;
using Domain.Primitives;

namespace Domain.Entities;
public sealed class Invitation : Entity {

    internal Invitation(Guid id, Member member, Gathering gathering) : base(id)
    {
        MemberId = member.Id;
        GatheringId = gathering.Id;
        Status = InvitationStatus.Pending;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public Guid MemberId { get; private set; }
    public Guid GatheringId { get; private set; }
    public InvitationStatus Status { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }

    internal Attendee Accept()
    {
        Status = InvitationStatus.Accepted;
        ModifiedOnUtc = DateTime.UtcNow;

        var attendee = Attendee.Create(this);

        return attendee;
    }

    internal void Expire()
    {
        Status = InvitationStatus.Expired;
        ModifiedOnUtc = DateTime.UtcNow;
    }
}
