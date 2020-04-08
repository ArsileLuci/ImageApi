using ImageApi.Data;
using ImageMagick;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Image = ImageApi.Data.Image;

namespace ImageApi.Services
{
    public class ImageService
    {
        public ImageService(ImageDbContext imageDbContext)
        {
            this._dbContext = imageDbContext;
        }
        private ImageDbContext _dbContext;

        public Task<Image> GetImageFromUrlAsync(Uri uri)
        {
            using (WebClient wc = new WebClient())
            {
                using (Stream input = wc.OpenRead(uri))
                {
                    return this.getImageFromStream(input);
                }
            }
        }

        public async Task<Image> GetImageFromBase64(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64.Substring(base64.LastIndexOf(',') + 1));
            ImageFormat format = GetImageFormat(bytes);
            if (format == ImageFormat.unknown)
            {
                return null;
            }


            Image img = new Image()
            {
                ImageContent = bytes,
                Format = format
            };

            await this._dbContext.Images.AddAsync(img);
            await this._dbContext.SaveChangesAsync();

            return img;
        }

        public Task<Image> GetImageFromMultipartFile(IFormFile formFile)
        {
            using (Stream input = formFile.OpenReadStream())
            {
                return this.getImageFromStream(input);
            }
        }

        private async Task<Image> getImageFromStream(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                byte[] bytes = ms.ToArray();
                ImageFormat format = GetImageFormat(bytes);
                if (format == ImageFormat.unknown)
                {
                    return null;
                }
                Image img = new Image()
                {
                    ImageContent = bytes,
                    Format = format
                };
                await this._dbContext.Images.AddAsync(img);
                await this._dbContext.SaveChangesAsync();
                return img;
            }
        }

        #region Headers
        private static byte[] _bmp = System.Text.Encoding.ASCII.GetBytes("BM");      // BMP
        private static byte[] _gif = System.Text.Encoding.ASCII.GetBytes("GIF");     // GIF
        private static byte[] _png = new byte[] { 137, 80, 78, 71 };                 // PNG
        private static byte[] _tiff = new byte[] { 73, 73, 42 };                     // TIFF
        private static byte[] _tiff2 = new byte[] { 77, 77, 42 };                    // TIFF
        private static byte[] _jpeg = new byte[] { 255, 216, 255, 224 };             // jpeg
        private static byte[] _jpeg2 = new byte[] { 255, 216, 255, 225 };            // jpeg canon
        #endregion

        public static ImageFormat GetImageFormat(byte[] bytes)
        {
            if (_bmp.SequenceEqual(bytes.Take(_bmp.Length)))
                return ImageFormat.bmp;

            if (_gif.SequenceEqual(bytes.Take(_gif.Length)))
                return ImageFormat.gif;

            if (_png.SequenceEqual(bytes.Take(_png.Length)))
                return ImageFormat.png;

            if (_tiff.SequenceEqual(bytes.Take(_tiff.Length)))
                return ImageFormat.tiff;

            if (_tiff2.SequenceEqual(bytes.Take(_tiff2.Length)))
                return ImageFormat.tiff;

            if (_jpeg.SequenceEqual(bytes.Take(_jpeg.Length)))
                return ImageFormat.jpeg;

            if (_jpeg2.SequenceEqual(bytes.Take(_jpeg2.Length)))
                return ImageFormat.jpeg;

            return ImageFormat.unknown;
        }

        public const int PreviewWidth = 100;
        public const int PreviewHeight = 100;
        public async Task<byte[]> GetImagePreview(int id)
        {
            Image img = await this._dbContext.Images.FindAsync(id);
            if (img == null)
            {
                return null;
            }
            using (MagickImage image = new MagickImage(img.ImageContent))
            {
                double ratio = ((double)image.Width) / ((double)image.Height);
                double widthRatio = (double)PreviewWidth / (double)image.Width;
                double heightRatio = (double)PreviewHeight / (double)image.Height;
                double scaleratio = Math.Max(widthRatio, heightRatio);
                if (scaleratio != 1d)
                {
                    image.Resize(new Percentage(100 * scaleratio));
                }
                image.Crop(PreviewWidth, PreviewHeight, Gravity.Center);
                return image.ToByteArray();
            }
        }
        internal async Task<byte[]> GetImage(int id)
        {
            Image img = await this._dbContext.Images.FindAsync(id);
            return img?.ImageContent;
        }
    }
}
