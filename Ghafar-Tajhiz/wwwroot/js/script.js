/* =========================================================
   NAVBAR
========================================================= */

document.addEventListener("DOMContentLoaded", function () {

    const navButton =
        document.getElementById("mobileNavbarBtn");

    const mobileNav =
        document.getElementById("desktopnavbar");

    if (!navButton || !mobileNav) {
        return;
    }


    navButton.addEventListener(
        "click",
        function () {

            const isOpen =
                mobileNav.classList.toggle("active");

            navButton.setAttribute(
                "aria-expanded",
                isOpen.toString()
            );


            const icon =
                navButton.querySelector("i");

            if (icon) {

                icon.classList.toggle(
                    "fa-bars",
                    !isOpen
                );

                icon.classList.toggle(
                    "fa-times",
                    isOpen
                );
            }
        }
    );


    // بستن منوی موبایل هنگام کلیک روی لینک
    mobileNav
        .querySelectorAll("a")
        .forEach(link => {

            link.addEventListener(
                "click",
                function () {

                    if (
                        window.innerWidth <= 760 &&
                        mobileNav.classList.contains("active")
                    ) {

                        mobileNav.classList.remove("active");

                        navButton.setAttribute(
                            "aria-expanded",
                            "false"
                        );


                        const icon =
                            navButton.querySelector("i");

                        if (icon) {

                            icon.classList.remove(
                                "fa-times"
                            );

                            icon.classList.add(
                                "fa-bars"
                            );
                        }
                    }
                }
            );
        });

});


/* =========================================================
   HELPERS
========================================================= */

function formatPrice(price) {

    const value =
        Number(price);

    if (Number.isNaN(value)) {
        return "۰";
    }

    return new Intl.NumberFormat("fa-IR")
        .format(value);
}


function getAntiForgeryToken() {

    const token =
        document.querySelector(
            '#antiForgeryForm input[name="__RequestVerificationToken"]'
        );

    return token
        ? token.value
        : null;
}


async function parseResponse(response) {

    const contentType =
        response.headers.get("content-type") || "";


    if (contentType.includes("application/json")) {

        const data =
            await response.json();


        if (!response.ok) {
            throw data;
        }


        return data;
    }


    const text =
        await response.text();


    if (!response.ok) {

        throw {
            msg:
                text ||
                "خطای غیرمنتظره‌ای رخ داد."
        };
    }


    return text;
}


function getFileValidationResult(file) {

    if (!file) {

        return {
            valid: false,
            message: "لطفاً یک فایل انتخاب کنید."
        };
    }


    const allowedTypes = [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];


    const maxFileSize =
        5 * 1024 * 1024;


    if (!allowedTypes.includes(file.type)) {

        return {
            valid: false,
            message:
                "فرمت فایل باید JPG، JPEG، PNG یا WEBP باشد."
        };
    }


    if (file.size > maxFileSize) {

        return {
            valid: false,
            message:
                "حجم فایل نمی‌تواند بیشتر از ۵ مگابایت باشد."
        };
    }


    return {
        valid: true,
        message: ""
    };
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


    if (
        Number.isNaN(basePrice) ||
        Number.isNaN(stockQuantity)
    ) {
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

            stockMessage.style.color =
                "#d32f2f";
        }

        return;
    }


    if (
        Number.isNaN(count) ||
        count < 1
    ) {
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

            stockMessage.style.color =
                "#ef6c00";

        } else {

            stockMessage.style.color =
                "#666";
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


    if (
        !countInput ||
        countInput.disabled
    ) {
        return;
    }


    countInput.addEventListener(
        "input",
        updatePrice
    );


    countInput.addEventListener(
        "change",
        updatePrice
    );


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


    if (
        !toggleButton ||
        !passwordInput
    ) {
        return;
    }


    toggleButton.addEventListener(
        "click",
        function () {

            const isPassword =
                passwordInput.type === "password";


            passwordInput.type =
                isPassword
                    ? "text"
                    : "password";


            toggleButton.setAttribute(
                "aria-label",
                isPassword
                    ? "مخفی کردن رمز عبور"
                    : "نمایش رمز عبور"
            );
        }
    );
}


/* =========================================================
   PRODUCT MODAL
========================================================= */

function closeProductModal() {

    const modal =
        document.getElementById("modal");


    if (!modal) {
        return;
    }


    modal.style.display =
        "none";


    modal.setAttribute(
        "aria-hidden",
        "true"
    );
}


