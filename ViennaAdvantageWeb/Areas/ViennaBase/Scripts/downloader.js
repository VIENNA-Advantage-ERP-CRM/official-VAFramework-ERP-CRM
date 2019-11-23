$(function () {
    "use strict";

    var Promise = window.Promise;
    if (!Promise) {
        Promise = JSZip.external.Promise;
    }

    /**
     * Reset the message.
     */
    function resetMessage() {
        $("#reportdownloadresult")
        .removeClass()
        .text("");
    }
    /**
     * show a successful message.
     * @param {String} text the text to show.
     */
    function showMessage(text) {
        resetMessage();
        $("#reportdownloadresult")
        .addClass("alert alert-success")
        .text(text);
    }
    /**
     * show an error message.
     * @param {String} text the text to show.
     */
    function showError(text) {
        resetMessage();
        $("#reportdownloadresult")
        .addClass("alert alert-danger")
        .text(text);
    }
    /**
     * Update the progress bar.
     * @param {Integer} percent the current percent
     */
    function updatePercent(percent) {
        $("#reportdownload").removeClass("hide")
        .find(".progress-bar")
        .attr("aria-valuenow", percent)
        .css({
            width: percent + "%"
        }).text(percent.toFixed(0) + "%");
    }

    /**
     * Fetch the content and return the associated promise.
     * @param {String} url the url of the content to fetch.
     * @return {Promise} the promise containing the data.
     */
    function urlToPromise(url) {
        return new Promise(function (resolve, reject) {
            JSZipUtils.getBinaryContent(url, function (err, data) {
                if (err) {
                    reject(err);
                } else {
                    resolve(data);
                }
            });
        });
    }

    if (!JSZip.support.blob) {
        showError("This works only with a recent browser !");
        return;
    }

    function formatDate() {
        var currentdate = new Date();
        var datetime = currentdate.getDate() + "/"
                    + (currentdate.getMonth() + 1) + "/"
                    + currentdate.getFullYear() + "@"
                    + currentdate.getHours() + ":"
                    + currentdate.getMinutes() + ":"
                    + currentdate.getSeconds();


        return datetime;
    }

    var $form = $(document).on("submit", "#download_form", function () {

        resetMessage();

        var zip = new JSZip();

        // find every checked item
        $(this).find(":checked").each(function () {
            var $this = $(this);
            var url = $this.data("url");
            var filename = url.replace(/.*\//g, "");
            var pNo = $this.data("num");
            filename = [filename.slice(0, filename.lastIndexOf('\\') + 1), pNo + '_', filename.slice(filename.lastIndexOf('\\') + 1)].join('');
            zip.file(filename, urlToPromise(url), { binary: true });
        });

        // when everything has been downloaded, we can trigger the dl
        zip.generateAsync({ type: "blob" }, function updateCallback(metadata) {
            var msg = "progression : " + metadata.percent.toFixed(2) + " %";
            if (metadata.currentFile) {
                msg += ", current file = " + metadata.currentFile;
            }
            showMessage(msg);
            updatePercent(metadata.percent | 0);
        })
        .then(function callback(blob) {

            // see FileSaver.js
            saveAs(blob, VIS.Msg.getMsg("Reports") + "_" + formatDate() + ".zip");

            showMessage(VIS.Msg.getMsg("Downloaded"));
            $('#download_form').remove();
        }, function (e) {
            showError(e);
        });

        return false;
    });
});

// vim: set shiftwidth=4 softtabstop=4:
