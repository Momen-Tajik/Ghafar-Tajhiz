
/*Navbar*//*Navbar*//*Navbar*/
var navBtn = document.getElementById("mobileNavbarBtn");
var desktopNav = document.getElementById("desktopnavbar");

navBtn.addEventListener("click", () => {
    if (desktopNav.style.display === "flex") {
        desktopNav.style.display = "none";
    } else {
        desktopNav.style.display = "flex";
        desktopNav.style.flexDirection = "column";
    }
});

// تابع برای فرمت کردن قیمت
function formatPrice(price) {
    // تبدیل به عدد و سپس فرمت با جداکننده هزارگان
    return new Intl.NumberFormat('fa-IR').format(price);
}

// تابع برای به‌روزرسانی قیمت
function updatePrice() {
    let basePriceElement = document.getElementById("basePrice");
    let countInput = document.getElementById("countInput");
    let stockQuantityElement = document.getElementById("stockQuantity");
    let basePriceShow = document.getElementById("basePriceShow");
    let stockMessage = document.getElementById("stockMessage");

    // اگر المان‌ها وجود نداشته باشند، تابع را متوقف کن
    if (!basePriceElement || !countInput || !stockQuantityElement || !basePriceShow) {
        return;
    }

    let basePrice = parseFloat(basePriceElement.value);
    let stockQuantity = parseInt(stockQuantityElement.value);
    let count = parseInt(countInput.value);

    // اگر موجودی انبار صفر باشد
    if (stockQuantity <= 0) {
        basePriceShow.innerHTML = `قیمت نهایی: ۰ تومان`;
        if (countInput) {
            countInput.disabled = true;
        }
        return;
    }

    // محدود کردن مقدار ورودی به موجودی انبار
    if (count > stockQuantity) {
        count = stockQuantity;
        countInput.value = count;
    }

    if (count <= 0) {
        count = 1;
        countInput.value = 1;
    }

    // به‌روزرسانی پیام موجودی
    if (stockMessage) {
        stockMessage.innerHTML = `موجودی انبار: ${stockQuantity} عدد`;

        // تغییر رنگ پیام وقتی موجودی کم است
        if (stockQuantity < 5) {
            stockMessage.style.color = "orange";
        } else if (stockQuantity <= 0) {
            stockMessage.style.color = "red";
        } else {
            stockMessage.style.color = "#666";
        }
    }

    // محاسبه و نمایش قیمت نهایی
    let totalPrice = basePrice * count;
    basePriceShow.innerHTML = `قیمت نهایی: ${formatPrice(totalPrice)} تومان`;
}

// اجرای تابع هنگام بارگذاری صفحه
document.addEventListener('DOMContentLoaded', function () {
    // اولین بار قیمت را محاسبه کن
    updatePrice();

    // اضافه کردن event listener برای تغییرات
    let countInput = document.getElementById("countInput");
    if (countInput && !countInput.disabled) {
        countInput.addEventListener("input", updatePrice);
        countInput.addEventListener("change", updatePrice);

        // اضافه کردن event برای جلوگیری از ورود مقادیر نامعتبر
        countInput.addEventListener("keydown", function (e) {
            let stockQuantity = parseInt(document.getElementById("stockQuantity").value);
            let currentValue = parseInt(this.value) || 0;

            // اگر کاربر عددی بیشتر از موجودی وارد کرد
            if (currentValue > stockQuantity) {
                this.value = stockQuantity;
                updatePrice();
            }
        });
    }
});

// همچنین می‌توانیم event listener برای تغییرات دستی مقدار input اضافه کنیم
document.addEventListener('input', function (e) {
    if (e.target && e.target.id === 'countInput') {
        updatePrice();
    }
});


/*Register*//*Register*//*Register*//*Register*/

document.getElementById("togglePassword")?.addEventListener("click", function () {
    const input = document.getElementById("passwordInput");
    input.type = input.type === "password" ? "text" : "password";
});

/*Modal *//*Modal *//*Modal *//*Modal *//*Modal */

//Product Information Modal
document.addEventListener("DOMContentLoaded", function () {

    const modal = document.getElementById("modal");
    const closeBtn = document.querySelector(".close");
    const modalBody = document.getElementById("modalBody");

    document.querySelectorAll(".openModal").forEach(btn => {

        btn.addEventListener("click", async function () {

            const productId = btn.dataset.productId;
            console.log("ProductId:", productId);

            modal.style.display = "block";
            modalBody.innerHTML = document.getElementById("modelBody");

            try {
                const response = await fetch(`/Product/GetProduct?id=${productId}`);

                if (!response.ok)
                    throw new Error("Request failed");

                const html = await response.text();
                modalBody.innerHTML = html;
            }
            catch (err) {
                modalBody.innerHTML = "خطا در دریافت اطلاعات محصول";
                console.error(err);
            }
        });

    });

    closeBtn.onclick = () => modal.style.display = "none";

    window.onclick = e => {
        if (e.target === modal)
            modal.style.display = "none";
    };
});

