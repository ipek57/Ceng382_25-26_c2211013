using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Week5Project.Models;

namespace Week5Project.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> classData = new();
        private static int idCounter = 1;

        [BindProperty]
        public ClassInformationModel NewClass { get; set; }

        public List<ClassInformationModel> ClassList => classData.OrderBy(x => x.Id).ToList();  // ID sırasına göre sıralıyoruz

        // View'a geçilecek flag
        public bool EditMode => NewClass != null && NewClass.Id != 0;

        public void OnGet()
        {
            // boş form
            NewClass = new ClassInformationModel();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            // Yeni sınıf eklenirken ID'yi doğru şekilde veriyoruz
            NewClass.Id = idCounter++; 
            classData.Add(NewClass);

            // Sıralama işlemi
            classData = classData.OrderBy(x => x.Id).ToList();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = classData.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                classData.Remove(item);

                // Silme işleminden sonra ID'leri birer birer kaydırıyoruz
                for (int i = 0; i < classData.Count; i++)
                {
                    classData[i].Id = i + 1; // ID'yi 1'den başlayarak yeniden sıralıyoruz
                }

                // ID sayacını yeniden düzenliyoruz
                idCounter = classData.Count > 0 ? classData.Max(x => x.Id) + 1 : 1;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            var item = classData.FirstOrDefault(x => x.Id == id);
            if (item != null)
            {
                NewClass = new ClassInformationModel
                {
                    Id = item.Id,
                    ClassName = item.ClassName,
                    StudentCount = item.StudentCount,
                    Description = item.Description
                };
            }

            return Page(); // EditMode burada aktif olacak
        }

        public IActionResult OnPostUpdate()
        {
            if (!ModelState.IsValid)
                return Page();

            var item = classData.FirstOrDefault(x => x.Id == NewClass.Id);
            if (item != null)
            {
                item.ClassName = NewClass.ClassName;
                item.StudentCount = NewClass.StudentCount;
                item.Description = NewClass.Description;
            }

            return RedirectToPage();
        }
    }
}
