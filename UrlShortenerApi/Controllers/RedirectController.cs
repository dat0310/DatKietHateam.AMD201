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
        public IActionResult Get(string shortCode)
        {
            var shortUrl = _context.ShortUrls.FirstOrDefault(u => u.ShortCode == shortCode);
            if (shortUrl == null)
            {
                return NotFound();
            }
            return Redirect(shortUrl.OriginalUrl);
        }
    }
}