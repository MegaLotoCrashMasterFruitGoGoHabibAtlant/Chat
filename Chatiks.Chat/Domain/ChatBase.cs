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
    
    public ChatMessage SendMessage(long externalUserId, string text = null, params long[] externalImagesIds)
    {
        return CreateMessage(externalUserId, null, text, externalImagesIds);
    }
    
    public ChatMessage ReplyMessage(long externalUserId, long repliedMessageId, string text = null, params long[] externalImagesIds)
    {
        return CreateMessage(externalUserId, repliedMessageId, text, externalImagesIds);
    }

    public void DeleteMessage(long messageId, long userDeletesMessageExternalId)
    {
        var message = _messages.FirstOrDefault(x => x.Id == messageId);
        
        if (message == null)
        {
            throw new InvalidOperationException("Message is not in chat. Check that you included it");
        }
        
        var chatUser = ChatUsers.FirstOrDefault(x => x.ExternalUserId == userDeletesMessageExternalId);
        
        if (chatUser == null)
        {
            throw new InvalidOperationException("User is not in chat. Check that you included it");
        }
        
        if (message.ChatUser != chatUser)
        {
            throw new InvalidOperationException("User can't delete message of another user");
        }
        
        _messages.Remove(message);
    }

    private ChatMessage CreateMessage(
        long externalUserId, 
        long? repliedMessageId = null,
        string text = null, 
        params long[] externalImagesIds)
    {
        var chatUser = ChatUsers.FirstOrDefault(x => x.ExternalUserId == externalUserId);
        
        if (chatUser == null)
        {
            throw new InvalidOperationException("User is not in chat. Check that you included it");
        }
        
        var repliedMessage = _messages.FirstOrDefault(x => x.Id == repliedMessageId);
        
        if(repliedMessageId.HasValue && repliedMessage == null)
        {
            throw new InvalidOperationException("Replied message is not in chat. Check that you included it");
        }
        
        var chatMessage = ChatMessage.Create(this, chatUser, repliedMessage, text, externalImagesIds);
        
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