Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports System.Collections.Generic
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.UserControls
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library
Imports Target.Library
Imports System.Data.SqlClient
Imports System.Text

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Admin page used to maintain Expenditure Accounts.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JF    18/09/2013  Ensure population of service type dropdown (#8183)
    ''' JF    05/09/2013  Updated ExpenditureAccountType enum (#8159)
    ''' MvO   12/05/2009  D11549 - added reporting support.
    ''' Paul  05/02/2009  D11491 - Original Version.
    ''' </history>
    Partial Public Class ExpenditureAccount
        Inherits Target.Web.Apps.BasePage

#Region " Constants "

        Const ACCOUNT_TYPE_COUNCIL As Integer = 1
        Const ACCOUNT_TYPE_PCT As Integer = 2
        Const ACCOUNT_TYPE_CLIENT_THIRDPARTY As Integer = 3
        Const ACCOUNT_TYPE_OLA As Integer = 4
        Const ACCOUNT_TYPE_OTHER As Integer = 5

        Const VIEWSTATE_KEY_DATA_ST As String = "DataListST"
        Const VIEWSTATE_KEY_COUNTER_ST As String = "NewCounterST"
        Const VIEWSTATE_KEY_DATA_FC As String = "DataListFC"
        Const VIEWSTATE_KEY_COUNTER_FC As String = "NewCounterFC"

        Const ROW_PREFIX_FC As String = "rowFC"
        Const ROW_PREFIX_ST As String = "rowST"

        Const CTRL_PREFIX_SERVICE_TYPES As String = "servType"
        Const CTRL_PREFIX_REMOVED_ST As String = "removeST"

        Const CTRL_PREFIX_EXPACC_ID As String = "expenditurecodeid"
        Const CTRL_PREFIX_INCOME_DUE As String = "incomeDue"
        Const CTRL_PREFIX_EXPENDITURE As String = "expenditure"
        Const CTRL_PREFIX_REMOVED_FC As String = "removeFC"

        Const UNIQUEID_PREFIX_NEW As String = "N"
        Const UNIQUEID_PREFIX_UPDATE As String = "U"
        Const UNIQUEID_PREFIX_DELETE As String = "D"

#End Region

#Region " Global Variables "


        Private _newSvcTypeIDCounter As Integer
        Private _newFinanceCodeIDCounter As Integer
        Private _startup2JS As StringBuilder = New StringBuilder()

        Private _serviceTypes As DomServiceTypeCollection = Nothing
        Private _expenditureAccountGroupID As Integer
        Private _stdBut As StdButtonsBase

        Private _btnAuditDetails As HtmlInputButton = New HtmlInputButton("button")

#End Region

#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                AddHandler .AddCustomControls, AddressOf StdButtons_AddCustomControls
            End With

        End Sub

#End Region

#Region " Page_Load "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim js As String
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ExpenditureAccount"), "Expenditure Accounts")
            Dim msg As ErrorMessage = New ErrorMessage
            Dim sysInfo As SystemInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, True)
            Dim expAccount As DataClasses.ExpenditureAccount = New DataClasses.ExpenditureAccount(Me.DbConnection)
            msg = expAccount.Fetch(sysInfo.DefaultExpenditureAccountID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                If expAccount.ExpenditureAccountGroupID = CType(Me.stdButtons1, StdButtonsBase).SelectedItemID AndAlso Me.IsPostBack Then
                    .AllowDelete = False
                    .AllowEdit = False
                Else
                    .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ExpenditureAccount.Delete"))
                    .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ExpenditureAccount.Edit"))
                End If
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ExpenditureAccount.AddNew"))

                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.ExpenditureAccount
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ExpenditureAccounts")
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            With CType(Me.client, InPlaceSelectors.InPlaceClientThirdPartySelector)
                .Client_Selector.ValidationGroup = "Save"
                .ValidationGroup = "Save"
            End With

            With CType(Me.pct, InPlaceSelectors.InPlacePctSelector)
                .ValidationGroup = "Save"
            End With

            ' re-create the list of Service Types (from view state)
            Dim list As List(Of String) = GetUniqueIDsFromViewState_ST()
            For Each id As String In list
                OutputServiceTypeControls(id, Nothing)
            Next
            ' re-create the list of Finance Codes (from view state)
            Dim listFC As List(Of String) = GetUniqueIDsFromViewState_FC()
            For Each id As String In listFC
                OutputFinanceCodeControls(id, Nothing)
            Next

            Me.JsLinks.Add("ExpenditureAccount.js")

            js = String.Format("clientSelectorID='{0}';pctSelectorID='{1}';olaSelectorID='{2}';orgSelectorID='{3}';", _
                                    client.ClientID, pct.ClientID, ola.ClientID, org.ClientID)
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "EditSuspension.Startup", _
                            Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

#End Region

#Region " Page_PreRenderXXX "

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim msg As ErrorMessage = New ErrorMessage
            Dim inUse As Boolean

            If _expenditureAccountGroupID > 0 Then
                msg = FinanceCodeBL.IsExpenditureAccountGroupInUse(Me.DbConnection, _expenditureAccountGroupID, inUse)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If inUse Then
                    optClient.Disabled = True
                    optCouncil.Disabled = True
                    optOLA.Disabled = True
                    optOther.Disabled = True
                    optPCT.Disabled = True
                End If
            End If

            If _stdBut.ButtonsMode <> StdButtonsMode.Edit And _stdBut.ButtonsMode <> StdButtonsMode.Fetched Then
                CType(auditDetails, IBasicAuditDetails).Collapsed = True
                CType(_btnAuditDetails, HtmlInputButton).Disabled = True
            Else
                CType(auditDetails, IBasicAuditDetails).Collapsed = True
                CType(_btnAuditDetails, HtmlInputButton).Disabled = False
            End If

            If _stdBut.ButtonsMode <> StdButtonsMode.AddNew Then
                optClient.Disabled = True
                optCouncil.Disabled = True
                optOLA.Disabled = True
                optOther.Disabled = True
                optPCT.Disabled = True
            Else
                optClient.Disabled = False
                optCouncil.Disabled = False
                optOLA.Disabled = False
                optOther.Disabled = False
                optPCT.Disabled = False
            End If

        End Sub

#End Region

#Region " StdButtons_AddCustomControls "

        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)

            With _btnAuditDetails
                .ID = "btnAuditDetails"
                .Value = "Audit Details"
            End With
            controls.Add(_btnAuditDetails)

            With CType(auditDetails, IBasicAuditDetails)
                .ToggleControlID = _btnAuditDetails.ClientID
                .Collapsed = True
            End With

        End Sub

