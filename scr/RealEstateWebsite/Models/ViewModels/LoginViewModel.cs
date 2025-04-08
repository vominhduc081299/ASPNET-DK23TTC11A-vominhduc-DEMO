// Models/ViewModels/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
    