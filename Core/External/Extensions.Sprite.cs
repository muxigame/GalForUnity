using UnityEngine;

namespace GalForUnity.Core.External
{
    public static partial class Extension
    {
        internal static Vector2 GetPixelPivot(this Sprite sprite)
        {
            return sprite.pivot;
        }

        internal static Vector2 GetPointPivot(this Sprite sprite)
        {
            var spritePivot = sprite.pivot;
            spritePivot.x /= sprite.rect.width;
            spritePivot.y /= sprite.rect.height;
            return spritePivot;
        }

    }
}