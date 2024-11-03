using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineFormProject.Constants;
using OnlineFormProject.Models;
using System.Diagnostics;
using InfluxDB3.Client;
using InfluxDB3.Client.Write;

namespace OnlineFormProject.Controllers
{
    public class CoffeeOrderController : Controller
    {
        private readonly string bucket = "bucket";

        private readonly InfluxDBClient _influxClient;

        public CoffeeOrderController(InfluxDBClient influxClient)
        {
            _influxClient = influxClient;
        }


        [HttpGet]
        public IActionResult CoffeeOrderForm()
        {
            var model = new CoffeeOrderModel
            {
                Name = string.Empty,
                Email = string.Empty,
                Phone = string.Empty,
                SelectedCoffee = string.Empty,
                SelectedSize = string.Empty,
                SelectedToppings = new List<string>(),
                Membership = MembershipType.None,
                Quantity = 1,
                TotalPrice = 0m,
            };

            LoadViewData();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CoffeeOrderForm(CoffeeOrderModel model)
        {
            model.SelectedToppings = [.. Request.Form["SelectedToppings"]];
            model.ExtraToppings = [.. Request.Form["ExtraToppings"]];

            if (!ModelState.IsValid)
            {
                LoadViewData();
                return View(model);
            }

            GenerateReceipt(model);
            AppendToCSV(model);
            await SendOrderToInfluxDb(model);
            ResetModelOnSubmit(model);

            LoadViewData();
            return View(model);
        }

        private void GenerateReceipt(CoffeeOrderModel model)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var randomNumber = new Random().Next(1000, 9999);
            var fileName = $"Receipt_{timestamp}_{randomNumber}.txt";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Receipts", fileName);

            var dataToWrite = $"Name: {model.Name}\nEmail: {model.Email}\nPhone: {model.Phone}\n" +
                              $"Order: {model.SelectedSize} {model.SelectedCoffee}\n" +
                              $"Toppings: {string.Join(", ", model.SelectedToppings)}\n" +
                              $"Membership: {model.Membership}\nExtra Topping: {model.ExtraToppings.FirstOrDefault()}\n" +
                              $"Quantity: {model.Quantity}\n" +
                              $"Total Price: {model.TotalPrice:C}\n\n";

            System.IO.File.AppendAllText(filePath, dataToWrite);
        }

        private async Task SendOrderToInfluxDb(CoffeeOrderModel model)
        {
            var point = PointData
                .Measurement("coffee_orders")
                .SetStringField("name", model.Name)
                .SetStringField("email", model.Email)
                .SetStringField("phone", model.Phone)
                .SetStringField("size", model.SelectedSize)
                .SetStringField("coffee_type", model.SelectedCoffee)
                .SetStringField("toppings", string.Join(";", model.SelectedToppings) ?? "")
                .SetTag("membership", model.Membership.ToString())
                .SetStringField("extra_toppings", string.Join(";", model.ExtraToppings) ?? "")
                .SetIntegerField("quantity", model.Quantity)
                .SetField("total_price", model.TotalPrice);

            await _influxClient.WritePointAsync(point, bucket);
        }


        private void AppendToCSV(CoffeeOrderModel model)
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Receipts");
            var csvFilePath = Path.Combine(directoryPath, "Orders.csv");

            var csvLine = string.Join(",", new string[]
            {
                model.Name,
                model.Email,
                model.Phone,
                model.SelectedSize,
                model.SelectedCoffee,
                string.Join(";", model.SelectedToppings) ?? "",
                model.Membership.ToString(),
                string.Join(";", model.ExtraToppings) ?? "",
                model.Quantity.ToString(),
                model.TotalPrice.ToString("F2"),
            });

            using (var writer = new StreamWriter(csvFilePath, append: true))
            {
                if (new FileInfo(csvFilePath).Length == 0)
                {
                    writer.WriteLine("Name,Email,Phone,Size,Coffee,Toppings,Membership,ExtraToppings,Quantity,TotalPrice");
                }

                writer.WriteLine(csvLine);
            }
        }

        private void ResetModelOnSubmit(CoffeeOrderModel model)
        {
            model.Name = string.Empty;
            model.Email = string.Empty;
            model.Phone = string.Empty;
            model.SelectedCoffee = string.Empty;
            model.SelectedSize = string.Empty;
            model.SelectedToppings = new List<string>();
            model.Membership = MembershipType.None;
            model.Quantity = 1;
            model.TotalPrice = 0m;
        }

        private void LoadViewData()
        {
            SetViewData<MembershipType>("MembershipType");
            SetViewData<CoffeeType>("CoffeeType");
            SetViewData<CoffeeSize>("CoffeeSize");
            SetViewData<CoffeeToppings>("SelectedToppings");
            SetViewData<CoffeeToppings>("ExtraToppings");
        }

        private void SetViewData<TEnum>(string viewDataKey) where TEnum : struct, Enum
        {
            var enumValues = Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(c => new { Value = c.ToString(), Text = c.ToString() })
                .ToList();

            ViewData[viewDataKey] = new SelectList(enumValues, "Value", "Text");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
