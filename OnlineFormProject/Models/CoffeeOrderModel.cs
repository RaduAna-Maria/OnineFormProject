using OnlineFormProject.Constants;

namespace OnlineFormProject.Models
{
    public class CoffeeOrderModel
    {
        // User details
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Coffee selection
        public string SelectedCoffee { get; set; }
        public string SelectedSize { get; set; }
        public List<string> SelectedToppings { get; set; }

        // Membership perks
        public MembershipType Membership { get; set; }
        public List<string> ExtraToppings { get; set; }

        // Order
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        // Discounts and prices
        public static Dictionary<CoffeeType, decimal> CoffeePrices => CoffeeOrderPrices.CoffeePrices;
        public static Dictionary<CoffeeSize, decimal> SizePrices => CoffeeOrderPrices.SizePrices;
        public static decimal ToppingPrice => CoffeeOrderPrices.ToppingPrice;
        public static Dictionary<MembershipType, decimal> MembershipDiscounts => CoffeeOrderPrices.MembershipDiscounts;

        public CoffeeOrderModel()
        {
            Name = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            SelectedCoffee = string.Empty;
            SelectedSize = string.Empty;
            SelectedToppings = new List<string>();
            ExtraToppings = new List<string>();
        }
    }
}