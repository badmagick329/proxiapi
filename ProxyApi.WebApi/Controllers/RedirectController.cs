using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class RedirectController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public RedirectController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36"
        );
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("text/html")
        );
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
    }

    [HttpPost]
    public async Task<IActionResult> GetRedirectUrl([FromBody] LinkRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("Invalid request payload.");
        }

        try
        {
            var response = await _httpClient.SendAsync(
                new HttpRequestMessage(HttpMethod.Head, request.Url),
                HttpCompletionOption.ResponseHeadersRead
            );

            var redirectUrl = response.RequestMessage?.RequestUri?.ToString();
            if (redirectUrl == null)
            {
                return NotFound("No redirect URL found.");
            }

            return Ok(new { RedirectUrl = redirectUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}

public class LinkRequest
{
    public string Url { get; set; } = string.Empty;
}
