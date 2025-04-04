using System.Reflection;

namespace CSVParser.Tests
{
    public class CSVParserTests
    {
        const string OUTPUT_FILE = "output.csv";
        const string SOURCE_FILE = "TestFile.csv";

        #region CSVParser_ParseCSV_Tests
        [Fact]
        public void CSVParser_ParseCSV_ReturnsNull_IfCSVContentIsNull()
        {
            //Arrange - variables, classes, mocks
            string csvContent = null;


            //Act - calling the method to test
            var result = CSVParser.ParseCSV(csvContent);

            //Assert - verifying the result
            Assert.Null(result);
        }

        [Fact]
        public void CSVParser_ParseCSV_ReturnsEmptyList_IfCSVContentIsEmpty()
        {
            //Arrange - variables, classes, mocks
            string csvContent = "";

            //Act - calling the method to test
            var result = CSVParser.ParseCSV(csvContent);

            //Assert - verifying the result
            Assert.Empty(result!);
        }


        [Fact]
        public void CSVParser_ParseCSV_ReturnsValidCSV()
        {
            //Arrange - variables, classes, mocks
            StreamReader csvFile = new StreamReader("TestFile.csv");

            List<List<String>> expectedResult = new List<List<string>>()
            {
                new List<string>() {"Message", "Str1", "Str2"},
                new List<string>() {"single line with comma and double quotes", "\"hello, World\"", ""},
                new List<string>() {"single line with comma", "hello, world", ""},
                new List<string>() {"two columns: first ending with comma", "\"hello, ", "world\""},
                new List<string>() {"two columns: second starting with comma", "\"hello", ", world\""},
                new List<string>() {"two Columns", "\"hello", "world\""},
                new List<string>() {"single double quote in the middle", "hello \" world", ""},
                new List<string>() {"single double quote", "\"", ""},
                new List<string>() {"", "", ""},
            };


            //Act - calling the method to test
            var result = CSVParser.ParseCSV(csvFile);

            //Assert - verifying the result
            Assert.Equal(expectedResult, result);
        }


        #endregion

        #region CSVParser_EscapeCsvValue_Tests
        [Fact]
        public void CSVParser_EscapeCsvValue_ReturnsEmptyString_IfValueIsNull()
        {
            // Arrange
            string value = null;

            // Act
            var result = CSVParser.EscapeCSV(value);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void CSVParser_EscapeCsvValue_ReturnsEmptyString_IfValueIsEmpty()
        {
            // Arrange
            string value = "";

            // Act
            var result = CSVParser.EscapeCSV(value);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void CSVParser_EscapeCsvValue_EscapesValueWithComma()
        {
            // Arrange
            string value = "hello, world";

            // Act
            var result = CSVParser.EscapeCSV(value);

            // Assert
            Assert.Equal("\"hello, world\"", result);
        }

        [Fact]
        public void CSVParser_EscapeCsvValue_EscapesValueWithDoubleQuote()
        {
            // Arrange
            string value = "hello \"world\"";

            // Act
            var result = CSVParser.EscapeCSV(value);

            // Assert
            Assert.Equal("\"hello \"\"world\"\"\"", result);
        }

        [Fact]
        public void CSVParser_EscapeCsvValue_EscapesValueWithNewLine()
        {
            // Arrange
            string value = "hello\nworld";

            // Act
            var result = CSVParser.EscapeCSV(value);

            // Assert
            Assert.Equal("\"hello\nworld\"", result);
        }

        [Fact]
        public void CSVParser_EscapeCsvValue_EscapesValueWithCarriageReturn()
        {
            // Arrange
            string value = "hello\rworld";

            // Act
            var result = CSVParser.EscapeCSV(value);

            // Assert
            Assert.Equal("\"hello\rworld\"", result);
        }

        [Fact]
        public void CSVParser_EscapeCsvValue_DoesNotEscapeSimpleValue()
        {
            // Arrange
            string value = "helloworld";

            // Act
            var result = CSVParser.EscapeCSV(value);

            // Assert
            Assert.Equal("helloworld", result);
        }
        #endregion

        #region CSVParser_ExportToCSV_Tests

        [Fact]
        public void CSVParser_ExportToCSV_ThrowsArgumentNullException_IfItemsIsNull()
        {
            // Arrange
            List<string> items = null;
            string filePath = "output.csv";
            Func<string, string> convertToCsvLine = s => s;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CSVParser.ExportToCSVFile(items, filePath, convertToCsvLine));
        }



        [Fact]
        public void CSVParser_ExportToCSV_ThrowsArgumentNullException_IfConvertToCsvLineIsNull()
        {
            // Arrange
            List<string> items = new List<string> { "item1", "item2" };
            string filePath = "output.csv";
            Func<string, string> convertToCsvLine = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => CSVParser.ExportToCSVFile(items, filePath, convertToCsvLine));
        }

