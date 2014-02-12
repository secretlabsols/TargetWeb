
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
    ''' CD   17/08/2011  D12102 - After discussion with Angus removed refreshing of the parent tree as it is not required...i.e. does not update the tree in any way.
    ''' MvO  15/12/2009  A4WA#5967 - revert controls when Cancel is clicked.
    ''' MvO  07/04/2009  D11537 - need to suppress Csrf check due to use of iframe.
    ''' MvO  01/12/2008  D11490 - added Measured In column.
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class UnitCost
        Inherits BasePage

        Const CTRL_PREFIX_UNITCOST As String = "txtUnitCost"

        Private _contractID As Integer
        Private _periodID As Integer
        Private _refreshTree As Boolean
        Private _stdBut As StdButtonsBase
        Private _contractEnded As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Domiciliary Contract Period Unit Cost")

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
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked

            ' get the contract
            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = contract.Fetch(_contractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            _contractEnded = (contract.EndDate <> DataUtils.MAX_DATE)

            CreateControls(GetRates(Nothing))

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            Response.Redirect(Request.Url.ToString())
        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = New ErrorMessage()
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim unitCost As DomContractDomRateCategory_DomContractPeriod
            Dim enteredRate As String
            Dim rates As DataTable = Nothing
            Dim trans As SqlTransaction = Nothing

            If Me.IsValid Then

                Try
                    trans = SqlHelper.GetTransaction(Me.DbConnection)

                    rates = GetRates(trans)

                    For Each rate As DataRow In rates.Rows
                        unitCost = New DomContractDomRateCategory_DomContractPeriod(trans, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                        With unitCost
                            msg = .Fetch(rate("ID"))
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                            enteredRate = CType(phUnitCosts.FindControl(CTRL_PREFIX_UNITCOST & rate("ID")), TextBoxEx).Text
                            .UnitCost = Convert.ToDecimal(enteredRate).ToString("F2")
                            .AuditLogOverriddenParentID = .DomContractID
                            msg = .Save()
                            If Not msg.Success Then WebUtils.DisplayError(msg)
                        End With
                    Next

                    trans.Commit()

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

        Private Function GetRates(ByVal trans As SqlTransaction) As DataTable

            Const SP_NAME As String = "spxDomContractViewRates_FetchList"

            Dim ds As DataSet = Nothing
            Dim spParams As SqlParameter() = Nothing

            Try
                If trans Is Nothing Then
                    spParams = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME, False)
                Else
                    spParams = SqlHelperParameterCache.GetSpParameterSet(trans.Connection, SP_NAME, False)
                End If
                spParams(0).Value = _contractID
                spParams(1).Value = _periodID

                If trans Is Nothing Then
                    ds = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, SP_NAME, spParams)
                Else
                    ds = SqlHelper.ExecuteDataset(trans, CommandType.StoredProcedure, SP_NAME, spParams)
                End If

            Catch ex As Exception
                WebUtils.DisplayError(Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber))    ' unexpected
            End Try

            Return ds.Tables(0)

        End Function

        Private Sub CreateControls(ByVal rates As DataTable)

            Dim tr As TableRow, td As TableCell, txtUnitCost As TextBoxEx

            For Each rate As DataRow In rates.Rows
                tr = New TableRow()
                phUnitCosts.Controls.Add(tr)

                ' description
                td = New TableCell()
                td.Width = New Unit(15, UnitType.Em)
                td.Style.Add("vertical-align", "top")
                td.Text = rate("Description")
                tr.Cells.Add(td)

                ' measured in
                td = New TableCell()
                td.Style.Add("vertical-align", "top")
                td.Text = rate("MeasuredIn")
                tr.Cells.Add(td)

                ' abbreviation
                td = New TableCell()
                td.Style.Add("vertical-align", "top")
                td.Text = rate("Abbreviation")
                tr.Cells.Add(td)

                ' unit cost
                td = New TableCell()
                td.Style.Add("vertical-align", "top")
                txtUnitCost = New TextBoxEx()
                With txtUnitCost
                    .ID = CTRL_PREFIX_UNITCOST & rate("ID")
                    .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                    .Width = New Unit(5, UnitType.Em)
                    .Required = True
                    .RequiredValidatorErrMsg = "* Required"
                    .ValidationGroup = "Save"
                    .Text = Convert.ToDecimal(rate("UnitCost")).ToString("F2")
                End With
                td.Controls.Add(txtUnitCost)
                tr.Cells.Add(td)
            Next

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If _refreshTree Then
                ClientScript.RegisterStartupScript(Me.GetType(), "Startup", String.Format("window.parent.RefreshTree({0}, 'uc', {1});", _contractID, _periodID), True)
            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            _stdBut.Visible = Not _contractEnded
        End Sub

    End Class

End Namespace