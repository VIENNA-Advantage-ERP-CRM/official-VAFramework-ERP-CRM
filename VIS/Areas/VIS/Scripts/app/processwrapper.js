; (function (VIS, $) {
    function ProcessWrapper(processName, parent) {
        this.parent = parent;
        var $self = this;

        function startProcess() {
            try {
                var sql = "SELECT AD_Process_ID,name,CLASSNAME,ENTITYTYPE FROM AD_Process WHERE value='" + processName + "' AND ISACTIVE='Y'";
                var ds = VIS.DB.executeDataSet(sql, null);
                if (ds != null && ds.getTables()[0].getRows().length > 0) {
                    var processId = ds.tables[0].getRow(0).getCell(0);
                    var name = ds.tables[0].getRow(0).getCell(1);
                    var className = ds.tables[0].getRow(0).getCell(2);
                    var type = ds.tables[0].getRow(0).getCell(3);
                    if (processId > 0) {
                        var pp = new VIS.ParameterDialog($self.parent.windowNo, $self);
                        pp.initDialog(processId, name, type, className, false);
                        pp.onCloseMain = function (output) {
                            if (!output) {
                                //process now
                                VIS.ADialog.info('Process Canceled', true, "", "");
                            }
                            else {
                                VIS.ADialog.info('ProcessWait', true, "", "");
                            }
                        }
                        pp.showDialog();
                        pp = null;
                    }
                    else {
                        VIS.ADialog.info('ProcessNotFound', true, "", "");
                    }
                }
            }
            catch (e) {

            }
        };

        startProcess();

        this.lockUI = function (pi) {
            if ($self.lockUI) {
                $self.lockUI(pi);
            }
        }

        this.unlockUI = function (pi) {
            //alert("comlpeted");
            if ($self.unlockUI) {
                $self.unlockUI(pi);
            }
        }

        this.parentcall = function (output) {
            if ($self.parentcall) {
                $self.parentcall(output);
            }
        };

    };

    VIS.ProcessWrapper = ProcessWrapper;

})(VIS, jQuery);