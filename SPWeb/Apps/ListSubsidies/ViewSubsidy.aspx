<%@ Page Language="vb" AutoEventWireup="false" Codebehind="ViewSubsidy.aspx.vb" Inherits="Target.SP.Web.Apps.ListSubsidies.ViewSubsidy" 
	EnableViewState="True" AspCompat="True" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

	<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
		Displayed below are the details of the selected subsidy. Click on 'End Subsidy' to notify the Supporting People
		Team that the subsidy should be ended.
	</asp:Content>
	<asp:Content ContentPlaceHolderID="MPContent" runat="server">
	    <input type="button" value="Back"  title="Navigates to the previous screen." onclick="javascript:history.back()" />
	    <input type="button" style="width:13.5em;" value="View Primary Service User" title="View the Primary Service Users Details." runat="server" ID="btnPrimaryClient" />
	    <input type="button" style="width:14.5em;" value="View Secondary Service User" title="View the Secondary Service Users Details." runat="server" ID="btnSecondaryClient" />
	    <input type="button" value="End Subsidy" title="End this subsidy." onclick="document.location.href='EndSubsidy.aspx?id=' + GetQSParam(document.location.search, 'id') ;" />
	    <input type="button" id="btnEdit" runat="server" value="Edit" title="Request amendments to the data on this screen." onclick="javascript:document.location.href=AddQSParam(RemoveQSParam(document.location.href, 'editMode'), 'editMode', '1');" NAME="btnEdit"/>
	    <input type="button" id="btnCancel" runat="server" value="Cancel" title="Do not proceed with the amendment request." onclick="javascript:document.location.href=AddQSParam(RemoveQSParam(document.location.href, 'editMode'), 'editMode', '0');" NAME="btnCancel"/>
	    <br />
	    <br />
	    <asp:Label id="lblAmendReq" runat="server" CssClass="warningText" Visible="False">
		    Your new <a href="../../../Apps/AmendReq/ListAmendReq.aspx">amendment request(s)</a> have been submitted.<br /><br />
	    </asp:Label>
	    <label class="label" for="lblProvider">Provider</label> 
	    <ASP:Label id="lblProvider" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblService">Service</label> 
	    <ASP:Label id="lblService" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblPrimaryServiceUser">Primary Service User</label> 
	    <ASP:Label id="lblPrimaryServiceUser" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblSecondaryServiceUser">Secondary Service User</label> 
	    <ASP:Label id="lblSecondaryServiceUser" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblDateFrom">Date From</label> 
	    <ASP:Label id="lblDateFrom" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblDateTo">Date To</label> 
	    <ASP:Label id="lblDateTo" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblEndReason">End Reason</label> 
	    <ASP:Label id="lblEndReason" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblReviewDate">Review Date</label> 
	    <ASP:Label id="lblReviewDate" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <cc1:TextBoxEx id="txtProviderRef" LabelText="Provider Reference" LabelWidth="20em" LabelBold="True"
		    EditableDataItemConstant="SPamendReqDataItemSubsidyProviderRef" 
		    EditableDataFieldConstant="SPamendReqDataFieldSubsidyProviderRefProviderRef"
		    ReadOnlyContentCssClass="content" IsReadOnly="True" runat="server"></cc1:TextBoxEx>
	    <br />	
	    <label class="label" for="lblSubsidy">Subsidy</label> 
	    <ASP:Label id="lblSubsidy" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblVAT">VAT</label> 
	    <ASP:Label id="lblVAT" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblServiceUserContribution">Service User Contribution</label> 
	    <ASP:Label id="lblServiceUserContribution" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblLevel">Level</label> 
	    <ASP:Label id="lblLevel" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblUnitCost">Unit Cost</label> 
	    <ASP:Label id="lblUnitCost" runat="server" CssClass="content"></ASP:Label>
	    <br />
	    <label class="label" for="lblStatus">Status</label> 
	    <ASP:Label id="lblStatus" runat="server" CssClass="content"></ASP:Label>
	    <br /><br />	
	    <fieldset style="width:90%;" id="grpHB" runat="server">
		    <legend>Housing Benefit</legend>
		    <label class="label" for="lblHBReference">HB Reference</label> 
		    <ASP:Label id="lblHBReference" runat="server" CssClass="content1"></ASP:Label>
		    <br />
		    <label class="label" for="lblHBAppliedOn">Applied On</label> 
		    <ASP:Label id="lblHBAppliedOn" runat="server" CssClass="content1"></ASP:Label>
		    <br />
		    <label class="label" for="lblHBStatus">Status</label> 
		    <ASP:Label id="lblHBStatus" runat="server" CssClass="content1"></ASP:Label>
		    <br />
		    <label class="label" for="lblHBStatusDate">Status Date</label> 
		    <ASP:Label id="lblHBStatusDate" runat="server" CssClass="content1"></ASP:Label>
		    <br />
		    <label class="label" for="lblDPWaiver">Data Protection Waiver</label> 
		    <ASP:Label id="lblDPWaiver" runat="server" CssClass="content1"></ASP:Label>
		    <br />
	    </fieldset>
	    <br /><br />
	    <fieldset style="width:90%;" id="grpFC" runat="server">
		    <legend>Fairer Charging</legend>
		    <label class="label" for="lblFCReference">FC Reference</label> 
		    <ASP:Label id="lblFCReference" runat="server" CssClass="content1"></ASP:Label>
		    <br />
		    <label class="label" for="lblFCAppliedOn">Applied On</label> 
		    <ASP:Label id="lblFCAppliedOn" runat="server" CssClass="content1"></ASP:Label>
		    <br />
		    <label class="label" for="lblFCStatus">Status</label> 
		    <ASP:Label id="lblFCStatus" runat="server" CssClass="content1"></ASP:Label>
		    <br />
		    <label class="label" for="lblFCStatusDate">Status Date</label> 
		    <ASP:Label id="lblFCStatusDate" runat="server" CssClass="content1"></ASP:Label>
		    <br />
	    </fieldset>
	    <div id="clearer"></div>
	    <br />
	    <asp:ValidationSummary ID="valSum" runat="server"
            HeaderText="Please correct the following error(s) before proceeding:"
            />
	    <asp:button id="btnSubmit" runat="server" Text="Submit" Title="Click here to submit your amendment requests."></asp:button>
	    <br />
	    <br />
    </asp:Content>