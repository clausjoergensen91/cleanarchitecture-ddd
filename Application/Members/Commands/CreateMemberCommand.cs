using MediatR;

namespace Application.Members.Commands;
public sealed record CreateMemberCommand(
    string Email, 
    string FirstName, 
    string LastName) : IRequest;
