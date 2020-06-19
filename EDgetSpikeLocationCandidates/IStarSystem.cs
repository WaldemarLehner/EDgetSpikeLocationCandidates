using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EDgetSpikeLocationCandidates
{
    public interface IStarSystem
    {
        (float x, float y, float z) Coordinates { get; }

        string SystemName { get; }

        ulong Id { get; }

        ICollection<ISystemBody> Bodies { get; }
    }
}