using UnityEngine;

namespace NiftyFramework.Core
{
    public class SpritePreviewAttribute : PropertyAttribute
    {
        public int Height { get; }

        public SpritePreviewAttribute()
        {
            Height = 32;
        }

        public SpritePreviewAttribute(int height)
        {
            Height = height;
        }
    }
}