#End Region

#Region " ClearViewState "

        Private Sub ClearViewState(ByRef e As StdButtonEventArgs)
            ViewState.Remove(VIEWSTATE_KEY_DATA_ST)
            phSvcTypes.Controls.Clear()
            ViewState.Remove(VIEWSTATE_KEY_DATA_FC)
            phFinanceCodes.Controls.Clear()
        End Sub

#End Region

#Region " Std Buttons Event Handlers "

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            SetupValidators()
            CancelClicked(e)
            ClearViewState(e)
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim ExpAccountGrp As ExpenditureAccountGroup
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim svcTypeList As ExpenditureAccountGroupDomServiceTypeCollection = New ExpenditureAccountGroupDomServiceTypeCollection()
            Dim finCodeList As ExpenditureAccountCollection = New ExpenditureAccountCollection
            Dim list As List(Of String)


            ExpAccountGrp = New ExpenditureAccountGroup(Me.DbConnection)
            With ExpAccountGrp
                _expenditureAccountGroupID = e.ItemID
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                UnselectOptionButtons()
                Select Case .Type
                    Case ExpenditureAccountGroupType.Council
                        optCouncil.Checked = True
                    Case ExpenditureAccountGroupType.ClinicalCommissioningGroup
                        optPCT.Checked = True
                        SetupPCTSelector(.PCTID)
                    Case ExpenditureAccountGroupType.ClientSpecificThirdParty
                        optClient.Checked = True
                        SetupClientThirdPartySelector(.ClientID, .ThirdPartyID)
                    Case ExpenditureAccountGroupType.OtherLocalAuthority
                        optOLA.Checked = True
                        SetupOLASelector(.OtherFundingOrganizationID)
                    Case ExpenditureAccountGroupType.Other
                        optOther.Checked = True
                        SetupOtherOrgSelector(.OtherFundingOrganizationID)
                End Select

                SetupValidators()

                txtDescription.Text = .Description

                CType(auditDetails, IBasicAuditDetails).EnteredBy = .CreatedBy
                CType(auditDetails, IBasicAuditDetails).DateEntered = .DateCreated.ToString("dd MMM yyyy")
                If Utils.IsDate(.DateAmended) Then
                    CType(auditDetails, IBasicAuditDetails).DateLastAmended = .DateAmended.ToString("dd MMM yyyy")
                    CType(auditDetails, IBasicAuditDetails).LastAmendedBy = .AmendedBy
                End If

                chkRedundant.CheckBox.Checked = .Redundant

                msg = ExpenditureAccountGroupDomServiceType.FetchList(Me.DbConnection, svcTypeList, e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' refresh the list of existing bands and save in view state
                ClearViewState(e)
                list = GetUniqueIDsFromViewState_ST()
                For Each expSvcType As ExpenditureAccountGroupDomServiceType In svcTypeList
                    Dim id As String = GetUniqueID_ST(expSvcType)
                    OutputServiceTypeControls(id, expSvcType)
                    list.Add(id)
                Next
                PersistUniqueIDsToViewState_ST(list)


                msg = Target.Abacus.Library.DataClasses.ExpenditureAccount.FetchList(Me.DbConnection, finCodeList, e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' refresh the list of existing bands and save in view state
                list = GetUniqueIDsFromViewState_FC()
                For Each financeCode As Target.Abacus.Library.DataClasses.ExpenditureAccount In finCodeList
                    Dim id As String = GetUniqueID_FC(financeCode)
                    OutputFinanceCodeControls(id, financeCode)
                    list.Add(id)
                Next
                PersistUniqueIDsToViewState_FC(list)

            End With

            ' print button
            With CType(ctlPrint, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ExpenditureAccount")
                .Parameters.Add("intExpenditureAccountGroupID", _expenditureAccountGroupID)
            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                ClearViewState(e)
                UnselectOptionButtons()
                SetupClientThirdPartySelector(0, 0)
                SetupOLASelector(0)
                SetupOtherOrgSelector(0)
                SetupPCTSelector(0)
                optCouncil.Checked = True
                txtDescription.Text = String.Empty

                CType(auditDetails, IBasicAuditDetails).EnteredBy = String.Empty
                CType(auditDetails, IBasicAuditDetails).DateEntered = Nothing
                CType(auditDetails, IBasicAuditDetails).DateLastAmended = Nothing
                CType(auditDetails, IBasicAuditDetails).LastAmendedBy = String.Empty
                chkRedundant.CheckBox.Checked = False
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage = New ErrorMessage
            Dim trans As SqlTransaction = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                'Delete Account
                msg = FinanceCodeBL.DeleteExpenditureAccountGroup(trans, currentUser.ExternalUsername, e.ItemID)
                If Not msg.Success Then
                    If msg.Number = FinanceCodeBL.ERR_COULD_NOT_DELETE_EXPENDITURE_ACCOUNT Then
                        ' could not save expenditure account
                        lblError.Text = msg.Message.Replace(vbCrLf, "<br />")
                        msg = New ErrorMessage
                        msg.Success = True
                        SqlHelper.RollbackTransaction(trans)
                        e.Cancel = True
                        FindClicked(e)
                    Else
                        e.Cancel = True
                        WebUtils.DisplayError(msg)
                    End If
                Else

                    trans.Commit()

                    e.ItemID = 0
                    CancelClicked(e)
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage
            Dim expAccountGrp As Abacus.Library.DataClasses.ExpenditureAccountGroup
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim intExpAccountGroupID As Integer
            Dim svcTypeList As List(Of String)
            Dim serviceType As ExpenditureAccountGroupDomServiceType
            Dim serviceTypes As ExpenditureAccountGroupDomServiceTypeCollection = New ExpenditureAccountGroupDomServiceTypeCollection
            Dim svcTypesToDelete As List(Of String)
            Dim financeCode As Target.Abacus.Library.DataClasses.ExpenditureAccount = New Target.Abacus.Library.DataClasses.ExpenditureAccount
            Dim financeCodes As ExpenditureAccountCollection = New ExpenditureAccountCollection
            Dim financeCodesToDelete As List(Of Integer)
            Dim financeCodeList As List(Of String)
            Dim transRolledBack As Boolean = False

            SetupValidators()

            'Me.Validate("Save")
            'If Me.IsValid Then
            expAccountGrp = New Abacus.Library.DataClasses.ExpenditureAccountGroup(Me.DbConnection)
            If e.ItemID > 0 Then
                ' update
                With expAccountGrp
                    msg = .Fetch(e.ItemID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With
            End If

            With expAccountGrp
                If optCouncil.Checked = True Then
                    .Type = ACCOUNT_TYPE_COUNCIL
                ElseIf optPCT.Checked = True Then
                    .Type = ACCOUNT_TYPE_PCT
                    .PCTID = Utils.ToInt32(Request.Form(CType(Me.pct, InPlaceSelectors.InPlacePctSelector).HiddenFieldUniqueID))

                ElseIf optClient.Checked = True Then
                    .Type = ACCOUNT_TYPE_CLIENT_THIRDPARTY
                    .ClientID = Utils.ToInt32(Request.Form(CType(Me.client, InPlaceSelectors.InPlaceClientThirdPartySelector).Client_Selector.HiddenFieldUniqueID))
                    .ThirdPartyID = Utils.ToInt32(Request.Form(CType(Me.client, InPlaceSelectors.InPlaceClientThirdPartySelector).HiddenFieldUniqueID))
                ElseIf optOLA.Checked = True Then
                    .Type = ACCOUNT_TYPE_OLA
                    .OtherFundingOrganizationID = Utils.ToInt32(Request.Form(CType(Me.ola, InPlaceSelectors.InPlaceOtherFundingOrganizationSelector).HiddenFieldUniqueID))
                ElseIf optOther.Checked = True Then
                    .Type = ACCOUNT_TYPE_OTHER
                    .OtherFundingOrganizationID = Utils.ToInt32(Request.Form(CType(Me.org, InPlaceSelectors.InPlaceOtherFundingOrganizationSelector).HiddenFieldUniqueID))
                End If
                .Description = txtDescription.Text
                .Redundant = chkRedundant.CheckBox.Checked
            End With

            'Do the Save using the busines logic class FinanceCodeBL
            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)

                msg = FinanceCodeBL.SaveExpenditureAccountGroup(trans, expAccountGrp, currentUser.ExternalUsername, intExpAccountGroupID)
                If Not msg.Success Then
                    If msg.Number = FinanceCodeBL.ERR_COULD_NOT_SAVE_EXPENDITURE_ACCOUNT Then
                        ' could not save expenditure account
                        lblError.Text = msg.Message.Replace(vbCrLf, "<br />")
                        msg = New ErrorMessage
                        msg.Success = True
                        trans.Rollback()
                        transRolledBack = True
                    Else
                        trans.Rollback()
                        transRolledBack = True
                        WebUtils.DisplayError(msg)
                    End If
                Else

                    ' get list of Service Types

                    svcTypesToDelete = New List(Of String)
                    svcTypeList = GetUniqueIDsFromViewState_ST()
                    For Each uniqueID As String In svcTypeList
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                            ' we are deleting
                            svcTypesToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, String.Empty))
                        Else
                            ' create the empty Service Type
                            serviceType = New ExpenditureAccountGroupDomServiceType(trans)
                            If uniqueID.StartsWith(UNIQUEID_PREFIX_UPDATE) Then
                                ' we are updating
                                msg = serviceType.Fetch(Convert.ToInt32(uniqueID.Replace(UNIQUEID_PREFIX_UPDATE, String.Empty)))
                                If Not msg.Success Then WebUtils.DisplayError(msg)
                            End If
                            ' set the Service Type properties
                            With serviceType
                                .DomServiceTypeID = CType(phSvcTypes.FindControl(CTRL_PREFIX_SERVICE_TYPES & uniqueID), DropDownListEx).DropDownList.SelectedValue
                                .ExpenditureAccountGroupID = intExpAccountGroupID
                            End With
                            ' add to the collection
                            serviceTypes.Add(serviceType)
                        End If
                    Next

                    ' get list of Finance Codes
                    financeCodesToDelete = New List(Of Integer)
                    financeCodeList = GetUniqueIDsFromViewState_FC()
                    For Each uniqueID As String In financeCodeList
                        If uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then
                            ' we are deleting
                            financeCodesToDelete.Add(uniqueID.Replace(UNIQUEID_PREFIX_DELETE, ""))
                        Else
                            ' create the empty financeCode

                            financeCode = New Target.Abacus.Library.DataClasses.ExpenditureAccount(trans)

                            ' set the Finance Code properties
                            With financeCode
                                .ID = Utils.ToInt32(CType(phFinanceCodes.FindControl(CTRL_PREFIX_EXPACC_ID & uniqueID), HiddenField).Value)
                                .FinanceCodeID_Expenditure = CType(phFinanceCodes.FindControl(CTRL_PREFIX_EXPENDITURE & uniqueID), DropDownListEx).DropDownList.SelectedValue
                                .FinanceCodeID_IncomeDue = CType(phFinanceCodes.FindControl(CTRL_PREFIX_INCOME_DUE & uniqueID), DropDownListEx).DropDownList.SelectedValue
                                .ExpenditureAccountGroupID = intExpAccountGroupID

                            End With
                            ' add to the collection
                            financeCodes.Add(financeCode)
                        End If
                    Next

                    msg = FinanceCodeBL.SaveExpenditureAccountGroupDomServiceTypes(trans, intExpAccountGroupID, serviceTypes, svcTypesToDelete)
                    If Not msg.Success Then
                        If msg.Number = FinanceCodeBL.ERR_COULD_NOT_SAVE_EXPENDITURE_ACCOUNT Then
                            ' could not save Expenditure Account Service Types
                            lblError.Text = msg.Message.Replace(vbCrLf, "<br />")
                            msg = New ErrorMessage
                            msg.Success = True
                            trans.Rollback()
                            transRolledBack = True
                        Else
                            trans.Rollback()
                            transRolledBack = True
                            WebUtils.DisplayError(msg)
                        End If
                    Else

                        msg = FinanceCodeBL.SaveExpenditureAccount(trans, intExpAccountGroupID, financeCodes, financeCodesToDelete)
                        If Not msg.Success Then
                            If msg.Number = FinanceCodeBL.ERR_COULD_NOT_SAVE_EXPENDITURE_ACCOUNT Then
                                ' could not save Expenditure Account Service Types
                                lblError.Text = msg.Message.Replace(vbCrLf, "<br />")
                                msg = New ErrorMessage
                                msg.Success = True
                                trans.Rollback()
                                transRolledBack = True
                            Else
                                trans.Rollback()
                                transRolledBack = True
                                WebUtils.DisplayError(msg)
                            End If
                        End If
                    End If
                End If
                If Not transRolledBack Then
                    trans.Commit()
                    e.ItemID = intExpAccountGroupID
                    FindClicked(e)
                Else
                    e.Cancel = True
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)     ' unexpected
                e.Cancel = True
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

            'Else
            '    e.Cancel = True
            'End If

        End Sub

