using MediatR;

namespace Application.Invitations.Commands.AcceptInvitation;

public sealed record AcceptInvitationCommand(Guid GatheringId, Guid InvitationId) : IRequest;
