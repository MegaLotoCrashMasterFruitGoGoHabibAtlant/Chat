using System.Collections.Generic;
using System.Threading.Tasks;
using Chatiks.Handlers.Chat.GetChats;
using Chatiks.Handlers.Chat.GetMessages;
using HotChocolate;
using HotChocolate.Types;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Chatiks.Queries.Chat;

[ExtendObjectType("Query")]
public class ChatQueries
{
    [Authorize]
    public class ChatQueriesData
    {
        public async Task<ICollection<GetChatsResponse>> GetChats(
            [Service] IMediator mediator,
            GetChatsQueryIn request) => await mediator.Send(request.Adapt<GetChatsRequest>());

        public async Task<GetChatMessagesResponse> GetChatMessages(
            [Service] IMediator mediator,
            GetChatMessagesQueryIn request) => await mediator.Send(request.Adapt<GetChatMessagesRequest>());
    }

    public ChatQueriesData Chat => new ChatQueriesData();
}

