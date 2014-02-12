
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
	''' Project	 : Target.Abacus.Web
	''' Class	 : Abacus.Web.Apps.UserControls.ClientSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of clients.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MoTahir     16/12/2011  BTI556 - Column size issue in New Creditor Payments  
    '''     MikeVO      13/04/2011  SDS issue #415 - corrected default DateFrom behaviour (reversing SDS issue #345).
    '''     ColinD      23/11/2010  SDS345 Overloaded InitControl with addtional parameter defaultDateFrom
    '''     MoTahir     27/08/2010  D11814 Service User Enquiry
    '''     JohnF       10/02/2009  Replaced NI Number with Address (D11494)
	''' 	[Mikevo]	09/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ClientSelector
        Inherits System.Web.UI.UserControl

#Region " Private Variables"
        Private _hideCreditorRef As Boolean
        Private _hideDebtorRef As Boolean
#End Region

#Region " Private Constants"
        Private Const CONST_ENQUIRY_TYPE_SERVICEUSER As String = "1"
        Private Const CONST_ENQUIRY_TYPE_SERVICEUSERBUDGETYEARS As String = "2"
#End Region

#Region " Properties"
        ''' <summary>
        ''' Gets the base web page.
        ''' </summary>
        ''' <value>The base web page.</value>
        Private ReadOnly Property MyBasePage() As BasePage
            Get
                Return CType(Me.Page, BasePage)
            End Get
        End Property
        ''' <summary>
        ''' Gets or sets the hidden status of the Debtor Ref.
        ''' </summary>
        ''' <value>Boolean representing the hidden state of Debtor Ref</value>
        Public Property HideCreditorRef() As Boolean
            Get
                Return _hideCreditorRef
            End Get
            Set(ByVal value As Boolean)
                _hideCreditorRef = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the hidden status of the Debtor Ref.
        ''' </summary>
        ''' <value>Boolean representing the hidden state of Debtor Ref</value>
        Public Property HideDebtorRef() As Boolean
            Get
                Return _hideDebtorRef
            End Get
            Set(ByVal value As Boolean)
                _hideDebtorRef = value
            End Set
        End Property
#End Region

#Region " Init"
        Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedClientID As Integer)

            InitControl(thePage, selectedClientID, Nothing)

        End Sub

        Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedClientID As Integer, ByVal defaultDateFrom As Nullable(Of DateTime))

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
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
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/ClientSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Clients))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.ClientSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(String.Format( _
              "currentPage={0};ClientSelector_selectedClientID={1};btnViewID='{2}';btnBudgetPeriodsID='{3}';ClientSelector_DefaultDateFrom='{4}';hide_DebtorRef={5};hide_CreditorRef={6};", _
              currentPage, selectedClientID, btnView.ClientID, btnBudgetPeriods.ClientID, defaultDateFromStr, HideDebtorRef.ToString.ToLower, HideCreditorRef.ToString.ToLower) _
             ) _
            )

            MyBasePage.UseJQuery = True

        End Sub
#End Region

#Region " Page Events "
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Dim enquiry As Integer = Target.Library.Utils.ToInt32(Request.QueryString("enquiry"))

            Select Case enquiry
                Case CONST_ENQUIRY_TYPE_SERVICEUSER
                    btnBudgetPeriods.Visible = False
                    btnView.Visible = True
                Case CONST_ENQUIRY_TYPE_SERVICEUSERBUDGETYEARS
                    btnView.Visible = False
                    btnBudgetPeriods.Visible = True
                Case Else
                    btnBudgetPeriods.Visible = False
                    btnView.Visible = False
            End Select
        End Sub
#End Region

    End Class

End Namespace

