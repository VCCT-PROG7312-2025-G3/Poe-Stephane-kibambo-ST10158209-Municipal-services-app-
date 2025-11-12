using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Municipal_services_app.Services;
using MunicipalMvcApp.Data;
using MunicipalMvcApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace Municipal_services_app.Controllers
{
    public class RequestController : Controller
    {
        private readonly RequestStore _store;
        private readonly AppDbContext _db;

        public RequestController(RequestStore store, AppDbContext db)
        {
            _store = store;
            _db = db;
        }

        // GET: /Request/Status
        // optional query params: q (text), category, refId (highlight a specific request)
        public IActionResult Status(string? q, string? category, string? refId)
        {
            // Use in-memory indexes where possible, DB as source of truth for fresh reads
            List<Issue> reports;
            if (!string.IsNullOrEmpty(category))
            {
                reports = _store.GetByCategory(category);
            }
            else
            {
                reports = _store.GetAll();
            }

            if (!string.IsNullOrEmpty(q))
            {
                var lc = q.ToLowerInvariant();
                reports = reports.Where(r =>
                    (!string.IsNullOrEmpty(r.Description) && r.Description.ToLowerInvariant().Contains(lc))
                    || (!string.IsNullOrEmpty(r.Location) && r.Location.ToLowerInvariant().Contains(lc))
                    || (!string.IsNullOrEmpty(r.RequestRef) && r.RequestRef.ToLowerInvariant().Contains(lc))
                ).ToList();
            }

            var vm = new RequestStatusVm
            {
                Reports = reports,
                Categories = _store.Categories.ToList(),
                Query = q,
                SelectedCategory = category,
                HighlightRef = refId
            };

            return View(vm);
        }

        // GET: /Request/Details/{id}
        public IActionResult Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var issue = _store.FindByRef(id);
            if (issue == null) return NotFound();

            var neighbors = _store.GetNeighbors(id)
                                  .Select(rref => _store.FindByRef(rref))
                                  .Where(x => x != null)
                                  .ToList()!;

            var vm = new RequestDetailsVm
            {
                Report = issue,
                Related = neighbors
            };

            return View(vm);
        }

// --- Ajoute cette action dans la classe RequestController ---
[HttpGet]
    public IActionResult DownloadAttachment(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest();

        var issue = _store.FindByRef(id);
        if (issue == null || string.IsNullOrWhiteSpace(issue.AttachmentPath))
            return NotFound();

        // Normalise le chemin enregistré dans la DB
        var stored = issue.AttachmentPath.Replace('\\', '/').Trim();

        // Si la valeur est un chemin absolu (C:\...) utilisez-le directement,
        // sinon considérez-le relatif à la racine du projet.
        string physicalPath;
        if (Path.IsPathRooted(stored))
        {
            physicalPath = stored;
        }
        else
        {
            // Assure que les fichiers restent sous App_Data (sécurité)
            // On autorise soit "App_Data/..." soit "attachments/..." précédé par App_Data.
            var appRoot = Directory.GetCurrentDirectory();
            // si la DB contient déjà "App_Data/attachments/...", respectons-le
            if (stored.StartsWith("App_Data/", StringComparison.OrdinalIgnoreCase) ||
                stored.StartsWith("App_Data\\", StringComparison.OrdinalIgnoreCase))
            {
                physicalPath = Path.GetFullPath(Path.Combine(appRoot, stored));
            }
            else
            {
                // sinon on assume attachments sous App_Data/attachments
                physicalPath = Path.GetFullPath(Path.Combine(appRoot, "App_Data", stored));
            }
        }

        // Security: ensure the resolved full path is inside App_Data folder
        var appDataFull = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "App_Data")) + Path.DirectorySeparatorChar;
        if (!physicalPath.StartsWith(appDataFull, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid(); // tentative d'accès en dehors de App_Data
        }

        if (!System.IO.File.Exists(physicalPath))
            return NotFound();

        // Resolve content type
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(physicalPath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var fileName = Path.GetFileName(physicalPath);
        return PhysicalFile(physicalPath, contentType, fileName);
    }
}

    // --- Small ViewModels included here to avoid creating extra files for now ---
    public class RequestStatusVm
    {
        public List<Issue> Reports { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public string? Query { get; set; }
        public string? SelectedCategory { get; set; }
        public string? HighlightRef { get; set; }
    }

    public class RequestDetailsVm
    {
        public Issue? Report { get; set; }
        public List<Issue>? Related { get; set; }
    }
}