function initializeProductModal() {

    const modal =
        document.getElementById("modal");

    const modalBody =
        document.getElementById("modalBody");

    const closeButton =
        document.getElementById("closeModal");


    if (
        !modal ||
        !modalBody
    ) {
        return;
    }


    document
        .querySelectorAll(".openModal")
        .forEach(button => {

            button.addEventListener(
                "click",
                async function () {

                    const productId =
                        Number(
                            this.dataset.productId
                        );


                    if (
                        !productId ||
                        productId <= 0
                    ) {
                        return;
                    }


                    modal.style.display =
                        "block";


                    modal.setAttribute(
                        "aria-hidden",
                        "false"
                    );


                    modalBody.innerHTML =
                        `
                        <div class="text-center">
                            در حال دریافت اطلاعات...
                        </div>
                        `;


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


                        modalBody.innerHTML =
                            html;

                    }
                    catch (error) {

                        console.error(
                            "Product Modal Error:",
                            error
                        );


                        modalBody.innerHTML =
                            `
                            <div class="text-danger text-center">
                                خطا در دریافت اطلاعات محصول
                            </div>
                            `;
                    }
                }
            );
        });


    if (closeButton) {

        closeButton.addEventListener(
            "click",
            closeProductModal
        );
    }


    window.addEventListener(
        "click",
        function (event) {

            if (event.target === modal) {
                closeProductModal();
            }
        }
    );


    document.addEventListener(
        "keydown",
        function (event) {

            if (event.key === "Escape") {
                closeProductModal();
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


    if (
        !productIdElement ||
        !qtyElement
    ) {
        return;
    }


    const productId =
        Number(productIdElement.value);


    const qty =
        Number(qtyElement.value);


    if (
        !productId ||
        productId <= 0
    ) {

        showFailAlert(
            "خطا",
            "محصول نامعتبر است."
        );

        return;
    }


    if (
        !qty ||
        qty <= 0
    ) {

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
                        productId:
                            productId,

                        qty:
                            qty
                    })
                }
            );


        if (response.status === 401) {

            showFailAlert(
                "نیاز به ورود",
                "ابتدا وارد حساب کاربری خود شوید."
            );

            return;
        }


        if (response.status === 403) {

            showFailAlert(
                "دسترسی غیرمجاز",
                "شما اجازه انجام این عملیات را ندارید."
            );

            return;
        }


        const data =
            await parseResponse(response);


        if (!data.res) {

            showFailAlert(
                "خطا",
                data.msg ||
                "افزودن به سبد خرید انجام نشد."
            );

            return;
        }


        showSuccessAlert(
            "",
            data.msg ||
            "محصول به سبد خرید اضافه شد."
        );


        await updateCartBadge();

    }
    catch (error) {

        console.error(
            "AddToBasket Error:",
            error
        );


        showFailAlert(
            "خطا",
            error?.msg ||
            "خطای غیرمنتظره‌ای رخ داد."
        );
    }
}


async function RemoveBasketItem(id) {

    if (
        !id ||
        id <= 0
    ) {
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
                        basketItemId:
                            id
                    })
                }
            );


        if (response.status === 401) {

            showFailAlert(
                "نیاز به ورود",
                "ابتدا وارد حساب کاربری خود شوید."
            );

            return;
        }


        if (response.status === 403) {

            showFailAlert(
                "دسترسی غیرمجاز",
                "شما اجازه انجام این عملیات را ندارید."
            );

            return;
        }


        const data =
            await parseResponse(response);


        if (!data.res) {

            showFailAlert(
                "خطا",
                data.msg ||
                "حذف محصول انجام نشد."
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
            data.msg ||
            "محصول حذف شد."
        );

    }
    catch (error) {

        console.error(
            "RemoveBasketItem Error:",
            error
        );


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

    const receiptElement =
        document.getElementById("receipt");


    if (
        !addressElement ||
        !mobileElement ||
        !receiptElement
    ) {
        return false;
    }


    const address =
        addressElement.value.trim();


    const mobile =
        mobileElement.value.trim();


    const receipt =
        receiptElement.files[0];


    const mobileRegex =
        /^09\d{9}$/;


    // ==============================
    // Address
    // ==============================

    if (!address) {

        showFailAlert(
            "خطا",
            "لطفاً آدرس را وارد کنید."
        );

        return false;
    }


    if (
        address.length < 10
    ) {

        showFailAlert(
            "خطا",
            "آدرس وارد شده خیلی کوتاه است."
        );

        return false;
    }


    if (
        address.length > 500
    ) {

        showFailAlert(
            "خطا",
            "آدرس نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد."
        );

        return false;
    }


    // ==============================
    // Mobile
    // ==============================

    if (
        !mobileRegex.test(mobile)
    ) {

        showFailAlert(
            "خطا",
            "شماره موبایل وارد شده معتبر نیست."
        );

        return false;
    }


    // ==============================
    // Receipt
    // ==============================

    const fileValidation =
        getFileValidationResult(
            receipt
        );


    if (!fileValidation.valid) {

        showFailAlert(
            "خطا",
            fileValidation.message
        );


        if (receiptElement) {
            receiptElement.value = "";
        }


        return false;
    }


    return true;
}


