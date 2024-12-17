function jwtLogin() {
    let loginUrl = "http://localhost:5173/api/User/Login";
    let loginData = {
        "username": $("#username").val(),
        "password": $("#password").val()
    }
    $.ajax({
        method: "POST",
        url: loginUrl,
        data: JSON.stringify(loginData),
        contentType: 'application/json'
    }).done(function (tokenData) {
        //console.log(tokenData);
        localStorage.setItem("JWT", tokenData);

        // redirect
        window.location.href = "Views/MentorDashboard/Index.cshtml";
    }).fail(function (err) {
        alert(err.responseText);
        localStorage.removeItem("JWT");
    });
}