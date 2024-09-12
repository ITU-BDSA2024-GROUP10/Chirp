using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB
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
            IEnumerable<T> elements;
            using (var reader = new StreamReader(_fileName, Encoding.UTF8))
                using (var csv = new CsvReader(reader, _config))
                {
                    csv.Context.RegisterClassMap(_classMap);
                    elements = csv.GetRecords<T>().ToList();
                }
            return limit == null ? elements : elements.Take(limit.Value);
        }

        public void Store(T record)
        {
            var fileExists = File.Exists(_fileName);
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