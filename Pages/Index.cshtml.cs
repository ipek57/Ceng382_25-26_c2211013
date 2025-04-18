using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5Project.Models;
using Week5Project.Helpers;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

//bu kodda, bir de export filtered buttonu olmalı, filterda yazdığım dosyaya göre filtelenen dosyaları 
//yine aynı mantıkta farklı butonla indirmeli, lütfen tüm kodları buna göre düzelt ve tam halini yolla

namespace Week5Project.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassModel> _classList = GenerateSampleData();

        [BindProperty]
        public ClassModel NewClass { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalItemCount { get; set; }

        [TempData]
        public bool EditMode { get; set; }

        public List<ClassModel> PagedClassList { get; set; } = new();

        public void OnGet()
        {
            var query = _classList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(c => c.ClassName.ToLower().Contains(SearchString.ToLower()));
            }

            TotalItemCount = query.Count();

            PagedClassList = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public IActionResult OnGetCancel()
        {
            EditMode = false;
            NewClass = new ClassModel();
            return RedirectToPage();
        }

        [ValidateAntiForgeryToken]
        public IActionResult OnPostAdd()
        {
            if (NewClass != null && !string.IsNullOrWhiteSpace(NewClass.ClassName))
            {
                int newId = _classList.Any() ? _classList.Max(c => c.Id) + 1 : 1;
                NewClass.Id = newId;

                _classList.Add(new ClassModel
                {
                    Id = NewClass.Id,
                    ClassName = NewClass.ClassName,
                    StudentCount = NewClass.StudentCount,
                    Description = NewClass.Description
                });

                NewClass = new ClassModel();
            }

            OnGet();
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            TempData.Remove("EditMode");
            ModelState.Remove(nameof(SearchString));

            if (!ModelState.IsValid)
            {
                OnGet();
                return Page();
            }

            var item = _classList.FirstOrDefault(c => c.Id == NewClass.Id);
            if (item != null)
            {
                item.ClassName = NewClass.ClassName;
                item.StudentCount = NewClass.StudentCount;
                item.Description = NewClass.Description;
            }

            return RedirectToPage(new { SearchString, PageNumber });
        }

        public IActionResult OnPostEdit(int id)
        {
            var item = _classList.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                NewClass = new ClassModel
                {
                    Id = item.Id,
                    ClassName = item.ClassName,
                    StudentCount = item.StudentCount,
                    Description = item.Description
                };
                EditMode = true;
            }

            OnGet();
            return Page();
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = _classList.FirstOrDefault(c => c.Id == id);
            if (item != null)
                _classList.Remove(item);

            return RedirectToPage(new { SearchString, PageNumber });
        }

        // ✅ Yeni JSON Export Handler – Tüm veriler
        public IActionResult OnPostExportAllJson(string SelectedColumns)
        {
            var selectedIndexes = SelectedColumns?.Split(',').Select(int.Parse).ToList() ?? new List<int> { 0, 1, 2 };
            var columnNames = new[] { "ClassName", "StudentCount", "Description" };
            var selectedProps = selectedIndexes.Select(i => columnNames[i]).ToList();

            var json = Utils.Instance.ExportToJson(_classList, selectedProps);

            var fileName = $"ExportedAll_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);
            System.IO.File.WriteAllText(filePath, json);

            return Redirect($"/exports/{fileName}"); 
        }

        // ✅ Yeni JSON Export Handler – Filtreli veriler
        public IActionResult OnPostExportFilteredJson(string SelectedColumns, string SearchString)
        {
            var selectedIndexes = SelectedColumns?.Split(',').Select(int.Parse).ToList() ?? new List<int> { 0, 1, 2 };
            var columnNames = new[] { "ClassName", "StudentCount", "Description" };
            var selectedProps = selectedIndexes.Select(i => columnNames[i]).ToList();

            var query = _classList.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                query = query.Where(c => c.ClassName.ToLower().Contains(SearchString.ToLower()));
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

        private static List<ClassModel> GenerateSampleData()
        {
            var list = new List<ClassModel>();
            for (int i = 1; i <= 100; i++)
            {
                var classModel = new ClassModel
                {
                    Id = i,
                    ClassName = $"Class {i}",
                    StudentCount = 10 + (i % 15),
                    Description = $"Sample description {i}"
                };
                list.Add(classModel);
            }

            Console.WriteLine($"Generated {list.Count} classes.");
            return list;
        }
    }
}
