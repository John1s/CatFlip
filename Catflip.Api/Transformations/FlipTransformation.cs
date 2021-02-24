using System;
using System.Drawing;

namespace Catflip.Api.Transformations
{
    public class FlipTransformation : ITransformation
    {
        public static string QueryKey = "flip";
        public Bitmap ApplyTransformation(Bitmap bitmap)
        {
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            return bitmap;
        }
    }
}
