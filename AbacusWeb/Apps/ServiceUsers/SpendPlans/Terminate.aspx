<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Terminate.aspx.vb" Inherits="Target.Abacus.Web.Apps.ServiceUsers.SpendPlans.Terminate" MasterPageFile="~/Popup.master" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">

        <h3 style="padding : 10px 0px 0px 10px">Terminate Spend Plan</h3>
        <div style="padding : 0px 10px 0px 10px">
            <asp:Label ID="lblError" runat="server" CssClass="errorText" />
            <fieldset id="fsControls" runat="server" style="width:95%;">
                <br />
                <%--Service User--%>
	            <cc1:TextBoxEx ID="txtServiceUser" runat="server" LabelText="Service User" LabelWidth="10.5em" IsReadOnly="true"></cc1:TextBoxEx>
	            <br /><br />
	            <%--Reference--%>
	            <cc1:TextBoxEx ID="txtReference" runat="server" LabelText="Reference" LabelWidth="10.5em" IsReadOnly="true"></cc1:TextBoxEx>
	            <br /><br />
	            <%--Date From--%>
	            <cc1:TextBoxEx ID="txtDateFrom" runat="server" LabelText="Date From" LabelWidth="10.5em" IsReadOnly="true"></cc1:TextBoxEx>
	            <br /><br />
	            <%--Date To--%>
                <cc1:TextBoxEx ID="dteDateTo" runat="server" LabelText="Date To" LabelWidth="10.5em"
	                Required="true" RequiredValidatorErrMsg="Please enter a end date" Format="DateFormat"
	                ValidationGroup="Terminate"></cc1:TextBoxEx>
                <br />
                <%--End Reason--%>
	            <cc1:DropDownListEx ID="cboEndReason" runat="server" LabelText="End Reason" LabelWidth="10.5em"
		            Required="true" RequiredValidatorErrMsg="Please select an end reason" ValidationGroup="Terminate"></cc1:DropDownListEx>
	            <br />
	        </fieldset>
	        <br />
	        <asp:Button id="btnTerminate" style="float:right; width:100px;" runat="server" text="Terminate" ValidationGroup="Terminate" />
	        <input type="button" id="btnCancel" value="Back" style="float:right; width:100px;" onclick="btnCancel_Click();" />
	        

	    </div>
</asp:Content>

