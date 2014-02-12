Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Abacus.Library.DomProviderInvoice
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports System.Configuration.ConfigurationManager
Imports System.Web.UI
Imports System.Text
Imports System.Collections.Generic
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls

Namespace Apps.Dom.ProviderInvoices

    ''' <summary>
    ''' Screen used to Maintain Dom Provider Invoice Suspensions.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     20/02/2012  A4WA#7207 The timeout period elapsed prior to obtaining a connection from the pool.
    '''     MoTahir     04/03/2010  A4WA#6684
    '''     PaulW       09/06/2009  Created (D11550)
    ''' </history>
    Partial Class Suspensions
        Inherits Target.Web.Apps.BasePage

        Private Const SESSION_NEW_DOM_PROVIDER_INVOICE As String = "NewDomProviderInvoiceData"

        Private _stdBut As StdButtonsBase
        Private _invoiceID As Integer
        Private _estabID As Integer
        Private _dpi As DomProviderInvoiceBL
        Private WithEvents _btnAddComment As Button = New Button
        Private _InvoiceStatus As DomProviderInvoiceStatus

#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

        End Sub

#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryProviderInvoiceSuspensions"), "Domiciliary Provider Invoice Suspensions")
            Dim msg As ErrorMessage
            Dim sysInfo As SystemInfo
            Dim currentUser As WebSecurityUser
            Dim style As New StringBuilder

            style.Append("label.label { float:left; width:14.5em; font-weight:bold; }")
            style.Append("span.label { float:left; width:14.5em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add("Suspensions.js")

            With _stdBut
                .AllowDelete = False
                .AllowEdit = False
                .AllowFind = False
                .AllowNew = False
                .AllowBack = True
            End With

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProviderInvoiceSuspensionReasonType))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Abacus.Library.DomProviderInvoiceSuspensionReasonAutoType))

            _invoiceID = Utils.ToInt32(Request.QueryString("id"))
            _estabID = Utils.ToInt32(Request.QueryString("estabID"))

            sysInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)
            currentUser = SecurityBL.GetCurrentUser()

            _dpi = New DomProviderInvoiceBL( _
                ConnectionStrings("Abacus").ConnectionString, _
                sysInfo.LicenceNo, _
                currentUser.ExternalUsername, _
                currentUser.ExternalUserID, _
                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings).Substring(0, 50) _
            )

            ' fetch
            msg = PrimeDpiClass()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'Populate the screen
            msg = PopulateScreen()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            cboComment.DropDownList.Attributes.Add("onchange", "cboComment_Change();")

        End Sub

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Dim js As String
            Dim hasUnsuspendPermission As Boolean = False
            Dim hasSuspendPermission As Boolean = False
            Dim hasAddCommentPermission As Boolean = False
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            hasSuspendPermission = SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                    currentUser.ID, _
                    Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryProviderInvoiceSuspensions.Suspend"), _
                    Me.Settings.CurrentApplicationID)

            hasUnsuspendPermission = SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                    currentUser.ID, _
                    Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryProviderInvoiceSuspensions.Unsuspend"), _
                    Me.Settings.CurrentApplicationID)

            hasAddCommentPermission = SecurityBL.UserHasMenuItemCommand(Me.DbConnection, _
                    currentUser.ID, _
                    Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryProviderInvoiceSuspensions.AddComment"), _
                    Me.Settings.CurrentApplicationID)

            'In Abacus you cant add a comment to invoices that have been retracted.
            'it has been decided that in intranet that this restriction is to be removed.
            If _InvoiceStatus = DomProviderInvoiceStatus.Paid Then
                optSuspend.Disabled = True
                optUnsuspend.Disabled = True
                optAddComment.Disabled = True
                _btnAddComment.Enabled = False
            Else
                If _InvoiceStatus = DomProviderInvoiceStatus.Suspended Then
                    optAddComment.Disabled = Not hasAddCommentPermission
                    optSuspend.Disabled = True
                Else
                    optSuspend.Disabled = Not hasSuspendPermission
                    optAddComment.Disabled = True
                End If
                If _InvoiceStatus <> DomProviderInvoiceStatus.Suspended Then
                    optUnsuspend.Disabled = True
                Else
                    'if the invoice is suspended we need to enable the Unsuspend comments
                    optUnsuspend.Disabled = Not hasUnsuspendPermission
                End If
            End If

            'Select the first available option button
            If optSuspend.Disabled = False Then
                optSuspend.Checked = True
            ElseIf optAddComment.Disabled = False Then
                optAddComment.Checked = True
            ElseIf optUnsuspend.Disabled = False Then
                optUnsuspend.Checked = True
            End If

            If optAddComment.Disabled And optSuspend.Disabled And optUnsuspend.Disabled Then
                _btnAddComment.Enabled = False
                cboComment.Enabled = False
            End If

            lblSuspend.Disabled = optSuspend.Disabled
            lblUnsuspend.Disabled = optUnsuspend.Disabled
            lblAddComment.Disabled = optAddComment.Disabled

            js = String.Format("btnAddComment_ClientID='{0}';cboComment_ClientID='{1}';", _
                                    _btnAddComment.ClientID, cboComment.ClientID)
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Suspensions.Startup", _
                            Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

#End Region

#Region " PrimeDpiClass "

        Private Function PrimeDpiClass() As ErrorMessage

            Dim msg As ErrorMessage
            Dim newDpiData As NewDomProviderInvoiceData

            If _invoiceID > 0 Then
                ' editing an existing invoice
                ' re-fetch the invoice
                msg = _dpi.Fetch(_invoiceID, _estabID)
            Else
                ' creating a new invoice
                newDpiData = FetchNewDpiData()
                ' re-call AddNew() to re-create the suggested invoice
                msg = _dpi.AddNew(newDpiData, DomProviderInvoiceStyle.SummaryLevel)
            End If

            Return msg

        End Function

#End Region

#Region " FetchNewDpiData "

        Private Function FetchNewDpiData() As NewDomProviderInvoiceData
            Return Session(SESSION_NEW_DOM_PROVIDER_INVOICE)
        End Function

#End Region

#Region " PopulateScreen "

        Private Function PopulateScreen() As ErrorMessage

            Dim msg As ErrorMessage
            Dim suspHistory As List(Of DomProviderInvoiceSuspensionHistory) = Nothing

            ' header tab
            If _dpi.ID > 0 Then
                With _dpi
                    ' existing DPI
                    _InvoiceStatus = .Status
                    lblInvoiceStatus.Text = .StatusDescription
                    lblInvoiceNumber.Text = .InvoiceNumber
                    lblInvoiceDate.Text = .InvoiceDate
                    lblInvoiceValue.Text = Convert.ToDecimal(.InvoiceTotal).ToString("c")
                    lblInvoiceNotes.Text = .Notes

                    'Get the Contract information
                    Dim contract As DomContract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                    msg = contract.Fetch(.DomContractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    lblContractNo.Text = contract.Number

                    'Get the Provider Information
                    Dim provider As Establishment = New Establishment(Me.DbConnection)
                    msg = provider.Fetch(_estabID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    lblProviderName.Text = provider.Name

                    'Get the Service User Information
                    Dim svcUser As ClientDetail = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
                    msg = svcUser.Fetch(.ClientID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    lblServiceUserName.Text = svcUser.Name

                    msg = DomProviderInvoiceBL.GetSuspensionHistory(.SuspensionHistory, suspHistory)
                    rptHistory.DataSource = suspHistory
                    rptHistory.DataBind()

                End With
            End If

            _dpi.Dispose()
            
            msg = New ErrorMessage()
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " cboComment_AfterDropDownControlAdded "

        Private Sub cboComment_AfterDropDownControlAdded(ByVal sender As DropDownListEx) Handles cboComment.AfterDropDownControlAdded
            Dim space As Literal = New Literal

            space.Text = "&nbsp;"
            sender.Controls.Add(space)
            _btnAddComment.Text = "Update"
            sender.Controls.Add(_btnAddComment)
        End Sub

#End Region

#Region "  _btnAddComment_Click "

        Private Sub _btnAddComment_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _btnAddComment.Click
            Dim sysInfo As SystemInfo
            Dim currentUser As WebSecurityUser
            Dim msg As ErrorMessage

            sysInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)
            currentUser = SecurityBL.GetCurrentUser()

            _dpi = New DomProviderInvoiceBL( _
             ConnectionStrings("Abacus").ConnectionString, _
             sysInfo.LicenceNo, _
             currentUser.ExternalUsername, _
              currentUser.ExternalUserID, _
             AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings).Substring(0, 50) _
            )

            ' fetch
            msg = PrimeDpiClass()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If Utils.ToInt32(cboComment.GetPostBackValue) <> 0 Then
                msg = _dpi.AmendSuspension(cboComment.GetPostBackValue)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            _dpi.Dispose()

            Response.Redirect(Request.Url.AbsoluteUri)
            'Response.Redirect(String.Format("Suspensions.aspx?id={0}&estabID={1}&backUrl={2}", _invoiceID, _estabID, Request.QueryString("backUrl")))

        End Sub

#End Region

#Region " Render "

        Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

            ' output postback javascript for add comment button
            Dim options As PostBackOptions = New PostBackOptions(_btnAddComment)
            If Not options Is Nothing Then
                Page.ClientScript.RegisterForEventValidation(options)
            End If
            Page.ClientScript.RegisterForEventValidation(_btnAddComment.UniqueID)
            MyBase.Render(writer)


        End Sub

#End Region

    End Class

End Namespace