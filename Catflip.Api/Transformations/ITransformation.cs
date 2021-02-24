using System.Drawing;

namespace Catflip.Api.Transformations
{
    public interface ITransformation
    {
        Bitmap ApplyTransformation(Bitmap bitmap);
    }
}
