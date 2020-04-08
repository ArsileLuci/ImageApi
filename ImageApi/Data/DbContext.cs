using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ImageApi
{
    public class ImageDbContext : DbContext
    {
        public ImageDbContext() : base() { }
        public ImageDbContext(DbContextOptions options) : base(options) { }
        public DbSet<ImageApi.Data.Image> Images { get; set; }
    }
}
