// Fetch and display statistics
function fetchStatistics() {
    fetch('/api/User/GetAllUsers', {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}` // Prosljeđivanje JWT-a
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch users');
            }
            return response.json();
        })
        .then(users => {
            // Prikazuje broj korisnika
            const userCount = users.length;
            document.getElementById('user-count').innerText = `Trenutni broj korisnika: ${userCount}`;
        })
        .catch(error => console.error('Error fetching statistics:', error));
}

function fetchRoleStatistics() {
    fetch('/api/Admin/GetRoleStatistics', {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        }
    })
        .then(response => {
            if (!response.ok) throw new Error('Failed to fetch role statistics');
            return response.json();
        })
        .then(data => {
            // Pronađi broj studenata i teachera
            const studentCount = data.find(role => role.roleId === 1)?.count || 0; // RoleId 1 = Student
            const teacherCount = data.find(role => role.roleId === 2)?.count || 0; // RoleId 2 = Teacher

            // Ažuriraj statistiku na stranici
            document.getElementById('student-count').innerText = `Broj studenata: ${studentCount}`;
            document.getElementById('teacher-count').innerText = `Broj teachera: ${teacherCount}`;
        })
        .catch(error => console.error('Error fetching role statistics:', error));
}

// Fetch and display users
function fetchUsers() {
    fetch('/api/User/GetAllUsers', {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}` // Prosljeđivanje JWT-a
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch users');
            }
            return response.json();
        })
        .then(data => {
            const userTableBody = document.getElementById('user-table').querySelector('tbody');
            userTableBody.innerHTML = ''; // Očisti tablicu prije popunjavanja

            data.forEach(user => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${user.id}</td>
                    <td>${user.username}</td>
                    <td>${user.email}</td>
                    <td>${user.firstName}</td>
                    <td>${user.lastName}</td>
                    <td>${user.role}</td>
                    <td>
                        <button class="edit-button" onclick="editUser(${user.id})">Edit</button>
                        <button class="delete-button" onclick="deleteUser(${user.id})">Delete</button>
                   </td>
                `;
                userTableBody.appendChild(row);
            });
        })
        .catch(error => console.error('Error fetching users:', error));
}

// Add or Edit User using Register action
function addUser(event) {
    event.preventDefault(); // Spriječava ponovno učitavanje stranice

    const user = {
        username: document.getElementById('username').value.trim(),
        email: document.getElementById('email').value.trim(),
        password: document.getElementById('password').value.trim(),
        firstName: document.getElementById('firstName').value.trim(),
        lastName: document.getElementById('lastName').value.trim(),
        phone: document.getElementById('phone').value.trim(),
        roleId: parseInt(document.getElementById('role').value, 10),
    };

    if (!user.username || !user.email || !user.password || !user.firstName || !user.lastName || !user.phone) {
        alert('All fields are required!');
        return;
    }

    fetch('/api/User/AddUser', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        },
        body: JSON.stringify(user)
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    throw new Error(errorData.message || 'Failed to add user');
                });
            }
            return response.json();
        })
        .then(data => {
            alert(data.message); // Prikaži poruku iz odgovora
            hideUserForm();
            fetchUsers(); // Osvježi tablicu korisnika
            fetchStatistics();
            fetchRoleStatistics();
        })
        .catch(error => {
            console.error('Error adding user:', error);
            alert(`Error: ${error.message}`);
        });
}


// Edit User
function editUser(userId) {
    console.log('Editing user with ID:', userId); // Debugging
    fetch(`/api/Admin/GetUserById/${userId}`, {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        }
    })
        .then(response => {
            console.log('Response status:', response.status); // Debugging status
            if (!response.ok) throw new Error('Failed to fetch user data');
            return response.json();
        })
        .then(user => {
            console.log('Fetched user data:', user); // Debugging podataka
            showEditUserForm(user); // Prikaz forme za uređivanje
        })
        .catch(error => console.error('Error fetching user data:', error));
}



function updateUser(event) {
    event.preventDefault();

    const userId = document.getElementById('user-id').value;

    const user = {
        userId: parseInt(userId, 10),
        username: document.getElementById('username').value,
        email: document.getElementById('email').value,
        password: document.getElementById('password').value || null, // Optional password
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        phone: document.getElementById('phone').value,
        roleId: parseInt(document.getElementById('role').value, 10)
    };

    fetch('/api/User/EditUser', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        },
        body: JSON.stringify(user)
    })
        .then(response => {
            if (!response.ok) throw new Error('Failed to update user');
            alert('User updated successfully');
            hideUserForm();
            fetchUsers();
            fetchStatistics();
            fetchRoleStatistics();
        })
        .catch(error => console.error('Error updating user:', error));
}




// Delete User
function deleteUser(userId) {
    fetch(`/api/User/DeleteUser/${userId}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to delete user');
            }
            alert('User deleted successfully');
            fetchUsers(); // Ponovno dohvaćanje korisnika
            fetchStatistics();
            fetchRoleStatistics();
        })
        .catch(error => console.error('Error deleting user:', error));
}

// Show Add/Edit Form
function showAddUserForm() {
    document.getElementById('user-id').value = '';
    document.getElementById('username').value = '';
    document.getElementById('email').value = '';
    document.getElementById('password').value = ''; // Required for new user
    document.getElementById('role').value = '1'; // Default: Student
    document.getElementById('user-form').style.display = 'block';

    // Set form title to "Add User"
    document.getElementById('form-title').innerText = 'Add User';
}
function hideUserForm() {
    document.getElementById('user-form').style.display = 'none';
}
function showEditUserForm(user) {
    document.getElementById('edit-user-id').value = user.userId;
    document.getElementById('edit-username').value = user.username;
    document.getElementById('edit-email').value = user.email;
    document.getElementById('edit-phone').value = user.phone || '';
    document.getElementById('edit-firstName').value = user.firstName;
    document.getElementById('edit-lastName').value = user.lastName;
    document.getElementById('edit-role').value = user.roleId;

    // Prikaži formu za uređivanje
    document.getElementById('edit-user-form').classList.remove('hidden');
}


function hideEditUserForm() {
    document.getElementById('edit-user-form').classList.add('hidden');
}

// Logout
function logout() {
    localStorage.removeItem('jwtToken');
    window.location.href = '/Login.html';
}

// Initialize page
fetchStatistics();
fetchUsers();
fetchRoleStatistics();
