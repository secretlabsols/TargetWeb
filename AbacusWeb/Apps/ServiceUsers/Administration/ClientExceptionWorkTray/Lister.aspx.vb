﻿Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library.Results.Messages
Imports Target.Abacus.Library.Results.Messages.Settings
Imports Target.Abacus.Library.Selectors
Imports Target.Library.JsonSerializerExtensions
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web

Namespace Apps.ServiceUsers.Administration.ClientExceptionWorkTray

    Public Class Lister
        Inherits BasePage


#Region "Fields"

        Private Const _WebNavMenuItemKey As String = "AbacusIntranet.WebNavMenuItem.ServiceUsers.Administration.ClientExceptionWorkTray"

#End Region


        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            ' setup the page
            InitPage(WebUtils.ConstantsManager.GetConstant(_WebNavMenuItemKey), "Service User Work Tray")

            Dim resultSettings As New ClientExceptionWorkTraySettings()

            With resultSettings
                .Load(Me)
                .RegisterAsJavascriptVariable(Me, "cwtResultSettings")
            End With

            ' add in page script
            JsLinks.Add("Lister.js")

        End Sub

    End Class

End Namespace