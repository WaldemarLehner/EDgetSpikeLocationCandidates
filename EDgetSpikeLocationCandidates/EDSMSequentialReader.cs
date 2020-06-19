using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EDgetSpikeLocationCandidates
{
    public class EDSMSequentialReader : ISequentialReader, IDisposable
    {
        private readonly FileStream fs = null;
        private readonly StreamReader sr = null;
        private readonly Task reader;
        private bool isRunning = false;

        public EDSMSequentialReader(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            this.Queue = new ConcurrentQueue<string>();
            this.reader = new Task(this.Read);
            this.fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            this.sr = new StreamReader(this.fs);
        }

        public float Progress => (float)this.sr.BaseStream.Position / this.sr.BaseStream.Length;

        public bool Finished { get; private set; } = false;

        public ConcurrentQueue<string> Queue { get; private set; }

        public void Dispose()
        {
            this.sr.Dispose();
            this.fs.Dispose();
        }

        public void Start()
        {
            if (this.isRunning)
            {
                return;
            }

            this.isRunning = true;
            this.reader.Start();
        }

        public void Stop()
        {
            if (!this.isRunning)
            {
                return;
            }

            this.isRunning = false;
        }

        private void Read()
        {
            StringBuilder sb = new StringBuilder(2048);
            int depth = 0;
            while (this.isRunning && !this.sr.EndOfStream)
            {
                // If queue > 1024, wait a bit to clear the queue
                if (this.Queue.Count >= 1024)
                {
                    Thread.Sleep(5);
                }

                int x = this.sr.Read();
                if (x == -1)
                {
                    // End of file
                    if (depth != 0)
                    {
                        throw new Exception();
                    }

                    this.Finished = true;
                    return;
                }

                char c = (char)x;

                if (c == '{')
                {
                    depth++;
                    sb.Append(c);
                }
                else if (c == '}')
                {
                    depth--;
                    sb.Append(c);
                    if (depth == 0)
                    {
                        this.Queue.Enqueue(sb.ToString());
                        sb.Clear();
                    }
                    else if (depth < 0)
                    {
                        throw new Exception(); // TODO: Add proper error handling
                    }
                }
                else
                {
                    if (depth > 0)
                    {
                        sb.Append(c);
                    }
                }
            }

            if (this.sr.EndOfStream)
            {
                this.Finished = true;
                this.isRunning = false;
            }
        }
    }
}
