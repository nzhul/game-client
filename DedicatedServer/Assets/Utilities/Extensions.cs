using UnityEngine.SocialPlatforms;

namespace Assets.Utilities
{
    public static class Extensions
    {
        public static bool Overlap(this Range first, Range second)
        {
            // x1 <= y2 && y1 <= x2
            return first.from <= second.count && second.from <= first.count;
        }
    }
}
