
Imports System.Collections.Generic
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.Admin
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Dom.Contracts

	''' <summary>
    ''' Screen used to maintain a Service User Minutes Calculation Method on a dom contract period.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' John Finch    07/08/2011  Initial version (D12051)
    ''' </history>
    Partial Class Sucm
        Inherits Target.Web.Apps.BasePage

        Private _contract As DomContract
        Private _contractID As Integer
        Private _period As DomContractPeriod
        Private _periodID As Integer
        Private _calcMethodID As Integer
        Private _serviceTypeInUse As Boolean
        Private _stdBut As StdButtonsBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "S/U Minutes Calc Method")

            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _periodID = Utils.ToInt32(Request.QueryString("periodID"))

            Dim msg As ErrorMessage
            Dim canEdit As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            '++ Fetch the parent contract..
            _contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = _contract.Fetch(_contractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            '++ ..and the current contract period.
            _period = New DomContractPeriod(Me.DbConnection, String.Empty, String.Empty)
            msg = _period.Fetch(_periodID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With _stdBut
                .AllowNew = False
                .ShowNew = False
                .AllowFind = False
                .AllowEdit = canEdit
                .AllowDelete = False
                .EditableControls.Add(fsControls.Controls)
                .AuditLogTableNames.Add("ServiceUserMinutesCalculationMethod")
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked

            Dim ctrlCalcMethod As UserControls.ucServiceUserMinutesCalc = CType(ucCalcMethod, UserControls.ucServiceUserMinutesCalc)
            Dim calcMethod As New Target.Library.ServiceUserMinutesCalculationMethod( _
                        conn:=Me.DbConnection, auditUserName:=String.Empty, auditLogTitle:=String.Empty)
            ctrlCalcMethod.InitControl(Me, Nothing, calcMethod, Nothing)
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim msg As New ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim calcMethod As New Target.Library.ServiceUserMinutesCalculationMethod(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            Dim ctrlCalcMethod As UserControls.ucServiceUserMinutesCalc = CType(ucCalcMethod, UserControls.ucServiceUserMinutesCalc)
            Dim calcMethodID As Integer = 0

            If optUseLocalSettings.Checked Then
                With calcMethod
                    If e.ItemID > 0 Then
                        '++ Updating changes to an existing calc method record..
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    Else
                        '++ Need to create a new calc method record..
                        .Unhook()
                        .ID = 0
                    End If

                    .MinutesFrom1 = Utils.ToNumeric(ctrlCalcMethod.MinutesFrom1)
                    .MinutesFrom2 = Utils.ToNumeric(ctrlCalcMethod.MinutesFrom2)
                    .MinutesFrom3 = Utils.ToNumeric(ctrlCalcMethod.MinutesFrom3)
                    .MinutesTo1 = Utils.ToNumeric(ctrlCalcMethod.MinutesTo1)
                    .MinutesTo2 = Utils.ToNumeric(ctrlCalcMethod.MinutesTo2)
                    .MinutesTo3 = Utils.ToNumeric(ctrlCalcMethod.MinutesTo3)
                    .CalculationMethod1 = Utils.ToNumeric(ctrlCalcMethod.CalcMethod1)
                    .CalculationMethod2 = Utils.ToNumeric(ctrlCalcMethod.CalcMethod2)
                    .CalculationMethod3 = Utils.ToNumeric(ctrlCalcMethod.CalcMethod3)

                    msg = .Save()
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    If e.ItemID = 0 Then
                        '++ Link the new calc method record to the current contract period..
                        _period.ServiceUserMinutesCalculationMethodID = .ID
                        msg = _period.Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End If

                    e.ItemID = .ID
                End With
            ElseIf optUseSystemSettings.Checked Then
                '++ Need to clear any calc method linked to the contract period..
                calcMethodID = Utils.ToInt32(_period.ServiceUserMinutesCalculationMethodID)

                _period.ServiceUserMinutesCalculationMethodID = Nothing
                msg = _period.Save()

                If calcMethodID > 0 Then
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    msg = Target.Library.ServiceUserMinutesCalculationMethod.Delete( _
                            conn:=Me.DbConnection, auditUserName:=currentUser.ExternalUsername, _
                            auditLogTitle:=AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                            id:=calcMethodID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If

                e.ItemID = 0
            End If

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Dim startupJS As StringBuilder = New StringBuilder()

            Me.JsLinks.Add("Sucm.js")

            Const SP_FETCH_CALCULATION_METHODS As String = "spxServiceUserMinutesCalculationMethod_FetchAll"
            Dim msg As ErrorMessage
            Dim calcMethod As New Target.Library.ServiceUserMinutesCalculationMethod( _
                        conn:=Me.DbConnection, auditUserName:=String.Empty, auditLogTitle:=String.Empty)
            Dim ctrlCalcMethod As UserControls.ucServiceUserMinutesCalc = CType(ucCalcMethod, UserControls.ucServiceUserMinutesCalc)

            '++ Get the (local) calc method stored on the contract period, if any..
            _stdBut.SelectedItemID = Utils.ToInt32(_period.ServiceUserMinutesCalculationMethodID)

            With calcMethod
                _calcMethodID = _stdBut.SelectedItemID

                If _calcMethodID > 0 Then
                    optUseLocalSettings.Checked = True

                    msg = .Fetch(_calcMethodID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    ctrlCalcMethod.MinutesFrom1 = .MinutesFrom1
                    ctrlCalcMethod.MinutesFrom2 = .MinutesFrom2
                    ctrlCalcMethod.MinutesFrom3 = .MinutesFrom3
                    ctrlCalcMethod.MinutesTo1 = .MinutesTo1
                    ctrlCalcMethod.MinutesTo2 = .MinutesTo2
                    ctrlCalcMethod.MinutesTo3 = .MinutesTo3
                    ctrlCalcMethod.CalcMethod1 = .CalculationMethod1
                    ctrlCalcMethod.CalcMethod2 = .CalculationMethod2
                    ctrlCalcMethod.CalcMethod3 = .CalculationMethod3
                    'ctrlCalcMethod.InitControl(Me, Nothing, calcMethod, Nothing)
                Else
                    optUseSystemSettings.Checked = True

                    '++ Default the Calc Method control to the values derived from the combination of
                    '++ ApplicationSettingPeriod/ServiceUserMinutesCalculationMethod in effect on
                    '++ the period end date..
                    Dim spSPParams() As SqlParameter = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_FETCH_CALCULATION_METHODS, False)
                    '++ Fetch all values available for this application setting.
                    '++ NB - the results are sorted in reverse date order automatically..
                    spSPParams(0).Value = Convert.DBNull
                    Dim spDataset As DataSet = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_FETCH_CALCULATION_METHODS, spSPParams)
                    Dim spTable As DataTable = spDataset.Tables(0)

                    If spTable.Rows.Count > 0 Then
                        '++ There are Calc Method records available..
                        Dim settingsToDisplay As Boolean = False
                        Dim allowEdit As Boolean = False

                        For Each spRow As DataRow In spTable.Rows
                            If _period.DateTo >= spRow("DateFrom") Then
                                Dim tempID As Integer = spRow("CalculationMethodID")
                                msg = .Fetch(tempID)
                                If Not msg.Success Then WebUtils.DisplayError(msg)

                                Exit For
                            End If
                        Next
                    Else
                        ctrlCalcMethod.Reset()
                    End If

                    ctrlCalcMethod.MinutesFrom1 = .MinutesFrom1
                    ctrlCalcMethod.MinutesFrom2 = .MinutesFrom2
                    ctrlCalcMethod.MinutesFrom3 = .MinutesFrom3
                    ctrlCalcMethod.MinutesTo1 = .MinutesTo1
                    ctrlCalcMethod.MinutesTo2 = .MinutesTo2
                    ctrlCalcMethod.MinutesTo3 = .MinutesTo3
                    ctrlCalcMethod.CalcMethod1 = .CalculationMethod1
                    ctrlCalcMethod.CalcMethod2 = .CalculationMethod2
                    ctrlCalcMethod.CalcMethod3 = .CalculationMethod3
                End If
            End With

            optUseLocalSettings.Enabled = (_stdBut.ButtonsMode = StdButtonsMode.Edit)
            optUseSystemSettings.Enabled = optUseLocalSettings.Enabled
        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            _stdBut.Visible = (_contract.EndDate = DataUtils.MAX_DATE)
        End Sub
    End Class

End Namespace