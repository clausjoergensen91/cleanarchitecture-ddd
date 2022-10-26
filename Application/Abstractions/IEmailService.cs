using Domain.Entities;

namespace Application.Abstractions;
public interface IEmailService {
    Task SendInvitationAcceptedEmailAsync(Gathering gathering, CancellationToken cancellationToken);
    Task SendInvitationSentEmailAsync(Member member, Gathering gathering, CancellationToken cancellationToken);
}
