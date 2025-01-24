

document.getElementById("Telephone").addEventListener("input", function () {

    let phoneNumberString1 = document.getElementById("Telephone").value;
    var cleaned = ("" + phoneNumberString1).replace(/\D/g, '')
    var match = cleaned.match(/^(\d{3})(\d{3})(\d{4})$/)
    if (match) {
        let input = '(' + match[1] + ') ' + match[2] + '-' + match[3]
        document.getElementById("Telephone").value = input;
        document.getElementById("Telephone").innerHTML = input;
    }

});