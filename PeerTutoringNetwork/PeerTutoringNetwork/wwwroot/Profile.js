import jwt_decode from 'jwt-decode';

const token = localStorage.getItem('jwtToken');
if (token) {
    const decoded = jwt_decode(token);
    console.log(decoded);
}
function getUserInfoFromToken(token) {
    try {
        const decoded = jwt_decode(token);
        const userId = decoded.userId;
        const roleId = decoded.roleId;
        return { userId, roleId };
    } catch (error) {
        console.error("Invalid token", error);
        return null;
    }
}

// Example usage:
const userInfo = getUserInfoFromToken(token);
console.log('User ID:', userInfo.userId);
console.log('Role ID:', userInfo.roleId);

//var userId = userInfo.userId; 
var userId = userInfo.roleId; 
// Funkcija za dohvaćanje profila i popunjavanje podataka
function fetchProfile() {
    fetch(`/api/User/GetProfile?userId=${userId}`)
        .then(response => response.json())
        .then(data => {
            // Popunjavanje forme podacima
            document.getElementById('username').value = data.username;
            document.getElementById('firstName').value = data.firstName;
            document.getElementById('lastName').value = data.lastName;
            document.getElementById('email').value = data.email;
            document.getElementById('phone').value = data.phone;

            // Prikaz korisničkog imena u navigaciji
            document.getElementById('username-navbar').innerText = data.username;
        })
        .catch(error => console.error('Error fetching profile:', error));
}

// Logout funkcija
function logout() {
    
    alert('Successfully logged out!');
    window.location.href = '/Login.html'; 
}

// Funkcija za ažuriranje profila 
function updateProfile(event) {
    event.preventDefault(); // Sprečava ponovno učitavanje stranice

    const updateDto = {
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        email: document.getElementById('email').value,
        phone: document.getElementById('phone').value
    };

    fetch('/api/User/UpdateProfile', {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(updateDto)
    })
        .then(response => {
            if (!response.ok) throw new Error('Failed to update profile');
            alert('Profile updated successfully');
        })
        .catch(error => console.error('Error updating profile:', error));
}


// Funkcija za brisanje neobaveznih podataka 
function clearOptionalData() {
    fetch(`/api/User/ClearOptionalData?userId=${userId}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (!response.ok) throw new Error('Failed to clear optional data');
            alert('Optional data cleared successfully');
            fetchProfile(); // Ponovno dohvaćanje podataka
        })
        .catch(error => console.error('Error clearing optional data:', error));
}

// Automatski učitaj podatke pri otvaranju stranice
fetchProfile();