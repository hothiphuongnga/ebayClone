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
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
}

public class UserLoginResponseDTO{
    public string Token { get; set; } = null!;
}

