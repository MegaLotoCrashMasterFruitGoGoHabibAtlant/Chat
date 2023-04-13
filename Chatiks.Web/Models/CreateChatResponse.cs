namespace Chatiks.Models;

public class CreateChatResponse
{
    public bool IsPrivate { get; set; }
    public long ChatId { get; set; }
    public string Name { get; set; }
    public CreateChatChatUserResponse[] ChatUsers { get; set; }
}

public class CreateChatChatUserResponse
{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}