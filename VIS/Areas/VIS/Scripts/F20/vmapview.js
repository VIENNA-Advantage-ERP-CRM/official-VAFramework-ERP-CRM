; (function (VIS, $) {
/* Map View */

function VMapView(lstCols) {

    var map = null;
    var root = $('<div class="vis-mv-main">');
    var mapDiv = "";
    var busyDiv = "";
    var markers = [];
    var mapProp;
    var bounds = null;
    var cmbLoc = null;
    var cmbDiv = null;
    var self = this;

    var isMapAvail = window.google && google.maps ? true : false;




    function addMarker(location, msg) {
        var marker = new google.maps.Marker({
            position: location,
            animation: google.maps.Animation.DROP,
            map: map,
            title: msg
        });
        marker.info = new google.maps.InfoWindow({
            content: msg
        });

        marker.info.open(map, marker);//(map, this);  

        google.maps.event.addListener(marker, 'click', function (point) {
            //this = marker  
            this.info.open(map, this);//(map, this);  
        });
        markers.push(marker);
    };

    function addMarkerWithTimeout(location, msg, timeout) {
        window.setTimeout(function () {
            addMarker(location, msg);
        }
            , timeout);
    };


    // Sets the map on all markers in the array.
    function setAllMap(map) {
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(map);
            google.maps.event.clearListeners(markers[i], 'click');
            markers[i].info = null;
        }
    };
    // Removes the markers from the map, but keeps them in the array.
    function clearMarkers() {
        setAllMap(null);
    };

    // Shows any markers currently in the array.
    function showMarkers() {
        setAllMap(map);
    };

    // Deletes all markers in the array by removing references to them.
    function deleteMarkers() {
        clearMarkers();
        markers = [];
    };

    function removeMap() {
        deleteMarkers();
        //mapDiv.remove();
        map = null;
    };

    function renderMap() {
        if (!map) {
            map = new google.maps.Map(mapDiv[0], mapProp);
            fillCombo();


        }
    };

    function fillCombo() {
        var html = '';
        var f = self.mapFields;
        if (f.length > 1) {

            for (var i = 0; i < f.length; i++) {
                html += '<option value=' + f[i].getColumnName() + ' >' + f[i].getHeader() + '</option>';
            }
            cmbLoc.html(html);
            cmbLoc[0].selectedIndex = 0;
            bindEvent();
        }
        else {
            cmbDiv.hide();
            mapDiv.css('top', '0');
        }
    };

    function initialize() {

        mapDiv = $('<div class="vis-mv-map">');
        busyDiv = $('<div class="vis-apanel-busy vis-full-height">').hide();
        cmbDiv = $('<div class="vis-mv-header"> <select class="vis-mv-select vis-pull-right" /> </div>');
        cmbLoc = cmbDiv.find(".vis-mv-select");
        root.append(cmbDiv).append(mapDiv).append(busyDiv);
        if (isMapAvail) {
            mapProp = {
                center: new google.maps.LatLng(26, 76),
                zoom: 4,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
        }
    };

    initialize();

    function bindEvent() {
        cmbLoc.on("change", function (e) {
            self.setBusy(true);
            self.curIndex = this.selectedIndex;
            self.setMapData(self.mapcols[self.curIndex]);
        });
    };

    this.setBusy = function (busy) {
        if (busy)
            busyDiv.show();
        else
            busyDiv.hide();
    };

    this.getRoot = function () {
        return root;
    };

    this.sizeChanged = function (h, w) {

    };

    this.setMapData = function (lstLatLng) {
        if (!isMapAvail)
            return;

        renderMap();
        deleteMarkers();
        bounds = null;
        bounds = new google.maps.LatLngBounds();


        google.maps.event.trigger(map, 'resize');
        // window.setTimeout(function () {
        var len = lstLatLng.length;
        for (var i = 0; i < len; i++) {
            if (!lstLatLng[i].Latitude || !lstLatLng[i].Longitude)
                continue;
            var ll = null;
            try {

                ll = new google.maps.LatLng(Number(lstLatLng[i].Latitude), Number(lstLatLng[i].Longitude));
                addMarkerWithTimeout(ll, lstLatLng[i].msg, 1 * 100);

                //addMarker(ll, lstLatLng[i].msg);
                bounds.extend(ll);
                map.fitBounds(bounds);
            }
            catch (e) {
                console.log(e);
            }
        }
        map.fitBounds(bounds);
        self.setBusy(false);
        //}, 10);
    };

    this.dc = function () {
        removeMap();
        root.remove();

        this.cols = this.gc = this.aPanel = this.mapcols = null;
        this.mapFields = null;
        this.curIndex = 0;

        this.getRoot = null;
        this.dc = null;
    };

};

VMapView.prototype.setupMapView = function (aPanel, GC, mTab, mapContainer, vMapId) {

    this.mapFields = [];


    var cols = mTab.getMapColumns();

    for (var i = 0; i < cols.length; i++) {
        var f = mTab.getField(cols[i]);
        if (f)
            this.mapFields.push(f);
    }
    this.cols = cols;

    this.gc = GC;
    this.aPanel = aPanel;
    this.mapcols = {};
    this.curIndex = 0;

    mapContainer.append(this.getRoot());
};

VMapView.prototype.refreshUI = function (width) {

    var records = this.gc.getSelectedRows();
    var len = records.length;
    if (records.length < 1 || this.cols.length < 1)
        return;
    var mapcols = [[]];

    for (var i = 0; i < this.cols.length; i++) {
        var colName = this.cols[i];
        var l = this.mapFields[i].getLookup();
        var locIds = [];
        for (var j = 0; j < len; j++) {
            var lid = records[j][colName.toLowerCase()];
            if (lid) {
                var ll = l.getLatLng(lid);
                if (ll) {
                    ll.msg = l.getDisplay(lid);
                    locIds.push(ll);
                }
            }
        }
        this.mapcols[i] = locIds;
    }
    this.setMapData(this.mapcols[this.curIndex]);
};

VMapView.prototype.dispose = function () {
    this.dc();
    };


    VIS.VMapView = VMapView;

}(VIS, jQuery));