<%@ Page Language="vb" AutoEventWireup="false" Codebehind="EndSubsidy.aspx.vb" Inherits="Target.SP.Web.Apps.ListSubsidies.EndSubsidy" 
	EnableViewState="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">If you wish to notify the Supporting People Team that this subsidy has ended, enter the details below and click "End Subsidy".</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <label class="label" for="lblProvider">Provider</label> 
	    <asp:Label id="lblProvider" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblService">Service</label> 
	    <asp:Label id="lblService" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblPrimaryServiceUser">Primary Service User</label> 
	    <asp:Label id="lblPrimaryServiceUser" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblSecondaryServiceUser">Secondary Service User</label> 
	    <asp:Label id="lblSecondaryServiceUser" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblDateFrom">Date From</label> 
	    <asp:Label id="lblDateFrom" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <label class="label" for="lblSubsidy">Subsidy</label> 
	    <asp:Label id="lblSubsidy" runat="server" CssClass="content"></asp:Label>
	    <br />
	    <cc1:TextBoxEx id="txtEndDate" LabelText="End Date" LabelWidth="20em" LabelBold="True"
		    Required="True" RequiredValidatorErrMsg="Please enter a Date on which the subsidy ended."
		    ReadOnlyContentCssClass="content" IsReadOnly="False" Format="DateFormat" runat="server"></cc1:TextBoxEx>
	    <label for="cboEndReason" class="label" style="width:19em;">End Reason</label><select id="cboEndReason" style="width:20em;" runat="server"></select>
	    <br />
	    <br />
	    <input type="button" value="End Subsidy"  title="End this subsidy." onclick="btnEndSubsidy_Click();" />
	    <input type="button" value="Cancel"  title="Do no end this subsidy and return to the previous screen." onclick="history.back();" />
    </asp:Content>