using Chatiks.Chat.Data.EF;
using Chatiks.Chat.Domain;
using Chatiks.Chat.DomainApi.Interfaces;
using Chatiks.Chat.DomainApi.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Chat.DomainApi;

public class ChatStore : IChatStore
{
    private readonly ChatContext _context;

    public ChatStore(ChatContext context)
    {
        _context = context;
    }

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

        _context.Update(chat);

        await _context.SaveChangesAsync();
        
        return chat;
    }

    public async Task<ICollection<ChatBase>> UpdateChatsAsync(ICollection<ChatBase> chats)
    {
        _context.AttachRange(chats);
        
        _context.UpdateRange(chats);

        await _context.SaveChangesAsync();

        return chats;
    }

    public async Task<bool> IsUserInChatAsync(long externalUserId, long chatId)
    {
        return await _context.Chats.Where(x => x.Id == chatId)
            .AnyAsync(x => x.ChatUsers.Any(y => y.ExternalUserId == externalUserId));
    }

    public async Task<ChatBase> AddChatAsync(ChatBase chat)
    {
        await _context.AddAsync(chat);
        
        await _context.SaveChangesAsync();
        
        return chat;
    }

    public async Task<ICollection<ChatBase>> AddChatsAsync(ICollection<ChatBase> chats)
    {
        await _context.AddRangeAsync(chats);
        
        await _context.SaveChangesAsync();
        
        return chats;
    }
}