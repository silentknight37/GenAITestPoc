using Microsoft.ML;
using Microsoft.ML.Data;
using System.Text.Json;

string mode = args.Length > 0 ? args[0].ToLowerInvariant() : "train";

var root = FindRepoRoot();
var modelPath = Path.Combine(root, "tools", "RuleML", "rule-model.zip");

if (mode == "train")
{
    Train(root, modelPath);
    return;
}
if (mode == "generate")
{
    Generate(root, modelPath);
    return;
}

Console.WriteLine("Usage:");
Console.WriteLine("  dotnet run --project tools/RuleML -- train");
Console.WriteLine("  dotnet run --project tools/RuleML -- generate");

static void Train(string root, string modelPath)
{
    var ml = new MLContext(seed: 42);

    var trainPath = Path.Combine(root, "tools", "RuleML", "training.tsv");
    if (!File.Exists(trainPath))
    {
        Console.WriteLine($"Training file not found: {trainPath}");
        return;
    }

    var data = ml.Data.LoadFromTextFile<TrainRow>(
    trainPath,
    hasHeader: true,
    separatorChar: '\t',
    allowQuoting: true,
    trimWhitespace: true
);

    var preview = data.Preview(maxRows: 5);
    Console.WriteLine("=== DATA PREVIEW ===");
    foreach (var row in preview.RowView)
    {
        var text = row.Values.First(v => v.Key == "Text").Value;
        var label = row.Values.First(v => v.Key == "Label").Value;
        Console.WriteLine($"Text={text} | Label={label}");
    }
    Console.WriteLine("====================");

    var pipeline =
     ml.Transforms.Text.NormalizeText("TextNorm", nameof(TrainRow.Text))
     .Append(ml.Transforms.Text.FeaturizeText("Features", "TextNorm"))
     .Append(ml.Transforms.Conversion.MapValueToKey("LabelKey", nameof(TrainRow.Label)))
     .Append(ml.MulticlassClassification.Trainers.LbfgsMaximumEntropy(
         new Microsoft.ML.Trainers.LbfgsMaximumEntropyMulticlassTrainer.Options
         {
             LabelColumnName = "LabelKey",
             FeatureColumnName = "Features",
             MaximumNumberOfIterations = 200
         }))
     .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabelText", "PredictedLabel"));

    var split = ml.Data.TrainTestSplit(data, testFraction: 0.1, seed: 42);

    var model = pipeline.Fit(split.TrainSet);

    Console.WriteLine();
    Console.WriteLine("Model Evaluation Summary");
    Console.WriteLine("------------------------");

    var trainPred = model.Transform(split.TrainSet);
    var trainMetrics = ml.MulticlassClassification.Evaluate(
        trainPred,
        labelColumnName: "LabelKey",
        scoreColumnName: "Score",
        predictedLabelColumnName: "PredictedLabel"
    );

    Console.WriteLine(
    $"Training Accuracy (learning capability)        : {trainMetrics.MicroAccuracy * 100:0.0}%"
    );

    var testPred = model.Transform(split.TestSet);
    var testMetrics = ml.MulticlassClassification.Evaluate(
        testPred,
        labelColumnName: "LabelKey",
        scoreColumnName: "Score",
        predictedLabelColumnName: "PredictedLabel"
    );

    Console.WriteLine(
    $"Test Accuracy (generalization to new rules)    : {testMetrics.MicroAccuracy * 100:0.0}%"
    );
    var cvResults = ml.MulticlassClassification.CrossValidate(
    data: data,
    estimator: pipeline,
    numberOfFolds: 5,
    labelColumnName: "LabelKey"
);

    Console.WriteLine(
    $"Cross-Validation Accuracy (overall stability)  : {cvResults.Average(r => r.Metrics.MicroAccuracy) * 100:0.0}%"
    );
    ml.Model.Save(model, split.TrainSet.Schema, modelPath);
    Console.WriteLine($"Model saved: {modelPath}");
}

