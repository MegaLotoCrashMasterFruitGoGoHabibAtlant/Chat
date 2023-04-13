using Chatiks.Handlers.Interfaces;
using MediatR;

namespace Chatiks.Handlers.Users;

public class GetUsersRequest: IPaginationRequest, IRequest<GetUsersResponse>
{
    public long[] AdditionalIds { get; set; }
    public bool? ExcludeMe { get; set; }
    public bool? ExcludeHasChatsWithMe { get; set; }
    public string? FullNameFilter { get; set; }
    public string? PhoneOrMailFilter { get; set; }
    public long[]? IdFilter { get; set; }
    public int Offset { get; set; }
    public int Count { get; set; }
}