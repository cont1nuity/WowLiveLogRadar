
using System.Diagnostics;
using System.Globalization;

Console.WriteLine("Log to run: ");
string sourcePath = Console.ReadLine()!;
var reader = new StreamReader(sourcePath);


Console.WriteLine("Absolute path to target file"); ;
string targetPath = Console.ReadLine()!;
var fs = new FileStream(targetPath, FileMode.Open);
var writer = new StreamWriter(fs);

// since I wanted to use datetime helper methods, I just added a bogus year, since the log files doesn't have year.
var bogusYear = 2018;
var format = "yyyy/M/d HH:mm:ss.fff";

var startTime = (dynamic)null;

var stopWatch = new Stopwatch();
stopWatch.Start();

bool inEncounter = false;

string? currentLine = null;
while (true) {
    currentLine ??= reader.ReadLine()!;

    if (currentLine == null) {
        Console.WriteLine("Done");
        break;
    }

    if (!inEncounter && currentLine.Contains("ENCOUNTER_START")) {
        inEncounter = true;
        stopWatch.Reset();
        stopWatch.Start();
        startTime = (dynamic)null;
    }
    else if (inEncounter && currentLine.Contains("ENCOUNTER_END")) {
        inEncounter = false;
        stopWatch.Stop();
        stopWatch.Reset();
        startTime = (dynamic)null;
        Console.WriteLine(currentLine);
        writer.WriteLine(currentLine);
        currentLine = null;
        continue;
    }
    else if (!inEncounter && (currentLine.Contains("WORLD_MARKER_PLACED") || currentLine.Contains("WORLD_MARKER_REMOVED"))) {
        Console.WriteLine(currentLine);
        writer.WriteLine(currentLine);
        currentLine = null;
        continue;
    }

    if (inEncounter && currentLine != null) {
        var timeString = currentLine.Split("  ")[0];
        var lineTime = DateTime.ParseExact($"{bogusYear}/{timeString}", format, CultureInfo.InvariantCulture);
        startTime ??= lineTime;

        var includetime = startTime.AddMilliseconds(stopWatch.ElapsedMilliseconds);
        if (lineTime < includetime)
        {
            Console.WriteLine(currentLine);
            writer.WriteLine(currentLine);
            currentLine = null;
        }
    } else {
        currentLine = null;
    }
}