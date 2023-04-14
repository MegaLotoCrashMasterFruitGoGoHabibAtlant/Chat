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
    
    public bool IsChatCreator { get; init; }

    public long ChatId { get; }

    public long ExternalUserId { get; init; }

    public long? InviterId { get; }

    public ChatBase Chat { get; init; }

    public PrivateChat GetPrivateChat() => Chat is PrivateChat privateChat ? privateChat : null;
    
    public PublicChat GetPublicChat() => Chat is PublicChat publicChat ? publicChat : null;

    public ChatUser Inviter { get; init; }

    [BackingField(nameof(_messages))]
    public virtual IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();
    
    [BackingField(nameof(_invitedUsers))]
    public virtual IReadOnlyCollection<ChatUser> InvitedUsers => _invitedUsers.AsReadOnly();

    public void LeaveChat()
    {
        if (Chat is PublicChat publicChat)
        {
            publicChat.LeaveChat(ExternalUserId);
        }
        else
        {
            throw new Exception("Can't leave private chat");
        }
    }
    
    public static ChatUser CreateCreator(ChatBase createdChat, long externalUserId)
    {
        return new()
        {
            ExternalUserId = externalUserId,
            Chat = createdChat,
            IsChatCreator = true
        };
    }
    
    public static ChatUser CreateEnteredUser(ChatBase enterChat, long externalUserId, ChatUser inviter = null)
    {
        return new()
        {
            ExternalUserId = externalUserId,
            Chat = enterChat,
            Inviter = inviter,
            IsChatCreator = false
        };
    }
}