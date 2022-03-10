namespace Cu.Yemekhane.API.Web.Models.Data;
public class Date
{
    public Date(string day, string month, string year)
    {
        Day = day;
        Month = month;
        Year = year;
    }
    public string Day { get; private set; }
    public string Month { get; private set; }
    public string Year { get; private set; }

    public override string ToString()
    {
        return $"{Day}.{Month}.{Year}";
    }
}