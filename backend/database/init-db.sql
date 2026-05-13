IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'StaffManagementDb')
BEGIN
    CREATE DATABASE StaffManagementDb;
END
GO

USE StaffManagementDb;
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Staffs')
BEGIN
    CREATE TABLE Staffs (
        Id            NVARCHAR(8)    NOT NULL CONSTRAINT PK_Staffs PRIMARY KEY,
        FullName      NVARCHAR(100)  NOT NULL,
        Birthday      DATE           NOT NULL,
        Gender        INT            NOT NULL CHECK (Gender IN (1, 2))
    );

    CREATE NONCLUSTERED INDEX IX_Staffs_FullName ON Staffs (FullName);
    CREATE NONCLUSTERED INDEX IX_Staffs_Gender ON Staffs (Gender);
    CREATE NONCLUSTERED INDEX IX_Staffs_Birthday ON Staffs (Birthday);
END
GO

IF NOT EXISTS (SELECT 1 FROM Staffs WHERE Id = 'STF00001')
BEGIN
    INSERT INTO Staffs (Id, FullName, Birthday, Gender) VALUES
        ('STF00001', N'Sokha Makara',      '1990-05-15', 1),
        ('STF00002', N'Chan Sovannary',    '1992-08-22', 2),
        ('STF00003', N'Dara Vichet',       '1988-01-10', 1),
        ('STF00004', N'Mean Sokunthea',    '1995-11-30', 2),
        ('STF00005', N'Rath Panha',        '1993-03-05', 1),
        ('STF00006', N'Nget Monyneath',    '1991-07-18', 2),
        ('STF00007', N'Koy Sopheap',       '1987-12-25', 1),
        ('STF00008', N'Hul Channary',      '1994-09-12', 2),
        ('STF00009', N'Yorn Borey',        '1990-06-08', 1),
        ('STF00010', N'Chea Sotheary',     '1996-02-14', 2);
END
GO
