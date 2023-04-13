using System.Collections.Generic;

namespace Chatiks.Models;

public class GetChatsResponse
{
    public bool IsPrivate { get; set; }
    public string? Name { get; set; }
    public string? LastMessage { get; set; }
    public string? LastMessageSender { get; set; }
    public string? LastMessageSendTime { get; set; }
    public long Id { get; set; }
    public ICollection<ChatUserResponse> ChatUsers { get; set; }
}