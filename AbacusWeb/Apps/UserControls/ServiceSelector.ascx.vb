﻿Imports System.Collections.Generic
Imports System.Text
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' <summary>
    ''' Class representing a selector tool for DomService records
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   23/11/2010 D11964A - Created
    ''' </history>
    Partial Public Class ServiceSelector
        Inherits System.Web.UI.UserControl

#Region "Fields"

        Private _FilterExcludeIds As List(Of Integer) = Nothing
        Private _FilterIncludeIds As List(Of Integer) = Nothing
        Private _FilterRedundant As TriState = TriState.UseDefault
        Private _FilterSavedId As Integer = 0
        Private _FilterServiceTypeVisitBased As TriState = TriState.UseDefault
        Private _FilterSelectedID As Integer = 0
        Private _FilterExcludeIfNotAssociatedWithServiceType As TriState = TriState.UseDefault
        Private _FilterForUseWithDomiciliaryContracts As TriState = TriState.UseDefault
        Private _FilterIncludeUomIds As List(Of Integer) = Nothing
        Private _FilterIncludeServiceTypeIds As List(Of Integer) = Nothing

        ' constants
        Private Const _SelectorName As String = "ServiceSelector"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the current page.
        ''' </summary>
        ''' <value>The current page.</value>
        Private ReadOnly Property CurrentPage() As Integer
            Get
                Dim page As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
                If page <= 0 Then page = 1
                Return page
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the ids to always exclude.
        ''' </summary>
        ''' <value>The ids to always exclude.</value>
        Public Property FilterExcludeIds() As List(Of Integer)
            Get
                Return _FilterExcludeIds
            End Get
            Set(ByVal value As List(Of Integer))
                _FilterExcludeIds = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the ids to always include (except when filtered out by column filters).
        ''' </summary>
        ''' <value>The ids to always include (except when filtered out by column filters).</value>
        Public Property FilterIncludeIds() As List(Of Integer)
            Get
                Return _FilterIncludeIds
            End Get
            Set(ByVal value As List(Of Integer))
                _FilterIncludeIds = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the uom ids to always include (except when filtered out by column filters).
        ''' </summary>
        ''' <value>The uom ids to always include (except when filtered out by column filters).</value>
        Public Property FilterIncludeUomIds() As List(Of Integer)
            Get
                Return _FilterIncludeUomIds
            End Get
            Set(ByVal value As List(Of Integer))
                _FilterIncludeUomIds = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the filter saved id.
        ''' </summary>
        ''' <value>The filter saved id.</value>
        Public Property FilterSavedId() As Integer
            Get
                Return _FilterSavedId
            End Get
            Set(ByVal value As Integer)
                _FilterSavedId = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the service type ids to always include (except when filtered out by column filters).
        ''' </summary>
        ''' <value>The service type ids to always include (except when filtered out by column filters).</value>
        Public Property FilterIncludeServiceTypeIds() As List(Of Integer)
            Get
                Return _FilterIncludeServiceTypeIds
            End Get
            Set(ByVal value As List(Of Integer))
                _FilterIncludeServiceTypeIds = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether to filter by Redundant flag.
        ''' </summary>
        ''' <value>Whether to filter by Redundant flag.</value>
        Public Property FilterRedundant() As TriState
            Get
                Return _FilterRedundant
            End Get
            Set(ByVal value As TriState)
                _FilterRedundant = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets selected id.
        ''' </summary>
        ''' <value>The selected id.</value>
        Public Property FilterSelectedID() As Integer
            Get
                Return _FilterSelectedID
            End Get
            Set(ByVal value As Integer)
                _FilterSelectedID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether to filter by VisitBased flag.
        ''' </summary>
        ''' <value>Whether to filter by VisitBased flag.</value>
        Public Property FilterServiceTypeVisitBased() As TriState
            Get
                Return _FilterServiceTypeVisitBased
            End Get
            Set(ByVal value As TriState)
                _FilterServiceTypeVisitBased = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the type of the filter exclude if not associated with service.
        ''' </summary>
        ''' <value>The type of the filter exclude if not associated with service.</value>
        Public Property FilterExcludeIfNotAssociatedWithServiceType() As TriState
            Get
                Return _FilterExcludeIfNotAssociatedWithServiceType
            End Get
            Set(ByVal value As TriState)
                _FilterExcludeIfNotAssociatedWithServiceType = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets whether to filter by the ForUseWithDomiciliaryContracts flag.
        ''' </summary>
        ''' <value>Whether to filter by the ForUseWithDomiciliaryContracts flag.</value>
        Public Property FilterForUseWithDomiciliaryContracts() As TriState
            Get
                Return _FilterForUseWithDomiciliaryContracts
            End Get
            Set(ByVal value As TriState)
                _FilterForUseWithDomiciliaryContracts = value
            End Set
        End Property

