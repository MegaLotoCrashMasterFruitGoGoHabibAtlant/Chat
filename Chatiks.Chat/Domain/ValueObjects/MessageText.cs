using System;
using System.Collections.Generic;
using Chatiks.Tools.Domain;

namespace Chatiks.Chat.Domain.ValueObjects;

public class MessageText: ValueObject
{
    public string Value { get; }
    
    public MessageText(string value)
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
        
        if (value.Length > 500)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Message text cannot be longer than 500 characters");
        }
    }

    public override string ToString()
    {
        return Value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}