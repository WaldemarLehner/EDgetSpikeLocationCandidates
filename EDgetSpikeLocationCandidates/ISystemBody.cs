using System.Collections;
using System.Collections.Generic;

namespace EDgetSpikeLocationCandidates
{
    public interface ISystemBody
    {
        ulong Id { get; }

        string Name { get; }

        string Type { get; }

        string SubType { get; }

        float? DistanceToArrival { get; }

        float? SurfaceTemp { get; }

        bool Landable { get; }

        float Gravity { get; }

        string VolcanismType { get; }

        string AtmosphereType { get; }

        ICollection<ISystemBody> Parents { get; }

        ICollection<ISystemBody> Children { get; }
    }
}