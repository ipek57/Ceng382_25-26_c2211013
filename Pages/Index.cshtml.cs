using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5Project.Models;
using Week5Project.Helpers;
using Week5Project.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

/*Ben bir ASP.NET Core Razor Pages projesinde Index.cshtml ve Index.cshtml.cs dosyalarını nasıl düzenlemeliyim, 
add, edit, delete, cancel butonları doğru çalışsın, edit moduna geçince formda güncel veriler dolsun ama cancel deyince form sıfırlansın, 
export all ve export filtered JSON düzgün çalışsın, 
başlangıçta 100 sınıf verisini bir seferlik yüklemek için nasıl bir buton koyup sonra onu kaldırabilirim, 
sayfalama, arama filtresi, görünürlük sadece IsActive true olanlar için nasıl yapılır, TempData ve RedirectToPage kullanımı nasıl senkronize edilir, 
ve son olarak tüm bu işlevlerin düzgün çalışması için arka uç ve ön uç kodlarını nasıl tam entegre ederim?”*/

namespace Week5Project.Pages
{
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

        [TempData]
        public bool EditMode { get; set; }

        public List<Class> PagedClassList { get; set; } = new();

        public void OnGet()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var cookieUsername = Request.Cookies["username"];
            var sessionToken = HttpContext.Session.GetString("token");
            var cookieToken = Request.Cookies["token"];

            if (sessionUsername == null || cookieUsername == null || sessionToken == null || cookieToken == null
                || sessionUsername != cookieUsername || sessionToken != cookieToken)
            {
                Response.Redirect("/Login");
                return;
            }

            var query = _context.Classes
                .Where(c => c.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(c => c.Name.ToLower().Contains(SearchString.ToLower()));
            }

            TotalItemCount = query.Count();

            PagedClassList = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            if (!EditMode)
            {
                NewClass = new Class { IsActive = true };
            }
        }

        public IActionResult OnGetCancel()
        {
            EditMode = false;
            NewClass = new Class();
            return RedirectToPage();
        }

        public IActionResult OnPostAdd()
        {
            if (NewClass != null && !string.IsNullOrWhiteSpace(NewClass.Name))
            {
                NewClass.Id = 0;
                NewClass.IsActive = true;
                _context.Classes.Add(NewClass);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            var selected = _context.Classes.FirstOrDefault(c => c.Id == id && c.IsActive);
            if (selected != null)
            {
                NewClass = selected;
                EditMode = true;
            }

            OnGet(); // Listelemeyi tekrar yap
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            var selected = _context.Classes.FirstOrDefault(c => c.Id == NewClass.Id);
            if (selected != null)
            {
                selected.Name = NewClass.Name;
                selected.PersonCount = NewClass.PersonCount;
                selected.Description = NewClass.Description;
                if (!selected.IsActive)
                    selected.IsActive = true;

                _context.SaveChanges();
            }

            EditMode = false;
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var selected = _context.Classes.FirstOrDefault(c => c.Id == id);
            if (selected != null)
            {
                selected.IsActive = false;
                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostCancel()
        {
            EditMode = false;      // EditMode bitir
            NewClass = new Class(); // Form sıfırla
            TempData.Remove(nameof(EditMode)); // ✨ Redirect sonrasında EditMode taşınmasın!
            return RedirectToPage(new { SearchString, PageNumber });
        }



        public IActionResult OnPostExportFilteredJson(string SelectedColumns, string SearchString)
        {
            var selectedIndexes = SelectedColumns?.Split(',').Select(int.Parse).ToList() ?? new List<int> { 0, 1, 2 };
            var columnNames = new[] { "Name", "PersonCount", "Description" };
            var selectedProps = selectedIndexes.Select(i => columnNames[i]).ToList();

            var query = _context.Classes.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(c => c.Name.ToLower().Contains(SearchString.ToLower()));
            }

            var filteredData = query.ToList();

            var json = Utils.Instance.ExportToJson(filteredData, selectedProps);

            var fileName = $"ExportedFiltered_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            System.IO.File.WriteAllText(filePath, json);

            return Redirect($"/exports/{fileName}");
        }

        public IActionResult OnPostExportAllJson(string SelectedColumns)
        {
            var selectedIndexes = SelectedColumns?.Split(',').Select(int.Parse).ToList() ?? new List<int> { 0, 1, 2 };
            var columnNames = new[] { "Name", "PersonCount", "Description" };
            var selectedProps = selectedIndexes.Select(i => columnNames[i]).ToList();

            var allData = _context.Classes
                .Where(c => c.IsActive)
                .ToList();

            var json = Utils.Instance.ExportToJson(allData, selectedProps);

            var fileName = $"ExportedAll_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            System.IO.File.WriteAllText(filePath, json);

            return Redirect($"/exports/{fileName}");
        }
        /*
        public IActionResult OnPostLoadSampleData()
        {
            for (int i = 1; i <= 100; i++)
            {
                var cls = new Class
                {
                    Name = $"Sample Class {i}",
                    PersonCount = 20 + (i % 30),
                    Description = $"Description for Sample Class {i}",
                    IsActive = true
                };

                _context.Classes.Add(cls);
            }

            _context.SaveChanges();

            return RedirectToPage();
        }
        */
    }
}
