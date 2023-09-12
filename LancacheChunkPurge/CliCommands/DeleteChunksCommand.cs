//// ReSharper disable MemberCanBePrivate.Global - Properties used as parameters can't be private with CliFx, otherwise they won't work.
//namespace LancacheChunkPurge.CliCommands
//{
//    //TODO document
//    //TODO implement being able to enter a single request line and it will spit out the md5 version
//    //TODO rename?
//    [UsedImplicitly]
//    [Command("delete-chunks", Description = "TODO")]
//    public sealed class DeleteChunksCommand : ICommand
//    {
//        //TODO document + validation
//        //TODO test how many lines can be read and how quickly
//        [CommandOption("count", shortName: 'c',  Description = "TODO  Default: TODO")]
//        public int? NumberOfLogEntries
//        {
//            get => _numberOfLogEntries ?? 1000;
//            // Need to use a setter in order to set a default value, so that the default will only be used when the option flag is specified
//            set => _numberOfLogEntries = value;
//        }

//        private IAnsiConsole _ansiConsole;
//        private int? _numberOfLogEntries;

//        //TODO need to validate that permissions exist
//        // TODO is it possible for me to automatically look this up?
//        //TODO needs to be configured
//        string accessLogPath = "/mnt/cache/lancache/logs/access.log";

//        public ValueTask ExecuteAsync(IConsole console)
//        {
//            _ansiConsole = console.CreateAnsiConsole();

//            var timer = Stopwatch.StartNew();

//            _ansiConsole.LogMarkupLine("Starting...");
//            _ansiConsole.LogMarkupLine($"Reading last {LightYellow(NumberOfLogEntries.Value)} lines from access.log:");
//            var lines = ReadLastNLines(accessLogPath, NumberOfLogEntries.Value);

//            var parsed = NginxLogParser.ParseRequestLogs2(lines);
//            foreach (var logInfo in parsed)
//            {
//                var computed = logInfo.ComputeOnDiskFileName();

//                if (computed.Count > 1)
//                {
//                    _ansiConsole.MarkupLine($"{logInfo.RequestAsString} => On Disk : ");
//                    foreach (var onDisk in computed)
//                    {
//                        _ansiConsole.MarkupLine("     " + LightYellow(onDisk));
//                    }
//                    continue;
//                }
//                _ansiConsole.MarkupLine($"{logInfo.RequestAsString} => On Disk : {LightYellow(computed.First())}");

//            }

//            var requestsPerSecond = (int)(lines.Count / timer.Elapsed.TotalSeconds);
//            AnsiConsole.Console.LogMarkupLine($"Requests per second : {LightYellow(requestsPerSecond)}");
//            _ansiConsole.LogMarkupLine("Done");
//            return default;
//        }

//        public static List<string> ReadLastNLines(string path, int lineCount)
//        {
//            var lines = new List<string>();

//            using var reader = new Argus.IO.ReverseFileReader(path);
//            reader.LineEnding = Argus.IO.LineEnding.Lf;
//            while (lines.Count != lineCount)
//            {
//                lines.Add(reader.ReadLine());
//            }
//            return lines;
//        }
//    }
//}