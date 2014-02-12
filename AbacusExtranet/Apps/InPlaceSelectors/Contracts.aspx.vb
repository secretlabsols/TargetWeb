Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports WebUtils = Target.Library.Web.Utils
'Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Web.Apps

Namespace Apps.InPlaceSelectors

    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class Contracts
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Provider")
            Me.JsLinks.Add("Contracts.js")

            Dim estabID As Integer = Utils.ToInt32(Request.QueryString("estabID"))
            Dim reference As String = Request.QueryString("ref")
            Dim name As String = Request.QueryString("name")
            Dim contractId As Integer = Utils.ToInt32(Request.QueryString("cid"))

            Dim contractGroupId As Integer = 0
            Dim dateFrom As Date = Date.MinValue
            Dim dateTo As Date = Date.MaxValue



            selector.thePage = Me
            selector.establishmentID = estabID
            selector.contractType = Abacus.Library.DomContractType.Unknown
            selector.contractGroupID = contractGroupId
            selector.dateFrom = dateFrom
            selector.dateTo = dateTo
            selector.showNewButton = False
            selector.showViewButton = False
            selector.selectedContractID = contractId

        End Sub

    End Class

End Namespace