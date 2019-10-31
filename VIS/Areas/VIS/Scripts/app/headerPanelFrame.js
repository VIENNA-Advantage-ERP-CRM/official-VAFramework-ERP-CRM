; (function (VIS, $) {

    function HeaderPanel() {
        var $root = null;
        var $parent = null;
        this.headerItems = null;
        var $self = this;
        var setHeaderLayout = function (gTab, $parentRoot) {
            if (gTab.getIsHeaderPanel()) {
                $parent = $parentRoot;
                $self.headerItems = gTab.getHeaderPanelItems();

                if ($self.headerItems) {
                    var tHeight = gTab.getHeaderHeight();

                    $root = $('<div style="display:grid">');

                    var rows = gTab.getHeaderTotalRow();
                    var columns = gTab.getHeaderTotalColumn();
                    var backColor = gTab.getHeaderBackColor();
                    var alignment = gTab.getHeaderBackColor();

                    $root.css(
                        {
                            'grid-template-columns': 'repeat(' + columns + ', auto)',
                            'grid-template-rows': 'repeat(' + rows + ', auto)',
                        });

                    $self.setHeaderItems(gTab);
                    $parentRoot.css('background-color', 'rgba(var(--v-c-primary))');
                    $parentRoot.append($root);
                }
            }
        };


        this.setHeaderItems = function (gTab) {

            var fields = gTab.gridTable.gridFields;
            $root.empty();
            for (var i = 0; i < fields.length; i++) {
                var mField = fields[i];
                if (mField.getIsHeaderPanelitem()) {

                    var headerItem = $self.headerItems[mField.getHeaderSeqno()];
                    var startCol = headerItem.StartColumn;
                    var colSpan = headerItem.ColumnSpan;
                    var startRow = headerItem.StartRow;
                    var rowSpan = headerItem.RowSpan;
                    var $div = $('<div class="vis-w-p-header-data-f" style="grid-column: ' + startCol + '/ span ' + colSpan + '; grid-row: ' + startRow + '/ span ' + rowSpan + '"/>');

                    var $divIcon = $('<div class="vis-w-p-header-icon-f"></div>');

                    var $divLabel = $('<div class="vis-w-p-header-Label-f"></div>');

                    var $label = "<h3>" + w2utils.encodeTags(mField.getHeader()) + "</h3>";

                    var $spanIcon = $('<span><i></i><img></span>');

                    var colValue = mField.getValue();

                    if (!mField.getIsDisplayed())
                        continue;
                    if (colValue) {


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
                    }
                    else {
                        colValue = "";
                    }

                    var $value = "<p>" + w2utils.encodeTags(colValue) + "</p>";

                    $divIcon.append($spanIcon);
                    $divLabel.append($label).append($value);
                    $div.append($divIcon).append($divLabel);
                    $root.append($div);
                }
            }
           
        };

        this.init = function (gTab, $parentRoot) {
            setHeaderLayout(gTab, $parentRoot);
        };

        this.getRoot = function () {
            return $root;
        };

    };

    HeaderPanel.prototype.navigate = function (gTab) {
        this.setHeaderItems(gTab);
    };

    VIS.HeaderPanel = HeaderPanel;

}(VIS, jQuery));