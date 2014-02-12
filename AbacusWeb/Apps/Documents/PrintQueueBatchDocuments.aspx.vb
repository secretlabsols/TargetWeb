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
    ''' Screen displaying batched documents for printing and providing 
	''' option to print or queue them for later printing
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     IHS      14/04/2011  D11960 - Created
    ''' </history>
    Partial Class PrintQueueBatchDocuments
        Inherits Target.Web.Apps.BasePage

        Private Const _NavigationItemKey As String = "AbacusIntranet.WebNavMenuItem.Documents.DocumentsTab"
        Private Const _PageTitle As String = "Print Queue Batch Documents"
        Private _documentPrintQueueBatchID As Integer = -1

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim ucDocSelector As DocumentSelector = CType(docSelector, DocumentSelector)

            Me.InitPage(ConstantsManager.GetConstant(_NavigationItemKey), _PageTitle)

            ucDocSelector.ServiceUserType = DocumentAssociationType.Any

            _documentPrintQueueBatchID = Target.Library.Utils.ToInt32(Request.QueryString("documentPrintQueueBatchID"))
            ucDocSelector.DocumentPrintQueueBatchID = _documentPrintQueueBatchID

            ucDocSelector.Show_Buttons = (ShowButtons.View + ShowButtons.Properties + ShowButtons.Print)

            ucDocSelector.Show_Filters = (ShowFilters.DocumentType + ShowFilters.PrintStatusCheckBoxes + _
                                          ShowFilters.PrintStatus + ShowFilters.PrintStatusBy + _
                                          ShowFilters.PrintStatusFrom + ShowFilters.PrintStatusTo)

            ucDocSelector.InitControl(Me.Page, -1)

        End Sub

    End Class

End Namespace