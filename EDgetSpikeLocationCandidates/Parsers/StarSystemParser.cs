using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace EDgetSpikeLocationCandidates.Parsers
{
    public static class StarSystemParser
    {
        public static IStarSystem ParseStarSystem(string data)
        {
            dynamic sysJson = JObject.Parse(data);
            (float, float, float) coords = ((float)sysJson.coords.x, (float)sysJson.coords.y, (float)sysJson.coords.z);

            var system = new StarSystem()
            {
                Coordinates = coords,
                SystemName = sysJson.name,
                Id = (ulong)sysJson.id64,
            };

            // typeof Array
            var bodies = sysJson.bodies;
            List<ISystemBody> systemBodies = new List<ISystemBody>(bodies.Count);
            Dictionary<string, List<ISystemBody>> bodyIdentifiersAndChildren = new Dictionary<string, List<ISystemBody>>();
            foreach (var b in bodies)
            {
                var body = new SystemBody()
                {
                    Id = (ulong)b.id64,
                    BodyId = (uint)b.bodyId,
                    Name = b.name ?? string.Empty,
                    Type = b.type ?? string.Empty,
                    SubType = b.subType ?? string.Empty,
                    DistanceToArrival = (float?)b.distanceToArrival ?? -1f,
                    VolcanismType = b.volcanismType ?? "No volcanism",
                    AtmosphereType = b.atmosphereType ?? "No atmosphere",
                    SurfaceTemp = (float?)b.surfaceTemperature,
                    Gravity = (float?)b.gravity ?? -1,
                    Landable = (bool?)b.isLandable ?? false,
                };

                foreach (var entry in GetParentIdentifiers(GetOwnIdentier(system.SystemName, body.Name)))
                {
                    if (!bodyIdentifiersAndChildren.ContainsKey(entry))
                    {
                        bodyIdentifiersAndChildren[entry] = new List<ISystemBody>();
                    }

                    bodyIdentifiersAndChildren[entry].Add(body);
                }

                systemBodies.Add(body);
            }

            // Link parents / children
            foreach (var keys in bodyIdentifiersAndChildren.Keys)
            {
                foreach (var body in bodyIdentifiersAndChildren[keys])
                {
                    var identifier = GetOwnIdentier(system.SystemName, body.Name);
                    if (!bodyIdentifiersAndChildren.ContainsKey(identifier))
                    {
                        // No children
                        continue;
                    }

                    foreach (var child in bodyIdentifiersAndChildren[identifier])
                    {
                        child.Parents.Add(body);
                        body.Children.Add(child);
                    }
                }
            }
            system.BodyList = systemBodies;
            return system;
        }

        public static string GetOwnIdentier(string systemName, string bodyName)
        {
            if (!bodyName.Contains(systemName.Trim()))
            {
                return null; // Body has a special name that does not correspond to the regular naming convention.
            }

            return bodyName.Trim().Substring(systemName.Length).TrimStart();
        }

        public static IEnumerable<string> GetParentIdentifiers(string ownIdent)
        {
            if (ownIdent == null)
            {
                return new string[0];
            }

            List<string> identifiers = ownIdent.Split(" ").ToList();
            if (identifiers.Count == 1)
            {
                // There is no parent
                return new string[] { string.Empty };
            }

            // Remove the object itself from the List
            identifiers.RemoveAt(identifiers.Count - 1);
            string[] parentNodes = SplitInto1LongStrings(identifiers[identifiers.Count - 1]);

            // Remove the parent, as it has already been extracted.
            identifiers.RemoveAt(identifiers.Count - 1);
            string identifierBeforeParent = string.Join(" ", identifiers);
            string[] returnValues = new string[parentNodes.Length];

            for (int i = 0; i < parentNodes.Length; i++)
            {
                returnValues[i] = (identifierBeforeParent.TrimEnd() + " " + parentNodes[i]).Trim();
            }

            return returnValues;
        }

        private static string[] SplitInto1LongStrings(string v)
        {
            List<string> retList = new List<string>(v.Length);
            for (int i = 0; i < v.Length; i++)
            {
                retList.Add(v.Substring(i, 1));
            }

            return retList.ToArray();
        }
    }
}
