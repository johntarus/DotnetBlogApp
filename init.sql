IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BlogApp')
    BEGIN
        CREATE DATABASE BlogApp;
        PRINT 'Database BlogApp created';
    END
GO
