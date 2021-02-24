using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Catflip.Api.Transformations
{
    public interface ITransformationFactory
    {
        ICollection<ITransformation> Transformations();
        ICollection<ITransformation> Transformations(float? alpha, string text, float? scale);
    }

    public class TransformationFactory: ITransformationFactory
    {
        IHttpContextAccessor HttpContextAccessor { get; }
        public TransformationFactory(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public ICollection<ITransformation> Transformations()
        {
            var request = HttpContextAccessor.HttpContext.Request;
            var transformations = new List<ITransformation>();

            if (request.Query.TryGetValue(FlipTransformation.QueryKey, out StringValues flip))
            {
                if(bool.TryParse(flip, out bool shouldFlip) && shouldFlip)
                {
                    transformations.Add(new FlipTransformation());
                }
            }

            if (request.Query.TryGetValue(TransparencyTransformation.QueryKey, out StringValues alphaText))
            {
                if (float.TryParse(alphaText, out float alpha))
                {
                    transformations.Add(new TransparencyTransformation(alpha));
                }
            }

            if (request.Query.TryGetValue(AddTextTransformation.QueryKey, out StringValues text))
            {
                transformations.Add(new AddTextTransformation(text));
            }

            if (request.Query.TryGetValue(ScaleTransformation.QueryKey, out StringValues scaleText))
            {
                if (float.TryParse(scaleText, out float scale))
                {
                    transformations.Add(new ScaleTransformation(scale));
                }
            }
            return transformations;
        }

        public ICollection<ITransformation> Transformations(float? alpha, string text, float? scale)
        {
            var request = HttpContextAccessor.HttpContext.Request;
            var transformations = new List<ITransformation>();

            transformations.Add(new FlipTransformation());
            if (alpha.HasValue)
            {
                transformations.Add(new TransparencyTransformation(alpha.Value));
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                transformations.Add(new AddTextTransformation(text));
            }

            if (scale.HasValue)
            {
                transformations.Add(new ScaleTransformation(scale.Value));
            }
            return transformations;
        }
    }
}
