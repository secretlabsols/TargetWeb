
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.DPContractSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of domiciliary contracts.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     ColinD  04/04/2013  #7835 - fix button availability
    '''     MoTahir 24/10/2012  D12399 Copy Function For Direct Payment Contracts.
    '''     ColinD  01/11/2010  D11801 - SDS Issue 290, fixes to enable showing of contract number hyperlink when showViewButton is true
    '''     ColinD  08/08/2010  D11802 - Added Create Payments Button support
    '''     JohnF   26/07/2010  Initial version (D11801)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class DPContractSelector
        Inherits System.Web.UI.UserControl
#Region " Fields"
        Dim enableCopyButton As Boolean
        Private _CurrentUser As WebSecurityUser = Nothing
        Private js As String = String.Empty
#End Region

#Region " Properties"
        ''' <summary>
        ''' Gets base page.
        ''' </summary>
        ''' <value>Base page.</value>
        Private ReadOnly Property MyBasePage() As BasePage
            Get
                Return CType(Page, BasePage)
            End Get
        End Property
        ''' <summary>
        ''' Gets the current user.
        ''' </summary>
        ''' <value>The current user.</value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _CurrentUser Is Nothing Then
                    _CurrentUser = SecurityBL.GetCurrentUser()
                End If
                Return _CurrentUser
            End Get
        End Property

#End Region

#Region " Event Handlers"
        Public Sub InitControl(ByVal thePage As BasePage, ByVal clientID As Integer, _
                              ByVal budgetHolderID As Integer, _
                              ByVal dateFrom As Date, ByVal dateTo As Date, _
                              ByVal showNewButton As Boolean, ByVal showViewButton As Boolean, _
                              ByVal showReinstateButton As Boolean, _
                              ByVal showTerminateButton As Boolean, _
                              ByVal selectedContractID As Integer, _
                              ByVal showCreatePaymentsButton As Boolean, _
                              ByVal viewContractInNewWindow As Boolean, _
                              ByVal showCopyButton As Boolean, _
                              ByVal isSDS As TriState)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            DPContractSelector_btnNew.Visible = showNewButton
            DPContractSelector_btnView.Visible = showViewButton
            DPContractSelector_btnReinstate.Visible = showReinstateButton
            DPContractSelector_btnTerminate.Visible = showTerminateButton
            DPContractSelector_btnCreatePayments.Visible = showCreatePaymentsButton
            DPContractSelector_btnCopy.Visible = showCopyButton

            'enable or disable the copy button
            enableCopyButton = SecurityBL.UserHasMenuItemCommand(MyBasePage.DbConnection, CurrentUser.ID, _
                               Target.Library.Web.ConstantsManager.GetConstant( _
                               "AbacusIntranet.WebNavMenuItemCommand.DirectPaymentContract.AddNew"), _
                               MyBasePage.Settings.CurrentApplicationID)

            DPContractSelector_btnCopy.Disabled = Not enableCopyButton

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DPContractSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DPContract))

            js = String.Format( _
             "DPContractSelector_currentPage={0};DPContractSelector_clientID={1};DPContractSelector_budgetHolderID={2};DPContractSelector_dateFrom={3};DPContractSelector_dateTo={4};" & _
             "DPContractSelector_selectedContractID={5};DPContractSelector_btnViewID='{6}';" & _
             "DPContractSelector_btnTerminateID='{7}';" & _
             "DPContractSelector_btnReinstateID='{8}';" & _
             "DPContractSelector_btnCreatePaymentsID='{9}';" & _
             "DPContractSelector_canCreate='{10}';" & _
             "DPContractSelector_viewContractInNewWindow={11};" & _
             "DPContractSelector_canView={12};" & _
             "DPContractSelector_listIsSDS={13};" & _
             "DPContractSelector_btnCopyID='{14}';" & _
             "DPContractSelector_enableCopyButton={15};", _
             currentPage, _
             clientID, _
             budgetHolderID, _
             IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
             IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), _
             selectedContractID, _
             "{0}", _
             "{1}", _
             "{2}", _
             "{3}", _
             IIf(showNewButton, "Y", "N"), _
             viewContractInNewWindow.ToString().ToLower(), _
             showViewButton.ToString().ToLower(), _
             Integer.Parse(isSDS), _
             "{4}", _
             enableCopyButton.ToString().ToLower() _
            )

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(sender As Object, e As System.EventArgs) Handles Me.PreRender

            ' set the client ids at the right time...this is a fudge but this control
            ' is being replaced in a future development and as such quick fix is fine
            js = String.Format(js, _
                               DPContractSelector_btnView.ClientID, _
                               DPContractSelector_btnTerminate.ClientID, _
                               DPContractSelector_btnReinstate.ClientID, _
                               DPContractSelector_btnCreatePayments.ClientID, _
                               DPContractSelector_btnCopy.ClientID)

            ' output the js
            MyBasePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                 "Target.Abacus.Web.Apps.UserControls.DPContractSelector.Startup", _
                 Target.Library.Web.Utils.WrapClientScript(js) _
            )

        End Sub

#End Region

    End Class

End Namespace

