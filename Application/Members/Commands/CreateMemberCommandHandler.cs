using Application.Invitations.Commands.SendInvitation;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;
using MediatR;

namespace Application.Members.Commands;
public sealed class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand> {

	private readonly IMemberRepository _memberRepository;
	private readonly IUnitOfWork _unitOfWork;

	public CreateMemberCommandHandler(
		IMemberRepository memberRepository, 
		IUnitOfWork unitOfWork)
	{
		_memberRepository = memberRepository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Unit> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
	{
        var firstNameResult = FirstName.Create(request.FirstName);
        var lastNameResult = LastName.Create(request.LastName);
        var emailResult = Email.Create(request.Email);

        if (firstNameResult.IsFailure || lastNameResult.IsFailure || emailResult.IsFailure)
        {
            // Log error
            return Unit.Value;
        }

        var member = new Member(
            Guid.NewGuid(),
            firstNameResult.Value,
            request.LastName,
            request.Email);

        _memberRepository.Add(member);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}
