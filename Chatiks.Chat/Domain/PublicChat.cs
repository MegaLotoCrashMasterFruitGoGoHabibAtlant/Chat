using System;
using System.Collections.Generic;
using System.Linq;
using Chatiks.Chat.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Chat.Domain;

public class PublicChat: ChatBase
{
    private List<ChatUser> _chatUsers = new();

    protected PublicChat()
    {
        
    }

    [BackingField(nameof(_chatUsers))]
    public override IReadOnlyCollection<ChatUser> ChatUsers => _chatUsers.AsReadOnly();
    
    public ChatName ChatName { get; private set; }
    
    public void AddChatUser(long externalUserId, ChatUser inviter = null)
    {
        if (_chatUsers.Any(x => x.ExternalUserId == externalUserId))
        {
            throw new InvalidOperationException("User already in chat");
        }
        
        _chatUsers.Add(ChatUser.CreateEnteredUser(this, externalUserId, inviter));
    }
    
    public void AddChatUsers(IEnumerable<long> externalUsersIds, ChatUser inviter = null)
    {
        foreach (var externalUserId in externalUsersIds)
        {
            AddChatUser(externalUserId, inviter);
        }
    }

    public static PublicChat Create(long externalCreatorId, ChatName chatName)
    {
        var publicChat = new PublicChat
        {
            CreationTime = DateTime.Now
        };
        
        publicChat.Creator = ChatUser.CreateCreator(publicChat, externalCreatorId);
        
        publicChat.ChatName = chatName;
        
        publicChat.AddChatUser(externalCreatorId, null);
        
        return publicChat;
    }
    
    public void LeaveChat(long externalUserId)
    {
        var chatUser = _chatUsers.FirstOrDefault(x => x.ExternalUserId == externalUserId);
        
        if (chatUser == null)
        {
            throw new InvalidOperationException("User is not in chat");
        }
        
        _chatUsers.Remove(chatUser);
    }
}