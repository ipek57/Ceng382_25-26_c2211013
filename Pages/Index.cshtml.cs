using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Week5Project.Data;
using Week5Project.Helpers;
using Week5Project.Models;

namespace Week5Project.Pages.Classes
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Class NewClass { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalItemCount { get; set; }
        public List<Class> PagedClassList { get; set; } = new();

        [TempData]
        public bool EditMode { get; set; }

        public void OnGet()
        {
            // Sadece aktif kayıtları getir
            var query = _context.Classes
                .Where(c => c.IsActive);

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(SearchString))
                query = query.Where(c => c.Name
                    .Contains(SearchString, StringComparison.OrdinalIgnoreCase));

            TotalItemCount = query.Count();

            // Sayfalama
            PagedClassList = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Eğer EditMode false ise formu resetle
            if (!EditMode)
                NewClass = new Class { IsActive = true };
        }

        public IActionResult OnGetCancel()
        {
            EditMode = false;
            NewClass = new Class { IsActive = true };
            return RedirectToPage(new { SearchString, PageNumber });
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            // Yeni kaydı ekleyen kullanıcıyı ata
            NewClass.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            NewClass.IsActive = true;

            _context.Classes.Add(NewClass);
            _context.SaveChanges();

            return RedirectToPage(new { SearchString, PageNumber });
        }

        public IActionResult OnPostEdit(int id)
        {
            var selected = _context.Classes
                .FirstOrDefault(c => c.Id == id && c.IsActive);

            if (selected != null)
            {
                NewClass = selected;
                EditMode = true;
            }

            OnGet(); 
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (!ModelState.IsValid)
                return Page();

            var selected = _context.Classes
                .FirstOrDefault(c => c.Id == NewClass.Id);

            if (selected != null)
            {
                selected.Name = NewClass.Name;
                selected.PersonCount = NewClass.PersonCount;
                selected.Description = NewClass.Description;
                selected.IsActive = true;
                _context.SaveChanges();
            }

            EditMode = false;
            return RedirectToPage(new { SearchString, PageNumber });
        }

        public IActionResult OnPostDelete(int id)
        {
            var selected = _context.Classes
                .FirstOrDefault(c => c.Id == id);

            if (selected != null)
            {
                selected.IsActive = false;
                _context.SaveChanges();
            }

            return RedirectToPage(new { SearchString, PageNumber });
        }

        public IActionResult OnPostCancelEdit()
        {
            // Düzeltiyoruz: edit modu iptal
            EditMode = false;
            NewClass = new Class { IsActive = true };
            return RedirectToPage(new { SearchString, PageNumber });
        }

        public IActionResult OnPostExportFilteredJson(string SelectedColumns)
        {
            var indices = SelectedColumns?.Split(',')
                .Select(int.Parse).ToList() 
                ?? new List<int> { 0, 1, 2 };

            var columns = new[] { "Name", "PersonCount", "Description" };
            var props = indices.Select(i => columns[i]).ToList();

            var query = _context.Classes.Where(c => c.IsActive);
            if (!string.IsNullOrWhiteSpace(SearchString))
                query = query.Where(c => c.Name
                    .Contains(SearchString, StringComparison.OrdinalIgnoreCase));

            var data = query.ToList();
            var json = Utils.Instance.ExportToJson(data, props);

            var fileName = $"Filtered_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, fileName);
            System.IO.File.WriteAllText(path, json);

            return Redirect($"/exports/{fileName}");
        }

        public IActionResult OnPostExportAllJson(string SelectedColumns)
        {
            var indices = SelectedColumns?.Split(',')
                .Select(int.Parse).ToList() 
                ?? new List<int> { 0, 1, 2 };

            var columns = new[] { "Name", "PersonCount", "Description" };
            var props = indices.Select(i => columns[i]).ToList();

            var allData = _context.Classes.Where(c => c.IsActive).ToList();
            var json    = Utils.Instance.ExportToJson(allData, props);

            var fileName = $"All_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var dir      = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
            Directory.CreateDirectory(dir);
            var path     = Path.Combine(dir, fileName);
            System.IO.File.WriteAllText(path, json);

            return Redirect($"/exports/{fileName}");
        }

        public IActionResult OnPostLoadSampleData()
        {
            // Eğer tablo boşsa 100 örnek ekle
            if (!_context.Classes.Any())
            {
                for (var i = 1; i <= 100; i++)
                {
                    _context.Classes.Add(new Class
                    {
                        Name        = $"Sample Class {i}",
                        PersonCount = 20 + (i % 30),
                        Description = $"Description for Sample Class {i}",
                        IsActive    = true,
                        UserId      = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    });
                }
                _context.SaveChanges();
            }
            return RedirectToPage(new { SearchString, PageNumber });
        }
    }
}
