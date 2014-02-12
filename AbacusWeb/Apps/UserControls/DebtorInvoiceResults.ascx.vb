
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DebtorInvoices
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.DebtorInvoiceResults
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of debtor invoices.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     Iftikhar    01/03/2011  D11966 - added button DebtorInvoiceResults_btnNew
    '''     MikeVO      23/07/2009  D11651 - parameter consolidation
    ''' 	JohnF   	01/05/2009	Created (D11604)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class DebtorInvoiceResults
        Inherits System.Web.UI.UserControl

        Public Sub InitControl(ByVal thePage As BasePage, _
                               ByVal qsParser As WizardScreenParameters, _
                               ByVal showCreateBatchButton As Boolean, _
                               ByVal showExcludeButton As Boolean)

            Me.InitControl(thePage, qsParser, showCreateBatchButton, showExcludeButton, False)

        End Sub

        Public Sub InitControl(ByVal thePage As BasePage, _
                               ByVal qsParser As WizardScreenParameters, _
                               ByVal showCreateBatchButton As Boolean, _
                               ByVal showExcludeButton As Boolean, _
                               ByVal viewCreateSDSV2InvoiceJobInNewWindow As Boolean)

            Dim currentPage As Integer = 0

            If Not thePage.Request.QueryString("currentStep") Is DBNull.Value AndAlso thePage.Request.QueryString("currentStep") <> "" Then
                currentPage = Target.Library.Utils.ToInt32(thePage.Request.QueryString("currentStep"))
            Else
                If currentPage <= 0 Then currentPage = 1
            End If

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
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DebtorInvoiceResults.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))

            With CType(DebtorInvoiceResults_btnView, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DebtorInvoiceLayout")
                .Position = SearchableMenu.SearchableMenuPosition.BottomLeft
            End With

            DebtorInvoiceResults_btnExcInc.Visible = showExcludeButton
            DebtorInvoiceResults_btnCreateBatch.Visible = showCreateBatchButton
            DebtorInvoiceResults_btnNew.Visible = SecurityBL.UserHasMenuItem(thePage.DbConnection, _
                                                   SecurityBL.GetCurrentUser().ID, _
                                                   Target.Library.Web.ConstantsManager.GetConstant( _
                                                   "AbacusIntranet.WebNavMenuItem.Income.DebtorInvoices.CreateSDSInvoicingV2Job"), _
                                                   thePage.Settings.CurrentApplicationID)

            js = String.Format( _
                "DebtorInvoiceResults_currentPage={0};DebtorInvoiceResults_clientID='{1}';DebtorInvoiceResults_invRes='{2}';DebtorInvoiceResults_invDom='{3}';DebtorInvoiceResults_invLD='{4}';" _
                    & "DebtorInvoiceResults_invClient='{5}';DebtorInvoiceResults_invTP='{6}';DebtorInvoiceResults_invProp='{7}';DebtorInvoiceResults_invOLA='{8}';DebtorInvoiceResults_invPenColl='{9}';DebtorInvoiceResults_invHomeColl='{10}';" _
                    & "DebtorInvoiceResults_invStd='{11}';DebtorInvoiceResults_invManual='{12}';DebtorInvoiceResults_invSDS='{13}';DebtorInvoiceResults_invActual='{14}';DebtorInvoiceResults_invProvisional='{15}';" _
                    & "DebtorInvoiceResults_invRetracted='{16}';DebtorInvoiceResults_invViaRetract='{17}';DebtorInvoiceResults_invDateFrom='{18}';DebtorInvoiceResults_invDateTo='{19}';DebtorInvoiceResults_invBatchSel='{20}';" _
                    & "DebtorInvoiceResults_invZeroValue='{21}';DebtorInvoiceResults_invExclude='{22}';DebtorInvoiceResults_btnViewID='{23}';divDownloadContainer_ID='{24}';DebtorInvoiceResults_viewCreateSDSV2InvoiceJobInNewWindow={25};", _
                currentPage, qsParser.ClientID, _
                qsParser.InvoiceRes.ToLower, qsParser.InvoiceDom.ToLower, qsParser.InvoiceLD.ToLower, qsParser.InvoiceClient.ToLower, _
                qsParser.InvoiceTP.ToLower, qsParser.InvoiceProp.ToLower, qsParser.InvoiceOLA.ToLower, qsParser.InvoicePenColl.ToLower, _
                qsParser.InvoiceHomeColl.ToLower, qsParser.InvoiceStd.ToLower, qsParser.InvoiceMan.ToLower, qsParser.InvoiceSDS.ToLower, _
                qsParser.InvoiceActual.ToLower, qsParser.InvoiceProvisional.ToLower, qsParser.InvoiceRetracted.ToLower, qsParser.InvoiceViaRetract.ToLower, _
                qsParser.InvoiceDateFrom, qsParser.InvoiceDateTo, qsParser.InvoiceBatchSel, qsParser.InvoiceZeroValue, qsParser.InvoiceExclude, _
                DebtorInvoiceResults_btnView.ClientID, CType(DebtorInvoiceResults_btnView, IReportsButton).DownloadContainer.ClientID, _
                IIf(viewCreateSDSV2InvoiceJobInNewWindow, "true", "false") _
            )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                 "Target.Abacus.Web.Apps.UserControls.DebtorInvoiceResults.Startup", _
                 Target.Library.Web.Utils.WrapClientScript(js))

            CType(DebtorInvoiceResults_btnView, Target.Library.Web.UserControls.IReportsButton).ReportToExcel = False
            CType(DebtorInvoiceResults_btnView, Target.Library.Web.UserControls.IReportsButton).ReportToView = False

        End Sub

    End Class

End Namespace

