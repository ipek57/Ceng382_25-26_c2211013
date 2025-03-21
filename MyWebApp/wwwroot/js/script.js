document.addEventListener("DOMContentLoaded", function () {
  document
    .getElementById("loginForm")
    .addEventListener("submit", function (event) {
      let emailField = document.getElementById("email");
      let emailError = document.getElementById("emailError");

      // ✅ Doğru Regex Deseni:
      let emailPattern = /^[a-zA-Z0-9._%+-]+@(hotmail|gmail|outlook)\.com$/;

      if (!emailPattern.test(emailField.value)) {
        emailError.innerText = "Please enter a valid email!";
        event.preventDefault(); // Formun gönderilmesini engelle
      } else {
        emailError.innerText = ""; // Hata mesajını temizle
      }
    });
});
