using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chatiks.Chat.Data.EF;
using Chatiks.Chat.Domain;
using Chatiks.Chat.DomainApi.Interfaces;
using Chatiks.Chat.DomainApi.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Chat.DomainApi;

public class ChatStore : IChatStore
{
    private readonly ChatContext _context;

    public IQueryable<ChatBase> Chats => _context.Chats;

    public Task<ChatBase> GetChatAsync(ChatSpecification specification)
    {
        var chat = _context.Chats.Where(specification).FirstAsync();

        return chat;
    }

    public async Task<ICollection<ChatBase>> GetChatsAsync(ChatSpecification specification)
    {
        var chats = await _context.Chats.Where(specification).ToListAsync();

        return chats;
    }

    public async Task<ChatBase> UpdateChatAsync(ChatBase chat)
    {
        _context.Attach(chat);

        await _context.SaveChangesAsync();
        
        return chat;
    }

    public async Task<ICollection<ChatBase>> UpdateChatsAsync(ICollection<ChatBase> chats)
    {
        _context.AttachRange(chats);

        await _context.SaveChangesAsync();

        return chats;
    }

    public async Task<bool> IsUserInChatAsync(long externalUserId, long chatId)
    {
        return await _context.Chats
            .Where(x => x.Id == chatId)
            .SelectMany(x => x.ChatUsers)
            .AnyAsync(x => x.ExternalUserId == externalUserId);
    }
}