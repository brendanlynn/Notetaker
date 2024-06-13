using static System.Console;
const int displayCount = 1000;
Title = "Notetaker";
string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
string noteSaveLocationFilePath = Path.Combine(baseDirectory, "NoteSaveLocation.txt");
string noteSaveLocation;
if (File.Exists(noteSaveLocationFilePath))
{
    try
    {
        noteSaveLocation = File.ReadAllText(noteSaveLocationFilePath);
    }
    catch
    {
        WriteLine("An unidentified error occured. Press any key to exit.");
        _ = ReadKey(true);
        return;
    }
    try
    {
        if (!Directory.Exists(noteSaveLocation))
            _ = Directory.CreateDirectory(noteSaveLocation);
    }
    catch
    {
        WriteLine("Could not create save directory. Press any key to exit.");
        _ = ReadKey(true);
        return;
    }
}
else
{
    WriteLine("Please enter save directory.");
    while (true)
    {
        WriteLine();
        noteSaveLocation = ReadLine() ?? "";
        WriteLine();
        try
        {
            if (!Directory.Exists(noteSaveLocation))
                _ = Directory.CreateDirectory(noteSaveLocation);
        }
        catch
        {
            WriteLine("Could not create save directory.");
            WriteLine("Please enter another save directory.");
            continue;
        }
        break;
    }
    try
    {
        File.WriteAllText(noteSaveLocationFilePath, noteSaveLocation);
    }
    catch
    {
        WriteLine("An error occured in saving the save directory filepath. The save directory will have to be re-specified next application start.");
    }
}
Queue<(DateTime, string)> entries = new();
string[] allFiles = Directory.GetFiles(noteSaveLocation);
for (int i = int.Max(0, allFiles.Length - displayCount); i < allFiles.Length; i++)
{
    string immediatePath = allFiles[i];
    string namePlusExtension;
    string name;
    DateTime date;
    try
    {
        namePlusExtension = immediatePath.Split('\\')[^1];
        name = namePlusExtension.Remove(namePlusExtension.Length - 4);
        date = GetDateTime(name);
    }
    catch
    {
        WriteLine($"Could not read entry #{i + 1}.");
        continue;
    }
    string c;
    try
    {
        c = File.ReadAllText(immediatePath);
    }
    catch
    {
        WriteLine($"Could not read entry #{i + 1}, from {DateNiceFormat(date)}.");
        continue;
    }
    entries.Enqueue((date, c));
}
WriteLine("Notetaker initialized.");
Thread.Sleep(1000);
Clear();
foreach ((DateTime when, string entry) in entries)
    WriteEntry(when, entry);
while (true)
{
    int current = CursorTop;
    string? s = ReadLine();
    DateTime time = DateTime.UtcNow;
    string tempSaveLocation = Path.Combine(noteSaveLocation, time.ToString("yyyyyMMddHHmmss") + ".txt");
    CursorTop = current;
    CursorLeft = 0;
    if (s is null)
        continue;
    if (string.IsNullOrWhiteSpace(s))
        continue;
    Write(new string(' ', s.Length));
    CursorTop = current;
    CursorLeft = 0;
    s = s.Trim();
    WriteEntry(time, s);
    try
    {
        File.WriteAllText(tempSaveLocation, s);
    }
    catch
    {
        WriteLine();
        WriteLine("An error occured in writing to the file. Press 10 keys to terminate.");
        for (int i = 10; i > 0; i--)
        {
            WriteLine(i);
            _ = ReadKey(true);
        }
    }
}
static DateTime GetDateTime(string DateString)
{
    return new(
        int.Parse(DateString[..5]),
        int.Parse(DateString[5..7]),
        int.Parse(DateString[7..9]),
        int.Parse(DateString[9..11]),
        int.Parse(DateString[11..13]),
        int.Parse(DateString[13..15])
    );
}
static string DateNiceFormat(DateTime DateTime)
    => DateTime.ToString("MM/dd/yyyy HH:mm:ss") + " UTC";
static void WriteEntry(DateTime When, string Entry)
{
    WriteLine(DateNiceFormat(When) + ":");
    WriteLine(Entry);
    WriteLine();
}