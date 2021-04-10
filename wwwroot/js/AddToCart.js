window.onload = function () {
    let elemList = document.getElementsByClassName("addItem");
    for (let i = 0; i < elemList.length; i++)
        elemList[i].addEventListener("click", AddItem);
}
function AddItem(event) {
    let elem = event.currentTarget;
    let id = elem.getAttribute('data-prodId');
    let name = elem.getAttribute('data-name');
    let price = elem.getAttribute('data-price');
    let desc = elem.getAttribute('data-desc');
    let image = elem.getAttribute('data-imagePath');

    SendItem(id, name, price, desc, image);
}
function SendItem(id, name, price, desc, image) {
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/AddToCart");
    xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");
    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            // check if HTTP operation is okay
            if (this.status !== 200)
                return;

            let data = JSON.parse(this.responseText);

            // if some error on server, don't update client's view
            if (!data.success)
                return;

            if (data.success)
                document.getElementById("lblCartCount").innerHTML = data.quantity;
        }
    };






    let data = {
        "ProductId": id,
        "ProductName": name,
        "Price": price,
        "description": desc,
        "imagePath": image
    };
    // send data to server
    xhr.send(JSON.stringify(data));
}