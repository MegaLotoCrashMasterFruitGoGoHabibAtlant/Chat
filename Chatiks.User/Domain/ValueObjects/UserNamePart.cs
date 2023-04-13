using System;
using System.Collections.Generic;
using System.Linq;
using Chatiks.Tools.Domain;

namespace Chatiks.User.Domain.ValueObjects;

public class UserNamePart: ValueObject
{
    public UserNamePart(string value)
    {
        Validate(value);

        Value = value;
    }
    
    private void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(nameof(value));
        }
        
        if (value.Length > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "User name cannot be longer than 50 characters");
        }
        
        if (value.Any(char.IsDigit))
        {
            throw new ArgumentException("User name cannot contain digits", nameof(value));
        }
    }
    
    public string Value { get; init; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value;
    }
}