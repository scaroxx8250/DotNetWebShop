window.onload = function () {
    let decreaseQty = document.getElementsByClassName("cart-reduce");
    for (let i = 0; i < decreaseQty.length; i++) {
        decreaseQty[i].addEventListener("click", decreaseValue);
    }
    let increaseQty = document.getElementsByClassName("cart-add");
    for (let i = 0; i < increaseQty.length; i++) {
        increaseQty[i].addEventListener("click", increaseValue);
    }
}
function increaseValue() {
    let elem = event.currentTarget;
    var value = parseInt(elem.getElementById('number').innerHTML);
    value = isNaN(value) ? 0 : value;
    value++;
    document.getElementById('number').innerHTML = value;
}

function decreaseValue() {
    var value = parseInt(document.getElementById('number').innerHTML);
    value = isNaN(value) ? 0 : value;
    value < 2 ? value = 1 : value--;
    document.getElementById('number').innerHTML = value;
}
