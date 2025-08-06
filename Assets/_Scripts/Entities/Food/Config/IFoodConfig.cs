using _Scripts.GlobalConfigs;

namespace _Scripts.Entities.Food.Config
{
    public interface IFoodConfig
    {
        GridConfig GridConfig { get; set; }
        string FoodSpriteAddressableKey { get; set; }
    }
}