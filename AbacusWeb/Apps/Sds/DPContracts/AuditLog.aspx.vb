
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.Controls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.Sds.DPContracts

    ''' <summary>
    ''' Screen used to launch the audit log for a direct payment contract.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' </history>
    Partial Public Class AuditLog
        Inherits BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DirectPaymentContracts"), "Direct Payment Contract Audit Log")

            With cboTableName.DropDownList.Items
                .Add(New ListItem(String.Empty, "|DPContract|DPContractPeriod|DPContractDetail|DPContractDetailBreakdown|"))
                .Add(New ListItem("Header", "|DPContract|"))
                .Add(New ListItem("Periods", "|DPContractPeriod|"))
                .Add(New ListItem("Payments", "|DPContractDetail|"))
                .Add(New ListItem("Payment Breakdown", "|DPContractDetailBreakdown|"))
            End With

            Me.JsLinks.Add("AuditLog.js")

            ' prime audit logging popup menu item ID
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID

        End Sub

    End Class

End Namespace