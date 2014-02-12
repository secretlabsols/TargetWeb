Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Sds.DPContracts.UserControls

    ''' <summary>
    ''' User Control to List Project Terminations for DP Contracts
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class ucDPContractTerminationProjections
        Inherits System.Web.UI.UserControl

#Region "Fields"

        ' locals
        Private _ParameterDpContractRequiredEndDate As Nullable(Of DateTime) = Nothing
        Private _ParameterDpContractID As Integer = 0

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the base page.
        ''' </summary>
        ''' <value>The base page.</value>
        Private ReadOnly Property BasePage() As BasePage
            Get
                Return CType(Me.Page, BasePage)
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether this instance has required properties.
        ''' </summary>
        ''' <value>
        ''' <c>true</c> if this instance has required properties; otherwise, <c>false</c>.
        ''' </value>
        Private ReadOnly Property HasRequiredParameters() As Boolean
            Get
                Return (ParameterDpContractID > 0 AndAlso ParameterDpContractRequiredEndDate.HasValue)
            End Get
        End Property

#Region "Parameters"

        ''' <summary>
        ''' Gets or sets the parameter dp contract required end date.
        ''' </summary>
        ''' <value>The parameter dp contract required end date.</value>
        Public Property ParameterDpContractRequiredEndDate() As Nullable(Of DateTime)
            Get
                Return _ParameterDpContractRequiredEndDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                _ParameterDpContractRequiredEndDate = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the parameter dp contract ID.
        ''' </summary>
        ''' <value>The parameter dp contract ID.</value>
        Public Property ParameterDpContractID() As Integer
            Get
                Return _ParameterDpContractID
            End Get
            Set(ByVal value As Integer)
                _ParameterDpContractID = value
            End Set
        End Property

#End Region

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            SetupJavaScript()

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim js As New StringBuilder()
            Dim jsOut As String = String.Empty

            If HasRequiredParameters Then
                ' if we have params the tell the ui to fetch data from db

                js.AppendFormat("$(function(){{ DpContractProjectedTerminationsUpdateManager = new DpContractProjectedTerminations.Updater({{ service: new Target.Abacus.Web.Apps.WebSvc.DPContract_class() }}); DpContractProjectedTerminationsUpdateManager.Update({{ dpContractID: {0}, dpContractEndDate: {1} }}); }});", ParameterDpContractID, WebUtils.GetDateAsJavascriptString(ParameterDpContractRequiredEndDate.Value))

            End If

            ' output the scipt...only once using name of control as key
            jsOut = js.ToString()
            BasePage.ClientScript.RegisterStartupScript(Me.GetType(), "ucDPContractTerminationProjections", jsOut, True)

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Setups the java script.
        ''' </summary>
        Private Sub SetupJavaScript()

            ' add in js link for handlers
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Sds/DPContracts/UserControls/ucDPContractTerminationProjections.js"))

            ' add utility js link
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add dialog js
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))

            ' add the date js
            BasePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DPContract))

        End Sub

#End Region

    End Class

End Namespace
