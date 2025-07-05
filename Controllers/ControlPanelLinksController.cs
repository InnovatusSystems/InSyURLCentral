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
                    // Support up to 7 columns: Name, Icon, Title, Url, LinkIcon, StatusUrl, MatchPhrase
                    var folderName = parts.Length > 0 ? parts[0].Trim() : string.Empty;
                    var folderIcon = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                    var linkTitle = parts.Length > 2 ? parts[2].Trim() : string.Empty;
                    var linkUrl = parts.Length > 3 ? parts[3].Trim() : string.Empty;
                    var linkIcon = parts.Length > 4 ? parts[4].Trim() : string.Empty;
                    var statusUrl = parts.Length > 5 ? parts[5].Trim() : string.Empty;
                    var matchPhrase = parts.Length > 6 ? parts[6].Trim() : string.Empty;
                    if (!folderMap.TryGetValue(folderName, out var folder))
                    {
                        folder = new ControlPanelFolder { Name = folderName, Icon = folderIcon, Links = new List<ControlPanelLink>() };
                        folderMap[folderName] = folder;
                        folders.Add(folder);
                    }
                    folder.Links.Add(new ControlPanelLink { Title = linkTitle, Url = linkUrl, Icon = linkIcon, StatusUrl = statusUrl, MatchPhrase = matchPhrase });
                }
            }
            return Ok(folders);
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var csvPath = Path.Combine(_env.WebRootPath, "data", "control_panel_links.csv");
            if (!System.IO.File.Exists(csvPath))
                return Ok(new List<FlatLinkRow>());
            var rows = new List<FlatLinkRow>();
            using (var reader = new StreamReader(csvPath, Encoding.UTF8))
            {
                string? headerLine = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(',');
                    rows.Add(new FlatLinkRow
                    {
                        Name = parts.Length > 0 ? parts[0].Trim() : string.Empty,
                        Icon = parts.Length > 1 ? parts[1].Trim() : string.Empty,
                        Title = parts.Length > 2 ? parts[2].Trim() : string.Empty,
                        Url = parts.Length > 3 ? parts[3].Trim() : string.Empty,
                        LinkIcon = parts.Length > 4 ? parts[4].Trim() : string.Empty,
                        StatusUrl = parts.Length > 5 ? parts[5].Trim() : string.Empty,
                        MatchPhrase = parts.Length > 6 ? parts[6].Trim() : string.Empty
                    });
                }
            }
            return Ok(rows);
        }

        [HttpPost]
        public IActionResult Add([FromBody] FlatLinkRow row)
        {
            var csvPath = Path.Combine(_env.WebRootPath, "data", "control_panel_links.csv");
            var sb = new StringBuilder();
            if (!System.IO.File.Exists(csvPath))
            {
                sb.AppendLine("Name,Icon,Title,Url,LinkIcon,StatusUrl,MatchPhrase");
            }
            else
            {
                sb.Append(System.IO.File.ReadAllText(csvPath));
            }
            sb.AppendLine($"{row.Name},{row.Icon},{row.Title},{row.Url},{row.LinkIcon},{row.StatusUrl},{row.MatchPhrase}");
            System.IO.File.WriteAllText(csvPath, sb.ToString());
            return Ok();
        }

        [HttpPut("{idx}")]
        public IActionResult Update(int idx, [FromBody] FlatLinkRow row)
        {
            var csvPath = Path.Combine(_env.WebRootPath, "data", "control_panel_links.csv");
            if (!System.IO.File.Exists(csvPath)) return NotFound();
            var lines = System.IO.File.ReadAllLines(csvPath).ToList();
            if (idx + 1 >= lines.Count) return NotFound();
            lines[idx + 1] = $"{row.Name},{row.Icon},{row.Title},{row.Url},{row.LinkIcon},{row.StatusUrl},{row.MatchPhrase}";
            System.IO.File.WriteAllLines(csvPath, lines);
            return Ok();
        }

        [HttpDelete("{idx}")]
        public IActionResult Delete(int idx)
        {
            var csvPath = Path.Combine(_env.WebRootPath, "data", "control_panel_links.csv");
            if (!System.IO.File.Exists(csvPath)) return NotFound();
            var lines = System.IO.File.ReadAllLines(csvPath).ToList();
            if (idx + 1 >= lines.Count) return NotFound();
            lines.RemoveAt(idx + 1);
            System.IO.File.WriteAllLines(csvPath, lines);
            return Ok();
        }

        [HttpGet("fetch")]
        public async Task<IActionResult> FetchUrl([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest("Failed : URL parameter is required");
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                return BadRequest("Failed : Invalid URL");
            try
            {
                using var httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                using var response = await httpClient.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, response.Content.Headers.ContentType?.ToString() ?? "text/plain");
            }
            catch (TaskCanceledException)
            {
                return BadRequest("Failed : Request timed out");
            }
            catch (HttpRequestException ex)
            {
                return BadRequest($"Failed : {ex.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed : {ex.Message}");
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
            public string StatusUrl { get; set; } = string.Empty;
            public string MatchPhrase { get; set; } = string.Empty;
        }
    }

    public class FlatLinkRow
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string LinkIcon { get; set; } = string.Empty;
        public string StatusUrl { get; set; } = string.Empty;
        public string MatchPhrase { get; set; } = string.Empty;
    }
}
