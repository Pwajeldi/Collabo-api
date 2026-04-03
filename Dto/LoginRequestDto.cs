using System.ComponentModel.DataAnnotations;

namespace Collabo_app.Dto
{
    public class LoginRequestDto
    {
        [Required] public required string Email {  get; set; }
        [Required] public required string Password { get; set; }
    }
}
