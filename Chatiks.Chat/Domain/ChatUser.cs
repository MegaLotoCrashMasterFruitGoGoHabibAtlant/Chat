using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Chat.Domain;

public class ChatUser
{
    protected ChatUser()
    {
    }

    private readonly List<ChatMessage> _messages = new();
    private readonly List<ChatUser> _invitedUsers = new();

    public long Id { get; }

    public long ChatId { get; }

    public long ExternalUserId { get; init; }

    public long? InviterId { get; }

    public ChatBase CreatedChat { get; init; }

    /// <summary>
    /// TODO Разобраться можно ли это смапить в EF
    /// </summary>
    public PrivateChat GetCreatedPrivateChat() => CreatedChat is PrivateChat priv ? priv : null;
    
    public PrivateChat EnteredPrivateChat { get; init; }
    
    public PublicChat GetCreatedPublicChat() => CreatedChat is PublicChat pub ? pub : null;
    
    public PublicChat EnteredPublicChat { get; init; }

    public ChatUser Inviter { get; init; }

    [BackingField(nameof(_messages))]
    public virtual IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();
    
    [BackingField(nameof(_invitedUsers))]
    public virtual IReadOnlyCollection<ChatUser> InvitedUsers => _invitedUsers.AsReadOnly();
    
    public static ChatUser CreateCreator(ChatBase createdChat, long externalUserId)
    {
        if (createdChat.CreatorId != 0)
        {
            throw new InvalidOperationException("Chat already has creator");
        }
        
        return new()
        {
            ExternalUserId = externalUserId,
            CreatedChat = createdChat
        };
    }
    
    public static ChatUser CreateEnteredUser(ChatBase enterChat, long externalUserId, ChatUser inviter = null)
    {
        return new()
        {
            ExternalUserId = externalUserId,
            EnteredPrivateChat = enterChat is PrivateChat priv ? priv : null,
            EnteredPublicChat = enterChat is PublicChat pub ? pub : null,
            Inviter = inviter
        };
    }
}