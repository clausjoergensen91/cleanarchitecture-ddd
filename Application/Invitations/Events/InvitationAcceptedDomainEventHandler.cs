using Application.Abstractions;
using Domain.DomainEvents;
using Domain.Repositories;
using MediatR;

namespace Application.Invitations.Events;

internal sealed class InvitationAcceptedDomainEventHandler
    : INotificationHandler<InvitationAcceptedDomainEvent> {

    private readonly IEmailService _emailService;
    private readonly IGatheringRepository _gatheringRepository;

    public async Task Handle(InvitationAcceptedDomainEvent notification, CancellationToken cancellationToken)
    {
        var gathering = await _gatheringRepository.GetByIdWithCreatorAsync(
            notification.GatheringId, cancellationToken);

        if (gathering is null)
        {
            return;
        }

        await _emailService.SendInvitationAcceptedEmailAsync(
            gathering,
            cancellationToken);
    }
}
