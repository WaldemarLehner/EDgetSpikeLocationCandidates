using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using EDgetSpikeLocationCandidates.DataHandlers;

namespace EDgetSpikeLocationCandidates.Commands
{
    [Command]
    internal sealed class DefaultCommand : ICommand
    {
        private Stopwatch stopwatch;

        [CommandParameter(0, Description = "Path to json.")]
        public string Filepath { get; set; }

        [CommandOption("outputDir", 'o', Description = "File Output Directory. Default is at input file's directory.")]
        public string OutputDir { get; set; } = null;

        [CommandOption("log", 'l', Description = "Should Systems that cannot be parsed be logged in a seperate file?")]
        public bool Logging { get; set; } = false;

        public ValueTask ExecuteAsync(IConsole console)
        {
            this.HandleParameters();
            Console.WriteLine("Loggin is " + ( this.Logging ? "enabled" : "disabled"));
            ISequentialReader reader = new EDSMSequentialReader(this.Filepath);
            ParsedSystemCSVWriter xmlWriter = new ParsedSystemCSVWriter(Path.Combine(this.OutputDir, DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".csv"));
            ParserPool pool = new ParserPool(reader, Parsers.StarSystemParser.ParseStarSystem, xmlWriter.HandleSystem, this.Logging, DateTimeOffset.UtcNow.ToUnixTimeSeconds() + ".log");
            pool.Start();
            reader.Start();

            ICollection<IFinishable> actionsToFinish = new List<IFinishable>() { pool, reader };

            while (!actionsToFinish.All(x => x.Finished))
            {
                Thread.Sleep(1000);
                Console.Clear();
                ConsoleHelper.PrintProgress(reader.Progress, this.stopwatch.Elapsed);
            }

            xmlWriter.Commit();
            ConsoleHelper.PrintElapsedTime(this.stopwatch.Elapsed);
            return default;
        }

        private void HandleParameters()
        {
            if (!File.Exists(this.Filepath))
            {
                throw new FileNotFoundException("The given file path doesnt exist");
            }

            if (this.OutputDir == null)
            {
                this.OutputDir = Path.GetDirectoryName(this.Filepath);
            }
            else
            {
                if (!Directory.Exists(this.OutputDir))
                {
                    throw new DirectoryNotFoundException("The given output directory does nto exist.");
                }
            }

            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
        }
    }
}
