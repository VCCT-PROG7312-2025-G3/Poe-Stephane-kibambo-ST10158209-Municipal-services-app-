using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MunicipalMvcApp.Data;
using MunicipalMvcApp.Models;

namespace Municipal_services_app.Services
{
    public class RequestStore : IDisposable
    {
        private readonly AppDbContext _db;

        // ---- Core In-Memory Structures ----
        private readonly SortedDictionary<string, Issue> _byRequestRef = new(StringComparer.OrdinalIgnoreCase); // balanced tree (Red-Black)
        private PriorityQueue<Issue, DateTime> _minHeap = new(); // oldest-first priority
        private readonly Dictionary<string, List<Issue>> _byCategory = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, HashSet<string>> _graph = new(StringComparer.OrdinalIgnoreCase); // adjacency list graph

        public RequestStore(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            LoadFromDatabase();
        }

        // --- Load data into structures ---
        public void LoadFromDatabase()
        {
            _byRequestRef.Clear();
            _byCategory.Clear();
            _graph.Clear();
            _minHeap = new PriorityQueue<Issue, DateTime>();

            var issues = _db.Issues.AsNoTracking().OrderBy(i => i.CreatedAt).ToList();

            foreach (var issue in issues)
            {
                if (string.IsNullOrWhiteSpace(issue.RequestRef))
                    issue.RequestRef = GenerateRef(issue.Id);

                _byRequestRef[issue.RequestRef] = issue;
                _minHeap.Enqueue(issue, issue.CreatedAt);

                if (!_byCategory.TryGetValue(issue.Category ?? "", out var list))
                {
                    list = new List<Issue>();
                    _byCategory[issue.Category ?? ""] = list;
                }
                list.Add(issue);

                if (!_graph.ContainsKey(issue.RequestRef))
                    _graph[issue.RequestRef] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            // build relationships by location
            var groupedByLocation = issues.Where(i => !string.IsNullOrEmpty(i.Location))
                                          .GroupBy(i => i.Location!.Trim().ToLowerInvariant());
            foreach (var grp in groupedByLocation)
            {
                var refs = grp.Select(i => i.RequestRef).Distinct().ToList();
                for (int a = 0; a < refs.Count; a++)
                {
                    for (int b = a + 1; b < refs.Count; b++)
                        AddEdge(refs[a], refs[b]);
                }
            }
        }

        // --- Public Accessors ---
        public List<Issue> GetAll() => _db.Issues.OrderByDescending(i => i.CreatedAt).ToList();

        public Issue? FindByRef(string requestRef)
        {
            if (_byRequestRef.TryGetValue(requestRef, out var issue))
                return issue;

            // fallback from DB
            return _db.Issues.AsNoTracking().FirstOrDefault(i => i.RequestRef == requestRef);
        }

        public List<Issue> GetByCategory(string category) =>
            _byCategory.TryGetValue(category ?? "", out var list)
                ? list.OrderByDescending(i => i.CreatedAt).ToList()
                : new List<Issue>();

        public List<Issue> GetNextN(int n = 10) =>
            _minHeap.UnorderedItems.Select(x => x.Element).OrderBy(i => i.CreatedAt).Take(n).ToList();

        public IEnumerable<string> Categories => _byCategory.Keys.OrderBy(k => k);

        // --- Graph Methods ---
        public void AddEdge(string aRef, string bRef)
        {
            if (string.IsNullOrWhiteSpace(aRef) || string.IsNullOrWhiteSpace(bRef) || aRef == bRef) return;

            if (!_graph.ContainsKey(aRef)) _graph[aRef] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!_graph.ContainsKey(bRef)) _graph[bRef] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            _graph[aRef].Add(bRef);
            _graph[bRef].Add(aRef);
        }

        public IEnumerable<string> GetNeighbors(string requestRef) =>
            _graph.ContainsKey(requestRef) ? _graph[requestRef] : Enumerable.Empty<string>();

        // --- Graph Traversal (BFS) ---
        public List<Issue> BreadthFirstTraversal(string startRef)
        {
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<string>();
            var result = new List<Issue>();

            if (string.IsNullOrWhiteSpace(startRef) || !_graph.ContainsKey(startRef))
                return result;

            queue.Enqueue(startRef);
            visited.Add(startRef);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (_byRequestRef.TryGetValue(current, out var issue))
                    result.Add(issue);

                foreach (var neighbor in _graph[current])
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return result;
        }

        // --- CRUD and Update Methods ---
        public bool UpdateStatus(string requestRef, string newStatus)
        {
            var issue = _db.Issues.SingleOrDefault(i => i.RequestRef == requestRef);
            if (issue == null) return false;

            var prev = issue.Status ?? "Unknown";
            issue.Status = newStatus;
            issue.DateUpdated = DateTime.UtcNow;
            issue.StatusHistory += $"{DateTime.UtcNow:u} | {prev} -> {newStatus}\n";

            _db.Issues.Update(issue);
            _db.SaveChanges();
            LoadFromDatabase();
            return true;
        }

        public Issue AddIssue(Issue issue)
        {
            if (issue.CreatedAt == default) issue.CreatedAt = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(issue.RequestRef)) issue.RequestRef = GenerateRef(issue.Id);

            _db.Issues.Add(issue);
            _db.SaveChanges();
            LoadFromDatabase();
            return issue;
        }

        public bool Remove(string requestRef)
        {
            var issue = _db.Issues.SingleOrDefault(i => i.RequestRef == requestRef);
            if (issue == null) return false;

            _db.Issues.Remove(issue);
            _db.SaveChanges();
            LoadFromDatabase();
            return true;
        }

        private string GenerateRef(int id)
        {
            var basePart = id > 0 ? id.ToString("D6") : DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var rand = Guid.NewGuid().ToString("N")[..4].ToUpperInvariant();
            return $"MS-{basePart}-{rand}";
        }

        public void Dispose() => GC.SuppressFinalize(this);
    }
}