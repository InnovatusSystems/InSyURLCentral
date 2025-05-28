using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace InSyURLCentral.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ControlPanelLinksController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public ControlPanelLinksController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var csvPath = Path.Combine(_env.WebRootPath, "data", "control_panel_links.csv");
            if (!System.IO.File.Exists(csvPath))
                return NotFound("Links data not found.");

            var folders = new List<ControlPanelFolder>();
            var folderMap = new Dictionary<string, ControlPanelFolder>();
            using (var reader = new StreamReader(csvPath, Encoding.UTF8))
            {
                string? headerLine = reader.ReadLine();
                if (headerLine == null) return Ok(folders);
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(',');
                    if (parts.Length < 5) continue;
                    var folderName = parts[0].Trim();
                    var folderIcon = parts[1].Trim();
                    var linkTitle = parts[2].Trim();
                    var linkUrl = parts[3].Trim();
                    var linkIcon = parts[4].Trim();
                    if (!folderMap.TryGetValue(folderName, out var folder))
                    {
                        folder = new ControlPanelFolder { Name = folderName, Icon = folderIcon, Links = new List<ControlPanelLink>() };
                        folderMap[folderName] = folder;
                        folders.Add(folder);
                    }
                    folder.Links.Add(new ControlPanelLink { Title = linkTitle, Url = linkUrl, Icon = linkIcon });
                }
            }
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
