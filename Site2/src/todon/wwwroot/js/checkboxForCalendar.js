

        $(document).ready(function () {
            console.log("ready!");

            $(":checkbox").change(function (event, ui) {
                // alert('1');
                event.preventDefault();
                var field = $(this);
                var sd = field.val();
                if (this.checked) {
                   // alert('2');
                    // document.getElementById('TaskCalendar').style.backgroundColor = "blue";
                      $(this).parent().children("a").first().css("text-decoration", "line-through");
                     
                      location.href = field.data("target");

                } else {
                    $(this).parent().children("a").first().css("text-decoration", "none");
                   // document.getElementById('TaskCalendar').style.backgroundColor = "darkgrey";
                   //  alert('3');
                    location.href = field.data("target");
                }
                // alert("3");
            })

        });
   
