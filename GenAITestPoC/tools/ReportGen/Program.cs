using System.Text;
using System.Text.Json;
using System.Xml.Linq;

var root = FindRepoRoot();

var testcasesPath = Path.Combine(root, "artifacts", "testcases.json");
var reportsDir = Path.Combine(root, "artifacts", "reports");
Directory.CreateDirectory(reportsDir);

var outHtml = Path.Combine(reportsDir, "report.html");

string? trxPath =
    FindLatestFile(Path.Combine(root, "tests"), "results.trx")
    ?? FindLatestFile(root, "results.trx");

if (trxPath == null)
{
    Console.WriteLine("results.trx not found. Run dotnet test with trx logger first.");
    return;
}

var testcases = File.Exists(testcasesPath)
    ? (JsonSerializer.Deserialize<List<TestCase>>(File.ReadAllText(testcasesPath),
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<TestCase>())
    : new List<TestCase>();

var trxDoc = XDocument.Load(trxPath);

var unitResults = trxDoc.Descendants()
    .Where(x => x.Name.LocalName == "UnitTestResult")
    .Select(x => new TrxResult
    {
        TestName = (string?)x.Attribute("testName") ?? "(unknown)",
        Outcome = (string?)x.Attribute("outcome") ?? "(unknown)",
        Duration = (string?)x.Attribute("duration") ?? "",
        ErrorMessage = x.Descendants().FirstOrDefault(d => d.Name.LocalName == "Message")?.Value,
        StackTrace = x.Descendants().FirstOrDefault(d => d.Name.LocalName == "StackTrace")?.Value
    })
    .ToList();

int total = unitResults.Count;
int passed = unitResults.Count(r => r.Outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase));
int failed = unitResults.Count(r => r.Outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase));

var allRules = testcases
    .SelectMany(t => t.RulesCovered ?? Array.Empty<string>())
    .Where(r => !string.IsNullOrWhiteSpace(r))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .OrderBy(r => r)
    .ToList();

var passingRuleSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
foreach (var tc in testcases)
{
    if (tc.RulesCovered == null || tc.RulesCovered.Length == 0) continue;
    if (WasTestCasePassed(unitResults, tc))
    {
        foreach (var r in tc.RulesCovered)
            if (!string.IsNullOrWhiteSpace(r)) passingRuleSet.Add(r);
    }
}

