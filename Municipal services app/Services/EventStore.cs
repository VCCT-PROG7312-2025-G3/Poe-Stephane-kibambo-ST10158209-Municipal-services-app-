
    using Microsoft.EntityFrameworkCore;
    using Municipal_services_app.Models;
    using MunicipalMvcApp.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    namespace Municipal_services_app.Services
{
        public class EventStore : IDisposable
        {
            private readonly AppDbContext _db;
            // required structures
            private readonly SortedDictionary<DateTime, List<Event>> byDate = new();
            private readonly Dictionary<string, List<Event>> byCategory = new(StringComparer.OrdinalIgnoreCase);
            private readonly HashSet<string> categories = new(StringComparer.OrdinalIgnoreCase);
            private readonly PriorityQueue<Event, DateTime> upcomingQueue = new();
            private readonly Queue<string> recentSearches = new();
            private readonly Dictionary<string, int> searchCounts = new(StringComparer.OrdinalIgnoreCase);
            private readonly Stack<Action> undoStack = new();

            public EventStore(AppDbContext db)
            {
                _db = db;
                LoadFromDatabase();
            }

            public void LoadFromDatabase()
            {
                // clear
                byDate.Clear();
                byCategory.Clear();
                categories.Clear();
                while (upcomingQueue.Count > 0) upcomingQueue.Dequeue();
                searchCounts.Clear();
                while (recentSearches.Count > 0) recentSearches.Dequeue();
                while (undoStack.Count > 0) undoStack.Pop();

                var events = _db.Events.AsNoTracking().ToList();
                foreach (var ev in events)
                {
                    AddToIndexes(ev);
                }
            }

            private void AddToIndexes(Event ev)
            {
                var key = ev.Date.Date;
                if (!byDate.TryGetValue(key, out var list)) { list = new(); byDate[key] = list; }
                list.Add(ev);

                if (!byCategory.TryGetValue(ev.Category, out var catList)) { catList = new(); byCategory[ev.Category] = catList; }
                catList.Add(ev);

                categories.Add(ev.Category);
                upcomingQueue.Enqueue(ev, ev.Date);
                // undo sample: push DB delete action (not executed now)
                undoStack.Push(() => {
                    // This action only demonstrates stack usage; don't call here.
                });
            }

            // Public read-only access
            public IEnumerable<string> Categories => categories.OrderBy(c => c);

            // Search
            public List<Event> Search(string? text = null, string? category = null, DateTime? from = null, DateTime? to = null)
            {
                text = text?.Trim();
                IEnumerable<Event> candidates;

                if (!string.IsNullOrEmpty(category) && byCategory.TryGetValue(category, out var catList))
                    candidates = catList;
                else
                    candidates = byDate.Values.SelectMany(x => x);

                if (from.HasValue || to.HasValue)
                {
                    var low = from?.Date ?? DateTime.MinValue.Date;
                    var high = to?.Date ?? DateTime.MaxValue.Date;
                    candidates = byDate.Where(kv => kv.Key >= low && kv.Key <= high).SelectMany(kv => kv.Value)
                                       .Where(e => candidates.Contains(e));
                }

                if (!string.IsNullOrEmpty(text))
                {
                    var t = text.ToLowerInvariant();
                    candidates = candidates.Where(e => e.Title.ToLowerInvariant().Contains(t)
                                                    || e.Description.ToLowerInvariant().Contains(t)
                                                    || e.Location.ToLowerInvariant().Contains(t)
                                                    || e.Organizer.ToLowerInvariant().Contains(t));
                    RecordSearch(text);
                }

                return candidates.OrderBy(e => e.Date).ToList();
            }

            private void RecordSearch(string term)
            {
                if (string.IsNullOrWhiteSpace(term)) return;
                if (recentSearches.Count >= 20) recentSearches.Dequeue();
                recentSearches.Enqueue(term);

                if (!searchCounts.ContainsKey(term)) searchCounts[term] = 0;
                searchCounts[term]++;
                if (categories.Contains(term))
                {
                    if (!searchCounts.ContainsKey(term)) searchCounts[term] = 0;
                    searchCounts[term]++;
                }
            }
        // --- Persistence helpers for recommendations (paste inside EventStore class) ---

        /// <summary>
        /// Record a search term in-memory and persist it to the DB so recommendations survive restarts.
        /// Call this from controllers when a user performs a search (debounced on client side).
        /// </summary>
        public void RecordSearchAndPersist(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return;

            // update in-memory structures (keeps current behavior)
            RecordSearch(term);

            // persist/update in DB
            SaveSearchTermToDb(term);
        }

        /// <summary>
        /// Save or update a Recommendation row for the given term.
        /// </summary>
        private void SaveSearchTermToDb(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return;

            try
            {
                // Try find existing recommendation
                var rec = _db.Recommendations.FirstOrDefault(r => r.Term == term);
                if (rec == null)
                {
                    rec = new MunicipalMvcApp.Models.Recommendation
                    {
                        Term = term,
                        Count = 1,
                        LastSeen = DateTime.UtcNow
                    };
                    _db.Recommendations.Add(rec);
                }
                else
                {
                    rec.Count++;
                    rec.LastSeen = DateTime.UtcNow;
                    _db.Recommendations.Update(rec);
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                // fail-safe: do not throw for UI; log if you have logging configured
                // e.g. _logger?.LogWarning(ex, "Failed to save recommendation term");
            }
        }

        /// <summary>
        /// Read top search terms from DB (persisted recommendations).
        /// </summary>
        public List<string> GetTopSearchTermsFromDb(int top = 3)
        {
            try
            {
                return _db.Recommendations
                          .OrderByDescending(r => r.Count)
                          .ThenByDescending(r => r.LastSeen)
                          .Take(top)
                          .Select(r => r.Term)
                          .ToList();
            }
            catch
            {
                // fallback to in-memory counts if DB read fails
                return searchCounts.OrderByDescending(kv => kv.Value).Take(top).Select(kv => kv.Key).ToList();
            }
        }
        // Recommend
        public List<Event> Recommend(int topN = 5)
            {
                var picks = new List<Event>();

                var topTerms = searchCounts.OrderByDescending(kv => kv.Value).Take(3).Select(kv => kv.Key).ToList();

                foreach (var term in topTerms)
                {
                    if (byCategory.TryGetValue(term, out var catList))
                        picks.AddRange(catList.OrderBy(e => e.Date).Take(3));
                    else
                        picks.AddRange(Search(term).Take(3));
                    if (picks.Count >= topN) break;
                }

                if (picks.Count < topN)
                {
                    var temp = upcomingQueue.UnorderedItems.Select(x => x.Element).OrderBy(e => e.Date);
                    picks.AddRange(temp.Where(e => !picks.Contains(e)).Take(topN - picks.Count));
                }

                return picks.Distinct().OrderBy(e => e.Date).Take(topN).ToList();
            }

            // CRUD that syncs to DB
            public void AddEvent(Event ev)
            {
                _db.Events.Add(ev);
                _db.SaveChanges();
                AddToIndexes(ev);
                undoStack.Push(() => RemoveEvent(ev.EventID));
            }

            public bool RemoveEvent(int eventId)
            {
                var ev = _db.Events.Find(eventId);
                if (ev == null) return false;
                _db.Events.Remove(ev);
                _db.SaveChanges();
                // rebuild indexes from DB for correctness
                LoadFromDatabase();
                return true;
            }

            public void Dispose() => _db.Dispose();
        }
    }


