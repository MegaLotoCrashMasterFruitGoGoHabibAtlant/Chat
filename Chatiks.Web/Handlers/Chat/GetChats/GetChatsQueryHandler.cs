using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatiks.Chat.DomainApi.Interfaces;
using Chatiks.Chat.DomainApi.Specifications;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Chatiks.Handlers.Chat.GetChats;

public class GetChatsQueryHandler: IRequestHandler<GetChatsRequest, ICollection<GetChatsResponse>>
{
    private readonly UserManager<User.Domain.User> _userManager;
    private readonly HttpContextAccessor _contextAccessor;
    private readonly IChatStore _chatStore;
    
    public GetChatsQueryHandler(UserManager<User.Domain.User> userManager, HttpContextAccessor contextAccessor, IChatStore chatStore)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _chatStore = chatStore;
    }

    public async Task<ICollection<GetChatsResponse>> Handle(GetChatsRequest request, CancellationToken cancellationToken)
    {
        var chatsData = new List<GetChatsResponse>();
        var me = await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);

        var spec = ChatSpecification.Empty()
            .WithNameFilter(request.NameFilter);

        spec = request.OnlyUserChats == true
            ? spec.HavingUser(me.Id)
            : spec;
        
        var chats = await _chatStore.GetChatsAsync(spec);

        // foreach (var chat in chats)
        // {
        //     var chatData = chat.Adapt<GetChatsResponse>();
        //     chatsData.Add(chatData);
        //     
        //     if (string.IsNullOrEmpty(chat))
        //     {
        //         var other = chat.ChatUsers.FirstOrDefault(c => c.ExternalUserId != me.Id);
        //         if (other != null)
        //         {
        //             var otherUser = await _userManager.FindByIdAsync(other.ExternalUserId.ToString());
        //             chatData.Name = otherUser.FullName;
        //         }
        //         else
        //         {
        //             chatData.Name = me.FullName;
        //         }
        //     }
        //     
        //     var lastMess = chat.Messages.LastOrDefault();
        //     if (lastMess != null)
        //     {
        //         var lastMessageOwner = await _userManager.FindByIdAsync(lastMess.ExternalOwnerId.ToString());
        //         chatData.LastMessage = lastMess.Text;
        //         chatData.LastMessageSender = lastMessageOwner.FullName;
        //         chatData.LastMessageSendTime = lastMess.SendTime.ToShortDateString();
        //     }
        // }

        return chatsData;
    }
}