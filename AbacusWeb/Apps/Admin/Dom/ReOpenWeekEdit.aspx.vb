
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

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Screen used to maintain a domiciliary contract re-opened weeks.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Public Class ReOpenWeekEdit
        Inherits BasePage

        Private _providerID As Integer
        Private _contractID As Integer
        Private _weekID As Integer

        Private _stdBut As StdButtonsBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Re-openedWeeks"), "Domiciliary Contract Re-Opened Weeks")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            _providerID = Utils.ToInt32(Request.QueryString("estabID"))
            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _weekID = Utils.ToInt32(Request.QueryString("id"))

            If _weekID = 0 Then
                If _contractID > 0 Then
                    ' get contact and provider
                    PopulateProvider(PopulateContract(_contractID))
                ElseIf _providerID > 0 Then
                    ' get provider
                    PopulateProvider(_providerID)
                End If
            End If

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Re-openedWeeks.AddNew"))
                .ShowNew = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Re-openedWeeks.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Re-openedWeeks.Delete"))
                .AllowBack = True
                .AllowFind = False
                .AuditLogTableNames.Add("DomContractReOpenedWeek")
                .EditableControls.Add(fsControls.Controls)
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

        End Sub

        Private Sub PopulateProvider(ByVal id As Integer)
            Dim msg As ErrorMessage
            Dim provider As Establishment
            provider = New Establishment(Me.DbConnection)
            With provider
                msg = .Fetch(id)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtProvider.Text = String.Format("{0}/{1}", .AltReference, .Name)
            End With
        End Sub

        Private Function PopulateContract(ByVal id As Integer) As Integer
            Dim msg As ErrorMessage
            Dim contract As DomContract
            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            With contract
                msg = .Fetch(id)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtContract.Text = String.Format("{0}/{1}", .Number, .Title)
                Return .ProviderID
            End With
        End Function

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim week As DomContractReOpenedWeek

            week = New DomContractReOpenedWeek(Me.DbConnection, String.Empty, String.Empty)
            With week
                msg = week.Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                PopulateProvider(PopulateContract(.DomContractID))
                dteWeekEnding.Text = .WeekEnding
                txtReason.Text = .ReOpenedReason
                dteClosure.Text = .ExpectedClosureDate
                pnlViewOnly.Visible = True
                txtReOpenedBy.Text = .ReOpenedBy
                dteReOpenedDate.Text = .ReOpenedDate

            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                dteWeekEnding.Text = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = DomContractReOpenedWeek.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim week As DomContractReOpenedWeek

            If Me.IsValid Then
                week = New DomContractReOpenedWeek(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                With week
                    If e.ItemID > 0 Then
                        ' update
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        ' re-read w/e as we don't update this field
                        dteWeekEnding.Text = .WeekEnding
                        pnlViewOnly.Visible = True
                        txtReOpenedBy.Text = .ReOpenedBy
                        dteReOpenedDate.Text = .ReOpenedDate
                    Else
                        .DomContractID = _contractID
                        .WeekEnding = dteWeekEnding.Text
                    End If
                    PopulateProvider(PopulateContract(.DomContractID))

                    .ReOpenedReason = txtReason.Text
                    .ExpectedClosureDate = dteClosure.Text
                    .ReOpenedBy = currentUser.ExternalUsername
                    .ReOpenedDate = DateTime.Now

                    msg = DomContractBL.ValidateReOpenedWeek(Me.DbConnection, .DomContractID, .WeekEnding)
                    If Not msg.Success Then
                        If msg.Number = DomContractBL.ERR_INVALID_WEEK_ENDING Or msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_REOPENED_WEEK Then
                            ' invalid dom week ending date or could not save re-opened week
                            lblError.Text = msg.Message
                            e.Cancel = True
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    Else
                        msg = .Save()
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                        e.ItemID = .ID
                        pnlViewOnly.Visible = True
                        txtReOpenedBy.Text = .ReOpenedBy
                        dteReOpenedDate.Text = .ReOpenedDate
                    End If

                End With
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            If _stdBut.ButtonsMode = StdButtonsMode.Edit Then
                dteWeekEnding.Enabled = False
            End If
        End Sub

    End Class

End Namespace