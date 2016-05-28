function movetodate(dateVal, changeFormat) {
    //  alert(dateVal);
    if (!changeFormat)
        location.href = '/TasksCalendar/TaskCalendarWeek/?taskDate=' + dateVal;
    else
        location.href = '/TasksCalendar/TaskCalendarByDateChangeFormat/?taskDate=' + dateVal;
}