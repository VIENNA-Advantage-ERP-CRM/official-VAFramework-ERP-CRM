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
            for (var i = 0; i < fields.length; i++) {
                var mField = fields[i];
                if (mField.getIsDisplayed() && mField.getIsHeaderPanelitem()) {

                    var headerItem = items[mField.getHeaderSeqno()];
                    var startCol = headerItem.StartColumn;
                    var colSpan = headerItem.ColumnSpan;
                    var startRow = headerItem.StartRow;
                    var rowSpan = headerItem.RowSpan;
                    var div = $('<div style="grid-column: ' + startCol + '/ span ' + colSpan + '; grid-row: ' + startRow + '/ span ' + rowSpan + ';background-color:pink"/>');

                  var  label = "<h3>" + w2utils.encodeTags(field.getHeader()) + "</h3>";

                    var colValue = mField.getValue();

                    if (!colValue || colValue == "" || !mField.getIsDisplayed())
                        continue;

                    var displayType = mField.getDisplayType();

                    if (mField.lookup) {
                        colValue = mField.lookup.getDisplay(colValue, true);
                    }

                    //	Date
                    else if (VIS.DisplayType.IsDate(displayType)) {
                        colValue = new Date(colValue).toLocaleString();
                    }

                    //	YesNo
                    else if (displayType == VIS.DisplayType.YesNo) {
                        var str = colValue.toString();
                        if (mField.getIsEncryptedColumn())
                            str = VIS.secureEngine.decrypt(str);
                        colValue = str.equals("Y");	//	Boolean
                    }
                    //	LOB 

                    else
                        colValue = colValue.toString();//string
                    //	Encrypted
                    if (mField.getIsEncryptedColumn() && displayType != VIS.DisplayType.YesNo)
                        colValue = VIS.secureEngine.decrypt(colValue);


                    var value = "<h5>" + w2utils.encodeTags(colValue) + "</h5>";
                    div.append(label).append(value);

                    $root.append(div);

                }
            }
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