using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TimeTracker.Models
{
    class PastebinClient : IDisposable
    {
        private readonly HttpClient _http = new HttpClient
        {
            BaseAddress = new Uri("https://pastebin.com/")
        };

        private readonly string _devKey;

        public PastebinClient(string devKey)
        {
            _devKey = devKey ?? throw new ArgumentNullException(nameof(devKey));
        }

        public void Dispose() => _http.Dispose();

        public async Task<string> GetUserKeyAsync(string username, string password)
        {
            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["api_dev_key"] = _devKey,
                ["api_user_name"] = username,
                ["api_user_password"] = password
            });

            var resp = await _http.PostAsync("api/api_login.php", form);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode || body.StartsWith("Bad API request", StringComparison.OrdinalIgnoreCase))
                throw new Exception($"Login failed: {body}");

            return body.Trim(); // this is api_user_key
        }

        public async Task<PasteMeta?> GetLatestPasteMetaAsync(string userKey)
        {
            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["api_dev_key"] = _devKey,
                ["api_user_key"] = userKey,
                ["api_option"] = "list",
                ["api_results_limit"] = "1" // newest first; 1 = latest
            });

            var resp = await _http.PostAsync("api/api_post.php", form);
            var xml = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode || xml.StartsWith("Bad API request", StringComparison.OrdinalIgnoreCase))
                throw new Exception($"List failed: {xml}");

            // The response is an XML document with <paste> nodes.
            var doc = XDocument.Parse($"<root>{xml}</root>"); // wrap in a root to make parsing easier
            var paste = doc.Root?.Element("paste");
            if (paste == null) return null;

            return new PasteMeta
            {
                Key = paste.Element("paste_key")?.Value,
                DateUnix = long.TryParse(paste.Element("paste_date")?.Value, out var ts) ? ts : 0,
                Title = paste.Element("paste_title")?.Value,
                Size = int.TryParse(paste.Element("paste_size")?.Value, out var size) ? size : 0,
                ExpireDateUnix = long.TryParse(paste.Element("paste_expire_date")?.Value, out var exp) ? exp : 0,
                Privacy = paste.Element("paste_private")?.Value, // "0" public, "1" unlisted, "2" private
                Url = paste.Element("paste_url")?.Value,
                Format = paste.Element("paste_format_long")?.Value
            };
        }

        public async Task<string> GetPasteRawAsync(string pasteKey, string? userKey = null)
        {
            if (!string.IsNullOrWhiteSpace(userKey))
            {
                // API method — works for your private pastes too
                var form = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["api_dev_key"] = _devKey,
                    ["api_user_key"] = userKey!,
                    ["api_option"] = "show_paste",
                    ["api_paste_key"] = pasteKey
                });

                var resp = await _http.PostAsync("api/api_raw.php", form);
                var text = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode || text.StartsWith("Bad API request", StringComparison.OrdinalIgnoreCase))
                    throw new Exception($"Fetch raw failed: {text}");

                return text;
            }
            else
            {
                // Public/unlisted raw endpoint
                return await _http.GetStringAsync($"raw/{pasteKey}");
            }
        }

    }

    public record PasteMeta
    {
        public string? Key { get; init; }
        public long DateUnix { get; init; }
        public string? Title { get; init; }
        public int Size { get; init; }
        public long ExpireDateUnix { get; init; }
        public string? Privacy { get; init; }
        public string? Url { get; init; }
        public string? Format { get; init; }
    }

}
