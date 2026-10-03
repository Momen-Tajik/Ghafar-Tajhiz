function getAntiForgeryToken() {
    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    );

    return token ? token.value : null;
}


async function State(basketId, status) {
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

    try {
        const response = await fetch(
            '/Orders/SetStateCommand',
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token,
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify({
                    BasketId: basketId,
                    Status: status
                })
            }
        );

        const contentType =
            response.headers.get('content-type') || '';

        let data;

        if (contentType.includes('application/json')) {
            data = await response.json();
        } else {
            throw new Error(
                'پاسخ نامعتبر از سرور دریافت شد.'
            );
        }

        if (!response.ok || !data.res) {
            throw new Error(
                data.msg || 'تغییر وضعیت سفارش انجام نشد.'
            );
        }

        showSuccessAlert(
            '',
            data.msg || 'وضعیت سفارش با موفقیت تغییر کرد.'
        );

        setTimeout(() => {
            window.location.reload();
        }, 1000);

    } catch (error) {
        console.error(error);

        showFailAlert(
            'خطا',
            error.message || 'عملیات با شکست مواجه شد.'
        );
    }
}


/* =========================================================
   SWEET ALERT
========================================================= */

function showSuccessAlert(
    title = '',
    text = 'عملیات با موفقیت انجام شد.'
) {
    Swal.fire({
        title: title,
        text: text,
        icon: 'success',
        showConfirmButton: false,
        timer: 1000
    });
}


function showFailAlert(
    title = 'خطا',
    text = 'عملیات ناموفق بود.'
) {
    Swal.fire({
        title: title,
        text: text,
        icon: 'error',
        confirmButtonText: 'باشه'
    });
}

   // Receipt SweetAlert

            function showReceipt(receiptUrl) {

                Swal.fire({

                    title: "رسید پرداخت",

                    imageUrl: receiptUrl,

                    imageAlt: "رسید پرداخت",

                    width: "900px",

                    showCloseButton: true,

                    showConfirmButton: false,

                    allowOutsideClick: true,

                    allowEscapeKey: true,

                    background: "#ffffff",

                    customClass: {
                        popup: "receipt-swal-popup",
                        image: "receipt-swal-image"
                    }

                });

    }


function refreshPage() {
    window.location.reload();
}