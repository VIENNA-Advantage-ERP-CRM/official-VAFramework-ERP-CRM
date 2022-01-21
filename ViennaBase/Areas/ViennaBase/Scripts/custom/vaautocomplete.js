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
            onSelect: function (e, item) { },
            html:""
        }, options);


        var ctrl = this[0];
        var currentFocus, arr = [], setTime;
        var self, isSearch = false;
        var a, isAdd = false;
        var response = function (data) {
            suggestion(data);
        };

        var suggestion = function (arr) {
            isSearch = true;
            isAdd = false;
            var b, i, val = self.value;
            $("#" + self.name + "vis-autocomplete-list").remove();
            /*create a DIV element that will contain the items (values):*/
            a = document.createElement("DIV");
            $('#' + self.name + "vis-autocomplete-list").remove();
            a.setAttribute("id", self.name + "vis-autocomplete-list");
            a.setAttribute("class", "vis-autocomplete-items");
            /*append the DIV element as a child of the autocomplete container:*/
            $('body').append(a);
            /*for each item in the array...*/
            for (i = 0; i < arr.length; i++) {
                /*check if the item starts with the same letters as the text field value:*/
                var idx = arr[i].value.toUpperCase().indexOf(val.toUpperCase());
                if (idx != -1) {
                    /*create a DIV element for each matching element:*/
                    b = document.createElement("DIV");
                    b.setAttribute("class", "vis-autocompleteList-item");
                    /*make the matching letters bold:var idx*/
                    var boldVal = arr[i].value.substr(idx, val.length)
                    b.innerHTML = arr[i].value.replace(boldVal, "<strong>" + boldVal + "</strong>");
                    //b.innerHTML += arr[i].value.substr(val.length);                                      
                } else if (arr[i].id == 'vis-AddNew') {
                    isAdd = true;
                    b = document.createElement("DIV");
                    b.innerHTML += "<center>" + arr[i].msg + "</center><center><div style='display:inline-block;padding:5px 10px; margin: 5px;background-color:rgba(var(--v-c-secondary), 1);border-radius: 0.25rem;'>" + arr[i].value + "</div></center>";
                }

                if (b) {
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
                        //if (obj.id == 'vis-AddNew') {
                        isSearch = false;
                        //}
                        settings.onSelect(e, obj);
                        /*close the list of autocompleted values,
                        (or any other open lists of autocompleted values:*/
                        closeAllLists();
                    });
                    a.appendChild(b);
                }
            }

            if (settings.html != "" && !b) {
                $(a).append($('<div></div>')).append(settings.html);
                $(a).find(".vis-autocomplete-active").removeClass("vis-autocomplete-active");
            }
        /*calculate list postion*/

            var slf = $(self);
            var ancr = $(a);
            var width = slf.outerWidth();
            var Height = slf.outerHeight();
            var offset = slf.offset();
            var listHeight = ancr.height();
            var listWidth = ancr.width();
            var windowHeight = $(window).height();
            var windowWidth = $(window).width();
            var top = offset.top;
            var left = offset.left;
            var bottom = windowHeight - (top + Height + 10);
            var right = windowWidth - left - listWidth;
            var topbottom = (listHeight <= bottom) ? bottom : top;
            var leftright = ((listWidth - width) <= right) ? right : left;
            var yPos = top + Height;
            var xPos = left;
            if (topbottom == top) {
                yPos = top - listHeight;
            }
            if (leftright == left) {
                xPos = left - (listWidth - width);
            }
            ancr.attr("style", "left:" + xPos + "px; top:" + yPos + "px;min-width:" + width + "px;max-width:" + (width + 200) + "px");
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
                isSearch = false;
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
            if (x) x = x.getElementsByClassName("vis-autocompleteList-item");
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
            if (isAdd && currentFocus == 0) {
                currentFocus++;
            }
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
        ctrl.addEventListener("blur", function (e) {
            setTimeout(function () {
                if (isSearch) {
                    ctrl.value = '';
                }
                if (a) {
                    a.remove();
                }
            }, 400);

            // closeAllLists(e.target);
        });
        // Greenify the collection based on the settings variable.
    };

}(jQuery));