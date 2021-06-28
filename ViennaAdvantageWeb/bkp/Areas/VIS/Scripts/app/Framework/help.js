; (function (VIS, $) {
    //VIS.Apps = VIS.Apps || {};
    function help(gridWindow) {

        //this.frame;
        //this.windowNo; 

        var $root;
        var htmlText = "";
        function initializeComponent() {
            $root = $('<div></div>');
            //var helpText = "";
            var helpText = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>";
            if (VIS.Application.isRTL)
            {
                helpText += "<html dir='rtl' xmlns='http://www.w3.org/1999/xhtml'>";
            }
            else
            {
                helpText += "<html xmlns='http://www.w3.org/1999/xhtml'>";
            }

            helpText += "<head>" +
            "<script type='text/javascript' language='javascript'> " +
            "document.onkeydown = function(){ " +
            "if((window.event && window.event .keyCode == 116)||(window.event && window.event .keyCode == 78)) " +
            "{ " +
            "window.event.keyCode = 505; " +
            "}" +
            "if(window.event && window.event .keyCode == 505)" +
            "{ " +
            "return false; " +
            "} }" +
             //"  function anchorJump(anchor) {" +
             //   "  "+
             //     " var targAnchor = null,"+
             //      " anchorID = anchor.replace(/.*#([^\?]+).*/, '$1'); "+

             //     " if (!(targAnchor = document.getElementsByName(anchorID))) " +
             //       "   for (var i = 0, found = false, da = document.anchors, len = da.length; i < len && !targAnchor; i++) "+
             //          "    targAnchor = (da[i].name == anchorID ? da[i] : null); "+

             //      " if (targAnchor) { "+
             //         " disp = getElemOffset(targAnchor); "+
             //         " window.scrollTo(disp.x, disp.y); "+
             //     " } "+
             //     " else "+
             //       "   alert('Did not find anchor/element ' + anchorID + '');"+

             //     " function getElemOffset(elem) { "+
             //         " var left = !!elem.offsetLeft ? elem.offsetLeft : 0; "+
             //         " var top = !!elem.offsetTop ? elem.offsetTop : 0; "+

             //        //" while ((elem = elem.offsetParent)) { "+
             //        //     " left += !!elem.offsetLeft ? elem.offsetLeft : 0; "+
             //        //     " top += !!elem.offsetTop ? elem.offsetTop : 0; "+
             //        //  "}"+

             //          "return { x: left, y: top };"+
             //      "}" +
             //         "return false;"+
             //  "}"+

            "</script> " +
            "<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />" +
            "<title>Help &amp; Support " + gridWindow.vo.Name + "</title>" +
            "<style type='text/css'>" +
            "<!--" +
            "body {" +
                "margin-left: 0px;" +
                "margin-top: 0px;" +
                "margin-right: 0px;" +
                "margin-bottom: 0px;" +
                "font-family:verdana;" +
                "font-size:12px;" +
                "color:#000000;" +
            "}" +
            ".hed1" +
            "{" +
                "font-family:verdana;" +
                "font-weight:bold;" +
                "font-size:12px;" +
                "color:#000000;" +
            "}" +
            ".border" +
            "{" +
                "border:1px solid #c6daf3;" +
            "}" +
            ".border-1" +
            "{" +
                "border:1px solid #aac7e7;" +
                    "background-color:#e9e8e8;" +
            "}" +
            ".border-2" +
            "{" +
                "border:1px solid #aac7e7;" +
            "}" +
            ".border-bottom" +
            "{" +
                "border-bottom:1px solid #c6daf3;" +
            "}" +
            ".border-top-none" +
            "{" +
                "border-bottom:1px solid #c6daf3;" +
                "border-right:1px solid #c6daf3;" +
                "border-left:1px solid #c6daf3;" +
            "}" +
            ".red" +
            "{" +
                "font-size:12px;" +
                "color:#FF0000;" +
                "font-weight:bold;" +
            "}" +
            ".white" +
            "{" +
                "font-size:16px;" +
                "color:#FFffff;" +
                "font-weight:bold;" +
            "}" +
            ".black" +
            "{" +
                "font-size:16px;" +
                "color:#000000;" +
                "font-weight:bold;" +
            "}" +
            ".blue" +
            "{" +
                "color:#5490fa;" +
                "font-weight:bold;" +
            "}" +
            ".orange" +
            "{" +
                "color:#f2b227;" +
                "font-weight:bold;" +
            "}" +
            ".pad" +
            "{" +
                "padding:5px;" +
            "}" +
            "a" +
            "{" +
                "color:#5490fa;	" +
            "}" +
            "a:hover" +
            "{" +
                "color:#000000;	" +
                "text-decoration:none;" +
            "}" +
            "-->" +
            "</style></head>" +

            "<body oncontextmenu='return false;'>";


            helpText = helpText + "<table width='100%' border='0' align='center' cellpadding='0' cellspacing='0' class='border'>" +
              "<tr>" +
                "<td><table width='100%' border='0' align='center' cellpadding='0' cellspacing='0' bgcolor='#c6daf3' class='border-bottom'>" +
                  "<tr>" +
                //"<td width='5%'><img src='logo.gif' width='42' height='29' /></td>" +
                    "<td width='100%' class='hed1' height='29'>Vienna Solutions</td>" +
                  "</tr>" +

                "</table></td>" +
              "</tr>" +
              "<tr>" +
                "<td>&nbsp;</td>";
            helpText = helpText + "</tr>" +
            "<tr>" +

              "<td><table width='98%' border='0' align='center' cellpadding='0' cellspacing='0' class='border'>" +
                "<tr>" +
                  "<td><table width='99%' border='0' align='center' cellpadding='0' cellspacing='0'>" +
                    "<tr>" +
                      "<td class='red'>&nbsp;</td>" +
                    "</tr>" +
                    "<tr>" +
                //get window data
                      "<td class='red'>Window: " + gridWindow.vo.Name + "</td>" +
                    "</tr>" +
                    "<tr>" +
                      "<td>&nbsp;</td>" +
                    "</tr>" +
                    "<tr>" +
                      "<td>" + gridWindow.vo.Description + "</td>" +
                    "</tr>" +
                    "<tr>" +
                      "<td>&nbsp;</td>" +
                    "</tr>" +
                    "<tr>" +
                      "<td>" + gridWindow.vo.Help + "</td>" +
                    "</tr>" +
                //........................
                    "<tr>" +
                      "<td>&nbsp;</td>" +
                    "</tr>" +
                    "<tr>" +
                      "<td>";
            //get all tabs of a window
            var strTab = "";
            for (var i = 0; i < gridWindow.tabs.length; i++) {
                //var tab = gridWindow.getTab(i);
                if (strTab == "") {
                    strTab = "<a href='#" + gridWindow.tabs[i].vo.Name + "" + i + "'>" + gridWindow.tabs[i].vo.Name + "</a>";
                }
                else {
                    strTab += " -&gt; <a href='#" + gridWindow.tabs[i].vo.Name + "" + i + "'>" + gridWindow.tabs[i].vo.Name + "</a> ";
                }
            }
            //..............................
            //<a href='#'>Table</a> -&gt; <a href='#'>Colomn</a> -&gt; <a href='#'>Selection Sequence</a> -&gt; <a href='#'>Summary Sequence</a>+
            helpText = helpText + strTab;
            helpText = helpText + "</td>" +
          "</tr>" +
          "<tr>" +
            "<td>&nbsp;</td>" +
          "</tr>" +
        "</table></td>" +
      "</tr>" +
    "</table></td>" +
  "</tr>" +
  "<tr>" +
    "<td>&nbsp;</td>" +
  "</tr>";
            helpText = helpText + "<tr>" +
   "<td><table width='98%' border='0' align='center' cellpadding='0' cellspacing='0' class='border'>";
            //fo each tab
            for (var i = 0; i < gridWindow.tabs.length; i++) {
                // var tab = gridWindow.getTab(i);
                helpText = helpText + "<tr>" +
                   "<td><table width='99%' border='0' align='center' cellpadding='0' cellspacing='0'>" +
                     "<tr>" +
                       "<td class='black' style='padding-top:5px;' ><table width='100%' border='0' align='center' cellpadding='0' cellspacing='0'>" +
                        "<tr>" +
                            "<td width='340' height='5' bgcolor='#f9b31c'></td>" +
                            "<td width='4'></td>" +
                            "<td width='624' bgcolor='#f9b31c'></td>" +
                        "</tr>" +
                       "<tr>" +
                             "<td height='35' bgcolor='#FFFFFF' class='border-top-none' style='padding-left:10px;'>Tab:" + gridWindow.tabs[i].vo.Name + "<a name='" + gridWindow.tabs[i].vo.Name + "" + i + "'></a> </td>" +
                             "<td ></td>" +
                             "<td bgcolor='#FFFFFF' class='border-top-none' style='padding-left:10px;'>" + gridWindow.tabs[i].vo.Description; + " Definitions</td>" +
                           "</tr>" +
                       "</table></td>" +
                     "</tr>" +
                     "<tr>" +
                       "<td >&nbsp;</td>" +
                     "</tr>" +
                     "<tr>" +
                       "<td><table width='100%' border='0' align='center' cellpadding='0' cellspacing='0' class='border-2'>" +
                         "<tr>" +
                           "<td style='padding:10px; text-align:justify'><span class='red'>" + gridWindow.tabs[i].vo.Help; + "</span><br />" +
                               "<br />";
                var strField = "";
                //get field of each tab
                for (var j = 0; j < gridWindow.tabs[i].gTab._gridTable.m_fields.length; j++) {
                    // var objField = tab.getField(j);
                    if (strField == "" && (gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.IsDisplayedMR || gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.IsDisplayedf)) {
                        strField = "<a href='#" + gridWindow.tabs[i].vo.Name + "" + i + "" + gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Header + "" + j + "'>" + gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Header + "</a>";
                        //strField = "<a href='#'>" + gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Header + "</a>";
                    }
                    else if((gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.IsDisplayedMR || gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.IsDisplayedf)) {
                        strField += " | <a href='#" + gridWindow.tabs[i].vo.Name + "" + i + "" + gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Header + "" + j + "'>" + gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Header + "</a>";
                        //strField += " | <a href='#'>" + gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Header + "</a>";
                    }
                }
                //..........................
                helpText = helpText + strField;
                helpText = helpText + " </tr>" +
            "</table></td>" +
          "</tr>" +
          "<tr>" +
            "<td>&nbsp;</td>" +
          "</tr>";

                for (var j = 0; j < gridWindow.tabs[i].gTab._gridTable.m_fields.length; j++) {
                    // var objField = tab.getField(j);
                    if (gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Header != "" && (gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.IsDisplayedMR || gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.IsDisplayedf)) {
                        //if (gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Description != "" || gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Help != "") {
                        helpText = helpText + "<tr>" +
              "<td><table width='100%' border='0' align='center' cellpadding='0' cellspacing='0' class='border-1'>" +
                  "<tr>" +
                    "<td style='padding:10px; text-align:justify'><span class='blue'>Field: " + gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Header + "</span><a name='" + gridWindow.tabs[i].vo.Name + "" + i + "" + gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Header + "" + j + "'></a><br />" +
                        "<br />" +
                      "" + gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Description + "<br />" +
                      "<br />" +
                      "" + gridWindow.tabs[i].gTab._gridTable.m_fields[j]._vo.Help + " </td>" +
                  "</tr>" +
              "</table></td>" +
            "</tr>" +
            "<tr>" +
              "<td>&nbsp;</td>" +
            "</tr>";
                        //   }
                    }
                }
                helpText = helpText + "</table></td>" +
      "</tr>" +
      "";


            }
            helpText = helpText + "</table></td></tr></table></body></html>";




            //var tab="<table width='200px' border='0' align='center' cellpadding='0' cellspacing='0' class='border'>"
            //  +"<tr> <td><table width='100%' border='0' align='center' cellpadding='0' cellspacing='0' bgcolor='#c6daf3' class='border-bottom'>"
            //      +"<tr> <td width='5%'><img src='logo.gif' width='42' height='29' /></td> <td width='100%' class='hed1' height='29'>Vienna Solutions</td>"
            //      + "</tr></table></td></tr><tr><td>&nbsp;</td></tr></table>";

            $root.append(jQuery.parseHTML(helpText));
            htmlText = helpText;

        };
        function show() {
            var mywindow = window.open();
            //mywindow.document.title = "Help";
            //mywindow.location.href = "Help";
            mywindow.document.open();
            mywindow.document.write(htmlText);
            //mywindow.close();
            //var ch = new VIS.ChildDialog();
            //ch.setContent($root);
            //ch.setHeight(450);
            //ch.setWidth(655);
            //ch.setTitle(VIS.Msg.getMsg("Help"));
            //ch.setModal(true);
            //ch.show();
        };
        initializeComponent();
        show();

        //this.getRoot = function () {
        //    return $root;
        //};

    };
    ////Must Implement with same parameter
    //VIS.Apps.help.prototype.init = function (windowNo, frame) {
    //    //Assign to this Varable
    //    this.frame = frame;
    //    this.windowNo = windowNo;
    //    this.frame.getContentGrid().append(this.getRoot());
    //};



    ////Must implement dispose
    //VIS.Apps.help.prototype.dispose = function () {
    //    /*CleanUp Code */
    //    //dispose this component
    //    this.disposeComponent();

    //    //call frame dispose function
    //    if (this.frame)
    //        this.frame.dispose();
    //    this.frame = null;
    //};
    VIS.Apps.help = help;
})(VIS, jQuery);