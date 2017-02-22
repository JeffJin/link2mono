USE [appointments]
GO

/****** Object:  Table [dbo].[events]    Script Date: 2/20/2017 1:19:41 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[events](
	[SourceId] [uniqueidentifier] NOT NULL,
	[Version] [int] NOT NULL,
	[SourceType] [nvarchar](50) NOT NULL,
	[Payload] [nvarchar](max) NOT NULL,
	[CorrelationId] [uniqueidentifier] NULL,
	[AssemblyName] [nvarchar](50) NULL,
	[Namespace] [nvarchar](50) NULL,
	[FullName] [nvarchar](50) NULL,
	[MetaTypeName] [nvarchar](50) NULL,
 CONSTRAINT [PK_events] PRIMARY KEY CLUSTERED 
(
	[SourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


