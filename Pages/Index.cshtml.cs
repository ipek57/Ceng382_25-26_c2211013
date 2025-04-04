using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Week5Project.Models;
using System.Collections.Generic;
using System.Linq;

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
            NewClass = new ClassModel(); // Formu da temizle
            return RedirectToPage(); // Sayfayı yenile
        }

        [ValidateAntiForgeryToken]
        public IActionResult OnPostAdd()
{
    if (NewClass != null && !string.IsNullOrWhiteSpace(NewClass.ClassName))
    {
        // Yeni ID oluştur (en büyük ID'ye +1)
        int newId = _classList.Any() ? _classList.Max(c => c.Id) + 1 : 1;
        NewClass.Id = newId;

        // Listeye en sona ekle
        _classList.Add(new ClassModel
        {
            Id = NewClass.Id,
            ClassName = NewClass.ClassName,
            StudentCount = NewClass.StudentCount,
            Description = NewClass.Description
        });

        // Formu temizle
        NewClass = new ClassModel();
    }

    // Filtreli liste veya görünüm için
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

        private static List<ClassModel> GenerateSampleData()
        {
            var list = new List<ClassModel>();
            for (int i = 1; i <= 100; i++)
            {
                list.Add(new ClassModel
                {
                    Id = i,
                    ClassName = $"Class {i}",
                    StudentCount = 10 + (i % 15),
                    Description = $"Sample description {i}"
                });
            }
            return list;
        }
    }
}
