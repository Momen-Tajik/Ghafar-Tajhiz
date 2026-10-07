/* =========================================================
   ADMIN UI HELPERS
   ========================================================= */

(function () {
    'use strict';

    function normalizeText(value) {
        return String(value || '')
            .replace(/[۰-۹]/g, function (digit) {
                return '۰۱۲۳۴۵۶۷۸۹'.indexOf(digit);
            })
            .replace(/[٠-٩]/g, function (digit) {
                return '٠١٢٣٤٥٦٧٨٩'.indexOf(digit);
            })
            .replace(/ي/g, 'ی')
            .replace(/ك/g, 'ک')
            .replace(/[\u200c\u200f\u200e]/g, ' ')
            .replace(/\s+/g, ' ')
            .trim()
            .toLowerCase();
    }

    window.normalizeAdminText = normalizeText;
})();


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
    const contentType = response.headers.get('content-type') || '';

    if (!contentType.includes('application/json')) {
        throw new Error('پاسخ نامعتبر از سرور دریافت شد.');
    }

    const data = await response.json();

    if (!response.ok || !data.res) {
        throw new Error(data.msg || 'عملیات انجام نشد.');
    }

    return data;
}


/* =========================================================
   SWEET ALERT
   ========================================================= */

const adminSwalBase = {
    confirmButtonText: 'تأیید',
    cancelButtonText: 'انصراف',
    reverseButtons: true,
    customClass: {
        popup: 'admin-swal-popup',
        title: 'admin-swal-title',
        htmlContainer: 'admin-swal-text',
        confirmButton: 'admin-swal-confirm',
        cancelButton: 'admin-swal-cancel'
    },
    buttonsStyling: true
};


/* =========================================================
   SUCCESS ALERT
   ========================================================= */

function showSuccessAlert(
    title = 'موفق',
    text = 'عملیات با موفقیت انجام شد.'
) {
    return Swal.fire({
        ...adminSwalBase,
        title: title,
        text: text,
        icon: 'success'
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
        ...adminSwalBase,
        title: title,
        text: text,
        icon: 'error'
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
        timer: 2800,
        timerProgressBar: true,
        customClass: {
            popup: 'admin-toast'
        }
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
        timer: 3500,
        timerProgressBar: true,
        customClass: {
            popup: 'admin-toast'
        }
    });
}


/* =========================================================
   APPROVE PAYMENT
   ========================================================= */

