using Application.Abstractions;
using Domain.Enums;
using Domain.Repositories;
using MediatR;

namespace Application.Invitations.Commands.AcceptInvitation
{
    internal class AcceptInvitationCommandHandler : IRequestHandler<AcceptInvitationCommand> {

        private readonly IMemberRepository _memberRepository;
        private readonly IGatheringRepository _gatheringRepository;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IAttendeeRepository _attendeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public AcceptInvitationCommandHandler(
            IMemberRepository memberRepository,
            IGatheringRepository gatheringRepository,
            IInvitationRepository invitationRepository,
            IAttendeeRepository attendeeRepository,
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _memberRepository = memberRepository;
            _gatheringRepository = gatheringRepository;
            _invitationRepository = invitationRepository;
            _attendeeRepository = attendeeRepository;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
        {
            var invitation = await _invitationRepository
                .GetByIdAsync(request.InvitationId, cancellationToken);

            if (invitation is null || invitation.Status != InvitationStatus.Pending)
            {
                return Unit.Value;
            }

            var member = await _memberRepository.GetByIdAsync(invitation.MemberId, cancellationToken);

            var gathering = await _gatheringRepository
                .GetByIdWithCreatorAsync(invitation.GatheringId, cancellationToken);

            if (member is null || gathering is null)
            {
                return Unit.Value;
            }

            var attendee = gathering.AcceptInvitation(invitation);

            if (attendee is not null)
            {
                _attendeeRepository.Add(attendee);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (invitation.Status == InvitationStatus.Accepted)
            {
                await _emailService.SendInvitationAcceptedEmailAsync(gathering, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
