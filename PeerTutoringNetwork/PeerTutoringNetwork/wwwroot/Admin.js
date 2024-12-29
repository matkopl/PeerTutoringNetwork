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
                    <td>${user.role}</td>
                    <td>
                        <button onclick="editUser(${user.id})">Edit</button>
                        <button onclick="deleteUser(${user.id})">Delete</button>
                    </td>
                `;
                userTableBody.appendChild(row);
            });
        })
        .catch(error => console.error('Error fetching users:', error));
}

// Add or Edit User using Register action
// Add User
function addUser(event) {
    event.preventDefault();

    const user = {
        username: document.getElementById('username').value,
        email: document.getElementById('email').value,
        password: document.getElementById('password').value,
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        roleId: parseInt(document.getElementById('role').value, 10)
    };

    fetch('/api/User/AddUser', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        },
        body: JSON.stringify(user)
    })
        .then(response => {
            if (!response.ok) throw new Error('Failed to add user');
            alert('User added successfully');
            hideUserForm();
            fetchUsers();
        })
        .catch(error => console.error('Error adding user:', error));
}

// Edit User
function editUser(userId) {
    fetch(`/api/User/GetUserById/${userId}`, {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        }
    })
        .then(response => {
            if (!response.ok) throw new Error('Failed to fetch user data');
            return response.json();
        })
        .then(user => {
            document.getElementById('user-id').value = user.userId;
            document.getElementById('username').value = user.username;
            document.getElementById('email').value = user.email;
            document.getElementById('password').value = ''; // Lozinka se ne prikazuje
            document.getElementById('firstName').value = user.firstName;
            document.getElementById('lastName').value = user.lastName;
            document.getElementById('role').value = user.roleId;

            document.getElementById('user-form').style.display = 'block';
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

// Hide Form
function hideUserForm() {
    document.getElementById('user-form').style.display = 'none';
}

// Logout
function logout() {
    localStorage.removeItem('jwtToken');
    window.location.href = '/Login.html';
}

// Initialize page
fetchStatistics();
fetchUsers();