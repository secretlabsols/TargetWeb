Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.ServiceOrders.NonRes.ServiceOrders

    ''' <summary>
    ''' Web Form used to List and Maintain Service Orders
    ''' </summary>
    ''' <history>
    ''' Paul Wheaver   19/11/2013 D12526E - Created
    ''' </history>
    Partial Class Lister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusExtranet.WebNavMenuItem.ServiceOrders"

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Service Orders")

            Dim resultSettings As New GenericServiceOrdersSettings

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "psResultSettings")
            End With

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.GenericServiceOrder))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.Documents))

            ' add in the jquery library
            Me.UseJQuery = True

            ' add in the jquery ui library for popups and table filtering etc
            Me.UseJqueryUI = True

            '' add in the table filter library 
            '.UseJqueryTableFilter = True

            '' add the table scroller library as we might have large amounts of data
            '.UseJqueryTableScroller = True

            '' add the searchable menu
            '.UseJquerySearchableMenu = True

            ' add the templates pluggin
            Me.UseJqueryTemplates = True

            'add the jquery tooltip library
            Me.UseJqueryTooltip = True

            ' add in page script
            JsLinks.Add("Lister.js")
            JsLinks.Add("ServiceOrderDialog.js")

        End Sub

#End Region

    End Class

End Namespace