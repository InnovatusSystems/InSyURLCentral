using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace InSyURLCentral.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControlPanelLinksController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ControlPanelLinksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var folders = _configuration.GetSection("ControlPanelLinks").Get<List<ControlPanelFolder>>();
            return Ok(folders);
        }
    }

    public class ControlPanelFolder
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<ControlPanelLink> Links { get; set; } = new();
    }

    public class ControlPanelLink
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
