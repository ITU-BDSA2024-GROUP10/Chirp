using SimpleDB;
using CsvHelper;
using CsvHelper.Configuration;
using System.Text;
using System.Globalization;

namespace Chirp.CLI.SimpleDB
{
    public class CSVDatabase<T> : IDatabaseRepository<T>
    {   
        private readonly string _fileName;
        private readonly CsvConfiguration _config;
        private readonly ClassMap<T> _classMap;

        public CSVDatabase(string fileName, ClassMap<T> classMap) 
        {
            _fileName = fileName;
            _classMap = classMap;
            _config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Encoding = Encoding.UTF8
            };
        }

        public IEnumerable<T> Read(int? limit = null)
        {
            List<T> elements = new();
            using (var reader = new StreamReader(_fileName, Encoding.UTF8))
            using (var csv = new CsvReader(reader, _config))
            {
                csv.Context.RegisterClassMap(_classMap);
                elements = csv.GetRecords<T>().ToList();
            }
            
            var amountToRead = Math.Min(elements.Count, limit ?? elements.Count); // get or default to elements.Count

            return elements.GetRange(elements.Count - amountToRead, amountToRead);
        }

        public void Store(T record)
        {
            bool fileExists = File.Exists(_fileName);
            using var writer = new StreamWriter(_fileName, append: true, Encoding.UTF8);
            using var csv = new CsvWriter(writer, _config);
            csv.Context.RegisterClassMap(_classMap);

            if (!fileExists)
            {
                csv.WriteHeader<T>();
                csv.NextRecord();
            }

            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }
}