function jwtLogin() {
    let loginUrl = "http://localhost:5173/api/User/Login";
    let loginData = {
        username: $("#username").val(),
        password: $("#password").val()
    };

    $.ajax({
        method: "POST",
        url: loginUrl,
        data: JSON.stringify(loginData),
        contentType: 'application/json'
    })
        .done(function (response) {
            // Pretpostavljamo da je token u polju response.token
            const token = response.token;

            // Spremi JWT token u localStorage
            localStorage.setItem("jwtToken", token);

            // Dekodiraj token i izvuci roleId za preusmjeravanje
            const decodedToken = jwt_decode(token);
            const roleId = decodedToken.roleId;

            // Preusmjeri prema ulozi
            if (roleId == 3) {
                window.location.href = "Admin.html"; // Admin
                console.log(roleId);
            }
            else if (roleId == 2) {
                window.location.href = "MentorDashboard/Index"; // Tutor
                console.log(roleId);
            }
            else if (roleId == 1) {
                window.location.href = "StudentDashboard/Index"; // Stavi StudentDashboard/Index MATKOOOOOO
                console.log(roleId);
            }
            else {
                alert("Unknown role ID: " + roleId);
                localStorage.removeItem("jwtToken");
            }
        })
        .fail(function () {
            alert("Username or password is incorrect.");
            localStorage.removeItem("jwtToken");
        });
}