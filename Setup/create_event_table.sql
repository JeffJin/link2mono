USE [appointments]
GO

/****** Object:  Table [dbo].[events]    Script Date: 2/22/2017 11:20:48 PM ******/
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
	[TypeName] [nvarchar](50) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


