using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ApiTest
{
    public class Tests
    {
        private IServiceProvider serviceProvider;
        [SetUp]
        public void Setup()
        {
            this.serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddTransient<ImageApi.Services.ImageService>()
                .AddDbContext<ImageApi.ImageDbContext>((options) => { options.UseInMemoryDatabase("imageApi"); })
                .BuildServiceProvider()
                .CreateScope()
                .ServiceProvider;
        }
        /// <summary>
        /// Тест на загрузку файла как multipart\form-data 
        /// </summary>
        /// <returns></returns>
        [Test, Order(1)]
        public async Task TestLoadMultipart()
        {

            ImageApi.Services.ImageService service = this.serviceProvider.GetService<ImageApi.Services.ImageService>();

            FileStream fs = File.OpenRead("test1.png");
            ImageApi.Data.Image result = await service.GetImageFromMultipartFile(new FormFile(fs, 0, fs.Length, "test1", "test1.png"));

            Assert.AreEqual(result.Format, ImageApi.Data.ImageFormat.png);
            Assert.AreEqual(result.Id, 1);
            Assert.AreEqual(result.ImageContent, File.ReadAllBytes("test1.png"));
        }
        /// <summary>
        /// Тест загрузки файла по адресу
        /// </summary>
        /// <returns></returns>
        [Test, Order(2)]
        public async Task TestLoadFromUrl()
        {

            ImageApi.Services.ImageService service = this.serviceProvider.GetService<ImageApi.Services.ImageService>();

            Uri uri = new Uri(@"https://media.discordapp.net/attachments/617725195691491349/692406625381646496/yin14x1vbuo41.png");
            ImageApi.Data.Image result = await service.GetImageFromUrlAsync(uri);
            byte[] bytes;
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                using (Stream stream = wc.OpenRead(uri))
                {
                    byte[] buffer = new byte[16 * 1024];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        int read;
                        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ms.Write(buffer, 0, read);
                        }
                        bytes = ms.ToArray();
                    }
                }
            }

            Assert.AreEqual(result.Format, ImageApi.Data.ImageFormat.png);
            Assert.AreEqual(result.Id, 2);
            Assert.AreEqual(result.ImageContent, bytes);
        }
        /// <summary>
        /// Тест на загрузку файла в виде base64
        /// </summary>
        /// <returns></returns>
        [Test, Order(3)]
        public async Task TestLoadBase64()
        {
            ImageApi.Services.ImageService service = this.serviceProvider.GetService<ImageApi.Services.ImageService>();

            byte[] bytes;
            using (FileStream fs = File.OpenRead("test1.png"))
            {
                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    bytes = ms.ToArray();
                }
            }
            ImageApi.Data.Image result = await service.GetImageFromBase64(Convert.ToBase64String(bytes));

            Assert.AreEqual(result.Format, ImageApi.Data.ImageFormat.png);
            Assert.AreEqual(result.Id, 3);
            Assert.AreEqual(result.ImageContent, bytes);
        }
        /// <summary>
        /// Тест на загрузку файлов в формате bmp
        /// </summary>
        /// <returns></returns>
        [Test, Order(4)]
        public async Task TestLoadMultipart_bmp()
        {

            ImageApi.Services.ImageService service = this.serviceProvider.GetService<ImageApi.Services.ImageService>();

            FileStream fs = File.OpenRead("test2.bmp");
            ImageApi.Data.Image result = await service.GetImageFromMultipartFile(new FormFile(fs, 0, fs.Length, "test2", "test2.bmp"));

            Assert.AreEqual(result.Format, ImageApi.Data.ImageFormat.bmp);
            Assert.AreEqual(result.Id, 4);
            Assert.AreEqual(result.ImageContent, File.ReadAllBytes("test2.bmp"));
        }
        /// <summary>
        /// Тест на загрузку файлов в формате jpeg/jpg
        /// </summary>
        /// <returns></returns>
        [Test, Order(5)]
        public async Task TestLoadMultipart_jpeg()
        {

            ImageApi.Services.ImageService service = this.serviceProvider.GetService<ImageApi.Services.ImageService>();

            FileStream fs = File.OpenRead("test3.jpg");
            ImageApi.Data.Image result = await service.GetImageFromMultipartFile(new FormFile(fs, 0, fs.Length, "test3", "test3.jpg"));

            Assert.AreEqual(result.Format, ImageApi.Data.ImageFormat.jpeg);
            Assert.AreEqual(result.Id, 5);
            Assert.AreEqual(result.ImageContent, File.ReadAllBytes("test3.jpg"));
        }
        /// <summary>
        /// Тест на пустой ответ сервиса в случае добавления файла не являющегося изображением
        /// </summary>
        /// <returns></returns>
        [Test, Order(6)]
        public async Task TestLoadNotImageFile()
        {
            ImageApi.Services.ImageService service = this.serviceProvider.GetService<ImageApi.Services.ImageService>();

            FileStream fs = File.OpenRead("NotImage.txt");
            ImageApi.Data.Image result = await service.GetImageFromMultipartFile(new FormFile(fs, 0, fs.Length, "NotImage", "NotImage.txt"));

            Assert.AreEqual(result, null);
        }
        /// <summary>
        /// Тест на соответствие размеров превью
        /// </summary>
        /// <returns></returns>
        [Test, Order(7)]
        public async Task TestGetPreviewSize()
        {
            ImageApi.Services.ImageService service = this.serviceProvider.GetService<ImageApi.Services.ImageService>();

            var image = await service.GetImagePreview(1);
            ImageMagick.MagickImage img = new ImageMagick.MagickImage(image);
            Assert.AreEqual(img.Height, ImageApi.Services.ImageService.PreviewHeight);
            Assert.AreEqual(img.Width, ImageApi.Services.ImageService.PreviewWidth);
            Assert.AreEqual(img.Width, img.Height);
        }


        /// <summary>
        /// Тест на пустой ответ сервиса в случае отсутствия изображение по заданному индексу
        /// </summary>
        /// <returns></returns>
        [Test, Order(8)]
        public async Task NullOnWrongId()
        {
            ImageApi.Services.ImageService service = this.serviceProvider.GetService<ImageApi.Services.ImageService>();
            var result = await service.GetImagePreview(1000);
            Assert.AreEqual(result, null);
        }
    }
}