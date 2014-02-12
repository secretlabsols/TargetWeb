
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.Controls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.Dom.Contracts

	''' <summary>
	''' Screen used to launch the audit log for a domiciliary contract.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO  01/12/2008  D11444 - security overhaul.
    ''' MikeVO  18/08/2008  D11398 - support for visit service types.
    ''' </history>
	Partial Public Class AuditLog
		Inherits BasePage

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Domiciliary Contract Audit Log")

			With cboTableName.DropDownList.Items
                .Add(New ListItem(String.Empty, "|DomContract|DomContractPeriod|DomContractDomRateCategory_DomContractPeriod|DomContractPeriodSystemAccount|DomContractVisitServiceType|DomContractVisitRateCategory|DomContractReOpenedWeek|"))
				.Add(New ListItem("Header", "|DomContract|"))
				.Add(New ListItem("Periods", "|DomContractPeriod|"))
				.Add(New ListItem("Unit Costs", "|DomContractDomRateCategory_DomContractPeriod|"))
                .Add(New ListItem("System Accounts", "|DomContractPeriodSystemAccount|"))
                .Add(New ListItem("Visit Service Types", "|DomContractVisitServiceType|"))
				.Add(New ListItem("Visit Rate Categories", "|DomContractVisitRateCategory|"))
				.Add(New ListItem("Re-Opened Weeks", "|DomContractReOpenedWeek|"))
			End With

            Me.JsLinks.Add("AuditLog.js")

            ' prime audot logging popup menu item ID
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID

		End Sub

	End Class

End Namespace