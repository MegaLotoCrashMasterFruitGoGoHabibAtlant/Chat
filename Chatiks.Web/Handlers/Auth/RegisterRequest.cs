using MediatR;

namespace Chatiks.Handlers.Auth;

public class RegisterRequest: IRequest<RegisterResponse>
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string MobileOrEmail { get; set; }
    
    public string Login { get; set; }

    public string Password { get; set; }
}