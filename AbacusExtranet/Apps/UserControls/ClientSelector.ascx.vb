
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' <summary>
    ''' Enumerates the different mode that the client selctors functions in.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO  12/11/2008  Added ClientsWithDomProviderInvoices and ClientsWithVisitBasedDomProviderInvoices.
    ''' </history>
    Public Enum ClientStepMode
        Clients = 1
        ResidentialClients = 2
        ClientsWithDomSvcOrders = 3
        ClientsWithDomProviderInvoices = 4
        ClientsWithVisitBasedDomProviderInvoices = 5
    End Enum

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.UserControls.ClientSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of clients.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	12/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ClientSelector
        Inherits System.Web.UI.UserControl

        Private _mode As ClientStepMode

        Protected ReadOnly Property Mode() As ClientStepMode
            Get
                Return _mode
            End Get
        End Property


        Private _showServiceOrderWithValidPeriod As Boolean = False
        Public Property showServiceOrderWithValidPeriod() As Boolean
            Get
                Return _showServiceOrderWithValidPeriod
            End Get
            Set(ByVal value As Boolean)
                _showServiceOrderWithValidPeriod = value
            End Set
        End Property


        Public Sub InitControl(ByVal thePage As BasePage, _
                               ByVal establishmentID As Integer, _
                               ByVal contractID As Integer, _
                               ByVal selectedClientID As Integer, _
                                ByVal ceDateFrom As Date, _
                                ByVal ceDateTo As Date, _
                                ByVal viewClientBaseUrl As String, _
                                ByVal mode As ClientStepMode, _
                                Optional ByVal InPlaceClient As Boolean = False)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            Dim pScheduleId As Integer = 0
            pScheduleId = Target.Library.Utils.ToInt32(Request.QueryString("pScheduleId"))

            If currentPage <= 0 Then currentPage = 1

            _mode = mode

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/ClientSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.Clients))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(ClientStepMode))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.UserControls.ClientSelector.Startup", _
                Target.Library.Web.Utils.WrapClientScript(String.Format( _
                    "currentPage={0};ClientStep_clientID={1};ClientStep_establishmentID={2};ClientStep_dateFrom={3};ClientStep_dateTo={4};ClientStep_viewBaseUrl=""{5}"";ClientStep_contractID={6};ClientStep_mode={7};InPlaceClient=""{8}"";pScheduleId={9};showServiceOrderWithValidPeriod={10};", _
                    currentPage, selectedClientID, establishmentID, _
                    IIf(Target.Library.Utils.IsDate(ceDateFrom), WebUtils.GetDateAsJavascriptString(ceDateFrom), "null"), _
                    IIf(Target.Library.Utils.IsDate(ceDateTo), WebUtils.GetDateAsJavascriptString(ceDateTo), "null"), _
                    IIf(viewClientBaseUrl Is Nothing, String.Empty, viewClientBaseUrl), _
                    contractID, Convert.ToInt32(_mode), InPlaceClient.ToString().ToLower(), _
                    pScheduleId, showServiceOrderWithValidPeriod.ToString().ToLower() _
                )) _
            )
        End Sub

        Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedClientID As Integer, ByVal mode As ClientStepMode, _
                               ByVal defaultDateFrom As Nullable(Of DateTime), ByVal InPlaceClient As Boolean)

            _mode = mode

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            Dim pScheduleId As Integer
            pScheduleId = Target.Library.Utils.ToInt32(Request.QueryString("pScheduleId"))

            Dim defaultDateFromStr As String = ""
            If currentPage <= 0 Then currentPage = 1

            If defaultDateFrom.HasValue Then
                ' if we have default date from then parse as string

                defaultDateFromStr = defaultDateFrom.Value.ToString("dd/MM/yyyy")

            End If

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/ClientSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.Clients))
            AjaxPro.Utility.RegisterEnumForAjax(GetType(ClientStepMode))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.ClientSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(String.Format( _
              "currentPage={0};ClientSelector_selectedClientID={1};ClientSelector_DefaultDateFrom=""{2}"";ClientStep_viewBaseUrl=""{3}"";ClientStep_mode={4};InPlaceClient=""{5}"";pScheduleId={6};showServiceOrderWithValidPeriod={7};", _
              currentPage, selectedClientID, defaultDateFromStr, String.Empty, Convert.ToInt32(_mode), InPlaceClient.ToString().ToLower(), pScheduleId, _
              showServiceOrderWithValidPeriod.ToString().ToLower()) _
             ) _
            )

        End Sub

    End Class

End Namespace

