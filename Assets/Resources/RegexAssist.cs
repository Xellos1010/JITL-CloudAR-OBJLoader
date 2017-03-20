using System.Text.RegularExpressions;

public static class RegexAssist
{
    public static string RemoveReturnCarriage(string unformattedText)
    {
        Regex regex_newline = new Regex("(\r)");
        return regex_newline.Replace(unformattedText, string.Empty);
    }

    public static string CustomRemove(string unformattedText, string regex)
    {
        Regex regex_newline = new Regex(regex);
        return regex_newline.Replace(unformattedText, string.Empty);
    }
}