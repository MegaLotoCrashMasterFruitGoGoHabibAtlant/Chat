using System.Linq;
using Chatiks.Chat.Domain;

namespace Chatiks.Chat.DomainApi.Interfaces;

public interface IChatMessagesStore
{
    IQueryable<ChatMessage> Messages { get; }
}