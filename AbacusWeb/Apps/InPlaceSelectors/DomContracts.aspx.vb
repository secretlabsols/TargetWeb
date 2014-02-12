
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Web.Apps.UserControls

Namespace Apps.InPlaceSelectors

	''' <summary>
	''' Popup screen that allows a user to select a domiciliary contract.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     27/11/2009  D11681
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class DomContracts
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.EnableTimeout = False
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "Select Domiciliary Contract")
            Me.JsLinks.Add("DomContracts.js")

            Dim estabID As Integer = Utils.ToInt32(Request.QueryString("estabID"))
            Dim contractID As Integer = Utils.ToInt32(Request.QueryString("contractID"))
            Dim contractTypeID As DomContractType = Utils.ToInt32(Request.QueryString("ctID"))
            Dim contractGroupID As Integer = Utils.ToInt32(Request.QueryString("cgID"))
            Dim contractEndReasonID As Integer = Utils.ToInt32(Request.QueryString("cerID"))
            Dim serviceGroupID As Integer = Utils.ToInt32(Request.QueryString("svcGroupID"))
            Dim serviceGroupClassificationID As Integer = Utils.ToInt32(Request.QueryString("serviceGroupClassificationID"))
            Dim frameworkTypeID As Integer = Utils.ToInt32(Request.QueryString("ftid"))

            selector.InitControl(Me, estabID, contractTypeID, contractGroupID, Nothing, Nothing, contractEndReasonID, False, False, False, False, False, False, contractID, serviceGroupID, serviceGroupClassificationID, frameworkTypeID)

        End Sub

    End Class

End Namespace