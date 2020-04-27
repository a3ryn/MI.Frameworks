CREATE TABLE [cfg].[Node] (
    [Id]          INT            CONSTRAINT [DF_Node_Id] DEFAULT (NEXT VALUE FOR [cfg].[NodeIdSeq]) NOT NULL,
    [Name]        VARCHAR (50)  NOT NULL,
    [Value]       VARCHAR (2000) NULL,

    [CreatedBy]   VARCHAR (10)     CONSTRAINT [DF_Node_CreatedBy] DEFAULT ('app') NOT NULL,
    [CreatedDate] DATETIME2 (0)  CONSTRAINT [DF_Node_CreatedDate] DEFAULT (sysdatetime()) NULL,
    [UpdatedBy]   VARCHAR (10)     NULL,
    [UpdatedDate] DATETIME2 (0)  NULL,

    CONSTRAINT [PK_Node] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [IX_Node] UNIQUE NONCLUSTERED ([Id] ASC, [Name] ASC)
);