static void Generate(string root, string modelPath)
{
    var reqPath = Path.Combine(root, "artifacts", "requirements.txt");
    if (!File.Exists(reqPath))
    {
        Console.WriteLine($"requirements.txt not found: {reqPath}");
        return;
    }

    if (!File.Exists(modelPath))
    {
        Console.WriteLine($"Model not found: {modelPath}");
        Console.WriteLine("Run: dotnet run --project tools/RuleML -- train");
        return;
    }

    var ml = new MLContext();
    var model = ml.Model.Load(modelPath, out _);
    var emptyData = ml.Data.LoadFromEnumerable(new List<TrainRow>());

    var outSchema = model.GetOutputSchema(emptyData.Schema);

    VBuffer<ReadOnlyMemory<char>> keyValues = default;
    outSchema["PredictedLabel"].GetKeyValues(ref keyValues);

    var labels = keyValues.DenseValues().Select(x => x.ToString()).ToArray();

    var predEngine = ml.Model.CreatePredictionEngine<TrainRow, PredRow>(model);

    var lines = File.ReadAllLines(reqPath)
        .Select(l => l.Trim())
        .Where(l => !string.IsNullOrWhiteSpace(l))
        .ToList();

    var detectedRules = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    Console.WriteLine("Predictions:");
    foreach (var line in lines)
    {
        var pred = predEngine.Predict(new TrainRow { Text = line });

        Console.WriteLine(
            $"- \"{line}\" => {pred.PredictedLabel} (scoreLen={pred.Score.Length})"
        );

        if (!string.IsNullOrWhiteSpace(pred.PredictedLabel))
            detectedRules.Add(pred.PredictedLabel);
    }

    var testcases = new List<TestCase>();

    testcases.Add(new TestCase
    {
        id = "TC-REG-001",
        title = "Valid registration returns 201",
        method = "POST",
        url = "/api/users/register",
        body = new Dictionary<string, object> { ["email"] = "ok@test.com", ["password"] = "Password1", ["age"] = 22 },
        expectedStatus = 201,
        rulesCovered = detectedRules.ToArray()
    });

    if (detectedRules.Contains("PasswordPolicy") || detectedRules.Contains("InvalidInputReturns400"))
    {
        testcases.Add(new TestCase
        {
            id = "TC-REG-002",
            title = "Weak password returns 400",
            method = "POST",
            url = "/api/users/register",
            body = new Dictionary<string, object> { ["email"] = "weak@test.com", ["password"] = "pass", ["age"] = 22 },
            expectedStatus = 400,
            rulesCovered = new[] { "PasswordPolicy", "InvalidInputReturns400" }.Where(detectedRules.Contains).ToArray()
        });
    }

    if (detectedRules.Contains("AgeMin18") || detectedRules.Contains("InvalidInputReturns400"))
    {
        testcases.Add(new TestCase
        {
            id = "TC-REG-003",
            title = "Underage returns 400",
            method = "POST",
            url = "/api/users/register",
            body = new Dictionary<string, object> { ["email"] = "kid@test.com", ["password"] = "Password1", ["age"] = 17 },
            expectedStatus = 400,
            rulesCovered = new[] { "AgeMin18", "InvalidInputReturns400" }.Where(detectedRules.Contains).ToArray()
        });
    }

    if (detectedRules.Contains("UniqueEmail") || detectedRules.Contains("DuplicateEmailReturns409"))
    {
        testcases.Add(new TestCase
        {
            id = "TC-REG-004",
            title = "Duplicate email returns 409",
            method = "POST",
            url = "/api/users/register",
            setup = new[]
            {
                new SetupStep
                {
                    action = "register",
                    body = new Dictionary<string, object> { ["email"] = "dup@test.com", ["password"] = "Password1", ["age"] = 22 }
                }
            },
            body = new Dictionary<string, object> { ["email"] = "dup@test.com", ["password"] = "Password1", ["age"] = 22 },
            expectedStatus = 409,
            rulesCovered = new[] { "UniqueEmail", "DuplicateEmailReturns409" }.Where(detectedRules.Contains).ToArray()
        });
    }

    var outPath = Path.Combine(root, "artifacts", "testcases.json");
    Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
    File.WriteAllText(outPath, JsonSerializer.Serialize(testcases, new JsonSerializerOptions { WriteIndented = true }));

    Console.WriteLine($"\nGenerated -> {outPath}");
}

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir != null && !File.Exists(Path.Combine(dir.FullName, "GenAITestPoC.sln")))
        dir = dir.Parent;
    return dir?.FullName ?? Directory.GetCurrentDirectory();
}

public class TrainRow
{
    [LoadColumn(0)] public string Text { get; set; } = "";
    [LoadColumn(1)] public string Label { get; set; } = "";
}

public class PredRow
{
    [ColumnName("PredictedLabelText")]
    public string PredictedLabel { get; set; } = "";

    public float[] Score { get; set; } = Array.Empty<float>();
}

public class TestCase
{
    public string id { get; set; } = "";
    public string title { get; set; } = "";
    public string method { get; set; } = "POST";
    public string url { get; set; } = "";
    public Dictionary<string, object>? body { get; set; }
    public int expectedStatus { get; set; }
    public string[]? rulesCovered { get; set; }
    public SetupStep[]? setup { get; set; }
}

public class SetupStep
{
    public string action { get; set; } = "";
    public Dictionary<string, object>? body { get; set; }
}
