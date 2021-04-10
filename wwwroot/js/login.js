window.onload = function () {
	let errDiv = document.getElementById("err_msg");

	let form = document.getElementById("form");
	form.onsubmit = function () {
		let elemUname = document.getElementById("Username");
		let elemPwd = document.getElementById("Password");

		elemUname.value.trim();
		elemPwd.value.trim();

		//if (username.length === 0 || password.length === 0) {
			//errDiv.innerHTML = "Please fill up all fields.";
			//return false;	// cancel form submission
		//}

		return true;	// allow form submission to continue
	}

	let elems = document.getElementsByClassName("form-control");
	for (let i = 0; i < elems.length; i++) {
		// remove our error message as long as any 
		// of the input boxes have focus
		elems[i].onfocus = function () {
			errDiv.innerHTML = "";
		}
	}
}


var pw = document.querySelector('[name="password"]');

pw.addEventListener('keypress', function (event) {
	var key = event.keyCode;
	if (key === 32) {
		event.preventDefault();
	}
});

