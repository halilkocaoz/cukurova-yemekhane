namespace Cu.Yemekhane.Common.Data.Models;
public class Date
{
    public Date(string day, string month, string year)
    {
        Day = day;
        Month = month;
        Year = year;
        Fully = ToString();
    }
    public string Day { get; private set; }
    public string Month { get; private set; }
    public string Year { get; private set; }
    public string Fully { get; private set; }


    public override string ToString()
    {
        return $"{Day}.{Month}.{Year}";
    }
}