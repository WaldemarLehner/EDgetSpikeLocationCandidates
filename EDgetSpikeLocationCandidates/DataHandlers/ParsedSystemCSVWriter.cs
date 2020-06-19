using System;
using System.Collections.Concurrent;
using System.IO;
using System.Numerics;
using System.Threading;

namespace EDgetSpikeLocationCandidates.DataHandlers
{
    public class ParsedSystemCSVWriter : IDisposable
    {
        private const int BatchSize = 256;
        private const char Separator = ';';
        private readonly StreamWriter sw;

        private ConcurrentQueue<IStarSystem> systemQueue = new ConcurrentQueue<IStarSystem>();

        public ParsedSystemCSVWriter(string outputFileName)
        {
            if (File.Exists(outputFileName))
            {
                throw new ArgumentException("File already exists", nameof(outputFileName));
            }

            this.sw = File.CreateText(outputFileName);
            this.WriteHeader();
        }

        public int QueueCount => this.systemQueue.Count;

        public void HandleSystem(IStarSystem system)
        {
            IStarSystem filteredSystem = DataFilters.SpikeCandidatesFilter.SpikeFilter(system);
            if (filteredSystem == null)
            {
                return;
            }

            this.systemQueue.Enqueue(filteredSystem);
            if (this.systemQueue.Count > BatchSize)
            {
                this.Commit();
            }
        }

        /// <summary>
        /// Force a write, regardless of whether or not the Batch Size has been hit. It is assumed that only relevant bodies remain in the system Object.
        /// </summary>
        public void Commit()
        {
            while (!this.systemQueue.IsEmpty)
            {
                if (this.systemQueue.TryDequeue(out IStarSystem result))
                {
                    foreach (var body in result.Bodies)
                    {
                        Vector3 colonia = new Vector3(-9530.5f, -910.28125f, 19808.125f);
                        Vector3 systemPos = new Vector3(result.Coordinates.x, result.Coordinates.y, result.Coordinates.z);


                        string[] values =
                        {
                            result.Id.ToString(),
                            result.Coordinates.x.ToString(),
                            result.Coordinates.y.ToString(),
                            result.Coordinates.z.ToString(),
                            result.SystemName,
                            body.Id.ToString(),
                            body.Name,
                            body.Type ?? "N/A",
                            body.DistanceToArrival.ToString() ?? "N/A",
                            body.SurfaceTemp.ToString() ?? "N/A",
                            body.Landable.ToString(),
                            body.Gravity.ToString(),
                            body.VolcanismType,
                            body.AtmosphereType,
                            systemPos.Length().ToString(),
                            Vector3.Distance(systemPos, colonia).ToString(),
                        };

                        lock (this.sw)
                        {
                            this.sw.Write(string.Join(Separator, values));
                            this.sw.Write("\n");
                            this.sw.Flush();
                        }
                    }
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }

        public void Dispose()
        {
            this.sw.Dispose();
        }

        private void WriteHeader()
        {
            string[] keys = { "systemid64", "x", "y", "z", "systemName", "bodyid64", "bodyName", "type", "distanceToArrival", "surfacetemp", "landable", "gravity", "volcanism", "atmosphere", "distanceToSol", "distanceToColonia" };
            this.sw.Write(string.Join(Separator, keys));
            this.sw.Write("\n");
            this.sw.Flush();
        }
    }
}
