
Imports Target.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

    ''' <summary>
    ''' Popup screen that allows a user to select a sdebtor invoice.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir       15/01/2013 D12092G - Old-style Debtor Invoices In Place Selector
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class DebtorInvoices
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Debtor Invoices")
            Me.JsLinks.Add("DebtorInvoices.js")

            Dim fcID As Integer = Utils.ToInt32(Request.QueryString("id"))

            selector.InitControl(Me, fcID)

        End Sub

    End Class

End Namespace
