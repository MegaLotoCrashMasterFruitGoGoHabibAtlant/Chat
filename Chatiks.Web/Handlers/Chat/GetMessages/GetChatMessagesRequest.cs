using Chatiks.Handlers.Interfaces;

namespace Chatiks.Handlers.Chat.GetMessages;

public class GetChatMessagesRequest : CheckUserInChatQueryInBase<GetChatMessagesResponse>, IPaginationRequest
{

    public int Offset { get; set; }

    public int Count { get; set; }
}