using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB
{
    public class CsvDatabase<T> : IDatabaseRepository<T>
    {   
        private readonly string _fileName;
        private readonly CsvConfiguration _config;
        private readonly ClassMap<T> _classMap;

        public CsvDatabase(string fileName, ClassMap<T> classMap) 
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
            try
            {
                return PerformRead(limit);
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine($"ERROR: File {_fileName} not found while reading cheeps. Exception: {ex.Message}");
                throw;
            }
            catch (CsvHelperException ex)
            {
                Console.Error.WriteLine($"ERROR: Error occurred while parsing the CSV file {_fileName}. Exception: {ex.Message}");
                throw new InvalidOperationException($"Error occurred while reading from the file {_fileName}", ex);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR: An unexpected error occurred while reading cheeps. Exception: {ex.Message}");
                throw new Exception("An unexpected error occurred while reading from the file.", ex);
            }
        }

        public IEnumerable<T> PerformRead(int? limit = null)
        {
            List<T> elements;
            using (var reader = new StreamReader(_fileName, Encoding.UTF8))
            using (var csv = new CsvReader(reader, _config))
            {
                csv.Context.RegisterClassMap(_classMap);
                elements = csv.GetRecords<T>().ToList();
            }
            
            if (limit == null || limit > elements.Count())
            {
                limit = elements.Count();
            }

            return elements.GetRange((int)(elements.Count() - limit), (int)limit);
        }

        public void Store(T record)
        {
            try
            {
                PerformStore(record);
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine($"ERROR: File {_fileName} not found while storing cheep. Exception: {ex.Message}");
                throw;
            }
            catch (CsvHelperException ex)
            {
                Console.Error.WriteLine($"ERROR: Error occurred while saving cheep. Exception: {ex.Message}");
                throw new InvalidOperationException($"Error occurred while writing cheep to file {_fileName}", ex);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR: An unexpected error occurred while storing the cheep. Exception: {ex.Message}");
                throw new Exception("An unexpected error occurred while storing the cheep.", ex);
            }
        }

        public void PerformStore(T record)
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