<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Edit.aspx.vb" Inherits="Target.Abacus.Web.Apps.ServiceUsers.Enquiry.Edit" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>
<%@ Register TagPrefix="uc2" TagName="ServiceUserHeader" Src="~/AbacusWeb/Apps/UserControls/ServiceUserHeader.ascx" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>

<asp:Content ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to view the details of a service user.
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MPPageError" runat="server">
    <asp:Label ID="lblError" runat="server" CssClass="errorText"></asp:Label>
    <asp:Label ID="lblWarning" runat="server" CssClass="warningText"></asp:Label>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    <uc1:StdButtons id="stdButtons1" runat="server"></uc1:StdButtons>
    <input type="hidden" id="hidSelectedTab" runat="server" /> 
     <fieldset id="fsControls" style="padding:0.5em;" runat="server" EnableViewstate="false">
        <legend>Service User Information</legend>
        <div>
            <uc2:ServiceUserHeader id="serviceUserHeader1" runat="server"></uc2:ServiceUserHeader>
        </div>
        <div class="clearer"></div>
        <br />
        <div>
            <ajaxToolkit:TabContainer runat="server" ID="tabStrip" OnClientActiveTabChanged="ActiveTabChanged" EnableViewState="false">
                <%--Administration Tab--%>
                <ajaxToolkit:TabPanel runat="server" ID="tabAdministration" HeaderText="Administration">
                    <ContentTemplate>
                    <iframe width="100%" height="100%" frameborder="0" id="ifrAdministration">
                      <p>Your browser does not support iframes.</p>
                    </iframe>               
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <%--Addresses Tab--%>
                <ajaxToolkit:TabPanel runat="server" ID="tabAddresses" HeaderText="Addresses">
                    <ContentTemplate> 
                   <iframe width="100%" height="100%" frameborder="0" id="ifrAddresses">
                      <p>Your browser does not support iframes.</p>
                    </iframe>         
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <%--Services Tab--%>
                <ajaxToolkit:TabPanel runat="server" ID="tabServices" HeaderText="Services">
                    <ContentTemplate> 
                    <iframe width="100%" height="100%" frameborder="0" id="ifrServices">
                      <p>Your browser does not support iframes.</p>
                    </iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <%--Finance Tab--%>
                <ajaxToolkit:TabPanel runat="server" ID="tabFinance" HeaderText="Finance">
                    <ContentTemplate> 
                    <iframe width="100%" height="100%" frameborder="0" id="ifrFinance">
                      <p>Your browser does not support iframes.</p>
                    </iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <%--Sds Contributions Tab--%>
                <ajaxToolkit:TabPanel runat="server" ID="tabSdsContribs" HeaderText="SDS Contributions">
                    <ContentTemplate> 
                    <iframe width="100%" height="100%" frameborder="0" id="ifrSdsContribs">
                      <p>Your browser does not support iframes.</p>
                    </iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <%--Notes Tab--%>
                <ajaxToolkit:TabPanel runat="server" ID="tabNotes" HeaderText="Notes">
                    <ContentTemplate> 
                    <iframe width="100%" height="100%" frameborder="0" id="ifrNotes">
                       <p>Your browser does not support iframes.</p>
                    </iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
                <%--Document Tab--%>
                <ajaxToolkit:TabPanel runat="server" ID="tabDocuments" HeaderText="Documents">
                    <ContentTemplate> 
                    <iframe width="100%" height="100%" frameborder="0" id="ifrDocuments">
                      <p>Your browser does not support iframes.</p>
                    </iframe>
                    </ContentTemplate>
                </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>
        </div>
        <br />  
    </fieldset>
    <input type="hidden" id="hidSelectedTabIndex" runat="server" />
</asp:Content>
