using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageApi.DTO
{
    public class ImageUrlDto
    {
        [Required]
        public string Url { get; set; }
    }
}
