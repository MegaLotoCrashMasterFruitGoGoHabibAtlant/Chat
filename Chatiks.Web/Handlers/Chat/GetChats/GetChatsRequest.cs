using System.Collections.Generic;
using MediatR;

namespace Chatiks.Handlers.Chat.GetChats;

public class GetChatsRequest: IRequest<ICollection<GetChatsResponse>>
{
    public string NameFilter { get; set; }
    public bool? OnlyUserChats { get; set; }
}