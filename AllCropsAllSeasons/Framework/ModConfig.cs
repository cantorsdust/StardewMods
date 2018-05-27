using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCropsAllSeasons.Framework
{
    class ModConfig
    {
        public bool WinterHoeSnow { get; set; } = false;
        public bool WaterCropSnow { get; set; } = true;
        public bool CropGrowSpring { get; set; } = true;
        public bool CropGrowSummer { get; set; } = true;
        public bool CropGrowFall { get; set; } = true;
        public bool CropGrowWinter { get; set; } = true;
    }
}
