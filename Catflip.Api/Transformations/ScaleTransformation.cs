using System;
using System.Drawing;


namespace Catflip.Api.Transformations
{
    public class ScaleTransformation : ITransformation
    {
        public static string QueryKey = "scale";

        public float Fraction { get; }
        public ScaleTransformation(float scalePercentage)
        {
            if(scalePercentage < 10 || scalePercentage > 200)
            {
                throw new ArgumentException("Scale should be between 10 and 200 percent.");
            }
            Fraction = scalePercentage / 100;
        }

        public Bitmap ApplyTransformation(Bitmap bitmap)
        {
            var newHeight = (int)(bitmap.Height * Fraction);
            var newWidth = (int)(bitmap.Width * Fraction);
            return new Bitmap(bitmap, new Size(newWidth, newHeight));
        }
    }
}
