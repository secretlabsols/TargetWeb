Imports System.Collections.Generic
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Dom.SvcOrder

    ''' <summary>
    ''' Used to displays the tree in the main Service Order Funding screen.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Paul  17/02/2009  D11491 - Initial Version.
    ''' </history>
    Partial Class FundingTree
        Inherits Target.Web.Apps.BasePage

        Private _canAddNew As Boolean, _canEdit As Boolean, _canDelete As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceOrderFunding"), "Service Order Funding")

            Dim serviceOrderID As Integer = Utils.ToInt32(Request.QueryString("id"))
            Dim selectID As String = Utils.ToInt32(Request.QueryString("selectID"))
            Dim selectType As String = Request.QueryString("selectType")

            ' security
            _canAddNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderFunding.AddNew"))
            _canEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderFunding.Edit"))
            _canDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderFunding.Delete"))

            If Not _canAddNew AndAlso serviceOrderID = 0 Then WebUtils.DisplayAccessDenied()

            PopulateTree(serviceOrderID, selectType, selectID)

        End Sub

        Private Sub PopulateTree(ByVal serviceOrderID As Integer, ByVal selectType As String, ByVal selectID As Integer)

            Const URL_FUNDINGHEADER As String = "FundingHeader.aspx?id={0}&fundingID={1}&mode={2}"
            Const URL_FUNDINGDETAIL As String = "FundingDetail.aspx?id={0}&detailID={1}&fundingID={2}&mode={3}"

            Const SELECT_TYPE_HEADER As String = "h"
            Const SELECT_TYPE_DETAIL As String = "d"
            Const SELECT_TYPE_ORDER As String = "o"

            Dim fundingList As DomServiceOrderFundingCollection = Nothing
            Dim fundingDetailList As DomServiceOrderFundingDetailCollection = Nothing
            Dim domServiceType As DomServiceType = New DomServiceType(Me.DbConnection, String.Empty, String.Empty)
            Dim percent As Double
            Dim msg As ErrorMessage
            Dim DSONode As TreeNode, singleFundingNode As TreeNode, singleFundingDetailNode As TreeNode
            Dim builder As Target.Library.Web.UriBuilder = New Target.Library.Web.UriBuilder(Request.Url)
            builder.QueryItems.Remove("backUrl")

            With tree
                'Dom service Order Node
                DSONode = AddNode(.Nodes, "DOM Service Order", String.Empty, True)
                If selectType = SELECT_TYPE_ORDER Then DSONode.Selected = True
                'before we go any further check that we have a service Order ID
                If serviceOrderID > 0 Then
                    'Provide a means of adding new funding header rows for the Service Order.
                    If _canAddNew Then
                        AddNode(DSONode.ChildNodes, "Add New...", String.Format(URL_FUNDINGHEADER, serviceOrderID, 0, Convert.ToInt32(StdButtonsMode.AddNew)), False)
                    End If

                    'fetch all funding records for the selected Service Order and output a node for each.
                    'TODO D12199
                    'msg = DomServiceOrderFunding.FetchList(Me.DbConnection, fundingList, serviceOrderID)
                    'If Not msg.Success Then WebUtils.DisplayError(msg)
                    For Each funding As DomServiceOrderFunding In fundingList
                        msg = domServiceType.Fetch(funding.DomServiceTypeID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        'ouput existing Funding Header nodes
                        singleFundingNode = AddNode(DSONode.ChildNodes, domServiceType.Description, String.Format(URL_FUNDINGHEADER, serviceOrderID, funding.ID, Convert.ToInt32(StdButtonsMode.Fetched)), True)
                        'Determine if we want to select this node
                        If selectType = SELECT_TYPE_HEADER AndAlso selectID = funding.ID Then singleFundingNode.Selected = True
                        'Output node that will allow new detail lines to be created.
                        If _canAddNew Then
                            AddNode(singleFundingNode.ChildNodes, "Add New...", String.Format(URL_FUNDINGDETAIL, serviceOrderID, 0, funding.ID, Convert.ToInt32(StdButtonsMode.AddNew)), False)
                        End If
                        'output Funding Detail Lines
                        msg = DomServiceOrderFundingDetail.FetchList(Me.DbConnection, fundingDetailList, funding.ID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        For Each fundDetail As DomServiceOrderFundingDetail In fundingDetailList
                            If fundDetail.Denominator = 0 Then
                                percent = 0
                            Else
                                percent = Math.Round(Convert.ToDouble(fundDetail.Numerator / fundDetail.Denominator * 100), 2)
                            End If
                            Dim expAccount As ExpenditureAccount = New ExpenditureAccount(Me.DbConnection)
                            msg = expAccount.Fetch(fundDetail.ExpenditureAccountID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            Dim financeCode As FinanceCode = New FinanceCode(Me.DbConnection)
                            msg = financeCode.Fetch(expAccount.FinanceCodeID_Expenditure)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            'output the node
                            Dim nodeText As String

                            If percent > 0 Then
                                nodeText = String.Format("{0} ({1}%)", financeCode.Code, percent)
                            Else
                                nodeText = String.Format("<div style='color:orange'>{0} ({1}%) </div>", financeCode.Code, percent)
                            End If

                            singleFundingDetailNode = AddNode(singleFundingNode.ChildNodes, _
                                    nodeText, _
                                    String.Format(URL_FUNDINGDETAIL, serviceOrderID, fundDetail.ID, funding.ID, Convert.ToInt32(StdButtonsMode.Fetched)), _
                                    False)
                            'Determine if we want to select this node
                            If selectType = SELECT_TYPE_DETAIL AndAlso selectID = fundDetail.ID Then singleFundingDetailNode.Selected = True
                        Next
                    Next

                End If

            End With

        End Sub

        Private Function AddNode(ByVal nodes As TreeNodeCollection, ByVal text As String, ByVal url As String, ByVal expanded As Boolean) As TreeNode

            Dim newNode As TreeNode = New TreeNode(text)
            With newNode
                If url Is Nothing Then
                    .SelectAction = TreeNodeSelectAction.None
                Else
                    .NavigateUrl = String.Format("javascript:window.parent.NavigateContent('{0}');", url)
                End If
                .Expanded = expanded
            End With
            nodes.Add(newNode)
            Return newNode

        End Function

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            WebUtils.TreeViewClientSelectFix(tree, String.Empty, String.Empty, "aspTreeView_SelectedNode", "aspTreeView_SelectedNode")
        End Sub

    End Class

End Namespace