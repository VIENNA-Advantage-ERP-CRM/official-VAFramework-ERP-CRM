; (function (VIS, $) {
    function ARequest(invoker, AD_Table_ID, Record_ID, C_BPartner_ID, iBusy, container) {
        var AD_Window_ID = 232;
        var m_where = '';
        var window = null;
        var tab = null;

        this.getRequests = function (item) {

            var sql = "SELECT COUNT(*) FROM AD_Table WHERE TableName='R_Request'";
            var dr = VIS.DB.executeReader(sql, null);
            while (dr.read()) {
                if (parseInt(dr.getString(0)) == 0) {
                    VIS.ADialog.error('VIS_NotSupported');
                    return;
                }
            }
            m_where = "(AD_Table_ID=" + AD_Table_ID + " AND Record_ID=" + Record_ID + ")";

            if (AD_Table_ID == 114) {// MUser.Table_ID){
                m_where += " OR AD_User_ID=" + Record_ID + " OR SalesRep_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 291) {//MBPartner.Table_ID){
                m_where += " OR C_BPartner_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 259) {// MOrder.Table_ID){
                m_where += " OR C_Order_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 318) {//MInvoice.Table_ID){
                m_where += " OR C_Invoice_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 335) {// MPayment.Table_ID){
                m_where += " OR C_Payment_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 208) {//MProduct.Table_ID){
                m_where += " OR M_Product_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 203) {//MProject.Table_ID){
                m_where += " OR C_Project_ID=" + Record_ID;
            }
            else if (AD_Table_ID == 539) {// MAsset.Table_ID){
                m_where += " OR A_Asset_ID=" + Record_ID;
            }
            sql = "SELECT Processed, COUNT(*) "
                + "FROM R_Request WHERE " + m_where
                + " GROUP BY Processed "
                + "ORDER BY Processed DESC";

            dr = VIS.DB.executeReader(sql, null);


            var inactiveCount = 0;
            var activeCount = 0;

            while (dr.read()) {
                if ("Y" == dr.getString(0))
                    inactiveCount = dr.getString(1);
                else
                    activeCount += dr.getString(1);
            }

            var $root = $("<div>");
            var ul = $('<ul class=vis-apanel-rb-ul>');
            $root.append(ul);
            var li = $("<li data-id='RequestCreate'>");
            li.append(VIS.Msg.getMsg("RequestNew"));
            li.on("click", function (e) {
                createNewRequest(e);
            });
            ul.append(li);
            if (activeCount > 0) {
                li = $("<li data-id='RequestActive'>");
                li.append(VIS.Msg.getMsg("RequestActive"));
                li.on("click", function (e) {
                    activeRequest(e);
                });
                ul.append(li);
            }
            if (inactiveCount > 0) {
                li = $("<li data-id='RequestActive'>");
                li.append(VIS.Msg.getMsg("RequestAll"));
                li.on("click", function (e) {
                    allRequest(e);
                });
                ul.append(li);
            }
            container.w2overlay($root.clone(true), { css: { height: '200px' } });

        };


        var createNewRequest = function (e) {

            e.stopImmediatePropagation();
            //var vm=new VIS.viewManager();
            window = VIS.viewManager.startWindow(AD_Window_ID, null);
            window.onLoad = function () {
                var gc = window.cPanel.curGC;

                gc.onRowInserting = function () {
                    window.cPanel.cmd_new(false);
                };

                gc.onRowInserted = function () {
                    tab = window.cPanel.curTab;
                    tab.setValue("AD_Table_ID", AD_Table_ID);
                    tab.setValue("Record_ID", Record_ID);

                    if (C_BPartner_ID != null && C_BPartner_ID > 0)
                    {
                        tab.setValue("C_BPartner_ID", C_BPartner_ID);
                    }

                    if (AD_Table_ID == 291)// MBPartner.Table_ID)
                        tab.setValue("C_BPartner_ID", Record_ID);
                    else if (AD_Table_ID == 114)//MUser.Table_ID)
                        tab.setValue("AD_User_ID", Record_ID);
                        //
                    else if (AD_Table_ID == 203)// MProject.Table_ID)
                        tab.setValue("C_Project_ID", Record_ID);
                    else if (AD_Table_ID == 539)// MAsset.Table_ID)
                        tab.setValue("A_Asset_ID", Record_ID);

                    else if (AD_Table_ID == 259)
                        tab.setValue("C_Order_ID", Record_ID);
                    else if (AD_Table_ID == 318)//MInvoice.Table_ID)
                        tab.setValue("C_Invoice_ID", Record_ID);
                        //
                    else if (AD_Table_ID == 208)//MProduct.Table_ID)
                        tab.setValue("M_Product_ID", Record_ID);
                    else if (AD_Table_ID == 335)//MPayment.Table_ID)
                        tab.setValue("C_Payment_ID", Record_ID);
                        //
                    else if (AD_Table_ID == 319)// MInOut.Table_ID)
                        tab.setValue("M_InOut_ID", Record_ID);
                };


               // window.cPanel.cmd_new(false);
                //tab = window.cPanel.curTab;


                
            };

            var overlay = $('#w2ui-overlay');
            overlay.hide();
            overlay = null;
            //window = null;
            //tab = null;
        };

        var activeRequest = function (e) {
            e.stopImmediatePropagation();
            var zoomQuery = new VIS.Query();
            zoomQuery.addRestriction("(" + m_where + ") AND Processed='N'");
            VIS.viewManager.startWindow(AD_Window_ID, zoomQuery);
            var overlay = $('#w2ui-overlay');
            overlay.hide();
            overlay = null;
        };

        var allRequest = function (e) {

            e.stopImmediatePropagation();
            var zoomQuery = new VIS.Query();
            zoomQuery.addRestriction(m_where);
            VIS.viewManager.startWindow(AD_Window_ID, zoomQuery);
            var overlay = $('#w2ui-overlay');
            overlay.hide();
            overlay = null;
        };

        

    };
    VIS.ARequest = ARequest;
})(VIS, jQuery);