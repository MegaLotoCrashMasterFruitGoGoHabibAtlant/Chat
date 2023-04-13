using System;
using System.Collections.Generic;

namespace Chatiks.Models;

public class SendMessageToChatResponse
{
    public bool IsMe { get; set; }
    public long MessageId { get; set; }
    public long ChatId { get; set; }
    public string SenderName  { get; set; }
    public string Text { get; set; }
    public DateTime SendTime  { get; set; }
    public ICollection<SendMessageToChatImageResponse> Images  { get; set; }
}

public class SendMessageToChatImageResponse
{
    public string Base64String { get; set; }
}