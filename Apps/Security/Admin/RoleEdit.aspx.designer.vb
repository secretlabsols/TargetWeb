﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.3615
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace Apps.Security.Admin

    Partial Public Class RoleEdit

        '''<summary>
        '''litPageError control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents litPageError As Global.System.Web.UI.WebControls.Literal

        '''<summary>
        '''stdButtons1 control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents stdButtons1 As Global.Target.Web.Library.UserControls.StdButtons

        '''<summary>
        '''cpe control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents cpe As Global.AjaxControlToolkit.CollapsiblePanelExtender

        '''<summary>
        '''pnlReports control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents pnlReports As Global.System.Web.UI.WebControls.Panel

        '''<summary>
        '''lstReports control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lstReports As Global.System.Web.UI.WebControls.ListBox

        '''<summary>
        '''divPermissions control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents divPermissions As Global.System.Web.UI.HtmlControls.HtmlGenericControl

        '''<summary>
        '''ctlPermissions control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents ctlPermissions As Global.Target.Web.UserControls.ReportsButton

        '''<summary>
        '''divMembership control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents divMembership As Global.System.Web.UI.HtmlControls.HtmlGenericControl

        '''<summary>
        '''ctlMembership control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents ctlMembership As Global.Target.Web.UserControls.ReportsButton

        '''<summary>
        '''TabStrip control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents TabStrip As Global.AjaxControlToolkit.TabContainer

        '''<summary>
        '''tabDetails control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents tabDetails As Global.AjaxControlToolkit.TabPanel

        '''<summary>
        '''fsControls control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents fsControls As Global.System.Web.UI.HtmlControls.HtmlGenericControl

        '''<summary>
        '''txtName control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents txtName As Global.Target.Library.Web.Controls.TextBoxEx

        '''<summary>
        '''txtDescription control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents txtDescription As Global.Target.Library.Web.Controls.TextBoxEx

        '''<summary>
        '''roleTree control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents roleTree As Global.System.Web.UI.WebControls.TreeView

        '''<summary>
        '''tabServiceGroups control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents tabServiceGroups As Global.AjaxControlToolkit.TabPanel

        '''<summary>
        '''dlServiceGroups control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents dlServiceGroups As Global.Target.Library.Web.Controls.DualList

        '''<summary>
        '''tabProviderTypes control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents tabProviderTypes As Global.AjaxControlToolkit.TabPanel

        '''<summary>
        '''dlProviderTypes control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents dlProviderTypes As Global.Target.Library.Web.Controls.DualList

        '''<summary>
        '''tabReportCategories control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents tabReportCategories As Global.AjaxControlToolkit.TabPanel

        '''<summary>
        '''dlReportCategories control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents dlReportCategories As Global.Target.Library.Web.Controls.DualList

        '''<summary>
        '''tabJobTypes control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents tabJobTypes As Global.AjaxControlToolkit.TabPanel

        '''<summary>
        '''dlJobTypes control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents dlJobTypes As Global.Target.Library.Web.Controls.DualList

        '''<summary>
        '''tabDocumentTypes control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents tabDocumentTypes As Global.AjaxControlToolkit.TabPanel

        '''<summary>
        '''dlDocumentTypes control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents dlDocumentTypes As Global.Target.Library.Web.Controls.DualList
    End Class
End Namespace