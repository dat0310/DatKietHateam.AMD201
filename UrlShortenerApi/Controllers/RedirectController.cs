using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Data;

namespace UrlShortenerApi.Controllers
{
    [Route("")] 
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly UrlShortenerContext _context;

        public RedirectController(UrlShortenerContext context)
        {
            _context = context;
        }


        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectShortUrl(string shortCode)
        {
            var shortUrl = await _context.ShortUrls
                .FirstOrDefaultAsync(s => s.ShortCode == shortCode);

            if (shortUrl == null)
                return NotFound("Short URL not found.");

            return Redirect(shortUrl.OriginalUrl);
        }
    }
}