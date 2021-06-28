; (function (VIS, $) {


    var baseUrl = VIS.Application.contextUrl;
    var dataSetUrl = baseUrl + "JsonData/JDataSetWithCode";
    //executeDataSet
    var executeDataSet = function (sql, param, callback) {
        var async = callback ? true : false;

        var dataIn = { sql: sql, page: 1, pageSize: 0 };
        if (param) {
            dataIn.param = param;
        }

        var dataSet = null;

        getDataSetJString(dataIn, async, function (jString) {
            dataSet = new VIS.DB.DataSet().toJson(jString);
            if (callback) {
                callback(dataSet);
            }
        });

        return dataSet;
    };

    //DataSet String
    function getDataSetJString(data, async, callback) {
        var result = null;
        //data.sql = VIS.secureEngine.encrypt(data.sql);
        $.ajax({
            url: dataSetUrl,
            type: "POST",
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            async: async,
            data: JSON.stringify(data)
        }).done(function (json) {
            result = json;
            if (callback) {
                callback(json);
            }
            //return result;
        });
        return result;
    };

    function ProcessWrapper(processName, parent) {
        this.parent = parent;
        var $self = this;

        function startProcess() {
            try {
                var sql = "VIS_100";

                var param = [];
                param[0] = new VIS.DB.SqlParam("@processName", processName);

                var ds = executeDataSet(sql, param);
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