#End Region

#Region "Methods"

        ''' <summary>
        ''' Gets the tri state object as javascript string.
        ''' </summary>
        ''' <param name="convertee">The value to convert to javascript string</param>
        ''' <returns></returns>
        Private Function GetListOfIntegerAsJavascriptArrayString(ByVal convertee As List(Of Integer)) As String

            Dim sb As New StringBuilder("[")

            If Not convertee Is Nothing AndAlso convertee.Count > 0 Then
                ' if we have some items to convert then do so

                For Each intToAdd As Integer In convertee
                    ' loop each int in the convertee and create a comma seperated array
                    If sb.Length > 1 Then

                        sb.Append(",")

                    End If

                    sb.Append(intToAdd)

                Next

            End If

            sb.Append("]")

            Return sb.ToString()

        End Function

        ''' <summary>
        ''' Gets the tri state object as javascript string.
        ''' </summary>
        ''' <param name="convertee">The value to convert to javascript string</param>
        ''' <returns></returns>
        Private Function GetTriStateAsJavascriptString(ByVal convertee As TriState) As String

            Dim stringToReturn As String = String.Empty

            Select Case convertee

                Case TriState.True

                    stringToReturn = "true"

                Case TriState.False

                    stringToReturn = "false"

                Case Else

                    stringToReturn = "null"

            End Select

            Return stringToReturn

        End Function

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Inits the control.
        ''' </summary>
        ''' <param name="thePage">The page.</param>
        Public Sub InitControl(ByVal thePage As BasePage)

            Dim js As New StringBuilder()

            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))

            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))

            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath(String.Format("AbacusWeb/Apps/UserControls/{0}.js", _SelectorName)))

            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))

            ' add filter properties to page in js format
            js.AppendFormat("{1}_FilterSavedId = {0};", FilterSavedId, _SelectorName)
            js.AppendFormat("{1}_FilterServiceTypeVisitBased = {0};", GetTriStateAsJavascriptString(FilterServiceTypeVisitBased), _SelectorName)
            js.AppendFormat("{1}_FilterRedundant = {0};", GetTriStateAsJavascriptString(FilterRedundant), _SelectorName)
            js.AppendFormat("{1}_FilterExcludeIds = {0};", GetListOfIntegerAsJavascriptArrayString(FilterExcludeIds), _SelectorName)
            js.AppendFormat("{1}_FilterIncludeIds = {0};", GetListOfIntegerAsJavascriptArrayString(FilterIncludeIds), _SelectorName)
            js.AppendFormat("{1}_FilterIncludeServiceTypeIds = {0};", GetListOfIntegerAsJavascriptArrayString(FilterIncludeServiceTypeIds), _SelectorName)
            js.AppendFormat("{1}_FilterIncludeUomIds = {0};", GetListOfIntegerAsJavascriptArrayString(FilterIncludeUomIds), _SelectorName)
            js.AppendFormat("{1}_SelectedID = {0};", FilterSelectedID, _SelectorName)
            js.AppendFormat("{1}_CurrentPage = {0};", CurrentPage, _SelectorName)
            js.AppendFormat("{1}_FilterExcludeIfNotAssociatedWithServiceType = {0};", GetTriStateAsJavascriptString(FilterExcludeIfNotAssociatedWithServiceType), _SelectorName)
            js.AppendFormat("{1}_FilterForUseWithDomiciliaryContracts = {0};", GetTriStateAsJavascriptString(FilterForUseWithDomiciliaryContracts), _SelectorName)
            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
                                                       String.Format("Target.Abacus.Web.Apps.UserControls.{0}.Startup", _SelectorName), _
                                                       Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

#End Region

    End Class

End Namespace