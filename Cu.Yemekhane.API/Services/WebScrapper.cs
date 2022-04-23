using System.Text;
using Cu.Yemekhane.Common.Models.Data;
using HtmlAgilityPack;

namespace Cu.Yemekhane.API.Services;

public interface IWebScrapper
{
    List<Menu> ScrapMenus();
}

public class WebScrapper : IWebScrapper
{
    private readonly HtmlWeb _htmlWeb;
    private const string cuYemekhaneUrl = "https://yemekhane.cu.edu.tr/default.asp";
    public WebScrapper()
    {
        EncodingProvider encodingProvider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(encodingProvider);
        _htmlWeb = new HtmlWeb
        {
            AutoDetectEncoding = false,
            OverrideEncoding = Encoding.GetEncoding("iso-8859-9"),
        };
    }

    public List<Menu> ScrapMenus()
    {
        List<Menu> result = new();
        List<Food> tempFoods = new();
        var doc = _htmlWeb.Load(cuYemekhaneUrl);
        var menuDivNodes = doc.DocumentNode.SelectNodes("//div[@data-animation='flipInY']").ToList();
        menuDivNodes.ForEach(menuDivNode =>
        {
            var menuDivAElementNodes = menuDivNode.Descendants().Where(x => x.OriginalName == "a").ToList();
            var menuHasItems = menuDivAElementNodes.Any();
            if (menuHasItems)
            {
                var menuDateInformationNode = menuDivAElementNodes.First().ChildNodes;
                var menuDate = menuDateInformationNode.First().InnerText;
                var menuFoodNodes = menuDivAElementNodes.Skip(1).ToList();
                menuFoodNodes.ForEach(menuFoodNode =>
                {
                    var foodName = menuFoodNode.ChildNodes.First().InnerText.Trim();
                    int.TryParse(menuFoodNode.ChildNodes.Last().InnerText.Replace("Kalori", "").Trim(), out int foodCalories);
                    tempFoods.Add(new Food(foodName, foodCalories));
                });
                result.Add(new Menu(menuDate, tempFoods, 0));
                tempFoods = new();
            }
        });
        return result;
    }
}