using MediatR;

namespace Chatiks.Handlers.Chat;

public class CheckUserInChatQueryInBase<TOut>: IRequest<TOut> where TOut: class
{
    public long ChatId { get; set; }
}