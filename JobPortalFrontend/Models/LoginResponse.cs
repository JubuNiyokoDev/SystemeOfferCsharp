public class LoginResponse
{
    public string Message { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public string RedirectUrl { get; set; }
}
