Imports Target.Abacus.Library.RequestPayments
Imports System.Collections.Generic
Imports Target.Abacus.Library.DataClasses
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Library
Imports System.Text
Imports Target.Abacus.Library.DataClasses.Collections
Imports System.Web.Script.Serialization
Imports Target.Abacus.Library
Imports Target.Web.Apps.Security

Namespace Apps.Dom.PaymentSchedules

    Partial Public Class RequestPaymentComplete
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.RequestPayments"), "Request Payments")
            SetPScheduleUrl()

        End Sub

        Public Sub SetPScheduleUrl()
            Dim msg As New ErrorMessage
            Dim menuItemId As Integer = Utils.ToInt32(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"))
            Dim menu As Target.Web.Apps.Navigation.WebNavMenuItem = New Target.Web.Apps.Navigation.WebNavMenuItem(Me.DbConnection)
            msg = menu.Fetch(menuItemId)
            If msg.Success Then
                lnkPSchedule.NavigateUrl = menu.URL
            Else
                Target.Library.Web.Utils.DisplayError(msg)
            End If


        End Sub
    End Class

End Namespace