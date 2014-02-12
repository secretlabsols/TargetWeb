<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ProgressForm.aspx.vb" Inherits="Target.Web.Apps.FileStore.ProgressForm" MasterPageFile="~/Popup.master" %>

    <asp:Content ContentPlaceHolderID="MPContent" runat="server">
		<div id="fileUploadProgress" class="fileUploadProgress">
			<span id="title" class="title">Uploading, please wait...</span>
			<br />
			<br />
			<strong>Status: </strong><span id="status">Initialising</span>
			<br />
			<br />
			<strong>Size: </strong><span id="size">N/A</span>
			<br />
			<br />
			<strong>Time: </strong><span id="time">N/A</span>
			<br />
			<br />
		</div>
		<div style="text-align: center">
			<input type="button" id="btnClose" value="Close" disabled="true" onclick="window.close();" />
		</div>
    </asp:Content>