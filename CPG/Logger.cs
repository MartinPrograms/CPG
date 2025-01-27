namespace CPG;

public class Logger
{
    public static readonly List<LogItem> LogItems = new();
    
    public static void Log(string message, string title, string stackTrace, LogSeverity severity)
    {
        var item = new LogItem(message, title, stackTrace, severity);
        LogItems.Add(item);
        Console.WriteLine(item);
    }
    
    public static void Info(string message, string title, string stackTrace = "")
    {
        Log(message, title, stackTrace, LogSeverity.Info);
    }
    
    public static void Warning(string message, string title, string stackTrace = "")
    {
        Log(message, title, stackTrace, LogSeverity.Warning);
    }
    
    public static void Error(string message, string title, string stackTrace = "")
    {
        Log(message, title, stackTrace, LogSeverity.Error);
    }
    
    public static void Fatal(string message, string title, string stackTrace = "")
    {
        Log(message, title, stackTrace, LogSeverity.Fatal);
    }
    
    public static void Clear()
    {
        LogItems.Clear();
    }
    
    public static void Print()
    {
        foreach (var logItem in LogItems)
        {
            Console.WriteLine(logItem);
        }
    }

    public static void Init()
    {
        // On application crash, print all logs, and dump to file in "./dumps/TIME-DATE.log"
        
        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var dumpPath = Path.Combine("dumps", $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");
            Directory.CreateDirectory("dumps");
            File.WriteAllLines(dumpPath, LogItems.Select(logItem => logItem.ToString()));
        };
    }
}

public class LogItem
{
    public string Message { get; }
    public string Title { get; }
    public string StackTrace { get; }
    public LogSeverity Severity { get; }
    
    public LogItem(string message, string title, string stackTrace, LogSeverity severity)
    {
        Message = message;
        Title = title;
        StackTrace = stackTrace;
        Severity = severity;
    }
    
    public override string ToString()
    {
        return $"[{Severity}] {Title}: {Message}{(StackTrace != "" ? $"\n{StackTrace}" : "")}";
    }
}

public enum LogSeverity
{
    Info,
    Warning,
    Error,
    Fatal
}