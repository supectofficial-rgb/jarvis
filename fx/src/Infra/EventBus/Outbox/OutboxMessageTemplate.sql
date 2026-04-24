/*
OutboxMessages
(
    Id UNIQUEIDENTIFIER PK,
    EventType NVARCHAR(200),
    Payload NVARCHAR(MAX),
    Metadata NVARCHAR(MAX),
    RetryCount INT,
    Processed BIT
)
*/