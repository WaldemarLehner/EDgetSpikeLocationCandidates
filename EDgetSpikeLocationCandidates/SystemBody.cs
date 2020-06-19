using System;
using System.Collections.Generic;
using System.Text;

namespace EDgetSpikeLocationCandidates
{
    public class SystemBody : ISystemBody
    {
        public SystemBody()
        {
            this.ChildrenList = new List<ISystemBody>();
            this.ParentList = new List<ISystemBody>();
        }

        public ulong Id { get; set; }

        public uint BodyId { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string SubType { get; set; }

        public float? DistanceToArrival { get; set; }

        public float? SurfaceTemp { get; set; }

        public bool Landable { get; set; }

        public float Gravity { get; set; }

        public string VolcanismType { get; set; }

        public string AtmosphereType { get; set; }

        public List<ISystemBody> ParentList { get; set; }

        public ICollection<ISystemBody> Parents => this.ParentList;

        public List<ISystemBody> ChildrenList { get; set; }

        public ICollection<ISystemBody> Children => this.ChildrenList;
    }
}
