function calculateTotalPrice() {
    const coffeePrices = JSON.parse(document.getElementById("CoffeePrices").value);
    const sizePrices = JSON.parse(document.getElementById("SizePrices").value);
    const membershipDiscounts = JSON.parse(document.getElementById("MembershipDiscounts").value);
    const toppingPrice = parseFloat(document.getElementById("ToppingPrice").value);

    const selectedCoffee = document.getElementById("SelectedCoffee").value;
    const selectedSize = document.getElementById("SelectedSize").value;
    const quantity = parseInt(document.getElementById("Quantity").value, 10) || 1;
    const selectedMembership = document.getElementById("Membership").value;

    let totalPrice = 0;

    // Get the price of the selected coffee
    if (coffeePrices[selectedCoffee]) {
        totalPrice += coffeePrices[selectedCoffee];
    }

    // Get the price of the selected size
    if (sizePrices[selectedSize]) {
        totalPrice += sizePrices[selectedSize];
    }

    // Add topping costs
    const selectedToppings = document.getElementsByName("SelectedToppings");
    for (const topping of selectedToppings) {
        if (topping.checked) {
            totalPrice += toppingPrice;
        }
    }

    // Apply membership discount
    if (membershipDiscounts[selectedMembership]) {
        totalPrice -= totalPrice * membershipDiscounts[selectedMembership];
    }

    totalPrice *= quantity;

    document.getElementById("TotalPrice").value = totalPrice.toFixed(2);
}

window.addEventListener("load", function () {
    calculateTotalPrice();

    document.getElementById("SelectedCoffee").onchange = calculateTotalPrice;
    document.getElementById("SelectedSize").onchange = calculateTotalPrice;
    document.getElementById("Quantity").onchange = calculateTotalPrice;

    const toppings = document.getElementsByName("SelectedToppings");
    for (const topping of toppings) {
        topping.onchange = calculateTotalPrice;
    }
});
