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

Namespace Apps.Admin.SystemSettings

    Partial Public Class ExternalDataConfiguration
        Inherits BasePage

        Private _canToggleExternalIsMaster As Boolean
        Private _canToggleExceptionIfBlank As Boolean
        Private _canToggleAllowEditInAbacus As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Administration.SystemSettings.ExternalDataConfiguration"), "External Data Configuration")
            Dim selectorTypes As New List(Of SelectorTypes)()
            Const SCRIPT_STARTUP As String = "Startup"

            ' add in page script
            JsLinks.Add("ExternalDataConfiguration.js")

            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.ExternalFields))

            _canToggleExternalIsMaster = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Administration.SystemSettings.ExternalDataConfiguration.AmendExternalIsMaster"))
            _canToggleExceptionIfBlank = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Administration.SystemSettings.ExternalDataConfiguration.AmendExceptionIfBlank"))
            _canToggleAllowEditInAbacus = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Administration.SystemSettings.ExternalDataConfiguration.AmendAllowEditInAbacus"))

            ' add any selectors for registration
            selectorTypes.Add(Target.Abacus.Library.Selectors.Messages.SelectorTypes.ExternalField)

            ' register the selector
            SelectorRegistrationUtility.Register(Page, selectorTypes, True)

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                 String.Format("canToggleExternalIsMaster={0}; canToggleExceptionIfBlank={1}; canToggleAllowEditInAbacus={2};", _
                  _canToggleExternalIsMaster.ToString.ToLower, _canToggleExceptionIfBlank.ToString.ToLower, _canToggleAllowEditInAbacus.ToString.ToLower), True)
            End If


        End Sub

    End Class

End Namespace