using System.Text;

namespace SpixManga.Misc;

public class Base36Converter
{
    internal static string ToBase36(long number)
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (number == 0) return "0";

        StringBuilder result = new StringBuilder();
        while (number > 0)
        {
            result.Insert(0, chars[(int)(number % 36)]);
            number /= 36;
        }

        return result.ToString();
    }

    internal static long FromBase36(string input)
    {
        const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        input = input.ToUpper();

        long result = 0;
        foreach (char c in input)
        {
            result = result * 36 + chars.IndexOf(c);
        }

        return result;
    }
}