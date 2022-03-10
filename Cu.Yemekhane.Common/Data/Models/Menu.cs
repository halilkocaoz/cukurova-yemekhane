using System.Text;

namespace Cu.Yemekhane.Common.Data.Models;

public class Menu
{
    public Menu(string date, List<Food> foods, int? totalCalories)
    {
        Date = date;
        Foods = foods;
        TotalCalories = totalCalories is 0 or null ? foods.Sum(x => x.Calories) : totalCalories.Value;
        Detail = this.ToString();
    }

    public string Date { get; private set; }
    public List<Food> Foods { get; private set; }
    public int TotalCalories { get; private set; }
    public string Detail { get; private set; }

    public override string ToString()
    {
        StringBuilder detailStrBuilder = new StringBuilder($"Menu({Date}):");
        Foods.ForEach(food => detailStrBuilder.Append($"\n{food.ToString()}"));
        return detailStrBuilder.ToString();
    }
}