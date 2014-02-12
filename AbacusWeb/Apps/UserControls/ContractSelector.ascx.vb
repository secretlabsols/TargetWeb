Imports System.Collections.Generic
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.ContractSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of domiciliary contracts.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [PaulW]     29/06/2010  D11795 - SDS, Generic Contracts and Service Orders (Control renamed)
    '''     [MoTahir]   27/11/2009  D11681
    ''' 	[Mikevo]	16/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ContractSelector
        Inherits System.Web.UI.UserControl

        Private _showCreditorColumn As Boolean

        Protected ReadOnly Property ShowCreditorColumn() As Boolean
            Get
                Return _showCreditorColumn
            End Get
        End Property

        Private _ShowProviderColumn As Boolean

        Protected ReadOnly Property ShowProviderColumn() As Boolean
            Get
                Return _ShowProviderColumn
            End Get
        End Property

        Public Sub InitControl(ByVal thePage As BasePage, ByVal establishmentID As Integer, _
                              ByVal contractGroupID As Integer, _
                              ByVal dateFrom As Date, ByVal dateTo As Date, ByVal contractEndReasonID As Integer, _
                              ByVal showNewButton As Boolean, ByVal showViewButton As Boolean, _
                              ByVal showCopyButton As Boolean, ByVal showReinstateButton As Boolean, _
                              ByVal showTerminateButton As Boolean, ByVal showCreditorColumn As Boolean, _
                              ByVal selectedContractID As Integer, ByVal serviceGroupID As Integer, ByVal serviceGroupClassificationID As Integer, ByVal showProviderColumn As Boolean, ByVal genericCreditorId As Integer, ByVal types As List(Of Integer))

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1
            Dim js As String
            Dim jsTypes As String = "[]"

            btnNew.Visible = showNewButton
            btnView.Visible = showViewButton
            btnCopy.Visible = showCopyButton
            btnReinstate.Visible = showReinstateButton
            btnTerminate.Visible = showTerminateButton
            _showCreditorColumn = showCreditorColumn
            _ShowProviderColumn = showProviderColumn

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/ContractSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.CreditorPayments))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Web.Apps.Mru.WebSvc.Mru))

            If Not types Is Nothing AndAlso types.Count > 0 Then
                ' if we have some types

                Dim hasPreviousType As Boolean = False

                jsTypes = "["

                For Each typeToDisplay As Integer In types

                    If hasPreviousType Then
                        ' if previous type then sep with comma

                        jsTypes += ", "

                    End If

                    ' add the current type
                    jsTypes += typeToDisplay.ToString()

                    ' indicate that there are previous types
                    hasPreviousType = True

                Next

                jsTypes += "]"

            End If

            js = String.Format( _
             "currentPage={0};establishmentID={1};contractGroupID={2};dateFrom={3};dateTo={4};" & _
             "ContractSelector_selectedContractID={5};ContractSelector_btnViewID='{6}';ContractSelector_btnCopyID='{7}';" & _
             "ContractSelector_showCreditorColumn={8};contractEndReasonID={9};ContractSelector_btnTerminateID='{10}';" & _
             "ContractSelector_btnReinstateID='{11}';serviceGroupID={12};serviceGroupClassificationID={13};ContractSelector_showProviderColumn={14}; ContractSelector_GenericCreditorID={15};ContractSelector_Types={16};", _
             currentPage, establishmentID, contractGroupID, _
             IIf(Target.Library.Utils.IsDate(dateFrom), WebUtils.GetDateAsJavascriptString(dateFrom), "null"), _
             IIf(Target.Library.Utils.IsDate(dateTo), WebUtils.GetDateAsJavascriptString(dateTo), "null"), _
             selectedContractID, btnView.ClientID, btnCopy.ClientID, _showCreditorColumn.ToString().ToLower(), _
             contractEndReasonID, btnTerminate.ClientID, btnReinstate.ClientID, serviceGroupID, serviceGroupClassificationID, showProviderColumn.ToString().ToLower(), genericCreditorId, jsTypes)

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Web.Apps.UserControls.ContractSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js) _
            )

        End Sub

    End Class

End Namespace

