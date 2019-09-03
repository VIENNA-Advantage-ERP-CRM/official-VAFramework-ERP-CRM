// allInOne.gif - icon images with different width and heights - disabled states are on the right
bobj.crv.allInOne = (function () {
	var o = new Object();
	o.uri = bobj.crvUri('images/allInOne.gif');
		
	var iconHeight22 = 22;
	var offset = 0;
	
	// Toolbar icons are 22 pixels height and have 3 pixels place holder
	offset += 3;
	offset = o.toolbarExportDy = offset;
	offset = o.toolbarPrintDy = offset + iconHeight22;
	
	offset = o.toolbarRefreshDy = offset + iconHeight22;
	offset = o.toolbarSearchDy = offset + iconHeight22;
	offset = o.toolbarUpDy = offset + iconHeight22;
	// Rest of the images don't need the place holder 
	offset -= 3;

	// These are 22 pixels height
	offset = o.groupTreeToggleDy = offset + iconHeight22;
	offset = o.paramPanelToggleDy = offset + iconHeight22;
	
	// These two are 20 pixels height
	offset = o.toolbarPrevPageDy = offset + iconHeight22; // 22x20
	offset = o.toolbarNextPageDy = offset + 20; // 22x20
	
	offset = o.paramRunDy = offset + 20; // 22x22
	offset = o.paramDataFetchingDy = offset + 22; // 16x16
	offset = o.closePanelDy = offset + 16;	// 8x7
	offset = o.openParameterArrowDy = offset + 7;	// 15x15
	offset = o.plusDy = offset + 15; // 13x12
	offset = o.minusDy = offset + 12;  // 13x12
	offset = o.undoDy = offset + 12;  // 16x16
	
	return o;
})();
