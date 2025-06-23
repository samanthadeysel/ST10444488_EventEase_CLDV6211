CREATE TABLE EventType (
    EventTypeID INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(100) NOT NULL
);
INSERT INTO EventType (TypeName)
VALUES ('Conference'), ('Wedding'), ('Birthday'), ('Concert'), ('Exhibition');

SELECT * FROM EventType;