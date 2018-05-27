using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCropsAllSeasons.Framework
{
    class ModConfig
    {
        public bool WinterAliveEnabled { get; set; } = true;
        public bool WinterHoeSnow { get; set; } = false;
        public bool WaterCropSnow { get; set; } = true;
    }
}