#End Region

#Region " UnselectOptionButtons "

        Public Sub UnselectOptionButtons()
            optCouncil.Checked = False
            optClient.Checked = False
            optPCT.Checked = False
            optOLA.Checked = False
            optOther.Checked = False
        End Sub

#End Region

#Region " Service Types Table Code "

#Region "           btnAddSvcTypes_Click "

        Private Sub btnAddSvcTypes_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddSvcTypes.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState_ST()
            Dim newSvcType As ExpenditureAccountGroupDomServiceType = New ExpenditureAccountGroupDomServiceType(Me.DbConnection)

            ' add a new row to the Service Type list
            id = GetUniqueID_ST(newSvcType)
            ' create the controls
            OutputServiceTypeControls(id, newSvcType)
            ' persist the data into view state
            list.Add(id)
            PersistUniqueIDsToViewState_ST(list)

        End Sub

#End Region

#Region "           OutputServiceTypeControls "

        Private Sub OutputServiceTypeControls(ByVal uniqueID As String, ByVal sType As ExpenditureAccountGroupDomServiceType)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim removeButton As Button
            Dim cboServiceType As DropDownListEx

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

                row = New HtmlTableRow()
                row.ID = ROW_PREFIX_ST & uniqueID
                phSvcTypes.Controls.Add(row)

                ' Service Type
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboServiceType = New DropDownListEx()
                With cboServiceType
                    .ID = CTRL_PREFIX_SERVICE_TYPES & uniqueID
                    .ValidationGroup = "Save"
                    .Required = True
                    .RequiredValidatorErrMsg = "Please Enter a Service Type"
                    If sType Is Nothing Then
                        LoadServiceTypesDropdown(cboServiceType, 0)
                    Else
                        LoadServiceTypesDropdown(cboServiceType, sType.DomServiceTypeID)
                    End If

                    If Not sType Is Nothing AndAlso sType.DomServiceTypeID > 0 Then .DropDownList.SelectedValue = sType.DomServiceTypeID
                    '.DropDownList.Attributes.Add("onchange", "cboServiceType_Change();")
                End With
                cell.Controls.Add(cboServiceType)

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                removeButton = New Button()
                With removeButton
                    .ID = CTRL_PREFIX_REMOVED_ST & uniqueID
                    .Text = "Remove"
                    AddHandler .Click, AddressOf Remove_ST_Click
                End With
                cell.Controls.Add(removeButton)

            End If

        End Sub

