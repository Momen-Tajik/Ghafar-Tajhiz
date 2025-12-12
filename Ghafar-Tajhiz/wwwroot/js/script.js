var navBtn=document.getElementById("mobileNavbarBtn");
var desktobNav=document.getElementById("desktopnavbar");

navBtn.addEventListener("click", () => {
    if(desktobNav.style.display==="flex")
    {
        desktobNav.style.display="none";
    }
    else
    {
        desktobNav.style.display="flex";
        desktobNav.style.flexDirection="column";
    }
});

let countInput = document.getElementById("countInput");
let basePriceShow = document.getElementById("basePriceShow");

countInput.addEventListener("change", () => {

  let basePrice = Number(document.getElementById("basePrice").value);
  let count = Number(countInput.value);

  if (count == 1) {
    basePriceShow.innerHTML = `قیمت نهایی: ${basePrice.toLocaleString()}`;
  } else {
    basePriceShow.innerHTML = `قیمت نهایی: ${(basePrice * count).toLocaleString()}`;
  }
});
