using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Municipal_services_app.Services;
using MunicipalMvcApp.Data;
using MunicipalMvcApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Municipal_services_app.Controllers
{
    // Simple admin controller to manage request statuses.
    // NOTE: add authorization later if you want to protect these endpoints.
    [Authorize(Roles = "Admin")]
    public class AdminRequestsController : Controller
    {
        private readonly RequestStore _store;
        private readonly AppDbContext _db;

        public AdminRequestsController(RequestStore store, AppDbContext db)
        {
            _store = store;
            _db = db;
        }

        // GET: /AdminRequests
        // shows all requests (paginated/simple)
        public IActionResult Index(string? q, string? category)
        {
            // Use in-memory index for categories, DB for source-of-truth
            var list = _store.GetAll();

            if (!string.IsNullOrWhiteSpace(category))
                list = list.Where(i => string.Equals(i.Category, category, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var lc = q.ToLowerInvariant();
                list = list.Where(i =>
                    (!string.IsNullOrEmpty(i.Description) && i.Description.ToLowerInvariant().Contains(lc)) ||
                    (!string.IsNullOrEmpty(i.Location) && i.Location.ToLowerInvariant().Contains(lc)) ||
                    (!string.IsNullOrEmpty(i.RequestRef) && i.RequestRef.ToLowerInvariant().Contains(lc))
                ).ToList();
            }

            var vm = new AdminIndexVm
            {
                Requests = list,
                Categories = _store.Categories.ToList(),
                Query = q,
                SelectedCategory = category
            };

            return View(vm);
        }

        // GET: /AdminRequests/Edit/{ref}
        public IActionResult Edit(string id) // id = RequestRef
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var issue = _store.FindByRef(id);
            if (issue == null) return NotFound();

            var vm = new AdminEditVm
            {
                RequestRef = issue.RequestRef,
                CurrentStatus = issue.Status,
                AvailableStatuses = GetStatusList()
            };

            return View(vm);
        }

        // POST: /AdminRequests/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AdminEditVm model)
        {
            if (!ModelState.IsValid) return View(model);

            var ok = _store.UpdateStatus(model.RequestRef, model.NewStatus ?? model.CurrentStatus ?? "Submitted");
            if (!ok)
            {
                TempData["AdminMsg"] = "Update failed: request not found.";
                return RedirectToAction("Index");
            }

            TempData["AdminMsg"] = $"Status updated for {model.RequestRef} → {model.NewStatus}";
            return RedirectToAction("Index");
        }

        // Helper: a simple list of statuses (could be moved to config / enum)
        private static List<string> GetStatusList() => new List<string>
        {
            "Submitted",
            "Under Review",
            "In Progress",
            "Resolved",
            "Closed"
        };

        // --- Small view models for the controller (one-file convenience) ---
        public class AdminIndexVm
        {
            public List<Issue> Requests { get; set; } = new();
            public List<string> Categories { get; set; } = new();
            public string? Query { get; set; }
            public string? SelectedCategory { get; set; }
        }

        public class AdminEditVm
        {
            public string RequestRef { get; set; } = string.Empty;
            public string? CurrentStatus { get; set; }
            public string? NewStatus { get; set; }
            public List<string> AvailableStatuses { get; set; } = new();
        }
    }
}