
//*********************************************************************************************************
// LIST FILTER MANAGER
//*********************************************************************************************************
addNamespace("Target.Web.ListFilter");

Target.Web.ListFilter = function(callback) {
	this._callback = callback;
	this.Columns = new Collection();
	this.TableCaption = null;
	this.TableCaptionFilter = null;
	// icons
	this.FilterOffIconSrc = SITE_VIRTUAL_ROOT + "images/filteroff.gif";
	this.FilterOnIconSrc = SITE_VIRTUAL_ROOT + "images/filteron.gif";
}
Target.Web.ListFilter.prototype.AddColumn = function(key, column, initialFilter) {
	// columm param must be the TH tag of the column
	this.Columns.add(key, new Target.Web.ListFilter.Column(column));
	// grab the caption for the table that the TH belongs to
	this.TableCaption = column.parentNode.parentNode.parentNode.getElementsByTagName("CAPTION")[0];
	// add filter icon and link
	var columnHtml = column.innerHTML;
	var filterOffIcon = document.createElement("IMG");	
	filterOffIcon.src = this.FilterOffIconSrc;
	var filterLink = document.createElement("A");
	filterLink.href = "javascript:void(0);";
	filterLink.title = "Click here to filter on this column"
	AddOnClick(filterLink, this.Filter, new Array(this, key));
	filterLink.innerHTML = columnHtml;
	filterLink.appendChild(filterOffIcon);
	
	// apply initial filter?
	if(initialFilter) {
	    this.Columns.item(key).Filter = initialFilter;
	    this.ApplyFilterCaption(listFilter);
	    filterOffIcon.src = this.FilterOnIconSrc;
	}
	
	// remove existing and then add re-jigged column header
	column.innerHTML = "";
	column.appendChild(filterLink);
}
Target.Web.ListFilter.prototype.Filter = function(evt, args) {
	var listFilter = args[0];
	var key = args[1];
	var column = listFilter.Columns.item(key)
	// setup dialog with the current filter
	var d = new Target.Web.ListFilter.Dialog(listFilter, column);
	// show dialog
	d.Show();
	d.FilterBox.select();
}

Target.Web.ListFilter.prototype.ApplyFilterCaption = function(listFilter) {
        
    if(listFilter.TableCaption) {
		var filterText = "";
		var keys, item;
		if(!this.TableCaptionFilter) {
			this.TableCaptionFilter = document.createElement("SPAN");
			this.TableCaptionFilter.id = "listFilter";
			this.TableCaptionFilter.className = "errorText"
			listFilter.TableCaption.appendChild(this.TableCaptionFilter);
		}
		SetInnerText(this.TableCaptionFilter, filterText);
		keys = listFilter.Columns.getKeys();
		for(i=0; i<keys.length; i++) {
			item = listFilter.Columns.item(keys[i]);
			if(item.Filter.length > 0) {
				filterText += item.Name + " = " + item.Filter + " and ";
			}
		}
		if(filterText.length > 0) SetInnerText(this.TableCaptionFilter, " Filtered By: " + filterText.substr(0, filterText.length - 5) + ";");
	}	
    
}
Target.Web.ListFilter.prototype.SetIcon = function(column, filter) {
column.getElementsByTagName("IMG")[0].src = (filter.length == 0) ? this.FilterOffIconSrc : this.FilterOnIconSrc;
}
Target.Web.ListFilter.prototype.FilterCallback = function(evt, args) {
	
	var dialog = args[0];
	var column = args[1];
	var filter = args[2].value.trim();
	var listFilter = args[3];
	var answer = args[4];
	var execCallback = false;
	
	switch(answer) {
		case 1:		// Apply
			if(filter != column.Filter) {
				column.Filter = filter;
				listFilter.SetIcon(column.Column, filter);
				execCallback = true;
			}
			break;
		case 2:		// Clear
			if(column.Filter.trim().length > 0) {
				column.Filter = "";
				listFilter.SetIcon(column.Column, column.Filter);
				execCallback = true;
			}
			break;
		case 3:		// Cancel
			// do nothing
			break;
		default:
			alert("Target.Web.ListFilter.Dialog: unknown answer specified.");
			break;
	}
	dialog.Hide();
	if(execCallback) { 
		// apply filter text to the table caption
		listFilter.ApplyFilterCaption(listFilter);
		// call page callback function
		listFilter._callback(column);
	}
}

//*********************************************************************************************************
// LIST FILTER COLUMN
//*********************************************************************************************************
addNamespace("Target.Web.ListFilter.Column");

Target.Web.ListFilter.Column = function(column) {
	this.Column = column;
	this.Name = GetInnerText(column);
	this.Filter = "";
}
Target.Web.ListFilter.Column.prototype.SetFilter = function(filter) {
	this.Filter = filter;
}

//*********************************************************************************************************
// LIST FILTER DIALOG
//*********************************************************************************************************
addNamespace("Target.Web.ListFilter.Dialog");

Target.Web.ListFilter.Dialog = function(listFilter, column) {
    var doc = GetDocForModalDIV();
	this.SetTitle("Filter By \"" + column.Name + "\"");
	
	// descriptive text
	this.SetContentText("To filter these records by <strong>" + column.Name + "</strong>, enter a filter below.<br />For example:" +
						"<ul style=\"margin:1em;\">" +
							"<li>To only show " + column.Name + "(s) that <strong>start</strong> with ABC, enter <strong>ABC*</strong></li>" + 
							"<li>To only show " + column.Name + "(s) that <strong>end</strong> with ABC, enter <strong>*ABC</strong></li>" + 
							"<li>To only show " + column.Name + "(s) that <strong>contain</strong> ABC, enter <strong>*ABC*</strong></li>" + 
						"</ul>Filter: ");
	this.SetWidth("39");
	// filter box	
	this.FilterBox = AddInput(this._content, "txtFilter", "text", "", "", column.Filter, null, null, doc);
	this.FilterBox.maxLength = 50;
	// workaround for FireFox bug (see https://bugzilla.mozilla.org/show_bug.cgi?id=236791)
	this.FilterBox.setAttribute("autocomplete","off");
	// buttons
	this.ClearButtons();
	var btnApply = this.AddButton("Apply", "Click here to apply this filter", listFilter.FilterCallback, new Array(this, column, this.FilterBox, listFilter, 1));		// 1 = Apply
	this.SetDefaultButton(this, this.FilterBox, btnApply);
	this.AddButton("Clear", "Click here to clear this filter", listFilter.FilterCallback, new Array(this, column, this.FilterBox, listFilter, 2));		// 2 = Clear
	this.AddButton("Cancel", "Click here to close this dialog", listFilter.FilterCallback, new Array(this, column, this.FilterBox, listFilter, 3));	// 3 = Cancel
}

// inherit from base
Target.Web.ListFilter.Dialog.prototype = new Target.Web.Dialog();


