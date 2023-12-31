
USE [BankDataDb]
GO
/****** Object:  Table [dbo].[payments]    Script Date: 9/13/2023 8:58:41 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[payments](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[PaymentID] [int] NULL,
	[AccountHolder] [varchar](100) NULL,
	[BranchCode] [varchar](100) NULL,
	[AccountNumber] [varchar](100) NULL,
	[AccountType] [int] NULL,
	[Amount] [decimal](18, 2) NULL,
	[Status] [varchar](100) NULL,
	[EffectiveStatusDate] [date] NULL,
	[TransactionDate] [date] NULL
) ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [BankDataDb] SET  READ_WRITE 
GO
