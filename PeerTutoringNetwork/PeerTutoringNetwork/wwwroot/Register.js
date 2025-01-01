function register() {   
    let registerUrl = "http://localhost:5173/api/User/Register";
    let registerData = {
        "firstName": $("#firstName").val(),
        "lastName": $("#lastName").val(),
        "email": $("#email").val(),
        "phone": $("#phone").val(),
        "username": $("#username").val(),
        "roleId": $("#roleId").val(),
        "password": $("#password").val()
    }
    $.ajax({
        method: "POST",
        url: registerUrl,
        data: JSON.stringify(registerData),
        contentType: 'application/json'
    }).done(function (tokenData) {
        console.log(tokenData);

        alert("Succesful registration.")
        window.location.href = "Login.html";
       
    }).fail(function (err) {
        alert(err.responseText);

        localStorage.removeItem("JWT");
    });
}
