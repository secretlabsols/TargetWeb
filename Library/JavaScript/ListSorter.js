
//*********************************************************************************************************
// LIST SORTER MANAGER
//*********************************************************************************************************
addNamespace("Target.Web.ListSorter");

Target.Web.ListSorter = function(callback) {
	this._callback = callback;
	this.Columns = new Collection();
	this.TableCaption = null;
	this.TableCaptionSort = null;
	this.CurrentColumn = null;
	this.CurrentDirection = null;
	// icons
	this.SortOffIconSrc = SITE_VIRTUAL_ROOT + "images/sortoff.gif";
	this.SortAscIconSrc = SITE_VIRTUAL_ROOT + "images/sortasc.gif";
	this.SortDescIconSrc = SITE_VIRTUAL_ROOT + "images/sortdesc.gif";
}
Target.Web.ListSorter.prototype.AddColumn = function(key, column, sortColumnID) {
	// columm param must be the TH tag of the column
	this.Columns.add(key, new Target.Web.ListSorter.Column(column, sortColumnID));
	// grab the caption for the table that the TH belongs to
	this.TableCaption = column.parentNode.parentNode.parentNode.getElementsByTagName("CAPTION")[0];
	// add sort icon and link
	var columnText = GetInnerText(column);
	var sortIcon = document.createElement("IMG");
	sortIcon.id = "listSorterHeaderIcon";
	sortIcon.src = this.SortOffIconSrc;
	sortIcon.style.width = "11px";
	sortIcon.style.height = "10px";
	this.Columns.item(key).Image = sortIcon;
	var sortLink = document.createElement("A");
	sortLink.href = "javascript:void(0);";
	sortLink.title = "Click here to sort on this column"
	AddOnClick(sortLink, this.Sort, new Array(this, key));
	sortLink.appendChild(sortIcon);
	// append to column header
	column.appendChild(document.createTextNode(" "));
	column.appendChild(sortLink);
}
Target.Web.ListSorter.prototype.Sort = function(evt, args) {
	var listSorter = args[0];
	var key = args[1];
	var column = listSorter.Columns.item(key)
	// setup dialog with the current sort
	var d = new Target.Web.ListSorter.Dialog(listSorter, column);
	// show dialog
	d.Show();
}
Target.Web.ListSorter.prototype.SortCallback = function(evt, args) {
	
	var dialog = args[0];
	var column = args[1];
	var listSorter = args[2];
	var direction = args[3];
	var execCallback = false;
	
	if(column.Name != this.CurrentColumn || this.CurrentDirection != direction) {
		this.CurrentColumn = column.Name;
		this.CurrentDirection = direction;
		column.Direction = direction;
		
		// clear existing icons
		var keys = listSorter.Columns.getKeys();
		for(i=0; i<keys.length; i++) {
			var item = listSorter.Columns.item(keys[i]);
			var imgs = item.Column.getElementsByTagName("IMG");
			for(j=0; j<imgs.length; j++) {
				if(imgs[j].id == "listSorterHeaderIcon") imgs[j].src = listSorter.SortOffIconSrc
			}
		}
		
		switch(direction) {
			case 1:		// Ascending
				column.Image.src = listSorter.SortAscIconSrc;
				column.DirectionText = "ASC";
				execCallback = true;
				break;
			case 2:		// Descending
				column.Image.src = listSorter.SortDescIconSrc;
				column.DirectionText = "DESC";
				execCallback = true;
				break;
			case 0:		// No sorting
				column.Image.src = listSorter.SortOffIconSrc;
				column.DirectionText = "";
				execCallback = true;
				break;
			default:
				alert("Target.Web.ListSorter.Dialog: unknown answer specified.");
				break;
		}
	}
	dialog.Hide();
	if(execCallback) { 
		// apply sort text to the table caption
		if(listSorter.TableCaption) {
			var sortText = "";
			if(!this.TableCaptionSort) {
				this.TableCaptionSort = document.createElement("SPAN");
				this.TableCaptionSort.id = "listSorter";
				this.TableCaptionSort.className = "errorText"
				listSorter.TableCaption.appendChild(this.TableCaptionSort);
			}
			SetInnerText(this.TableCaptionSort, sortText);
			if(column.Direction != 0) {
				sortText = column.Name + " " + column.DirectionText;
				if(sortText.length > 0) SetInnerText(this.TableCaptionSort, " Sorted By: " + sortText + ";");
			}
		}	
		// call page callback function
		listSorter._callback(column);
	}
}

//*********************************************************************************************************
// LIST SORTER COLUMN
//*********************************************************************************************************
addNamespace("Target.Web.ListSorter.Column");

Target.Web.ListSorter.Column = function(column, sortColumnID) {
	this.Column = column;
	this.SortColumnID = sortColumnID;
	this.Name = GetInnerText(column);
	this.Direction = 1;
	this.DirectionText = "ASC";
}
Target.Web.ListSorter.Column.prototype.Image = function(img) {
	this.Image = img;
}
Target.Web.ListSorter.Column.prototype.Direction = function(direction) {
	this.Direction = direction;
}
Target.Web.ListSorter.Column.prototype.DirectionText = function(direction) {
	this.DirectionText = direction;
}

//*********************************************************************************************************
// LIST SORTER DIALOG
//*********************************************************************************************************
addNamespace("Target.Web.ListSorter.Dialog");

Target.Web.ListSorter.Dialog = function(listSorter, column) { 
	this.SetTitle("Sort By \"" + column.Name + "\"");
	
	// descriptive text
	this.SetContentText("To sort these records by <strong>" + column.Name + "</strong>, choose a sort direction below.<br /><br />");
	this.SetWidth("39");
	// sort direction buttons
	this.ClearButtons();
	var btn;
	btn = this.AddButton("Ascending", "", listSorter.SortCallback, new Array(this, column, listSorter, 1));			// 1 = Ascedning
	btn.style.width = "7em";
	btn = this.AddButton("Descending", "", listSorter.SortCallback, new Array(this, column, listSorter, 2));		// 2 = Descending
	btn.style.width = "7em";
	btn = this.AddButton("Clear Sorting", "", listSorter.SortCallback, new Array(this, column, listSorter, 0));		// 0 = Clear
	btn.style.width = "7em";	
}

// inherit from base
Target.Web.ListSorter.Dialog.prototype = new Target.Web.Dialog();