async function ApprovePayment(basketId) {
    if (!basketId || basketId <= 0) {
        showFailAlert('خطا', 'شناسه سفارش نامعتبر است.');
        return;
    }

    const token = getAntiForgeryToken();

    if (!token) {
        showFailAlert('خطا', 'توکن امنیتی پیدا نشد.');
        return;
    }

    const result = await Swal.fire({
        ...adminSwalBase,
        title: 'تأیید پرداخت',
        text: 'آیا از تأیید پرداخت این سفارش مطمئن هستید؟',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'بله، تأیید کن',
        cancelButtonText: 'انصراف',
        focusCancel: true
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

        const data = await handleApiResponse(response);

        await showSuccessAlert('پرداخت تأیید شد', data.msg);
        window.location.reload();
    }
    catch (error) {
        console.error(error);

        showFailAlert(
            'خطا',
            error.message || 'تأیید پرداخت انجام نشد.'
        );
    }
}


/* =========================================================
   REJECT PAYMENT
   ========================================================= */

async function RejectPayment(basketId) {
    if (!basketId || basketId <= 0) {
        showFailAlert('خطا', 'شناسه سفارش نامعتبر است.');
        return;
    }

    const token = getAntiForgeryToken();

    if (!token) {
        showFailAlert('خطا', 'توکن امنیتی پیدا نشد.');
        return;
    }

    const result = await Swal.fire({
        ...adminSwalBase,
        title: 'رد پرداخت',
        input: 'textarea',
        inputLabel: 'دلیل رد پرداخت',
        inputPlaceholder: 'دلیل رد پرداخت را وارد کنید...',
        inputAttributes: {
            maxlength: '500'
        },
        showCancelButton: true,
        confirmButtonText: 'رد پرداخت',
        cancelButtonText: 'انصراف',
        focusCancel: true,
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

    const reason = result.value.trim();

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

        const data = await handleApiResponse(response);

        await showSuccessAlert('پرداخت رد شد', data.msg);
        window.location.reload();
    }
    catch (error) {
        console.error(error);

        showFailAlert(
            'خطا',
            error.message || 'رد پرداخت انجام نشد.'
        );
    }
}


/* =========================================================
   SHIP ORDER
   ========================================================= */

async function ShipOrder(basketId) {
    if (!basketId || basketId <= 0) {
        showFailAlert('خطا', 'شناسه سفارش نامعتبر است.');
        return;
    }

    const token = getAntiForgeryToken();

    if (!token) {
        showFailAlert('خطا', 'توکن امنیتی پیدا نشد.');
        return;
    }

    const result = await Swal.fire({
        ...adminSwalBase,
        title: 'ارسال سفارش',
        text: 'آیا مطمئن هستید که این سفارش ارسال شده است؟',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'بله، ارسال شد',
        cancelButtonText: 'انصراف',
        focusCancel: true
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

        const data = await handleApiResponse(response);

        await showSuccessAlert('سفارش ارسال شد', data.msg);
        window.location.reload();
    }
    catch (error) {
        console.error(error);

        showFailAlert(
            'خطا',
            error.message || 'ارسال سفارش انجام نشد.'
        );
    }
}


/* =========================================================
   DELETE CONFIRMATION
   ========================================================= */

document.addEventListener('submit', async function (event) {
    const form = event.target.closest('.delete-confirm-form');

    if (!form) {
        return;
    }

    event.preventDefault();

    const itemName = form.dataset.itemName || 'این مورد';

    const result = await Swal.fire({
        ...adminSwalBase,
        title: 'حذف مورد',
        text: `آیا از حذف «${itemName}» مطمئن هستید؟ این عملیات قابل بازگشت نیست.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'بله، حذف کن',
        cancelButtonText: 'انصراف',
        focusCancel: true
    });

    if (!result.isConfirmed) {
        return;
    }

    const submitButton = form.querySelector('button[type="submit"]');

    if (submitButton) {
        submitButton.disabled = true;
        submitButton.innerHTML = '<i class="fa fa-spinner fa-spin"></i>';
    }

    form.submit();
});


/* =========================================================
   SHOW RECEIPT
   ========================================================= */

function showReceipt(receiptUrl) {
    if (!receiptUrl) {
        showFailAlert('خطا', 'آدرس رسید موجود نیست.');
        return;
    }

    Swal.fire({
        ...adminSwalBase,
        title: 'رسید پرداخت',
        imageUrl: receiptUrl,
        imageAlt: 'رسید پرداخت',
        width: 'min(900px, 95vw)',
        showConfirmButton: false,
        showCloseButton: true,
        allowOutsideClick: true,
        allowEscapeKey: true,
        customClass: {
            popup: 'receipt-swal-popup',
            image: 'receipt-swal-image',
            title: 'admin-swal-title'
        }
    });
}


/* =========================================================
   PASSWORD TOGGLE
   ========================================================= */

document.addEventListener('click', function (event) {
    const button = event.target.closest('[data-password-toggle]');

    if (!button) {
        return;
    }

    const inputId = button.getAttribute('data-password-toggle');
    const input = document.getElementById(inputId);

    if (!input) {
        return;
    }

    const icon = button.querySelector('i');
    const shouldShow = input.type === 'password';

    input.type = shouldShow ? 'text' : 'password';

    if (icon) {
        icon.classList.toggle('fa-eye', !shouldShow);
        icon.classList.toggle('fa-eye-slash', shouldShow);
    }

    button.setAttribute(
        'aria-label',
        shouldShow ? 'پنهان کردن رمز عبور' : 'نمایش رمز عبور'
    );

    button.setAttribute(
        'title',
        shouldShow ? 'پنهان کردن رمز عبور' : 'نمایش رمز عبور'
    );
});


/* =========================================================
   USER TABLE SEARCH
   ========================================================= */

document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.querySelector('[data-user-search]');
    const clearButton = document.querySelector('[data-search-clear]');
    const rows = Array.from(document.querySelectorAll('[data-user-row]'));
    const emptyRow = document.querySelector('[data-user-empty]');
    const countElement = document.getElementById('usersCount');

    if (!searchInput || !rows.length) {
        return;
    }

    function updateSearch() {
        const query = window.normalizeAdminText(searchInput.value);
        let visibleCount = 0;

        rows.forEach(function (row) {
            const searchableText = window.normalizeAdminText(
                row.getAttribute('data-search') || row.textContent
            );

            const matched = !query || searchableText.includes(query);

            row.classList.toggle('d-none', !matched);

            if (matched) {
                visibleCount++;
            }
        });

        if (emptyRow) {
            emptyRow.classList.toggle('d-none', visibleCount !== 0);
        }

        if (countElement) {
            countElement.textContent = visibleCount;
        }

        if (clearButton) {
            clearButton.classList.toggle('is-visible', Boolean(searchInput.value));
        }
    }

    searchInput.addEventListener('input', updateSearch);

    if (clearButton) {
        clearButton.addEventListener('click', function () {
            searchInput.value = '';
            searchInput.focus();
            updateSearch();
        });
    }

    updateSearch();
});


/* =========================================================
   FORM SUBMIT FEEDBACK
   ========================================================= */

document.addEventListener('submit', function (event) {
    const form = event.target.closest('form[data-show-loading]');

    if (!form) {
        return;
    }

    const button = form.querySelector('[type="submit"]');

    if (!button || button.disabled) {
        return;
    }

    button.dataset.originalHtml = button.innerHTML;
    button.disabled = true;
    button.innerHTML = '<i class="fa fa-spinner fa-spin"></i> در حال پردازش...';
});


/* =========================================================
   REFRESH PAGE
   ========================================================= */

function refreshPage() {
    window.location.reload();
}