/* =========================================================
   RESUBMIT RECEIPT
========================================================= */

function initializeResubmitReceiptForms() {

    const forms =
        document.querySelectorAll(
            ".resubmit-form"
        );


    if (!forms.length) {
        return;
    }


    forms.forEach(form => {

        const fileInput =
            form.querySelector(
                'input[type="file"][name="Receipt"]'
            );


        const fileLabel =
            form.querySelector(
                ".receipt-file-text strong"
            );


        const fileDescription =
            form.querySelector(
                ".receipt-file-text small"
            );


        if (!fileInput) {
            return;
        }


        fileInput.addEventListener(
            "change",
            function () {

                const file =
                    this.files?.[0];


                if (!file) {
                    return;
                }


                const validation =
                    getFileValidationResult(
                        file
                    );


                if (!validation.valid) {

                    showFailAlert(
                        "فایل نامعتبر",
                        validation.message
                    );


                    this.value =
                        "";


                    return;
                }


                if (fileLabel) {

                    fileLabel.textContent =
                        file.name;
                }


                if (fileDescription) {

                    const sizeInMb =
                        (
                            file.size /
                            (1024 * 1024)
                        ).toFixed(2);


                    fileDescription.textContent =
                        `${sizeInMb} MB`;
                }
            }
        );


        form.addEventListener(
            "submit",
            function (event) {

                const file =
                    fileInput.files?.[0];


                const validation =
                    getFileValidationResult(
                        file
                    );


                if (!validation.valid) {

                    event.preventDefault();


                    showFailAlert(
                        "خطا",
                        validation.message
                    );
                }
            }
        );

    });
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


        if (
            response.status === 401 ||
            response.status === 403
        ) {
            return;
        }


        if (!response.ok) {
            return;
        }


        const count =
            await response.json();


        const desktopBadge =
            document.getElementById(
                "cart-badge-desktop"
            );


        const mobileBadge =
            document.getElementById(
                "cart-badge-mobile"
            );


        if (desktopBadge) {

            desktopBadge.textContent =
                count;


            desktopBadge.setAttribute(
                "data-count",
                count
            );
        }


        if (mobileBadge) {

            mobileBadge.textContent =
                count;


            mobileBadge.setAttribute(
                "data-count",
                count
            );
        }

    }
    catch (error) {

        console.error(
            "Cart Badge Error:",
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


    if (
        !productIdElement ||
        !commentTextElement
    ) {
        return;
    }


    const productId =
        Number(productIdElement.value);


    const commentText =
        commentTextElement.value.trim();


    const token =
        getAntiForgeryToken();


    if (
        !productId ||
        productId <= 0
    ) {

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


    if (
        commentText.length > 500
    ) {

        showFailAlert(
            "خطا",
            "متن نظر نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد."
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
                        productId:
                            productId,

                        text:
                            commentText
                    })
                }
            );


        if (
            response.status === 401
        ) {

            let message =
                "لطفاً ابتدا وارد حساب کاربری شوید.";


            try {

                const errorData =
                    await response.json();


                if (errorData.msg) {
                    message =
                        errorData.msg;
                }

            }
            catch {
                // پیام پیش‌فرض باقی می‌ماند
            }


            showFailAlert(
                "نیاز به ورود",
                message
            );


            return;
        }


        if (
            response.status === 403
        ) {

            showFailAlert(
                "دسترسی غیرمجاز",
                "شما اجازه انجام این عملیات را ندارید."
            );


            return;
        }


        const data =
            await parseResponse(response);


        if (!data.res) {

            showFailAlert(
                "خطا",
                data.msg ||
                "ثبت نظر انجام نشد."
            );


            return;
        }


        showSuccessAlert(
            "",
            data.msg ||
            "نظر شما با موفقیت ثبت شد."
        );


        setTimeout(
            () => location.reload(),
            800
        );

    }
    catch (error) {

        console.error(
            "AddComment Error:",
            error
        );


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


    if (
        !errorMessage ||
        !errorMessage.trim()
    ) {
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

        title:
            title,

        text:
            text,

        icon:
            "success",

        showConfirmButton:
            false,

        timer:
            1200
    });
}


function showFailAlert(
    title = "خطا",
    text = "عملیات ناموفق بود."
) {

    Swal.fire({

        title:
            title,

        text:
            text,

        icon:
            "error",

        confirmButtonText:
            "باشه"
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

        initializeResubmitReceiptForms();

        showLoginError();

        updateCartBadge();
    }
);