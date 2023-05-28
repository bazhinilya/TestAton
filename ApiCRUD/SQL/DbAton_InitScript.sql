CREATE DATABASE Test

USE Test
GO

CREATE TABLE [dbo].[Users](
	[Guid] [uniqueidentifier] NOT NULL,
	[Login] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Gender] [int] NOT NULL,
	[Admin] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ModifiedOn] [datetime] NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[RevokedOn] [datetime] NULL,
	[RevokedBy] [nvarchar](100) NULL,
	[BirthDate] [datetime] NULL
) ON [PRIMARY]
GO