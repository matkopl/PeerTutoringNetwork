const jwtDecode = '../node_modules/jwt-decode/build/cjs/index.js';

// Dohvati token iz LocalStorage
const token = localStorage.getItem('jwtToken');

if (!token) {
    alert('You are not logged in!');
    window.location.href = '/Login.html'; // Preusmjeri na login ako token nije prisutan
}

// Dekodiranje tokena i dohvaćanje userId
function getUserInfoFromToken(token) {
    try {
        const decoded = jwt_decode(token);
        return { userId: decoded.userId, roleId: decoded.roleId };
    } catch (error) {
        console.error("Invalid token", error);
        return null;
    }
}
const userInfo = getUserInfoFromToken(token);
if (!userInfo) {
    alert('Invalid session. Please log in again.');
    window.location.href = '/Login.html';
}

const userId = userInfo.roleId;

// Dohvati korisničke podatke
/*function fetchProfile() {
    fetch(`/api/User/GetProfile?userId=${userId}`, {
        headers: { 'Authorization': `Bearer ${token}` }
    })
        .then(response => {
            if (!response.ok) throw new Error('Failed to fetch profile');
            return response.json();
        })
        .then(data => {
            // Popuni formu podacima
            document.getElementById('username').value = data.username;
            document.getElementById('firstName').value = data.firstName;
            document.getElementById('lastName').value = data.lastName;
            document.getElementById('email').value = data.email;
            document.getElementById('phone').value = data.phone;
        })
        .catch(error => console.error('Error fetching profile:', error));
}*/
function fetchProfile() {
    fetch('/api/User/GetProfile', {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        }
    })
    .then(response => {
        if (!response.ok) throw new Error('Failed to fetch profile');
        return response.json();
    })
        .then(data => {// Popuni formu s podacima korisnika
            document.getElementById('username').value = data.username;
            document.getElementById('firstName').value = data.firstName;
            document.getElementById('lastName').value = data.lastName;
            document.getElementById('email').value = data.email;
            document.getElementById('phone').value = data.phone;
        })
        .catch(error => console.error('Error fetching profile:', error));
}
// Ažuriraj korisničke podatke
function updateProfile(event) {
    event.preventDefault();

    const updateDto = {
        userId: userInfo.userId,
        username: document.getElementById('username').value,
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        email: document.getElementById('email').value,
        phone: document.getElementById('phone').value
    };

    fetch('/api/User/UpdateProfile', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(updateDto)
    })
        .then(response => {
            if (!response.ok) throw new Error('Failed to update profile');
            alert('Profile updated successfully');
        })
        .catch(error => console.error('Error updating profile:', error));
}
// Brisanje neobaveznih podataka
function clearOptionalData() {
    fetch(`/api/User/ClearOptionalData?userId=${userInfo.userId}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
    })
        .then(response => {
            if (!response.ok) throw new Error('Failed to clear optional data');
            alert('Optional data cleared successfully');
            fetchProfile();
        })
        .catch(error => console.error('Error clearing optional data:', error));
}

// Logout funkcija
function logout() {
    localStorage.removeItem('jwtToken');
    alert('Successfully logged out!');
    window.location.href = '/Login.html';
}

// Automatski učitaj podatke pri otvaranju stranice
fetchProfile();
