<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DurationClaimedRoundingRules.aspx.vb"
    Inherits="Target.Abacus.Extranet.Apps.Dom.ReferenceData.DurationClaimedRoundingRules" ValidateRequest="false"  %>

<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="InPlaceExternalAccount" Src="~/AbacusExtranet/Apps/InPlaceSelectors/InPlaceExternalAccountSelector.ascx" %>
<asp:content id="Content3" contentplaceholderid="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning"  text="" runat="server" CssClass="warningText"></asp:Label>
</asp:content>
<asp:content id="conContent" contentplaceholderid="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server" />
    <ajaxToolkit:TabContainer runat="server" ID="TabStrip">
            <ajaxToolkit:TabPanel runat="server" ID="tabDetails" HeaderText="Details">
                <ContentTemplate>
                <fieldset id="fsControls" style="padding:0.5em;" runat="server">
                    <cc1:TextBoxEx ID="txtReference" runat="server" LabelText="Reference" LabelWidth="12em" Required="true"
                        RequiredValidatorErrMsg="Please enter a Reference" MaxLength="50" Width="20em"
                         EnableViewState="true" ValidationGroup="Save"></cc1:TextBoxEx>
                    <br />
	                <cc1:TextBoxEx ID="txtDescription" runat="server" LabelText="Description" LabelWidth="12em" Required="true"
                        RequiredValidatorErrMsg="Please enter a Description" MaxLength="255" Width="20em" 
                        EnableViewState="true" ValidationGroup="Save"></cc1:TextBoxEx>
                    <br />
		            <label id="lblExternalAccount" runat="server" style="padding-right:2em; width:9.6em;" >External Account</label>	    
		            <uc2:InPlaceExternalAccount  id="InPlaceExternalAccount1" runat="server" ></uc2:InPlaceExternalAccount>
		            <br /><br />
                 <fieldset>
		    <legend>Transformation rules</legend>
		    <table class="listTable" cellspacing="0" cellpadding="2" summary="List of Duration Claimed Transformation Rules" width="100%">
            <caption>List of Duration Claimed Transformation Rules</caption>
            <thead>
                <tr>
	                <th>Minutes From</th>
	                <th>Minutes To</th>
	                <th>Becomes</th>
	                <th>&nbsp;</th>
                </tr>
            </thead>
            <tbody>
			    <asp:PlaceHolder ID="phRules" runat="server"></asp:PlaceHolder>
            </tbody>
		    </table>
		    <asp:Button id="btnAddRule" runat="server" Text="Add" />
        </fieldset>
                </fieldset>
                </ContentTemplate>
            </ajaxToolkit:TabPanel>
            <ajaxToolkit:TabPanel runat="server" ID="tabRoundingUsedBy" HeaderText="Rounding used by">
            <ContentTemplate>
             <fieldset id="fsControlsRoundingUsedBy" style="padding:0.5em;" runat="server">
                
		    <legend>Rounding used by</legend>
		    <table class="listTable" cellspacing="0" cellpadding="2" summary="List of Non-Residential Contracts to which rounding is to be applied:" width="100%">
            <caption>List of Non-Residential Contracts to which rounding is to be applied:</caption>
            <thead>
                <tr>
	                <th>Contract</th>
	                <th>&nbsp;</th>
                </tr>
            </thead>
            <tbody>
			    <asp:PlaceHolder ID="phRoundingUsedBy" runat="server"></asp:PlaceHolder>
            </tbody>
		    </table>
		    <asp:Button id="btnAddContract" runat="server" Text="Add" />
       
             </fieldset>
            </ContentTemplate>
            </ajaxToolkit:TabPanel>
          </ajaxToolkit:TabContainer>
</asp:content>
