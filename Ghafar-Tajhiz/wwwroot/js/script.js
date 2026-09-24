/* =========================================================
   NAVBAR
========================================================= */

document.addEventListener("DOMContentLoaded", function () {
    const navBtn = document.getElementById("mobileNavbarBtn");
    const desktopNav = document.getElementById("desktopnavbar");

    if (navBtn && desktopNav) {
        navBtn.addEventListener("click", function () {
            desktopNav.classList.toggle("active");
        });
    }
});


/* =========================================================
   HELPERS
========================================================= */

function formatPrice(price) {
    const value = Number(price);

    if (Number.isNaN(value)) {
        return "۰";
    }

    return new Intl.NumberFormat("fa-IR").format(value);
}


function getAntiForgeryToken() {
    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    );

    return token ? token.value : null;
}


async function parseResponse(response) {
    const contentType = response.headers.get("content-type") || "";

    if (contentType.includes("application/json")) {
        const data = await response.json();

        if (!response.ok) {
            throw data;
        }

        return data;
    }

    const text = await response.text();

    if (!response.ok) {
        throw {
            msg: text || "خطای غیرمنتظره"
        };
    }

    return text;
}


/* =========================================================
   PRODUCT PRICE / QUANTITY
========================================================= */

function updatePrice() {
    const basePriceElement =
        document.getElementById("basePrice");

    const countInput =
        document.getElementById("countInput");

    const stockQuantityElement =
        document.getElementById("stockQuantity");

    const basePriceShow =
        document.getElementById("basePriceShow");

    const stockMessage =
        document.getElementById("stockMessage");

    if (
        !basePriceElement ||
        !countInput ||
        !stockQuantityElement ||
        !basePriceShow
    ) {
        return;
    }

    const basePrice =
        Number(basePriceElement.value);

    const stockQuantity =
        Number(stockQuantityElement.value);

    let count =
        Number(countInput.value);

    if (Number.isNaN(basePrice) ||
        Number.isNaN(stockQuantity)) {
        return;
    }

    if (stockQuantity <= 0) {
        countInput.value = 0;
        countInput.disabled = true;

        basePriceShow.textContent =
            "قیمت نهایی: ۰ تومان";

        if (stockMessage) {
            stockMessage.textContent =
                "موجودی: ۰";

            stockMessage.style.color = "red";
        }

        return;
    }

    if (Number.isNaN(count) || count < 1) {
        count = 1;
    }

    if (count > stockQuantity) {
        count = stockQuantity;
    }

    countInput.value = count;

    if (stockMessage) {
        stockMessage.textContent =
            `موجودی انبار: ${formatPrice(stockQuantity)} عدد`;

        if (stockQuantity < 5) {
            stockMessage.style.color = "orange";
        } else {
            stockMessage.style.color = "#666";
        }
    }

    const totalPrice =
        basePrice * count;

    basePriceShow.textContent =
        `قیمت نهایی: ${formatPrice(totalPrice)} تومان`;
}


function initializeQuantityInput() {
    const countInput =
        document.getElementById("countInput");

    if (!countInput || countInput.disabled) {
        return;
    }

    countInput.addEventListener("input", updatePrice);
    countInput.addEventListener("change", updatePrice);

    updatePrice();
}


/* =========================================================
   PASSWORD
========================================================= */

function initializePasswordToggle() {
    const toggleButton =
        document.getElementById("togglePassword");

    const passwordInput =
        document.getElementById("passwordInput");

    if (!toggleButton || !passwordInput) {
        return;
    }

    toggleButton.addEventListener("click", function () {
        const isPassword =
            passwordInput.type === "password";

        passwordInput.type =
            isPassword ? "text" : "password";
    });
}


/* =========================================================
   PRODUCT MODAL
========================================================= */

