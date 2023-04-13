using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatiks.Chat.Domain;
using Chatiks.Chat.DomainApi.Specifications;

namespace Chatiks.Chat.DomainApi.Interfaces;

public interface IChatStore
{
    IQueryable<ChatBase> Chats { get; }

    Task<ChatBase> GetChatAsync(ChatSpecification specification);
    
    Task<ICollection<ChatBase>> GetChatsAsync(ChatSpecification specification);
    
    Task<ChatBase> UpdateChatAsync(ChatBase chat);
    
    Task<ICollection<ChatBase>> UpdateChatsAsync(ICollection<ChatBase> chats);
    
    Task<bool> IsUserInChatAsync(long externalUserId, long chatId);
}