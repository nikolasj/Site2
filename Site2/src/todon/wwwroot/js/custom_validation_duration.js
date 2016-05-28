function toDuration(intDuration) {

    if (intDuration <= 0) return '';
    var d = Math.floor(intDuration / (60 * 24));
    var h = Math.floor((intDuration / 60) % 24);
    var m = intDuration - d * 24 * 60 - h * 60;
    result = '';
    if (d > 0) result = result + d + 'd ';
    if (h > 0) result = result + h + 'h ';
    if (m > 0) result = result + m + 'm ';

    return result.substring(0, result.length - 1);
}

function parseDuration(strDuration) {
    if (strDuration == null || strDuration == '') return 0;
    mrx = new RegExp(/([0-9]{1,4}?)[ ]?m/);
    hrx = new RegExp(/([0-9]{1,2}?)[ ]?h/);
    drx = new RegExp(/([0-9]{1,4}?)[ ]?d/);
    days = 0;
    hours = 0;
    mins = 0;
    //alert(strDuration);

    if (mrx.test(strDuration))
        mins = mrx.exec(strDuration)[1];
    if (hrx.test(strDuration))
        hours = hrx.exec(strDuration)[1];
    if (drx.test(strDuration)) {
        // alert('555');
        days = drx.exec(strDuration)[1];
        // alert('556');
    }
    return toMins(days, hours, mins);
}

function toMins(days, hours, mins) {
    d = parseInt(days);
    h = parseInt(hours);
    m = parseInt(mins);
    if (isNaN(d)) d = 0;
    if (isNaN(h)) h = 0;
    if (isNaN(m)) m = 0;
    t = d * 24 * 60 + h * 60 + m;
    // alert(t);
    return t;
}


function checkDateVal(sender)
{
    var field = $(sender);
    var sd = (field.val());
   //  alert(sd);
    minutes = parseDuration(sd);
   // alert(minutes);
    var fieldstart = $("#TaskDataStart");
    var sdstart = fieldstart.val();
    var datestart = new Date(sdstart);
    taskStart = datestart;
   // alert(taskStart);

    var fieldend = $("#TaskDataEnd");
    var sdend = fieldend.val();
    var dateEnd = new Date(sdend);
    taskEnd = dateEnd;
    //alert(taskEnd);
    result = (taskEnd - taskStart) / 86400000;
    //alert(result);

    if (parseInt(result) != result)
        var isInt = false;
    else
        var isInt = true;
    if (result > 0 && isInt) {
        result = result * 1440;
       // alert(result);
    }
    else
        if (result > 0) {
            //  alert('3');
            result = (taskEnd - taskStart) / 60000;
           // alert(result);
        }

    if (sd != '' && minutes == 0 || result != minutes) {
        field.css("background-color", "red");
        //  alert('no');
        return false;
    }
    else {
        field.val(toDuration(minutes));
        field.css("background-color", "green");
        //   alert('yes');
        return true;
    }
}

jQuery(document).ready(function ($) {
    console.log("ready d!");

    $("#TaskDataStart").change(function (event, ui) {
       // alert('TaskDataStartChange');

        event.preventDefault();
        var field = $(this);
        var sd = field.val();
        var date = new Date(sd);
        taskStart = date;
       // alert('1');
    });

    $("#TaskDataEnd").change(function (event, ui) {
        event.preventDefault();
        var fieldstart = $("#TaskDataStart");
        var sdstart = fieldstart.val();
        var datestart = new Date(sdstart);
        taskStart = datestart;
       // alert('TaskDataEndChange');
        var field = $(this);
        var sd = field.val();
        var date = new Date(sd);
        taskEnd = date;
        //alert(taskEnd);
        //alert(taskStart);
        result = (taskEnd - taskStart) / 86400000;
         // alert(result);
        if (parseInt(result) != result)
            var isInt = false;
        else
            var isInt = true;
        if (result > 0 && isInt) {
            //  alert('3');
            document.getElementById('TaskDuration').value = result + 'd';
        }
        else
            if (result > 0) {
                //  alert('3');
                result = (taskEnd - taskStart) / 60000;
                document.getElementById('TaskDuration').value = result + 'm';
            }

        //--------------------------------------------

        return checkDateVal(document.getElementById('TaskDuration'));

    });

    $("#TaskDuration").change(function (event, ui) {
        // alert('TaskDurationChange');
        event.preventDefault();
        var field = $(this);
        return checkDateVal(this);

    });

    //--------------------------------
});