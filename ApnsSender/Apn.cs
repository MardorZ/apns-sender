using System.Security.Cryptography;
using System.Text;
using Jose;
using Newtonsoft.Json;

namespace ApnsSender;

/// <summary>
/// APN (Apple Push Notification) helper class.
/// </summary>
public static class Apn
{
    /// <summary>
    /// Sends a test notification to an iOS device.
    /// </summary>
    /// <param name="deviceToken">The device token of the iOS device you want to send the notification to.</param>
    /// <param name="p8PrivateKey">The contents of your .p8 private key file (including the "-----BEGIN PRIVATE KEY-----" and "-----END PRIVATE KEY-----" lines).</param>
    /// <param name="teamId">Your Apple Developer Team ID.</param>
    /// <param name="keyId"> Your Apple Developer Team ID.</param>
    /// <param name="bundleId"> Your Apple Developer Team ID.</param>
    public static async Task SendTestNotificationAsync(string deviceToken, string p8PrivateKey, string teamId, string keyId, string bundleId)
    {
        var audience = "https://api.sandbox.push.apple.com"; // Use "https://api.push.apple.com" for production
        var path = $"/3/device/{deviceToken}";

        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri(audience)
        };

        // Generate JWT token
        var header = new Dictionary<string, object>
        {
            { "alg", "ES256" },
            { "kid", keyId }
        };

        var payload = new Dictionary<string, object>
        {
            { "iss", teamId },
            { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() }
        };

        var privateKeyBytes = Encoding.UTF8.GetBytes(p8PrivateKey);
        using var ecdsa = ECDsa.Create();
        ecdsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
        var jwtToken = JWT.Encode(payload, ecdsa, JwsAlgorithm.ES256, header);

        var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Headers =
            {
                { "authorization", $"bearer {jwtToken}" },
                { "apns-topic", bundleId }
            },
            Content = new StringContent(JsonConvert.SerializeObject(new
            {
                aps = new
                {
                    alert = new
                    {
                        title = "Test title",
                        body = "Test body"
                    },
                    sound = "default"
                }
            }), Encoding.UTF8, "application/json")
        };

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}