$(function () {
    // Reusable global AJAX error handler.
    $(document).ajaxError(function (_event, xhr) {
        if (!xhr || !xhr.responseText) {
            return;
        }

        console.error('AJAX error:', xhr.responseText);
    });
});
