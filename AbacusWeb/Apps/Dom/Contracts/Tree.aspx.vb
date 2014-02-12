
Imports System.Collections.Generic
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports System.Data.SqlClient
Imports Target.Web.Apps
Imports Target.Abacus.Library.PaymentTolerance

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Used to displays the tree in the main Edit screen when maintaining domiciliary contracts.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MoTahir  21/10/2013  A8263 -  Modify Contract Type selector
    ''' MoTahir  17/09/2013  Block Contract Payment Plan (D12459)
    ''' JAF  29/08/2013  Added Block Agreed Cost node under Period (D12503)
    ''' MoTahir 16/11/2012  D12343 - Remove Framework Type From Service Group
    ''' JF   17/08/2011  Removed incorrect commenting out (D12051)
    ''' JF   12/08/2011  Added Service User Calculation Method under contract period (D12051)
    ''' CD   09/08/2011  D11965 - Changes to use Framework Type to determine bevahiour of form.
    ''' MoTahir 08/08/2011 D11766 - Provider Invoice Tolerances
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' MvO  19/08/2008  Fix to sorted list of VRCs to ensure the sorting key is unique.
    ''' MvO  18/08/2008  D11398 - Added Visit Service Types.
    ''' MvO  12/03/2008  Removed re-opended weeks functionality.
    ''' </history>
    Partial Class Tree
        Inherits Target.Web.Apps.BasePage

        Private _canAddNew As Boolean, _canEdit As Boolean, _canCopy As Boolean, _canDelete As Boolean
        Private _contractID As Integer

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Domiciliary Contract Tree")

            _contractID = Utils.ToInt32(Request.QueryString("id"))
            Dim copyFromID As Integer = Utils.ToInt32(Request.QueryString("copyFromID"))
            Dim selectID As String = Utils.ToInt32(Request.QueryString("selectID"))
            Dim selectType As String = Request.QueryString("selectType")

            If selectType Is Nothing Then selectType = String.Empty

            ' security
            _canAddNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.AddNew"))
            _canEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))
            _canCopy = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Copy"))
            _canDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Delete"))

            If Not _canAddNew AndAlso _contractID = 0 Then WebUtils.DisplayAccessDenied()
            If Not _canCopy AndAlso copyFromID <> 0 Then WebUtils.DisplayAccessDenied()

            PopulateTree(_contractID, selectType, selectID)

        End Sub

        Private Sub PopulateTree(ByVal contractID As Integer, ByVal selectType As String, ByVal selectID As Integer)

            Const URL_HEADER As String = "Header.aspx{0}&mode={1}"
            Const URL_PERIOD As String = "Period.aspx?contractID={0}&id={1}&mode={2}"
            Const URL_UNITCOST As String = "UnitCost.aspx?contractID={0}&periodID={1}&mode=1"   ' mode=1 means Fetched
            Const URL_LINECOST_OVERRIDES As String = "LineCostOverride.aspx?contractID={0}&periodID={1}&mode=1"   ' mode=1 means Fetched
            Const URL_SERVICEDAYS As String = "ServiceDays.aspx?contractID={0}&periodID={1}&mode=1"   ' mode=1 means Fetched
            Const URL_SYSTEM_ACCOUNT As String = "SystemAccount.aspx?contractID={0}&periodID={1}&id={2}&mode={3}"
            Const URL_VST As String = "Vst.aspx?contractID={0}&id={1}&mode={2}"
            Const URL_VRC As String = "Vrc.aspx?contractID={0}&id={1}&mode={2}"
            Const URL_AUDIT As String = "AuditLog.aspx?contractID={0}"
            Const URL_PAYMENT_TOLERANCE_GROUP As String = "PaymentTolerances.aspx?contractID={0}&periodID={1}&mode=1"   ' mode=1 means Fetched
            Const URL_CALCMETHOD As String = "Sucm.aspx?contractID={0}&periodID={1}&mode=1"   ' mode=1 means Fetched
            Const URL_BLOCKAGREEDCOST As String = "BlockAgreedCost.aspx?contractID={0}&periodID={1}&mode=1"   ' mode=1 means Fetched
            Const URL_PAYMENTPLAN As String = "PaymentPlan.aspx?contractID={0}&periodID={1}&mode=1"   ' mode=1 means Fetched
            Const SELECT_TYPE_HEADER As String = "c"
            Const SELECT_TYPE_PERIOD As String = "p"
            Const SELECT_TYPE_UNITCOST As String = "uc"
            Const SELECT_TYPE_LINECOST_OVERRIDES As String = "lco"
            Const SELECT_TYPE_SYSACC As String = "sa"
            Const SELECT_TYPE_VRC As String = "vrc"
            Const SELECT_TYPE_VST As String = "vst"
            Const SELECT_TYPE_SERVICEDAYS As String = "sd"
            Const SELECT_TYPE_PAYTOL As String = "pt"
            Const SELECT_TYPE_BLOCKAGREEDCOST As String = "bac"
            Const SELECT_TYPE_PAYMENTPLAN As String = "pp"

            Dim contract As DomContract
            Dim contractEnded As Boolean
            Dim periods As DomContractPeriodCollection = Nothing
            Dim systemAccounts As DomContractPeriodSystemAccountCollection = Nothing
            Dim unitCosts As DomContractDomRateCategory_DomContractPeriodCollection = Nothing
            Dim visitRateCategories As DomContractVisitRateCategoryCollection = Nothing
            Dim visitServiceTypes As DomContractVisitServiceTypeCollection = Nothing
            Dim client As ClientDetail
            Dim msg As ErrorMessage
            Dim contractNode As TreeNode, singlePeriodNode As TreeNode, singleSysAccNode As TreeNode, singleVrcNode As TreeNode, singleVstNode As TreeNode, paymentToleranceGroupNode As TreeNode
            Dim periodsNode As TreeNode, sysAccsNode As TreeNode, unitCostsNode As TreeNode, vrcsNode As TreeNode, vstsNode As TreeNode, serviceDaysNode As TreeNode, lineCostsOverrideNode As TreeNode
            Dim blockAgreedCostNode As TreeNode = Nothing
            Dim paymentPlanNode As TreeNode = Nothing
            Dim calcMethodNode As TreeNode
            Dim abbreviation As String = Nothing, vrcDesc As String
            Dim sortedVrcs As SortedDictionary(Of String, Pair)
            Dim st As DomServiceType
            Dim frType As FrameworkType = Nothing
            Dim builder As Target.Library.Web.UriBuilder = New Target.Library.Web.UriBuilder(Request.Url)
            builder.QueryItems.Remove("backUrl")
            Dim hasPaymentTolerance As Boolean

            With tree
                ' contract
                contractNode = AddNode(.Nodes, "Contract", String.Format(URL_HEADER, builder.Query, Convert.ToInt32(IIf(contractID = 0, StdButtonsMode.AddNew, StdButtonsMode.Fetched))), True)
                ' select?
                If selectType = SELECT_TYPE_HEADER Then contractNode.Selected = True
                If contractID > 0 Then
                    ' contract
                    contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
                    msg = contract.Fetch(contractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    'Fetch the service group assigned to the contract
                    frType = New FrameworkType(Me.DbConnection, String.Empty, String.Empty)
                    msg = DomContractBL.FetchFrameWorkTypeByContract(Me.DbConnection, contract.DomRateFrameworkID, frType)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    contractEnded = (contract.EndDate <> DataUtils.MAX_DATE)
                    ' periods
                    periodsNode = AddNode(contractNode.ChildNodes, "Periods", Nothing, True)
                    If Not contractEnded AndAlso _canEdit Then
                        AddNode(periodsNode.ChildNodes, "Add New...", String.Format(URL_PERIOD, contractID, 0, Convert.ToInt32(StdButtonsMode.AddNew)), False)
                    End If
                    ' get all periods in the contract
                    msg = DomContractPeriod.FetchList(Me.DbConnection, periods, String.Empty, String.Empty, contractID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    ' start with the latest
                    periods.Sort(New CollectionSorter("DateFrom", SortDirection.Descending))
                    ' for each period
                    For Each period As DomContractPeriod In periods
                        ' add the node
                        singlePeriodNode = AddNode(periodsNode.ChildNodes, String.Format("{0} - {1}", period.DateFrom.ToString("dd/MM/yyyy"), IIf(period.DateTo = DataUtils.MAX_DATE, DataUtils.MAX_END_DATE_DESC, period.DateTo.ToString("dd/MM/yyyy"))), String.Format(URL_PERIOD, contractID, period.ID, Convert.ToInt32(StdButtonsMode.Fetched)), False)
                        ' select?
                        If selectType = SELECT_TYPE_PERIOD AndAlso selectID = period.ID Then singlePeriodNode.Selected = True

                        '++ If the contract is a 'Block Guarantee', so display a unique period option..
                        If contract.ContractType = "BlockGuarantee" Then
                            ' create the Service Days node for the period
                            blockAgreedCostNode = AddNode(singlePeriodNode.ChildNodes, "Block Agreed Cost", String.Format(URL_BLOCKAGREEDCOST, contractID, period.ID), False)
                            ' select?
                            If selectType = SELECT_TYPE_BLOCKAGREEDCOST AndAlso selectID = period.ID Then
                                singlePeriodNode.Expanded = True
                                blockAgreedCostNode.Selected = True
                            End If
                        End If

                        '++ If the contract is a 'Periodoc Block', so display a unique period option..
                        If contract.ContractType = DomContractType.BlockPeriodic.ToString() Then
                            ' create the Payment Plan node..
                            paymentPlanNode = AddNode(singlePeriodNode.ChildNodes, "Payment Plan", String.Format(URL_PAYMENTPLAN, contractID, period.ID), False)
                            ' select?
                            If selectType = SELECT_TYPE_PAYMENTPLAN AndAlso selectID = period.ID Then
                                singlePeriodNode.Expanded = True
                                paymentPlanNode.Selected = True
                            End If
                        End If

                        'if the framework type is attendance then show the Service Days Node
                        If frType.ID = FrameworkTypes.ServiceRegister Then
                            ' create the Service Days node for the period
                            serviceDaysNode = AddNode(singlePeriodNode.ChildNodes, "Service Days", String.Format(URL_SERVICEDAYS, contractID, period.ID), False)
                            ' select?
                            If selectType = SELECT_TYPE_SERVICEDAYS AndAlso selectID = period.ID Then
                                singlePeriodNode.Expanded = True
                                serviceDaysNode.Selected = True
                            End If
                        End If

                        ' create the unit costs node for the period
                        unitCostsNode = AddNode(singlePeriodNode.ChildNodes, "Unit Costs", String.Format(URL_UNITCOST, contractID, period.ID), False)
                        ' select?
                        If selectType = SELECT_TYPE_UNITCOST AndAlso selectID = period.ID Then
                            singlePeriodNode.Expanded = True
                            unitCostsNode.Selected = True
                        End If

                        ' create the line costs override node for the period
                        lineCostsOverrideNode = AddNode(singlePeriodNode.ChildNodes, "Invoice Line Cost Override", String.Format(URL_LINECOST_OVERRIDES, contractID, period.ID), False)
                        ' select?
                        If selectType = SELECT_TYPE_LINECOST_OVERRIDES AndAlso selectID = period.ID Then
                            singlePeriodNode.Expanded = True
                            lineCostsOverrideNode.Selected = True
                        End If

                        ' create the payment tolerance group node
                        ' check if Intranet Non Res is licenced
                        If IntranetNonResLicenced(Me.DbConnection) Then

                            'check if associated framework's rate categories has one or more payment tolerance groups
                            msg = PaymentToleranceBL.DoesContractHaveOneOrMorePaymentToleranceGroups(Me.DbConnection, _
                                                                                                     _contractID, _
                                                                                                     hasPaymentTolerance)

                            If hasPaymentTolerance Then
                                ' create the payment tolerance group node for the period
                                paymentToleranceGroupNode = AddNode(singlePeriodNode.ChildNodes, "Payment Tolerance Groups", _
                                                                    String.Format(URL_PAYMENT_TOLERANCE_GROUP, contractID, period.ID), False)

                                'select?
                                If selectType = SELECT_TYPE_PAYTOL AndAlso selectID = period.ID Then
                                    singlePeriodNode.Expanded = True
                                    paymentToleranceGroupNode.Selected = True
                                End If
                            End If
                        End If

                        ' system accounts
                        sysAccsNode = AddNode(singlePeriodNode.ChildNodes, "System Accounts", Nothing, False)
                        If Not contractEnded AndAlso _canEdit Then
                            AddNode(sysAccsNode.ChildNodes, "Add New...", String.Format(URL_SYSTEM_ACCOUNT, contractID, period.ID, 0, Convert.ToInt32(StdButtonsMode.AddNew)), False)
                        End If
                        ' get all system accounts for the period
                        msg = DomContractPeriodSystemAccount.FetchList(Me.DbConnection, systemAccounts, String.Empty, String.Empty, period.ID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        ' for each system account
                        For Each sysAcc As DomContractPeriodSystemAccount In systemAccounts
                            ' get the name of the client
                            client = New ClientDetail(Me.DbConnection, String.Empty, String.Empty)
                            msg = client.Fetch(sysAcc.ClientID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            ' add the node
                            singleSysAccNode = AddNode(sysAccsNode.ChildNodes, client.Name, String.Format(URL_SYSTEM_ACCOUNT, contractID, period.ID, sysAcc.ID, Convert.ToInt32(StdButtonsMode.Fetched)), False)
                            ' select?
                            If selectType = SELECT_TYPE_SYSACC AndAlso selectID = sysAcc.ID Then
                                singlePeriodNode.Expanded = True
                                sysAccsNode.Expanded = True
                                singleSysAccNode.Selected = True
                            End If
                        Next

                        If frType.ID <> FrameworkTypes.ServiceRegister _
                            AndAlso frType.ID <> FrameworkTypes.CommunityGeneral Then

                            '++ Create the Service User Minutes Calc Method node for the period..
                            calcMethodNode = AddNode(singlePeriodNode.ChildNodes, "S/U Minutes Calc Method", String.Format(URL_CALCMETHOD, contractID, period.ID), False)

                        End If
                        
                    Next

                    If contract.VisitBasedReturns AndAlso frType.ID <> FrameworkTypes.CommunityGeneral Then
                        ' visit service types
                        vstsNode = AddNode(contractNode.ChildNodes, "Visit Service Types", Nothing, False)
                        If Not contractEnded AndAlso _canEdit Then
                            AddNode(vstsNode.ChildNodes, "Add New...", String.Format(URL_VST, contractID, 0, Convert.ToInt32(StdButtonsMode.AddNew)), False)
                        End If
                        ' get all VSTs for the contract
                        msg = DomContractVisitServiceType.FetchList(Me.DbConnection, visitServiceTypes, String.Empty, String.Empty, contractID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        For Each vst As DomContractVisitServiceType In visitServiceTypes
                            ' get the service type description
                            st = New DomServiceType(Me.DbConnection, String.Empty, String.Empty)
                            msg = st.Fetch(vst.DomServiceTypeID)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            ' add the node
                            singleVstNode = AddNode(vstsNode.ChildNodes, st.Description, String.Format(URL_VST, contractID, vst.ID, Convert.ToInt32(StdButtonsMode.Fetched)), False)
                            ' select?
                            If selectType = SELECT_TYPE_VST AndAlso selectID = vst.ID Then
                                vstsNode.Expanded = True
                                singleVstNode.Selected = True
                            End If
                        Next

                        ' visit rate categories
                        vrcsNode = AddNode(contractNode.ChildNodes, "Visit Rate Categories", Nothing, False)
                        If Not contractEnded AndAlso _canEdit Then
                            AddNode(vrcsNode.ChildNodes, "Add New...", String.Format(URL_VRC, contractID, 0, Convert.ToInt32(StdButtonsMode.AddNew)), False)
                        End If
                        ' get all VRCs for the contract
                        msg = DomContractVisitRateCategory.FetchList(Me.DbConnection, visitRateCategories, String.Empty, String.Empty, contractID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        ' for each vrc, add them into a sorted structure
                        sortedVrcs = New SortedDictionary(Of String, Pair)
                        For Each vrc As DomContractVisitRateCategory In visitRateCategories
                            ' get the constructed abbreviation
                            msg = DomContractBL.ConstructRateCategoryAbbreviation(Me.DbConnection, Nothing, contract.DomRateFrameworkID, vrc.DomServiceTypeID, vrc.DomDayCategoryID, vrc.DomTimeBandID, abbreviation)
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            vrcDesc = String.Format("{0} {1} - {2} mins{3}", _
                                                    abbreviation, _
                                                    vrc.MinutesFrom, _
                                                    vrc.MinutesTo, _
                                                    IIf(vrc.CareWorkers = DomContractVisitRateCategoryCareWorker.All, _
                                                        String.Empty, _
                                                        " (" & [Enum].GetName(GetType(DomContractVisitRateCategoryCareWorker), vrc.CareWorkers) & ")" _
                                                    ) _
                                        )
                            sortedVrcs.Add(String.Format("{0}{1}", vrcDesc, vrc.ID), New Pair(vrc.ID, vrcDesc))
                        Next
                        For Each p As Pair In sortedVrcs.Values
                            ' add the node
                            singleVrcNode = AddNode(vrcsNode.ChildNodes, p.Second, String.Format(URL_VRC, contractID, p.First, Convert.ToInt32(StdButtonsMode.Fetched)), False)
                            ' select?
                            If selectType = SELECT_TYPE_VRC AndAlso selectID = p.First Then
                                vrcsNode.Expanded = True
                                singleVrcNode.Selected = True
                            End If
                        Next
                    End If

                    ' audit log
                    AddNode(contractNode.ChildNodes, "Audit Log", String.Format(URL_AUDIT, contractID), False)

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

        Private Function IntranetNonResLicenced(ByVal conn As SqlConnection) As Boolean

            Dim msg As ErrorMessage
            Dim isLicensed As Boolean

            msg = Licensing.ModuleLicence.AreAnyModulesLicensed( _
                conn, _
                New Licensing.ModuleLicenses() {Licensing.ModuleLicenses.IntranetNonResidential}, _
                isLicensed _
            )

            Return isLicensed

        End Function

    End Class

End Namespace