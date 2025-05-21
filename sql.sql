CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255),
    Password VARCHAR(255),
    IsLocked BIT,
    IsLoggedIn BIT,
    IsAdmin BIT
)