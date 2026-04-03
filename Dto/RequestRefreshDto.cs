using System.ComponentModel.DataAnnotations;

namespace Collabo_app.Dto
{
    public class RequestRefreshDto
    {
        [Required]public required string RefreshToken { get; set; }
    }
}
