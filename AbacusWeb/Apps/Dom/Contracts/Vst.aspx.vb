
Imports System.Collections.Generic
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Dom.Contracts

	''' <summary>
    ''' Screen used to maintain a domiciliary contract visit service types.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' CD   29/04/2010  A4WA#6252 - prevented refresh of tree if delete failed (DeleteClicked), so user can see error message
    ''' MvO  15/12/2009  A4WA#5967 - fix when deleting immediately after creating and control re-initialisation.
    ''' MvO  28/05/2009  D11549 - tweak the way expect min/max start/end times are used to plugs gaps and remove ambiguity.
    ''' MvO  23/04/2009  A4WA#5395 - various fixes following test plan execution.
    ''' MvO  07/04/2009  D11537 - need to suppress Csrf check due to use of iframe.
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class Vst
        Inherits BasePage

        Private _contract As DomContract
        Private _contractID As Integer
        Private _vstID As Integer
        Private _serviceTypeInUse As Boolean
        Private _refreshTree As Boolean
        Private _stdBut As StdButtonsBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Domiciliary Contract Visit Service Type")

            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _vstID = Utils.ToInt32(Request.QueryString("id"))

            Dim msg As ErrorMessage
            Dim canEdit As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            ' fetch the contract
            _contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = _contract.Fetch(_contractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With _stdBut
                .AllowNew = canEdit
                .ShowNew = False
                .AllowFind = False
                .AllowEdit = canEdit
                .AllowDelete = canEdit
                .EditableControls.Add(fsControls.Controls)
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            tmeMinStartTime.OnChangeJavascript = "BuildExpectedVisitTimes();"
            tmeMaxEndTime.OnChangeJavascript = "BuildExpectedVisitTimes();"

            Me.JsLinks.Add("Vst.js")

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            PopulateDropdowns()
            chkReEvalAtMidnight.CheckBox.Checked = True
            chkReEvalAtTimeBandBoundary.CheckBox.Checked = True
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim vst As DomContractVisitServiceType

            vst = New DomContractVisitServiceType(Me.DbConnection, String.Empty, String.Empty)
            With vst
                _vstID = e.ItemID
                msg = .Fetch(_vstID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                PopulateDropdowns()

                CheckServiceTypeInUseByDso(.DomServiceTypeID, .DomContractID)

                If .DomServiceTypeID > 0 Then cboServiceType.DropDownList.SelectedValue = .DomServiceTypeID
                chkReEvalAtMidnight.CheckBox.Checked = .ReEvaluateRatesAtMidnight
                chkReEvalAtTimeBandBoundary.CheckBox.Checked = .ReEvaluateRatesAtTimeBandBoundary
                txtMinDuration.Text = .MinimumDuration
                tmeMinStartTime.Hours = .ExpectedMinStartTime.Hour()
                tmeMinStartTime.Minutes = .ExpectedMinStartTime.Minute()
                tmeMaxEndTime.Hours = .ExpectedMaxEndTime.Hour()
                tmeMaxEndTime.Minutes = .ExpectedMaxEndTime.Minute()

            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                PopulateDropdowns()
                cboServiceType.DropDownList.SelectedValue = String.Empty
                txtMinDuration.Text = String.Empty
                tmeMinStartTime.Hours = "00"
                tmeMinStartTime.Minutes = "00"
                tmeMaxEndTime.Hours = "00"
                tmeMaxEndTime.Minutes = "00"
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = DomContractBL.DeleteContractVisitServiceType(Me.DbConnection, e.ItemID, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            If Not msg.Success Then
                If msg.Number = DomContractBL.ERR_COULD_NOT_DELETE_VST Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                Else
                    WebUtils.DisplayError(msg)
                End If
            Else
                _vstID = 0
                _refreshTree = True
                NewClicked(Nothing)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim vst As DomContractVisitServiceType
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            vst = New DomContractVisitServiceType(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With vst
                If e.ItemID > 0 Then
                    ' update
                    msg = .Fetch(e.ItemID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    CheckServiceTypeInUseByDso(.DomServiceTypeID, .DomContractID)
                Else
                    .DomContractID = _contractID
                    .DomServiceTypeID = Utils.ToInt32(cboServiceType.GetPostBackValue())
                End If
            End With

            PopulateDropdowns()
            cboServiceType.DropDownList.SelectedValue = vst.DomServiceTypeID

            Me.Validate("Save")

            If Me.IsValid Then

                With vst
                    .AuditLogOverriddenParentID = .DomContractID
                    If Not _serviceTypeInUse Then
                        .ReEvaluateRatesAtMidnight = chkReEvalAtMidnight.CheckBox.Checked
                        .ReEvaluateRatesAtTimeBandBoundary = chkReEvalAtTimeBandBoundary.CheckBox.Checked
                        .MinimumDuration = Utils.ToInt32(txtMinDuration.Text)
                    End If
                    chkReEvalAtMidnight.CheckBox.Checked = .ReEvaluateRatesAtMidnight
                    chkReEvalAtTimeBandBoundary.CheckBox.Checked = .ReEvaluateRatesAtTimeBandBoundary
                    txtMinDuration.Text = .MinimumDuration

                    .ExpectedMinStartTime = tmeMinStartTime.ToString(DomContractBL.TIME_ONLY_DATE)
                    .ExpectedMaxEndTime = tmeMaxEndTime.ToString(DomContractBL.TIME_ONLY_DATE)

                    msg = DomContractBL.SaveContractVisitServiceType(Me.DbConnection, vst, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    If Not msg.Success Then
                        If msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_VST Then
                            lblError.Text = msg.Message
                            e.Cancel = True
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    Else
                        e.ItemID = .ID
                        _vstID = .ID
                        _refreshTree = True
                    End If
                End With
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub PopulateDropdowns()

            Dim msg As ErrorMessage
            Dim list As List(Of ViewablePair) = Nothing

            With cboServiceType
                ' get a list of visit-based service types available to the contract
                msg = DomContractBL.FetchServiceTypesAvailableToContract(Me.DbConnection, _contractID, TriState.True, list)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .DataSource = list
                    .DataTextField = "Text"
                    .DataValueField = "Value"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

        End Sub

        Private Sub CheckServiceTypeInUseByDso(ByVal serviceTypeID As Integer, ByVal domContractID As Integer)
            Dim msg As ErrorMessage
            msg = DomContractBL.ServiceTypeInUseByDsoProformaDpi(Me.DbConnection, serviceTypeID, domContractID, _serviceTypeInUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            Dim startupJS As StringBuilder = New StringBuilder()

            With startupJS
                .AppendFormat("expMinStartTime_HoursID='{0}_cboHours';", tmeMinStartTime.ClientID)
                .AppendFormat("expMinStartTime_MinsID='{0}_cboMinutes';", tmeMinStartTime.ClientID)
                .AppendFormat("expMaxEndTime_HoursID='{0}_cboHours';", tmeMaxEndTime.ClientID)
                .AppendFormat("expMaxEndTime_MinsID='{0}_cboMinutes';", tmeMaxEndTime.ClientID)
                If _refreshTree Then
                    .AppendFormat("window.parent.RefreshTree({0}, 'vst', {1});", _contractID, _vstID)
                End If
            End With

            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", startupJS.ToString(), True)

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            _stdBut.Visible = (_contract.EndDate = DataUtils.MAX_DATE)
            If _vstID > 0 Then
                cboServiceType.DropDownList.Enabled = False
            End If
            If _serviceTypeInUse Then
                chkReEvalAtMidnight.CheckBox.Enabled = False
                chkReEvalAtTimeBandBoundary.CheckBox.Enabled = False
                txtMinDuration.TextBox.Enabled = False
            End If
        End Sub

    End Class

End Namespace