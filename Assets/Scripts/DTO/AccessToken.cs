using Newtonsoft.Json;

public class AccessToken
{
    [JsonProperty(PropertyName = "access_token")]
    public string Token { get; set; }

    [JsonProperty(PropertyName = "userName")]
    public string Email { get; set; }

    [JsonProperty(PropertyName = "refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty(PropertyName = "refresh_expires_in")]
    public float RefreshExpireIn { get; set; }

    [JsonProperty(PropertyName = "expires_in")]
    public float TokenExpiresIn { get; set; }
}
