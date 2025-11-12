using Microsoft.AspNetCore.Mvc;
using Municipal_services_app.Models;
using Municipal_services_app.Services;
using MunicipalMvcApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Municipal_services_app.Controllers
{
    public class EventController : Controller
    {
        private readonly EventStore _store;
        private readonly AppDbContext _db;

        public EventController(EventStore store, AppDbContext db)
        {
            _store = store;
            _db = db;
        }

        // GET /Event/Load
        public IActionResult Load(string? q, string? category, DateTime? from, DateTime? to)
        {
            if (category == "All") category = null;

            var results = _store.Search(q, category, from, to);

            // Build recommendations from persisted popular search terms (DB-backed)
            var recs = new List<Event>();
            try
            {
                var topTerms = _store.GetTopSearchTermsFromDb(3); // top 3 persisted terms
                foreach (var term in topTerms)
                {
                    // for each popular term, take up to 3 matching events
                    recs.AddRange(_store.Search(term).Take(3));
                    if (recs.Count >= 5) break;
                }
            }
            catch
            {
                // fallback to in-memory recommend if DB read fails
                recs = _store.Recommend(5);
            }

            // ensure distinct and sorted
            recs = recs.Distinct().OrderBy(e => e.Date).Take(5).ToList();

            var announcements = _db.Announcements
                                   .OrderByDescending(a => a.DatePosted)
                                   .ToList();

            var model = new EventsIndexViewModel
            {
                Query = q,
                SelectedCategory = category,
                From = from,
                To = to,
                Events = results,
                Recommendations = recs,
                Categories = _store.Categories.ToList(),
                Announcements = announcements
            };

            return View(model);
        }

        // POST /Event/RecordSearch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RecordSearch([FromForm] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest();

            try
            {
                _store.RecordSearchAndPersist(term.Trim());
                return Ok();
            }
            catch
            {
                // silent failure: don't break UI if DB or logic fails
                return StatusCode(500);
            }
        }
    }
}