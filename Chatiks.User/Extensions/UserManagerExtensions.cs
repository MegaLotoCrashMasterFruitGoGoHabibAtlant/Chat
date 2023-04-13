using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Chatiks.User.Extensions;

public static class UserManagerExtensions
{
    public static async Task ThrowIfNoExist(this UserManager<Domain.User> userManager, long id)
    {
        (await userManager.FindByIdAsync(id.ToString())).ThrowIfNull();
    }
    
    public static void ThrowIfNull(this Domain.User user)
    {
        if (user == null)
        {
            throw new Exception();
        }
    }
}