using System.ComponentModel.DataAnnotations;

namespace ebay.Dtos;

public class UserRegisterDTO
{
    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FullName { get; set; } = null!;
}
public class UserLoginDTO
{
    [Required(ErrorMessage = "Username không được để trống")]
    public string Username { get; set; } = null!;
    [Required(ErrorMessage = "Password không được để trống")]
    public string Password { get; set; } = null!;
}

public class UserLoginResponseDTO{
    public string Token { get; set; } = null!;
}

