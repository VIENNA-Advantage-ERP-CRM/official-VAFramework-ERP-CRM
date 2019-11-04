; (function (VIS, $) {


    var AWINDOW_HEADER_HEIGHT = 43;
    var APANEL_HEADER_HEIGHT = 50; //margin adjust of first tr
    var APANEL_FOOTER_HEIGHT = 40


    var tmpvc = document.querySelector('#vis-ad-viewctrltmp').content;// $("#vis-ad-windowtmp");


//****************************************************//
//**            Grid Controller                    **//
//**************************************************//
VIS.GridController = function (showRowNo, doPaging, id) {

    this.id = id;
    this.vGridPanel = new VIS.VGridPanel();
    this.vTable = new VIS.VTable();
    this.vCardView = new VIS.VCardView();
    this.vMapView = new VIS.VMapView();
    this.vHeaderPanel = null;
    this.windowNo = 0;
    this.aPanel = null;
    this.singleRow = false;
    this.isCardRow = false;
    this.doPaging = doPaging;
    this.vIncludedGC = null;
    this.m_tree = null;

    this.onRowInserted = null;
    this.onRowInserting = null;
    this.curTabPanel = null;
    this.ul_tabPanels = null;




    this.rightPaneLinkItems = [];
    this.leftPaneLinkItems = [];

    this.showClient = false;
    this.showOrg = false;

    var level = VIS.Env.getCtx().getShowClientOrg();

    if (level == VIS.Env.SHOW_CLIENT_ONLY) {
        this.showClient = true;

    }
    else if (level == VIS.Env.SHOW_ORG_ONLY) {
        this.showOrg = true;
    }
    else if (level == VIS.Env.SHOW_CLIENT_ORG) {
        this.showOrg = true;
        this.showClient = true;
    }

    this.isParentDetailVisible = false; //gc has parent detail panel used in swutch row presentation

    this.isIncludedGCVisible = false; // Is Include Grid  Visible or Not

    this.displayAsIncludedGC = false; // is this GC act as IncludedGrid in other GC

    var $divPanel, $divCard, $divMap, $tabControl, $tableMain, $divHeader, tabItems = [];  //layout
    var td1_tr1, td1_tr2, td1_tr3, $divGrid, $divTree, $divContent, $divMain, $td0_tr3;
    var $layout = null;

    var aAdd, aEdit = null; //toolbar action

    function initlizeComponent() {

        var clone = $(document.importNode(tmpvc, true));

        //            $tableMain = $("<div class='vis-height-full'>").hide();

      //  td1_tr1 = $("<td colspan='2' class='vis-height-auto'>");
        //td1_tr2 = $("<td colspan='2' class='vis-height-auto'>");
        //td1_tr3 = $("<td style='width:100%'>");

        /* Tree Div */
        $divTree = $("<div>"); //tree div

        $td0_tr3 = clone.find(".vis-ad-w-p-vc-tree").append($divTree).hide();

        $tableMain = clone.find(".vis-ad-w-p-vc").hide();

        //$tableMain = $("<table class='vis-gc-table'>").append($("<tr>").append(td1_tr1))
        //    .append($("<tr>").append(td1_tr2))
        //    .append($("<tr  class='vis-height-full'>").append($td0_tr3).append(td1_tr3)).hide();

        /* Tab Control */
        $tabControl = clone.find(".vis-ad-w-p-vc-actions").hide();
        /* End */

        /*divHeader*/
        $divHeader = clone.find(".vis-ad-w-p-vc-actions");// $("<div class='vis-gc-header'>").hide();
        /*end*/

        /* Multi,card and single view */
        $divGrid = $("<div class='vis-gc-vtable'>");
        $divPanel = $("<div class='vis-ad-w-p-vc-editview' id='AS_" + id + "'>");
        $divCard = $("<div class='vis-gc-vcard'>");
        $divMap = $("<div class='vis-gc-vmap'>");
        /* End */

       // td1_tr1.append($divHeader); //first Row
        //td1_tr2.append($tabControl); //Second Row

        $divContent = clone.find(".vis-ad-w-p-vc-gc"); // $("<div class='vis-height-full' style='overflow:hidden'>"); //Main Contant
        //$divMain = $("<div class='vis-height-full'>");
        $divContent.append($divGrid).append($divPanel).append($divCard).append($divMap);
       // td1_tr3.append($divContent);

    }

    initlizeComponent();

    var self = this;

    var onsubToolBarClick = function (action) {
        //console.log(action);

        if (action == "Edit_sub") {
            if (self.displayAsIncludedGC) {
                //fire Tab changed and open in edit mode
                if (self.aPanel.tabActionPerformed(self.id)) {
                    self.switchSingleRow();
                    $tabControl.find('.vis-apanel-tab-selected')[0].scrollIntoView();
                }
                return;
            }
        }
        else {
            if (self.displayAsIncludedGC) {
                //fire Tab changed and open in edit mode
                if (!self.aPanel.tabActionPerformed(self.id))
                    return;
                self.switchSingleRow();
                //self.aPanel.cmd_new();
                // return;
                setTimeout(function (t) {
                    t.aPanel.cmd_new()
                }, 500, self);
            }
        }
    };

    function createToolbar() {

        aAdd = new VIS.AppsAction({ action: "Add_sub", parent: null, enableDisable: true, toggle: false, imageOnly: true, isSmall: true, onAction: onsubToolBarClick }); //Create Apps Action
        aEdit = new VIS.AppsAction({ action: "Edit_sub", parent: null, enableDisable: true, toggle: false, imageOnly: true, isSmall: true, onAction: onsubToolBarClick }); //Create Apps Action
        $tabControl.append(aEdit.getListItm()).append(aAdd.getListItm());
    };
    createToolbar();

    this.initLayout = function () {
        //console.log(this.id);
        //var pstyle = 'border: 1px solid #dfdfdf; padding: 0px;';
        //var pstyle = 'padding: 0px;background-color:transparent;';

        //var panels = [];
        //if (this.m_tree != null) {
        //    panels.push({ type: 'left', size: 250, style: pstyle, resizable: true, content: this.m_tree.getRoot() });
        //}
        //panels.push({ type: 'main', style: pstyle, content: $divContent });

        //$layout = $divMain.w2layout({
        //    name: 'layout_' + id,
        //    panels: panels,
        //    resizer: 3,
        //});

        //$divMain.w2render($layout['name']);
        this.layoutLoaded = true;
    };

    this.sizeChanged = function (height, width) {
        return;
        /* SetHeight */
        if (!height) {
            height = VIS.Env.getScreenHeight() - (AWINDOW_HEADER_HEIGHT + APANEL_HEADER_HEIGHT + APANEL_FOOTER_HEIGHT);
        }
        $tableMain.height(height);
        $divGrid.height(height - 2);
        $divPanel.height(height);
        $divTree.height(height);
        $divCard.height(height);
        $divMap.height(height);
        if (this.m_tree) {
            this.m_tree.setSize(height, width);
        }
        if (this.vCardView)
            this.vCardView.sizeChanged(height, $divCard.width());
        if (this.vMapView)
            this.vMapView.sizeChanged(height, $divMap.width());

    };

    this.sizeChanged();
    $divPanel.append(this.vGridPanel.getRoot()); //apaend Single Layout

    this.getRoot = function () {
        return $tableMain;
    };




    this.getTreeArea = function () {
        return $divTree;
    };

    this.setTreePanelWidth = function (width) {
        $td0_tr3.show();
    };


    this.getId = function () {
        return id;
    };

    this.getReocrdDiv = function () {
        return $divHeader;
    };

    this.getTabControl = function () {
        return $tabControl;
    };

    this.setRecord = function (record) {

        // $divRecords.empty();
        // $divRecords.html(record + " " + VIS.Msg.getMsg("Results"));
    };

    this.getVTablePanel = function () {
        return $divGrid;
    };

    this.getVPanel = function () {
        return $divPanel;
    };

    this.getVCardPanel = function () {
        return $divCard;
    };

    this.getVMapPanel = function () {
        return $divMap;
    };

    //  this.setRecord(0);

    this.setUI = function (isIncluded) {
        if (isIncluded) {
            $divHeader.html(this.gTab.getName());
            $divHeader.css('white-space', 'nowrap');
            $divHeader.show();
            $tabControl.show();
            aEdit.setEnabled(false);
            this.vTable.grid.show.selectColumn = false;
        }
        else {
            $divHeader.hide();
            $tabControl.hide();
            this.vTable.grid.show.selectColumn = true;
        }
    };

    this.enableDisableToolbarItems = function (isEnable) {
        aEdit.setEnabled(isEnable);
    };

    //Bind Table Event
    this.vTable.onSelect = function (event) {

        if (self.aPanel && self.aPanel.setBusy) {
            self.aPanel.setBusy(true);
        }
        self.cancelSel = false;
        //var cRow = -1, nRow;
        //if (self.gTab.needSave(true, false))
        //    cRow = self.gTab.getCurrentRow();

        self.onTableRowSelect(event);

        if (self.cancelSel)
            event.isCancelled = true;
        else {
            //    nRow = self.gTab.getCurrentRow();
            //  if (cRow != -1 && cRow != nRow)
            ///    setTimeout(function (t, r) {
            //     t.refreshRow(r); //refresh old row
            // }, 10, this, cRow);
        }
        if (self.aPanel && self.aPanel.setBusy) {
            self.aPanel.setBusy(false);
        }
    };


    this.vTable.onCellValueChanged = function (event, invokeReq) {
        if (invokeReq)
            window.setTimeout(function () {
                self.vetoablechange(event);
                self.vTable.refreshCells();
            }, 10);
        else {
            self.vetoablechange(event);
            self.vTable.refreshCells();
        }
    };

    this.vCardView.onCardEdit = function (event, onlySelect) {
        self.onTableRowSelect(event);
        //switch self.singleRow = false; //force single view
        if (!onlySelect)
            self.switchSingleRow();
    };

    //On Sort event
    this.vTable.onSort = function (event) {


        //this.vTable.getGrid().records
        window.setTimeout(function () {
            self.navigate(self.gTab.getCurrentRow(), true);
        }, 10);
        // console.log(self.vTable.getGrid().records);
        // console.log(self.gTab.getRecords());
        // console.log(self.gTab.getTableModel().getSortModel());
    };

    //show single layout
    //this.vTable.onEdit = function (recid) {
    //    // if (self.singleRow)
    //    //  return true;
    //    // if (self.vTable.getSelection().length < 1)
    //    // return;

    //    if (self.displayAsIncludedGC) {
    //        //fire Tab changed and open in edit mode
    //        self.aPanel.tabActionPerformed(tabItems[self.selTabIndex].action);
    //        self.switchSingleRow();
    //        return;
    //    }

    //    self.switchRowPresentation();
    //};


    //this.vTable.onAdd = function (recid) {
    //    // if (self.singleRow)
    //    //  return true;
    //    // if (self.vTable.getSelection().length < 1)
    //    // return;

    //    if (self.displayAsIncludedGC) {
    //        //fire Tab changed and open in edit mode
    //        self.aPanel.tabActionPerformed(tabItems[self.selTabIndex].action);
    //        self.switchSingleRow();
    //        //self.aPanel.cmd_new();
    //        // return;
    //    }
    //    setTimeout(function (t) {
    //        t.aPanel.cmd_new()
    //    }, 500, self);
    //};


    ////Called by editor controls
    //this.vetoablechangeListner = function (event) {
    //    self.vetoablechangeHandler(event);
    //}

    this.disposeComponent = function () {

        //$divRecords.off("tap click");

        this.rightPaneLinkItems.length = 0;
        this.rightPaneLinkItems = null;
        this.leftPaneLinkItems.length = 0;
        this.leftPaneLinkItems = null;


        $divGrid = null;
        $divRecords = null;
        //tabItems.length = 0;

        for (var i = 0; i < tabItems.length; i++) {
            tabItems[i].dispose("ul_" + this.id);
        }

        tabItems = null;
        this.seletedTab = null;
        td1_tr1 = null;
        td1_tr2 = null;
        //td1_tr3 = null;
        this.vGridPanel.dispose();
        this.vGridPanel = null;

        this.vCardView.dispose();
        this.vCardView.onSelect = null;

        this.vMapView.dispose();


        this.vTable.dispose();
        this.vTable.onSelect = null;
        this.vTable.onSort = null;
        this.vTable = null;

        $divGrid = null;
        $divPanel = null;
        $divCard = null;
        $divMap = null;
        self = null;
        this.getId = null;
        this.getReocrdDiv = null;
        this.getRoot = null;
        this.getVTablePanel = null;
        this.getVPanel = null;
        this.getVCardPanel = null;
        $tableMain.remove();
        //console.log($tableMain);
        $tableMain = null;
        if ($layout)
            $layout.destroy();
        $layout = null;
        this.onRowInserted = null;
    };
};


VIS.GridController.prototype.createTabPanel = function (panels) {
    if (!this.ul_tabPanels) {
        if (panels && panels.length > 0) {
            var str = '';
            for (var i = 0; i < panels.length; i++) {
                var iconPath = '';
                if (panels[i].getIconPath()) {
                    iconPath = panels[i].getIconPath();
                }
                else {
                    iconPath = 'VIS/Images/base/defPanel.ico';
                }

                str += '<li ><img alt="' + panels[i].getName() + '" title="' + panels[i].getName() + '" default="' + panels[i].getIsDefault() + '" data-panelID="' + panels[i].getAD_TabPanel_ID() + '" data-cName="' + panels[i].getClassName()
                    + '" data-Name="' + panels[i].getName() + '" src="' + VIS.Application.contextUrl + 'Areas/' + iconPath + '"></img></li>';
            }
            this.ul_tabPanels = str;
        }
    }
    else {
        this.ul_tabPanels = this.ul_tabPanels.replace('default="true"', '');
    }
    return this.ul_tabPanels;
};

VIS.GridController.prototype.setCurrentPanelID = function (ID) {
    this.panelID = ID;
};

VIS.GridController.prototype.getCurrentPanelID = function () {
    return this.panelID;
};

VIS.GridController.prototype.setCurrentPanel = function (className, windowNo) {

    if (this.curTabPanel) {
        if (this.curTabPanel.dispose) {
            this.curTabPanel.dispose();
        }
        this.curTabPanel = null;
    }

    if (className) {
        var type = VIS.Utility.getFunctionByName(className, window);
        if (type) {
            var panel = new type();
            panel.startPanel(windowNo, this.gTab);
            if (this.gTab.getRecord_ID() > -1) {
                panel.refreshPanelData(this.gTab.getRecord_ID(), this.gTab.getTableModel().getRow(this.gTab.getCurrentRow()));
            }
            this.curTabPanel = panel;
        }
    }
};

VIS.GridController.prototype.getCurrentPanel = function (panel) {
    return this.curTabPanel;
};

VIS.GridController.prototype.refreshTabPanelData = function (record_ID) {
    if (this.curTabPanel && $(this.curTabPanel.getRoot()).is(':visible')) {
        this.curTabPanel.refreshPanelData(record_ID, this.gTab.getTableModel().getRow(this.gTab.getCurrentRow()));
    }
};


/** initGrid
  * - Map table to model
    - Update (multi-row) table info with renderers/editors
    - build single-row panel
    - initialize display
@param onlyMultirow
@param curwindowNumber
@name  aPanel
@name mTab
<returns></returns>*/
VIS.GridController.prototype.initGrid = function (onlyMultiRow, curWindowNo, aPanel, mTab) {

    var fields = mTab.gridTable.gridFields;
    var mField = null;
    var vGridId = curWindowNo + "_" + mTab.vo.AD_Tab_ID;
    var vCardId = curWindowNo + "_c" + mTab.vo.AD_Tab_ID;
    var vMapId = curWindowNo + "_m" + mTab.vo.AD_Tab_ID;

    mTab.getTableModel().setDoPaging(this.doPaging);

    //bindingSource =  tab.GetDataTable();
    //var bindingSource = null;
    var role = VIS.MRole;
    if (!role.getIsDisplayClient() || !this.showClient)
        mTab.getField("AD_Client_ID").setDisplayed(false);
    if (!role.getIsDisplayOrg() || !this.showOrg)
        mTab.getField("AD_Org_ID").setDisplayed(false);



    var size = this.vTable.setupGridTable(aPanel, fields, this.getVTablePanel(), vGridId, mTab, this);


    mTab.addDataStatusListener(this);

    if (!onlyMultiRow) {

        this.vCardView.setupCardView(aPanel, mTab, this.getVCardPanel(), vCardId);

        if (mTab.getIsMapView())
            this.vMapView.setupMapView(aPanel, this, mTab, this.getVMapPanel(), vMapId);



        for (var i = 0; i < size; i++) {
            mField = fields[i];

            if (mField.getIsDisplayed()) {
                var iControl = VIS.VControlFactory.getControl(mTab, mField, false, false, false);
                if (iControl == null && !mField.getIsHeading()) {
                    //log.warning("Editor not created for " + mField.getColumnName());
                    continue;
                }
                if (iControl != null) {
                    //  MField => VEditor - New Field value to be updated to editor
                    iControl.setReadOnly(true);
                    mField.setPropertyChangeListener(iControl);
                    //  VEditor => this - New Editor value to be updated here (MTable)
                    iControl.addVetoableChangeListener(this);
                }
                this.vGridPanel.addField(iControl, mField);

                if (iControl instanceof VIS.Controls.VButton) {

                    if (mField.getIsLink()) {
                        if (mField.getIsRightPaneLink()) {
                            this.rightPaneLinkItems.push(iControl);
                        }
                        else {
                            this.leftPaneLinkItems.push(iControl);
                        }
                    }
                    iControl.addActionListner(aPanel);
                }

                iControl = null;
            }
        }
    }


    //  Tree Graphics Layout
    var AD_Tree_ID = 0;

    if (mTab.getIsTreeTab()) {
        //, Name
        var sql = "VIS_120";

        var param = [];
        param[0] = new VIS.DB.SqlParam("@AD_Client_ID", VIS.Env.getCtx().getAD_Client_ID());
        param[1] = new VIS.DB.SqlParam("@AD_Table_ID", mTab.getAD_Table_ID());
        AD_Tree_ID = executeScalar(sql, param);

        //if (AD_Tree_ID > 0) {
        //    this.m_tree = new VIS.TreePanel(curWindowNo, false, true);
        //    //Set Style
        //    if (mTab.getTabNo() == 0)	//	initialize other tabs later
        //    {
        //        this.m_tree.initTree(AD_Tree_ID);
        //    }

        //    this.getTreeArea().append(this.m_tree.getRoot());
        //    this.m_tree.addSelectionChangeListner(this);
        //    this.setTreePanelWidth("300px");
        //    this.getTreeArea().width("300px");
        //    this.m_tree.setSize(this.getTreeArea().height());

        //}
        if (AD_Tree_ID > 0) {
            this.treeID = AD_Tree_ID;
            if (mTab.getShowSummaryLevel()) {
                this.onDemandTree = true;
                this.m_tree = new VIS.TreePanel(curWindowNo, false, true, true, this);
                aPanel.aShowSummaryLevel.show();

            }
            else {
                this.onDemandTree = false;
                this.m_tree = new VIS.TreePanel(curWindowNo, false, true, false, this);
                aPanel.aShowSummaryLevel.hide();
            }
            this.m_tree.setTabID(mTab.getAD_Tab_ID());
            //Set Style
            if (mTab.getTabNo() == 0)	//	initialize other tabs later
            {
                this.m_tree.initTree(AD_Tree_ID);
            }

            this.getTreeArea().append(this.m_tree.getRoot());
            this.m_tree.addSelectionChangeListner(this);



            this.setTreePanelWidth("300px");
            this.getTreeArea().width("300px");
            this.m_tree.setSize(this.getTreeArea().height());

        }
        else    //  No Graphics - hide
        {

        }
    }

    //ADD Table Model Event Listner
    mTab.getTableModel().addTableModelListener(this.vTable);
    mTab.getTableModel().addCardModelListener(this.vCardView);


    //ADD Table Model Event Listner
    mTab.getTableModel().addRowChangedListener(this);

    //AddQueryCompleteListner
    mTab.getTableModel().addQueryCompleteListner(this);

    mTab.getTableModel().setDoPaging(this.doPaging);
    mTab.getTableModel().setCurrentPage(1);

    this.gTab = mTab;
    this.windowNo = curWindowNo
    this.onlyMultiRow = onlyMultiRow;
    this.aPanel = aPanel;

    //  Set initial presentation
    if (onlyMultiRow || !mTab.getIsSingleRow()) {
        // this.switchMultiRow();
        this.singleRow = false;
        //this.switchMultiRow();
    }
    else
        this.switchSingleRow(true);
};

    VIS.GridController.prototype.getIsHeaderPanel = function () {
        return this.gTab.getIsHeaderPanel();
    };

VIS.GridController.prototype.detachDynamicAction = function () {
    var i = 0;
    var j = 0;

    for (var i = 0, j = this.leftPaneLinkItems.length; i < j; i++) {
        this.leftPaneLinkItems[i].getControl().detach();
    }
    for (i = 0, j = this.rightPaneLinkItems.length; i < j; i++) {
        this.rightPaneLinkItems[i].getControl().detach();
    }
    i = null;
    j = null;
};

VIS.GridController.prototype.switchRowPresentation = function () {

    if (this.singleRow)
        this.switchMultiRow();
    else
        this.switchSingleRow();
};

VIS.GridController.prototype.getIsSingleRow = function () {
    return this.singleRow;
};

VIS.GridController.prototype.getIsMapRow = function () {
    return this.isMapRow;
};

VIS.GridController.prototype.onTableRowSelect = function (event) {

    if (this.rowSetting) { return };
    //  no rows
    if (this.gTab.getRowCount() == 0)
        return;

    //	vTable.stopEditor(graphPanel);
    var rowTable = this.vTable.get(event.recid, true);
    var rowCurrent = this.gTab.getCurrentRow();

    if (rowTable == -1)  //  nothing selected
    {
        if (rowCurrent >= 0) {
            this.vTable.select(event.recid);
            this.vCardView.navigate(event.recid, !this.isCardRow);
            return;
        }
    }
    if (rowTable != rowCurrent) {
        this.rowSetting = true;
        this.navigate(rowTable);
        this.rowSetting = false;
    }
    else if (!this.settingGridSelecton) {
        return;
    }
    this.dynamicDisplay(-1);


    //	TreeNavigation - Synchronize 	-- select node in tree
    //if (m_tree != null)
    //    m_tree.setSelectedNode (m_mTab.getRecord_ID());	//	ignores new (-1)


    var selfThis = this;

    //	Query Included Tab
    if (!this.getIsSingleRow()) {
        window.setTimeout(function () {
            if (selfThis.vIncludedGC != null) {
                selfThis.switchIncludedGC();
                //vIncludedGC.getMTab().query(0, 0, false);
            }
        }, 50);
    }
    else {
        if (selfThis.vIncludedGC != null) {
            selfThis.switchIncludedGC();
            //vIncludedGC.getMTab().query(0, 0, false);
        }
    }
    //if (this.currentRowIndex === event.index) {
    //    return;
    //}
    //this.currentRowIndex = event.index;

    //this.gTab.navigate(this.currentRowIndex, true);
    //this.dynamicDisplay(-1);
};

VIS.GridController.prototype.switchIncludedGC = function () {
    if (this.vIncludedGC != null) {
        this.switchIncludedGC();
        //vIncludedGC.getMTab().query(0, 0, false);
    }
};

VIS.GridController.prototype.dynamicDisplay = function (col) {
    //if (!this.getIsSingleRow() || this.onlyMultiRow)
    //    return;
    if (this.gTab == null)
        return;

    if (this.isCardRow || this.isMapRow) {
        return;
    }


    var recID = this.gTab.getRecord_ID();

    if (!this.getIsSingleRow()) {
        this.dynamicDisplayLinks(col);
        //if (recID == -1) {
        //    this.setDefaultFocus();
        //}
        return;
    }


    if (!this.gTab.getIsOpen())
        return;
    if (col >= 0) {
        var changedField = this.gTab.getField(col);
        var columnName = changedField.getColumnName();
        var dependants = this.gTab.getDependantFields(columnName);
        //log.config("(" + m_mTab.toString() + ") "
        //	+ columnName + " - Dependents=" + dependants.size());
        //	No Dependents and no Callout - Set just Background
        if (dependants.length == 0 && changedField.getCallout().length > 0) {
            // List<Control> comp = currentTab.GetControls();
            //for (var i = 0; i < vPanel.allControls.Count; i++)
            // {
            //if (columnName.equals(vPanel.allControls[i].Name))
            //{
            // vPanel.allControls[i].BackColor = Color.LightSkyBlue;
            //ve.setBackground( changedField.isError() );
            //   break;
            //}
            // }
            return;
        }
    }

    //  complete single row re-display
    var noData = this.gTab.getRowCount() == 0;
    //log.config(m_mTab.toString() + " - Rows=" + m_mTab.getRowCount());
    //  All Components in vPanel (Single Row)
    var compos = this.vGridPanel.getComponents();
    var size = compos.length;




    for (var i = 0; i < size; i++) {
        var comp = compos[i];
        //IControl compI = (IControl)comp;
        var columnName = comp.getName();

        if (columnName != null) {

            if (columnName.startsWith("lbl")) {
                columnName = columnName.substring(3);
            }
            var mField = this.gTab.getField(columnName);
            if (mField != null) {
                if (mField.getIsDisplayed(true)) {		//  check context
                    var vis = comp.tag;
                    if (!comp.getIsVisible() && ((vis == null || vis == "undefined") || vis))
                        comp.setVisible(true);		//  visibility
                    if (comp instanceof VIS.Controls.IControl) {
                        var ve = comp;
                        if (noData)
                            ve.setReadOnly(true);
                        else {
                            //   mField.vo.tabReadOnly = this.gTab.getIsReadOnly();
                            var rw = mField.getIsEditable(true) && !this.gTab.getIsReadOnly();	//  r/w - check Context



                            ve.setReadOnly(!rw);
                            //	log.log(Level.FINEST, "RW=" + rw + " " + mField);
                            ve.setMandatory(mField.getIsMandatory(true));
                            //mField.validateValue();
                            ve.setBackground(mField.getIsError());

                            // Check if new record and current field marked as default field and focus is not set yet, 
                            // then set focus.
                            if (mField.getIsDefaultFocus() && !this.isDefaultFocusSet && !comp.getName().startsWith("lbl")) {
                                ve.setDefaultFocus(true);
                                this.isDefaultFocusSet = true;
                            }

                        }
                    }
                }
                else if (comp.getIsVisible())
                    comp.setVisible(false);

                // reset error status for nondisplayed fields if they are not mandatory
                if (!mField.getIsDisplayed(true) && !mField.getIsMandatory(true)) {
                    mField.setError(false);
                }
            }
        }
    }
};

VIS.GridController.prototype.dynamicDisplayLinks = function (col) {
    if (this.displayAsIncludedGC) return;
    //  complete single row re-display
    var noData = this.gTab.getRowCount() == 0;
    var linkArr = this.vGridPanel.getLinkComponents();

    for (var i = 0; i < linkArr.length; i++) {
        var comp = linkArr[i];
        //IControl compI = (IControl)comp;
        var columnName = comp.getName();

        if (columnName != null) {

            if (columnName.startsWith("lbl")) {
                columnName = columnName.substring(3);
            }
            var mField = this.gTab.getField(columnName);
            if (mField != null) {
                if (mField.getIsDisplayed(true)) {//  check context
                    var vis = comp.tag;
                    if (!comp.getIsVisible() && ((vis == null || vis == "undefined") || vis))
                        comp.setVisible(true);		//  visibility
                    if (comp instanceof VIS.Controls.IControl) {
                        var ve = comp;
                        if (noData)
                            ve.setReadOnly(true);
                        else {
                            var rw = mField.getIsEditable(true) && !this.gTab.getIsReadOnly();	//  r/w - check Context
                            ve.setReadOnly(!rw);

                            //if (mField.getIsDefaultFocus()) {
                            //    ve.setDefaultFocus(true);
                            //}


                            //	log.log(Level.FINEST, "RW=" + rw + " " + mField);
                            ve.setMandatory(mField.getIsMandatory(true));
                            //mField.validateValue();
                            ve.setBackground(mField.getIsError());
                        }
                    }
                }
                else if (comp.getIsVisible())
                    comp.setVisible(false);
                // reset error status for nondisplayed fields if they are not mandatory
                if (!mField.getIsDisplayed(true) && !mField.getIsMandatory(true)) {
                    mField.setError(false);
                }
            }
        }
    }
};


//Set Default Focus for grid... Not in use Yet.
VIS.GridController.prototype.setDefaultFocus = function () {

    var noData = this.gTab.getRowCount() == 0;
    var compos = this.vGridPanel.getComponents();
    var size = compos.length;

    for (var i = 0; i < size; i++) {
        var comp = compos[i];
        //IControl compI = (IControl)comp;
        var columnName = comp.getName();

        if (columnName != null) {
            var mField = this.gTab.getField(columnName);
            if (mField != null) {
                if (mField.getIsDefaultFocus()) {
                    this.vTable.setDefaultFocus(columnName);
                    break;
                }
            }
        }
    }


};

VIS.GridController.prototype.setVisible = function (visible) {

    if (!this.layoutLoaded) {
        this.initLayout();

    }


    if (visible) {
        this.getRoot().show();
        this.vTable.resize();
    }
    else {
        this.getRoot().hide();
    }
};

VIS.GridController.prototype.getMTab = function () {
    return this.gTab;
};

VIS.GridController.prototype.getIsDisplayed = function () {
    return this.gTab.getIsDisplayed(false);
};

VIS.GridController.prototype.getTabLevel = function () {
    return this.gTab.getTabLevel();
};

VIS.GridController.prototype.getTitle = function () {
    return this.gTab.getName();
};

VIS.GridController.prototype.getSelection = function (retIndex) {
    return this.vTable.getSelection(retIndex);
};

/* 
 * Selected grid Rows 
 *  
 */
VIS.GridController.prototype.getSelectedRows = function () {
    return this.vTable.getSelectedRows();
};

VIS.GridController.prototype.getColumnNames = function () {

    if (this.colNames)
        return this.colNames;

    var fields = this.gTab.getTableModel().getFields();

    var colObj = {};
    for (var i = 0; i < fields.length; i++) {
        colObj[fields[i].getColumnName()] = fields[i].getHeader();
    }

    this.colNames = colObj;
    return this.colNames;
};

VIS.GridController.prototype.setMnemonics = function (setNum) {

};

VIS.GridController.prototype.activate = function (oldGC) {

    this.vTable.activate();
    oldGC = oldGC || {};

    //if (oldGC.isIncludedGCVisible) { //Hide Included of Old GC
    //    oldGC.vIncludedGC.setVisible(false);
    //}
    this.vTable.setReadOnly(false);

    if (this.displayAsIncludedGC) {
        var tdArea = this.aPanel.getLayout();
        this.setUI(false);
        this.getRoot().detach();
        tdArea.append(this.getRoot());
        this.displayAsIncludedGC = false;
        this.aPanel.getIncludedEmptyArea().css({ 'width': '', "padding": '' });
    }
    else if (this.gTab.getIncluded_Tab_ID() == 0) {
        var olcIncludedTab = oldGC.vIncludedGC;
        if (olcIncludedTab) {
            var tdArea = olcIncludedTab.aPanel.getLayout();
            olcIncludedTab.setUI(false);
            olcIncludedTab.getRoot().detach();
        }
        //tdArea.append(oldGC.getRoot());
        //oldGC.displayAsIncludedGC = false;
        //oldGC.aPanel.getIncludedEmptyArea().css({ 'width': '', "padding": '' });

        //old.switchIncludedGC();
    }
    //vIncludedGC
    this.isIncludedGCVisible = false;
    // || this.isIncludedGCVisible || oldGC.vIncludedGC) // this is Shown as Included GC then reset parameter
    //{
    //    if (!this.isIncludedGCVisible) {
    //        this.getRoot().width("100%");
    //    }

    //    //oldGC.isIncludedGCVisible = false;
    //    this.displayAsIncludedGC = false;
    //    this.isIncludedGCVisible = false;
    //    this.toggleTabItems();
    //}

    if (this.vIncludedGC) { // has included GC
        //  this.vIncludedGC.vTable.activate();
        this.vIncludedGC.displayAsIncludedGC = false;
        this.vIncludedGC.isIncludedGCVisible = false;

    }

    this.activateTree();


    //this.switchMultiRow();
    //oldGC = null;
};

VIS.GridController.prototype.multiRowResize = function () {
    if (!this.singleRow && !this.isCardRow)
        this.vTable.resize();
};

/* TREE */
/*
 * 	Activate Grid Controller.
 * 	Called by APanel when GridController is displayed (foreground)
 */
VIS.GridController.prototype.activateTree = function () {
    //	Tree to be initiated on second/.. tab
    if (this.gTab.getIsTreeTab() && (this.gTab.getTabNo() > 0)) {
        var AD_Tree_ID = 0;
        if (this.gTab.getTabLevel() > 0)	//	check previous tab for AD_Tree_ID
        {
            var keyColumnName = this.gTab.getKeyColumnName();
            var treeName = "AD_Tree_ID";
            if (keyColumnName.startsWith("CM")) {
                if (keyColumnName.equals("CM_Container_ID"))
                    treeName = "AD_TreeCMC_ID";
                else if (keyColumnName.equals("CM_CStage_ID"))
                    treeName = "AD_TreeCMS_ID";
                else if (keyColumnName.equals("CM_Template_ID"))
                    treeName = "AD_TreeCMT_ID";
                else if (keyColumnName.equals("CM_Media_ID"))
                    treeName = "AD_TreeCMM_ID";
            }
            AD_Tree_ID = VIS.Env.getCtx().getWindowContextAsInt(this.windowNo, treeName);
            //log.config(keyColumnName + " -> " + treeName + " = " + AD_Tree_ID);
        }
        if (AD_Tree_ID == 0) {

            var AD_Table_ID = this.gTab.getAD_Table_ID();
            var AD_Client_ID = VIS.Env.getCtx().getAD_Client_ID();

            if (AD_Table_ID == 0)
                return 0;

            //var dr = executeReader("SELECT AD_Tree_ID, Name FROM AD_Tree "
            //    + "WHERE AD_Client_ID=" + AD_Client_ID + " AND AD_Table_ID=" + AD_Table_ID + " AND IsActive='Y' AND IsAllNodes='Y' "
            //    + "ORDER BY IsDefault DESC, AD_Tree_ID");

            var sql = "VIS_121";
            var param = [];
            param[0] = new VIS.DB.SqlParam("@AD_Client_ID", AD_Client_ID);
            param[1] = new VIS.DB.SqlParam("@AD_Table_ID", AD_Table_ID);
            var dr = executeReader(sql, param);

            if (dr.read()) {
                AD_Tree_ID = dr.getInt(0);
            }
            dr = null;
            //AD_Tree_ID = 101;
            //MTree.getDefaultAD_Tree_ID(
            //		Env.getCtx().getAD_Client_ID(), m_mTab.getAD_Table_ID());
        }
        if (this.m_tree != null && AD_Tree_ID > 0)
            this.m_tree.initTree(AD_Tree_ID);
    }
};	//	activate

VIS.GridController.prototype.nodeSelectionChanged = function (e) {
    //	System.out.println("propertyChange");
    //	System.out.println(e);
    if (e == null)
        return;
    var value = e.newValue;
    if (value == null)
        return;
    //log.config(e.propertyName() + "=" + value

    //  We Have a TreeNode
    var nodeID = value;
    this.treeNodeID = nodeID;
    if (this.onDemandTree) {

        if (this.aPanel && this.aPanel.setBusy) {
            this.aPanel.setBusy(true);
        }

        this.gTab.setTreeNodeID(nodeID);

        this.gTab.gridTable.setCurrentPage(1);
        this.gTab.setQuery(null);
        this.navigate(0);

        this.query(0, 0, false, nodeID, this.treeID, this.gTab.getAD_Table_ID());   //  autoSize
        return;
    }

    //  Search all rows for mode id
    var size = this.gTab.getRowCount();
    var row = -1;
    for (var i = 0; i < size; i++) {
        if (this.gTab.getKeyID(i) == nodeID) {
            row = i;
            break;
        }
    }
    if (row == -1) {
        //this.log.log(Level.WARNING, "Tab does not have ID with Node_ID=" + nodeID);
        return;
    }

    //  Navigate to node row
    this.navigate(row);
};   //  pro

/** END*/

/*
  skip row inserting (used by window qiuck serch )  
  -if Automatic new row record set to true
  @param ignore true or false
 */
VIS.GridController.prototype.skipRowInserting = function (ignore) {
    this.skipInserting = ignore;
};

VIS.GridController.prototype.query = function (onlyCurrentDays, maxRows, created, nodeID, treeID, tableID) {
    if (this.aPanel && this.aPanel.setBusy) {
        this.aPanel.setBusy(true);
    }

    if (this.onDemandTree) {
        if (nodeID) {
            this.gTab.setTreeNodeID(nodeID);
        }
        if (treeID) {
            this.gTab.setTreeID(treeID);
        }
        if (tableID) {
            this.gTab.setTreeTable(tableID);
        }
        if (this.aPanel.isSummaryVisible) {
            this.gTab.setShowSummaryNodes(true);
        }
        else {
            this.gTab.setShowSummaryNodes(false);
        }
    }

    var result = this.gTab.prepareQuery(onlyCurrentDays, maxRows, created, false);
};

VIS.GridController.prototype.queryCompleted = function (result) {
    this.vTable.clear();
    this.gTab.clearSelectedRow();
    //
    this.vTable.add(this.gTab.getTableModel().getDataTable());
    this.gTab.getTableModel().setSortModel(this.vTable.getGrid().records);


    if (!this.displayAsIncludedGC) {
        if (this.onRowInserting)
            this.onRowInserting();
        else
            this.checkInsertNewRow();
    }

    //Updated by raghu 
    //date:19-01-2016
    //Change/Update for:Zoom from workflow on home page

    //if (!this.vIncludedGC || this.isIncludedGCVisible || this.isZoomAction || 1==1) {
    this.navigate(this.gTab.getCurrentRow(), !this.gTab.getTableModel().getIsInserting());
    //}
    //else {
    //    this.gTab.currentRow = -1;
    //    this.gTab.fireDataStatusEventOnly();
    //}

    //refresh card view
    if (this.isCardRow)
        this.vCardView.refreshUI(this.getVCardPanel().width());

    //refresh card view
    if (this.isMapRow)
        this.vMapView.refreshUI(this.getVMapPanel().width());

    if (this.aPanel) {
        this.aPanel.setBusy(false);
    }

    this.skipInserting = false; // reset 
    this.dynamicDisplay(-1);
    //if (this.aPanel.$parentWindow.onLoad) {  //Change/Update for:Zoom from workflow on home page
    //    this.aPanel.$parentWindow.onLoad();
    //    this.aPanel.$parentWindow.onLoad = null;
    //}
};

VIS.GridController.prototype.checkInsertNewRow = function () {
    if (this.aPanel == null)
        return false;

    var parentValid = true;
    var lc = this.gTab.getLinkColumnName();
    var lcValue = VIS.context.getWindowContext(this.windowNo, lc);
    if (lc.Length > 0 && lcValue.length == 0) {
        parentValid = false;
    }
    //Set Initial record
    //  Set initial record
    if (this.gTab.getTableModel().getTotalRowCount() == 0) {
        //	Automatically create New Record, if none & tab not RO
        if (!this.gTab.getIsReadOnly() &&
            (VIS.context.getIsAutoNew(this.windowNo)
                || this.gTab.getIsQueryNewRecord()) && parentValid) {
            if (this.gTab.getIsInsertRecord() && !this.skipInserting) {
                this.dataNew(false);
                return true;
            }
            else {
                //aPanel.SetButtons(false, false);
                //aPanel.SetNavigateButtons();
            }
        }
    }
    //reset
    return false;
};
/*
  - Handle Control's Change value Event
*/
VIS.GridController.prototype.vetoablechange = function (e) {

    ////  Get Row/Col Info
    var mTable = this.gTab.getTableModel();
    var row = this.gTab.getCurrentRow();
    var col = mTable.findColumn(e.propertyName);
    ////
    //// gwu: modified to enforce validation even when the new value      null
    mTable.setValueAt(e.newValue, row, col);	//	-> dataStatusChanged -> dynamicDisplay
    ////	Force Callout
    //if (e.getPropertyName().equals("S_ResourceAssignment_ID")) {
    //    //GridField mField = m_mTab.getField(col);
    //    // if ((mField != null) && (mField.getCallout().length() > 0))
    //    //   m_mTab.processFieldChange(mField);     //  Dependencies & Callout
    //}
};

/*
 - handle UI refresh Request 
*/
VIS.GridController.prototype.refreshUI = function (e) {
    this.dataRefreshAll();
};

VIS.GridController.prototype.navigate = function (tRow, force) {

    //  nothing to do
    //console.log(!force);
    if (!force && tRow == this.gTab.getCurrentRow())
        return this.gTab.getCurrentRow();

    //  new position
    var recid = this.gTab.navigate(tRow, true, force);
    if (recid > -1 && !this.rowSetting) {
        this.settingGridSelecton = true;
        this.vTable.select(recid) //select row for Grid
        this.settingGridSelecton = false;
        this.vCardView.navigate(recid, !this.isCardRow);
    }
    else if (recid > -1) {
        this.vCardView.navigate(recid, true);
    }

    if (this.vHeaderPanel)
    {
        this.vHeaderPanel.navigate();
    }
    

    if (recid == -1) {
        this.cancelSel = true;
        return;
    }
    if (recid > -1) {
        this.refreshTabPanelData(this.gTab.getRecord_ID());
    }
    //treeselectin
    //	TreeNavigation - Synchronize 	-- select node in tree
    if (this.m_tree != null)
        this.m_tree.setSelectedNode(this.gTab.getRecord_ID());	//	ignores new (-1)

    this.vTable.scrollInView(tRow);

    if (this.onDemandTree) {

        var recID = this.gTab.getRecord_ID();
        var treID = this.treeID;
        var selfApnel = this.aPanel;
        $.ajax({
            url: VIS.Application.contextUrl + "JsonData/GetTreeNodePath",
            data: { nodeID: recID, treeID: treID },
            success: function (result) {



                var data = JSON.parse(result);
                selfApnel.setStatusLine(data);
            },
            error: function (e) {
                console.log(e);
            }

        });
    }


    return this.gTab.getCurrentRow();
};

VIS.GridController.prototype.navigatePageExact = function (newPage) {

    this.gTab.getTableModel().setCurrentPage(newPage);
    //MRole role = MRole.GetDefault();
    this.query(this.gTab.getOnlyCurrentDays(),
        //role.GetMaxQueryRecords(), false);	//	updated
        0, false, this.treeNodeID, this.treeID, this.gTab.getAD_Table_ID());	//	updated
};

VIS.GridController.prototype.navigatePage = function (newPage) {

    this.gTab.getTableModel().setCurrentPageRelative(newPage);
    //MRole role = MRole.GetDefault();
    this.query(this.gTab.getOnlyCurrentDays(),
        //role.GetMaxQueryRecords(), false);	//	updated
        0, false, this.treeNodeID, this.treeID, this.gTab.getAD_Table_ID());	//	updated
};

VIS.GridController.prototype.navigateRelative = function (rowChange) {
    return this.navigate(this.gTab.getCurrentRow() + rowChange);
};

VIS.GridController.prototype.dataRefresh = function () {
    var record = this.gTab.dataRefresh();
    this.dynamicDisplay(-1);
    window.setTimeout(function (t) { t.switchIncludedGC(); t = null }, 500, this);
};

VIS.GridController.prototype.dataRefreshAll = function () {

    this.gTab.getTableModel().dataRefreshAll();

    //this.vTable.clear();
    //this.vTable.add(this.gTab.getTableModel().getDataTable());
    //this.gTab.getTableModel().setSortModel(this.vTable.getGrid().records);

    //this.setRecord(this.gTab.getTableModel().getTotalRowCount());

    //this.navigate(this.gTab.getCurrentRow(), true);

};

VIS.GridController.prototype.dataSave = function (manualCmd) {

    if (this.m_tree != null) {
        this.gTab.SetSelectedNode(this.m_tree.currentNode);
    }


    var retVal = this.gTab.dataSave(manualCmd);
    if (retVal) {
        //refresh Grid Row
        // this.vTable.refreshRow();
    }
    return retVal;
};

VIS.GridController.prototype.dataNew = function (copy) {
    //this.rowSetting = true;
    //this.switchSingleRow();

    // Default Focus should be set only for first time, not for every datastatus Changed. 
    //So to achieve this, a flag is set on every new click.
    this.isDefaultFocusSet = false;

    this.gTab.dataNew(copy);
    this.dynamicDisplay(-1);
    this.switchIncludedGC();
    if (this.onRowInserted) {
        this.onRowInserted();
    }

    this.isDefaultFocusSet = true;
};

VIS.GridController.prototype.canDeleteRecords = function () {
    var selIndices = this.vTable.getSelection(true);
    var records = this.vTable.getGrid().records;
    var retIndices = [];

    for (var i = 0; i < selIndices.length; i++) {
        var record = records[selIndices[i]];
        if ("ad_client_id" in record) {
            if (!VIS.MRole.getIsClientAccess(record.ad_client_id, true))
                retIndices.push(selIndices[i]);
        }
    }
    return retIndices;
};

VIS.GridController.prototype.dataDelete = function () {
    var retValue = this.gTab.dataDelete(this.vTable.getSelection(true));
    this.refreshTabPanelData(this.gTab.getRecord_ID());
    this.dynamicDisplay(-1);
    return retValue;
};

/**
 *  Row Changed - synchronize with Tree
 *
 *  @param  save    true the row was saved (changed/added), false if the row was deleted
 *  @param  keyID   the ID of the row changed
 */
VIS.GridController.prototype.rowChanged = function (save, keyID) {
    if (this.m_tree == null)
        return;
    if ($.isArray(keyID) && !save) {
        for (var i = 0; i < keyID.length; i++)
            this.m_tree.nodeChanged(save, keyID[i], "", "",
                "", "");
        return;
    }

    if (keyID <= 0)
        return;

    var name = this.gTab.getValue("Name");
    var description = this.gTab.getValue("Description");
    var IsSummary = this.gTab.getValue("IsSummary");
    var summary = IsSummary == true || IsSummary == "Y";
    var imageIndicator = this.gTab.getValue("Action");  //  Menu - Action
    //
    this.m_tree.nodeChanged(save, keyID, name, description,
        summary, imageIndicator);
};  //  rowChanged

VIS.GridController.prototype.dataIgnore = function () {
    this.gTab.dataIgnore();
    this.dynamicDisplay(-1);
    this.switchIncludedGC();
    this.vTable.refreshUndo();
};

VIS.GridController.prototype.dataStatusChanged = function (e) {

    if (this.displayAsIncludedGC) {
        this.enableDisableToolbarItems(true);
        return;
    }
    this.aPanel.dataStatusChanged(e);
    var col = e.getChangedColumn();
    if (!e.getIsChanged() || col < 0)
        return;

    //  Process Callout
    var mField = this.gTab.getField(col);
    if (mField != null) {
        //mField.validateValue();
        if (mField.getCallout().length > 0) {
            var msg = this.gTab.processFieldChange(mField);     //  Dependencies & Callout
            if (msg.length > 0) {
                VIS.ADialog.error(msg);
            }
        }
        else	//	no callout to set dependent fields
        {
            var columnName = mField.getColumnName();
            dependants = this.gTab.getDependantFields(columnName);
            for (var i = 0; i < dependants.length; i++) {
                var dep = dependants[i];
                if (dep == null)
                    continue;
                var lookup = dep.getLookup();
                if (lookup == null)
                    continue;
                //
                var val = lookup.getValidation();
                if (val.indexOf(columnName) != -1)	//	dep is dependent
                {
                    //Object oldValue = lookup.getSelectedItem();
                    //boolean mandatory = dep.isMandatory(false);
                    // lookup.fillComboBox (mandatory, true, true, false);
                    // lookup.setSelectedItemAlways(oldValue);	//	set old value with new rules
                }
            }	//	for all dependent fields
        }
    }
    this.dynamicDisplay(col);	//	 -1 = all
}; //  dataStatusChanged

VIS.GridController.prototype.includeTab = function (gc) {
    var imcludedMTab = gc.getMTab();
    if (this.gTab.getIncluded_Tab_ID() != imcludedMTab.getAD_Tab_ID())
        return false;
    this.vIncludedGC = gc;
    this.vIncludedGC.switchMultiRow();
    return true;
};	//	IncludeTab

VIS.GridController.prototype.switchSingleRow = function (skip) {
    if (this.onlyMultiRow || this.singleRow)
        return;
    this.singleRow = true;
    this.isCardRow = false;
    this.isMapRow = false;

    var p1 = this.getVTablePanel();
    var p = this.getVPanel();

    if (this.isIncludedGCVisible || true) {
        //p1.width("0%");//  css('width:50%');;
        //p.width("99%");//  css('width:50%');;
        p1.hide();
        p.css("display", "block");// .show();
        this.getVCardPanel().hide();
        this.getVMapPanel().hide();

    }
    this.dynamicDisplay(-1);
};

VIS.GridController.prototype.switchMultiRow = function () {
    if (this.singleRow) {

        this.singleRow = false;
        this.isCardRow = false;
        this.isMapRow = false;

        var p1 = this.getVTablePanel();
        this.getVPanel().hide();

        this.getVCardPanel().hide();
        this.getVMapPanel().hide();

        // p1.width(this.displayAsIncludedGC ? '98%' : '97%');
        if (this.isIncludedGCVisible)
            p1.css({ "float": 'right' });
        else p1.css({ "float": '' });

        p1.show();
        p1 = null;
        this.vTable.resize();
        this.vTable.refreshRow();
    }
    //this.switchIncludedGC();
};

VIS.GridController.prototype.switchCardRow = function () {
    if (!this.isCardRow) {

        this.singleRow = false;
        this.isCardRow = true;
        this.isMapRow = false;


        this.getVTablePanel().hide();
        this.getVPanel().hide();

        this.getVMapPanel().hide();

        var p1 = this.getVCardPanel();

        //p1.width(this.displayAsIncludedGC ? '98%' : '97%');
        if (this.isIncludedGCVisible)
            p1.css({ "float": 'right' });
        else p1.css({ "float": '' });

        p1.show();
        this.vCardView.refreshUI(this.getVCardPanel().width());
        p1 = null;
        //this.vTable.resize();
    }
    //this.switchIncludedGC();
};

VIS.GridController.prototype.switchMapRow = function () {
    if (!this.isMapRow) {

        this.singleRow = true;
        this.isCardRow = false;
        this.isMapRow = true;


        this.getVTablePanel().hide();
        this.getVPanel().hide();

        this.getVCardPanel().hide();

        var p1 = this.getVMapPanel();

        //p1.width(this.displayAsIncludedGC ? '98%' : '97%');
        if (this.isIncludedGCVisible)
            p1.css({ "float": 'right' });
        else p1.css({ "float": '' });

        p1.show();
        this.vMapView.refreshUI(this.getVMapPanel().width());
        p1 = null;
        //this.vTable.resize();
    }
    //this.switchIncludedGC();
};




//show hide Included grid
VIS.GridController.prototype.switchIncludedGC = function () {
    if (!this.vIncludedGC || this.displayAsIncludedGC) //has included grid
        return;

    var visible = this.isIncludedGCVisible; //`
    //return;
    //if (this.singleRow) { ///hide if parent gridcontroller in edit mode

    //if (visible) {
    //    this.vIncludedGC.getRoot().hide();
    //    this.vIncludedGC.getRoot().width("100%");
    //    this.getRoot().width("100%");
    //    this.isIncludedGCVisible = false;
    //    // this.displayAsIncludedGC = false;
    //    this.vIncludedGC.displayAsIncludedGC = false;
    //    //this.vIncludedGC.isIncludedGCVisible = false;

    //    //this.toggleTabItems();
    //    //this.vIncludedGC.toggleTabItems();
    //     return;
    //}

    //}

    if (!visible) {
        var tdArea = this.aPanel.getIncludedEmptyArea();
        tdArea.empty();
        var inGc = this.vIncludedGC.getRoot();
        inGc.detach();
        this.vIncludedGC.setUI(true);

        //this.getRoot().css({
        //    'float': 'left',
        //    width: '49%'
        //});

        //inGc.css({
        //    'position': 'absolute',
        //    'left': '100%',
        //    width: '100%'
        //});
        tdArea.append(inGc);

        if (this.gTab.getHasPanel()) {
            tdArea.css("padding", "0px 1px 0px 1px");
        }
        else {
            tdArea.css("padding", "0px 5px 0px 7px");
        }
        tdArea.width(VIS.Application.isMobile ? 250 : 350);

        inGc.show();
        this.vIncludedGC.vTable.activate();
        this.vIncludedGC.vTable.setReadOnly(true);
        //this.vIncludedGC.vTable.resize()
        //setTimeout(function (child) {
        //  child.getRoot().show().animate({

        //width: '49%'
        //    }, 400, function () {
        //        $(this).css('position', 'static');
        //        child.vTable.resize()
        //        child = null;
        //    });
        //}, 500, this.vIncludedGC);

        //this.getRoot().show();
        this.isIncludedGCVisible = true;

        if (!this.singleRow) {
            this.singleRow = true;
            this.switchMultiRow();
        }

        //this.displayAsIncludedGC = false;
        // this.vIncludedGC.isIncludedGCVisible = false;
        this.vIncludedGC.displayAsIncludedGC = true;
        this.vIncludedGC.singleRow = true;
        this.vIncludedGC.switchMultiRow();
        //this.toggleTabItems();
        //this.vIncludedGC.toggleTabItems();

        this.vTable.resize();
        if (this.gTab.getHasPanel()) {
            this.aPanel.setWidth(-1, true);
        }

        //setTimeout(function (that) {
        //    // that.vTable.refresh();

        //    that.vTable.resize();
        //    //that.vIncludedGC.getRoot().slideDown(5000);
        //    //this.vIncludedGC.getRoot().slideDown(1000).show();
        //    that = null;
        //}, 10, this);
    }

    window.setTimeout(function (s) { s.vIncludedGC.query(0, 0, false); s = null; }, 1, this);
};


VIS.GridController.prototype.removeRecord = function (e) {
    var selectedRecs = this.vTable.getSelection(true);
    if (selectedRecs && selectedRecs.length > 0) {

        for (var i = 0; i < selectedRecs.length; i++) {
            this.vTable.grid.unselect(selectedRecs[i] + 1);
            this.vTable.grid.remove(selectedRecs[i] + 1);
        }
    }
    this.navigate(0);
    this.aPanel.setBusy(false);
};

VIS.GridController.prototype.dispose = function () {
    //unwind events
    //this.vTable.getGrid().off('select', this.onTableRowSelect);

    this.gTab.removeDataStatusListener(this.aPanel);
    this.gTab.removeDataStatusListener();
    this.gTab.getTableModel().removeTableModelListener(this.vTable);
    this.gTab.getTableModel().removeCardModelListener(this.vCardView);
    this.gTab.getTableModel().removeRowChangedListener();
    this.gTab.getTableModel().removeQueryCompleteListner();
    this.disposeComponent();
    this.gTab = null;
    this.windowNo = null;
    this.onlyMultiRow = null;
    this.aPanel = null;
    if (this.m_tree)
        this.m_tree.dispose();
    if (this.curTabPanel) {
        this.curTabPanel.dispose();
        this.curTabPanel = null;
    }
    this.m_tree = null;
};

VIS.GridController.prototype.HEADER_HEIGHT = 55;

    //****************** END *****************************//
}(VIS, jQuery));