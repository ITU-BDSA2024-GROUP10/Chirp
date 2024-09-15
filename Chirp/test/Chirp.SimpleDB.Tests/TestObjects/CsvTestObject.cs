﻿namespace Chirp.SimpleDB.Tests;

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

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        CsvTestObject other = (CsvTestObject)obj;
        return field1String == other.field1String && field2Int == other.field2Int &&
               field3DataTime == other.field3DataTime && field4Bool == other.field4Bool;
    }
}