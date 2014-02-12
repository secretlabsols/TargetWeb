<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EditSuspension.aspx.vb" Inherits="Target.Abacus.Web.Apps.Dom.SvcOrders.EditSuspension" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc4" TagName="InPlaceClient" Src="~/AbacusWeb/Apps/InPlaceSelectors/InPlaceClientSelector.ascx" %>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to view, edit or create service order suspensions.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
        <fieldset id="fsHeader" runat="server">
            <legend>Suspension Period</legend>
			<asp:Label AssociatedControlID="client" runat="server" Text="Service User" Width="11.5em"></asp:Label>
			<uc4:InPlaceClient id="client" runat="server" ></uc4:InPlaceClient>
			<br />
	        <cc1:TextBoxEx ID="dteDateFrom"  runat="server" LabelText="Date From"  Format="DateFormat" LabelWidth="12em"
                Width="20em" Required="true" RequiredValidatorErrMsg="Please enter the start date of this suspension."
                ValidationGroup="Save"></cc1:TextBoxEx>
            <br />
            <cc1:TextBoxEx ID="dteDateTo"  runat="server" LabelText="Date To"  Format="DateFormat" LabelWidth="12em"
                Width="20em" ></cc1:TextBoxEx>
            <br />
	        <cc1:DropDownListEx ID="cboSuspensionReason" runat="server" LabelText="Suspension Reason" LabelWidth="12em"
			    Required="true" RequiredValidatorErrMsg="Please select a reason for suspending orders." ValidationGroup="Save"></cc1:DropDownListEx>
		    <br />
		</fieldset>
		<br />
		<fieldset id="fsOrders" runat="server">
		    <table class="listTable" id="tblSuspensions" style="table-layout:fixed;"  cellpadding="2" cellspacing="0" width="100%" summary="List of Service Orders.">
            <caption>List of suspensions.</caption>
            <thead>
                <tr>
                    <th style="width:1.5em;"></th>
                    <th ><div style="display:none;" >ServiceOrderID</div></th>
		            <th style="width:25%;">Provider Name</th>
		            <th style="width:15%;">Contract No</th>
		            <th style="width:15%;">Order Ref</th>
		            <th style="width:15%;">Date From</th>
		            <th style="width:15%;">Date To</th>
		            <th style="width:10%;">Suspended</th>
                </tr>
            </thead>
            <tbody>
		    <asp:Repeater id="rptSuspensions" runat="server">
		        <ItemTemplate>
		                <tr>
			                <td><input name="Radio1" type="radio" onclick="RadioButton_Click()" value="<%#DataBinder.Eval(Container.DataItem, "ID")%>"/></td>
				            <td valign="top" ><div style="display:none;" ><%#DataBinder.Eval(Container.DataItem, "ServiceOrderID")%></div>&nbsp;</td>
				            <td valign="top"><%#DataBinder.Eval(Container.DataItem, "ProviderName")%>&nbsp;</td>
				            <td valign="top"><%#DataBinder.Eval(Container.DataItem, "ContractNo")%>&nbsp;</td>
				                <td valign="top">
                                    <asp:HiddenField ID="dataPartitioned" Value='<%#DataBinder.Eval(Container.DataItem, "DataPartitioned")%>' runat="server"  />
				                    <%# GetOrderReference() %>
				                </td>
				            <td valign="top"><%#CType(DataBinder.Eval(Container.DataItem, "DateFrom"), Date).ToString("dd MMM yyyy")%>&nbsp;</td>
				            <td valign="top"><%#CType(DataBinder.Eval(Container.DataItem, "DateTo"), Date).ToString("dd MMM yyyy")%>&nbsp;</td>
				            <td valign="top"><%#DataBinder.Eval(Container.DataItem, "Suspended")%>&nbsp;</td>
				        </tr>
		        </ItemTemplate>
            </asp:Repeater>
            </tbody>
            </table>
            <input type="button" id="btnView" runat="server" style="float:right;width:7em;" value="View Order" onclick="btnView_Click();" />
            <input type="button" id="btnSuspend" runat="server" style="float:right;width:7em;" value="Re-instate" onclick="if(!window.confirm('Are you sure you wish to ' + btnSuspend.value  + ' this Service Order?')) return false;" />
        </fieldset>
</asp:Content>