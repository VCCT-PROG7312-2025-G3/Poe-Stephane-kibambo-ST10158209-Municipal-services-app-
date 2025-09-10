using System;
using System.ComponentModel.DataAnnotations;

namespace MunicipalMvcApp.Models
{
    public class Issue
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required, StringLength(4000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AttachmentPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [StringLength(30)]
        public string Status { get; set; } = "Submitted";
    }
}

