namespace Chatiks.Chat.Domain;

public class ChatMessageImageLink
{
    protected ChatMessageImageLink()
    {
    }
    
    public long Id { get; }
    
    public long ChatMessageId { get; }
    
    public long ExternalImageId { get; set; }
    
    public virtual ChatMessage ChatMessage { get; }
    
    public static ChatMessageImageLink Create(long externalImageId)
    {
        return new()
        {
            ExternalImageId = externalImageId
        };
    }
}