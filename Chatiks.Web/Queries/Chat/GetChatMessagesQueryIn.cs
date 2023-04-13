using Chatiks.Handlers.Interfaces;

namespace Chatiks.Queries.Chat;

public class GetChatMessagesQueryIn: IPaginationRequest
{
    public int Offset { get; set; }
    public int Count { get; set; }
    public long ChatId { get; set; }
}