window.onload = function () {
    let decreaseQty = document.getElementsByClassName("cart-reduce");
    for (let i = 0; i < decreaseQty.length; i++) {
        decreaseQty[i].addEventListener("click", decreaseValue);
    }
    let increaseQty = document.getElementsByClassName("cart-add");
    for (let i = 0; i < increaseQty.length; i++) {
        increaseQty[i].addEventListener("click", increaseValue);
    }
    let qty = document.getElementsByClassName("quantity");
    for (let i = 0; i < qty.length; i++) {
        qty[i].addEventListener("click", updateSubtotal);
    }
}
function increaseValue(event) {
    let elem = event.currentTarget;
    let name = elem.getAttribute("data-product");
    var value = parseInt(document.getElementById(name).innerHTML);
    value = isNaN(value) ? 0 : value;
    value++;
    document.getElementById(name).innerHTML = value;
}

function decreaseValue(event) {
    let elem = event.currentTarget;
    let name = elem.getAttribute("data-product");
    var value = parseInt(document.getElementById(name).innerHTML);
    value = isNaN(value) ? 0 : value;
    value < 2 ? value = 1 : value--;
    document.getElementById(name).innerHTML = value;
}

function updateSubtotal(event) {
    
    
}

