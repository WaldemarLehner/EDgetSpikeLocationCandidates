using System.Collections.Concurrent;

namespace EDgetSpikeLocationCandidates
{
    public interface ISequentialReader : IFinishable
    {
        float Progress { get; }

        ConcurrentQueue<string> Queue { get; }

        void Start();

        void Stop();
    }
}