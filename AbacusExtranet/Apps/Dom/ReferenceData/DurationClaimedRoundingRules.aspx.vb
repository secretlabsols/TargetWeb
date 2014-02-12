Imports Target.Library
Imports Target.Abacus.Library.SDS
Imports Target.Web.Apps.Security
Imports System.Collections.Generic
Imports Target.Library.Web.Controls
Imports Constants = Target.Library.Web
Imports System.Text.RegularExpressions
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Extranet.Apps.UserControls
Imports DbClass = Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Extranet.Apps.InPlaceSelectors

Namespace Apps.Dom.ReferenceData

    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class DurationClaimedRoundingRules
        Inherits Target.Web.Apps.BasePage

#Region " Fields "

        ' locals
        Private _currentUser As WebSecurityUser = Nothing
        Private _stdBut As StdButtonsBase
        Private _inEditMode As Boolean = False
        Private _isCouncilUser As Boolean
        Private _hasContracts As Boolean = False

        Private _newRuleIDCounter As Integer
        Private _newRoundingUsedByCounter As Integer

        ' constants
        Const QS_PageMode As String = "mode"
        Const VIEWSTATE_KEY_DATA As String = "DataList"
        Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"
        'RUB rounding used by 
        Const VIEWSTATE_KEY_DATA_RUB As String = "DataList_rub"
        Const VIEWSTATE_KEY_COUNTER_RUB As String = "NewCounter_rub"

        Const CTRL_PREFIX_FROM As String = "from"
        Const CTRL_PREFIX_TO As String = "to"
        Const CTRL_PREFIX_REMOVED As String = "remove"
        Const CTRL_PREFIX_BECOMES As String = "become"

        Const UNIQUEID_PREFIX_NEW As String = "N"
        Const UNIQUEID_PREFIX_UPDATE As String = "U"
        Const UNIQUEID_PREFIX_DELETE As String = "D"

        'RUB rounding used by 
        Const CTRL_PREFIX_RUB As String = "rub"
        Const CTRL_PREFIX_REMOVED_RUB As String = "remove"

        Const UNIQUEID_PREFIX_NEW_RUB As String = "N"
        Const UNIQUEID_PREFIX_UPDATE_RUB As String = "U"
        Const UNIQUEID_PREFIX_DELETE_RUB As String = "D"

        Private Const _AuditLogTable_DurationClaimedRounding As String = "DurationClaimedRounding"
        Private Const _AuditLogTable_DurationClaimedRoundingDetail As String = "DurationClaimedRoundingDetail"
        Private Const _AuditLogTable_DurationClaimedRoundingDomContract As String = "DurationClaimedRoundingDomContract"

        Private Const _WebCmdAddNewKey As String = "AbacusExtranet.WebNavMenuItemCommand.ReferenceData.DurationClaimedRoundingRules.AddNew"
        Private Const _WebCmdEditKey As String = "AbacusExtranet.WebNavMenuItemCommand.ReferenceData.DurationClaimedRoundingRules.Edit"
        Private Const _WebCmdDeleteKey As String = "AbacusExtranet.WebNavMenuItemCommand.ReferenceData.DurationClaimedRoundingRules.Delete"

        Dim _reference As String = String.Empty
        Dim _description As String = String.Empty
        Dim _externalAccount As String = String.Empty
        Dim _dcrId As Integer
        Dim _dcrCId As Integer

        Dim _auditUserName As String
        Dim _auditLogTitle As String

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets the in place external account selector control.
        ''' </summary>
        ''' 
        Private ReadOnly Property InPlaceExternalAccountSelectorControl() As InPlaceExternalAccountSelector
            Get
                Return CType(InPlaceExternalAccount1, InPlaceExternalAccountSelector)
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        ''' <remarks></remarks>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ReferenceData.DurationClaimRounding"), "Duration Claimed Rounding")
            Me.JsLinks.Add("DurationClaimedRoundingRules.js")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            _currentUser = SecurityBL.GetCurrentUser()
            _isCouncilUser = SecurityBL.IsCouncilUser(Me.DbConnection, Me.Settings, _currentUser.ExternalUserID)
            _auditUserName = _currentUser.ExternalUsername
            _auditLogTitle = AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)

            ' setup the standard buttons control
            With _stdBut
                .AllowBack = True
                .AllowDelete = UserHasDeleteCommand
                .AllowEdit = UserHasEditCommand
                .AllowFind = False
                .AllowNew = UserHasAddNewCommand
                .ShowNew = False
                .EditableControls.Add(fsControls.Controls)
                .EditableControls.Add(fsControlsRoundingUsedBy.Controls)
                .AuditLogTableNames.Add(_AuditLogTable_DurationClaimedRounding)
                .AuditLogTableNames.Add(_AuditLogTable_DurationClaimedRoundingDetail)
                .AuditLogTableNames.Add(_AuditLogTable_DurationClaimedRoundingDomContract)
                '.SearchBy.Add("Description", "Description")
                '.SearchBy.Add("Reference", "Reference")
                'add extra parameter
                'ExternalAccountId
                '.GenericFinderExtraParams.Add(_currentUser.ExternalUserID)
                'Is council user
                '.GenericFinderExtraParams.Add(_isCouncilUser)
                '.GenericFinderTypeID = GenericFinderType.DurationClaimedRounding
                AddHandler _stdBut.EditClicked, AddressOf EditClicked
                AddHandler _stdBut.FindClicked, AddressOf FindClicked
                AddHandler _stdBut.NewClicked, AddressOf NewClicked
                AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
                AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked
            End With

            ' re-create the list of duration claimed rounding rules (from view state)
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            For Each id As String In list
                OutputDcrRuleControls(id, Nothing)
            Next

            ' re-create the list of duration claimed rounding contracts (from view state)
            Dim listRub As List(Of String) = GetUniqueIDsFromViewStateRUB()
            For Each id As String In listRub
                OutputRoundingUsedByControls(id, Nothing)
            Next

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim js As New System.Text.StringBuilder()

            js.AppendFormat("externalAccountClientId=""{0}"";", InPlaceExternalAccountSelectorControl.ClientID)
            js.AppendFormat("externalAccountId={0};", Utils.ToInt32(InPlaceExternalAccountSelectorControl.hidExternalAccountID))
            js.AppendFormat("dcrId={0};", Utils.ToInt32(_stdBut.SelectedItemID))

            Me.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Extranet.Apps.Dom.ReferenceData.DurationClaimedRoundingRules.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js.ToString()) _
            )

        End Sub

        ''' <summary>
        ''' Handles the Page_prerender Complete
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        ''' <remarks></remarks>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            SetupInplaceExternalAccountSelector()
        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons CancelClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)

            FindClicked(e)

        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons DeleteClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()
            Dim trans As System.Data.SqlClient.SqlTransaction = Nothing

            Try
                ' get duration claimed rounding to delete
                Dim dcr As DbClass.DurationClaimedRounding = Nothing
                msg = DurationClaimedRoundingBL.GetDurationClaimedRounding(Me.DbConnection, dcr, e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                'get Duration claimed rounding Rules attached with this rounding. to delete
                Dim dcrRules As DurationClaimedRoundingDetailCollection = Nothing
                'get the duration claimed rounding rules
                msg = DurationClaimedRoundingBL.GetDurationClaimedRoundingDetails(Me.DbConnection, dcrRules, dcr.ID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                ' get the duration claimed rounding contracts attached with this rounding. to delete
                Dim dcrContracts As vwDurationClaimedRoundingDomContractCollection = Nothing

                'get the duration claimed rounding rules
                msg = DurationClaimedRoundingBL.GetDurationClaimedRoundingDomContracts(Me.DbConnection, dcrContracts, dcr.ID)
                If Not msg.Success Then WebUtils.DisplayError(msg)


                'open a transaction
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                'delete  dom contracts
                For Each contract As DbClass.vwDurationClaimedRoundingDomContract In dcrContracts
                    msg = DbClass.DurationClaimedRoundingDomContract.Delete( _
                    trans, _
                    _auditUserName, _
                    _auditLogTitle, _
                    contract.ID, _
                    contract.DurationClaimedRoundingID _
                    )
                Next
                'delete rules
                For Each dcrRule As DbClass.DurationClaimedRoundingDetail In dcrRules
                    msg = DbClass.DurationClaimedRoundingDetail.Delete( _
                    trans, _
                    _auditUserName, _
                    _auditLogTitle, _
                    dcrRule.ID, _
                    dcrRule.DurationClaimedRoundingID _
                    )
                Next

                msg = DbClass.DurationClaimedRounding.Delete( _
                trans, _
                _auditUserName, _
                _auditLogTitle, _
                dcr.ID)

                If msg.Success Then
                    trans.Commit()
                Else
                    WebUtils.DisplayError(msg)
                End If
            Catch ex As Exception
                If Not msg.Success Then
                    e.Cancel = True
                    WebUtils.DisplayError(msg)
                End If
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try
            If msg.Success Then
                Response.Redirect(Request.QueryString("backurl"))
            End If
        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons EditClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub EditClicked(ByRef e As StdButtonEventArgs)
            _inEditMode = True
            FindClicked(e)
        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons FindClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            ' display warning message 
            If Not Request.QueryString("msg") Is Nothing Then
                lblWarning.Text = Request.QueryString("msg")
            End If
            ' get qs id
            If Utils.ToInt32(Request.QueryString("id")) > 0 Then
                _dcrId = Utils.ToInt32(Request.QueryString("id"))
            End If
            If Utils.ToInt32(Request.QueryString("cid")) > 0 Then
                _dcrCId = Utils.ToInt32(Request.QueryString("cid"))
            End If

            If e.ItemID > 0 Then
                tabRoundingUsedBy.Visible = True
                PopulateDurationClaimRounding(e)
            ElseIf _dcrId > 0 Then
                e.ItemID = _dcrId
                PopulateDurationClaimRounding(e)
            ElseIf _dcrCId > 0 Then
                e.ItemID = _dcrCId
                PopulateDurationClaimRoundingDetails(_dcrCId, e, True)
                '' page is being loaded to copy DCR so hide Rounding used by 
                tabRoundingUsedBy.Visible = False
            Else
                '' page is being loaded to add new DCR so hide Rounding used by 
                tabRoundingUsedBy.Visible = False
            End If

        End Sub

        Private Sub PopulateDurationClaimRounding(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim dcr As DbClass.DurationClaimedRounding = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim eaUser As DbClass.Users = Nothing

            msg = DurationClaimedRoundingBL.GetDurationClaimedRounding(Me.DbConnection, dcr, e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' fill reference and description for duration claimed rounding
            txtDescription.Text = dcr.Description
            txtReference.Text = dcr.Reference

            ' get external account for duration claimed rounding
            msg = DurationClaimedRoundingBL.GetDurationClaimedRoundingExternalAccount(Me.DbConnection, _
                                                                                      eaUser, _
                                                                                      dcr.ExternalAccountID)
            ' fill external account details
            With InPlaceExternalAccountSelectorControl
                .hidExternalAccountID = dcr.ExternalAccountID
                .ExternalAccountText = eaUser.Name
            End With

            PopulateDurationClaimRoundingDetails(dcr.ID, e, False)
            PopulateDurationclaimedroundingDomContracts(dcr.ID, e)
        End Sub

        Private Sub PopulateDurationClaimRoundingDetails(ByVal dcrId As Integer, ByRef e As StdButtonEventArgs, ByVal IsCallForCopyRules As Boolean)
            Dim msg As ErrorMessage
            Dim dcrRules As DurationClaimedRoundingDetailCollection = Nothing
            Dim list As List(Of String)
            'get the duration claimed rounding rules
            msg = DurationClaimedRoundingBL.GetDurationClaimedRoundingDetails(Me.DbConnection, dcrRules, dcrId)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' refresh the list of existing dcr rules and save in view state
            ClearViewState(e)
            list = GetUniqueIDsFromViewState()
            For Each dcrRule As DbClass.DurationClaimedRoundingDetail In dcrRules
                ' while we want to copy the rules we will set the id to 0
                If IsCallForCopyRules Then dcrRule.ID = 0
                Dim id As String = GetUniqueID(dcrRule)
                OutputDcrRuleControls(id, dcrRule)
                list.Add(id)
            Next
            PersistUniqueIDsToViewState(list)
        End Sub

        Private Sub PopulateDurationclaimedroundingDomContracts(ByVal dcrId As Integer, ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim dcrContracts As vwDurationClaimedRoundingDomContractCollection = Nothing
            Dim list As List(Of String)
            'get the duration claimed rounding rules
            msg = DurationClaimedRoundingBL.GetDurationClaimedRoundingDomContracts(Me.DbConnection, dcrContracts, dcrId)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            '' if has contracts then set bool 
            If dcrContracts.Count > 0 Then
                _hasContracts = True
            Else
                _hasContracts = False
            End If

            ' refresh the list of existing dcr rules and save in view state
            ClearViewStateRUB(e)
            list = GetUniqueIDsFromViewStateRUB()
            For Each dcrContract As DbClass.vwDurationClaimedRoundingDomContract In dcrContracts
                Dim id As String = GetUniqueIDRUB(dcrContract)
                OutputRoundingUsedByControls(id, dcrContract)
                list.Add(id)
            Next
            PersistUniqueIDsToViewStateRUB(list)


        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons NewClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            FindClicked(e)
        End Sub

        ''' <summary>
        ''' EventHandler for the Standard Buttons SaveClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage = New ErrorMessage

            Dim dcr As DbClass.DurationClaimedRounding = _
            New DbClass.DurationClaimedRounding(Me.DbConnection, _
                                                _auditUserName, _
                                                _auditLogTitle)

            ' if dcr is for update 
            If e.ItemID > 0 Then
                msg = dcr.Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

            dcr.Reference = txtReference.Text.Trim()
            dcr.Description = txtDescription.Text.Trim()
            dcr.ExternalAccountID = InPlaceExternalAccountSelectorControl.hidExternalAccountID

            ' apply check so that reference should be unique across External account
            Dim dcrReferenceCheck As DurationClaimedRoundingCollection = New DurationClaimedRoundingCollection()
            msg = Abacus.Library.DataClasses.DurationClaimedRounding.FetchList(Me.DbConnection, _
                                                                               dcrReferenceCheck, _
                                                                               String.Empty, _
                                                                               String.Empty, _
                                                                               dcr.ExternalAccountID, _
                                                                               dcr.Reference)
            'for edit 1 record can exists which is being editted
            'for new 0 record can exists with same reference number
            If e.ItemID > 0 And dcrReferenceCheck.Count > 1 Or _
                e.ItemID = 0 And dcrReferenceCheck.Count = 1 Then
                lblError.Text = "Reference number already used. "
                e.Cancel = True
                Return
            End If

            Dim dcrDetailList As New DurationClaimedRoundingDetailCollection()
            Dim dcrRulesToDelete As List(Of String)
            Dim dcrRulesList As List(Of String)
            Dim dcrRule As DbClass.DurationClaimedRoundingDetail

            If Me.IsValid Then
                '    ' get list of dcr rules
                dcrRulesToDelete = New List(Of String)
                dcrRulesList = GetUniqueIDsFromViewState()
                For Each uniqueID As String In dcrRulesList
                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                        ' we are deleting
                        dcrRulesToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))
                    Else
                        ' create the empty dcr Rule detail
                        dcrRule = New DbClass.DurationClaimedRoundingDetail(Me.DbConnection, _
                                                                            _auditUserName, _
                                                                            _auditLogTitle)
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                            ' we are updating
                            msg = dcrRule.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE, String.Empty)))
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End If
                        ' set the dcr rule/detail properties
                        Try
                            With dcrRule
                                .MinutesFrom = CType(phRules.FindControl(CTRL_PREFIX_FROM & uniqueID), TextBoxEx).Text
                                .MinutesTo = CType(phRules.FindControl(CTRL_PREFIX_TO & uniqueID), TextBoxEx).Text
                                .Becomes = CType(phRules.FindControl(CTRL_PREFIX_BECOMES & uniqueID), TextBoxEx).Text
                                .AuditLogOverriddenParentID = dcrRule.DurationClaimedRoundingID
                            End With
                        Catch ex As Exception
                            msg.Message = "One or more rule minute range(s) are invalid."
                            msg.Success = False
                            lblError.Text = msg.Message
                            e.Cancel = True
                        End Try

                        ' add to the collection
                        dcrDetailList.Add(dcrRule)
                    End If
                Next

                ' dcr duration claimed rounding Dom contracts
                Dim rubCollection As New DurationClaimedRoundingDomContractCollection()
                Dim rubToDelete As New List(Of String)()
                Dim rubList As List(Of String)
                Dim rub As DbClass.DurationClaimedRoundingDomContract
                Dim existingContract As DbClass.DurationClaimedRoundingDomContract = Nothing
                Dim currentContractId As Integer = 0
                Dim existingContracts As DurationClaimedRoundingDomContractCollection = Nothing

                ' get any existing contracts
                msg = DbClass.DurationClaimedRoundingDomContract.FetchList(conn:=DbConnection, durationClaimedRoundingID:=e.ItemID, auditLogTitle:=_auditLogTitle, auditUserName:=_auditUserName, list:=existingContracts)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                '    ' get list of budgetPeriods
                rubList = GetUniqueIDsFromViewStateRUB()

                For Each uniqueID As String In rubList

                    If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                        ' we are deleting this object 

                        rubToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))

                    Else

                        ' get the contract id from the control....
                        currentContractId = CType(CType(phRoundingUsedBy.FindControl(CTRL_PREFIX_RUB & uniqueID), Control), InPlaceSelectors.InPlaceDcrDomContractSelector).SelectedContractID

                        ' get the first item from the current db items which matches on currentContractId...might not exist
                        existingContract = (From tmpContract As DbClass.DurationClaimedRoundingDomContract In existingContracts _
                                                Where tmpContract.DomContractID = currentContractId _
                                            Select tmpContract).FirstOrDefault()

                        ' get the contract object to exist
                        If Not existingContract Is Nothing Then
                            ' we have an existing item so we will update this

                            rub = existingContract

                        ElseIf uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                            ' we are updating an exisitng item but with a new contract id

                            rub = New DbClass.DurationClaimedRoundingDomContract(Me.DbConnection, _auditUserName, _auditLogTitle)
                            msg = rub.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE, String.Empty)))
                            If Not msg.Success Then WebUtils.DisplayError(msg)

                        Else
                            ' else this item is new so create a new object to be saved

                            rub = New DbClass.DurationClaimedRoundingDomContract(Me.DbConnection, _auditUserName, _auditLogTitle)

                        End If

                        ' set the period properties
                        With rub
                            .DomContractID = currentContractId
                            .AuditLogOverriddenParentID = rub.DurationClaimedRoundingID
                        End With

                        ' add to the collection
                        rubCollection.Add(rub)

                    End If

                Next

                For Each contract As DbClass.DurationClaimedRoundingDomContract In rubCollection
                    ' for each id to update remove from the delete collection if matched

                    If rubToDelete.Contains(contract.ID.ToString()) Then

                        rubToDelete.Remove(contract.ID.ToString())

                    End If

                Next

                '' Remove duplicates 
                Dim dcrDetailListDistinct As DurationClaimedRoundingDetailCollection = New DurationClaimedRoundingDetailCollection()
                Dim rubCollectionDistinct As DurationClaimedRoundingDomContractCollection = New DurationClaimedRoundingDomContractCollection()
                dcrDetailListDistinct.AddRange(dcrDetailList.ToArray.GroupBy(Function(tmp As DbClass.DurationClaimedRoundingDetail) New With {Key tmp.MinutesFrom, Key tmp.MinutesTo}).Select(Function(group) group.First()).ToArray())
                rubCollectionDistinct.AddRange(rubCollection.ToArray.GroupBy(Function(tmp As DbClass.DurationClaimedRoundingDomContract) New With {Key tmp.DomContractID}).Select(Function(group) group.First()).ToArray())

                ' save
                msg = DurationClaimedRoundingBL.SaveDurationClaimedRoundingDetail(conn:=Me.DbConnection, _
                                                                                  dcr:=dcr, _
                                                                                  dcrRules:=dcrDetailListDistinct, _
                                                                                  dcrRulesToDelete:=dcrRulesToDelete, _
                                                                                  rubContracts:=rubCollectionDistinct, _
                                                                                  rubContractsToDelete:=rubToDelete)



                If Not msg.Success Then
                    ' could not save 
                    lblError.Text = msg.Message
                    e.Cancel = True
                ElseIf Not msg.Message Is Nothing Then
                    lblWarning.Text = msg.Message
                End If
                '_clientID = cd.ID
            Else
                e.Cancel = True
            End If

            '' set e. itemid if sucess
            If msg.Success Then
                e.ItemID = dcr.ID
                FindClicked(e)
            End If
            ' this will reset the page in view mode.
            'HandleUrlAfterInsertNew(dcr.ID, msg.Message)
        End Sub

