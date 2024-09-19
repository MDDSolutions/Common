/*
https://github.com/MDDSolutions/Common/tree/main/MDDSQLCLR
*/

-- Drop functions if they exist
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.RegexMatch') AND type IN (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION dbo.RegexMatch;

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.RegexReplace') AND type IN (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION dbo.RegexReplace;

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.RegexSplit') AND type IN (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION dbo.RegexSplit;

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.RegexMatches') AND type IN (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION dbo.RegexMatches;

-- Drop assembly if it exists
IF EXISTS (SELECT * FROM sys.assemblies WHERE name = N'MDDSQLCLR')
    DROP ASSEMBLY MDDSQLCLR;
GO


EXEC sys.sp_configure @configname = 'clr strict security', -- varchar(35)
                      @configvalue = 0  -- int
GO
RECONFIGURE
GO

-- Create assembly
CREATE ASSEMBLY MDDSQLCLR
FROM 'D:\SoftwareDeploy\MDDSQLCLR.dll'
WITH PERMISSION_SET = SAFE;
GO

-- Create functions
CREATE FUNCTION dbo.RegexMatch(@input NVARCHAR(MAX), @pattern NVARCHAR(MAX))
RETURNS BIT
AS EXTERNAL NAME MDDSQLCLR.[MDDSQLCLR.RegexFunctions].RegexMatch;
GO

CREATE FUNCTION dbo.RegexReplace(@input NVARCHAR(MAX), @pattern NVARCHAR(MAX), @replacement NVARCHAR(MAX))
RETURNS NVARCHAR(MAX)
AS EXTERNAL NAME MDDSQLCLR.[MDDSQLCLR.RegexFunctions].RegexReplace;
GO

CREATE FUNCTION dbo.RegexSplit(@input NVARCHAR(MAX), @pattern NVARCHAR(MAX))
RETURNS TABLE (Value NVARCHAR(MAX))
AS EXTERNAL NAME MDDSQLCLR.[MDDSQLCLR.RegexFunctions].RegexSplit;
GO

CREATE FUNCTION dbo.RegexMatches(@input NVARCHAR(MAX), @pattern NVARCHAR(MAX))
RETURNS TABLE (Value NVARCHAR(MAX), MatchIndex INT, MatchLength INT)
AS EXTERNAL NAME MDDSQLCLR.[MDDSQLCLR.RegexFunctions].RegexMatches;
GO