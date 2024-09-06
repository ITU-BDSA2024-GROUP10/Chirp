﻿namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
        {
            foreach (var vCheep in cheeps)
            {
                Console.WriteLine(vCheep);
            }
        }
}