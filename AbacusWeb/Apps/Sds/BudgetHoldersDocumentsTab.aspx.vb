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

Namespace Apps.Sds
    ''' <summary>
    ''' Screen used to maintain Budget Holder document details.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO  26/04/2011  SDS issue #600 - created.
    ''' </history>
    Partial Public Class BudgetHoldersDocumentsTab
        Inherits Target.Web.Apps.BasePage

        Private _bhid As Integer

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.BudgetHolders"), "Budget Holders")
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("bhid")) > 0 Then
                _bhid = Utils.ToInt32(Request.QueryString("bhid"))
            End If

            CType(docSelector, DocumentSelector).ServiceUserType = DocumentAssociationType.BudgetHolder

            CType(docSelector, DocumentSelector).Show_Buttons = (ShowButtons.New + ShowButtons.View + ShowButtons.Properties)

            CType(docSelector, DocumentSelector).Show_Filters = ( _
                            ShowFilters.Created + ShowFilters.CreatedBy + ShowFilters.CreatedFrom + _
                            ShowFilters.CreatedTo + ShowFilters.DocumentType + ShowFilters.Origin + _
                            ShowFilters.PrintStatus + ShowFilters.PrintStatusBy + _
                            ShowFilters.PrintStatusCheckBoxes + ShowFilters.PrintStatusFrom + _
                            ShowFilters.PrintStatusTo)

            CType(docSelector, DocumentSelector).InitControl(Me.Page, _bhid)

        End Sub

    End Class
End Namespace