#End Region

#Region "Properties"

#Region "Authorisation Properties"

        ''' <summary>
        ''' Gets a value indicating whether can add new records.
        ''' </summary>
        ''' <value><c>true</c> if user can add new records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasAddNewCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(Constants.ConstantsManager.GetConstant(_WebCmdAddNewKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether can delete new records.
        ''' </summary>
        ''' <value><c>true</c> if user can delete new records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasDeleteCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(Constants.ConstantsManager.GetConstant(_WebCmdDeleteKey))
            End Get
        End Property

        ''' <summary>
        ''' Gets a value indicating whether can edit records.
        ''' </summary>
        ''' <value><c>true</c> if user can edit records; otherwise, <c>false</c>.</value>
        Private ReadOnly Property UserHasEditCommand() As Boolean
            Get
                Return UserHasMenuItemCommand(Constants.ConstantsManager.GetConstant(_WebCmdEditKey))
            End Get
        End Property

#End Region

        ''' <summary>
        ''' Gets the standard buttons control.
        ''' </summary>
        ''' <value>The standard buttons control.</value>
        Private ReadOnly Property StandardButtonsControl() As StdButtonsBase
            Get
                Return CType(stdButtons1, StdButtonsBase)
            End Get
        End Property

        ''' <summary>
        ''' Gets the audit log title.
        ''' </summary>
        ''' <value>The audit log title.</value>
        Private ReadOnly Property AuditLogTitle() As String
            Get
                Return AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)
            End Get
        End Property

        ''' <summary>
        ''' Gets the name of the audit log user.
        ''' </summary>
        ''' <value>The name of the audit log user.</value>
        Private ReadOnly Property AuditLogUserName() As String
            Get
                Return CurrentUser.ExternalUsername
            End Get
        End Property

        ''' <summary>
        ''' Gets the current user.
        ''' </summary>
        ''' <value>The current user.</value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _currentUser Is Nothing Then
                    ' if current user not fetched then get current user
                    _currentUser = SecurityBL.GetCurrentUser()
                End If
                Return _currentUser
            End Get
        End Property

        Private Function PageModeNew() As Boolean
            Dim returnValue As Boolean = False
            If Not Request.QueryString(QS_PageMode) Is Nothing Then
                If Request.QueryString(QS_PageMode) = "1" Then
                    'NewClicked()
                    returnValue = True
                End If
            End If
            Return returnValue
        End Function

        Private Function PageModeEdit() As Boolean
            Dim returnValue As Boolean = False
            If Not Request.QueryString(QS_PageMode) Is Nothing Then
                If Request.QueryString(QS_PageMode) = "2" Then
                    returnValue = True
                End If
            End If
            Return returnValue
        End Function

#End Region

#Region " Adding Duration Claim Rounding Rules "

#Region " ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA)
            phRules.Controls.Clear()
        End Sub

#End Region

#Region " btnAddDcrRule_Click "

        Private Sub btnAddRule_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddRule.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim newDcrRule As DbClass.DurationClaimedRoundingDetail = _
            New DbClass.DurationClaimedRoundingDetail(_auditUserName, _auditLogTitle)

            ' add a new row to the period list
            id = GetUniqueID(newDcrRule)
            ' create the controls
            OutputDcrRuleControls(id, newDcrRule)
            ' persist the data into view state
            list.Add(id)
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " Remove_Click "

        Private Sub btnRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetUniqueIDsFromViewState()
            Dim id As String = CType(sender, HtmlInputImage).ID.Replace(CTRL_PREFIX_REMOVED, String.Empty)

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE, UNIQUEID_PREFIX_DELETE)
                    End If
                    Exit For
                End If
            Next
            ' remove from the grid
            For index As Integer = 0 To phRules.Controls.Count - 1
                If phRules.Controls(index).ID = id Then
                    phRules.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewState(list)

        End Sub

