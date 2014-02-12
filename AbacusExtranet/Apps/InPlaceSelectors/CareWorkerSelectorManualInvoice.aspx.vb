Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports WebUtils = Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security


Namespace Apps.InPlaceSelectors

    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class CareWorkerSelectorManualInvoice
        Inherits Target.Web.Apps.BasePage

        Private _providerId As Integer
        Private _tabindex As Integer
        Private _existingIds As String
        Private _editMode As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.EnableTimeout = False
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Care Worker")

            _providerId = Utils.ToInt32(Request.QueryString("estabid"))
            _tabindex = Utils.ToInt32(Request.QueryString("tabindex"))
            _existingIds = Utils.ToString(Request.QueryString("existingIds"))
            _editMode = Utils.ToString(Request.QueryString("editMode"))


            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            ' add date utility JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/InPlaceSelectors/CareWorkerSelectorManualInvoice.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.UserControls.CareWorkerSelector.Startup", _
                Target.Library.Web.Utils.WrapClientScript(String.Format("currentPage={0}; providerID={1}; rdbtnSelectExistingID=""{2}""; rdbtnEnterNewCareWorkerID=""{3}""; rdbtnCareWorkerNotSpecifiedID=""{4}""; txtReferenceID=""{5}""; txtNameID=""{6}"";tabindex={7};existingIds=""{8}"";editMode=""{9}"";", _
                    currentPage, _providerId, rdbtnSelectExisting.ClientID, rdbtnNew.ClientID, rdbtnNotSpecified.ClientID, txtReference.ClientID & "_txtTextBox", txtName.ClientID & "_txtTextBox", _tabindex, _existingIds,  _editMode.ToString().ToLower())) _
            )

        End Sub

    End Class

End Namespace