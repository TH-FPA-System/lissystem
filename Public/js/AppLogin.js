$(document).ready(function () {
    $('#frmLogin').validate({
        rules: {
            account_name: "required",
            password: "required"
        },
        messages: {
            account_name: "Please specify account name!",
            password: "Please specify password!"
        }
    });
});