#End Region

#Region " OutputPeriodControls "

        Private Sub OutputDcrRuleControls(ByVal uniqueID As String, ByVal dcrRule As DbClass.DurationClaimedRoundingDetail)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim minutesFrom As TextBoxEx
            Dim minutesTo As TextBoxEx
            Dim becomes As TextBoxEx
            Dim removeButton As HtmlInputImage


            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

                row = New HtmlTableRow()
                row.ID = uniqueID
                'phRules.Controls.Add(row)

                ' minutes from
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                minutesFrom = New TextBoxEx()
                With minutesFrom
                    .ID = CTRL_PREFIX_FROM & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    If Not dcrRule Is Nothing AndAlso Utils.IsNumeric(dcrRule.MinutesFrom) Then .Text = dcrRule.MinutesFrom
                End With
                cell.Controls.Add(minutesFrom)

                ' minutes to
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                minutesTo = New TextBoxEx()
                With minutesTo
                    .ID = CTRL_PREFIX_TO & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                    .ValidationGroup = "Save"
                    If Not dcrRule Is Nothing AndAlso Utils.IsNumeric(dcrRule.MinutesTo) Then .Text = dcrRule.MinutesTo
                End With
                cell.Controls.Add(minutesTo)

                ' becomes to
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                becomes = New TextBoxEx()
                With becomes
                    .ID = CTRL_PREFIX_BECOMES & uniqueID
                    .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
                    .ValidationGroup = "Save"
                    If Not dcrRule Is Nothing AndAlso Utils.IsNumeric(dcrRule.Becomes) Then .Text = dcrRule.Becomes
                End With
                cell.Controls.Add(becomes)

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cell.Style.Add("text-align", "right")
                removeButton = New HtmlInputImage()
                With removeButton
                    .ID = CTRL_PREFIX_REMOVED & uniqueID
                    .Src = WebUtils.GetVirtualPath("Images/delete.png")
                    .Alt = "Remove this entry"
                    AddHandler .ServerClick, AddressOf btnRemove_Click
                    .Attributes.Add("onclick", "return btnRemove_Click();")
                End With
                cell.Controls.Add(removeButton)

                phRules.Controls.Add(row)

                removeButton.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant(_WebCmdDeleteKey))

            End If

        End Sub

