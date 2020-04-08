using ImageApi.Data;
using ImageApi.DTO;
using ImageApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ImageApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly ImageService _imgService;

        public ImagesController(ILogger<ImagesController> logger, ImageService imgService)
        {
            this._logger = logger;
            this._imgService = imgService;
        }

        [HttpPost("AddBase64")]
        public async Task<IActionResult> AddBase64([FromBody]ImageBase64Dto model)
        {
            if (model.Base64 == null)
            {
                return this.BadRequest("Field Base64 cannot be null");
            }
            try
            {
                Image img = await this._imgService.GetImageFromBase64(model.Base64);
                if (img == null)
                {
                    return this.BadRequest("Image has unknown format, supported formats JPEG, PNG, BMP, TIFF");
                }
                return this.Ok(new AddImageResponseDto() { Id = img.Id });
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, ex.Message);
            }
        }

        [HttpPost("AddFromUrl")]
        public async Task<IActionResult> AddImageFromUrl([FromBody]ImageUrlDto imageUrl)
        {
            if (imageUrl.Url == null)
            {
                return this.BadRequest("Url cannot be null");
            }
            if (!Uri.TryCreate(imageUrl.Url, UriKind.Absolute, out Uri uri))
            {
                return this.BadRequest("Url was incorrect");
            }

            try
            {
                Image img = await this._imgService.GetImageFromUrlAsync(uri);
                if (img == null)
                {
                    return this.BadRequest("Image has unknown format, supported formats JPEG, PNG, BMP, TIFF");
                }
                return this.Ok(new AddImageResponseDto() { Id = img.Id });
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, ex);
            }

        }

        [HttpPost("AddMultipart")]
        public async Task<IActionResult> AddMultipart([FromForm]MultipartImageDto image)
        {
            if (image.Image == null)
            {
                return this.BadRequest("Image cannot be null");
            }

            try
            {
                Image img = await this._imgService.GetImageFromMultipartFile(image.Image);
                if (img == null)
                {
                    return this.BadRequest("Image has unknown format, supported formats JPEG, PNG, BMP, TIFF");
                }
                return this.Ok(new AddImageResponseDto() { Id = img.Id });
            }
            catch (Exception ex)
            {
                return this.StatusCode(500, ex);
            }
        }

        [HttpPost("LoadImages")]
        public async Task<IActionResult> LoadMultipleFiles([FromForm]MultipleFilesDto images)
        {
            List<AddImageResponseDto> response = new List<AddImageResponseDto>();
            foreach (IFormFile file in images.Files)
            {
                try
                {
                    var img = await this._imgService.GetImageFromMultipartFile(file);
                    if (img == null)
                    {
                        response.Add(null);
                    }
                    else
                    {
                        response.Add(new AddImageResponseDto() { Id = img.Id });
                    }

                }
                catch (Exception ex)
                {
                    return this.StatusCode(500, ex);
                }
            }
            return Ok(response);

        }

        [HttpGet("Preview/{id}")]
        public async Task<IActionResult> GetPreview(int id)
        {
            if (id < 0)
            {
                return this.NotFound();
            }

            byte[] bytes = await this._imgService.GetImagePreview(id);
            if (bytes == null)
            {
                return this.BadRequest("Preview for this Image is not avaliable");
            }
            return this.Ok(bytes);
        }

        [HttpGet("Image/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {

            if (id < 0)
            {
                return this.NotFound();
            }

            byte[] bytes = await this._imgService.GetImage(id);
            if (bytes == null)
            {
                return this.BadRequest("Preview for this Image is not avaliable");
            }
            return this.Ok(bytes);
        }
    }
}
