function State(basketItemId, status) {
    const data = {
        BasketItemId: basketItemId,
        Status: status      
    };

    fetch('/Orders/SetStateCommand', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    })
        .then(res => res.json())
        .then(data => {
            showSuccessAlert('', 'وضعیت سفارش با موفقیت تغییر کرد');
            setTimeout(() => window.location.reload(), 1000);
        })
        .catch(err => {
            console.error(err.message);
            showFailAlert('خطا', 'عملیات با شکست مواجه شد');
        });
}

/* SWEET ALERT *//* SWEET ALERT *//* SWEET ALERT *//* SWEET ALERT */

function showSuccessAlert(title, text = 'موفق') {
    Swal.fire({
        title: title,
        text: text,
        icon: 'success',
        showConfirmButton: false,
        timer: 1000
    });
}

function showFailAlert(title, text = 'ناموفق') {
    Swal.fire({
        title: title,
        text: text,
        icon: 'error',
        confirmButtonText: 'باشه'
    });
}


function refreshPage() {
    window.location.reload();
}