function initializeProductModal() {
    const modal =
        document.getElementById("modal");

    const modalBody =
        document.getElementById("modalBody");

    const closeButton =
        document.getElementById("closeModal");

    if (!modal || !modalBody) {
        return;
    }

    document.querySelectorAll(".openModal")
        .forEach(button => {

            button.addEventListener("click", async function () {

                const productId =
                    Number(this.dataset.productId);

                if (!productId || productId <= 0) {
                    return;
                }

                modal.style.display = "block";

                modalBody.innerHTML =
                    '<div class="text-center">در حال دریافت اطلاعات...</div>';

                try {
                    const response =
                        await fetch(
                            `/Product/GetProduct?id=${productId}`,
                            {
                                method: "GET",
                                headers: {
                                    "X-Requested-With":
                                        "XMLHttpRequest"
                                }
                            }
                        );

                    if (!response.ok) {
                        throw new Error(
                            "دریافت اطلاعات محصول ناموفق بود."
                        );
                    }

                    const html =
                        await response.text();

                    modalBody.innerHTML = html;

                } catch (error) {

                    console.error(error);

                    modalBody.innerHTML =
                        "خطا در دریافت اطلاعات محصول";
                }
            });
        });

    if (closeButton) {
        closeButton.addEventListener(
            "click",
            function () {
                modal.style.display = "none";
            }
        );
    }

    window.addEventListener(
        "click",
        function (event) {
            if (event.target === modal) {
                modal.style.display = "none";
            }
        }
    );
}


/* =========================================================
   BASKET
========================================================= */

async function AddToBasket() {

    const productIdElement =
        document.getElementById("productId");

    const qtyElement =
        document.getElementById("countInput");

    if (!productIdElement || !qtyElement) {
        return;
    }

    const productId =
        Number(productIdElement.value);

    const qty =
        Number(qtyElement.value);

    if (!productId || productId <= 0) {
        showFailAlert(
            "خطا",
            "محصول نامعتبر است."
        );
        return;
    }

    if (!qty || qty <= 0) {
        showFailAlert(
            "خطا",
            "تعداد محصول نامعتبر است."
        );
        return;
    }

    const token =
        getAntiForgeryToken();

    if (!token) {
        showFailAlert(
            "خطا",
            "توکن امنیتی یافت نشد."
        );
        return;
    }

    try {

        const response =
            await fetch(
                "/Order/AddToBasket",
                {
                    method: "POST",
                    headers: {
                        "Content-Type":
                            "application/json",

                        "RequestVerificationToken":
                            token,

                        "X-Requested-With":
                            "XMLHttpRequest"
                    },
                    body: JSON.stringify({
                        productId: productId,
                        qty: qty
                    })
                }
            );

        const data =
            await parseResponse(response);

        if (!data.res) {
            showFailAlert(
                "خطا",
                data.msg || "افزودن به سبد خرید انجام نشد."
            );
            return;
        }

        showSuccessAlert(
            "",
            data.msg || "محصول به سبد خرید اضافه شد."
        );

        await updateCartBadge();

    } catch (error) {

        console.error(error);

        showFailAlert(
            "خطا",
            error?.msg ||
            "خطای غیرمنتظره‌ای رخ داد."
        );
    }
}


async function RemoveBasketItem(id) {

    if (!id || id <= 0) {
        return;
    }

    const token =
        getAntiForgeryToken();

    if (!token) {
        showFailAlert(
            "خطا",
            "توکن امنیتی یافت نشد."
        );
        return;
    }

    try {

        const response =
            await fetch(
                "/Order/RemoveBasketItem",
                {
                    method: "POST",
                    headers: {
                        "Content-Type":
                            "application/json",

                        "RequestVerificationToken":
                            token,

                        "X-Requested-With":
                            "XMLHttpRequest"
                    },
                    body: JSON.stringify({
                        basketItemId: id
                    })
                }
            );

        const data =
            await parseResponse(response);

        if (!data.res) {
            showFailAlert(
                "خطا",
                data.msg || "حذف محصول انجام نشد."
            );
            return;
        }

        const row =
            document.getElementById(
                "Basket_" + id
            );

        if (row) {
            row.remove();
        }

        await updateCartBadge();

        showSuccessAlert(
            "",
            data.msg || "محصول حذف شد."
        );

    } catch (error) {

        console.error(error);

        showFailAlert(
            "خطا",
            error?.msg ||
            "حذف محصول انجام نشد."
        );
    }
}


/* =========================================================
   CHECKOUT
========================================================= */

function validateCheckOutForm() {

    const addressElement =
        document.getElementById("address");

    const mobileElement =
        document.getElementById("mobile");

    if (!addressElement || !mobileElement) {
        return false;
    }

    const address =
        addressElement.value.trim();

    const mobile =
        mobileElement.value.trim();

    const mobileRegex =
        /^09\d{9}$/;

    if (!address) {

        showFailAlert(
            "خطا",
            "لطفاً آدرس را وارد کنید."
        );

        return false;
    }

    if (address.length < 10) {

        showFailAlert(
            "خطا",
            "آدرس وارد شده خیلی کوتاه است."
        );

        return false;
    }

    if (!mobileRegex.test(mobile)) {

        showFailAlert(
            "خطا",
            "شماره موبایل وارد شده معتبر نیست."
        );

        return false;
    }

    return true;
}


