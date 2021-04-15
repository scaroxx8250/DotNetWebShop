window.onload = function () {
    let elemList = document.getElementsByClassName("addItem");
    for (let i = 0; i < elemList.length; i++)
        elemList[i].addEventListener("click", GetProduct);
}
function GetProduct(event) {
    let elem = event.currentTarget;
    let id = elem.getAttribute('data-Id');
    let name = elem.getAttribute('data-name');
    let price = elem.getAttribute('data-price');
    let desc = elem.getAttribute('data-desc');
    let image = elem.getAttribute('data-imagePath');

    AddProduct(id, name, price, desc, image);
}
function AddProduct(id, name, price, desc, image) {
    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Home/AddToCart");
    xhr.setRequestHeader("Content-Type", "application/json; charset=utf8");
    xhr.onreadystatechange = function () {
        if (this.readyState === XMLHttpRequest.DONE) {
            // check if HTTP operation is ok
            if (this.status !== 200)
                return;

             //get response data from server
            let data = JSON.parse(this.responseText);

           //error from server
            if (!data.success)
                alert("Please try again");

            //success result from server
            if (data.success)
            {
                document.getElementById("lblCartCount").innerHTML = data.quantity;
                alert("The product is added to the cart");
            }
            
        }
    };

    //convert string into number in order to be able to pass parameter to controller
    id = id * 1;
    price = price * 1;

    //construct JSON object
        data = {
            "Id": id,
            "ProductName": name,
            "Price": price,
            "description": desc,
            "imagePath": image
        };
  
    // send data to server
    xhr.send(JSON.stringify(data));
}