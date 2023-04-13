using Chatiks.User.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Chatiks.User.Domain;

public class User: IdentityUser<long>
{
    public UserFullName FullName { get; set; }
}