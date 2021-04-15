window.onload = function () {
    let decreaseQty = document.getElementsByClassName("cart-reduce");
    for (let i = 0; i < decreaseQty.length; i++) {
        decreaseQty[i].addEventListener("click", decreaseValue);
    }
    let increaseQty = document.getElementsByClassName("cart-add");
    for (let i = 0; i < increaseQty.length; i++) {
        increaseQty[i].addEventListener("click", increaseValue);
    }
    let elemList = document.getElementsByClassName("cart-del");
    for (let i = 0; i < elemList.length; i++)
        elemList[i].addEventListener("click", SelectProduct);
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
            }

        }
    };
    //convert string into number in order to be able to pass parameter to controller
    productId *= 1;
    cartId *= 1;

    //construct JSON object
    data = { "ProductId": productId, "CartId": cartId };

    //send data to server
    xhr.send(JSON.stringify(data));
}


