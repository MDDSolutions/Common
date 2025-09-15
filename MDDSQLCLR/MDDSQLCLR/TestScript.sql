/*
Test script for MDDSQLCLR functions
*/

-- Test RegexMatch
DECLARE @input NVARCHAR(MAX) = 'This is a bound test, but not a boundary test.';
DECLARE @pattern NVARCHAR(MAX) = '\bbound\b';
SELECT dbo.RegexMatch(@input, @pattern) AS RegexMatchResult;

-- Test RegexReplace
DECLARE @replacement NVARCHAR(MAX) = 'found';
SELECT dbo.RegexReplace(@input, @pattern, @replacement) AS RegexReplaceResult;

-- Test RegexSplit
DECLARE @splitPattern NVARCHAR(MAX) = '\s';
SELECT *
FROM dbo.RegexSplit(@input, @splitPattern);

-- Test RegexMatches
DECLARE @matchPattern NVARCHAR(MAX) = '\b\w{4}\b'; -- Matches all 4-letter words
SELECT *
FROM dbo.RegexMatches(@input, @matchPattern);

--Test RegexContain
SELECT dbo.RegexContain(@input, @matchPattern) AS RegexContain;

-- Test LevenshteinSimilarity
DECLARE @str1 NVARCHAR(MAX) = 'kitten';
DECLARE @str2 NVARCHAR(MAX) = 'sitting';
SELECT dbo.LevenshteinSimilarity(@str1, @str2) AS LevenshteinSimilarityResult;
GO
-- Test SplitCLR
DECLARE @csv NVARCHAR(MAX) = 'apple,banana,cherry,date';
DECLARE @delimiter NVARCHAR(10) = ',';
SELECT *
FROM dbo.SplitCLR(@csv, @delimiter);
GO

