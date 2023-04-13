using System.ComponentModel.DataAnnotations;

namespace Chatiks.Models;

public class LoginModel
{
    [Required] 
    public string Login { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}