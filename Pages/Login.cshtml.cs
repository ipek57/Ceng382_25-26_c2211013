using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Week5Project.Models;
using Microsoft.AspNetCore.Authorization;

namespace Week5Project.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");//admin
            var json = System.IO.File.ReadAllText(jsonPath);
            var users = JsonSerializer.Deserialize<List<User>>(json);

            var user = users?.FirstOrDefault(u => u.Username == Username && u.Password == Password && u.IsActive);
            //kullanıcının username ve passwordu eşleşiyor mu
            if (user == null)
            {
                ErrorMessage = "Username or password is incorrect.";
                return Page();
            }

            // Login başarılı → Session ve Cookie ayarları
            var token = Guid.NewGuid().ToString(); // Rastgele bir UUID, hem token 
            // hem session içine yazılacak sonra sayfa güvenliği için kullanılacak.


            //Session içine yazılanlar
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token); //token eşlemesi
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);
            //tarayıcının oturumuyla eşleşmek için
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddMinutes(30),
                HttpOnly = true, //Js ile erişilmez
                Secure = true, //Https bağlantılarında çalışır
                SameSite = SameSiteMode.Strict //farklı sitelere cookie gönderilmez
            };

            Response.Cookies.Append("username", user.Username, cookieOptions);
            //Tarayıcıya username adında bir cookie yazıyor
            Response.Cookies.Append("token", token, cookieOptions);
            //Tarayıcıya token adında bir cookie yazıyor
            Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);
            //Serverdaki sessionla eşleşme
            // Yönlendirme
            return RedirectToPage("/Index");//giriş başarılı
        }
    }
}
