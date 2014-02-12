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

Namespace Apps.Dom.SvcOrders
    ''' <summary>
    ''' Screen used to maintain service user document details.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO  21/04/2011  SDS issue #606 - created.
    ''' </history>
    Partial Public Class DocumentsTab
        Inherits Target.Web.Apps.BasePage

        Private _clientID As Integer

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DomiciliaryServiceOrders"), "Non-Residential Service Order")
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID

            ' get qs ids
            If Utils.ToInt32(Request.QueryString("clientid")) > 0 Then
                _clientID = Utils.ToInt32(Request.QueryString("clientid"))
            End If

            CType(docSelector, DocumentSelector).ServiceUserType = DocumentAssociationType.ServiceUser

            CType(docSelector, DocumentSelector).Show_Buttons = (ShowButtons.New + ShowButtons.View + ShowButtons.Properties)

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
