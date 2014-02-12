Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Library.Web.UserControls

Namespace Apps.CreditorPayments.Batches
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.CreditorPayments.Batches.UserControls.RemittanceSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of Creditor Remittances.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	16/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class RemittanceSelector
        Inherits System.Web.UI.UserControl

#Region "Fields"

        ' Constants
        Private Const _QsCurrentPage As String = "page"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the current page.
        ''' </summary>
        ''' <value>The current page.</value>
        Private ReadOnly Property CurrentPage() As Integer
            Get
                Dim page As Integer = Target.Library.Utils.ToInt32(Request.QueryString(_QsCurrentPage))
                If page <= 0 Then page = 1
                Return page
            End Get
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Inits the control.
        ''' </summary>
        ''' <param name="thePage">The page.</param>
        ''' <param name="RemittanceSelector_selectedBatchID">The Remittanceselector_selected Batch ID.</param>

        Public Sub InitControl(ByVal thePage As BasePage, _
                               ByVal RemittanceSelector_selectedBatchID As Integer)

            Dim msg As New ErrorMessage()
            Dim js As String

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/CreditorPayments/Batches/UserControls/RemittanceSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.CreditorPayments))

            With CType(btnView, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DirectPaymentRemittanceLayout")
                .ShowButton = True
                .ReportToExcel = False
                .ReportToView = False
                .Position = Target.Library.Web.Controls.SearchableMenu.SearchableMenuPosition.TopRight
            End With

            js = String.Format( _
             "RemittanceSelector_currentPage={0};RemittanceSelector_selectedBatchID={1};RemittanceSelector_btnViewID='{2}';", _
             CurrentPage, RemittanceSelector_selectedBatchID, btnView.ClientID)

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Web.Apps.CreditorPayments.Batches.RemittanceSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js) _
            )

        End Sub

#End Region

    End Class

End Namespace