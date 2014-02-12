
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
	''' Project	 : Target.Abacus.Web
	''' Class	 : Abacus.Web.Apps.UserControls.DomContractSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
	'''     User control to encapsulate the listing and selecting of domiciliary contracts.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [MoTahir]   27/11/2009 D11681
	''' 	[Mikevo]	16/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
	Partial Class DomContractSelector
        Inherits System.Web.UI.UserControl

        Private _showServiceUserColumn As Boolean

        Protected ReadOnly Property ShowServiceUserColumn() As Boolean
            Get
                Return _showServiceUserColumn
            End Get
        End Property

        Public Sub InitControl(ByVal thePage As BasePage, ByVal establishmentID As Integer, _
                              ByVal contractType As DomContractType, ByVal contractGroupID As Integer, _
                              ByVal dateFrom As Date, ByVal dateTo As Date, ByVal contractEndReasonID As Integer, _
                              ByVal showNewButton As Boolean, ByVal showViewButton As Boolean, _
                              ByVal showCopyButton As Boolean, ByVal showReinstateButton As Boolean, _
                              ByVal showTerminateButton As Boolean, ByVal showServiceUserColumn As Boolean, _
                              ByVal selectedContractID As Integer, ByVal serviceGroupID As Integer, ByVal serviceGroupClassificationID As Integer, ByVal frameworkType As Nullable(Of FrameworkTypes))

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1
            Dim js As String
            Dim frameworkTypeID As Integer = 0

            btnNew.Visible = showNewButton
            btnView.Visible = showViewButton
            btnCopy.Visible = showCopyButton
            btnReinstate.Visible = showReinstateButton
            btnTerminate.Visible = showTerminateButton
            _showServiceUserColumn = showServiceUserColumn

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DomContractSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))

            If frameworkType.HasValue Then

                frameworkTypeID = Convert.ToInt32(frameworkType.Value)

            End If

            js = String.Format( _
             "currentPage={0};establishmentID={1};contractType='{2}';contractGroupID={3};dateFrom={4};dateTo={5};" & _
             "DomContractSelector_selectedContractID={6};DomContractSelector_btnViewID='{7}';DomContractSelector_btnCopyID='{8}';" & _
             "DomContractSelector_showServiceUserColumn={9};contractEndReasonID={10};DomContractSelector_btnTerminateID='{11}';" & _
             "DomContractSelector_btnReinstateID='{12}';serviceGroupID={13};serviceGroupClassificationID={14}; frameworkTypeID={15};", _
             currentPage, establishmentID, contractType, contractGroupID, _
             IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
             IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), _
             selectedContractID, btnView.ClientID, btnCopy.ClientID, _showServiceUserColumn.ToString().ToLower(), _
             contractEndReasonID, btnTerminate.ClientID, btnReinstate.ClientID, serviceGroupID, serviceGroupClassificationID, frameworkTypeID _
             )

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Web.Apps.UserControls.DomContractSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js) _
            )

        End Sub

	End Class

End Namespace