#End Region

#Region "           Remove_ST_Click "

        Private Sub Remove_ST_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetUniqueIDsFromViewState_ST()
            Dim id As String = CType(sender, Button).ID.Replace(CTRL_PREFIX_REMOVED_ST, String.Empty)

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
            For index As Integer = 0 To phSvcTypes.Controls.Count - 1
                If phSvcTypes.Controls(index).ID.Replace(ROW_PREFIX_ST, String.Empty) = id Then
                    phSvcTypes.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewState_ST(list)

        End Sub

#End Region

#Region "           GetUniqueID_ST "

        Private Function GetUniqueID_ST(ByVal newSvcType As ExpenditureAccountGroupDomServiceType) As String

            Dim id As String

            If newSvcType.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW & _newSvcTypeIDCounter
                _newSvcTypeIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE & newSvcType.ID
            End If

            Return id

        End Function

#End Region

#Region "           PersistUniqueIDsToViewState_ST "

        Private Sub PersistUniqueIDsToViewState_ST(ByVal list As List(Of String))
            ViewState.Add(VIEWSTATE_KEY_DATA_ST, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_ST, _newSvcTypeIDCounter)
        End Sub

#End Region

#Region "           GetUniqueIDsFromViewState_ST "

        Private Function GetUniqueIDsFromViewState_ST() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_ST) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_ST), List(Of String))
            End If
            If ViewState.Item(VIEWSTATE_KEY_COUNTER_ST) Is Nothing Then
                _newSvcTypeIDCounter = 0
            Else
                _newSvcTypeIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_ST), Integer)
            End If

            Return list

        End Function

