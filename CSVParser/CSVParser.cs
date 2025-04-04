using System.Text;

namespace CSVParser
{
    public class CSVParser
    {
        #region Public Members
        public static List<List<String>> ParseCSV(StreamReader csvContent)
        {
            List<List<string>> lines = new List<List<string>>();
            using (csvContent)
            {
                while (!csvContent.EndOfStream)
                {
                    var line = csvContent.ReadLine();
                    lines.Add(ParseLine(line));
                }
            }
            return lines;
        }

        public static List<List<String>>? ParseCSV(String? csvContent)
        {
            if (csvContent == null) return null;

            byte[] byteArray = Encoding.UTF8.GetBytes(csvContent);
            using (MemoryStream stream = new MemoryStream(byteArray))
            using (StreamReader reader = new StreamReader(stream))
            {
                return ParseCSV(reader);
            }
        }

        public static void ExportToCSVFile(List<List<string>> items, string filePath, bool overwrite = false)
        {
            ExportToCSVFile(items, filePath, EscapeStringArrayToCSVLine, overwrite);
        }

        public static void ExportToCSVFile<T>(List<T> items, string filePath, Func<T, string> convertToCsvLine, bool overwrite = false)
        {
            FileMode fileMode = overwrite ? FileMode.Create : FileMode.Append;
            using (FileStream fs = new FileStream(filePath, fileMode, FileAccess.Write))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                ExportToCSVFile(items, writer, convertToCsvLine);
            }
        }

        public static void ExportToCSVFile(List<List<string>> items, StreamWriter writer)
        {
            ExportToCSVFile(items, writer, EscapeStringArrayToCSVLine);
        }

        public static void ExportToCSVFile<T>(List<T> items, StreamWriter writer, Func<T, string> convertToCsvLine)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (convertToCsvLine == null) throw new ArgumentNullException(nameof(convertToCsvLine));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            try
            {
                foreach (var (item, index) in items.Select((value, i) => (value, i)))
                {
                    if(index == items.Count - 1)
                    {
                        writer.Write(convertToCsvLine(item));
                    }
                    else
                    {
                        writer.WriteLine(convertToCsvLine(item));
                    }
                }
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException("An error occurred while writing to the stream.", ex);
            }
        }

        public static string EscapeCSV(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }

        public static string EscapeStringArrayToCSVLine(List<String> values)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < values.Count; i++)
            {
                sb.Append(EscapeCSV(values[i]));
                if (i < values.Count - 1)
                {
                    sb.Append(",");
                }
            }
            return sb.ToString();
        }

        #endregion

        #region Private Members
        private static List<string> ParseLine(string? line)
        {
            if (line == null)
            {
                return new List<string>();
            }

            List<string> columns = new List<string>();
            var tokens = Q0(line);
            while (tokens != null)
            {
                columns.Add(tokens.Value.Token);
                tokens = Q0(tokens.Value.Remainder);
            }

            return columns;

        }

        private static (String Token, String? Remainder)? Q0(string? line)
        {
            if (line == null) return null;
            if (line == "") return ("", null);

            if (line[0] == ',')
            {
                return ("", line.Substring(1));
            }

            if (line[0] != ',' && line[0] != '"')
            {
                return Q1(line);
            }

            if (line[0] == '"')
            {
                return Q2(line.Substring(1));
            }

            throw new Exception("Invalid CSV format");
        }

        private static (String Token, String? Remainder) Q1(string line)
        {
            StringBuilder sb = new StringBuilder();
            int StrIdx = 0;

            while (StrIdx < line.Length && line[StrIdx] != ',' && line[StrIdx] != '"')
            {
                sb.Append(line[StrIdx]);
                StrIdx++;
            }

            if (StrIdx < line.Length && line[StrIdx] == ',')
            {
                return (sb.ToString(), line.Substring(StrIdx + 1));
            }

            if (line.Substring(StrIdx) == "")
            {
                return (sb.ToString(), null);
            }

            throw new Exception("Invalid CSV format");
        }

        private static (String Token, String? Remainder) Q2(string line)
        {
            StringBuilder sb = new StringBuilder();
            int StrIdx = 0;

            while (StrIdx < line.Length)
            {
                char c = line[StrIdx];

                if (c == '"')
                {
                    var result = Q3(line.Substring(StrIdx + 1));

                    if (result.Token == "\"")
                    {
                        StrIdx++;
                    }
                    else if (result.Token == "")
                    {
                        return (sb.ToString(), result.Remainder);
                    }

                }

                sb.Append(c);
                StrIdx++;
            }

            throw new Exception("Invalid CSV format");
        }

        private static (String Token, String? Remainder) Q3(string line)
        {
            if (line == "")
            {
                return ("", null);
            }

            if (line[0] == '\"')
            {
                return ("\"", line.Substring(1));
            }

            if (line[0] == ',')
            {
                return ("", line.Substring(1));
            }

            throw new Exception("Invalid CSV format");
        } 
        #endregion
    }
}