        [Fact]
        public void CSVParser_ExportToCSV_ThrowsArgumentException_IfFilePathIsNullOrWhitespace()
        {
            // Arrange
            List<string> items = new List<string> { "item1", "item2" };
            string filePath = " ";
            Func<string, string> convertToCsvLine = s => s;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => CSVParser.ExportToCSVFile(items, filePath, convertToCsvLine));
        }

        [Fact]
        public void CSVParser_ExportToCSV_ExportsAValidCSVFile()
        {
            //Arrange - variables, classes, mocks
            StreamReader csvFile = new StreamReader(SOURCE_FILE);
            var CSVData = CSVParser.ParseCSV(csvFile);

            // Act
            CSVParser.ExportToCSVFile(CSVData, OUTPUT_FILE, true);
            StreamReader csvOutputFile = new StreamReader(OUTPUT_FILE);
            var result = CSVParser.ParseCSV(csvOutputFile);


            // Assert
            Assert.Equal(CSVData, result);

            // Clean up
            if (File.Exists(OUTPUT_FILE))
            {
                File.Delete(OUTPUT_FILE);
            }

        }

        #endregion

        #region CSVParser_ParseLine_Tests
        [Fact]
        public void CSVParser_ParseLine_ReturnsSimpleTokens()
        {
            //Arrange - variables, classes, mocks
            string line = "Message,Str1,Str2";
            List<string> expected = line.Split(',').ToList();
            Type type = typeof(CSVParser);
            MethodInfo methodInfo = type.GetMethod("ParseLine", BindingFlags.NonPublic | BindingFlags.Static)!;

            //Act - calling the method to test
            var result = methodInfo.Invoke(null, new object[] { line });

            //Assert - verifying the result
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CSVParser_ParseLine_Returns_TokensWithCommasAndDoubleQuotes()
        {
            //Arrange - variables, classes, mocks
            string line = "single line with comma and double quotes,\"\"\"hello, World\"\"\"";
            List<string> expected = new List<string>() { "single line with comma and double quotes", "\"hello, World\"" };
            Type type = typeof(CSVParser);
            MethodInfo methodInfo = type.GetMethod("ParseLine", BindingFlags.NonPublic | BindingFlags.Static)!;

            //Act - calling the method to test
            var result = methodInfo.Invoke(null, new object[] { line });

            //Assert - verifying the result
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CSVParser_ParseLine_Returns_TokensWithCommasAndDoubleQuotesAndLastTokenIsEmptyString()
        {
            //Arrange - variables, classes, mocks
            string line = "single line with comma and double quotes,\"\"\"hello, World\"\"\",";
            List<string> expected = new List<string>() { "single line with comma and double quotes", "\"hello, World\"", "" };
            Type type = typeof(CSVParser);
            MethodInfo methodInfo = type.GetMethod("ParseLine", BindingFlags.NonPublic | BindingFlags.Static)!;

            //Act - calling the method to test
            var result = methodInfo.Invoke(null, new object[] { line });

            //Assert - verifying the result
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CSVParser_ParserLine_ReturnsSingleLineWithComma()
        {
            //Arrange - variables, classes, mocks
            string line = "single line with comma,\"hello, world\",";
            List<string> expected = new List<string>() { "single line with comma", "hello, world", "" };
            Type type = typeof(CSVParser);
            MethodInfo methodInfo = type.GetMethod("ParseLine", BindingFlags.NonPublic | BindingFlags.Static)!;

            //Act - calling the method to test
            var result = methodInfo.Invoke(null, new object[] { line });

            //Assert - verifying the result
            Assert.Equal(expected, result);
        }

        #endregion

        #region CSVParser_Q0_Tests
        [Fact]
        public void CSVParser_Q0_ReturnsNull_IfLineIsNull()
        {
            //Arrange - variables, classes, mocks
            string? line = null;
            Type type = typeof(CSVParser);
            MethodInfo methodInfo = type.GetMethod("Q0", BindingFlags.NonPublic | BindingFlags.Static)!;

            //Act - calling the method to test
            var result = methodInfo.Invoke(null, new object[] { line });

            //Assert - verifying the result
            Assert.Null(result);
        }

        [Fact]
        public void CSVParser_Q0_ReturnsEmptyStringAndNullRemaindeer_IfLineIsEmpty()
        {
            //Arrange - variables, classes, mocks
            string? line = "";
            (String Token, String? Remainder) expected = ("", null);
            Type type = typeof(CSVParser);
            MethodInfo methodInfo = type.GetMethod("Q0", BindingFlags.NonPublic | BindingFlags.Static)!;

            //Act - calling the method to test
            var result = methodInfo.Invoke(null, new object[] { line });

            //Assert - verifying the result
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CSVParser_Q0_ReturnsEmptryStringAndRemainder_IfLineStartsWithComma()
        {
            //Arrange - variables, classes, mocks
            string? line = ",a,b,c";
            (String Token, String? Remainder) expected = ("", "a,b,c");
            Type type = typeof(CSVParser);
            MethodInfo methodInfo = type.GetMethod("Q0", BindingFlags.NonPublic | BindingFlags.Static)!;

            //Act - calling the method to test
            var result = methodInfo.Invoke(null, new object[] { line });

            //Assert - verifying the result
            Assert.Equal(expected, result);
        }
        #endregion

        #region CSVParser_Q1_Tests
        [Fact]
        public void CSVParser_Q1_ReturnsFirstTokenAndRemainder_IfLineStartsWithNonCommaAndNonDoubleQuote()
        {
            //Arrange - variables, classes, mocks
            string? line = "uno,dos,tres";
            (String Token, String? Remainder) expected = ("uno", "dos,tres");
            Type type = typeof(CSVParser);
            MethodInfo methodInfo = type.GetMethod("Q1", BindingFlags.NonPublic | BindingFlags.Static)!;

            //Act - calling the method to test
            var result = methodInfo.Invoke(null, new object[] { line });

            //Assert - verifying the result
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CSVParser_Q1_ReturnsEmptyTokenAndRemainder_IfLineStartsWithComma()
        {
            //Arrange - variables, classes, mocks
            string? line = ",dos,tres";
            (String Token, String? Remainder) expected = ("", "dos,tres");
            Type type = typeof(CSVParser);
            MethodInfo methodInfo = type.GetMethod("Q1", BindingFlags.NonPublic | BindingFlags.Static)!;

            //Act - calling the method to test
            var result = methodInfo.Invoke(null, new object[] { line });

            //Assert - verifying the result
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CSVParser_Q1_RetrunsFirstTokenAndNullRemainder_IfLineOnlyHasASingleValue()
        {
            //Arrange - variables, classes, mocks
            string? line = "uno";
            (String Token, String? Remainder) expected = ("uno", null);
            Type type = typeof(CSVParser);
            MethodInfo methodInfo = type.GetMethod("Q1", BindingFlags.NonPublic | BindingFlags.Static)!;

            //Act - calling the method to test
            var result = methodInfo.Invoke(null, new object[] { line });

            //Assert - verifying the result
            Assert.Equal(expected, result);
        }
        #endregion

    }
}