using Domain.Primitives;
using Domain.ValueObjects;
using MediatR;

namespace Domain.Entities;
public sealed class Member : Entity {

    public Member(Guid id, FirstName firstName, string lastName, string email)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public FirstName FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public static Member Create(string firstName, string lastName, string email)
    {
        var firstNameResult = FirstName.Create(firstName);

        var member = new Member(
            Guid.NewGuid(),
            firstNameResult.Value,
            lastName,
            email);

        return member;
    }
}
