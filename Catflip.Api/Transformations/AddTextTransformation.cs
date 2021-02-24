using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Catflip.Api.Transformations
{
    public class AddTextTransformation : ITransformation
    {
        public static string QueryKey = "text";

        string Text { get; }
        public AddTextTransformation(string text)
        {
            Text = text;
        }

        public Bitmap ApplyTransformation(Bitmap bitmap)
        {
            var top = bitmap.Height - 100;
            RectangleF rectf = new RectangleF(0, top, bitmap.Width, 100);
            Graphics g = Graphics.FromImage(bitmap);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            StringFormat format = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            GraphicsPath p = new GraphicsPath();
            p.AddString(Text, FontFamily.GenericSansSerif, (int)FontStyle.Bold, 62, rectf, format);
            Pen blackPen = new Pen(Brushes.Black, 6);
            g.DrawPath(blackPen, p);
            g.FillPath(new SolidBrush(Color.White), p);
            g.Flush();
            return bitmap;
        }
    }
}
