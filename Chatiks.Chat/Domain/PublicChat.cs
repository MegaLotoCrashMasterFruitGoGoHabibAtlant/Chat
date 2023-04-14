using System;
using System.Collections.Generic;
using System.Linq;
using Chatiks.Chat.Domain.ValueObjects;

namespace Chatiks.Chat.Domain;

public class PublicChat : ChatBase
{
    protected PublicChat()
    {
    }

    public ChatName ChatName { get; private set; }

    public void AddChatUser(long externalUserId, ChatUser inviter = null)
    {
        if (ChatUsers.Any(x => x.ExternalUserId == externalUserId))
        {
            throw new InvalidOperationException("User already in chat");
        }

        AddUser(ChatUser.CreateEnteredUser(this, externalUserId, inviter));
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

        var creator = ChatUser.CreateCreator(publicChat, externalCreatorId);

        publicChat.ChatName = chatName;

        publicChat.AddUser(creator);

        return publicChat;
    }

    public void LeaveChat(long externalUserId)
    {
        var chatUser = ChatUsers.FirstOrDefault(x => x.ExternalUserId == externalUserId);

        if (chatUser == null)
        {
            throw new InvalidOperationException("User is not in chat");
        }

        RemoveUser(chatUser);
    }
}