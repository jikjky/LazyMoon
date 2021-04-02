using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyMoon.Data
{
    public class MyAppDbContext : IdentityDbContext
    {
        public MyAppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
