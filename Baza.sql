-- Active: 1723736230599@@127.0.0.1@1433@PeerTutoringNetwork
create database PeerTutoringNetwork;

use PeerTutoringNetwork



-- 1. Kreiranje tablice Roles
CREATE TABLE Roles (
    role_id INT PRIMARY KEY,
    role_name NVARCHAR(50) NOT NULL UNIQUE
);
go
-- 2. Kreiranje tablice Users
CREATE TABLE Users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(50) NOT NULL UNIQUE,
    pwd_hash VARBINARY(32) NOT NULL,
    pwd_salt VARBINARY(16) NOT NULL,
    first_name NVARCHAR(256) NOT NULL,
    last_name NVARCHAR(256) NOT NULL,
    email NVARCHAR(256) NOT NULL UNIQUE,
    phone NVARCHAR(20) NULL,
    role_id INT NOT NULL,
    FOREIGN KEY (role_id) REFERENCES Roles(role_id)
);
go
-- 3. Kreiranje tablice Password_Resets
CREATE TABLE Password_Resets (
    reset_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    reset_token NVARCHAR(255) NOT NULL UNIQUE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    expires_at DATETIME NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);
go
-- 4. Kreiranje tablice Login_Attempts
CREATE TABLE Login_Attempts (
    attempt_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    successful BIT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);
go
-- 5. Kreiranje tablice Sessions
CREATE TABLE Sessions (
    session_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    login_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    logout_time DATETIME NULL,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);
go
-- 6. Kreiranje tablice Subjects
CREATE TABLE Subjects (
    subject_id INT IDENTITY(1,1) PRIMARY KEY,
    subject_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(255),
    created_by_user_id INT NOT NULL,
    FOREIGN KEY (created_by_user_id) REFERENCES Users(user_id)
);
go
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
go
-- 8. Kreiranje tablice User_Roles (Ako podr�avate vi�e uloga)
CREATE TABLE User_Roles (
    user_id INT NOT NULL,
    role_id INT NOT NULL,
    PRIMARY KEY (user_id, role_id),
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (role_id) REFERENCES Roles(role_id) ON DELETE CASCADE
);
go
-- 9. Kreiranje tablice Appointments
CREATE TABLE Appointments (
    appointment_id INT IDENTITY(1,1) PRIMARY KEY,
    mentor_id INT NOT NULL,
    subject_id INT NOT NULL,
    appointment_date DATETIME NOT NULL,
    FOREIGN KEY (mentor_id) REFERENCES Users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES Subjects(subject_id) ON DELETE CASCADE
);
go
-- 10. Kreiranje tablice Appointment_Reservations
CREATE TABLE Appointment_Reservations (
    reservation_id INT IDENTITY(1,1) PRIMARY KEY,
    appointment_id INT NOT NULL,
    student_id INT NOT NULL,
    reservation_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (appointment_id) REFERENCES Appointments(appointment_id) ON DELETE CASCADE,
    FOREIGN KEY (student_id) REFERENCES Users(user_id) ON DELETE NO ACTION
);
go
-- 11. Kreiranje tablice Chat
CREATE TABLE Chat (
    chat_id INT IDENTITY PRIMARY KEY,
    title NVARCHAR(100),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);
go
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
go
-- Punjenje tablice Roles
INSERT INTO Roles (role_id, role_name) VALUES (1, 'Student'), (2, 'Teacher'), (3, 'Admin');
go
