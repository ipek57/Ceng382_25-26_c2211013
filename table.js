document.addEventListener("DOMContentLoaded", () => {
  const form = document.getElementById("class-form");
  const tableBody = document.querySelector("#class-table tbody");

  form.addEventListener("submit", function (event) {
    event.preventDefault(); // Sayfanın yenilenmesini engelle

    // Formdan bilgileri al
    const className = document.getElementById("class-name").value;
    const numPeople = document.getElementById("num-people").value;
    const description = document.getElementById("description").value;

    // Yeni bir satır oluştur
    const newRow = document.createElement("tr");

    newRow.innerHTML = `
            <td>${className}</td>
            <td>${numPeople}</td>
            <td>${description}</td>
        `;

    // ===EVENTLER===

    //Seçili satır vurgulansın
    newRow.addEventListener("click", function () {
      console.log(`Clicked row: ${className}, ${numPeople}, ${description}`);
      newRow.style.backgroundColor = "#f0e68c"; // Satırı renklendir
    });

    // (Üzerine gelince renk değişsin
    newRow.addEventListener("mouseover", function () {
      newRow.style.backgroundColor = "#add8e6";
    });

    // Fare çıkınca eski haline dönsün
    newRow.addEventListener("mouseout", function () {
      newRow.style.backgroundColor = "";
    });

    // Çift tıklamada satırı silelim
    newRow.addEventListener("dblclick", function () {
      tableBody.removeChild(newRow);
    });

    // Yeni satırı tabloya ekleyelim
    tableBody.appendChild(newRow);

    // Formu temizleyelim
    form.reset();
  });

  // Inputlara Focus ve Blur eventleri ekleyelim
  document.querySelectorAll("input").forEach((input) => {
    input.addEventListener("focus", function () {
      input.style.border = "2px solid #d8a44b";
    });

    input.addEventListener("blur", function () {
      input.style.border = "";
    });
  });

  // Keyup Event: Gerçek zamanlı doğrulama yapalım
  document.querySelectorAll("input").forEach((input) => {
    input.addEventListener("keyup", function () {
      if (input.value.length < 3) {
        input.style.border = "2px solid red";
      } else {
        input.style.border = "2px solid green";
      }
    });
  });
});
