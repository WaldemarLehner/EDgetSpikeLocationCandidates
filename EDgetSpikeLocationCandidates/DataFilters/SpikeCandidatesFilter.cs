using System.Linq;

namespace EDgetSpikeLocationCandidates.DataFilters
{
    public static class SpikeCandidatesFilter
    {
        public static IStarSystem SpikeFilter(IStarSystem system)
        {
            var distanceFilter = (from body in system.Bodies where body.DistanceToArrival != null && body.DistanceToArrival > 12000 select body).ToList();
            if (distanceFilter.Count == 0)
            {
                return null;
            }

            var landableFilter = (from body in distanceFilter where body.Landable select body).ToList();
            if (landableFilter.Count == 0)
            {
                return null;
            }

            var minorVolcanismActivity = (from body in landableFilter where body.VolcanismType.ToLower().Contains("minor") select body).ToList();
            if (minorVolcanismActivity.Count == 0)
            {
                return null;
            }

            var singleParent = (from body in minorVolcanismActivity where body.Parents.Count == 1 select body).ToList();
            if (singleParent.Count == 0)
            {
                return null;
            }

            var parentIsGasGiantWithLife = (from body in singleParent where body.Parents.First().SubType.ToLower().Contains("gas giant") && body.Parents.First().SubType.ToLower().Contains("life") select body).ToList();
            system.Bodies.Clear();
            parentIsGasGiantWithLife.ForEach(e => system.Bodies.Add(e));

            return system;
        }
    }
}
