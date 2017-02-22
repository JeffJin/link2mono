USE [appointments]
GO

/****** Object:  Table [dbo].[commands]    Script Date: 2/20/2017 1:31:15 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[commands](
	[Id] [uniqueidentifier] NOT NULL,
	[Body] [nvarchar](max) NOT NULL,
	[DeliveryDate] [datetime] NULL,
	[CorrelationId] [nvarchar](50) NULL,
 CONSTRAINT [PK_commands] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


