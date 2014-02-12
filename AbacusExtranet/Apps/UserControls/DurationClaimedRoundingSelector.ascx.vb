Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports System.Text

Namespace Apps.UserControls

    Partial Public Class DurationClaimedRoundingSelector
        Inherits System.Web.UI.UserControl

        Private _btnNewVisibility As String
        Private _btnCopyVisibility As String

        Private Const _WebCmdAddNewKey As String = "AbacusExtranet.WebNavMenuItemCommand.ReferenceData.DurationClaimedRoundingRules.AddNew"
        Private Const _WebCmdEditKey As String = "AbacusExtranet.WebNavMenuItemCommand.ReferenceData.DurationClaimedRoundingRules.Edit"


        Public Sub InitControl(ByVal thePage As BasePage, _
           ByVal clientID As Integer, _
           ByVal isCouncilUser As Boolean, _
           ByVal userHasAddNewCommand As Boolean _
           )

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            Dim id As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim js As New StringBuilder()


            If currentPage <= 0 Then currentPage = 1

            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/DurationClaimedRoundingSelector.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DurationClaimedRounding))

            js.AppendFormat("currentPage={0};", currentPage)
            js.AppendFormat("clientID={0};", clientID)
            js.AppendFormat("isCouncilUser={0};", isCouncilUser.ToString().ToLower())
            js.AppendFormat("userHasAddNewCommand={0};", userHasAddNewCommand.ToString.ToLower())
            js.AppendFormat("qs_DcrID={0};", id)

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js.ToString()))

        End Sub

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load




        End Sub


    End Class
End Namespace