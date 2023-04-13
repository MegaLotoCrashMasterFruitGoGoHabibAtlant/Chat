using System;
using System.Linq;
using System.Threading.Tasks;
using Chatiks.Chat.Domain;
using Chatiks.Chat.Domain.ValueObjects;
using Chatiks.Chat.DomainApi.Interfaces;
using Chatiks.Chat.DomainApi.Specifications;
using Chatiks.Core.DomainApi.Interfaces;
using Chatiks.Models;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Chatiks.Queries.ChatMutation;

[ExtendObjectType("Mutation")]
public class ChatMutations
{
    [Authorize]
    public class ChatMutationsData
    {
        public async Task<CreateChatResponse> CreateNewChatWithOneUser(
            [Service] HttpContextAccessor contextAccessor,
            [Service] UserManager<User.Domain.User> userManager,
            [Service] IChatStore chatStore,
            string name)
        {
            var user = await userManager.FindByNameAsync(contextAccessor.HttpContext.User.Identity.Name);

            var newChat = PublicChat.Create(user.Id, new ChatName(name));
            await chatStore.UpdateChatAsync(newChat);
            var id = newChat.Id;

            return new CreateChatResponse()
            {
                ChatId = id,
                ChatUsers = new[] { user }.Select(u => u.Adapt<CreateChatChatUserResponse>()).ToArray()
            };
        }
        
        public async Task<CreateChatResponse> CreatePrivateChat(
            [Service] HttpContextAccessor contextAccessor,
            [Service] UserManager<User.Domain.User> userManager,
            [Service] IChatStore chatStore,
            long otherUserId)
        {
            var user = await userManager.FindByNameAsync(contextAccessor.HttpContext.User.Identity.Name);
            var other = await userManager.FindByIdAsync(otherUserId.ToString());

            var newChat = PrivateChat.Create(user.Id, other.Id);
            await chatStore.UpdateChatAsync(newChat);
            var id = newChat.Id;

            return new CreateChatResponse()
            {
                ChatId = id,
                ChatUsers = new[] { user, other }.Select(u => u.Adapt<CreateChatChatUserResponse>()).ToArray()
            };
        }
        
        public async Task<SendMessageToChatResponse> SendMessageToChat(
            [Service] HttpContextAccessor contextAccessor,
            [Service] UserManager<User.Domain.User> userManager,
            [Service] IChatStore chatStore,
            long chatId,
            string text)
        {
            var user = await userManager.FindByNameAsync(contextAccessor.HttpContext.User.Identity.Name);
            
            var chat = await chatStore.Chats.FirstAsync(c => c.Id == chatId);

            var message = chat.SendMessage(user.Id, text);
            
            await chatStore.UpdateChatAsync(chat);
            
            return new SendMessageToChatResponse()
            {
                ChatId = chatId,
                Text = text,
                IsMe = true,
                MessageId = message.Id,
                SenderName = user.FullName.ToString(),
                SendTime = DateTime.Now
            };
        }
        
        public async Task AddUserToChat(
            [Service] HttpContextAccessor contextAccessor,
            [Service] UserManager<User.Domain.User> userManager,
            [Service] IChatStore chatStore,
            long chatId,
            long userId)
        {
            var inviter = await userManager.FindByNameAsync(contextAccessor.HttpContext.User.Identity.Name);
            var user = await userManager.FindByIdAsync(userId.ToString());

            var chat = await chatStore.Chats.FirstAsync(c => c.Id == chatId);

            if (chat is PrivateChat)
            {
                throw new Exception("chat is private");
            }
            
            ((PublicChat)chat).AddChatUser(user.Id);
        }
    }

    public ChatMutationsData Chat => new ChatMutationsData();
}

