﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.1433
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace Apps.Dom.ProviderInvoices

    Partial Public Class CreateBatch

        '''<summary>
        '''grpInvCriteria control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents grpInvCriteria As Global.System.Web.UI.HtmlControls.HtmlGenericControl

        '''<summary>
        '''lblFilterProvider control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblFilterProvider As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''lblFilterContract control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblFilterContract As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''lblFilterClient control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblFilterClient As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''lblFilterStatus control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblFilterStatus As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''lblFilterStatusDates control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblFilterStatusDates As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''lblFilterInvoiceNum control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblFilterInvoiceNum As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''lblFilterInvoiceRef control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblFilterInvoiceRef As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''lblFilterWEDates control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblFilterWEDates As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''lblFilter control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblFilter As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''lblFilters control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblFilters As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''grpBatchTotals control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents grpBatchTotals As Global.System.Web.UI.HtmlControls.HtmlGenericControl

        '''<summary>
        '''lblInvCount control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblInvCount As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''lblInvTotalValue control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblInvTotalValue As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''lblInvTotalVAT control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents lblInvTotalVAT As Global.System.Web.UI.WebControls.Label

        '''<summary>
        '''grpCreateInterface control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents grpCreateInterface As Global.System.Web.UI.HtmlControls.HtmlGenericControl

        '''<summary>
        '''optCreateNow control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents optCreateNow As Global.System.Web.UI.WebControls.RadioButton

        '''<summary>
        '''optDefer control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents optDefer As Global.System.Web.UI.WebControls.RadioButton

        '''<summary>
        '''dteStartDate control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents dteStartDate As Global.Target.Library.Web.Controls.TextBoxEx

        '''<summary>
        '''tmeStartDate control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents tmeStartDate As Global.Target.Library.Web.Controls.TimePicker

        '''<summary>
        '''dtePostingDate control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents dtePostingDate As Global.Target.Library.Web.Controls.TextBoxEx

        '''<summary>
        '''cboPostingYear control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents cboPostingYear As Global.Target.Library.Web.Controls.DropDownListEx

        '''<summary>
        '''cboPeriodNum control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents cboPeriodNum As Global.Target.Library.Web.Controls.DropDownListEx

        '''<summary>
        '''optFullRollback control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents optFullRollback As Global.System.Web.UI.WebControls.RadioButton

        '''<summary>
        '''optPartialRollback control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents optPartialRollback As Global.System.Web.UI.WebControls.RadioButton

        '''<summary>
        '''btnCreate control.
        '''</summary>
        '''<remarks>
        '''Auto-generated field.
        '''To modify move field declaration from designer file to code-behind file.
        '''</remarks>
        Protected WithEvents btnCreate As Global.System.Web.UI.WebControls.Button
    End Class
End Namespace
