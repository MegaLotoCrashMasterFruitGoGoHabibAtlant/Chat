using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Chat.Domain;

public abstract class ChatBase
{
    private List<ChatMessage> _messages = new();
    private List<ChatUser> _chatUsers = new();
    
    protected ChatBase()
    {
        CreationTime = DateTime.Now;
    }
    
    public long Id { get; }
    
    public DateTime CreationTime { get; init; }
    

    [BackingField(nameof(_messages))]
    public IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();

    [BackingField(nameof(_chatUsers))]
    public IReadOnlyCollection<ChatUser> ChatUsers => _chatUsers.AsReadOnly();
    
    public ChatUser GetCreator() => ChatUsers.First(x => x.IsChatCreator);
    
    public ChatMessage SendMessage(long externalUserId, string text, params long[] externalImagesIds)
    {
        var chatUser = ChatUsers.FirstOrDefault(x => x.ExternalUserId == externalUserId);
        
        if (chatUser == null)
        {
            throw new InvalidOperationException("User is not in chat. Check that you included it");
        }
        
        var chatMessage = ChatMessage.Create(this, chatUser, text, externalImagesIds);
        
        _messages.Add(chatMessage);
        
        return chatMessage;
    }
    
    protected void AddUser(ChatUser chatUser)
    {
        _chatUsers.Add(chatUser);
    }
    
    protected void RemoveUser(ChatUser chatUser)
    {
        _chatUsers.Remove(chatUser);
    }
}