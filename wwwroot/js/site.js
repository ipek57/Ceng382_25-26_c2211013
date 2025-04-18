document.addEventListener("DOMContentLoaded", function () {
  const tokenElement = document.querySelector(
    'input[name="__RequestVerificationToken"]'
  );
  const token = tokenElement ? tokenElement.value : "";

  // Seçilen kolonları topla (th'leri kontrol et)
  function getSelectedColumnIndexes() {
    const selected = [];
    document.querySelectorAll("table thead th").forEach((th, index) => {
      if (
        th.style.backgroundColor === "rgb(209, 255, 209)" ||
        th.style.backgroundColor === "#d1ffd1"
      ) {
        selected.push(index);
      }
    });
    return selected.length ? selected : [0, 1, 2];
  }

  // Export All JSON
  document
    .getElementById("exportAllBtn")
    .addEventListener("click", function () {
      const selectedCols = getSelectedColumnIndexes().join(",");

      fetch("/Index?handler=ExportAllJson", {
        method: "POST",
        headers: {
          "Content-Type": "application/x-www-form-urlencoded",
          RequestVerificationToken: token,
        },
        body: `SelectedColumns=${encodeURIComponent(selectedCols)}`,
      })
        .then(() => alert("All data exported to wwwroot/exports folder."))
        .catch((error) => console.error("Export all error: ", error));
    });

  // Export Filtered JSON
  document
    .getElementById("exportFilteredBtn")
    .addEventListener("click", function () {
      const searchValue = document.querySelector(
        'input[name="SearchString"]'
      ).value;
      const selectedCols = getSelectedColumnIndexes().join(",");

      fetch(
        `/Index?handler=ExportFilteredJson&SearchString=${encodeURIComponent(
          searchValue
        )}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            RequestVerificationToken: token,
          },
          body: `SelectedColumns=${encodeURIComponent(selectedCols)}`,
        }
      )
        .then(() => alert("Filtered data exported to wwwroot/exports folder."))
        .catch((error) => console.error("Export filtered error: ", error));
    });
});
