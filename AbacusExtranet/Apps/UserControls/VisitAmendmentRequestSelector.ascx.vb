Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Extranet.Apps.UserControls.VisitAmendmentRequestSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of Visit Amendments.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [WAQAS] 18/03/2011  D12082
    ''' 	[PaulW]	02/05/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class VisitAmendmentRequestSelector
        Inherits System.Web.UI.UserControl

#Region " Fields "
        Private _pScheduleId As Integer = 0
#End Region

#Region "Properties"
        Private _thePage As BasePage
        Public Property thePage() As BasePage
            Get
                Return _thePage
            End Get
            Set(ByVal value As BasePage)
                _thePage = value
            End Set
        End Property

        Private _providerID As Integer
        Public Property providerID() As Integer
            Get
                Return _providerID
            End Get
            Set(ByVal value As Integer)
                _providerID = value
            End Set
        End Property

        Private _contractID As Integer
        Public Property contractID() As Integer
            Get
                Return _contractID
            End Get
            Set(ByVal value As Integer)
                _contractID = value
            End Set
        End Property

        Private _requestDateFrom As Date
        Public Property requestDateFrom() As Date
            Get
                Return _requestDateFrom
            End Get
            Set(ByVal value As Date)
                _requestDateFrom = value
            End Set
        End Property

        Private _requestDateTo As Date
        Public Property requestDateTo() As Date
            Get
                Return _requestDateTo
            End Get
            Set(ByVal value As Date)
                _requestDateTo = value
            End Set
        End Property

        Private _status As Integer
        Public Property status() As Integer
            Get
                Return _status
            End Get
            Set(ByVal value As Integer)
                _status = value
            End Set
        End Property

        Private _statusDateFrom As Date
        Public Property statusDateFrom() As Date
            Get
                Return _statusDateFrom
            End Get
            Set(ByVal value As Date)
                _statusDateFrom = value
            End Set
        End Property

        Private _statusDateTo As Date
        Public Property statusDateTo() As Date
            Get
                Return _statusDateTo
            End Get
            Set(ByVal value As Date)
                _statusDateTo = value
            End Set
        End Property

        Private _reqCompanyID As Integer
        Public Property reqCompanyID() As Integer
            Get
                Return _reqCompanyID
            End Get
            Set(ByVal value As Integer)
                _reqCompanyID = value
            End Set
        End Property

        Private _reqUserID As Integer
        Public Property reqUserID() As Integer
            Get
                Return _reqUserID
            End Get
            Set(ByVal value As Integer)
                _reqUserID = value
            End Set
        End Property

        Private _serviceUserID As Integer
        Public Property serviceUserID() As Integer
            Get
                Return _serviceUserID
            End Get
            Set(ByVal value As Integer)
                _serviceUserID = value
            End Set
        End Property

        Private _originator As Integer
        Public Property Originator() As Integer
            Get
                Return _originator
            End Get
            Set(ByVal value As Integer)
                _originator = value
            End Set
        End Property

#End Region


        Public Sub InitControl()

            _pScheduleId = Target.Library.Utils.ToInt32(Request.QueryString("pscheduleid"))

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1
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
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/VisitAmendmentRequestSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomProviderInvoice))

            js = String.Format( _
             "currentPage={0};" & _
             "providerID={1};" & _
             "contractID={2};" & _
             "reqDateFrom={3};" & _
             "reqDateTo={4};" & _
             "status={5};" & _
             "statusDateFrom={6};" & _
             "statusDateTo={7};" & _
             "btnViewID=""{8}"";" & _
             "reqByCompanyID={9};" & _
             "reqByUserID={10};" & _
             "clientID={11};" & _
             "pScheduleId={12};", _
             currentPage, _
             providerID, _
             contractID, _
             IIf(Target.Library.Utils.IsDate(requestDateFrom), WebUtils.GetDateAsJavascriptString(requestDateFrom), "null"), _
             IIf(Target.Library.Utils.IsDate(requestDateTo), WebUtils.GetDateAsJavascriptString(requestDateTo), "null"), _
             status, _
             IIf(Target.Library.Utils.IsDate(statusDateFrom), WebUtils.GetDateAsJavascriptString(statusDateFrom), "null"), _
             IIf(Target.Library.Utils.IsDate(statusDateTo), WebUtils.GetDateAsJavascriptString(statusDateTo), "null"), _
             btnView.ClientID, _
             reqCompanyID, _
             reqUserID, _
             serviceUserID, _
             _pScheduleId _
             )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))
            SetReports(requestDateFrom, _
                      requestDateTo, _
                      status, _
                      statusDateFrom, _
                      statusDateTo, _
                      reqCompanyID, _
                      reqUserID, _
                      Originator)
        End Sub

        Private Sub SetReports(ByVal requestDateFrom As Date, ByVal requestDateTo As Date, _
                                ByVal status As Integer, ByVal statusDateFrom As Date, ByVal statusDateTo As Date, _
                                ByVal reqCompanyID As Integer, ByVal reqUserID As Integer, _
                                ByVal Originator As Integer)
            Dim user As WebSecurityUser
            user = SecurityBL.GetCurrentUser()

            With CType(rptPrint, Target.Library.Web.UserControls.IReportsButton)
                .Position = Target.Library.Web.Controls.SearchableMenu.SearchableMenuPosition.TopLeft
                .Enabled = True
                .ButtonText = "Print"
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebReport.VisitAmendmentRequest")
                .Parameters.Add("intUserId", user.ExternalUserID)
                .Parameters.Add("dteRequestDateFrom", IIf(Target.Library.Utils.IsDate(requestDateFrom), requestDateFrom, Nothing))
                .Parameters.Add("dteRequestDateTo", IIf(Target.Library.Utils.IsDate(requestDateTo), requestDateTo, Nothing))
                .Parameters.Add("intStatus", status)
                .Parameters.Add("dteStatusDateFrom", IIf(Target.Library.Utils.IsDate(statusDateFrom), statusDateFrom, Nothing))
                .Parameters.Add("dteStatusDateTo", IIf(Target.Library.Utils.IsDate(statusDateTo), statusDateTo, Nothing))
                If reqCompanyID <> 0 Then .Parameters.Add("intReqByCompanyID", reqCompanyID)
                If reqUserID <> 0 Then .Parameters.Add("intReqByUserID", reqUserID)
                If _pScheduleId <> 0 Then .Parameters.Add("intpScheduleId", _pScheduleId)
                If Originator <> 0 Then .Parameters.Add("intOriginator", Originator)
            End With
        End Sub

        Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
            InitControl()
        End Sub
    End Class

End Namespace