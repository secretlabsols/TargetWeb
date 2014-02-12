
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Dom.Contracts

    ''' <summary>
    ''' Screen used to maintain a domiciliary contract period unit costs.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO  28/08/2009  D11676 - Initial version
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class ServiceDays
        Inherits BasePage

        Const CTRL_PREFIX_DAY As String = "chkDay"

        Private _contractID As Integer
        Private _periodID As Integer
        Private _refreshTree As Boolean
        Private _stdBut As StdButtonsBase
        Private _contractEnded As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Domiciliary Contract Period Service Days")

            Dim msg As ErrorMessage
            Dim contract As DomContract

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _periodID = Utils.ToInt32(Request.QueryString("periodID"))

            With _stdBut
                .AllowNew = False
                .AllowDelete = False
                .AllowFind = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))
                .EditableControls.Add(fsControls.Controls)
            End With
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.CancelClicked, AddressOf FindClicked

            ' get the contract
            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = contract.Fetch(_contractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            _contractEnded = (contract.EndDate <> DataUtils.MAX_DATE)

            CreateControls()

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim periodDays As DomContractPeriodServiceDayCollection = Nothing

            msg = DomContractPeriodServiceDay.FetchList(Me.DbConnection, periodDays, String.Empty, String.Empty, _periodID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            For Each periodDay As DomContractPeriodServiceDay In periodDays
                CType(phUnitCosts.FindControl(CTRL_PREFIX_DAY & periodDay.DayOfWeek), CheckBoxEx).CheckBox.Checked = True
            Next

        End Sub


        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim trans As SqlTransaction = Nothing
            Dim periodDays As DomContractPeriodServiceDayCollection = Nothing

            If Me.IsValid Then

                Try
                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    msg = DomContractPeriodServiceDay.FetchList(trans, periodDays, String.Empty, String.Empty, _periodID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    For Each day As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
                        If CType(phUnitCosts.FindControl(CTRL_PREFIX_DAY & Convert.ToInt32(day)), CheckBoxEx).CheckBox.Checked Then
                            Dim recordExists As Boolean = False
                            'see if record already in database for this day.
                            For Each periodDay As DomContractPeriodServiceDay In periodDays
                                If periodDay.DayOfWeek = Convert.ToInt32(day) Then
                                    recordExists = True
                                    Exit For
                                End If
                            Next
                            If Not recordExists Then
                                'This day was not found, so lets add it
                                Dim periodDay As DomContractPeriodServiceDay = New DomContractPeriodServiceDay(trans, _
                                                                                        currentUser.ExternalUsername, _
                                                                                        AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

                                With periodDay
                                    .DayOfWeek = Convert.ToInt32(day)
                                    .Frequency = Frequency.EveryWeek
                                    .DomContractPeriodID = _periodID
                                    msg = .Save()
                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                End With
                            End If
                        Else
                             
                            'see if record already in database for this day.
                            For Each periodDay As DomContractPeriodServiceDay In periodDays
                                If periodDay.DayOfWeek = Convert.ToInt32(day) Then
                                    'As this day of week is not selected and the Item exists in the database we need to delete the record.
                                    msg = DomContractPeriodServiceDay.Delete(trans, _
                                                     currentUser.ExternalUsername, _
                                                     AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), periodDay.ID)
                                    If Not msg.Success Then WebUtils.DisplayError(msg)
                                    Exit For
                                End If
                            Next

                        End If
                    Next

                    trans.Commit()

                    _refreshTree = True

                    msg = New ErrorMessage()
                    msg.Success = True

                Catch ex As Exception
                    WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))    ' unexpected
                Finally
                    If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                End Try
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub CreateControls()

            Dim tr As TableRow, td As TableCell, chkDays As CheckBoxEx

            For Each day As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))

                tr = New TableRow()
                phUnitCosts.Controls.Add(tr)

                td = New TableCell()
                td.Style.Add("vertical-align", "top")
                chkDays = New CheckBoxEx()
                With chkDays
                    .ID = CTRL_PREFIX_DAY & Convert.ToInt32(day)
                    .Text = [Enum].GetName(GetType(DayOfWeek), day)
                    .LabelWidth = "7em"
                End With
                td.Controls.Add(chkDays)
                tr.Cells.Add(td)

            Next


        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If _refreshTree Then
                ClientScript.RegisterStartupScript(Me.GetType(), "Startup", String.Format("window.parent.RefreshTree({0}, 'sd', {1});", _contractID, _periodID), True)
            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            _stdBut.Visible = Not _contractEnded
        End Sub

    End Class

End Namespace