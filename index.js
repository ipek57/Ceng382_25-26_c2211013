const loginScreen = document.getElementById("login-screen");
const closeLoginBtn = document.getElementById("close-login");
const background = document.getElementById("background");
const bowContainer = document.querySelector(".bow-container");
const bow = document.querySelector(".bow");
const arrow = document.getElementById("arrow");
const loginButton = document.querySelector(".login-box button");
const usernameInput = document.querySelector(".login-box input[type='text']");
const passwordInput = document.querySelector(
  ".login-box input[type='password']"
);
const body = document.querySelector("body");

let loginData = [];
let formsVisible = true;

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

// Kullanıcı giriş bilgilerini saklayıp array içinde konsola nasıl bastırırız?
loginButton.addEventListener("click", () => {
  const username = usernameInput.value;
  const password = passwordInput.value;
  if (username && password) {
    loginData.push({ username, password });
    console.log("Login Attempts:", loginData);
  }
});

// Gerçek zamanlı saat ekleme
const clock = document.createElement("div");
clock.style.position = "absolute";
clock.style.top = "10px";
clock.style.right = "10px";
clock.style.fontSize = "20px";
clock.style.color = "white";
clock.style.fontFamily = "inherit";
clock.style.background = "rgba(0, 0, 0, 0.44)"; // Hafif transparan arka plan
clock.style.padding = "10px 15px";
clock.style.borderRadius = "10px";
clock.style.boxShadow = "0 4px 6px rgba(0, 0, 0, 0.3)";
body.appendChild(clock);

function updateClock() {
  const now = new Date();
  const timeString = now.toLocaleTimeString();
  clock.textContent = timeString;
}
setInterval(updateClock, 1000);
updateClock();

// 'H' tuşuna basıldığında form alanlarını ve yayı gizle/göster
window.addEventListener("keydown", (event) => {
  if (event.key.toLowerCase() === "h") {
    formsVisible = !formsVisible;
    document
      .querySelectorAll("form, .login-container, .bow-container")
      .forEach((element) => {
        element.style.display = formsVisible ? "flex" : "none";
      });
  }
});
