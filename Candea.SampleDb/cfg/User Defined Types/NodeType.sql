CREATE TYPE [cfg].[NodeType] AS TABLE (
    [Id]          INT            NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Value]       NVARCHAR (2000) NULL,
    [CreatedBy]   VARCHAR (10)     NOT NULL,
    [CreatedDate] DATETIME2 (0)  NULL,
    [UpdatedBy]   VARCHAR (10)     NULL,
    [UpdatedDate] DATETIME2 (0)  NULL
)

