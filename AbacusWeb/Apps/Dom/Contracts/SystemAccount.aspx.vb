
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
	''' Screen used to maintain a domiciliary contract period system account.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO  07/04/2009  D11537 - need to suppress Csrf check due to use of iframe.
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class SystemAccount
        Inherits BasePage

        Private _contractID As Integer
        Private _periodID As Integer
        Private _sysAccID As Integer
        Private _refreshTree As Boolean
        Private _stdBut As StdButtonsBase
        Private _contractEnded As Boolean

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .hideCreditorRef = True
                .hideDebtorRef = True
            End With
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payments.Contracts.ProviderContracts"), "Domiciliary Contract Period System Account")

            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _periodID = Utils.ToInt32(Request.QueryString("periodID"))
            _sysAccID = Utils.ToInt32(Request.QueryString("id"))

            Dim msg As ErrorMessage
            Dim contract As DomContract
            Dim canEdit As Boolean = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DomiciliaryContracts.Edit"))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

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

            ' get the contract
            contract = New DomContract(Me.DbConnection, String.Empty, String.Empty)
            msg = contract.Fetch(_contractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            _contractEnded = (contract.EndDate <> DataUtils.MAX_DATE)

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            SetupClientSelector(0)
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim sysAcc As DomContractPeriodSystemAccount

            sysAcc = New DomContractPeriodSystemAccount(Me.DbConnection, String.Empty, String.Empty)
            With sysAcc
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                SetupClientSelector(.ClientID)
                txtFinanceCode.Text = .FinanceCode
            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                SetupClientSelector(0)
                txtFinanceCode.Text = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = DomContractPeriodSystemAccount.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID, _contractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            _sysAccID = 0
            _refreshTree = True

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim sysAcc As DomContractPeriodSystemAccount
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If Me.IsValid Then
                sysAcc = New DomContractPeriodSystemAccount(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With sysAcc
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                Else
                    sysAcc.DomContractPeriodID = _periodID
                End If
                With sysAcc
                    .ClientID = Utils.ToInt32(Request.Form(CType(client, InPlaceClientSelector).HiddenFieldUniqueID))
                    .FinanceCode = IIf(txtFinanceCode.Text.Trim().Length > 0, txtFinanceCode.Text, Nothing)
                    .AuditLogOverriddenParentID = _contractID
                    msg = .Save()
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    e.ItemID = .ID
                    _sysAccID = .ID
                    _refreshTree = True
                End With
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub SetupClientSelector(ByVal clientID As Integer)
            With CType(Me.client, InPlaceSelectors.InPlaceClientSelector)
                .ClientDetailID = clientID
                .Required = True
                .ValidationGroup = "Save"
            End With
        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If _refreshTree Then
                ClientScript.RegisterStartupScript(Me.GetType(), "Startup", String.Format("window.parent.RefreshTree({0}, 'sa', {1});", _contractID, _sysAccID), True)
            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            _stdBut.Visible = Not _contractEnded
        End Sub

    End Class

End Namespace