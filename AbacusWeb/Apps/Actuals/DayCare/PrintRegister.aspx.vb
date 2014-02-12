
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security
Imports Target.Web.Apps
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.DataClasses
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.Controls
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library
Imports Target.Abacus.Web
Imports System.Configuration.ConfigurationManager
Imports System.Web.Services
Imports Target.Library.Web
Imports Target.Abacus.Web.Apps.WebSvc

Namespace Apps.Actuals.DayCare

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.Actuals.DayCare.PrintRegister
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Popup window to print the selected register.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     MoTahir     27/11/2009  D11681 Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class PrintRegister
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.EnableTimeout = False
            Me.InitPage(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DayCare"), "Register")
            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))

            Me.JsLinks.Add("PrintRegister.js")

        End Sub

    End Class

End Namespace