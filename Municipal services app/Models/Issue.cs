using System;
using System.ComponentModel.DataAnnotations;

namespace MunicipalMvcApp.Models
{
    public class Issue
    {
        public int Id { get; set; }

        // Friendly reference code that users can use to track their request
        [Required, StringLength(12)]
        public string RequestRef { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

        [Required, StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required, StringLength(4000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string? AttachmentPath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // keep existing Status as string for backwards compatibility
        [StringLength(30)]
        public string Status { get; set; } = "Submitted";

        // New: track last update time when status (or issue) changes
        public DateTime? DateUpdated { get; set; }

        // New: simple text history of status changes (timestamped)
        // Format example lines: "2025-10-23T12:34:56Z | Submitted -> UnderReview"
        public string? StatusHistory { get; set; }

        // Optional: reporter name (nullable so existing flows not broken)
        [StringLength(200)]
        public string? ReporterName { get; set; }
    }
}