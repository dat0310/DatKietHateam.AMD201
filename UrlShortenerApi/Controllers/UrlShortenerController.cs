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

        [HttpPost]
        public async Task<IActionResult> CreateShortUrl([FromBody] string originalUrl)
        {
            if (string.IsNullOrWhiteSpace(originalUrl))
                return BadRequest("URL cannot be empty.");

            if (!IsValidUrl(originalUrl))
                return BadRequest("Invalid URL format. Please provide a valid, well-formed URL.");

            var shortCode = GenerateShortCode();
            var shortUrl = new ShortUrl
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow
            };

            _context.ShortUrls.Add(shortUrl);
            await _context.SaveChangesAsync();

            var result = $"http://localhost:5281/{shortCode}";
            return Ok(new { ShortUrl = result });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUrls()
        {
            var urls = await _context.ShortUrls.ToListAsync();
            return Ok(urls);
        }

        [HttpDelete("{shortCode}")]
        public async Task<IActionResult> DeleteShortUrl(string shortCode)
        {
            var shortUrl = await _context.ShortUrls
                .FirstOrDefaultAsync(s => s.ShortCode == shortCode);

            if (shortUrl == null)
                return NotFound("Short URL not found.");

            _context.ShortUrls.Remove(shortUrl);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IsValidUrl(string url)
        {
            // Step 1: Basic URI validation
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                return false;
            }

            // Step 2: Stricter regex validation
            // This regex ensures a proper domain structure and valid characters
            string pattern = @"^(https?:\/\/)(www\.)?([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}(\/[a-zA-Z0-9\-\._~:\/?#[\]@!$&'()*+,;=]*)?$";
            return Regex.IsMatch(url, pattern);
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