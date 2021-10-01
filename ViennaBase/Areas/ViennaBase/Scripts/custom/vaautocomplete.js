/** 
 *  Purpose: autocomplete
 * Mandeep Singh( VIS0228) 13-Sep-2021
 */  
(function ($) {
    $.fn.vaautocomplete = function (options) {
        // This is the easiest way to have default options.
        var settings = $.extend({
            delay: 300,
            minLength: 1,
            source: [],
            response: null,
            onSelect: function (e, item) { }
        }, options);


        var ctrl = this[0];
        var currentFocus, arr = [], setTime;
        var self;
        var response = function (data) {
            suggestion(data);
        };

        var suggestion = function (arr) {



            var a, b, i, val = self.value;
            /*create a DIV element that will contain the items (values):*/
            a = document.createElement("DIV");
            a.setAttribute("id", self.name + "vis-autocomplete-list");
            a.setAttribute("class", "vis-autocomplete-items");
            /*append the DIV element as a child of the autocomplete container:*/
            $('body').append(a);
            var width = $(self).outerWidth();
            var Height = $(self).outerHeight();
            var offset = $(self).offset();
            $(a).attr("style", "left:" + offset.left + "px; top:" + (offset.top + Height) + "px;min-width:" + width + "px;max-width:" + (width + 200) + "px");
            /*for each item in the array...*/
            for (i = 0; i < arr.length; i++) {
                /*check if the item starts with the same letters as the text field value:*/
                var idx = arr[i].value.toUpperCase().indexOf(val.toUpperCase());
                if (idx != -1) {
                    /*create a DIV element for each matching element:*/
                    b = document.createElement("DIV");
                    /*make the matching letters bold:var idx*/
                    var boldVal = arr[i].value.substr(idx, val.length)
                    b.innerHTML = arr[i].value.replace(boldVal, "<strong>" + boldVal + "</strong>");
                    //b.innerHTML += arr[i].value.substr(val.length);
                    /*insert a input field that will hold the current array item's value:*/
                    b.innerHTML += "<input type='hidden' data-id='" + arr[i].id + "'  value='" + arr[i].value + "'>";
                    /*execute a function when someone clicks on the item value (DIV element):*/
                    b.addEventListener("click", function (e) {
                        /*insert the value for the autocomplete text field:*/
                        ctrl.value = this.getElementsByTagName("input")[0].value;
                        var obj = {
                            id: this.getElementsByTagName("input")[0].getAttribute('data-id'),
                            text: this.getElementsByTagName("input")[0].value
                        }
                        settings.onSelect(e, obj);
                        /*close the list of autocompleted values,
                        (or any other open lists of autocompleted values:*/
                        closeAllLists();
                    });
                    a.appendChild(b);
                }
            }
        }

        ctrl.addEventListener("focus", function (e) {
            ctrl.autocomplete = "off";
        });

        ctrl.addEventListener("input", function (e) {
            clearTimeout(setTime);
            self = this;
            /*close any already open lists of autocompleted values*/
            closeAllLists();
            if (!self.value) {
                return false;
            }
            if (ctrl.value.length < settings.minLength) {
                return false;
            }
            currentFocus = -1;
            setTime = setTimeout(function () {

                if ($.isArray(settings.source)) {
                    suggestion(settings.source);
                } else {
                    settings.source(self.value, response);
                }
            }, settings.delay);
        });
        /*execute a function presses a key on the keyboard:*/
        ctrl.addEventListener("keydown", function (e) {
            var x = document.getElementById(this.name + "vis-autocomplete-list");
            if (x) x = x.getElementsByTagName("div");
            if (e.keyCode == 40) {
                /*If the arrow DOWN key is pressed,
                increase the currentFocus variable:*/
                currentFocus++;
                /*and and make the current item more visible:*/
                addActive(x);
            } else if (e.keyCode == 38) { //up
                /*If the arrow UP key is pressed,
                decrease the currentFocus variable:*/
                currentFocus--;
                /*and and make the current item more visible:*/
                addActive(x);
            } else if (e.keyCode == 13) {
                /*If the ENTER key is pressed, prevent the form from being submitted,*/
                e.preventDefault();
                if (currentFocus > -1) {
                    /*and simulate a click on the "active" item:*/
                    if (x) x[currentFocus].click();
                }
            }
        });

        function addActive(x) {
            /*a function to classify an item as "active":*/
            if (!x) return false;
            /*start by removing the "active" class on all items:*/
            removeActive(x);
            if (currentFocus >= x.length) currentFocus = 0;
            if (currentFocus < 0) currentFocus = (x.length - 1);
            /*add class "autocomplete-active":*/
            x[currentFocus].classList.add("vis-autocomplete-active");
        }

        function removeActive(x) {
            /*a function to remove the "active" class from all autocomplete items:*/
            for (var i = 0; i < x.length; i++) {
                x[i].classList.remove("vis-autocomplete-active");
            }
        }

        function closeAllLists(elmnt) {
            /*close all autocomplete lists in the document,
            except the one passed as an argument:*/
            var x = document.getElementsByClassName("vis-autocomplete-items");
            for (var i = 0; i < x.length; i++) {
                if (elmnt != x[i] && elmnt != ctrl) {
                    x[i].parentNode.removeChild(x[i]);
                }

            }
        }
        /*execute a function when someone clicks in the document:*/
        document.addEventListener("click", function (e) {
            closeAllLists(e.target);
        });
        // Greenify the collection based on the settings variable.
    };

}(jQuery));