using Microsoft.EntityFrameworkCore;

namespace Chatiks.Chat.Domain;

public abstract class ChatBase
{
    private List<ChatMessage> _messages = new();
    
    public long Id { get; }
    
    public DateTime CreationTime { get; init; }
    
    public long CreatorId { get; }
    
    public ChatUser Creator { get; protected set; }

    [BackingField(nameof(_messages))]
    public IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();
    
    public abstract IReadOnlyCollection<ChatUser> ChatUsers { get; }

    protected ChatBase()
    {
        
    }
    
    public ChatMessage SendMessage(long externalUserId, string text, params long[] externalImagesIds)
    {
        var chatUser = ChatUsers.FirstOrDefault(x => x.ExternalUserId == externalUserId);
        
        if (chatUser == null)
        {
            throw new InvalidOperationException("User is not in chat");
        }
        
        var chatMessage = ChatMessage.Create(chatUser, text, externalImagesIds);
        
        _messages.Add(chatMessage);
        
        return chatMessage;
    }
}