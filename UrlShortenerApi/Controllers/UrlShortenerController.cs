using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Data;
using UrlShortenerApi.Models;
using System.Text.RegularExpressions;

namespace UrlShortenerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlShortenerController : ControllerBase
    {
        private readonly UrlShortenerContext _context;

        public UrlShortenerController(UrlShortenerContext context)
        {
            _context = context;
        }

        // POST: api/urlshortener
        [HttpPost]
        public async Task<IActionResult> CreateShortUrl([FromBody] string originalUrl)
        {
            if (!IsValidUrl(originalUrl))
                return BadRequest("Invalid URL format.");

            var shortCode = GenerateShortCode();
            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow
            };

            _context.ShortUrls.Add(shortUrl);
            await _context.SaveChangesAsync();

            var result = $"http://localhost:5000/{shortCode}"; // Adjust base URL as needed
            return Ok(new { ShortUrl = result });
        }

        // GET: api/urlshortener/{shortCode}
        [HttpGet("{shortCode}")]
        public async Task<IActionResult> GetOriginalUrl(string shortCode)
        {
            var shortUrl = await _context.ShortUrls
                .FirstOrDefaultAsync(s => s.ShortCode == shortCode);

            if (shortUrl == null)
                return NotFound("Short URL not found.");

            return Redirect(shortUrl.OriginalUrl);
        }

        // GET: api/urlshortener
        [HttpGet]
        public async Task<IActionResult> GetAllUrls()
        {
            var urls = await _context.ShortUrls.ToListAsync();
            return Ok(urls);
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private string GenerateShortCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}