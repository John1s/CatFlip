using System.Drawing;
using System.Drawing.Imaging;

namespace Catflip.Api.Transformations
{
    public class TransparencyTransformation : ITransformation
    {
        public static string QueryKey = "alpha";
        float Alpha { get; }
        public TransparencyTransformation(float alpha)
        {
            Alpha = alpha/100;           
        }

        public Bitmap ApplyTransformation(Bitmap bitmap)
        {
            Bitmap newBitmap = new Bitmap(bitmap.Width, bitmap.Height);
            using (Graphics graphic = Graphics.FromImage(newBitmap))
            {
                ColorMatrix colorMatrix = new ColorMatrix
                {
                    Matrix33 = Alpha
                };
                ImageAttributes imageAttribute = new ImageAttributes();
                imageAttribute.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                graphic.DrawImage(bitmap, new Rectangle(0, 0, newBitmap.Width, newBitmap.Height), 0, 0, bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imageAttribute);
            }
            return newBitmap;
        }
    }
}
