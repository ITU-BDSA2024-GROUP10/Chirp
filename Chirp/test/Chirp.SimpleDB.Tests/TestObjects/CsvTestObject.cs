namespace Chirp.SimpleDB.Tests;

public class CsvTestObject
{
    public string field1String { get; set; }
    public int field2Int { get; set; }
    public DateTime field3DataTime { get; set; }
    public bool field4Bool { get; set; }

    public CsvTestObject()
    {
    }

    public CsvTestObject(string field1String, int field2Int, DateTime field3DataTime, bool field4Bool)
    {
        this.field1String = field1String;
        this.field2Int = field2Int;
        this.field3DataTime = field3DataTime;
        this.field4Bool = field4Bool;
    }
    
    public CsvTestObject(string[] fields)
    {
        field1String = fields[0];
        field2Int = int.Parse(fields[1]);
        field3DataTime = DateTime.Parse(fields[2]);
        field4Bool = bool.Parse(fields[3]);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        CsvTestObject other = (CsvTestObject)obj;
        return field1String.Equals(other.field1String) && field2Int.Equals(other.field2Int) &&
               field3DataTime.ToString().Equals(other.field3DataTime.ToString()) && field4Bool.Equals(other.field4Bool);
    }

    public override string ToString()
    {
        return $"{field1String},{field2Int},{field3DataTime},{field4Bool}";
    }
}