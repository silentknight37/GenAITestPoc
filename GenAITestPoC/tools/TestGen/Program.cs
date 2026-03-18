using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var root = FindRepoRoot();
var jsonPath = Path.Combine(root, "artifacts", "testcases.json");
var outPath = Path.Combine(root, "tests", "MyApi.Tests.Integration", "GeneratedTests.cs");

if (!File.Exists(jsonPath))
{
    Console.WriteLine($"testcases.json not found: {jsonPath}");
    return;
}

var json = File.ReadAllText(jsonPath);
var cases = JsonSerializer.Deserialize<List<TestCase>>(json, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
}) ?? new List<TestCase>();

Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);

var code = GenerateXunitFile(cases);
File.WriteAllText(outPath, code);

Console.WriteLine($"Generated xUnit tests -> {outPath}");

static string GenerateXunitFile(List<TestCase> cases)
{
    var sb = new StringBuilder();

    sb.AppendLine("using System.Net;");
    sb.AppendLine("using System.Net.Http.Json;");
    sb.AppendLine("using Xunit;");
    sb.AppendLine("using MyApi.Models;"); 
    sb.AppendLine("");
    sb.AppendLine("namespace MyApi.Tests.Integration;");
    sb.AppendLine("");
    sb.AppendLine("public class GeneratedTests : IClassFixture<CustomWebAppFactory>, IDisposable");
    sb.AppendLine("{");
    sb.AppendLine("    private readonly CustomWebAppFactory _factory;");
    sb.AppendLine("    private readonly HttpClient _client;");
    sb.AppendLine("    public GeneratedTests(CustomWebAppFactory factory)");
    sb.AppendLine("    {");
    sb.AppendLine("        _factory = factory;");
    sb.AppendLine("        DbReset.Reset(_factory);");
    sb.AppendLine("        _client = factory.CreateClient();");
    sb.AppendLine("    }");
    sb.AppendLine("    public void Dispose()");
    sb.AppendLine("    {");
    sb.AppendLine("         _client.Dispose();");
    sb.AppendLine("    }");
    sb.AppendLine("");

    foreach (var tc in cases)
    {
        if (string.IsNullOrWhiteSpace(tc.Id)) continue;
        if (string.IsNullOrWhiteSpace(tc.Url)) continue;

        var method = (tc.Method ?? "POST").ToUpperInvariant();
        if (method != "POST") continue; // PoC: only POST

        var safeMethodName = ToSafeMethodName(tc.Id + "_" + tc.Title);

        sb.AppendLine("    [Fact]");
        sb.AppendLine($"    public async Task {safeMethodName}()");
        sb.AppendLine("    {");

        if (tc.Setup != null)
        {
            foreach (var step in tc.Setup)
            {
                if (step?.Action?.Equals("register", StringComparison.OrdinalIgnoreCase) == true)
                {
                    var email = step.Body?.GetString("email") ?? "setup@test.com";
                    //var email = $"gen_{Guid.NewGuid():N}@test.com";
                    var pass = step.Body?.GetString("password") ?? "Password1";
                    var age = step.Body?.GetInt("age") ?? 22;

                    sb.AppendLine($"        await _client.PostAsJsonAsync(\"/api/users/register\", new RegisterRequest(\"{Esc(email)}\", \"{Esc(pass)}\", {age}));");
                }
            }
            sb.AppendLine();
        }

        if (tc.Url.Contains("/api/users/register", StringComparison.OrdinalIgnoreCase))
        {
            var email = tc.Body?.GetString("email") ?? "test1@test.com";
            //var email = $"gen_{Guid.NewGuid():N}@test.com";
            var pass = tc.Body?.GetString("password") ?? "Password1";
            var age = tc.Body?.GetInt("age") ?? 22;

            sb.AppendLine($"        var res = await _client.PostAsJsonAsync(\"{tc.Url}\", new RegisterRequest(\"{Esc(email)}\", \"{Esc(pass)}\", {age}));");
            sb.AppendLine($"        Assert.Equal((HttpStatusCode){tc.ExpectedStatus}, res.StatusCode);");
        }
        else if (tc.Url.Contains("/api/users/login", StringComparison.OrdinalIgnoreCase))
        {
            var email = tc.Body?.GetString("email") ?? "test2@test.com";
            //var email = $"gen_{Guid.NewGuid():N}@test.com";
            var pass = tc.Body?.GetString("password") ?? "Password1";

            sb.AppendLine($"        var res = await _client.PostAsJsonAsync(\"{tc.Url}\", new LoginRequest(\"{Esc(email)}\", \"{Esc(pass)}\"));");
            sb.AppendLine($"        Assert.Equal((HttpStatusCode){tc.ExpectedStatus}, res.StatusCode);");
        }
        else
        {
            sb.AppendLine("        // Unsupported endpoint in PoC generator");
            sb.AppendLine("        Assert.True(true);");
        }

        sb.AppendLine("    }");
        sb.AppendLine("");
    }

    sb.AppendLine("}");

    return sb.ToString();
}

static string ToSafeMethodName(string s)
{
    var cleaned = new string(s.Select(ch => char.IsLetterOrDigit(ch) ? ch : '_').ToArray());
    if (string.IsNullOrWhiteSpace(cleaned)) cleaned = "Generated_Test";
    if (char.IsDigit(cleaned[0])) cleaned = "_" + cleaned;
    return cleaned.Length > 120 ? cleaned[..120] : cleaned;
}

static string Esc(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir != null && !File.Exists(Path.Combine(dir.FullName, "GenAITestPoC.sln")))
        dir = dir.Parent;
    return dir?.FullName ?? Directory.GetCurrentDirectory();
}

public class TestCase
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("method")] public string? Method { get; set; }
    [JsonPropertyName("url")] public string? Url { get; set; }
    [JsonPropertyName("expectedStatus")] public int ExpectedStatus { get; set; }
    [JsonPropertyName("body")] public JsonElement? BodyRaw { get; set; }
    [JsonPropertyName("setup")] public List<SetupStep>? Setup { get; set; }

    [JsonIgnore]
    public JsonDict? Body => BodyRaw.HasValue && BodyRaw.Value.ValueKind == JsonValueKind.Object
        ? new JsonDict(BodyRaw.Value)
        : null;
}

public class SetupStep
{
    [JsonPropertyName("action")] public string? Action { get; set; }
    [JsonPropertyName("body")] public JsonElement? BodyRaw { get; set; }

    [JsonIgnore]
    public JsonDict? Body => BodyRaw.HasValue && BodyRaw.Value.ValueKind == JsonValueKind.Object
        ? new JsonDict(BodyRaw.Value)
        : null;
}

public class JsonDict
{
    private readonly JsonElement _obj;
    public JsonDict(JsonElement obj) => _obj = obj;

    public string? GetString(string key)
        => _obj.TryGetProperty(key, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    public int? GetInt(string key)
    {
        if (!_obj.TryGetProperty(key, out var v)) return null;
        if (v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var i)) return i;
        if (v.ValueKind == JsonValueKind.String && int.TryParse(v.GetString(), out var j)) return j;
        return null;
    }
}