var sb = new StringBuilder();
sb.AppendLine("<!doctype html>");
sb.AppendLine("<html><head><meta charset='utf-8'/>");
sb.AppendLine("<meta name='viewport' content='width=device-width, initial-scale=1'/>");
sb.AppendLine("<title>GenAI Test Report</title>");
sb.AppendLine(@"
<style>
body{font-family:Arial, sans-serif; margin:20px; line-height:1.35;}
h1{font-size:22px; margin:0 0 6px;}
small{color:#555;}
.card{border:1px solid #ddd; border-radius:10px; padding:14px; margin:12px 0;}
.kpi{display:flex; gap:12px; flex-wrap:wrap;}
.kpi div{border:1px solid #eee; border-radius:10px; padding:10px 12px; min-width:160px;}
.ok{color:#137333; font-weight:bold;}
.bad{color:#b3261e; font-weight:bold;}
table{border-collapse:collapse; width:100%; margin-top:10px;}
th,td{border:1px solid #ddd; padding:8px; vertical-align:top;}
th{background:#f6f6f6;}
code{background:#f3f3f3; padding:2px 4px; border-radius:4px;}
details{margin-top:6px;}
</style>
");
sb.AppendLine("</head><body>");

sb.AppendLine("<h1>GenAI-Augmented Testing Report</h1>");
sb.AppendLine($"<small><b>TRX:</b> {Html(trxPath)}<br/>");
sb.AppendLine($"<b>Generated:</b> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</small>");

sb.AppendLine("<div class='card'>");
sb.AppendLine("<h2 style='margin:0 0 8px; font-size:18px;'>Summary</h2>");
sb.AppendLine("<div class='kpi'>");
sb.AppendLine($"<div><div>Total Tests</div><div><b>{total}</b></div></div>");
sb.AppendLine($"<div><div>Passed</div><div class='ok'>{passed}</div></div>");
sb.AppendLine($"<div><div>Failed</div><div class='bad'>{failed}</div></div>");
sb.AppendLine("</div>");
sb.AppendLine("</div>");

sb.AppendLine("<div class='card'>");
sb.AppendLine("<h2 style='margin:0 0 8px; font-size:18px;'>Rule Coverage (from AI/ML metadata)</h2>");
if (allRules.Count == 0)
{
    sb.AppendLine("<p>No <code>rulesCovered</code> found in <code>artifacts/testcases.json</code>.</p>");
}
else
{
    sb.AppendLine("<table><tr><th>Rule</th><th>Covered by Passing Tests?</th></tr>");
    foreach (var rule in allRules)
    {
        var ok = passingRuleSet.Contains(rule);
        sb.AppendLine($"<tr><td>{Html(rule)}</td><td>{(ok ? "<span class='ok'>Yes</span>" : "<span class='bad'>No</span>")}</td></tr>");
    }
    sb.AppendLine("</table>");
}
sb.AppendLine("</div>");

sb.AppendLine("<div class='card'>");
sb.AppendLine("<h2 style='margin:0 0 8px; font-size:18px;'>AI/ML Generated Testcases</h2>");
if (testcases.Count == 0)
{
    sb.AppendLine("<p>testcases.json not found or empty.</p>");
}
else
{
    sb.AppendLine("<table>");
    sb.AppendLine("<tr><th>ID</th><th>Title</th><th>Endpoint</th><th>Expected</th><th>Rules Covered</th><th>Outcome</th></tr>");
    foreach (var tc in testcases)
    {
        var outcome = GetOutcomeForTestCase(unitResults, tc);
        var outcomeHtml = outcome == "Passed"
            ? "<span class='ok'>Passed</span>"
            : outcome == "Failed"
                ? "<span class='bad'>Failed</span>"
                : Html(outcome);

        sb.AppendLine("<tr>");
        sb.AppendLine($"<td>{Html(tc.Id)}</td>");
        sb.AppendLine($"<td>{Html(tc.Title)}</td>");
        sb.AppendLine($"<td><code>{Html($"{tc.Method} {tc.Url}")}</code></td>");
        sb.AppendLine($"<td>{tc.ExpectedStatus}</td>");
        sb.AppendLine($"<td>{Html(string.Join(", ", tc.RulesCovered ?? Array.Empty<string>()))}</td>");
        sb.AppendLine($"<td>{outcomeHtml}</td>");
        sb.AppendLine("</tr>");
    }
    sb.AppendLine("</table>");
}
sb.AppendLine("</div>");

sb.AppendLine("<div class='card'>");
sb.AppendLine("<h2 style='margin:0 0 8px; font-size:18px;'>TRX Test Results (Details)</h2>");
sb.AppendLine("<table>");
sb.AppendLine("<tr><th>Test Name</th><th>Outcome</th><th>Duration</th><th>Failure Info</th></tr>");

foreach (var r in unitResults.OrderByDescending(r => r.Outcome))
{
    var outcomeHtml = r.Outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase)
        ? "<span class='ok'>Passed</span>"
        : r.Outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase)
            ? "<span class='bad'>Failed</span>"
            : Html(r.Outcome);

    sb.AppendLine("<tr>");
    sb.AppendLine($"<td>{Html(r.TestName)}</td>");
    sb.AppendLine($"<td>{outcomeHtml}</td>");
    sb.AppendLine($"<td>{Html(r.Duration)}</td>");

    if (r.Outcome.Equals("Failed", StringComparison.OrdinalIgnoreCase))
    {
        sb.AppendLine("<td>");
        sb.AppendLine("<details><summary>View</summary>");
        if (!string.IsNullOrWhiteSpace(r.ErrorMessage))
            sb.AppendLine($"<div><b>Message:</b><pre>{Html(r.ErrorMessage!)}</pre></div>");
        if (!string.IsNullOrWhiteSpace(r.StackTrace))
            sb.AppendLine($"<div><b>StackTrace:</b><pre>{Html(r.StackTrace!)}</pre></div>");
        sb.AppendLine("</details>");
        sb.AppendLine("</td>");
    }
    else
    {
        sb.AppendLine("<td></td>");
    }

    sb.AppendLine("</tr>");
}
sb.AppendLine("</table>");
sb.AppendLine("</div>");

sb.AppendLine("</body></html>");

File.WriteAllText(outHtml, sb.ToString());
Console.WriteLine($"HTML report generated -> {outHtml}");

static bool WasTestCasePassed(List<TrxResult> results, TestCase tc)
{
    var outcome = GetOutcomeForTestCase(results, tc);
    return outcome.Equals("Passed", StringComparison.OrdinalIgnoreCase);
}

static string GetOutcomeForTestCase(List<TrxResult> results, TestCase tc)
{
    if (string.IsNullOrWhiteSpace(tc.Id)) return "Unknown";

    // Normalize formats:
    // TC-REG-001 -> TC_REG_001
    var id1 = tc.Id;
    var id2 = tc.Id.Replace("-", "_");

    // also handle lowercase/spacing if needed
    var candidates = new[] { id1, id2 };

    var hit = results.FirstOrDefault(r =>
        candidates.Any(c => r.TestName.Contains(c, StringComparison.OrdinalIgnoreCase)));

    return hit?.Outcome ?? "NotFoundInTrx";
}


static string Html(string? s) =>
    (s ?? "")
        .Replace("&", "&amp;")
        .Replace("<", "&lt;")
        .Replace(">", "&gt;")
        .Replace("\"", "&quot;");

static string? FindLatestFile(string folder, string fileName)
{
    if (!Directory.Exists(folder)) return null;
    var files = Directory.GetFiles(folder, fileName, SearchOption.AllDirectories);
    return files.Length == 0 ? null : files.OrderByDescending(File.GetLastWriteTimeUtc).First();
}

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir != null && !File.Exists(Path.Combine(dir.FullName, "GenAITestPoC.sln")))
        dir = dir.Parent;
    return dir?.FullName ?? Directory.GetCurrentDirectory();
}

public class TestCase
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Method { get; set; } = "POST";
    public string Url { get; set; } = "";
    public int ExpectedStatus { get; set; }
    public string[]? RulesCovered { get; set; }
}

public class TrxResult
{
    public string TestName { get; set; } = "";
    public string Outcome { get; set; } = "";
    public string Duration { get; set; } = "";
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
}
