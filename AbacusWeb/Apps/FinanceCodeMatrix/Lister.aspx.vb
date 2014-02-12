Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Web.Apps
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports Target.Abacus.Library.Results.Messages
Imports Target.Abacus.Library.Results.Messages.SearcherSettings
Imports Target.Abacus.Library.Results.Messages.SearcherSettings.Items
Imports Target.Abacus.Library.Selectors
Imports Target.Abacus.Library.Selectors.Messages
Imports Target.Abacus.Library.Selectors.Utilities
Imports Target.Library
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections
Imports WebUtils = Target.Library.Web
Imports Target.Abacus.Library.FinanceCodes


Namespace Apps.FinanceCodeMatrix
    ''' <summary>
    ''' Page used to host finance code matrix selecltor control.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JohnF   15/10/2013 Reworked #8158 with user-friendly message (#8250)
    ''' Motahir 29/08/2013 A8158 Unfriendly message if option for matrix is False
    ''' MoTahir 19/06/2013 D12487 - Finance Code Componenet Matrix - Reference Data.
    ''' </history>
    Partial Public Class Lister
        Inherits BasePage


#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.Commitments.ReferenceDate.FinanceCodeMatrix"
        Private Const CONST_MATRIX_ACCESS_DENIED As String = "System setting for Finance Coding – Derive from matrix is not set which bars access to the Finance Code Matrix."

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Finance Code Matrix")

            Dim resultSettings As New FinanceCodeMatrixSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "fcmResultSettings")
            End With

            ' add in page script
            JsLinks.Add("Lister.js")

            If (Not Me.IsPostBack) Then
                ViewState("PreviousPage") = Request.UrlReferrer
            End If
        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Sub Page_PreRenderComplete(sender As Object, e As System.EventArgs) Handles Me.PreRenderComplete
            '++ Get the value for the system setting which governs whether access to this screen is allowed..
            Dim msg As ErrorMessage = Nothing
            Dim deriveFromMatrix As Boolean = False

            msg = FinanceCodeMatrixBL.GetDeriveFromMatrixApplicationSetting(Me.DbConnection, Nothing, deriveFromMatrix)
            If Not msg.Success Then WebUtils.Utils.DisplayAccessDenied()

            If Not deriveFromMatrix Then
                msg = New ErrorMessage()
                msg.Message = "The Finance Coding system setting 'Derive from matrix' is set to 'False'. Access to the Finance Code Matrix facility is thus prohibited."
                msg.Success = False
                WebUtils.Utils.DisplayInformation(msg)
            End If
        End Sub

#End Region

    End Class

End Namespace