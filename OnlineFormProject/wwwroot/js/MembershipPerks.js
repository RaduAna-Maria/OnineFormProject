function toggleExtraToppings() {
    const membershipSelect = document.getElementById("Membership");
    const extraToppingsContainer = document.querySelector(".extra-toppings-container");

    if (membershipSelect && extraToppingsContainer) {
        const selectedMembership = membershipSelect.value;

        // Show extra toppings if the membership type is Regular or Fancy
        if (selectedMembership === "Regular" || selectedMembership === "Fancy") {
            extraToppingsContainer.classList.remove("hidden");
        } else {
            extraToppingsContainer.classList.add("hidden");
        }
    }
}

function updateMembershipInfo() {
    toggleExtraToppings();
    calculateTotalPrice();
}

window.addEventListener("load", function () {
    document.getElementById("Membership").onchange = updateMembershipInfo;
});
