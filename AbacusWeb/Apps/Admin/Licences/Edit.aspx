<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="Edit.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.Licences.Edit"
    EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %> 

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen allows you to manage your module licences.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewstate="false">
        <fieldset>
			<legend>Module Licence Information</legend>
	         <div id="dvLicences">  
	         	<table cellspacing="0" cellpadding="0" summary="List of Software Modules" width="100%" id="tbModules" class="listTable">
	            <thead class="a">
                    <asp:PlaceHolder ID="phLicencesHeading" runat="server"></asp:PlaceHolder>
                </thead>
	            <tbody>
				    <asp:PlaceHolder ID="phLicences" runat="server"></asp:PlaceHolder>
                </tbody>
                </table>
	          </div>
        </fieldset>
    </fieldset>
    <br />
</asp:Content>