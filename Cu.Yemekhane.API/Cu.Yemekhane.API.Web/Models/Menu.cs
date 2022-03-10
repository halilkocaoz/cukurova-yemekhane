namespace Cu.Yemekhane.API.Web.Models;

public class Menu
{
    public Menu(Date date, List<Food> foods, int totalCalories)
    {
        Date = date;
        Foods = foods;
        TotalCalories = totalCalories;
    }

    public Date Date { get; private set; }
    public List<Food> Foods { get; private set; }
    public int TotalCalories { get; private set; }
}