#End Region

#Region " GetUniqueID "

        Private Function GetUniqueID(ByVal dcrRule As DbClass.DurationClaimedRoundingDetail) As String

            Dim id As String

            If dcrRule.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW & _newRuleIDCounter
                _newRuleIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE & dcrRule.ID
            End If

            Return id

        End Function

#End Region

#Region " GetUniqueIDsFromViewState "

        Private Function GetUniqueIDsFromViewState() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER) Is Nothing Then
                _newRuleIDCounter = 0
            Else
                _newRuleIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER), Integer)
            End If

            Return list

        End Function

#End Region

#Region " PersistUniqueIDsToViewState "

        Private Sub PersistUniqueIDsToViewState(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER, _newRuleIDCounter)
        End Sub

#End Region

#End Region

#Region " Adding Rounding used by "

#Region " ClearViewStateRUB "

        Private Sub ClearViewStateRUB(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA_RUB)
            phRoundingUsedBy.Controls.Clear()
        End Sub

#End Region

#Region " btnAddContract_Click "

        Private Sub btnAddContract_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddContract.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewStateRUB()
            Dim newDcrContract As DbClass.vwDurationClaimedRoundingDomContract = New DbClass.vwDurationClaimedRoundingDomContract()

            ' add a new row to the contract list
            id = GetUniqueIDRUB(newDcrContract)
            ' create the controls
            OutputRoundingUsedByControls(id, newDcrContract)
            ' persist the data into view state
            list.Add(id)
            PersistUniqueIDsToViewStateRUB(list)

        End Sub

