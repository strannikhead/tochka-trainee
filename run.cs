using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


class HotelCapacity
{
    private static bool CheckCapacity(int maxCapacity, List<Guest> guests)
    {
        var dates = new List<CheckDate>();
        foreach (var guest in guests)
        {
            dates.Add(new CheckDate { Date = guest.CheckIn, IsCheckIn = true });
            dates.Add(new CheckDate { Date = guest.CheckOut, IsCheckIn = false });
        }

        var sortedDates = dates.OrderBy(x => x.Date).ThenBy(x => x.IsCheckIn);
        var currentCount = 0;
        foreach (var date in sortedDates)
        {
            if (date.IsCheckIn)
                currentCount++;
            else
                currentCount--;

            if (currentCount > maxCapacity)
                return false;
        }

        return true;
    }


    private class CheckDate
    {
        public string Date { get; set; }
        public bool IsCheckIn { get; set; }
    }

    private class Guest
    {
        public string Name { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
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
            guest.CheckIn = checkInMatch.Groups[1].Value;


        var checkOutMatch = Regex.Match(json, "\"check-out\"\\s*:\\s*\"([^\"]+)\"");
        if (checkOutMatch.Success)
            guest.CheckOut = checkOutMatch.Groups[1].Value;


        return guest;
    }
}