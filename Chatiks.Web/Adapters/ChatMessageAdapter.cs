using System;
using System.Collections.Generic;
using System.Linq;
using Chatiks.Chat.Domain;
using Chatiks.Core.Domain;

namespace Chatiks.Adapters;

public class ChatMessageAdapter
{
    private readonly ChatMessage _message;
    private readonly User.Domain.User _sender;
    private readonly ICollection<Image> _images;

    public long Id => _message.Id;
    public long ChatId => _message.ChatId;
    public long OwnerId => _message.ChatUser.ExternalUserId;
    public string Text => _message.Text.ToString();
    public string SendTime => _message.SendTime.ToString();
    public string SenderName => _sender?.FullName?.ToString(); 
    public bool IsMe => false;

    public ICollection<ChatImageAdapter> MessageImages => (_message.MessageImageLinks ?? new List<ChatMessageImageLink>())
        .Join((_images ?? Array.Empty<Image>()), cml => cml.ExternalImageId, i => i.Id, (c, i) => i)
        .Select(i => new ChatImageAdapter(i))
        .ToArray();

    public ChatMessageAdapter(ChatMessage message, ICollection<Image> images, User.Domain.User sender)
    {
        _message = message;
        _images = images;
        _sender = sender;
    }
}
