using Domain.DomainEvents;
using Domain.Enums;
using Domain.Errors;
using Domain.Exceptions;
using Domain.Primitives;
using Domain.Shared;

namespace Domain.Entities;
public sealed class Gathering : AggregateRoot 
{

    private readonly List<Invitation> _invitations = new();
    private readonly List<Attendee> _attendees = new();

    private Gathering(
        Guid id,
        Member creator,
        GatheringType type,
        DateTime scheduleAtUtc,
        string name,
        string? location)
        : base(id)
    {
        Creator = creator;
        Type = type;
        ScheduleAtUtc = scheduleAtUtc;
        Name = name;
        Location = location;
    }

    public Member Creator { get; private set; }
    public GatheringType Type { get; private set; }
    public string Name { get; private set; }
    public DateTime ScheduleAtUtc { get; private set; }
    public string? Location { get; private set; }
    public int? MaximumNumberOfAttendees { get; private set; }
    public DateTime? InvitationsExpireAtUtc { get; private set; }
    public int NumberOfAttendees { get; private set; }
    public IReadOnlyCollection<Invitation> Invitations => _invitations;
    public IReadOnlyCollection<Attendee> Attendees => _attendees;

    public static Gathering Create(
        Guid id,
        Member creator,
        GatheringType type,
        DateTime scheduleAtUtc,
        string name,
        string? location,
        int? maximumNumberOfAttendees,
        int? invitationsValidBeforeInHours)
    {
        var gathering = new Gathering(
            Guid.NewGuid(),
            creator,
            type,
            scheduleAtUtc,
            name,
            location);

        switch (gathering.Type)
        {
            case GatheringType.WithFixedNumberOfAttendees:
                if (maximumNumberOfAttendees is null)
                {
                    throw new GatheringMaximumNumberOfAttendeesIsNullDomainException(
                        $"{nameof(maximumNumberOfAttendees)} can't be null");
                }

                gathering.MaximumNumberOfAttendees = maximumNumberOfAttendees;
                break;
            case GatheringType.WithExpirationForInvitations:
                if (invitationsValidBeforeInHours is null)
                {
                    throw new GatheringInvitationsValidBeforeInHoursIsNullDomainException(
                        $"{nameof(invitationsValidBeforeInHours)} can't be null");
                }

                gathering.InvitationsExpireAtUtc =
                    gathering.ScheduleAtUtc.AddHours(-invitationsValidBeforeInHours.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(GatheringType));
        }

        return gathering;
    }

    public Result<Invitation> SendInvitations(Member member)
    {
        if (Creator.Id == member.Id)
        {
            return Result.Failure<Invitation>(DomainErrors.Gathering.InvitingCreator);
        }

        if (ScheduleAtUtc < DateTime.UtcNow)
        {
            return Result.Failure<Invitation>(DomainErrors.Gathering.AlreadyPassed);
        }

        var invitation = new Invitation(Guid.NewGuid(), member, this);

        _invitations.Add(invitation);

        return invitation;
    }

    public Result<Attendee> AcceptInvitation(Invitation invitation)
    {
        var reachedMaximumNumberOfAttendees =
            Type == GatheringType.WithFixedNumberOfAttendees &&
            NumberOfAttendees == MaximumNumberOfAttendees;

        var reachedInvitationsExpiration =
            Type == GatheringType.WithExpirationForInvitations &&
            InvitationsExpireAtUtc < DateTime.UtcNow;

        var expired = reachedInvitationsExpiration ||
                      reachedMaximumNumberOfAttendees;

        if (expired)
        {
            invitation.Expire();

            return Result.Failure<Attendee>(DomainErrors.Gathering.Expired);
        }

        var attendee = invitation.Accept();

        RaiseDomainEvent(new InvitationAcceptedDomainEvent(invitation.Id, Id));

        _attendees.Add(attendee);
        NumberOfAttendees++;

        return attendee;
    }
}