//Term Modal  errrrrror

//document.addEventListener("DOMContentLoaded", function () {

//    const modal = document.getElementById("termsModal");
//    const closeBtn = document.querySelector(".close");
//    const modalBody = document.getElementById("termModalBody");

//    document.querySelectorAll(".openModal").forEach(btn => {

//        btn.addEventListener("click", async function () {


//            modal.style.display = "block";
//            modalBody.innerHTML = "قوانینننننننن";

//            try {

//                modalBody.innerHTML = "قوانینننننننن";
//            }
//            catch (err) {
//                modalBody.innerHTML = "خطا در دریافت اطلاعات محصول";
//                console.error(err);
//            }
//        });

//    });

//    closeBtn.onclick = () => modal.style.display = "none";

//    window.onclick = e => {
//        if (e.target === modal)
//            modal.style.display = "none";
//    };
//});


/*Basket*//*Basket*//*Basket*//*Basket*//*Basket*/


function AddToBasket() {
    var productId = parseInt(document.getElementById("productId").value);
    var qty = parseInt(document.getElementById("countInput").value);

    fetch('/Order/AddToBasket', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ productId, qty })
    })
        .then(async res => {
            const data = await res.json();
            if (!res.ok) throw data;
            return data;
        })
        .then(data => {
            if (data.res === false) {
                showFailAlert('', data.msg);
            } else {
                showSuccessAlert('', data.msg);
                updateCartBadge()
            }
        })
        .catch(err => {
            showFailAlert('', err.msg || 'خطای غیرمنتظره');
        }); 
}

function RemoveBasketItem(id) {

    var data = {
        BasketItemId:id
    };


    fetch('/Order/RemoveBasketItem', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)

    })
        .then(res => res.json())
        .then(data => {
            const row = document.getElementById("Basket_" + id);
            row.style.display = "none";
            updateCartBadge()
        })
        .catch(err => console.error(err.message));

}

function validateCheckOutForm() {
    const address = document.getElementById('address').value.trim();
    const mobile = document.getElementById('mobile').value.trim();
    const mobileRegex = /^09\d{9}$/;

    if (address === '') {
        showFailAlert('خطا', 'لطفا آدرس را وارد کنید');
        return false; // جلوی ارسال فرم رو می‌گیره
    }

    if (!mobileRegex.test(mobile)) {
        showFailAlert('خطا', 'شماره موبایل وارد شده معتبر نیست');
        return false; // جلوی ارسال فرم رو می‌گیره
    }

    return true; // فرم ارسال میشه
}

function updateCartBadge() {
    fetch('/Order/GetBasketCount')
        .then(response => response.json())
        .then(count => {
            const badgeDesktop = document.getElementById('cart-badge-desktop');
            const badgeMobile = document.getElementById('cart-badge-mobile');

            if (badgeDesktop) {
                badgeDesktop.textContent = count;
                badgeDesktop.setAttribute('data-count', count);
            }
            if (badgeMobile) {
                badgeMobile.textContent = count;
                badgeMobile.setAttribute('data-count', count);
            }
        })
        .catch(() => { });
}



/*Comment*/

function addComment() {
    // Get values
    const productId = parseInt(document.getElementById('productId').value);
    const commentText = document.getElementById('commentText').value;
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Validate
    if (!commentText.trim()) {
        showFailAlert('', 'متن نظر نمی‌تواند خالی باشد');
        return;
    }

    // Send AJAX
    fetch('/Product/AddProductComment', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify({
            productId: productId,
            text: commentText
        })
    })
        .then(async res => {
            const data = await res.json();
            if (!res.ok) throw data;
            return data;
        })
        .then(data => {
            if (data.res === false) {
                showFailAlert('', data.msg);
            } else {
                showSuccessAlert('', data.msg);
                // ✅ Refresh the page after a short delay so user sees the success message
                setTimeout(() => location.reload(), 750);
            }
        })
        .catch(err => {
            showFailAlert('', err.msg || 'خطای غیرمنتظره');
        });
}

/* SWEET ALERT *//* SWEET ALERT *//* SWEET ALERT *//* SWEET ALERT */

function showSuccessAlert(title,text='موفق') {
    Swal.fire({
        title: title,
        text: text,
        icon: 'success',
        showConfirmButton: false,
        timer: 1000
    });
}

function showFailAlert(title,text='ناموفق') {
    Swal.fire({
        title: title,
        text: text,
        icon: 'error',
        confirmButtonText: 'باشه'
    });
}


// Run on page load
document.addEventListener('DOMContentLoaded', updateCartBadge);