#End Region

#Region " RemoveRUB_Click "

        Private Sub btnRemoveRUB_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetUniqueIDsFromViewStateRUB()
            Dim id As String = CType(sender, HtmlInputImage).ID.Replace(CTRL_PREFIX_REMOVED_RUB, String.Empty)

            ' chnage/remove the id from view state
            For index As Integer = 0 To list.Count - 1
                If list(index) = id Then
                    If id.StartsWith(UNIQUEID_PREFIX_NEW_RUB) Then
                        ' newly added items just get deleted
                        list.RemoveAt(index)
                    Else
                        ' items that exist in the database are marked as needing deletion
                        list(index) = list(index).Replace(UNIQUEID_PREFIX_UPDATE_RUB, UNIQUEID_PREFIX_DELETE_RUB)
                    End If
                    Exit For
                End If
            Next
            ' remove from the grid
            For index As Integer = 0 To phRoundingUsedBy.Controls.Count - 1
                If phRoundingUsedBy.Controls(index).ID = id Then
                    phRoundingUsedBy.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewStateRUB(list)

        End Sub

#End Region

#Region " OutputRoundingUsedByControls "

        Private Sub OutputRoundingUsedByControls(ByVal uniqueID As String, ByVal dcrContract As DbClass.vwDurationClaimedRoundingDomContract)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim removeButtonRub As HtmlInputImage


            Dim FeaturedProductUserControl As Control = LoadControl("~/AbacusExtranet/Apps/InPlaceSelectors/InPlaceDcrDomContractSelector.ascx")

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE_RUB) Then

                row = New HtmlTableRow()
                row.ID = uniqueID
                phRoundingUsedBy.Controls.Add(row)

                '' Contract selector
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")

                Dim InPlaceSelectorControl As Control = LoadControl("~/AbacusExtranet/Apps/InPlaceSelectors/InPlaceDcrDomContractSelector.ascx")
                InPlaceSelectorControl.ID = CTRL_PREFIX_RUB & uniqueID
                cell.Controls.Add(InPlaceSelectorControl)
                If Not dcrContract Is Nothing Then
                    CType(InPlaceSelectorControl, InPlaceSelectors.InPlaceDcrDomContractSelector).SelectedContractID = dcrContract.DomContractID
                    CType(InPlaceSelectorControl, InPlaceSelectors.InPlaceDcrDomContractSelector).SelectedContractNumber = dcrContract.Number
                    CType(InPlaceSelectorControl, InPlaceSelectors.InPlaceDcrDomContractSelector).SelectedContractTitle = dcrContract.Title
                End If


                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cell.Style.Add("text-align", "right")
                removeButtonRub = New HtmlInputImage()
                With removeButtonRub
                    .ID = CTRL_PREFIX_REMOVED_RUB & uniqueID
                    .Src = WebUtils.GetVirtualPath("Images/delete.png")
                    .Alt = "Remove this entry"
                    AddHandler .ServerClick, AddressOf btnRemoveRUB_Click
                    .Attributes.Add("onclick", "return btnRemoveRUB_Click();")
                End With
                cell.Controls.Add(removeButtonRub)

                removeButtonRub.Visible = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant(_WebCmdDeleteKey))

            End If
        End Sub

