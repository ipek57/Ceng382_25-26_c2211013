using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Week5Project.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Session temizle
            HttpContext.Session.Clear();//kullanıcı çıkış yaptı

            // Tarayıcıdan Cookie’leri sil
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");

            // Login sayfasına yönlendir
            return RedirectToPage("/Login");//Giriş sayfasına yönlendirme
        }
    }
}
