
 --Punjenje tablice User
INSERT INTO [User] (username, password, role_id)
VALUES 
    ('student_user', 'password1', 1),
    ('teacher_user', 'password2', 2),
    ('admin_user', 'admin_pass', 3),
    ('student_2', 'password3', 1),
    ('teacher_2', 'password4', 2);

--Punjenje tablice Subjects
INSERT INTO Subjects (subject_name, description, created_by_user_id)
VALUES 
    ('Mathematics', 'Algebra, Geometry, and Calculus', 2),
    ('Physics', 'Mechanics and Thermodynamics', 2),
    ('History', 'Ancient and Modern History', 3),
    ('English Literature', 'Poetry, Prose, and Drama', 4),
    ('Computer Science', 'Programming Basics and Data Structures', 5);

--Punjenje tablice Reviews
INSERT INTO Reviews (user_id, subject_id, rating, comment)
VALUES 
    (1, 1, 5, 'Great explanation of complex topics!'),
    (2, 1, 4, 'Challenging but rewarding.'),
    (3, 2, 3, 'Good course, but needs more examples.'),
    (4, 3, 5, 'Engaging history lessons.'),
    (5, 5, 4, 'Very helpful introduction to programming.');

--Punjenje tablice User_Roles
INSERT INTO User_Roles (user_id, role_id)
VALUES 
    (1, 1),
    (2, 2),
    (3, 3),
    (4, 1),
    (5, 2);

--Punjenje tablice Appointments
INSERT INTO Appointments (mentor_id, subject_id, appointment_date)
VALUES
    (2, 1, '2024-12-10 10:00'), 
    (5, 5, '2024-12-11 15:30'), 
    (2, 2, '2024-12-12 10:30'); 

--Punjenje tablice Appointment_Reservations
INSERT INTO Appointment_Reservations (appointment_id, student_id)
VALUES
    (1, 1), 
    (2, 4), 
    (3, 1); 

--Dodavanje testnih podataka u 'Chat'
INSERT INTO Chat (Title) VALUES ('General Chat');
INSERT INTO Chat (Title) VALUES ('Project Discussion');

--Dodavanje testnih podataka u 'Message'
INSERT INTO Message (ChatId, SenderId, Content) VALUES (1, 1, 'Hello, everyone!');
INSERT INTO Message (ChatId, SenderId, Content) VALUES (1, 2, 'Hi there!');
INSERT INTO Message (ChatId, SenderId, Content) VALUES (2, 3, 'How is the project going?');
INSERT INTO Message (ChatId, SenderId, Content) VALUES (2, 1, 'We are making good progress.'); */
