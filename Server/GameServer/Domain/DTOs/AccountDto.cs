using System.ComponentModel.DataAnnotations;

namespace GameServer.Domain.DTOs;

public class AccountDto
{
    [Required]
    [EmailAddress]
    [MinLength(8), MaxLength(50)]
    public string Email { get; set; } = "";

    [Required]
    [MinLength(4), MaxLength(20)]
    public string Password { get; set; } = "";

    public string? Token { get; set; } = "";
}
