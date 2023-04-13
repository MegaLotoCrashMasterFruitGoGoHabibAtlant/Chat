using System.Linq;
using Chatiks.Chat.Data.EF;
using Chatiks.Chat.Domain;
using Chatiks.Chat.DomainApi.Interfaces;

namespace Chatiks.Chat.DomainApi;

public class ChatMessagesStore: IChatMessagesStore
{
    private readonly ChatContext _context;
    
    public ChatMessagesStore(ChatContext context)
    {
        _context = context;
    }
    
    public IQueryable<ChatMessage> Messages => _context.ChatMessages;
}