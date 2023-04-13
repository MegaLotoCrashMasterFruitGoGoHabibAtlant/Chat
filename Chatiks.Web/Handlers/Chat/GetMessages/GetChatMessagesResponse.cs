using System.Collections.Generic;
using Chatiks.Handlers.Interfaces;

namespace Chatiks.Handlers.Chat.GetMessages;

public class GetChatMessagesResponse : IPaginationResponse
{
    public ICollection<ChatMessageReponse> ChatMessages { get; set; }
    public long EntitiesLeft { get; set; }
}
