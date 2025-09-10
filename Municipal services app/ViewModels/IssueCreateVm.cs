using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MunicipalMvcApp.ViewModels
{
    public class IssueCreateVm
    {
        [Required, StringLength(200)] public string Location { get; set; } = string.Empty;
        [Required, StringLength(100)] public string Category { get; set; } = string.Empty;
        [Required, StringLength(4000)] public string Description { get; set; } = string.Empty;
        public IFormFile? Attachment { get; set; }
    }
}
