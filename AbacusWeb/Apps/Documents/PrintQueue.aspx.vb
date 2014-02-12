Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Utils = Target.Library.Utils
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.Documents
Imports Target.Abacus.Web.Apps.Documents.UserControls

Namespace Apps.Documents


    ''' <summary>
    ''' Screen displaying queued documents and providing option to batch them
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     IHS      08/04/2011  D11960 - Created
    ''' </history>
    Partial Class PrintQueue
        Inherits Target.Web.Apps.BasePage

        Private Const _NavigationItemKey As String = "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"
        Private Const _PageTitle As String = "Print Queue"

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(ConstantsManager.GetConstant(_NavigationItemKey), _PageTitle)

            CType(docSelector, DocumentSelector).ServiceUserType = DocumentAssociationType.Any

            CType(docSelector, DocumentSelector).Show_Buttons = (ShowButtons.View + ShowButtons.Properties + _
                                                                 ShowButtons.Remove + ShowButtons.CreateBatch)

            CType(docSelector, DocumentSelector).Show_Filters = (ShowFilters.DocumentType + ShowFilters.Recipient + _
                                                                 ShowFilters.PrintStatus + ShowFilters.PrintStatusBy)

            CType(docSelector, DocumentSelector).PrintStatusLabel = "Queued By"

            CType(docSelector, DocumentSelector).InitControl(Me.Page, -1)

        End Sub

    End Class

End Namespace