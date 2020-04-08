using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageApi.DTO
{
    public class MultipartImageDto
    {
        [Required]
        public IFormFile Image { get; set; }
    }
}
