-- Kreiranje baze podataka
CREATE DATABASE PeerTutoringNetwork;
GO

USE PeerTutoringNetwork;
GO

-- 1. Kreiranje tablice Roles
CREATE TABLE Roles (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name NVARCHAR(50) NOT NULL UNIQUE
);

-- 2. Kreiranje tablice User
CREATE TABLE [User] (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(100) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    role_id INT,
    FOREIGN KEY (role_id) REFERENCES Roles(role_id)
);

-- 3. Kreiranje tablice Profiles
CREATE TABLE Profiles (
    profile_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    first_name NVARCHAR(100),
    last_name NVARCHAR(100),
    phone_number NVARCHAR(20),
    bio NVARCHAR(500),
    FOREIGN KEY (user_id) REFERENCES [User](user_id) ON DELETE CASCADE
);

-- 4. Kreiranje tablice Password_Resets
CREATE TABLE Password_Resets (
    reset_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    reset_token NVARCHAR(255) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    expires_at DATETIME,
    FOREIGN KEY (user_id) REFERENCES [User](user_id) ON DELETE CASCADE
);

-- 5. Kreiranje tablice Login_Attempts
CREATE TABLE Login_Attempts (
    attempt_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    timestamp DATETIME DEFAULT GETDATE(),
    successful BIT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES [User](user_id) ON DELETE CASCADE
);

-- 6. Kreiranje tablice Sessions
CREATE TABLE Sessions (
    session_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    login_time DATETIME DEFAULT GETDATE(),
    logout_time DATETIME NULL,
    FOREIGN KEY (user_id) REFERENCES [User](user_id) ON DELETE CASCADE
);

-- 7. Kreiranje tablice Subjects
CREATE TABLE Subjects (
    subject_id INT IDENTITY(1,1) PRIMARY KEY,
    subject_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(255),
    created_by_user_id INT NOT NULL,
    FOREIGN KEY (created_by_user_id) REFERENCES [User](user_id)
);

-- 8. Kreiranje tablice Reviews
CREATE TABLE Reviews (
    review_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    subject_id INT NOT NULL,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    comment NVARCHAR(500),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES [User](user_id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id) ON DELETE CASCADE
);

-- 9. Kreiranje tablice User_Roles (Bridge tablica za više uloga po korisniku)
CREATE TABLE User_Roles (
    user_id INT NOT NULL,
    role_id INT NOT NULL,
    PRIMARY KEY (user_id, role_id),
    FOREIGN KEY (user_id) REFERENCES [User](user_id) ON DELETE CASCADE,
    FOREIGN KEY (role_id) REFERENCES Roles(role_id) ON DELETE CASCADE
);

-- 10. Kreiranje tablice Appointments
CREATE TABLE Appointments (
    appointment_id INT IDENTITY(1,1) PRIMARY KEY,
    mentor_id INT NOT NULL,
    subject_id INT NOT NULL,
    appointment_date DATE NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    FOREIGN KEY (mentor_id) REFERENCES [User](user_id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id) ON DELETE CASCADE
);

-- 11. Kreiranje tablice Appointment_Reservations
CREATE TABLE Appointment_Reservations (
    reservation_id INT IDENTITY(1,1) PRIMARY KEY,
    appointment_id INT NOT NULL,
    student_id INT NOT NULL,
    reservation_time DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (appointment_id) REFERENCES Appointments(appointment_id) ON DELETE CASCADE,
    FOREIGN KEY (student_id) REFERENCES [User](user_id) ON DELETE NO ACTION
);

-- 12. Kreiranje tablice 'Chat'
CREATE TABLE Chat (
    Id INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 13. Kreiranje tablice 'Message'
CREATE TABLE Message (
    Id INT IDENTITY PRIMARY KEY,
    ChatId INT,
    SenderId INT,
    Content NVARCHAR(MAX),
    SentAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ChatId) REFERENCES Chat(Id),
    FOREIGN KEY (SenderId) REFERENCES Users(user_id)
);



-- 12. Punjenje tablice Roles
SET IDENTITY_INSERT Roles ON;
INSERT INTO Roles (role_id, role_name) VALUES (1, 'Student'), (2, 'Teacher'), (3, 'Admin');
SET IDENTITY_INSERT Roles OFF;

-- 13. Punjenje tablice User
INSERT INTO [User] (username, password, role_id)
VALUES 
    ('student_user', 'password1', 1),
    ('teacher_user', 'password2', 2),
    ('admin_user', 'admin_pass', 3),
    ('student_2', 'password3', 1),
    ('teacher_2', 'password4', 2);

-- 14. Punjenje tablice Profiles
INSERT INTO Profiles (user_id, first_name, last_name, phone_number, bio)
VALUES 
    (1, 'John', 'Doe', '123-456-7890', 'Student of mathematics.'),
    (2, 'Jane', 'Smith', '987-654-3210', 'Physics teacher with 10 years experience.'),
    (3, 'Admin', 'User', '555-555-5555', 'Administrator of the platform.'),
    (4, 'Mark', 'Taylor', '444-444-4444', 'History enthusiast and student.'),
    (5, 'Emma', 'Wilson', '333-333-3333', 'Teacher of computer science.');

-- 15. Punjenje tablice Subjects
INSERT INTO Subjects (subject_name, description, created_by_user_id)
VALUES 
    ('Mathematics', 'Algebra, Geometry, and Calculus', 2),
    ('Physics', 'Mechanics and Thermodynamics', 2),
    ('History', 'Ancient and Modern History', 3),
    ('English Literature', 'Poetry, Prose, and Drama', 4),
    ('Computer Science', 'Programming Basics and Data Structures', 5);

-- 16. Punjenje tablice Reviews
INSERT INTO Reviews (user_id, subject_id, rating, comment)
VALUES 
    (1, 1, 5, 'Great explanation of complex topics!'),
    (2, 1, 4, 'Challenging but rewarding.'),
    (3, 2, 3, 'Good course, but needs more examples.'),
    (4, 3, 5, 'Engaging history lessons.'),
    (5, 5, 4, 'Very helpful introduction to programming.');

-- 17. Punjenje tablice User_Roles
INSERT INTO User_Roles (user_id, role_id)
VALUES 
    (1, 1),
    (2, 2),
    (3, 3),
    (4, 1),
    (5, 2);

-- 18. Punjenje tablice Appointments
INSERT INTO Appointments (mentor_id, subject_id, appointment_date, start_time, end_time)
VALUES
    (2, 1, '2024-12-10', '10:00', '11:00'), 
    (5, 5, '2024-12-11', '14:00', '15:30'), 
    (2, 2, '2024-12-12', '09:00', '10:30'); 

-- 19. Punjenje tablice Appointment_Reservations
INSERT INTO Appointment_Reservations (appointment_id, student_id)
VALUES
    (1, 1), 
    (2, 4), 
    (3, 1); 

-- 20. Dodavanje testnih podataka u 'Chat'
INSERT INTO Chat (Title) VALUES ('General Chat');
INSERT INTO Chat (Title) VALUES ('Project Discussion');

-- 21. Dodavanje testnih podataka u 'Message'
INSERT INTO Message (ChatId, SenderId, Content) VALUES (1, 1, 'Hello, everyone!');
INSERT INTO Message (ChatId, SenderId, Content) VALUES (1, 2, 'Hi there!');
INSERT INTO Message (ChatId, SenderId, Content) VALUES (2, 3, 'How is the project going?');
INSERT INTO Message (ChatId, SenderId, Content) VALUES (2, 1, 'We are making good progress.');
