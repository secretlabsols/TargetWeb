<%@ Page Language="vb" AutoEventWireup="false" Codebehind="EditFile.aspx.vb" Inherits="Target.Web.Apps.FileStore.Admin.EditFile" EnableViewState="True" MasterPageFile="~/Popup.master" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="MPContent" runat="server">
	    <script type="text/javascript">
		    var parentWindow = GetParentWindow();
		    var FileUploader_UploadID = GenerateGuid();
		    function SubmitButtonClickedOnce() {
			    GetElement("btnCancel").disabled = true;
			    GetElement("imgBusy").style.visibility = "visible";
		    }
		    function btnSubmit_OnClick(evt) {
    	
			    var results = "";
			    var clientValidate = false;
    			
			    // Page_ClientValidate() is IE only - problem with firefox in that if user chooses a file but doesn't fill in 
			    // the other fields, the file will be submitted to SlickUpload during the round-trip to do the server-side
			    // validation.  Hence, we need custom validation instead.
			    if(typeof(Page_ClientValidate) == "function") {
				    clientValidate = Page_ClientValidate();
			    } else {
				    clientValidate = true;
				    var description = GetElement("txtDescription");
				    if(description.value.trim().length == 0) results += "Please enter the description.\n";
			    }
			    if(results.length > 0) {
				    alert(results);
			    } else if (clientValidate) {
				    Go();
			    }
		    }
		    function Go() {
			    if(GetElement("newFile").value.length > 0) {
				    document.forms[0].action = AddQSParam(RemoveQSParam(document.forms[0].action, "uploadId"), "uploadId", FileUploader_UploadID);
				    OpenFileUploadProgress(FileUploader_UploadID);
				    window.setTimeout('document.forms[0].submit();', 1000);
			    } else {
				    document.forms[0].submit();
			    }
		    }
		    addEvent(window, "unload", DialogUnload);
	    </script>
	
	    <p style="padding:5px;">
		    <asp:Literal id="litPageOverview" runat="server"></asp:Literal>
	    </p>
	    <p>
		    <asp:Label id="lblErrorMsg" runat="server" CssClass="errorText"></asp:Label>
	    </p>
    		
	    <fieldset style="margin-left:7px;width:90%">
		    <legend id="grpLegend" runat="server"></legend>
    		
		    File
		    <a href="javascript:alert(fileHelp);"><img src="../../../Images/info.gif" alt="Help with files and images" border="0" /></a>
		    <input id="newFile" style="width:19.38em" type="file" runat="server" />
		    <input id="btnView" type="button" disabled="true" value="View" runat="server" NAME="btnView"/>
		    <input id="btnDownload" type="button" disabled="true" value="Download" runat="server" NAME="btnDownload"/>
		    <br />
		    <asp:RequiredFieldValidator id="reqNewFile" runat="server" ControlToValidate="newFile"
			    Display="Dynamic" ErrorMessage="Please select a file to upload."></asp:RequiredFieldValidator>
		    <br />
		    Description
		    <asp:TextBox id="txtDescription" runat="server" Width="27.91em" MaxLength="255"></asp:TextBox>
		    <br />
		    <asp:RequiredFieldValidator id="reqDescription" runat="server" ControlToValidate="txtDescription"
			    Display="Dynamic" ErrorMessage="Please enter a description."></asp:RequiredFieldValidator>
		    <br />
		    <img id="imgBusy" alt="Please wait..." style="visibility:hidden" runat="server" />
		    <input type="button" id="btnSubmit" value="Submit" onclick="btnSubmit_OnClick();" />
		    <input id="btnCancel" onclick="GetParentWindow().HideModalDIV();window.parent.close();" type="button" value="Cancel" />
	    </fieldset>
    </asp:Content>