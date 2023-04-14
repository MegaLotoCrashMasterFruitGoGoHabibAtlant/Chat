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

    public long Id { get; }

    public long ChatId { get; }

    public long ChatUserId { get; }

    public MessageText Text { get; private set; }

    public DateTime? EditTime { get; private set; }
    public DateTime SendTime { get; init; }

    public virtual ChatUser ChatUser { get; init; }

    public virtual ChatBase Chat { get; init; }

    [BackingField(nameof(_imageLinks))]
    public virtual IReadOnlyCollection<ChatMessageImageLink> MessageImageLinks => _imageLinks.AsReadOnly();

    public void Edit(string text, long[] imagesToDelete, long[] imagesToAddExternalIds)
    {
        Text = new MessageText(text);
        EditTime = DateTime.UtcNow;

        _imageLinks = _imageLinks.Where(x => !imagesToDelete.Contains(x.Id))
            .Concat(imagesToAddExternalIds.Select(ChatMessageImageLink.Create)).ToList();
    }

    public static ChatMessage Create(ChatBase chat, ChatUser sender, string text, params long[] externalImagesIds)
    {
        var chatMessage = new ChatMessage
        {
            SendTime = DateTime.Now,
            Text = new MessageText(text),
            ChatUser = sender,
            Chat = chat
        };

        foreach (var externalImageId in externalImagesIds)
        {
            chatMessage._imageLinks.Add(ChatMessageImageLink.Create(externalImageId));
        }

        return chatMessage;
    }
}