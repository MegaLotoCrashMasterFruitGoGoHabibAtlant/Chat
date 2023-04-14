using System.Linq;

namespace Chatiks.Chat.Domain;

public class PrivateChat : ChatBase
{
    protected PrivateChat()
    {
    }

    public ChatUser GetOtherUser()
    {
        return ChatUsers.First(x => !x.IsChatCreator);
    }

    public static PrivateChat Create(long externalCreatorId, long externalOtherUserId)
    {
        var privateChat = new PrivateChat();

        var creator = ChatUser.CreateCreator(privateChat, externalCreatorId);
        var otherUser = ChatUser.CreateEnteredUser(privateChat, externalOtherUserId, creator);

        privateChat.AddUser(otherUser);
        privateChat.AddUser(creator);

        return privateChat;
    }
}