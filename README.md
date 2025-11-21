1. Overview

The Contract Monthly Claim System (CMCS) is an ASP.NET MVC web application designed to help lecturers submit their monthly teaching claims. The system also supports HR and Management staff, who handle verification, approval, and reporting.
The main purpose of the system is to streamline the claims process and ensure that all claim records are captured securely and accurately.

2. Main Features
Lecturer

Log in using a secure username and password

Submit new monthly claims

Claims are automatically linked to the lecturer’s profile

View pending and approved claims

HR

Access all lecturers and claims

View, verify, and manage incoming claims

Update the hourly rate when needed

Generate PDF reports for lecturers

Manager

View all verified claims

Approve or decline claims

Final stage before payment processing

3. Technologies Used

ASP.NET Core MVC

C#

Entity Framework Core

SQL Server

Session Authentication

QuestPDF for generating PDF reports

Bootstrap for the front-end layout

4. How the System Works
Login & Roles

Every user logs in using a role (Lecturer, HR, or Manager).
Once authenticated, the system stores the user’s role in session and redirects them to the correct dashboard.

Claims Workflow

Lecturer creates and submits a claim

HR verifies and checks details

Manager approves verified claims

HR or Manager can generate a report

PDF Reporting

Reports include:

Lecturer information

Approved claims

Total hours

Total amount

5. Database Structure

The database includes the following tables:

Users

LecturerProfiles

Claims

HourlyRateSetting

Entity Framework Core handles all queries, inserts, and updates.

6. Unit Testing

The project includes:

Basic MSTest unit tests for controllers and logic

SQL-based tests for verifying database behaviour

Tests ensure that valid claims save correctly and that role checks work as expected

7. How to Run the Project

Clone or download the repository

Open the solution in Visual Studio

Restore NuGet packages

Update the database connection string if needed

Run the application (IIS Express or Kestrel)

Use the HR account to create users for testing

8. Notes

Only HR can create accounts

Lecturers cannot edit their hourly rate

Claims cannot be approved until they are verified

The system uses server-side session checks for security

9. Conclusion

The CMCS application provides a complete claim-management workflow for academic institutions.
It includes secure authentication, clear role separation, PDF reporting, and a clean user interface.
This project demonstrates practical use of MVC architecture, SQL databases, and backend logic working together.
