using MediatR;

namespace Application.Invitations.Commands.AcceptInvitation;
public sealed record AcceptInvitationCommand(Guid InvitationId) : IRequest;
