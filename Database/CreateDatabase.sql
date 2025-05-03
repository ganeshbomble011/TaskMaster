-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Task_Master')
BEGIN
    CREATE DATABASE Task_Master;
END
GO

USE Task_Master;
GO

-- Create Users Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(200) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

-- Create Tasks Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tasks')
BEGIN
    CREATE TABLE Tasks (
        TaskId INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(MAX) NULL,
        DueDate DATETIME NULL,
        Priority INT NOT NULL, -- 0=Low, 1=Med, 2=High
        IsCompleted BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END
GO

-- Create Attachments Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Attachments')
BEGIN
    CREATE TABLE Attachments (
        AttachmentId INT PRIMARY KEY IDENTITY(1,1),
        TaskId INT NOT NULL,
        FilePath NVARCHAR(500) NOT NULL,
        UploadedAt DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (TaskId) REFERENCES Tasks(TaskId)
    );
END
GO

-- Create Notifications Table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
BEGIN
    CREATE TABLE Notifications (
        NotificationId INT PRIMARY KEY IDENTITY(1,1),
        UserId INT NOT NULL,
        Message NVARCHAR(500) NOT NULL,
        IsRead BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
        FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END
GO

-- Create Indexes
CREATE INDEX IX_Tasks_UserId ON Tasks(UserId);
CREATE INDEX IX_Tasks_DueDate ON Tasks(DueDate);
CREATE INDEX IX_Tasks_IsCompleted ON Tasks(IsCompleted);
CREATE INDEX IX_Attachments_TaskId ON Attachments(TaskId);
CREATE INDEX IX_Notifications_UserId ON Notifications(UserId);
GO

-- Create Stored Procedures

-- Get User Tasks by Date Range
CREATE OR ALTER PROCEDURE usp_GetUserTasksByDateRange
    @UserId INT,
    @StartDate DATETIME,
    @EndDate DATETIME
AS
BEGIN
    SELECT t.*, u.Name as UserName
    FROM Tasks t
    INNER JOIN Users u ON t.UserId = u.UserId
    WHERE t.UserId = @UserId
    AND t.DueDate BETWEEN @StartDate AND @EndDate
    ORDER BY t.DueDate;
END
GO

-- Insert Task with Attachment
CREATE OR ALTER PROCEDURE usp_InsertTaskWithAttachment
    @UserId INT,
    @Title NVARCHAR(200),
    @Description NVARCHAR(MAX),
    @DueDate DATETIME,
    @Priority INT,
    @FilePath NVARCHAR(500)
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        DECLARE @TaskId INT;
        
        INSERT INTO Tasks (UserId, Title, Description, DueDate, Priority)
        VALUES (@UserId, @Title, @Description, @DueDate, @Priority);
        
        SET @TaskId = SCOPE_IDENTITY();
        
        IF @FilePath IS NOT NULL
        BEGIN
            INSERT INTO Attachments (TaskId, FilePath)
            VALUES (@TaskId, @FilePath);
        END
        
        COMMIT TRANSACTION;
        
        SELECT @TaskId as TaskId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Get User Tasks with Attachments
CREATE OR ALTER PROCEDURE usp_GetUserTasksWithAttachments
    @UserId INT
AS
BEGIN
    SELECT 
        t.*,
        u.Name as UserName,
        a.AttachmentId,
        a.FilePath,
        a.UploadedAt
    FROM Tasks t
    INNER JOIN Users u ON t.UserId = u.UserId
    LEFT JOIN Attachments a ON t.TaskId = a.TaskId
    WHERE t.UserId = @UserId
    ORDER BY t.DueDate;
END
GO

-- Get User Notifications
CREATE OR ALTER PROCEDURE usp_GetUserNotifications
    @UserId INT,
    @IsRead BIT = NULL
AS
BEGIN
    SELECT *
    FROM Notifications
    WHERE UserId = @UserId
    AND (@IsRead IS NULL OR IsRead = @IsRead)
    ORDER BY CreatedAt DESC;
END
GO 