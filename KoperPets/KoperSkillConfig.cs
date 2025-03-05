using System;
using Server;

namespace Server.Custom.KoperPets
{
    public static class KoperSkillConfig
    {
        private static double _tamingChanceMultiplier = 1.0; // Default 1.0 (normal gain)
        private static double _herdingChanceMultiplier = 1.0; // Default 1.0 (normal gain)

        [CommandProperty(AccessLevel.GameMaster)]
        public static double TamingChanceMultiplier
        {
            get { return _tamingChanceMultiplier; }
            set { _tamingChanceMultiplier = Math.Max(1.0, Math.Min(value, 10.0)); } // Ensure value is between 1.0 and 10.0
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public static double HerdingChanceMultiplier
        {
            get { return _herdingChanceMultiplier; }
            set { _herdingChanceMultiplier = Math.Max(1.0, Math.Min(value, 10.0)); } //  Ensure value is between 1.0 and 10.0
        }
    }
}
