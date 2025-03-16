namespace LibraryApi.Controllers.DTO.AUthenticationDTO
{
    public class RefreshTokenDto
    {
        public string RefreshToken { get; set; }  = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
