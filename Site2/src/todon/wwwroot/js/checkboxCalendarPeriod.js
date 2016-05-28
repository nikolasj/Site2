
$(document).ready(function () {
    console.log("ready!");

    $("#TaskType").change(function (event, ui) {
        // alert('1');
        event.preventDefault();
        var field = $(this);
        var sd = field.val();

        //  alert(sd);
        if (sd == 1) {
            document.getElementById('TaskPeriodDiv').style.display = "none";
            document.getElementById('TaskDataStart').style.display = document.getElementById('TaskDataStart').value;
            document.getElementById('TaskDataEnd').style.display = document.getElementById('TaskDataEnd').value;
            document.getElementById('TaskPeriod').style.display = "none";

        }
        if (sd == 2) {

            document.getElementById('TaskPeriod').value = "0 0 * * 0";
            document.getElementById('TaskPeriodDiv').style.display = "";
            document.getElementById('TaskPeriod').style.display = document.getElementById('TaskPeriod').value;
            document.getElementById('TaskPeriodDiv').style.display = document.getElementById('TaskPeriodDiv').value;
            document.getElementById('TaskDataStart').style.display = "none";
            document.getElementById('TaskDataEnd').style.display = "none";

        }

    })

});


//$(document).ready(function () {
//    console.log("ready!");

//    $(":checkbox").change(function (event, ui) {
//        // alert('1');
//        event.preventDefault();
//        var field = $(this);
//        var sd = field.val();
//        if (this.checked) {
//            document.getElementById('TaskPeriodDiv').style.display = "";
//            document.getElementById('TaskPeriod').value = document.getElementById('TaskPeriod').value;
//            //document.getElementById('TaskDataStart').style.display = "";
//            //document.getElementById('TaskDataEnd').style.display = "";
//            //document.getElementById('TaskDuration').style.display = "";
//        } else {
//            document.getElementById('TaskPeriodDiv').style.display = "none";
//            document.getElementById('TaskPeriod').value = "";
//            // alert('1');
//            //document.getElementById('TaskDataStart').style.display = "none";
//            //document.getElementById('TaskDataEnd').style.display = "none";
//            //document.getElementById('TaskDuration').style.display = "none";
//        }
//        // alert("3");
//    })

//});


//$(this).parent().children("a").first().css("text-decoration", "none");
// document.getElementById('TaskCalendar').style.backgroundColor = "darkgrey";
// alert("2");
//S location.href = field.data("target");

// document.getElementById('TaskCalendar').style.backgroundColor = "blue";
//$(this).parent().children("a").first().css("text-decoration", "line-through");
//alert("Вы отметили задачу как сделанную!!!");
// location.href = field.data("target");