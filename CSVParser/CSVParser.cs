using System.Text;

namespace CSVParser
{
    public class CSVParser
    {
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
    }
}
