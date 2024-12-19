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

        // Do login after registration
       
    }).fail(function (err) {
        alert(err.responseText);

        localStorage.removeItem("JWT");
    });
}
