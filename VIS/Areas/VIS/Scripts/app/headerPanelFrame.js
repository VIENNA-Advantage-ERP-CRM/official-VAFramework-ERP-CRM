; (function (VIS, $) {

    function HeaderPanel(gTab) {
        var $root = null;




        var setHeaderLayout = function () {
            if (gTab.getIsHeaderPanel()) {
                var headerItems = gTab.getHeaderPanelItems();

                if (headerItems && headerItems.length > 0) {
                    $root = $('<div style="display:grid;border:solid 1px red;height:' + tHeight + 'px;background-color:Gray">');

                    var rows = gTab.getHeaderTotalRow();
                    var columns = gTab.getHeaderTotalColumn();
                    var backColor = gTab.getHeaderBackColor();
                    var alignment = gTab.getHeaderBackColor();

                    $root.css(
                        {
                            'grid-template-columns': repeat(columns, 'auto'),
                            'grid-template-rows': repeat(rows, 'auto'),
                            'background-color': backColor
                        });

                    setHeaderItems(gTab, headerItems);
                }
            }
        };


        var setHeaderItems = function (gTab, items) {

            var fields = gTab.gridTable.gridFields;

            for (var i = 0; i < fields.length; i++)
            {
                var mField = fields[i];
                if (mField.getIsDisplayed() && mField.getIsHeaderPanelitem()) {

                    var headerItem = items[mField.getHeaderSeqno()];

                    //getHeaderHeadingOnly 
                    //getHeaderIconOnly


                }
            }



            //for (var j = 0; j < items.length; j++) {

            //    var startCol = items[j].StartColumn;
            //    var colSpan = items[j].ColumnSpan;
            //    var startRow = items[j].StartRow;
            //    var rowSpan = items[j].RowSpan;
            //    var div = $('<div style="grid-column: ' + startCol + '/ span ' + colSpan + '; grid-row: ' + startRow + '/ span ' + rowSpan + ';background-color:pink"/>');
            //    $root.append(div);
            //}
        };

        this.init = function () {
            setHeaderLayout();
        };

        this.getRoot = function () {
            return $root;
        };

    };

    VIS.HeaderPanel = HeaderPanel;

}(VIS, jQuery));