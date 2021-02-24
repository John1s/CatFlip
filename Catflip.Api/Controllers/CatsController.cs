using Catflip.Api.Services;
using Catflip.Api.Transformations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Catflip.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class CatsController : ControllerBase
    {
        ICatService CatService { get; }
        ITransformationFactory TransformationFactory { get; }
        public CatsController(ICatService catService, ITransformationFactory transformationFactory)
        {
            CatService = catService;
            TransformationFactory = transformationFactory;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Cat([FromQuery]string tag, [FromQuery]string text, [FromQuery]float? alpha, [FromQuery]float? scale)
        {
            var bytes = await CatService.GetCat(tag);

            var inputStream = new MemoryStream(bytes);
            var outStream = new MemoryStream();
            var bitmap = new Bitmap(inputStream);

            var transformations = TransformationFactory.Transformations(alpha, text, scale);
            foreach(var transformation in transformations)
            {
                bitmap = transformation.ApplyTransformation(bitmap);
            }

            bitmap.Save(outStream, ImageFormat.Jpeg);
            outStream.Position = 0;
            return File(outStream, "image/jpg");
        }
    }
}
