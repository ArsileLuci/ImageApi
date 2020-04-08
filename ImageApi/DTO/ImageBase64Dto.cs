using System;
using System.ComponentModel.DataAnnotations;

namespace ImageApi
{
    public class ImageBase64Dto
    {
        [Required]
        public string Base64 { get; set; }
    }
}
