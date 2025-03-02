using UnityEngine;
using static UnityModManagerNet.UnityModManager;

namespace EZ2FAI
{
    public class Settings : ModSettings
    {
        public bool DragEnabled = false;
        public string Username = "";
        public string ProfileImage = "";
        public Vector2 Position = new Vector2(0.16f, 0.1f);
        public Vector2 Scale = new Vector2(0.7f, 0.7f);
        public bool SongProgress = false;
        public bool IsCheckPoint = true;
        public float pixelsPerUnitMultiplier = 1f;
    }
}
