Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library.Results.Messages
Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Abacus.Library.Selectors
Imports Target.Library.JsonSerializerExtensions
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web



Namespace Apps.Actuals.DayCare

    ''' <summary>
    ''' Wizard screen to search for and view day care registers.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Waqas  02/08/2012  D12305 - created.
    ''' </history>
    Partial Public Class Lister
        Inherits BasePage

#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.DayCare"

#End Region

#Region " event Handlers "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Service Register")

            Dim resultSettings As New ServiceRegisterSettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "gsrResultSettings")
            End With

            ' add in page script
            JsLinks.Add("Lister.js")
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Date.js"))

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))

        End Sub

#End Region

    End Class

End Namespace