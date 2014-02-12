<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ManualEnterVisits.ascx.vb"
    Inherits="Target.Abacus.Extranet.ManualEnterVisits" EnableViewState="true" %>
<%@ Register TagPrefix="cc1" Namespace="Target.Library.Web.Controls" Assembly="Target.Library.Web" %>
<%@ Register Src="ManualEnterVisitGrid.ascx" TagName="ManualEnterVisitGrid" TagPrefix="uc1" %>

    <div style="padding-top:5px;" >
        <div style="float: left;">
           <strong> 
               <asp:Label ID="Label1" runat="server" Text="No. of Visits" Width="10em"></asp:Label>
            </strong>
            <asp:TextBox ID="txtCpNumberOfVisits" ReadOnly="true" runat="server"></asp:TextBox>
        </div>
        <div style="float: left;">
         <strong>
         &nbsp;&nbsp;&nbsp;<asp:Label ID="Label2" runat="server" Text="No. of Hours" Width="10em"></asp:Label>
          </strong>  
            <asp:TextBox ID="txtCpNumberOfHours"  ReadOnly="true"  runat="server"></asp:TextBox>
            <asp:HiddenField ID="txtCpNumberOfHours_Monday" runat="server" />
            <asp:HiddenField ID="txtCpNumberOfHours_Tuesday" runat="server" />
            <asp:HiddenField ID="txtCpNumberOfHours_Wednesday" runat="server" />
            <asp:HiddenField ID="txtCpNumberOfHours_Thursday" runat="server" />
            <asp:HiddenField ID="txtCpNumberOfHours_Friday" runat="server" />
            <asp:HiddenField ID="txtCpNumberOfHours_Saturday" runat="server" />
            <asp:HiddenField ID="txtCpNumberOfHours_Sunday" runat="server" />
        </div>
    </div>
    <div class="clearer">
    </div>
   
    <div style="padding-top:14px;">
        
     <cc1:CollapsiblePanel ID="cpVisitsMonday" runat="server" Width="100%">
            <ContentTemplate>
                 <uc1:ManualEnterVisitGrid ID="VisitGridMonday" runat="server" />
          </ContentTemplate>
        </cc1:CollapsiblePanel>
        <br />  <br />
        <cc1:CollapsiblePanel ID="cpVisitsTuesday" runat="server" Width="100%">
            <ContentTemplate>
                <uc1:ManualEnterVisitGrid ID="VisitGridTuesday" runat="server" />
            </ContentTemplate>
        </cc1:CollapsiblePanel>
        <br />  <br />
        <cc1:CollapsiblePanel ID="cpVisitsWednesday" runat="server" Width="100%">
            <ContentTemplate>
               <uc1:ManualEnterVisitGrid ID="VisitGridWednesday" runat="server" />
            </ContentTemplate>
        </cc1:CollapsiblePanel>
        <br />  <br />
        <cc1:CollapsiblePanel ID="cpVisitsThursday" runat="server" Width="100%">
            <ContentTemplate>
               <uc1:ManualEnterVisitGrid ID="VisitGridThursday" runat="server" />
            </ContentTemplate>
        </cc1:CollapsiblePanel>
        <br />  <br />
        <cc1:CollapsiblePanel ID="cpVisitsFriday" runat="server" Width="100%">
            <ContentTemplate>
                <uc1:ManualEnterVisitGrid ID="VisitGridFriday" runat="server" />
            </ContentTemplate>
        </cc1:CollapsiblePanel>
        <br />  <br />
        <cc1:CollapsiblePanel ID="cpVisitsSaturday" runat="server" Width="100%">
            <ContentTemplate>
                 <uc1:ManualEnterVisitGrid ID="VisitGridSaturday" runat="server" />
            </ContentTemplate>
        </cc1:CollapsiblePanel>
        <br />  <br />
        <cc1:CollapsiblePanel ID="cpVisitsSunday" runat="server" Width="100%">
            <ContentTemplate>
                <uc1:ManualEnterVisitGrid ID="VisitGridSunday" runat="server" />
            </ContentTemplate>
        </cc1:CollapsiblePanel>
    </div>