#End Region

#Region "           LoadServiceTypesDropdown "

        Private Sub LoadServiceTypesDropdown(ByVal dropdown As DropDownListEx, ByVal selectedServiceTypeID As Integer)

            Dim msg As ErrorMessage

            If _serviceTypes Is Nothing Then
                msg = DomServiceType.FetchList(conn:=Me.DbConnection, list:=_serviceTypes, _
                                               auditUserName:=String.Empty, auditLogTitle:=String.Empty, redundant:=False)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                _serviceTypes.Sort(New CollectionSorter("Description", SortDirection.Ascending))
            End If

            If selectedServiceTypeID > 0 Then
                'Check to see if the selected value exists in the collection
                Dim itemFound As Boolean = False
                For Each svcType As DomServiceType In _serviceTypes
                    If svcType.ID = selectedServiceTypeID Then
                        itemFound = True
                        Exit For
                    End If
                Next
                'if it doesnt already exist add it to the collection
                If Not itemFound Then
                    Dim svcType As DomServiceType = New DomServiceType(Me.DbConnection, String.Empty, String.Empty)
                    msg = svcType.Fetch(selectedServiceTypeID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    _serviceTypes.Add(svcType)
                End If
            End If

            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = _serviceTypes
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

        End Sub

#End Region

#End Region

#Region " Finance Code Table Code "

#Region "           btnAddFinanceCodes_Click "

        Private Sub btnAddFinanceCodes_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddFinanceCodes.Click

            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim id As String
            Dim list As List(Of String) = GetUniqueIDsFromViewState_FC()
            Dim newExpAcc As Target.Abacus.Library.DataClasses.ExpenditureAccount = New Target.Abacus.Library.DataClasses.ExpenditureAccount()

            ' add a new row to the Service Type list
            id = GetUniqueID_FC(newExpAcc)
            ' create the controls
            OutputFinanceCodeControls(id, Nothing)
            ' persist the data into view state
            list.Add(id)
            PersistUniqueIDsToViewState_FC(list)

        End Sub

#End Region

#Region "           Remove_FC_Click "

        Private Sub Remove_FC_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim list As List(Of String) = GetUniqueIDsFromViewState_FC()
            Dim id As String = CType(sender, Button).ID.Replace(CTRL_PREFIX_REMOVED_FC, String.Empty)

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
            For index As Integer = 0 To phFinanceCodes.Controls.Count - 1
                If phFinanceCodes.Controls(index).ID.Replace(ROW_PREFIX_FC, String.Empty) = id Then
                    phFinanceCodes.Controls.RemoveAt(index)
                    Exit For
                End If
            Next

            ' persist the data into view state
            PersistUniqueIDsToViewState_FC(list)

        End Sub

#End Region

#Region "           OutputFinanceCodeControls "

        Private Sub OutputFinanceCodeControls(ByVal uniqueID As String, ByVal finCode As Target.Abacus.Library.DataClasses.ExpenditureAccount)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim removeButton As Button
            Dim cboExpenditure As DropDownListEx
            Dim cboIncomeDue As DropDownListEx
            Dim hidField As HiddenField
            Dim selectedID As Integer

            ' don't output items marked as deleted
            If Not uniqueID.StartsWith(UNIQUEID_PREFIX_DELETE) Then

                row = New HtmlTableRow()
                row.ID = ROW_PREFIX_FC & uniqueID
                phFinanceCodes.Controls.Add(row)


                'cell = New HtmlTableCell()
                'row.Controls.Add(cell)
                'cell.Style.Add("vertical-align", "top")


                'Store the ID in a Hidden Field
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                hidField = New HiddenField
                With hidField
                    .ID = CTRL_PREFIX_EXPACC_ID & uniqueID
                    If Not finCode Is Nothing Then .Value = finCode.ID
                End With
                cell.Controls.Add(hidField)
                ' Expenditure Code
                cboExpenditure = New DropDownListEx()
                With cboExpenditure
                    .ID = CTRL_PREFIX_EXPENDITURE & uniqueID
                    .ValidationGroup = "Save"
                    .Required = True
                    .RequiredValidatorErrMsg = "Please Enter an Expenditure Code"
                    If .DropDownList.SelectedValue = "" Then
                        selectedID = 0
                    Else
                        selectedID = .DropDownList.SelectedValue
                    End If

                    LoadExpenditureCodesDropdown(cboExpenditure, selectedID)
                    If Not finCode Is Nothing AndAlso finCode.FinanceCodeID_Expenditure > 0 Then .DropDownList.SelectedValue = finCode.FinanceCodeID_Expenditure
                    '.DropDownList.Attributes.Add("onchange", "cboExpenditure_Change();")
                End With
                cell.Controls.Add(cboExpenditure)

                ' Income Code
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                cboIncomeDue = New DropDownListEx()
                With cboIncomeDue
                    .ID = CTRL_PREFIX_INCOME_DUE & uniqueID
                    .ValidationGroup = "Save"
                    .Required = True
                    .RequiredValidatorErrMsg = "Please Enter an Income Code"
                    If .DropDownList.SelectedValue = "" Then
                        selectedID = 0
                    Else
                        selectedID = .DropDownList.SelectedValue
                    End If
                    LoadIncomeDueCodesDropdown(cboIncomeDue, selectedID)
                    If Not finCode Is Nothing AndAlso finCode.FinanceCodeID_IncomeDue > 0 Then .DropDownList.SelectedValue = finCode.FinanceCodeID_IncomeDue
                    '.DropDownList.Attributes.Add("onchange", "cboIncomeDue_Change();")
                End With
                cell.Controls.Add(cboIncomeDue)

                ' remove button
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                cell.Style.Add("vertical-align", "top")
                removeButton = New Button()
                With removeButton
                    .ID = CTRL_PREFIX_REMOVED_FC & uniqueID
                    .Text = "Remove"
                    AddHandler .Click, AddressOf Remove_FC_Click
                End With
                cell.Controls.Add(removeButton)

            End If

        End Sub

#End Region

#Region "           GetUniqueID_FC "

        Private Function GetUniqueID_FC(ByVal newExpAcc As Target.Abacus.Library.DataClasses.ExpenditureAccount) As String

            Dim id As String

            If newExpAcc.ID = 0 Then
                id = UNIQUEID_PREFIX_NEW & _newFinanceCodeIDCounter
                _newFinanceCodeIDCounter += 1
            Else
                id = UNIQUEID_PREFIX_UPDATE & newExpAcc.ID
            End If

            Return id

        End Function


#End Region

#Region "           PersistUniqueIDsToViewState_FC "

        Private Sub PersistUniqueIDsToViewState_FC(ByVal list As List(Of String))

            ViewState.Add(VIEWSTATE_KEY_DATA_FC, list)
            ViewState.Add(VIEWSTATE_KEY_COUNTER_FC, _newFinanceCodeIDCounter)

        End Sub

#End Region

#Region "           GetUniqueIDsFromViewState_FC "

        Private Function GetUniqueIDsFromViewState_FC() As List(Of String)

            Dim list As List(Of String)

            ' get the details from view state
            If ViewState.Item(VIEWSTATE_KEY_DATA_FC) Is Nothing Then
                list = New List(Of String)
            Else
                list = CType(ViewState.Item(VIEWSTATE_KEY_DATA_FC), List(Of String))
            End If

            If ViewState.Item(VIEWSTATE_KEY_COUNTER_FC) Is Nothing Then
                _newFinanceCodeIDCounter = 0
            Else
                _newFinanceCodeIDCounter = CType(ViewState.Item(VIEWSTATE_KEY_COUNTER_FC), Integer)
            End If

            Return list

        End Function

#End Region

#Region "           LoadIncomeDueCodesDropdown "

        Private Sub LoadIncomeDueCodesDropdown(ByVal dropdown As DropDownListEx, ByVal mustIncludeID As Integer)

            Dim msg As ErrorMessage
            Dim _IncomeDueCodes As List(Of ViewablePair) = New List(Of ViewablePair)

            msg = FinanceCodeBL.FetchIncomeDueCodes(Me.DbConnection, mustIncludeID, _IncomeDueCodes)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            '_IncomeDueCodes.Sort(New CollectionSorter("Description", SortDirection.Ascending))

            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = _IncomeDueCodes
                .DataTextField = "Text"
                .DataValueField = "Value"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

        End Sub

#End Region

#Region "           LoadExpenditureCodesDropdown "

        Private Sub LoadExpenditureCodesDropdown(ByVal dropdown As DropDownListEx, ByVal mustIncludeID As Integer)

            Dim msg As ErrorMessage
            Dim _ExpenditureCodes As List(Of ViewablePair) = New List(Of ViewablePair)

            msg = FinanceCodeBL.FetchExpenditureCodes(Me.DbConnection, mustIncludeID, _ExpenditureCodes)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            '_ExpenditureCodes.Sort(New CollectionSorter("Description", SortDirection.Ascending))

            With dropdown.DropDownList
                .Items.Clear()
                .DataSource = _ExpenditureCodes
                .DataTextField = "Text"
                .DataValueField = "Value"
                .DataBind()
                .Items.Insert(0, String.Empty)
            End With

        End Sub

#End Region

#End Region

#Region "  Manage Selectors  "

        Private Sub SetupPCTSelector(ByVal pctID As Integer)
            With CType(Me.pct, InPlaceSelectors.InPlacePctSelector)
                .PctID = pctID
                '.Required = True
                .ValidationGroup = "Save"
            End With
        End Sub
        Private Sub SetupClientThirdPartySelector(ByVal clientID As Integer, ByVal thirdpartyID As Integer)
            With CType(Me.client, InPlaceSelectors.InPlaceClientThirdPartySelector)
                .Client_Selector.ClientDetailID = clientID
                .ThirdPartyID = thirdpartyID
                '.Required = True
                .ValidationGroup = "Save"
            End With
        End Sub
        Private Sub SetupOLASelector(ByVal olaID As Integer)
            With CType(Me.ola, InPlaceSelectors.InPlaceOtherFundingOrganizationSelector)
                .OrganisationID = olaID
                '.Required = True
                .ValidationGroup = "Save"
            End With
        End Sub
        Private Sub SetupOtherOrgSelector(ByVal orgID As Integer)
            With CType(Me.org, InPlaceSelectors.InPlaceOtherFundingOrganizationSelector)
                .OrganisationID = orgID
                '.Required = True
                .ValidationGroup = "Save"
            End With
        End Sub

        Private Sub SetupValidators()

            CType(Me.pct, InPlaceSelectors.InPlacePctSelector).RequiredValidator.EnableClientScript = False
            CType(Me.client, InPlaceSelectors.InPlaceClientThirdPartySelector).RequiredValidator.EnableClientScript = False
            CType(Me.client, InPlaceSelectors.InPlaceClientThirdPartySelector).Client_Selector.RequiredValidator.EnableClientScript = False
            CType(Me.ola, InPlaceSelectors.InPlaceOtherFundingOrganizationSelector).RequiredValidator.EnableClientScript = False
            CType(Me.org, InPlaceSelectors.InPlaceOtherFundingOrganizationSelector).RequiredValidator.EnableClientScript = False

            CType(Me.pct, InPlaceSelectors.InPlacePctSelector).Required = False
            CType(Me.client, InPlaceSelectors.InPlaceClientThirdPartySelector).Required = False
            CType(Me.ola, InPlaceSelectors.InPlaceOtherFundingOrganizationSelector).Required = False
            CType(Me.org, InPlaceSelectors.InPlaceOtherFundingOrganizationSelector).Required = False

            If optCouncil.Checked = True Then

            ElseIf optPCT.Checked = True Then

                CType(Me.pct, InPlaceSelectors.InPlacePctSelector).Required = True

            ElseIf optClient.Checked = True Then

                CType(Me.client, InPlaceSelectors.InPlaceClientThirdPartySelector).Required = True

            ElseIf optOLA.Checked = True Then
                CType(Me.ola, InPlaceSelectors.InPlaceOtherFundingOrganizationSelector).Required = True

            ElseIf optOther.Checked = True Then

                CType(Me.org, InPlaceSelectors.InPlaceOtherFundingOrganizationSelector).Required = True

            End If

        End Sub
#End Region

    End Class

End Namespace