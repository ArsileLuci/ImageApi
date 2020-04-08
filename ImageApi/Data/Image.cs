using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageApi.Data
{
    public class Image
    {
        public int Id { get; set; }
        public ImageFormat Format { get; set; }
        public byte[] ImageContent { get; set; }
    }
    public enum ImageFormat
    {
        bmp,
        jpeg,
        gif,
        tiff,
        png,
        unknown
    }
}
