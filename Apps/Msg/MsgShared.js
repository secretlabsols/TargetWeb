
addNamespace("Target.Web.Apps.Msg");

Target.Web.Apps.Msg.Recipient = function()
{
	this.Type = 0;
	this.ID = 0;
	this.Name = "";
}
Target.Web.Apps.Msg.RecipientCollection = function()
{
	this.Recipients = new Array();
	
	this.AddRecipient = function(r) {
		this.Recipients.push(r);
	}
	this.ClearRecipients = function() {
		this.Recipients = new Array();
	}
	this.GetRecipientList = function() {
		var i;
		var result = "";
		for (i=0; i<this.Recipients.length; i++) {
			result += '<span class="msgRecipient">' + this.Recipients[i].Name + ';</span>&nbsp;';
		}
		return result;
	}
	this.Serialize = function () {
		// serialize recipients as <Type>:<ID>;
		// e.g. 1:123;1:456;
		var result = "";
		for (i=0; i<this.Recipients.length; i++) {
			result += this.Recipients[i].Type + ":" + this.Recipients[i].ID + ";";
		}
		return result;
	}
}
function SetUIReadStatus(tr, td, readStatus) {
	tr.className = !readStatus ? "msgUnread" : "";
	td.className = !readStatus ? "msgUnread" : "msgRead";
}