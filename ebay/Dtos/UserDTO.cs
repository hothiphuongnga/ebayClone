namespace ebay.Dtos;

public class UserDTO
{
     public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }
}