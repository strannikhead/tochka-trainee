using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


class HotelCapacity
{
    private static bool CheckCapacity(int maxCapacity, List<Guest> guests)
    {
        var sortedDates = guests
            .SelectMany(guest => new[]
            {
                new CheckDate { Date = guest.CheckIn, IsCheckIn = true },
                new CheckDate { Date = guest.CheckOut, IsCheckIn = false }
            })
            .OrderBy(x => x.Date)
            .ThenBy(x => x.IsCheckIn);

        var currentCount = 0;
        foreach (var date in sortedDates)
        {
            currentCount += date.IsCheckIn ? 1 : -1;

            if (currentCount > maxCapacity)
                return false;
        }

        return true;
    }


    private struct CheckDate
    {
        public DateTime Date { get; set; }
        public bool IsCheckIn { get; set; }
    }

    private class Guest
    {
        public string Name { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }


    public static void Main()
    {
        var maxCapacity = int.Parse(Console.ReadLine());
        var n = int.Parse(Console.ReadLine());


        var guests = new List<Guest>();


        for (var i = 0; i < n; i++)
        {
            var line = Console.ReadLine();
            var guest = ParseGuest(line);
            guests.Add(guest);
        }

        var result = CheckCapacity(maxCapacity, guests);


        Console.WriteLine(result ? "True" : "False");
    }


    private static Guest ParseGuest(string json)
    {
        var guest = new Guest();


        var nameMatch = Regex.Match(json, "\"name\"\\s*:\\s*\"([^\"]+)\"");
        if (nameMatch.Success)
            guest.Name = nameMatch.Groups[1].Value;


        var checkInMatch = Regex.Match(json, "\"check-in\"\\s*:\\s*\"([^\"]+)\"");
        if (checkInMatch.Success)
            guest.CheckIn = DateTime.ParseExact(checkInMatch.Groups[1].Value, "yyyy-MM-dd", null);


        var checkOutMatch = Regex.Match(json, "\"check-out\"\\s*:\\s*\"([^\"]+)\"");
        if (checkOutMatch.Success)
            guest.CheckOut = DateTime.ParseExact(checkOutMatch.Groups[1].Value, "yyyy-MM-dd", null);


        return guest;
    }
}