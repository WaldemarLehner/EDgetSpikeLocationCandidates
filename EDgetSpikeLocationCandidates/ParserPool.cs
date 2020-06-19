using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace EDgetSpikeLocationCandidates
{
    public class ParserPool : IFinishable
    {
        private readonly ISequentialReader reader;

        private readonly Task[] workers;

        private readonly Func<string, IStarSystem> parser;

        private readonly Action<IStarSystem> systemHandler;

        private bool hasStarted = false;

        private StreamWriter debugSw;

        public ParserPool(ISequentialReader producer, Func<string, IStarSystem> parserFunction, Action<IStarSystem> systemHandler, bool printDebug, string debugOutputFilePath = null)
        {
            this.reader = producer ?? throw new ArgumentNullException(nameof(producer));
            this.systemHandler = systemHandler ?? throw new ArgumentNullException(nameof(systemHandler));
            this.parser = parserFunction ?? throw new ArgumentNullException(nameof(parserFunction));

            if (printDebug)
            {
                this.debugSw = File.CreateText(debugOutputFilePath);
            }
            this.workers = new Task[12];
            for (int i = 0; i < this.workers.Length; i++)
            {
                this.workers[i] = new Task(this.WorkerAction);
            }
        }

        public bool Finished => this.workers.All(x => x.Status == TaskStatus.RanToCompletion);

        public void Start()
        {
            if (this.hasStarted)
            {
                return;
            }

            this.hasStarted = true;
            foreach (var worker in this.workers)
            {
                worker.Start();
            }
        }

        private void WorkerAction()
        {
            while (!this.reader.Finished || !this.reader.Queue.IsEmpty)
            {
                if (this.reader.Queue.TryDequeue(out string data))
                {
                    try
                    {
                        var parsedData = this.parser(data);
                        this.systemHandler(parsedData);
                    }
                    catch (Exception e)
                    {
                        lock (this.debugSw)
                        {
                            this.debugSw.WriteLine($"<<<{data}>>> failed with the following exception: {e.Message}  Skipping.");
                        }
                    }
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}
