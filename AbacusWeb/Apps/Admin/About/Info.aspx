<%@ Page Language="vb" AutoEventWireup="true" CodeBehind="Info.aspx.vb" Inherits="Target.Abacus.Web.Apps.Admin.About.Info"
    EnableEventValidation="false" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %> 

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageOverview" runat="server">
    This screen shows you general information about this application.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">

<style type="text/css">
    table.listTable td { white-space:normal; }
</style>

    <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewstate="false">
			<legend>Application Information</legend>
	         <div id="dvLicences">  
	         	<table cellspacing="0" cellpadding="2" summary="Site Information" width="100%" id="tbSiteInfo" class="listTable">
	            <thead>
                    <asp:PlaceHolder ID="phSiteInfoHeading" runat="server"></asp:PlaceHolder>
                </thead>
	            <tbody>
				    <asp:PlaceHolder ID="phSiteInfo" runat="server"></asp:PlaceHolder>
                </tbody>
                </table>
                 <br />
	         	<table cellspacing="0" cellpadding="2" summary="List of Software Modules" width="100%" id="tbModules" class="listTable">
	            <thead>
                    <asp:PlaceHolder ID="phLicencesHeading" runat="server"></asp:PlaceHolder>
                </thead>
	            <tbody>
				    <asp:PlaceHolder ID="phLicences" runat="server"></asp:PlaceHolder>
                </tbody>
                </table>
                <br />
                <table cellspacing="0" cellpadding="2" summary="Version Information" width="100%" id="tbVersionInfo" class="listTable">
	            <thead>
                    <asp:PlaceHolder ID="phVersionInfoHeading" runat="server"></asp:PlaceHolder>
                </thead>
	            <tbody>
				    <asp:PlaceHolder ID="phVersionInfo" runat="server"></asp:PlaceHolder>
                </tbody>
                </table>
                <br />
                <table cellspacing="0" cellpadding="2" summary="Assembly Information" width="100%" id="tbAssemblyInfo" class="listTable">
	            <thead>
                    <asp:PlaceHolder ID="phAssemblyInfoHeading" runat="server"></asp:PlaceHolder>
                </thead>
	            <tbody>
				    <asp:PlaceHolder ID="phAssemblyInfo" runat="server"></asp:PlaceHolder>
                </tbody>
                </table>
	          </div>
    </fieldset>
    <br />
</asp:Content>