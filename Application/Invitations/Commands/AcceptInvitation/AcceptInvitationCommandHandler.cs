using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Domain.Shared;
using MediatR;

namespace Application.Invitations.Commands.AcceptInvitation {
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
            var gathering = await _gatheringRepository
                .GetByIdWithCreatorAsync(request.GatheringId, cancellationToken);

            if (gathering is null)
            {
                return Unit.Value;
            }

            var invitation = gathering.Invitations
                .FirstOrDefault(i => i.Id == request.InvitationId);

            if (invitation is null || invitation.Status != InvitationStatus.Pending)
            {
                return Unit.Value;
            }

            Result<Attendee> attendeeResult = gathering.AcceptInvitation(invitation);

            if (attendeeResult.IsSuccess)
            {
                _attendeeRepository.Add(attendeeResult.Value);
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