#End Region

#Region " GetUniqueIDRUB "

        Private Function GetUniqueIDRUB(ByVal dcrContract As DbClass.vwDurationClaimedRoundingDomContract) As String

            Dim id As String

            If dcrContract.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW_RUB & _newRoundingUsedByCounter
                _newRoundingUsedByCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE_RUB & dcrContract.ID
            End If

            Return id

        End Function

#End Region

#Region " GetUniqueIDsFromViewStateRUB "

        Private Function GetUniqueIDsFromViewStateRUB() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_RUB) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_RUB), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER_RUB) Is Nothing Then
                _newRoundingUsedByCounter = 0
            Else
                _newRoundingUsedByCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_RUB), Integer)
            End If

            Return list

        End Function

#End Region

#Region " PersistUniqueIDsToViewStateRUB "

        Private Sub PersistUniqueIDsToViewStateRUB(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA_RUB, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_RUB, _newRoundingUsedByCounter)
        End Sub

#End Region

#End Region

#Region " Security Handlers "

        ''' <summary>
        ''' Sets up the inplace external account selector to follow business rules
        ''' to control is availablity and value.
        ''' </summary>
        Private Sub SetupInplaceExternalAccountSelector()

            ' get current user
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            ' determine if the current user is a council user.
            _isCouncilUser = SecurityBL.IsCouncilUser(DbConnection, Settings, currentUser.ExternalUserID)

            If _stdBut.SelectedItemID > 0 Then
                ' if we have an item id to work with

                Dim dcr As New DbClass.DurationClaimedRounding(DbConnection, String.Empty, String.Empty)
                Dim dcrUser As New DbClass.Users(DbConnection)
                Dim msg As New ErrorMessage()

                ' fetch the item from db
                msg = dcr.Fetch(_stdBut.SelectedItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' fetch the user object associated with dcr
                msg = dcrUser.Fetch(dcr.ExternalAccountID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' setup default for the external account selector control
                With InPlaceExternalAccountSelectorControl
                    .hidExternalAccountID = dcrUser.ID
                    .ExternalAccountText = dcrUser.FullName
                End With

                ' determine if any contracts are defined by finding the number of inserted 
                ' or updated items...we want to ignore deleted for obvious reasons!
                _hasContracts = (From tmp As String In GetUniqueIDsFromViewStateRUB() _
                                    Where _
                                        tmp.StartsWith(UNIQUEID_PREFIX_NEW_RUB) _
                                        OrElse tmp.StartsWith(UNIQUEID_PREFIX_UPDATE_RUB) _
                                    Select tmp).Count() > 0

            End If

            ' set if the current user is a council user or not
            With InPlaceExternalAccountSelectorControl
                .ExternalAccountSelectorVisible = _isCouncilUser
            End With

            If Not _isCouncilUser _
                Or (_isCouncilUser And _hasContracts) Then
                ' if the user is not a council user or is but has some contracts
                ' then we do not want the external account to be modified...

                With InPlaceExternalAccountSelectorControl
                    If Not _isCouncilUser Then
                        ' as this is not a council user we always want to default to 
                        ' the current users details i.e. they cannot select any other user
                        .hidExternalAccountID = currentUser.ExternalUserID
                        .ExternalAccountText = currentUser.ExternalUsername
                    End If
                    ' disable all controls
                    .Enabled = False
                End With

            End If

        End Sub

#End Region

#Region " Manage URL "
        Private Sub HandleUrlAfterInsertNew(ByVal dcrId As Integer, ByVal message As String)
            Dim backUrl As String = String.Empty
            If Not Request.QueryString("backurl") Is Nothing Then
                backUrl = Request.QueryString("backurl")
            End If
            Dim noQueryUrl As String = Regex.Match(Page.Request.Url.ToString, "^(\S+)(\?)(\S+)$").Groups(1).ToString()
            Response.Redirect(String.Format("{0}?mode=1&id={1}&backurl={2}&msg={3}", noQueryUrl, dcrId, backUrl, message))
        End Sub
#End Region

    End Class

End Namespace