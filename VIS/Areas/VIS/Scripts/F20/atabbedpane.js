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

    this.TabLevelsItems = [];
    this.TabLevels = [];

    this.tabLIObj = [];

    this.contentPane = null;


    this.aTabPaneMgr = null;


    this.getIsWorkbench = function () {
        return _workbenchTab;
    }
};

    VIS.VTabbedPane.prototype.init = function (aPanel, multiTabView) {
        this.aPanel = aPanel;
        this.multiTabView = multiTabView;
        if (multiTabView) {
            this.contentPane = new VIS.ContentPane(this, aPanel.getIncludedEmptyArea());
        }
    };

    //@not used
    VIS.VTabbedPane.prototype.setTabObject = function (obj) {
        this.tabObj = obj
    };


    VIS.VTabbedPane.prototype.finishLayout = function (isMobile) {
        if (this.contentPane)
            this.contentPane.finishLayout(isMobile);
    };

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

    if (this.multiTabView) {
        // this.aTabPaneMgr.addTab(this.aPanel,)
        var tabLevel = gTab.getTabLevel();
        this.TabLevels.push(tabLevel);

        var id = 'st_' + tabItem.getAction();
        var tObj = { action: id, text: gTab.getName(), toolTipText: gTab.getDescription(), textOnly: true, iconName: '' };

        var subTab = new VIS.AppsAction(tObj);
        

        for (var prop in this.TabLevelsItems) {
            if (this.TabLevelsItems[prop].TabLevel <= tabLevel) {
                this.TabLevelsItems[prop].ChildEle.push(tabElement);
                this.TabLevelsItems[prop].ChildTabs.push(gTab);
                this.TabLevelsItems[prop].ChildTabsItems.push(subTab);
            }
        }
        
        this.TabLevelsItems.push({ 'TabLevel': tabLevel, 'ChildEle': [], 'ChildTabs': [], 'ChildTabsItems':[], 'Id': id });
    }
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
    };

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

 VIS.VTabbedPane.prototype.setTabControl = function (tabs) {
        var $ulTabControl = this.aPanel.getTabControl();
        if (!this.multiTabView) {
            var $ulTabControl = this.aPanel.getTabControl();

            for (var i = 0; i < tabs.length; i++) {
                var li = tabs[i].getListItm();
                this.tabLIObj[tabs[i].action] = li;
                $ulTabControl.append(li);
            }
            this.aPanel.setTabNavigation();
        }
        else {

            for (var i = 0; i < tabs.length; i++) {
                var li = tabs[i].getListItm();
                this.tabLIObj[tabs[i].action] = li;
                //if (i == 0)
                    $ulTabControl.append(li.hide());
            }
        }
     };

    /**
     * set selected tab
     * @param {any} id
     */
    VIS.VTabbedPane.prototype.setSelectedTab = function (id) {

        if (this.selectedTab)
            this.selectedTab.removeClass("vis-apanel-tab-selected");
        this.selectedTab = this.tabLIObj[id];
        this.selectedTab.addClass("vis-apanel-tab-selected");

        if (this.multiTabView) {
            var $ulTabControl = this.aPanel.getTabControl();
            // $ulTabControl.empty();
            for (var j in this.tabLIObj) {
                //if (this.oldTabIndex > this.ItemsIds.indexOf(j))
                    this.tabLIObj[j].hide();
            }

            for (var i = 0; i <= this.oldTabIndex; i++) {
                // $ulTabControl.append(
                this.tabLIObj[this.ItemsIds[i]].show();
            }
            /* set sub tabs*/
            this.contentPane.setTabControl(this.TabLevelsItems[this.oldTabIndex]);
        }
    }

    VIS.VTabbedPane.prototype.getAPanel = function () {
        return this.aPanel;
    };

    VIS.VTabbedPane.prototype.notifyDataChanged = function () {
        if (this.multiTabView) {
            this.contentPane.dataStatusChanged();
        }
    };

    VIS.VTabbedPane.prototype.sizeChanged = function () {
        if(this.multiTabView)
        this.contentPane.sizeChanged();
        return;
    };

    VIS.VTabbedPane.prototype.refresh = function () {
        if (this.multiTabView)
            this.contentPane.refresh();
        return;
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

    if (this.TabAppsItems) {
        for (var i = 0; i < this.TabAppsItems.length; i++) {
            this.TabAppsItems[i].dispose();
        }
    }

    if (this.contentPane)
        this.contentPane.dispose();
    this.contentPane = null;
    this.TabAppsItems = null;
};
    //****************APanel END************************//
    //Assignment Gobal Namespace
    

}(VIS, jQuery));