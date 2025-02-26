using System.Text.Json;
using NetCubeSolver;
using NetCubeSolver.Benchmarking;

namespace CubeSolver.Utils;

public class ProgramUtils
{
    static int LINE_LENGTH = 100;

    private static int _indent = 0;
    
    public static string WorkingDirectory { get; private set; }
    public static string OutputDirectory;
    public static string BenchmarkDirectory { get; private set; }

    public static void SetupWorkingDirectory(string workingDirectory)
    {
        WorkingDirectory = workingDirectory;
        BenchmarkDirectory = Path.Combine(WorkingDirectory, "Benchmarks");
    }

    public static void Write(string text="")
    {
        var prefix = string.Concat(Enumerable.Repeat(" ", _indent));
        Console.WriteLine($"{prefix}{text}");
    }

    public static void ReWrite(string text = "")
    {
        var prefix = string.Concat(Enumerable.Repeat(" ", _indent));
        Console.Write($"\r{prefix}{text}");
    }

    public static void WriteFilledLine(char fillCharacter, string line="")
    {
        var targetLineLength = LINE_LENGTH - _indent;
        var lineLength = line.Length;

        if (lineLength == 0)
        {
            Write(string.Concat(Enumerable.Repeat(fillCharacter, targetLineLength)));
        }
        else
        {
            var middleLineLength = (targetLineLength - lineLength - 2) / 2;
            var filler = string.Concat(Enumerable.Repeat(fillCharacter, middleLineLength));
            Write($"{filler} {line} {filler}");
        }
    }

    public static string GetUserInput(string prompt, string defaultValue = "")
    {
        var inputPrompt = $">> {prompt} ";
        var prefix = string.Concat(Enumerable.Repeat(" ", _indent));
        var defaultedValue = "";
        if (!string.IsNullOrWhiteSpace(defaultValue))
        {
            defaultedValue = $"({defaultValue}) ";
        }
        Console.Write($"{prefix}{inputPrompt}{defaultedValue}");
        var input = Console.ReadLine();
        Write();
        
        return input?.Replace(inputPrompt, "").Trim();
    }

    public static int GetUserInputInt(string prompt, int defaultValue = 1)
    {
        var input = GetUserInput(prompt, defaultValue.ToString());
        var selection = defaultValue;
        try
        {
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrEmpty(input))
            {
                return defaultValue;
            }
            selection = int.Parse(input ?? defaultValue.ToString());
        }
        catch (Exception e)
        {
            Environment.Exit(0);
        }

        return selection ;
    }

    public static int GetUserSelection(string context, string[] options)
    {
        Write($":::: {context} ::::");
        var optionIndex = 0;
        foreach (var option in options)
        {
            Write($" {optionIndex + 1}: {option}");
            optionIndex++;
        }
        
        Write($" {optionIndex + 1}: Exit ");
        
        var input = GetUserInput("Select an option: ");
        var selection = 1;
        try
        {
            selection = int.Parse(input ?? "-1");
            if (selection == optionIndex + 1)
            {
                Environment.Exit(0);
            }
        }
        catch (Exception e)
        {
            Environment.Exit(0);
        }

        return selection - 1;
    }

    public static void Indent() =>  _indent+=2;

    public static void Unindent()
    {
        _indent -= 2;
        _indent = Math.Max(0, _indent);
    }

    public static Config GetConfigFromUser()
    {
        DirectoryInfo dir = new DirectoryInfo(WorkingDirectory);
        var configs = dir.GetFiles().Where(f => f.Extension == ".json").ToList();
        var configNames = configs.Select(x => x.Name).ToArray();
        var configFile = GetUserSelection("Select a config file to load:", configNames);
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        Config config = JsonSerializer.Deserialize<Config>(File.ReadAllText(configs[configFile].FullName), options);
        config.InitializePieces();
        return config;
        
    }
    
    public static BenchmarkSettings GetBenchmarkSettingsFromUser()
    {
        DirectoryInfo dir = new DirectoryInfo(BenchmarkDirectory);
        var configs = dir.GetFiles().Where(f => f.Extension == ".json").ToList();
        var configNames = configs.Select(x => x.Name).ToArray();
        var configFile = GetUserSelection("Select a benchmark file to load:", configNames);
        
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        
        BenchmarkSettings settings = JsonSerializer.Deserialize<BenchmarkSettings>(File.ReadAllText(configs[configFile].FullName), options);
        return settings;
        
    }
}