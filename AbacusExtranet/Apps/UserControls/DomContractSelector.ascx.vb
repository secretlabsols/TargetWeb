
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
	''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Extranet.Apps.UserControls.DomContractSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
	'''     User control to encapsulate the listing and selecting of domiciliary contracts.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	30/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class DomContractSelector
        Inherits System.Web.UI.UserControl


#Region " Properties "

        Private _thePage As BasePage
        Public Property thePage() As BasePage
            Get
                Return _thePage
            End Get
            Set(ByVal value As BasePage)
                _thePage = value
            End Set
        End Property

        Private _establishmentID As Integer
        Public Property establishmentID() As Integer
            Get
                Return _establishmentID
            End Get
            Set(ByVal value As Integer)
                _establishmentID = value
            End Set
        End Property


        Private _contractType As DomContractType
        Public Property contractType() As DomContractType
            Get
                Return _contractType
            End Get
            Set(ByVal value As DomContractType)
                _contractType = value
            End Set
        End Property


        Private _contractGroupID As Integer
        Public Property contractGroupID() As Integer
            Get
                Return _contractGroupID
            End Get
            Set(ByVal value As Integer)
                _contractGroupID = value
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


        Private _showNewButton As Boolean
        Public Property showNewButton() As Boolean
            Get
                Return _showNewButton
            End Get
            Set(ByVal value As Boolean)
                _showNewButton = value
            End Set
        End Property


        Private _showViewButton As Boolean
        Public Property showViewButton() As Boolean
            Get
                Return _showViewButton
            End Get
            Set(ByVal value As Boolean)
                _showViewButton = value
            End Set
        End Property


        Private _selectedContractID As Integer
        Public Property selectedContractID() As Integer
            Get
                Return _selectedContractID
            End Get
            Set(ByVal value As Integer)
                _selectedContractID = value
            End Set
        End Property

#End Region

        'Public Sub InitControl(ByVal thePage As BasePage, ByVal establishmentID As Integer, _
        '  ByVal contractType As DomContractType, ByVal contractGroupID As Integer, _
        '  ByVal dateFrom As Date, ByVal dateTo As Date, ByVal showNewButton As Boolean, ByVal showViewButton As Boolean, _
        '  ByVal selectedContractID As Integer)



        'End Sub

        Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1
            Dim js As String

            btnNew.Visible = showNewButton
            btnView.Visible = showViewButton

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/DomContractSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))

            js = String.Format( _
             "currentPage={0};establishmentID={1};contractType=""{2}"";contractGroupID={3};dateFrom={4};dateTo={5};DomContractSelector_selectedContractID={6};btnViewID=""{7}"";", _
             currentPage, establishmentID, contractType, contractGroupID, _
             IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
             IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), _
             selectedContractID, btnView.ClientID _
             )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Extranet.Apps.UserControls.DomContractSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js) _
            )
        End Sub
    End Class

End Namespace

