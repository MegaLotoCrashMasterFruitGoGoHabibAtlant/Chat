using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatiks.Chat.Domain;
using Chatiks.Chat.DomainApi.Interfaces;
using Chatiks.Chat.DomainApi.Specifications;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Handlers.Chat.GetChats;

public class GetChatsQueryHandler: IRequestHandler<GetChatsRequest, ICollection<GetChatsResponse>>
{
    private readonly UserManager<User.Domain.User> _userManager;
    private readonly HttpContextAccessor _contextAccessor;
    private readonly IChatStore _chatStore;
    private readonly IChatMessagesStore _chatMessagesStore;
    
    public GetChatsQueryHandler(UserManager<User.Domain.User> userManager, HttpContextAccessor contextAccessor, IChatStore chatStore, IChatMessagesStore chatMessagesStore)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _chatStore = chatStore;
        _chatMessagesStore = chatMessagesStore;
    }

    public async Task<ICollection<GetChatsResponse>> Handle(GetChatsRequest request, CancellationToken cancellationToken)
    {
        var chatsData = new List<GetChatsResponse>();
        var me = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);

        IQueryable<ChatBase> chatsQuery = _chatStore.Chats;

        if(!string.IsNullOrEmpty(request.NameFilter))
        {
            var publicChatsFilteredQuery = _chatStore.Chats
                    .OfType<PublicChat>()
                    .Where(x => EF.Functions.Like(x.ChatName.ToString(), $"%{request.NameFilter}%"))
                    .Cast<ChatBase>();

            var usersHasChatsWithMeIds = await _chatStore.Chats
                .OfType<PrivateChat>()
                .Where(x => x.OtherUser.Id == me.Id || x.Creator.Id == me.Id)
                .Select(x => x.OtherUser.Id == me.Id ? x.Creator.Id : x.OtherUser.Id)
                .ToArrayAsync(cancellationToken: cancellationToken);

            var idsFilteredByNameFilter = await _userManager.Users
                .Where(x => usersHasChatsWithMeIds.Contains(x.Id) &&
                            EF.Functions.Like(x.FullName.ToString(), $"%{request.NameFilter}%"))
                .Select(x => x.Id)
                .ToArrayAsync(cancellationToken: cancellationToken);
            
            var privateChatsFilteredQuery = _chatStore.Chats
                .OfType<PrivateChat>()
                .Where(x => idsFilteredByNameFilter.Contains(x.OtherUser.Id) || idsFilteredByNameFilter.Contains(x.Creator.Id))
                .Cast<ChatBase>();
            
            chatsQuery = publicChatsFilteredQuery.Union(privateChatsFilteredQuery);
        }

        if (request.OnlyUserChats == true)
        {
            // TODO Implement
        }

        chatsQuery = chatsQuery
            .Include(x => x.Creator)
            .Include(x => x.Messages.OrderByDescending(m => m.SendTime).Take(1));

        var privateChats = await chatsQuery
            .OfType<PrivateChat>()
            .Include(x => x.OtherUser)
            .ToArrayAsync(cancellationToken: cancellationToken);

        var publicChats = await chatsQuery
            .OfType<PublicChat>()
            .Include(x => x.ChatUsers)
            .ToArrayAsync(cancellationToken: cancellationToken);
        
        var chats = privateChats.Cast<ChatBase>().Concat(publicChats.Cast<ChatBase>()).ToArray();
        
        var chatIds = chats.Select(x => x.Id).ToArray();

        foreach (var chat in chats)
        {
            var chatData = new GetChatsResponse();
            chatsData.Add(chatData);

            if (chat is PublicChat publicChat)
            {
                chatData.Name = publicChat.ChatName.ToString();
            }
            
            if (chat is PrivateChat privateChat)
            {
                var other = privateChat.GetChatUsers().FirstOrDefault(c => c.ExternalUserId != me.Id);
                var otherUser = await _userManager.FindByIdAsync(other.ExternalUserId.ToString());
                chatData.Name = otherUser.FullName.ToString();
            }

            var lastMess = chat.Messages.LastOrDefault();
            if (lastMess != null)
            {
                var lastMessageOwner = await _userManager.FindByIdAsync(lastMess.ChatUser.ExternalUserId.ToString());
                chatData.LastMessage = lastMess.Text.ToString();
                chatData.LastMessageSender = lastMessageOwner.FullName.ToString();
                chatData.LastMessageSendTime = lastMess.SendTime.ToShortDateString();
            }
        }

        return chatsData;
    }
}