

-- 1. Kreiranje tablice Roles
CREATE TABLE Roles (
    role_id INT IDENTITY(1,1) PRIMARY KEY,
    role_name NVARCHAR(50) NOT NULL UNIQUE
);

-- 2. Kreiranje tablice Users
CREATE TABLE Users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(50) NOT NULL UNIQUE,
    pwd_hash BINARY(256) NOT NULL,
    pwd_salt BINARY(256) NOT NULL,
    first_name NVARCHAR(256) NOT NULL,
    last_name NVARCHAR(256) NOT NULL,
    email NVARCHAR(256) NOT NULL UNIQUE,
    phone NVARCHAR(20) NULL,
    role_id INT NOT NULL,
    FOREIGN KEY (role_id) REFERENCES Roles(role_id)
);

-- 3. Kreiranje tablice Password_Resets
CREATE TABLE Password_Resets (
    reset_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    reset_token NVARCHAR(255) NOT NULL UNIQUE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    expires_at DATETIME NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);

-- 4. Kreiranje tablice Login_Attempts
CREATE TABLE Login_Attempts (
    attempt_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    successful BIT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);

-- 5. Kreiranje tablice Sessions
CREATE TABLE Sessions (
    session_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    login_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    logout_time DATETIME NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);

-- 6. Kreiranje tablice Subjects
CREATE TABLE Subjects (
    subject_id INT IDENTITY(1,1) PRIMARY KEY,
    subject_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(255),
    created_by_user_id INT NOT NULL,
    FOREIGN KEY (created_by_user_id) REFERENCES Users(user_id)
);

-- 7. Kreiranje tablice Reviews
CREATE TABLE Reviews (
    review_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    subject_id INT NOT NULL,
    rating INT CHECK (rating BETWEEN 1 AND 5),
    comment NVARCHAR(500),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id) ON DELETE CASCADE
);

-- 8. Kreiranje tablice User_Roles (Ako podržavate više uloga)
CREATE TABLE User_Roles (
    user_id INT NOT NULL,
    role_id INT NOT NULL,
    PRIMARY KEY (user_id, role_id),
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (role_id) REFERENCES Roles(role_id) ON DELETE CASCADE
);

-- 9. Kreiranje tablice Appointments
CREATE TABLE Appointments (
    appointment_id INT IDENTITY(1,1) PRIMARY KEY,
    mentor_id INT NOT NULL,
    subject_id INT NOT NULL,
    appointment_date DATETIME NOT NULL,
    FOREIGN KEY (mentor_id) REFERENCES Users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id) ON DELETE CASCADE
);

-- 10. Kreiranje tablice Appointment_Reservations
CREATE TABLE Appointment_Reservations (
    reservation_id INT IDENTITY(1,1) PRIMARY KEY,
    appointment_id INT NOT NULL,
    student_id INT NOT NULL,
    reservation_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (appointment_id) REFERENCES Appointments(appointment_id) ON DELETE CASCADE,
    FOREIGN KEY (student_id) REFERENCES Users(user_id) ON DELETE NO ACTION
);

-- 11. Kreiranje tablice Chat
CREATE TABLE Chat (
    chat_id INT IDENTITY PRIMARY KEY,
    title NVARCHAR(100),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- 12. Kreiranje tablice Messages
CREATE TABLE Messages (
    message_id INT IDENTITY PRIMARY KEY,
    chat_id INT NOT NULL,
    sender_id INT NOT NULL,
    content NVARCHAR(MAX),
    sent_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (chat_id) REFERENCES Chat(chat_id),
    FOREIGN KEY (sender_id) REFERENCES Users(user_id)
);


/*
-- Punjenje tablice Roles
SET IDENTITY_INSERT Roles ON;
INSERT INTO Roles (role_id, role_name) VALUES (1, 'Student'), (2, 'Teacher'), (3, 'Admin');
SET IDENTITY_INSERT Roles OFF;

-- Punjenje tablice User
INSERT INTO [User] (username, password, role_id)
VALUES 
    ('student_user', 'password1', 1),
    ('teacher_user', 'password2', 2),
    ('admin_user', 'admin_pass', 3),
    ('student_2', 'password3', 1),
    ('teacher_2', 'password4', 2);

-- Punjenje tablice Subjects
INSERT INTO Subjects (subject_name, description, created_by_user_id)
VALUES 
    ('Mathematics', 'Algebra, Geometry, and Calculus', 2),
    ('Physics', 'Mechanics and Thermodynamics', 2),
    ('History', 'Ancient and Modern History', 3),
    ('English Literature', 'Poetry, Prose, and Drama', 4),
    ('Computer Science', 'Programming Basics and Data Structures', 5);

-- Punjenje tablice Reviews
INSERT INTO Reviews (user_id, subject_id, rating, comment)
VALUES 
    (1, 1, 5, 'Great explanation of complex topics!'),
    (2, 1, 4, 'Challenging but rewarding.'),
    (3, 2, 3, 'Good course, but needs more examples.'),
    (4, 3, 5, 'Engaging history lessons.'),
    (5, 5, 4, 'Very helpful introduction to programming.');

-- Punjenje tablice User_Roles
INSERT INTO User_Roles (user_id, role_id)
VALUES 
    (1, 1),
    (2, 2),
    (3, 3),
    (4, 1),
    (5, 2);

-- Punjenje tablice Appointments
INSERT INTO Appointments (mentor_id, subject_id, appointment_date)
VALUES
    (2, 1, '2024-12-10 10:00'), 
    (5, 5, '2024-12-11 15:30'), 
    (2, 2, '2024-12-12 10:30'); 

-- Punjenje tablice Appointment_Reservations
INSERT INTO Appointment_Reservations (appointment_id, student_id)
VALUES
    (1, 1), 
    (2, 4), 
    (3, 1); 

-- Dodavanje testnih podataka u 'Chat'
INSERT INTO Chat (Title) VALUES ('General Chat');
INSERT INTO Chat (Title) VALUES ('Project Discussion');

-- Dodavanje testnih podataka u 'Message'
INSERT INTO Message (ChatId, SenderId, Content) VALUES (1, 1, 'Hello, everyone!');
INSERT INTO Message (ChatId, SenderId, Content) VALUES (1, 2, 'Hi there!');
INSERT INTO Message (ChatId, SenderId, Content) VALUES (2, 3, 'How is the project going?');
INSERT INTO Message (ChatId, SenderId, Content) VALUES (2, 1, 'We are making good progress.');*/