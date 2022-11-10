"uses strict";

window.onload = function () {
    var intro = document.getElementsByClassName("intro");

    for (var i = 0; i < intro.length; i++) {
        let result = "";
        console.log("intro: " + intro.item(i).innerHTML);
        var wordArray = intro.item(i).innerHTML.split(' ');
        if (wordArray.length > 12) {
            for (var j = 0; j < 12; j++) {
                result += wordArray[j] + " ";
            }
            result += "...";
        }
        if (result != "") {
            document.getElementsByClassName("intro").item(i).innerHTML = result;
        }
    }
}