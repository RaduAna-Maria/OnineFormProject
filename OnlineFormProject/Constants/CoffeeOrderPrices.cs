using System.Text.Json;

namespace OnlineFormProject.Constants
{
    public static class CoffeeOrderPrices
    {
        public static readonly Dictionary<CoffeeType, decimal> CoffeePrices = new Dictionary<CoffeeType, decimal>
        {
            { CoffeeType.Espresso, 2.00m },
            { CoffeeType.Latte, 3.00m },
            { CoffeeType.Cappuccino, 3.50m },
            { CoffeeType.Americano, 3.75m },
            { CoffeeType.Mocha, 3.00m },
            { CoffeeType.ColdBrew, 3.50m },
        };

        public static readonly Dictionary<CoffeeSize, decimal> SizePrices = new Dictionary<CoffeeSize, decimal>
        {
            { CoffeeSize.Small, 0.00m },
            { CoffeeSize.Medium, 0.50m },
            { CoffeeSize.Large, 1.00m },
        };

        public const decimal ToppingPrice = 0.25m;

        public static readonly Dictionary<MembershipType, decimal> MembershipDiscounts = new Dictionary<MembershipType, decimal>
        {
            { MembershipType.Regular, 0.10m },
            { MembershipType.Fancy, 0.15m },
            { MembershipType.None, 0.00m },
        };

        public static string GetSerializedCoffeePrices()
        {
            return JsonSerializer.Serialize(CoffeePrices);
        }

        public static string GetSerializedSizePrices()
        {
            return JsonSerializer.Serialize(SizePrices);
        }

        public static string GetSerializedMembershipDiscounts()
        {
            var discounts = new Dictionary<string, decimal>();
            foreach (var discount in MembershipDiscounts)
            {
                discounts.Add(discount.Key.ToString(), discount.Value);
            }
            return JsonSerializer.Serialize(discounts);
        }
    }
}