/* =========================================================
   CART BADGE
========================================================= */

async function updateCartBadge() {

    try {

        const response =
            await fetch(
                "/Order/GetBasketCount",
                {
                    method: "GET",
                    headers: {
                        "X-Requested-With":
                            "XMLHttpRequest"
                    }
                }
            );

        if (!response.ok) {
            return;
        }

        const count =
            await response.json();

        const badgeDesktop =
            document.getElementById(
                "cart-badge-desktop"
            );

        const badgeMobile =
            document.getElementById(
                "cart-badge-mobile"
            );

        if (badgeDesktop) {
            badgeDesktop.textContent =
                count;

            badgeDesktop.setAttribute(
                "data-count",
                count
            );
        }

        if (badgeMobile) {
            badgeMobile.textContent =
                count;

            badgeMobile.setAttribute(
                "data-count",
                count
            );
        }

    } catch (error) {

        console.error(
            "Failed to update basket badge:",
            error
        );
    }
}


/* =========================================================
   COMMENTS
========================================================= */

async function addComment() {

    const productIdElement =
        document.getElementById("productId");

    const commentTextElement =
        document.getElementById("commentText");

    if (!productIdElement ||
        !commentTextElement) {
        return;
    }

    const productId =
        Number(productIdElement.value);

    const commentText =
        commentTextElement.value.trim();

    const token =
        getAntiForgeryToken();

    if (!productId || productId <= 0) {
        showFailAlert(
            "خطا",
            "محصول نامعتبر است."
        );
        return;
    }

    if (!commentText) {
        showFailAlert(
            "خطا",
            "متن نظر نمی‌تواند خالی باشد."
        );
        return;
    }

    if (commentText.length > 500) {
        showFailAlert(
            "خطا",
            "متن نظر نمی‌تواند بیشتر از 500 کاراکتر باشد."
        );
        return;
    }

    if (!token) {
        showFailAlert(
            "خطا",
            "توکن امنیتی یافت نشد."
        );
        return;
    }

    try {

        const response =
            await fetch(
                "/Product/AddProductComment",
                {
                    method: "POST",
                    headers: {
                        "Content-Type":
                            "application/json",

                        "RequestVerificationToken":
                            token,

                        "X-Requested-With":
                            "XMLHttpRequest"
                    },
                    body: JSON.stringify({
                        productId: productId,
                        text: commentText
                    })
                }
            );

        const data =
            await parseResponse(response);

        if (!data.res) {
            showFailAlert(
                "خطا",
                data.msg || "ثبت نظر انجام نشد."
            );
            return;
        }

        showSuccessAlert(
            "",
            data.msg || "نظر شما ثبت شد."
        );

        setTimeout(
            () => location.reload(),
            800
        );

    } catch (error) {

        console.error(error);

        showFailAlert(
            "خطا",
            error?.msg ||
            "خطای غیرمنتظره‌ای رخ داد."
        );
    }
}


/* =========================================================
   LOGIN ERROR
========================================================= */

function showLoginError() {

    const loginErrorDiv =
        document.getElementById("loginError");

    if (!loginErrorDiv) {
        return;
    }

    const errorMessage =
        loginErrorDiv.dataset.error;

    if (!errorMessage ||
        !errorMessage.trim()) {
        return;
    }

    Swal.fire({
        icon: "error",
        title: "خطا",
        text: errorMessage,
        confirmButtonText: "باشه"
    });
}


/* =========================================================
   SWEET ALERT
========================================================= */

function showSuccessAlert(
    title = "",
    text = "عملیات با موفقیت انجام شد."
) {
    Swal.fire({
        title: title,
        text: text,
        icon: "success",
        showConfirmButton: false,
        timer: 1200
    });
}


function showFailAlert(
    title = "خطا",
    text = "عملیات ناموفق بود."
) {
    Swal.fire({
        title: title,
        text: text,
        icon: "error",
        confirmButtonText: "باشه"
    });
}


/* =========================================================
   INITIALIZATION
========================================================= */

document.addEventListener(
    "DOMContentLoaded",
    function () {

        initializeQuantityInput();

        initializePasswordToggle();

        initializeProductModal();

        showLoginError();

        updateCartBadge();
    }
);