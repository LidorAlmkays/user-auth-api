namespace Application.TokenService
{
    public class JWTSettings
    {
        public required string JWTSecret { get; set; }
        public required int RefreshTokenTime { get; set; }
        public required int AccessTokenTime { get; set; }
    }
}