<%@ Control Language="vb" AutoEventWireup="false" Codebehind="SelectorWizard.ascx.vb" Inherits="Target.Web.Library.UserControls.SelectorWizard" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>

<asp:Label ID="lblTitle" runat="server" CssClass="selectorWizardTitle"></asp:Label>
<asp:PlaceHolder ID="phProgress" runat="server"></asp:PlaceHolder>
<div class="clearer"></div>

<div id="divSelections" runat="server" style="margin-top:0.75em;">
    <cc1:CollapsiblePanel id="cpSelections" runat="server" HeaderLinkText="Selections" Expanded="false">
        <ContentTemplate>
            <asp:Panel ID="pnlSavedSelections" runat="server" Visible="false">
	            Saved Selections
	            <asp:DropdownList id="cboSavedSelections" runat="server" visible="true" class="savedWizardSelections"></asp:DropdownList>
	            <br /><br />
	        </asp:Panel>
    	    <asp:PlaceHolder ID="phHeader" runat="server"></asp:PlaceHolder>
    	    <div class="clearer"></div>
        </ContentTemplate>
    </cc1:CollapsiblePanel>
</div>

<asp:PlaceHolder ID="phContent" runat="server"></asp:PlaceHolder>

<div style="float:right">
	<input  type="button" id="btnNewEnquiry" value="New Enquiry" title="Click here to start a new enquiry." runat="server" onclick="SelectorWizard_NewEnquiry();" />
	<input   type="button" id="btnBack"  value="Back" title="Click here to go back to the previous step." runat="server" />
	<input   type="button" id="btnNext"  value="Next" title="Click here to go to the next step." runat="server" />
	<asp:PlaceHolder id="phHiddenEndStepButtons" runat="server"></asp:PlaceHolder>
	<input  type="button" id="btnFinish" value="Finish" title="Click here to see the results." runat="server" />
</div>
<div class="clearer"></div>
