using System;
using System.ComponentModel.DataAnnotations;

namespace MunicipalMvcApp.Models
{
    public class Recommendation
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Term { get; set; } = string.Empty;

        public int Count { get; set; } = 0;

        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    }
}