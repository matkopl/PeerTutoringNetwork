const userId = 1; // Pretpostavljamo da imamo userId iz sesije ili tokena

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
    // Ovde možete dodati brisanje tokena iz localStorage-a ili cookie-a
    alert('Successfully logged out!');
    window.location.href = '/Login.html'; // Preusmeravanje na login stranicu
}

// Funkcija za ažuriranje profila (ostaje ista)
function updateProfile(event) {
    event.preventDefault();

    const updateDto = {
        userId: userId,
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

// Funkcija za brisanje neobaveznih podataka (ostaje ista)
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