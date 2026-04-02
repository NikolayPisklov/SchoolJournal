namespace SchoolJournalAuthApi.Models
{
    public class RefreshTokensRequestDto
    {
        public int UserId { get; set; }
        public required string RefreshToken { get; set; }
    }
}
