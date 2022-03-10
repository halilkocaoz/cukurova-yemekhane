using System.Text;

namespace Cu.Yemekhane.API.Web.Models;

public class Menu
{
    public Menu(Date date, List<Food> foods, int? totalCalories)
    {
        Date = date;
        Foods = foods;
        TotalCalories = totalCalories is 0 or null ? foods.Sum(x => x.Calories) : totalCalories.Value;
    }

    public Date Date { get; private set; }
    public List<Food> Foods { get; private set; }
    public int TotalCalories { get; private set; }
    public string Detail => this.ToString();

    public override string ToString()
    {
        StringBuilder detailStrBuilder = new StringBuilder($"Menu({Date.ToString()}):");
        Foods.ForEach(food => detailStrBuilder.Append($"\n{food.ToString()}"));
        return detailStrBuilder.ToString();
    }
}