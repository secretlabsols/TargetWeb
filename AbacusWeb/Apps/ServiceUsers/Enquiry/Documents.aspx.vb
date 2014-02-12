Imports System.Text
Imports System.Collections.Generic
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Library.Web.Controls
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Library.Web.Extensions.AjaxControlToolkit
Imports Target.Abacus.Web.Apps.UserControls
Imports Target.Abacus.Library.DebtorInvoices
Imports Target.Abacus.Web.Apps.Documents.UserControls

Namespace Apps.ServiceUsers.Enquiry
    ''' <summary>
    ''' Screen used to maintain service user document details.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Iftikhar      31/01/2011  D11915 - Created
    ''' </history>
    Partial Public Class Documents
        Inherits Target.Web.Apps.BasePage

        Private _clientID As Integer
        Private _qsParser As WizardScreenParameters

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceUserEnquiry"), "Service User")
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("clientid")) > 0 Then
                _clientID = Utils.ToInt32(Request.QueryString("clientid"))
            End If

            CType(docSelector, DocumentSelector).ServiceUserType = DocumentAssociationType.ServiceUser

            CType(docSelector, DocumentSelector).Show_Buttons = (ShowButtons.New + ShowButtons.View + _
                                                                 ShowButtons.Properties + ShowButtons.Print)

            CType(docSelector, DocumentSelector).Show_Filters = ( _
                            ShowFilters.Created + ShowFilters.CreatedBy + ShowFilters.CreatedFrom + _
                            ShowFilters.CreatedTo + ShowFilters.DocumentType + ShowFilters.Origin + _
                            ShowFilters.PrintStatus + ShowFilters.PrintStatusBy + _
                            ShowFilters.PrintStatusCheckBoxes + ShowFilters.PrintStatusFrom + _
                            ShowFilters.PrintStatusTo)

            CType(docSelector, DocumentSelector).InitControl(Me.Page, _clientID)
        End Sub

    End Class
End Namespace
