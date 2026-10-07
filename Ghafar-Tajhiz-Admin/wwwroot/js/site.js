/* =========================================================
   ANTI FORGERY
========================================================= */

function getAntiForgeryToken() {

    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    );

    return token ? token.value : null;
}


/* =========================================================
   API RESPONSE
========================================================= */

async function handleApiResponse(response) {

    const contentType =
        response.headers.get('content-type') || '';

    if (!contentType.includes('application/json')) {

        throw new Error(
            'پاسخ نامعتبر از سرور دریافت شد.'
        );
    }

    const data = await response.json();

    if (!response.ok || !data.res) {

        throw new Error(
            data.msg || 'عملیات انجام نشد.'
        );
    }

    return data;
}


/* =========================================================
   SUCCESS ALERT
========================================================= */

function showSuccessAlert(
    title = 'موفق',
    text = 'عملیات با موفقیت انجام شد.'
) {

    return Swal.fire({
        title: title,
        text: text,
        icon: 'success',
        confirmButtonText: 'باشه'
    });
}


/* =========================================================
   ERROR ALERT
========================================================= */

function showFailAlert(
    title = 'خطا',
    text = 'عملیات ناموفق بود.'
) {

    return Swal.fire({
        title: title,
        text: text,
        icon: 'error',
        confirmButtonText: 'باشه'
    });
}


/* =========================================================
   SUCCESS TOAST
========================================================= */

function showSuccessToast(
    text = 'عملیات با موفقیت انجام شد.'
) {

    return Swal.fire({
        toast: true,
        position: 'top-start',
        icon: 'success',
        title: text,
        showConfirmButton: false,
        timer: 2500,
        timerProgressBar: true
    });
}


/* =========================================================
   ERROR TOAST
========================================================= */

function showErrorToast(
    text = 'عملیات ناموفق بود.'
) {

    return Swal.fire({
        toast: true,
        position: 'top-start',
        icon: 'error',
        title: text,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    });
}


/* =========================================================
   APPROVE PAYMENT
========================================================= */

async function ApprovePayment(basketId) {

    if (!basketId || basketId <= 0) {

        showFailAlert(
            'خطا',
            'شناسه سفارش نامعتبر است.'
        );

        return;
    }

    const token = getAntiForgeryToken();

    if (!token) {

        showFailAlert(
            'خطا',
            'توکن امنیتی پیدا نشد.'
        );

        return;
    }

    const result = await Swal.fire({

        title: 'تأیید پرداخت',

        text:
            'آیا از تأیید پرداخت این سفارش مطمئن هستید؟',

        icon: 'question',

        showCancelButton: true,

        confirmButtonText: 'بله، تأیید کن',
        cancelButtonText: 'انصراف',

        reverseButtons: true
    });

    if (!result.isConfirmed) {
        return;
    }

    try {

        const response = await fetch(
            `/Orders/ApprovePayment?id=${basketId}`,
            {
                method: 'POST',

                headers: {
                    'RequestVerificationToken': token,
                    'X-Requested-With': 'XMLHttpRequest'
                }
            }
        );

        const data =
            await handleApiResponse(response);

        await showSuccessAlert(
            'پرداخت تأیید شد',
            data.msg
        );

        window.location.reload();

    }
    catch (error) {

        console.error(error);

        showFailAlert(
            'خطا',
            error.message ||
            'تأیید پرداخت انجام نشد.'
        );
    }
}


/* =========================================================
   REJECT PAYMENT
========================================================= */

