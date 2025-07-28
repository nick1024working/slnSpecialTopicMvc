using Microsoft.AspNetCore.Mvc;
using prjSpecialTopicMvc.Features.Usedbook.Application.Services;
using prjSpecialTopicMvc.Features.Usedbook.Extends;
using prjSpecialTopicMvc.Features.Usedbook.Utilities;

namespace prjBookAppCoreMVC.Controllers.UsedBook
{
    [ApiController]
    [Route("api/image")]
    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageController(
            ImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet]
        public IActionResult GetImageList()
        {
            var queryResult = _imageService.GetImageList();
            return Ok(queryResult);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteImage(string id)
        {
            _imageService.DeleteImage(id);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateImage([FromForm] IFormFile file, CancellationToken ct)
        {
            var queryResult = await _imageService.SaveImageAsync(file, Request, ct);
            if (!queryResult.IsSuccess)
                return BadRequest(queryResult.ErrorMessage);
            return Ok(queryResult);
        }

        [HttpPost("batch")]
        public async Task<IActionResult> CreateImages([FromForm] List<IFormFile> files, CancellationToken ct)
        {
            var result = await _imageService.SaveImagesAsync(files, Request, ct);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public IActionResult GetMainFileById(string id)
        {
            var filePath = _imageService.GetMainAbsolutePath(id);
            if (filePath is null) return NotFound();

            var contentType = FileHelper.GetContentType(filePath);
            return PhysicalFile(filePath, contentType);
        }

        [HttpGet("{id}/main-url")]
        public IActionResult GetMainUrlById(string id)
        {
            var relativePath = _imageService.GetMainRelativePath(id);
            if (string.IsNullOrEmpty(relativePath))
                return NotFound();

            var filePath = "/" + relativePath.Replace("\\", "/");
            var baseUrl = Request.GetBaseUrl();

            return Ok($"{baseUrl}{filePath}");
        }

        [HttpGet("{id}/thumb")]
        public IActionResult GetThumbFileById(string id)
        {
            var filePath = _imageService.GetThumbAbsolutePath(id);
            if (filePath is null) return NotFound();

            var contentType = FileHelper.GetContentType(filePath);
            return PhysicalFile(filePath, contentType);
        }

        [HttpGet("{id}/thumb-url")]
        public IActionResult GetThumbUrlById(string id)
        {
            var relativePath = _imageService.GetThumbRelativePath(id);
            if (string.IsNullOrEmpty(relativePath))
                return NotFound();

            var filePath = "/" + relativePath.Replace("\\", "/");
            var baseUrl = Request.GetBaseUrl();

            return Ok($"{baseUrl}{filePath}");
        }
    }
    
}
