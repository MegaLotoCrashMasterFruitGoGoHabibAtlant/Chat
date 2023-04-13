using System.Collections.Generic;
using Chatiks.Tools.Domain;

namespace Chatiks.User.Domain.ValueObjects;

public class UserFullName: ValueObject
{
    public UserNamePart FirstName { get; }
    public UserNamePart LastName { get; }
    
    public UserFullName(UserNamePart firstName, UserNamePart lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public UserFullName(string firstName, string lastName)
    {
        FirstName = new UserNamePart(firstName);
        LastName = new UserNamePart(lastName);
    }
    
    protected UserFullName()
    {
        
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }
}