using System.Collections.Generic;
using System.Threading.Tasks;
using Chatiks.Handlers.Chat.GetChats;
using Chatiks.Handlers.Chat.GetMessages;
using Chatiks.Handlers.Users;
using HotChocolate.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Chatiks.Controllers;

// Add graphQL to front and remove it !!

[Route("api/{controller}")]
[Authorize]
public class ChatController: Controller
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("GetChats")]
    public Task<ICollection<GetChatsResponse>> GetChats([FromBody] GetChatsRequest request)
    {
        return _mediator.Send(request);
    }
    
    [HttpPost("GetMessages")]
    public Task<GetChatMessagesResponse> GetMessages([FromBody] GetChatMessagesRequest request)
    {
        return _mediator.Send(request);
    }
    
    [HttpPost("GetUsers")]
    public Task<GetUsersResponse> GetUsers([FromBody] GetUsersRequest request)
    {
        return _mediator.Send(request);
    }
}