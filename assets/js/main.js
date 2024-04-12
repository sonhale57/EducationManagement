function showSuccess(content, time) {
    var myToast = document.getElementById('myToastSuccess');
    var toast = new bootstrap.Toast(myToast);

    // Change toast content
    var toastBody = myToast.querySelector('.toast-body');
    toastBody.textContent = content;

    // Show toast
    toast.show();

    // Auto-hide toast after specified time
    setTimeout(function () {
        toast.hide();
    }, time);
}
function showError(content, time) {
    var myToast = document.getElementById('myToastError');
    var toast = new bootstrap.Toast(myToast);

    // Change toast content
    var toastBody = myToast.querySelector('.toast-body');
    toastBody.textContent = content;

    // Show toast
    toast.show();

    // Auto-hide toast after specified time
    setTimeout(function () {
        toast.hide();
    }, time);
}
