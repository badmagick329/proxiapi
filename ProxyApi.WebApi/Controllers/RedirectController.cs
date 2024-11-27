using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class RedirectController : ControllerBase
{
    private readonly HttpClient _httpClient;

    public RedirectController(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
