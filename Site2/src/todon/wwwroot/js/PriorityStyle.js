window.addEventListener("load", init, false);

function init() {

    var div1 = document.getElementsByTagName("div");
   // alert(div1.innerHTML);

   // alert(div1.length);
    for (var i = 0; i < div1.length; i++) {
        var div = div1[i];
        //alert(div);
        if (div.innerHTML == '5') {
           // div.innerText = "___";
            div.style.backgroundColor = "#FB142B";
            div.style.color = "#FB142B";
        }
        else
            if(div.innerHTML == '4'){
                div.style.backgroundColor = "#FB6031";
                div.style.color = "#FB6031";
            }
            else
                if (div.innerHTML == '3') {
                    div.style.backgroundColor = "#6C7FFB";
                    div.style.color = "#6C7FFB";
                }
                else
                    if (div.innerHTML == '2') {
                        div.style.backgroundColor = "#FB8092";
                        div.style.color = "#FB8092";
                    }
                    else
                        if (div.innerHTML == '1') {
                            div.style.backgroundColor = "#C6FB19";
                            div.style.color = "#C6FB19";
                        }
    }
}