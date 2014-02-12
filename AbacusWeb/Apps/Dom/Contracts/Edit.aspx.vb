Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.Contracts

	''' <summary>
	''' Main container screen used to maintain domiciliary contracts.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' MvO  12/03/2008  Support for copy contract functionality.
    ''' </history>
	Partial Class Edit
		Inherits Target.Web.Apps.BasePage

		Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Non-Residential Contract")

            Dim msg As ErrorMessage = New ErrorMessage
            Dim contractID As Integer = Utils.ToInt32(Request.QueryString("id"))
            Dim copyFromID As Integer = Utils.ToInt32(Request.QueryString("copyFromID"))
			Dim builder As Target.Library.Web.UriBuilder = New Target.Library.Web.UriBuilder(Request.Url)
			builder.QueryItems.Remove("backUrl")

			frmTree.Attributes.Add("src", String.Format("Tree.aspx{0}&selectType=c", builder.Query))
			frmContent.Attributes.Add("src", String.Format("Header.aspx{0}&mode={1}", builder.Query, Convert.ToInt32(IIf(contractID = 0, StdButtonsMode.AddNew, StdButtonsMode.Fetched))))

            If contractID <> 0 Or copyFromID <> 0 Then
                'Fetch the contract
                Dim contract As DomContract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                If contractID <> 0 Then
                    msg = contract.Fetch(contractID)
                Else
                    msg = contract.Fetch(copyFromID)
                End If
                If Not msg.Success Then WebUtils.DisplayError(msg)
                ' store MRU Contract
                Dim mruManager As Target.Library.Web.MostRecentlyUsedManager = New Target.Library.Web.MostRecentlyUsedManager(HttpContext.Current)
                mruManager("DOM_CONTRACTS")(contractID.ToString()) = String.Format("{0}: {1}", contract.Number, contract.Title)
                mruManager.Save(HttpContext.Current)
            End If

                Me.JsLinks.Add("Edit.js")

		End Sub

	End Class

End Namespace
