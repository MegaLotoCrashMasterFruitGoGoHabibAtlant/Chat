using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chatiks.Controllers;
using Chatiks.User.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Chatiks.Handlers.Auth;

public class RegisterQueryHandler: IRequestHandler<RegisterRequest, RegisterResponse>
{
    private readonly SignInManager<User.Domain.User> _signInManager;
    private readonly UserManager<User.Domain.User> _userManager;

    public RegisterQueryHandler(SignInManager<User.Domain.User> signInManager, UserManager<User.Domain.User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var isPhone = LoginController.PhoneDetectRegex.Match(request.MobileOrEmail).Success;

        var user = new User.Domain.User
        {
            FullName = new UserFullName(request.FirstName, request.LastName),
            PhoneNumber = isPhone ? request.MobileOrEmail : null,
            Email = isPhone ? null : request.MobileOrEmail,
            UserName = request.Login
        };
        var res = await _userManager.CreateAsync(user, request.Password);

        if (!res.Succeeded)
        {
            return new RegisterResponse()
            {
                Errors = res.Errors.Select(e => e.Description).ToList()
            };
        }

        return new RegisterResponse();
    }
}