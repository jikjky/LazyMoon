using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LazyMoon.Model
{
    public class ConnectionHistory
    {        
        [Required]
        [NotNull]
        public int Id { get; set; }
        public string Date { get; set; }
        public int Count { get; set; }
    }
}
