$(function() {
    $('.dp').datetimepicker({
        controlType: 'select',
        oneLine: true,
        timeFormat: 'HH:mm',
        prevText: "Précédent",
        nextText: "Suivant",
        timeText: "Temps",
        currentText: "Aujourd'hui",
        closeText: "Fermer",
        dayNamesMin: ["Dim", "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam"],
        monthNames: ["Janvier", "Février", "Mars", "Avril", "Mai", "Juin",
            "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre"]
    });
});



//Fonction supprimer plageHoraire
function SupprimerDP(id) {
    let leBonId = id - 1;
    let bonIdSupprimer = leBonId - 1;

    document.getElementById("plageHoraire" + leBonId).remove();
    document.getElementById("btnSupprimer" + bonIdSupprimer).classList.remove("invisible");
    document.getElementById("plageHoraire" + bonIdSupprimer).classList.add("paddingLeft");

}

//Fonction supprimer plageHoraire s'il y a une erreur ou une modification

function SupprimerDPModif(id) {
    let leBonId = id
    let bonIdSupprimer = leBonId - 1;

    document.getElementById("plageHoraire" + leBonId).remove();
    document.getElementById("btnSupprimer" + bonIdSupprimer).classList.remove("invisible");
    document.getElementById("plageHoraire" + bonIdSupprimer).classList.add("paddingLeft");
}

//Permet d'ajouter une plage horaire
function AddDP() {
    var leCompteur = document.getElementsByClassName("dp").length;
    var leCompteurDiv = document.getElementsByClassName("plageHoraireNombre").length;
    var inputDebut = document.querySelector("#ListeDateTimes0");
    this.$btnSupprimerX = $('<a></a>')

    var cloneInput = $(inputDebut).clone(true);
    var cloneInput2 = $(inputDebut).clone(true);

    var textDebut = $(document.querySelector("#textDebut")).clone(false);
    var textFin = $(document.querySelector("#textFin")).clone(false);

    var divSuivant = document.createElement("div");
    var divSuivant2 = document.createElement("div");

    var conteneurDeTout = document.createElement("div");

    var conteneurDuInput = document.createElement("div");
    var conteneurDuInput2 = document.createElement("div");

    cloneInput.attr("id", "ListeDateTimes" + leCompteur + "");
    cloneInput.attr("name", "ListeDateTimes[" + leCompteur + "]");

    divSuivant.id = "divTime" + leCompteur + "";
    divSuivant.classList.add("conteneurDivInput");
    divSuivant.classList.add("m-5");
    divSuivant.classList.add("marginRight");

    if (document.getElementsByClassName("plageHoraireNombre")[0].id == "plageHoraire0") {
        document.querySelector("#plageHoraire0").before(conteneurDeTout);
    } else {
        document.querySelector("#plageHoraire" + (leCompteurDiv - 1)).before(conteneurDeTout);
    }

    conteneurDeTout.id = "plageHoraire" + leCompteurDiv + "";
    conteneurDeTout.classList.add("row");
    conteneurDeTout.classList.add("plageHoraireNombre");
    conteneurDeTout.classList.add("col-12");
    conteneurDeTout.classList.add("plageHoraireConteneurTout");
    conteneurDeTout.classList.add("justify-content-center");
    conteneurDeTout.classList.add("moitieHauteur");
    conteneurDeTout.classList.add("paddingLeft");


    conteneurDuInput.id = "ConteneurInput0" + leCompteur;
    conteneurDuInput.classList.add("col-10");

    $btnSupprimerX.attr("class", "btnSupprimer ml-5");
    $btnSupprimerX.attr("id", "btnSupprimer" + leCompteurDiv);
    $btnSupprimerX.html("X");

    if (document.body.contains(document.getElementById("btnSupprimer" + (leCompteurDiv - 1)))) {
        document.getElementById("btnSupprimer" + (leCompteurDiv - 1)).classList.add("invisible");
    }

    $btnSupprimerX.on("click", function () {
        SupprimerDP(leCompteurDiv);
    });

    leCompteur++;
    leCompteurDiv++;

    cloneInput2.attr("id", "ListeDateTimes" + leCompteur + "");
    cloneInput2.attr("name", "ListeDateTimes[" + leCompteur + "]");

    divSuivant2.id = "divTime" + leCompteur + "";
    divSuivant2.classList.add("conteneurDivInput");
    divSuivant2.classList.add("marginSaufGauche");


    conteneurDuInput2.id = "ConteneurInput0" + leCompteur;
    conteneurDuInput2.classList.add("col-12");

    cloneInput
        .removeClass('hasDatepicker')
        .removeData('datetimepicker')
        .unbind()
        .datetimepicker({
            controlType: 'select',
            oneLine: true,
            timeFormat: 'HH:mm',
            prevText: "Précédent",
            nextText: "Suivant",
            timeText: "Temps",
            currentText: "Aujourd'hui",
            closeText: "Fermer",
            dayNamesMin: ["Dim", "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam"],
            monthNames: ["Janvier", "Février", "Mars", "Avril", "Mai", "Juin",
                "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre"]
        });

    cloneInput2
        .removeClass('hasDatepicker')
        .removeData('datetimepicker')
        .unbind()
        .datetimepicker({
            controlType: 'select',
            oneLine: true,
            timeFormat: 'HH:mm',
            prevText: "Précédent",
            nextText: "Suivant",
            timeText: "Temps",
            currentText: "Aujourd'hui",
            closeText: "Fermer",
            dayNamesMin: ["Dim", "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam"],
            monthNames: ["Janvier", "Février", "Mars", "Avril", "Mai", "Juin",
                "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre"]
        });

    cloneInput.val("");
    cloneInput2.val("");

    $(divSuivant).appendTo(conteneurDeTout);
    $(divSuivant2).appendTo(conteneurDeTout);

    $(conteneurDuInput).appendTo(divSuivant);
    $(conteneurDuInput2).appendTo(divSuivant2);

    $(cloneInput).appendTo(conteneurDuInput);
    $(cloneInput2).appendTo(conteneurDuInput2);
    $btnSupprimerX.insertAfter(cloneInput2);

    $(textDebut).insertBefore(conteneurDuInput);
    $(textFin).insertBefore(conteneurDuInput2);

    if ((leCompteurDiv - 1) >= 2) {
        document.getElementById("plageHoraire" + (leCompteurDiv - 2)).classList.remove("paddingLeft");
    }
}
