using System.Collections.Generic;

namespace Chatiks.Handlers.Auth;

public class RegisterResponse
{
    public ICollection<string> Errors { get; set; }
}