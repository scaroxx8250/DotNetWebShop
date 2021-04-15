window.onload = function () {
    let decreaseButtons = document.getElementsByClassName("cart-reduce");
    for (let i = 0; i < decreaseButtons.length; i++)
        decreaseButtons[i].addEventListener("click", DecreaseQty);

    let increaseButtons = document.getElementsByClassName("cart-add");
    for (let j = 0; j < increaseButtons.length; j++)
        increaseButtons[j].addEventListener("click", IncreaseQty);

}

function DecreaseQty(event) {
    let elem = event.currentTarget;
    let id = elem.getAttribute('data-Id');
    let qty = elem.getAttribute('data-Qty');

    qty = qty * 1;

    if (qty > 1) {
        qty = qty - 1;
        document.getElementById(id).innerHTML = qty;
    }
    else {
        document.getElementById(id).innerHTML = qty;
    }
}

function IncreaseQty(event) {
    let elem = event.currentTarget;
    let id = elem.getAttribute('data-Id');
    let qty = elem.getAttribute('data-Qty');
    qty = qty * 1;
    qty = qty + 1;
    document.getElementById(id).innerHTML = qty;
}