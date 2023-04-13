using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatiks.Chat.Domain;
using Chatiks.Chat.DomainApi.Interfaces;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Handlers.Users;

public class GetUsersHandler: IRequestHandler<GetUsersRequest, GetUsersResponse>
{
    private readonly TypeAdapterConfig _typeAdapterConfig;
    private readonly UserManager<User.Domain.User> _userManager;
    private readonly HttpContextAccessor _contextAccessor;
    private readonly IChatStore _chatStore;

    public GetUsersHandler(TypeAdapterConfig typeAdapterConfig, UserManager<User.Domain.User> userManager, HttpContextAccessor contextAccessor, IChatStore chatStore)
    {
        _typeAdapterConfig = typeAdapterConfig;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _chatStore = chatStore;
    }

    public async Task<GetUsersResponse> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
        var me =  await _userManager.FindByNameAsync(_contextAccessor.HttpContext.User.Identity.Name);
        var usersQuery = _userManager.Users.AsQueryable();

        if (request.IdFilter != null && request.IdFilter.Any())
        {
            usersQuery = usersQuery
                .Where(u => request.IdFilter.Contains(u.Id));
        }

        if (!string.IsNullOrEmpty(request.FullNameFilter))
        {
            usersQuery = usersQuery
                .Where(u => (u.FullName.FirstName + " " + u.FullName.LastName).Contains(request.FullNameFilter) ||
                            (u.FullName.LastName + " " + u.FullName.FirstName).Contains(request.FullNameFilter));
        }
        
        if (!string.IsNullOrEmpty(request.PhoneOrMailFilter))
        {
            usersQuery = usersQuery
                .Where(u => EF.Functions.ILike(u.Email, $"%{request.PhoneOrMailFilter.Trim()}%") ||
                                                                          EF.Functions.ILike(u.PhoneNumber, $"%{request.PhoneOrMailFilter.Trim()}%"));
        }

        if (request.ExcludeMe == true)
        {
            usersQuery = usersQuery.Where(u => u.UserName != _contextAccessor.HttpContext.User.Identity.Name);
        }
        
        if (request.ExcludeHasChatsWithMe == true)
        {
            var excludeUsers = await _chatStore.Chats
                .OfType<PrivateChat>()
                .Where(x => x.OtherUser.Id == me.Id || x.Creator.Id == me.Id)
                .Select(x => x.OtherUser.Id == me.Id ? x.Creator.Id : x.OtherUser.Id)
                .ToArrayAsync(cancellationToken: cancellationToken);

            usersQuery = usersQuery
                .Where(u => !excludeUsers.Contains(u.Id));
        }

        if (request.AdditionalIds != null && request.AdditionalIds.Any())
        {
            usersQuery = usersQuery
                .Union(_userManager.Users.Where(u => request.AdditionalIds.Contains(u.Id)));
        }

        var allCount = await usersQuery.CountAsync();
        
        var users = await usersQuery
            .Skip(request.Offset)
            .Take(request.Count)
            .ProjectToType<UserResponse>(_typeAdapterConfig)
            .ToArrayAsync();

        return new GetUsersResponse
        {
            EntitiesLeft = Math.Max(allCount - request.Offset - users.Length, 0),
            
            Users = users
        };
    }
}