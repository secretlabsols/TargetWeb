<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Default.Master" CodeBehind="SettingsList.aspx.vb" Inherits="Target.Web.Apps.ApplicationSystemSettings.Admin.SettingsList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="StdButtons" Src="~/Library/UserControls/StdButtons.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MPPageOverview" runat="server">
	This screen allows you to list and edit system settings
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MPPageError" runat="server">
	<asp:Literal id="litPageError" runat="server" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="MPContent" runat="server">
    
    <uc1:StdButtons id="stdButtons1" runat="server" />

    <hr />      
    
    <table id="tblFolderAndSettings" class="ContentTable" >
        <tr>
            <td class="ContentTableTree" >
                <div id="divFolderAndSettingsTree" class="FolderAndSettingsTreeViewContainer">
                    <asp:TreeView ID="tvFoldersAndSettings" runat="server" 
                        Target="ifSystemSettings" ShowLines="True" CssClass="FolderAndSettingsTreeView"> 
                        <SelectedNodeStyle CssClass="FolderAndSettingsTreeViewSelectedNode"  />
                        <NodeStyle CssClass="FolderAndSettingsTreeViewNode" />
                    </asp:TreeView>
                </div>             
            </td>            
            <td class="ContentTableEditor">
                <iframe id="ifSystemSettings" name="ifSystemSettings" class="ContentTableEditorIFrame" frameborder="0"></iframe>              
            </td>
        </tr>
    </table>     

</asp:Content>
