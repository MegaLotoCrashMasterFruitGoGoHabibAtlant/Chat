using System;
using System.Collections.Generic;
using Chatiks.Tools.Domain;

namespace Chatiks.Chat.Domain.ValueObjects;

public class ChatName: ValueObject
{
    public string Value { get; }

    public ChatName(string value)
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
            throw new ArgumentOutOfRangeException(nameof(value), "Chat name cannot be longer than 50 characters");
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}