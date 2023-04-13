using System;
using System.Threading;
using System.Threading.Tasks;
using Chatiks.Chat.DomainApi.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Chatiks.Handlers.Chat;

public abstract class CheckUserInChatQueryHandlerBase<TRequest, TOut>: IRequestHandler<TRequest, TOut>
    where TRequest: CheckUserInChatQueryInBase<TOut>
    where TOut: class
{
    protected readonly UserManager<User.Domain.User> UserManager;
    protected readonly HttpContextAccessor ContextAccessor;
    protected readonly IChatStore ChatStore;


    protected CheckUserInChatQueryHandlerBase(UserManager<User.Domain.User> userManager, HttpContextAccessor contextAccessor, IChatStore chatStore)
    {
        UserManager = userManager;
        ContextAccessor = contextAccessor;
        ChatStore = chatStore;
    }

    public async Task<TOut> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var user = await UserManager.FindByNameAsync(ContextAccessor.HttpContext.User.Identity.Name);
        
        if (!await ChatStore.IsUserInChatAsync(user.Id, request.ChatId))
        {
            throw new Exception("user not belogs to chat");
        }

        return await InnerHandle(request, cancellationToken);
    }
    
    protected abstract Task<TOut> InnerHandle(TRequest request, CancellationToken cancellationToken);
}