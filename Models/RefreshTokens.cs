namespace Collabo_app.Models
{
    public class RefreshTokens
    {
        public int Id { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Expiry {  get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
