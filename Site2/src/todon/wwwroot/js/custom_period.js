jQuery(document).ready(function ($) {
    var initValue = $('#TaskPeriod').val();
   // alert('1');
    var c = $('#TaskPeriodDiv').cron({
        initial: initValue,
        effectOpts: {
            openEffect: "fade",
            openSpeed: "slow"
        },
        onChange: function () {
             //alert($(this).cron("value"));
            $('#TaskPeriod').val($(this).cron("value"));
           // alert($('#TaskPeriod').val());
        }
    });
});