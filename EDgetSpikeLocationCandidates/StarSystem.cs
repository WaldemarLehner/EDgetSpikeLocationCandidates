using System;
using System.Collections.Generic;
using System.Text;

namespace EDgetSpikeLocationCandidates
{
    public class StarSystem : IStarSystem
    {
        public StarSystem()
        {
            this.BodyList = new List<ISystemBody>();
        }

        public (float x, float y, float z) Coordinates { get; set; }

        public string SystemName { get; set; }

        public ulong Id { get; set; }

        public ICollection<ISystemBody> Bodies => this.BodyList;

        public List<ISystemBody> BodyList { get; set; }
    }
}
