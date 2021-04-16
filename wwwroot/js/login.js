window.onload = function () {
	let form = document.getElementById("form");

	document.getElementById("PasswordToggle").addEventListener("mousedown", peekPwd);
	document.getElementById("PasswordToggle").addEventListener("mouseup", hidePwd);

	form.onsubmit = function () {
		let elemUname = document.getElementById("Username");
		let elemPwd = document.getElementById("Password");

		elemUname.value = elemUname.value.trim();
		elemPwd.value = elemPwd.value.trim();

		return true;
	}

	let elems = document.getElementsByClassName("form-control");
	for (let i = 0; i < elems.length; i++) {

		// remove our error message as long as any of the input boxes have focus
		elems[i].onfocus = function () {
			document.getElementById("err_msg").innerHTML = "";
		}
	}

	var field = document.querySelector('[name="password"]');

	field.addEventListener('keypress', function (event) {
		var key = event.keyCode;
		if (key === 32) {
			event.preventDefault();
		}
	});
}

function peekPwd(event) {
	let elem = event.currentTarget;
	let id = elem.getAttribute("data-id");

	document.getElementById(id).setAttribute("type", "text");

}
function hidePwd(event) {
	let elem = event.currentTarget;
	let id = elem.getAttribute("data-id");
	document.getElementById(id).setAttribute("type", "password");
}