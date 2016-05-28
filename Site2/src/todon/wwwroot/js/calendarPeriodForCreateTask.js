function visibilityChange()
{
    var field = $("#TaskType");
    var sd = field.val();
    //var val = document.getElementById('T').val;
   // alert(sd);
   // alert(val);
    if (sd == 1) {
        //document.getElementById('TaskPeriodDiv').style.display = "none";
        $("#TaskPeriodDiv").parent().parent().hide();
        $("#TaskDataStart").parent().parent().show();
        $("#TaskDataEnd").parent().parent().show();
        $("#TaskPeriod").parent().parent().hide();
        $("#periodLabel").parent().parent().hide();
        $("#dateStartLabel").parent().parent().show();
        $("#dateEndLabel").parent().parent().show();
        //document.getElementById('TaskDataStart').style.display = "";
        //document.getElementById('TaskDataEnd').style.display = "";
        //document.getElementById('TaskPeriod').style.display = "none";
        //document.getElementById('periodLabel').style.display = "none";
        //document.getElementById('dateStartLabel').style.display = "";
        //document.getElementById('dateEndLabel').style.display = "";


    }
    if (sd == 2) {
        // alert('3');
        // document.getElementById('TaskPeriod').value = "0 0 * * 0";
        //document.getElementById('TaskPeriodDiv').style.display = "";
        //document.getElementById('TaskPeriodDiv').value = "";
        //// document.getElementById('TaskPeriod').style.display = document.getElementById('TaskPeriod').value;
        ////document.getElementById('TaskPeriodDiv').style.display = document.getElementById('TaskPeriodDiv').value;
        //document.getElementById('TaskDataStart').style.display = "none";
        //document.getElementById('TaskDataEnd').style.display = "none";
        //document.getElementById('dateStartLabel').style.display = "none";
        //document.getElementById('dateEndLabel').style.display = "none";
        //document.getElementById('periodLabel').style.display = "";
        $("#TaskPeriodDiv").parent().parent().show();
     
        $("#TaskDataStart").parent().parent().hide();
        $("#TaskDataEnd").parent().parent().hide();

        $("#periodLabel").parent().parent().show();
        $("#dateStartLabel").parent().parent().hide();
        $("#dateEndLabel").parent().parent().hide();
    }
}

$(document).ready(function () {
    console.log("TaskType!");
   

    $("#TaskType").change(function (event, ui) {
        //alert('111');
        event.preventDefault();
        visibilityChange();
    });

    visibilityChange();
});