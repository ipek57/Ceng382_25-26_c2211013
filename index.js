const loginScreen = document.getElementById("login-screen");
const closeLoginBtn = document.getElementById("close-login");
const background = document.getElementById("background");
const bowContainer = document.querySelector(".bow-container");
const bow = document.querySelector(".bow");
const arrow = document.getElementById("arrow");

// Yay konteynerine tıklandığında çalışacak olay dinleyicisini nasıl eklerim?
bowContainer.addEventListener("click", () => {
  bow.classList.add("pull");

  // Kısa bir gecikmeyle oku fırlatma işlemi başlatılıyor
  setTimeout(() => {
    arrow.classList.add("shoot");
    // Yayın çekilme animasyonu kaldırılıp serbest bırakılma animasyonu ekleniyor
    bow.classList.remove("pull");
    bow.classList.add("release");
  }, 300);

  // Belirli bir süre sonra giriş ekranını açıp arka planı değiştirelim
  setTimeout(() => {
    loginScreen.style.display = "flex";
    background.style.background =
      'url("./assets/download.gif") no-repeat center center fixed';
    background.style.backgroundSize = "cover";
  }, 800);
});

// Giriş ekranını kapatma butonuna tıklanınca çalışsın
closeLoginBtn.addEventListener("click", () => {
  loginScreen.style.display = "none";
  // Arka planı eski haline getirelim
  background.style.background =
    'url("./assets/wallpaperflare.com_wallpaper.jpg") no-repeat center center fixed';
  background.style.backgroundSize = "cover";

  arrow.classList.remove("shoot");
  bow.classList.remove("release");
});
