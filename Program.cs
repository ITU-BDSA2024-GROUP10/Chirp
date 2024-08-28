// See https://aka.ms/new-console-template for more information
if (args.Length == 3)
{
    if (args[0] == "say")
    {
        var message = args[1];
        var frequency = int.Parse(args[2]);
        foreach (var i in Enumerable.Range(1, frequency))
        {
            Console.Write(message + " ");
        }
    }
}
else
{
    Console.WriteLine("Incorrect use case:");
    Console.WriteLine("Correct use case: dotnet run -- say \"Message\" count");
}



