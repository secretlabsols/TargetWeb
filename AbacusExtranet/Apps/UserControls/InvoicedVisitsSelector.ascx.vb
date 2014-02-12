
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Extranet.Apps.UserControls.InvoicedVisitsSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of invoiced Visits.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	15/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class InvoicedVisitsSelector
        Inherits System.Web.UI.UserControl

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

        Private _svcUserID As Integer
        Public Property svcUserID() As Integer
            Get
                Return _svcUserID
            End Get
            Set(ByVal value As Integer)
                _svcUserID = value
            End Set
        End Property


        Private _careWorkerID As Integer
        Public Property careWorkerID() As Integer
            Get
                Return _careWorkerID
            End Get
            Set(ByVal value As Integer)
                _careWorkerID = value
            End Set
        End Property

        Private _dateFrom As Date
        Public Property dateFrom() As Date
            Get
                Return _dateFrom
            End Get
            Set(ByVal value As Date)
                _dateFrom = value
            End Set
        End Property

        Private _dateTo As Date
        Public Property dateTo() As Date
            Get
                Return _dateTo
            End Get
            Set(ByVal value As Date)
                _dateTo = value
            End Set
        End Property


     


        Protected Function DisplayCareWorkerColumn() As Boolean
            Return (_careWorkerID = 0 And _svcUserID <> 0)
        End Function

        Protected Function DisplayServiceuserColumn() As Boolean
            Return (_svcUserID = 0 And _careWorkerID <> 0)
        End Function



        Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load


            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1
            Dim js As String

            _careWorkerID = careWorkerID
            _svcUserID = svcUserID

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/InvoicedVisitsSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomProviderInvoice))

            js = String.Format( _
             "currentPage={0};providerID={1};contractID={2};svcUsrID={3};careWorkerID={4};dateFrom={5};dateTo={6};btnViewID=""{7}"";", _
             currentPage, _
             providerID, _
             contractID, _
             svcUserID, _
             careWorkerID, _
             IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
             IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), _
             btnView.ClientID _
             )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub
    End Class

End Namespace

