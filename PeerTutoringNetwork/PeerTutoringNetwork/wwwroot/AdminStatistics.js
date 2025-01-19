function fetchStatistics() {
    fetch('/api/Admin/GetUserStatistics', {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Failed to fetch statistics');
            }
            return response.json();
        })
        .then(data => {
            // Prikaz broja korisnika po rolama
            document.getElementById('user-count').innerText = `User Count: ${data.userCount}`;
            document.getElementById('admin-count').innerText = `Admin Count: ${data.adminCount}`;
            document.getElementById('teacher-count').innerText = `Teacher Count: ${data.teacherCount}`;
            document.getElementById('student-count').innerText = `Student Count: ${data.studentCount}`;
        })
        .catch(error => console.error('Error fetching user statistics:', error));
}

// Dohvati prosječne ocjene za svaki predmet
function fetchSubjectRatings() {
    fetch('/api/Admin/GetSubjectAverageRatings', {
        headers: {
            'Authorization': `Bearer ${localStorage.getItem('jwtToken')}`
        }
    })
        .then(response => response.json())
        .then(data => {
            const subjectTableBody = document.getElementById('subject-table').querySelector('tbody');
            subjectTableBody.innerHTML = '';  // Očisti tablicu prije popunjavanja

            data.forEach(subject => {
                const row = document.createElement('tr');

                row.innerHTML = `
            <td>${subject.subjectName}</td>
            <td>${subject.averageRating}</td>
        `;

                subjectTableBody.appendChild(row);
            });
        })
        .catch(error => console.error('Error fetching subjects and ratings:', error));
}

// Inicijaliziraj stranicu
fetchStatistics();
fetchSubjectRatings();