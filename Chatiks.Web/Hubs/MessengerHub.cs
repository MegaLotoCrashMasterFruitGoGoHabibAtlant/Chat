using Chatiks.Chat.Domain;
using Chatiks.Chat.Domain.ValueObjects;
using Chatiks.Chat.DomainApi.Interfaces;
using Chatiks.Core.DomainApi.Interfaces;
using Chatiks.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Hubs.Models.Chat;

namespace Chatiks.Hubs;

[Authorize]
[Route("/hub/messengerHub")]
public class MessengerHub : Hub
{
    private static Dictionary<long, string> _userConnections = new Dictionary<long, string>();

    private readonly UserManager<User.Domain.User> _userManager;
    private readonly IChatStore _chatStore;
    private readonly IImagesStore _imagesStore;


    public MessengerHub(UserManager<User.Domain.User> userManager, IChatStore chatStore, IImagesStore imagesStore)
    {
        _userManager = userManager;
        _chatStore = chatStore;
        _imagesStore = imagesStore;
    }

    [HubMethodName("sendMessageToChat")]
    public async Task SendMessageToChat(SendMessageToChatRequest request)
    {
        var response = new SendMessageToChatResponse();

        var user = await _userManager.FindByNameAsync(Context.User.Identity.Name);

        var chat = await _chatStore.Chats
            .Include(x => x.ChatUsers)
            .FirstAsync(x => x.Id == request.ChatId);

        var images = await _imagesStore.GetOrCreateImagesAsync(request.ImagesBase64 ?? new string[0]);

        chat.SendMessage(user.Id, request.Text, images.Select(x => x.Id).ToArray());

        await _chatStore.UpdateChatAsync(chat);

        response.MessageId = chat.Messages.Last().Id;

        response.Text = request.Text;
        response.ChatId = request.ChatId;
        response.SendTime = DateTime.Now;
        response.SenderName = user.FullName.ToString();
        response.Images = images.Select(i => new SendMessageToChatImageResponse
        {
            Base64String = i.Base64String
        }).ToArray();

        // Refactor this !!!
        response.IsMe = true;

        await Clients.Caller.SendCoreAsync("messageSendEvent", new[]
        {
            response
        });

        var connections = chat.ChatUsers.Select(x => x.ExternalUserId)
            .Where(u => u != user.Id)
            .Select(u => _userConnections.GetValueOrDefault(u))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        response.IsMe = false;

        var clientsFromChat = Clients.Clients(connections);

        await clientsFromChat.SendCoreAsync("messageSendEvent", new[]
        {
            response
        });
    }

    [HubMethodName("сreateChat")]
    public async Task CreateChat([FromBody] CreateChatRequest request)
    {
        var response = new CreateChatResponse();

        var user = await _userManager.FindByNameAsync(Context.User.Identity.Name);
        var otherIds = request.UsersIds.Except(new[] { user.Id }).ToArray();

        var newChat = PublicChat.Create(user.Id, new ChatName(request.Name));
        newChat.AddChatUsers(otherIds);

        await _chatStore.AddChatAsync(newChat);

        response.ChatId = newChat.Id;

        var allChatUsersIds = otherIds.Concat(new[] { user.Id }).ToArray();

        var allChatUsers = await _userManager.Users
            .Where(u => allChatUsersIds.Contains(u.Id))
            .ToArrayAsync();

        var connections = allChatUsersIds
            .Select(u => _userConnections.GetValueOrDefault(u))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        var clientsFromChat = Clients.Clients(connections);

        response.Name = request.Name;
        response.ChatUsers = allChatUsers.Select(u => u.Adapt<CreateChatChatUserResponse>()).ToArray();

        await clientsFromChat.SendCoreAsync("chatCreateEvent", new[]
        {
            response
        });
    }

    [HubMethodName("сreatePrivateChat")]
    public async Task CreatePrivateChat(long userId)
    {
        var response = new CreateChatResponse();

        var user = await _userManager.FindByNameAsync(Context.User.Identity.Name);
        var other = await _userManager.FindByIdAsync(userId.ToString());

        var newChat = PrivateChat.Create(user.Id, other.Id);
        
        await _chatStore.AddChatAsync(newChat);
        
        response.ChatId = newChat.Id;

        response.ChatUsers = new[] { user, other }.Select(u => u.Adapt<CreateChatChatUserResponse>()).ToArray();

        response.Name = other.FullName.ToString();
        
        await Clients.Caller.SendCoreAsync("chatCreateEvent", new[]
        {
            response
        });

        if (_userConnections.ContainsKey(other.Id))
        {
            response.Name = user.FullName.ToString();
            await Clients.Client(_userConnections[other.Id]).SendCoreAsync("chatCreateEvent", new[]
            {
                response
            });
        }
    }

    [HubMethodName("addUserToChat")]
    public async Task AddUserToChat(AddUserToChatRequest request)
    {
        var inviter = await _userManager.FindByNameAsync(Context.User.Identity.Name);
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        var response = new AddUserToChatResponse();
        response.ChatId = request.ChatId;
        response.FirstName = user.FullName.FirstName.ToString();
        response.LastName = user.FullName.LastName.ToString();

        var chat = await _chatStore.Chats
            .Include(x => x.ChatUsers)
            .FirstAsync(x => x.Id == request.ChatId);
        
        ((PublicChat)chat).AddChatUser(request.UserId);
        
        await _chatStore.UpdateChatAsync(chat);

        var connections = chat.ChatUsers.Select(u => u.ExternalUserId)
            .Select(u => _userConnections.GetValueOrDefault(u))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        var clientsFromChat = Clients.Clients(connections);

        await clientsFromChat.SendCoreAsync("addUserEvent", new[]
        {
            response
        });
    }

    [HubMethodName("leaveChat")]
    public async Task LeaveChat(long chatId)
    {
        var me = await _userManager.FindByNameAsync(Context.User.Identity.Name);
        var response = new LeaveChatResponse()
        {
            ChatId = chatId,
            LeavedUserName = me.FullName.ToString()
        };

        var chat = await _chatStore.Chats
            .Include(x => x.ChatUsers)
            .FirstAsync(x => x.Id == chatId);

        ((PublicChat)chat).LeaveChat(me.Id);
        
        await _chatStore.UpdateChatAsync(chat);

        var connections = chat.ChatUsers.Select(u => u.ExternalUserId)
            .Select(u => _userConnections.GetValueOrDefault(u))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        var clientsFromChat = Clients.Clients(connections);

        await clientsFromChat.SendCoreAsync("leaveChatEvent", new[]
        {
            response
        });
    }

    public override async Task OnConnectedAsync()
    {
        var user = await _userManager.FindByNameAsync(Context.User.Identity.Name);

        _userConnections[user.Id] = Context.ConnectionId;

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await _userManager.FindByNameAsync(Context.User.Identity.Name);

        _userConnections.Remove(user.Id);

        await base.OnDisconnectedAsync(exception);
    }
}