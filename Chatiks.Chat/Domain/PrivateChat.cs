namespace Chatiks.Chat.Domain;

public partial class PrivateChat: ChatBase
{
    protected PrivateChat()
    {
        
    }

    public long OtherUserId { get; private set; }

    public ChatUser OtherUser { get; private set; }
    
    public override IReadOnlyCollection<ChatUser> ChatUsers => new List<ChatUser> { Creator, OtherUser }.AsReadOnly();
    
    public static PrivateChat Create(long externalCreatorId, long externalOtherUserId)
    {
        var privateChat = new PrivateChat
        {
            CreationTime = DateTime.Now
        };
        
        privateChat.Creator = ChatUser.CreateCreator(privateChat, externalCreatorId);
        
        privateChat.OtherUser = ChatUser.CreateEnteredUser(privateChat, externalOtherUserId, privateChat.Creator);
        
        return privateChat;
    }
}