var wasMidnight = false;
var today = formatDate(new Date());
var yesterday;

window.onload = function () {
    timer = window.setTimeout(checks, 100);
    setInterval(checks, 30000);

}

function checks()
{
    checkRedirect();
    checkHour();
}
function formatDate(date)
  {
    var dd = date.getDate();
    var mm = date.getMonth() + 1; //January is 0!
    var yyyy = date.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }
    return dd + '.' + mm + '.' + yyyy;
}
function checkRedirect()
{
    var Newtoday = new Date();
    var Newyesterday = new Date(new Date().setDate(new Date().getDate() - 1));

    var oldtoday = today;

    today = formatDate(Newtoday);
    yesterday = formatDate(Newyesterday);

    if (oldtoday != today)
    {
        wasMidnight = true;
    }
   // alert(today);
  //  alert(yesterday);
    //alert(wasMidnight);
    var path = location.href;
    if (location.href.lastIndexOf(yesterday) > 0 && wasMidnight)
        location.href = '/Task/TaskCalendarByDate/?taskDate=' + today;
   
}
function checkHour() {
    var hour = new Date().getHours();
    var time = hour + ':00';
    var div = document.getElementsByTagName('a');
    //alert('1');
   
    for (var i = 0; i < div.length;i++ )
    {
        
        var currentDiv = div[i];
        if (currentDiv.innerHTML.search(/[^0-9]/)) {
            if (time.toString() == currentDiv.innerHTML.toString()) {
                currentDiv.parentElement.style.backgroundColor = "#D3DCE0";
               // alert('2');
                currentDiv.style.backgroundColor = "#E0695C";
                break;
            }
            else {
                currentDiv.style.backgroundColor = "#D3DCE0";
                //alert('1');
            }
        }
    }
    //alert('3');
}

//var date = new Date().getMinutes();
////alert(date);
//var timeForDay = hour + ':' + date;
//var dateVal = document.getElementById('TimeNow');
////alert(dateVal);
//if (timeForDay == "23:59") {
//    location.href = '/Task/TaskCalendarByDate/?taskDate=' + dateVal;
//}

//var minutes = new Date().getUTCMinutes();
//if (minutes < 10) {
//    var time = hour + ':0' + minutes;
//}
//else