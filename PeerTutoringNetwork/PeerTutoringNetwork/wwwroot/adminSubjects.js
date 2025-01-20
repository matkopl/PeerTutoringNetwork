function fetchSubjects() {
    fetch('/api/Admin/GetAllSubjects', {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch subjects');
            }
            return response.json();
        })
        .then(subjects => {
            const subjectTableBody = document.getElementById('subject-table').querySelector('tbody');
            subjectTableBody.innerHTML = '';

            subjects.forEach(subject => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${subject.subjectId}</td>
                    <td>${subject.name}</td>
                    <td>${subject.description}</td>
                    <td>
                        <button class="delete-button" onclick="deleteSubject(${subject.subjectId})">Delete</button>
                    </td>
                `;
                subjectTableBody.appendChild(row);
            });
        })
        .catch(error => console.error('Error fetching subjects:', error));
}

// Dodaj novi subjekt
function addSubject(event) {
    event.preventDefault();

    const subject = {
        name: document.getElementById('subject-name').value.trim(),
        description: document.getElementById('subject-description').value.trim(),
        userId: 10 // Admin ID je hardkodiran
    };

    if (!subject.name || !subject.description) {
        alert('All fields are required!');
        return;
    }

    console.log("Sending subject:", subject);

    fetch('/api/Admin/AddSubject', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        },
        body: JSON.stringify(subject)
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(errorData => {
                    console.error("Server response error:", errorData);
                    throw new Error(errorData.message || 'Failed to add subject');
                });
            }
            return response.json();
        })
        .then(data => {
            alert('Subject added successfully!');
            fetchSubjects(); // Osvježi tablicu
        })
        .catch(error => {
            console.error('Error adding subject:', error);
            alert(`Error: ${error.message}`);
        });
}


// Izbriši subjekt
function deleteSubject(subjectId) {
    if (!confirm('Are you sure you want to delete this subject?')) {
        return;
    }

    fetch(`/api/Admin/DeleteSubject/${subjectId}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to delete subject');
            }
            alert('Subject deleted successfully');
            fetchSubjects();
        })
        .catch(error => console.error('Error deleting subject:', error));
}

// Prikaži/Sakrij formu
function showSubjectForm() {
    document.getElementById('add-subject-form').classList.remove('hidden');
}

function hideSubjectForm() {
    document.getElementById('add-subject-form').classList.add('hidden');
}

// Inicijalizacija stranice
fetchSubjects();