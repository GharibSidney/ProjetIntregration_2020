let champDate = document.getElementById("champDate");
let radioDate = document.getElementById("rechercherDate");
//Je dois mettre une valeur pour ne pas qu'il n'y ait pas d'erreur si jamais l'élément n'est pas présent dans la page.
let radioPaiement = "";

//Retourne true si l'élément est dans la vue.
if (!!document.getElementById("rechercherPaiement")) {
    //Assigne l'élément si l'élément est dans la vue.
    radioPaiement = document.getElementById("rechercherPaiement");
}

//Section pour quand le vue refresh d'une recherche de dates, pour laisser apparaître les datepickers lors d'un refresh.
if (radioDate.checked == true || radioPaiement.checked == true) {
    $("#rechercherTextRechercher").addClass("invisible");
    $("#champDate").removeClass("invisible");
}

//Fonction principal qui rend les invisible (le textbox).
function invisibilite() {
    //Section pour quand le vue refresh d'une recherche de dates, pour laisser apparaître les datepickers lors d'un refresh.
    if (radioDate.checked == true || radioPaiement.checked == true) {
        $("#rechercherTextRechercher").addClass("invisible");
        $("#champDate").removeClass("invisible");
    }
}

//Fonction inverse d'invisibilité.
function inverse() {
    $("#rechercherTextRechercher").removeClass("invisible");
    $("#champDate").addClass("invisible");
}

//Je me fais un tableau sans le radiobutton date.
let radioButtonRechercherPar = document.getElementsByName("rechercherPar");
let listeRadioButtonSansDate = [];

let iterateur;
for (iterateur = 0; iterateur < radioButtonRechercherPar.length; iterateur++) {

    //Si je suis date ou plagehoraire je ne le veux pas dans mon tableau donc je ne fais rien.
    if (radioButtonRechercherPar[iterateur].value == "DatePaiement" || radioButtonRechercherPar[iterateur].value == "PlageHoraire") {

    } else {
        //Ajoute les autre boutons qui n'est pas date ou plagehoraire.
        listeRadioButtonSansDate.push(radioButtonRechercherPar[iterateur]);
    }
}

let iterateur2
for (iterateur2 = 0; iterateur2 < listeRadioButtonSansDate.length; iterateur2++) {

    listeRadioButtonSansDate[iterateur2].addEventListener("click", inverse);
}
