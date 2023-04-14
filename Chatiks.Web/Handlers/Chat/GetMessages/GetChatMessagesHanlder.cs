using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatiks.Adapters;
using Chatiks.Chat.DomainApi.Interfaces;
using Chatiks.Core.DomainApi.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Handlers.Chat.GetMessages;

public class GetChatMessagesHadnlder: CheckUserInChatQueryHandlerBase<GetChatMessagesRequest, GetChatMessagesResponse>
{
    private readonly TypeAdapterConfig _typeAdapterConfig;
    private readonly HttpContextAccessor _contextAccessor;
    private readonly IImagesStore _imagesStore;
    private readonly IChatMessagesStore _chatMessagesStore;
    
    public GetChatMessagesHadnlder(UserManager<User.Domain.User> userManager, HttpContextAccessor contextAccessor, IChatStore chatStore, TypeAdapterConfig typeAdapterConfig, IImagesStore imagesStore, IChatMessagesStore chatMessagesStore) : base(userManager, contextAccessor, chatStore)
    {
        _typeAdapterConfig = typeAdapterConfig;
        _contextAccessor = contextAccessor;
        _imagesStore = imagesStore;
        _chatMessagesStore = chatMessagesStore;
    }

    protected override async Task<GetChatMessagesResponse> InnerHandle(GetChatMessagesRequest request, CancellationToken cancellationToken)
    {
        request.Count = request.Count <= 0 ? int.MaxValue : request.Count;
        
        var me = await UserManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);

        var messagesQuery = _chatMessagesStore.Messages
            .Include(m => m.ChatUser)
            .Include(m => m.MessageImageLinks)
            .Where(m => m.ChatId == request.ChatId)
            .Skip(Math.Max(0, request.Offset))
            .Take(Math.Max(0, request.Count));
        
        var allCount = await messagesQuery.CountAsync(cancellationToken);
        
        var messages = await messagesQuery.ToArrayAsync(cancellationToken);
        
        var sendersIds = messages.Select(m => m.ChatUser.ExternalUserId).ToArray();
        var senders = await UserManager.Users.Where(u => sendersIds.Contains(u.Id)).ToDictionaryAsync(k => k.Id);

        var imagesIds = messages.SelectMany(m => m.MessageImageLinks).Select(l => l.ExternalImageId).ToArray();
        var images = await _imagesStore.GetImagesAsync(imagesIds);

        var messagesData = messages
            .Select(m => new ChatMessageAdapter(m, images, senders[m.ChatUser.ExternalUserId]).Adapt<ChatMessageReponse>())
            .ToArray();

        foreach (var mess in messagesData)
        {
            if (mess.OwnerId == me.Id)
            {
                mess.IsMe = true;
            }
        }
        
        return new GetChatMessagesResponse
        {
            ChatMessages = messagesData,

            EntitiesLeft = Math.Max(0, allCount - request.Offset - request.Count)
        };
    }
}