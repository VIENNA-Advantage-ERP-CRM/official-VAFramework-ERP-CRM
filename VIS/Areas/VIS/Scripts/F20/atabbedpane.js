; (function (VIS, $) {
//****************************************************//
//**             VTabbedPane                       **//
//**************************************************//

/**
 *  Tabbed Pane - Window Tab
 *  
 */

VIS.VTabbedPane = function (isWorkBench) {
    /** Workbench 				*/
    var _workbenchTab = false;
    /** List of dependent Variables		*/
    this.oldTabIndex = -1;

    this.Items = [];
    this.ItemsIds = [];
    this.count = 0;
    this.dependents = [];
    this.TabItems = [];

    this.getIsWorkbench = function () {
        return _workbenchTab;
    }
};

VIS.VTabbedPane.prototype.setTabObject = function (obj) {
    this.tabObj = obj
}

/**
 * 	Add Tab
 *	@param id tab id
 *	@param gTab grid tab model
 *	@param tabElement GridController or VSortTab
 */
VIS.VTabbedPane.prototype.addTab = function (id, gTab, tabElement, tabItem) {

    this.ItemsIds[this.count] = id;
    this.Items[this.count] = tabElement;
    this.TabItems.push(tabItem);

    var tabDependents = gTab.getDependentOn();

    for (var i = 0; i < tabDependents.length; i++) {
        var name = tabDependents[i];
        if (this.dependents.indexOf(name) < 0) { // this.dependents.contains(name)) {
            this.dependents.push(name);
        }
    }
    this.count++;
};

/**
* 	is tab really change
*	@param action tab id
*/
VIS.VTabbedPane.prototype.getIsTabChanged = function (action) {

    var index = this.ItemsIds.indexOf(action);
    var oldIndex = this.oldTabIndex;

    if (index === oldIndex) {  //Same Tab 
        console.log("same tab");
        return false;
    }



    var oldGC = this.Items[index];
    var newGC = null;

    if (oldGC instanceof VIS.GridController) {
        newGC = oldGC;
        var display = newGC.getIsDisplayed(); // if tab is not displayed
        if (!display) {
            //VLogger.Get().Info("Not displayed - " + newGC.ToString());
            return false;
        }
    }

    if (newGC != null && oldIndex >= 0 && index != oldIndex) {
        var oldGC = this.Items[oldIndex];//.Controls[0];
        if (oldGC != null && (oldGC instanceof VIS.GridController)) {

            /* check for tab Level of tab */
            if (newGC.getTabLevel() > oldGC.getTabLevel() + 1) {
                //	Search for right tab
                for (var i = index - 1; i >= 0; i--) {
                    var rightC = this.Items[i];// .Controls[0];// getComponentAt(i);
                    var rightGC = null;
                    if (rightC instanceof VIS.GridController) {
                        rightGC = rightC;
                        if (rightGC.getTabLevel() == oldGC.getTabLevel() + 1) {
                            VIS.ADialog.warn("TabSwitchJumpGo", true, "", rightGC.getTitle());
                            return false;;
                        }
                    }
                }
                VIS.ADialog.warn("TabSwitchJump");
                return false;
            }
            oldGC.setMnemonics(false);
        }
    }
    //	Switch
    if (newGC != null) {
        newGC.setMnemonics(true);
    }
    this.oldTabIndex = index;

    return true;
};

VIS.VTabbedPane.prototype.restoreTabChange = function () {
    this.oldTabIndex = -1;
}

/**
 *  current selected tab element either GridController or VSortTab
 */
VIS.VTabbedPane.prototype.getTabElement = function (action) {
    return this.Items[this.oldTabIndex];
};
/**
 *  current selected tab index
 */
VIS.VTabbedPane.prototype.getSelectedIndex = function () {
    return this.oldTabIndex;
};

VIS.VTabbedPane.prototype.sizeChanged = function (height, width) {
    for (var prop in this.Items) {
        this.Items[prop].sizeChanged(height, width);
    }
}

VIS.VTabbedPane.prototype.evaluate = function (e) {
    var process = e == null;
    var columnName = null;
    if (!process) {
        columnName = e;
        if (columnName != null)
            process = this.dependents.indexOf(columnName) > -1;//  contains(columnName);
        else
            process = true;
    }

    if (process) {
        //VLogger.Get().Config(columnName == null ? "" : columnName);
        for (var i = 0; i < this.TabItems.length; i++) {
            var c = this.Items[i];
            if (c instanceof VIS.GridController) {
                var gc = c;
                var display = gc.getIsDisplayed();
                this.TabItems[i].setEnabled(display);
            }
        }
    }
};


/**
 *  Dispose all contained VTabbedPanes and GridControllers
 */
VIS.VTabbedPane.prototype.dispose = function () {
    for (var prop in this.Items) {
        this.Items[prop].dispose();
        this.Items[prop] = null;
    }

    this.TabItems.length = 0;
    this.TabItems = null;

    this.Items.length = 0;
    this.Items = null;
    this.ItemsIds.length = 0;
    this.ItemsIds = null;

    this.dependents.length = 0;
    this.dependents = null;
};
    //****************APanel END************************//
    //Assignment Gobal Namespace
    

}(VIS, jQuery));