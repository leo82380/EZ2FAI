using System;
using System.Collections.Generic;

namespace EZ2FAI
{
    public static class Variables
    {
        public static readonly HitMargin[] HitMargins = (HitMargin[])Enum.GetValues(typeof(HitMargin));
        public static int CurrentCheckPoint;
        public static double TileBpm;
        public static double CurBpm;
        public static double RecKPS;
        public static double TileBpmWithoutPitch;
        public static double CurBpmWithoutPitch;
        public static double RecKPSWithoutPitch;
        public static void Reset()
        {
            TileBpm = CurBpm = RecKPS = 0;
            TileBpmWithoutPitch = CurBpmWithoutPitch = RecKPSWithoutPitch = 0;
        }
    }
}