async function RejectPayment(basketId) {

    if (!basketId || basketId <= 0) {

        showFailAlert(
            'خطا',
            'شناسه سفارش نامعتبر است.'
        );

        return;
    }

    const token = getAntiForgeryToken();

    if (!token) {

        showFailAlert(
            'خطا',
            'توکن امنیتی پیدا نشد.'
        );

        return;
    }

    const result = await Swal.fire({

        title: 'رد پرداخت',

        input: 'textarea',

        inputLabel: 'دلیل رد پرداخت',

        inputPlaceholder:
            'دلیل رد پرداخت را وارد کنید...',

        inputAttributes: {
            maxlength: '500'
        },

        showCancelButton: true,

        confirmButtonText: 'رد پرداخت',
        cancelButtonText: 'انصراف',

        reverseButtons: true,

        inputValidator: (value) => {

            if (!value || !value.trim()) {

                return 'وارد کردن دلیل رد پرداخت الزامی است.';
            }

            if (value.trim().length > 500) {

                return 'دلیل رد پرداخت نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.';
            }

            return undefined;
        }
    });

    if (!result.isConfirmed) {
        return;
    }

    const reason =
        result.value.trim();

    try {

        const response = await fetch(
            '/Orders/RejectPayment',
            {
                method: 'POST',

                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token,
                    'X-Requested-With': 'XMLHttpRequest'
                },

                body: JSON.stringify({
                    basketId: basketId,
                    reason: reason
                })
            }
        );

        const data =
            await handleApiResponse(response);

        await showSuccessAlert(
            'پرداخت رد شد',
            data.msg
        );

        window.location.reload();

    }
    catch (error) {

        console.error(error);

        showFailAlert(
            'خطا',
            error.message ||
            'رد پرداخت انجام نشد.'
        );
    }
}


/* =========================================================
   SHIP ORDER
========================================================= */

async function ShipOrder(basketId) {

    if (!basketId || basketId <= 0) {

        showFailAlert(
            'خطا',
            'شناسه سفارش نامعتبر است.'
        );

        return;
    }

    const token = getAntiForgeryToken();

    if (!token) {

        showFailAlert(
            'خطا',
            'توکن امنیتی پیدا نشد.'
        );

        return;
    }

    const result = await Swal.fire({

        title: 'ارسال سفارش',

        text:
            'آیا مطمئن هستید که این سفارش ارسال شده است؟',

        icon: 'question',

        showCancelButton: true,

        confirmButtonText: 'بله، ارسال شد',
        cancelButtonText: 'انصراف',

        reverseButtons: true
    });

    if (!result.isConfirmed) {
        return;
    }

    try {

        const response = await fetch(
            `/Orders/ShipOrder?id=${basketId}`,
            {
                method: 'POST',

                headers: {
                    'RequestVerificationToken': token,
                    'X-Requested-With': 'XMLHttpRequest'
                }
            }
        );

        const data =
            await handleApiResponse(response);

        await showSuccessAlert(
            'سفارش ارسال شد',
            data.msg
        );

        window.location.reload();

    }
    catch (error) {

        console.error(error);

        showFailAlert(
            'خطا',
            error.message ||
            'ارسال سفارش انجام نشد.'
        );
    }
}


/* =========================================================
   DELETE CONFIRMATION
========================================================= */

document.addEventListener(
    'submit',
    async function (event) {

        const form =
            event.target.closest('.delete-confirm-form');

        if (!form) {
            return;
        }

        event.preventDefault();

        const itemName =
            form.dataset.itemName || 'این مورد';

        const result = await Swal.fire({

            title: 'حذف مورد',

            text:
                `آیا از حذف «${itemName}» مطمئن هستید؟`,

            icon: 'warning',

            showCancelButton: true,

            confirmButtonText:
                'بله، حذف کن',

            cancelButtonText:
                'انصراف',

            reverseButtons: true,

            focusCancel: true

        });

        if (!result.isConfirmed) {
            return;
        }

        const submitButton =
            form.querySelector(
                'button[type="submit"]'
            );

        if (submitButton) {

            submitButton.disabled = true;

            submitButton.innerHTML =
                '<i class="fa fa-spinner fa-spin"></i> در حال حذف...';
        }

        form.submit();
    }
);

/* =========================================================
   SHOW RECEIPT
========================================================= */

function showReceipt(receiptUrl) {

    if (!receiptUrl) {

        showFailAlert(
            'خطا',
            'آدرس رسید موجود نیست.'
        );

        return;
    }

    Swal.fire({

        title: 'رسید پرداخت',

        imageUrl: receiptUrl,

        imageAlt: 'رسید پرداخت',

        width: '900px',

        showCloseButton: true,

        showConfirmButton: false,

        allowOutsideClick: true,

        allowEscapeKey: true,

        background: '#ffffff',

        customClass: {
            popup: 'receipt-swal-popup',
            image: 'receipt-swal-image'
        }
    });
}


/* =========================================================
   REFRESH PAGE
========================================================= */

function refreshPage() {
    window.location.reload();
}