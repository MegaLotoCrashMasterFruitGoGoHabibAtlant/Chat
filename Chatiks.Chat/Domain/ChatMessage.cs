using System;
using System.Collections.Generic;
using System.Linq;
using Chatiks.Chat.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Chat.Domain;

public class ChatMessage
{
    protected ChatMessage()
    {
    }

    private List<ChatMessageImageLink> _imageLinks = new();
    
    private List<ChatMessage> _replies = new();

    public long Id { get; }

    public long ChatId { get; }

    public long ChatUserId { get; }
    
    public long? RepliedMessageId { get; }

    public MessageText? Text { get; private set; }

    public DateTime? EditTime { get; private set; }
    public DateTime SendTime { get; init; }

    public virtual ChatUser ChatUser { get; init; }

    public virtual ChatBase Chat { get; init; }
    
    public virtual ChatMessage RepliedMessage { get; init; }

    [BackingField(nameof(_imageLinks))]
    public virtual IReadOnlyCollection<ChatMessageImageLink> MessageImageLinks => _imageLinks.AsReadOnly();
    
    [BackingField(nameof(_replies))]
    public virtual IReadOnlyCollection<ChatMessage> Replies => _replies.AsReadOnly();

    public void Edit(string text, long[] imagesToDelete, long[] imagesToAddExternalIds)
    {
        Text = new MessageText(text);
        EditTime = DateTime.UtcNow;

        _imageLinks = _imageLinks.Where(x => !imagesToDelete.Contains(x.Id))
            .Concat(imagesToAddExternalIds.Select(ChatMessageImageLink.Create)).ToList();
    }

    public static ChatMessage Create(
        ChatBase chat,
        ChatUser sender,
        ChatMessage repliedMessage = null,
        string text = null,
        params long[] externalImagesIds)
    {
        if (chat == null)
        {
            throw new ArgumentNullException(nameof(chat));
        }
        
        if (sender == null)
        {
            throw new ArgumentNullException(nameof(sender));
        }
        
        if (sender.Chat != chat)
        {
            throw new InvalidOperationException("ChatUser is not in chat");
        }
        
        if (repliedMessage != null && repliedMessage.Chat != chat)
        {
            throw new InvalidOperationException("RepliedMessage is not in chat");
        }

        var chatMessage = new ChatMessage
        {
            SendTime = DateTime.Now,
            Text = string.IsNullOrEmpty(text) ? null : new MessageText(text),
            ChatUser = sender,
            Chat = chat,
            RepliedMessage = repliedMessage
        };

        foreach (var externalImageId in externalImagesIds)
        {
            chatMessage._imageLinks.Add(ChatMessageImageLink.Create(externalImageId));
        }

        return chatMessage;
    }
}