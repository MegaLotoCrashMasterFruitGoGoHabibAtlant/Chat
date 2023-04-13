using System.Linq;
using Chatiks.Chat.Domain;

namespace Chatiks.Chat.DomainApi.Interfaces;

public interface IChatUsersStore
{
    IQueryable<ChatUser> ChatUsers { get; }
}