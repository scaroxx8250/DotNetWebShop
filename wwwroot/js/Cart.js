window.onload = function () {
    let decreaseQty = document.getElementsByClassName("cart-reduce");
    for (let i = 0; i < decreaseQty.length; i++) {
        decreaseQty[i].addEventListener("click", DecreaseValue);
    }

    let increaseQty = document.getElementsByClassName("cart-add");
    for (let i = 0; i < increaseQty.length; i++) {
        increaseQty[i].addEventListener("click", IncreaseValue);
    }

    let elemList = document.getElementsByClassName("cart-del");
    for (let i = 0; i < elemList.length; i++) {
        elemList[i].addEventListener("click", SelectProduct);
    }

    //replace all leading non-digits with nothing
    let total = parseInt(document.getElementById("total").innerHTML.replace(/^\D+/g, ''));
    let getSubtotal = document.getElementsByClassName("cart-subtotal");
    for (let i = 0; i < getSubtotal.length; i++) {
        total += parseInt(getSubtotal[i].innerHTML.replace(/^\D+/g, '')); 
    }
    document.getElementById("total").innerHTML = "$"+ total+ ".00";
    CheckTotal();
}

function CheckTotal() {
    if (document.getElementById("total").innerHTML === "$0.00") {
        document.getElementById("checkout").disabled = true;
        document.getElementById("checkout").classList.add("disabledBtn");
        return false;
    }
    else {
        document.getElementById("checkout").disabled = false;
        document.getElementById("checkout").classList.remove("disabledBtn");
        return true;
    }
}

function IncreaseValue(event) {
    let elem = event.currentTarget;
    let name = elem.getAttribute("data-product");
    let productId = elem.getAttribute("data-ProductId");
    let cartId = elem.getAttribute("data-CartId");

    var value = parseInt(document.getElementById(name).innerHTML);
    value = isNaN(value) ? 0 : value;
    value++;
    document.getElementById(name).innerHTML = value;

    let unitPrice = elem.getAttribute("data-price");
    let getTotal= elem.getAttribute("data-desc");

    var subtotal = unitPrice * value;
    document.getElementById(getTotal).innerHTML = "$"+subtotal + ".00";

    let total = 0;
    let subtotalList = document.getElementsByClassName("cart-subtotal");
    for (let i = 0; i < subtotalList.length; i++) {
        total += parseInt(subtotalList[i].innerHTML.replace(/^\D+/g,'')); 
    }
    document.getElementById("total").innerHTML = "$" + total + ".00";

    UpdateQuantity( productId, cartId, value);
}

function DecreaseValue(event) {
    let elem = event.currentTarget;
    let name = elem.getAttribute("data-product");
    let productId = elem.getAttribute("data-ProductId");
    let cartId = elem.getAttribute("data-CartId");

    var value = parseInt(document.getElementById(name).innerHTML);
    value = isNaN(value) ? 0 : value;
    value < 2 ? value = 1 : value--;
    document.getElementById(name).innerHTML = value;

    let unitPrice = elem.getAttribute("data-price");
    let getTotal = elem.getAttribute("data-desc");
    var subtotal = unitPrice * value;
    document.getElementById(getTotal).innerHTML = "$" + subtotal + ".00";

    let total = 0;
    let getSubtotal = document.getElementsByClassName("cart-subtotal");
    for (let i = 0; i < getSubtotal.length; i++) {
        total += parseInt(getSubtotal[i].innerHTML.replace(/^\D+/g, ''));
    }
    document.getElementById("total").innerHTML = "$" + total + ".00";
 
    UpdateQuantity( productId, cartId, value);
}

function UpdateQuantity( productId, cartId, value) {
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/UpdateCart");
    xhr.setRequestHeader("Content-Type", "application/json; charaset=utf8");
    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {

            //check if HTTP Operation is ok
            if (this.status !== 200)
                return;

            //get response data from server
            let data = JSON.parse(this.responseText);


            //error from server
            if (!data.success)
                return;

            //success result from server
            if (data.success) {
                return;
            }
        }
    };

    //convert string into number in order to be able to pass parameter to controller action method
    productId *= 1;
    cartId *= 1;
    value *= 1;

    //construct JSON object
    data = { "ProductId": productId, "CartId": cartId, "Qty": value };

    //send data to server
    xhr.send(JSON.stringify(data));
}

function SelectProduct(event) {
    let elem = event.currentTarget;
    let productId = elem.getAttribute('data-ProductId');
    let cartId = elem.getAttribute('data-CartId');
    RemoveProduct(productId, cartId);

}
function RemoveProduct(productId, cartId) {
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/RemoveFromCart");
    xhr.setRequestHeader("Content-Type", "application/json; charaset=utf8");
    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {

            //check if HTTP Operation is ok
            if (this.status !== 200)
                return;

            //get response data from server
            let data = JSON.parse(this.responseText);

            //error from server
            if (!data.success)
                return;

            //success result from server
            if (data.success) {

                //remove the product shown in the html page
                document.getElementById(productId).remove();

                //update the total price after remove product
                let total = 0;
                let getSubtotal = document.getElementsByClassName("cart-subtotal");
                for (let i = 0; i < getSubtotal.length; i++) {

                    //replace all leading non-digits with nothing
                    total += parseInt(getSubtotal[i].innerHTML.replace(/^\D+/g, '')); 
                }
                document.getElementById("total").innerHTML = "$" + total + ".00";
                CheckTotal();
            }
        }
    };

    //convert string into number in order to be able to pass parameter to controller action method
    productId *= 1;
    cartId *= 1;

    //construct JSON object
    data = { "ProductId": productId, "CartId": cartId };

    //send data to server
    xhr.send(JSON.stringify(data));
}


