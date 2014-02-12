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
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.Dom.SvcOrder

    ''' <summary>
    ''' Screen used to display Funding Details on the DSO funding screen.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO   07/04/2009  D11537 - need to suppress Csrf check due to use of iframe.
    ''' Paul  17/02/2009  D11491 - Initial Version.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class FundingDetail
        Inherits BasePage

        Private _svcOrderID As Integer
        Private _fundDetailID As Integer
        Private _fundID As Integer
        Private _refreshTree As Boolean
        Private _stdBut As StdButtonsBase
        Private _actualsExistForOrder As Boolean
        Private _zeroPercent As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceOrderFunding"), "Service Order Funding")
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage = New ErrorMessage
            Dim funding As DomServiceOrderFunding = New DomServiceOrderFunding(Me.DbConnection)

            _svcOrderID = Utils.ToInt32(Request.QueryString("id"))
            _fundDetailID = Utils.ToInt32(Request.QueryString("detailID"))
            _fundID = Utils.ToInt32(Request.QueryString("fundingID"))
            msg = funding.Fetch(_fundID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))

            cboExpAccountType.DropDownList.Attributes.Add("onchange", "cboExpaccountType_Click();")

            'Check if actuals exist for this order
            msg = DomContractBL.DomServiceOrder_ActualsExist(Me.DbConnection, _svcOrderID, _actualsExistForOrder)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderFunding.AddNew"))
                .ShowNew = False
                .AllowFind = False
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderFunding.Edit"))
                
                .EditableControls.Add(fsControls.Controls)
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            Me.JsLinks.Add("FundingDetail.js")
            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.Dom.SvcOrders.FundingDetail.Startup", _
                    Target.Library.Web.Utils.WrapClientScript(String.Format("cboExpAccountTypeID='{0}';expenditureAccountSelectorID='{1}';fundingDetail_serviceType={2};chkServiceUserFundedID='{3}';fundDetail_ExpCode='{4}', fundDetail_IncCode='{5}'", _
                                            cboExpAccountType.ClientID, expenditureAccount.ClientID, funding.DomServiceTypeID, chkUnitsFunded.ClientID, txtExpenditure.ClientID, txtIncomeDue.ClientID)))
        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage = New ErrorMessage

            PopulateDropdowns()

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim fundingDetail As DomServiceOrderFundingDetail = New DomServiceOrderFundingDetail(Me.DbConnection)
            Dim msg As ErrorMessage = New ErrorMessage

            PopulateDropdowns()
            cboExpAccountType.SelectPostBackValue()

            msg = fundingDetail.Fetch(_fundDetailID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With fundingDetail
                cboExpAccountType.DropDownList.SelectedValue = Convert.ToInt32(.ExpenditureAccountType)

                'ClientScript.RegisterStartupScript(Me.GetType(), "FindClicked", String.Format("serviceType={0};", .ExpenditureAccountType), True)
                'CType(Me.expenditureAccount, InPlaceSelectors.InPlaceExpenditureAccountSelector).ExpenditureAccountID = .ExpenditureAccountID

                Dim expAcc As ExpenditureAccount = New ExpenditureAccount(Me.DbConnection)
                msg = expAcc.Fetch(.ExpenditureAccountID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                Dim finCode As FinanceCode = New FinanceCode(Me.DbConnection)
                msg = finCode.Fetch(expAcc.FinanceCodeID_Expenditure)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtExpenditure.Text = finCode.Code

                msg = finCode.Fetch(expAcc.FinanceCodeID_IncomeDue)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtIncomeDue.Text = finCode.Code

                txtProportion.Text = String.Format("{0} / {1}", .Numerator, .Denominator)
                _zeroPercent = (.Numerator = 0)
                chkBalancingAmount.CheckBox.Checked = .RecieveBalancingAmount
                chkUnitsFunded.CheckBox.Checked = .ServiceUserFunded

            End With

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim fundDetail As DomServiceOrderFundingDetail = New DomServiceOrderFundingDetail(Me.DbConnection)
            Dim msg As ErrorMessage = New ErrorMessage
            Dim trans As SqlTransaction = Nothing
            Dim transRolledBack As Boolean
            Dim fDetailID As Integer

            PopulateDropdowns()
            cboExpAccountType.SelectPostBackValue()

            With fundDetail
                If _fundDetailID > 0 Then
                    msg = .Fetch(_fundDetailID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                Else
                    'new record
                    .Denominator = 1
                    .Numerator = 0
                End If

                .DomServiceOrderFundingID = _fundID
                .ExpenditureAccountID = Utils.ToInt32(CType(Me.expenditureAccount, InPlaceSelectors.InPlaceExpenditureAccountSelector).HiddenFieldUniqueID)

                If Not _actualsExistForOrder Then
                    .ExpenditureAccountType = Utils.ToInt32(cboExpAccountType.DropDownList.SelectedValue)
                    .RecieveBalancingAmount = chkBalancingAmount.CheckBox.Checked
                    .ServiceUserFunded = chkUnitsFunded.CheckBox.Checked
                End If

                Try
                    trans = SqlHelper.GetTransaction(Me.DbConnection)
                    msg = FinanceCodeBL.SaveDomServiceOrderFundingDetail(trans, fundDetail, fDetailID)
                    If Not msg.Success Then
                        If msg.Number = FinanceCodeBL.ERR_COULD_NOT_SAVE_DSO_FUNDING_DETAIL Then
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

                    If Not transRolledBack Then
                        trans.Commit()
                        'Store the new ID for use later (this may be the same as before if we are updating)
                        _fundDetailID = fDetailID
                        _refreshTree = True
                        FindClicked(e)
                    Else
                        e.Cancel = True
                    End If

                Catch ex As Exception
                    msg = Utils.CatchError(ex, "E0001")     ' unexpected
                    e.Cancel = True
                Finally
                    If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
                End Try

            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                PopulateDropdowns()
                'CType(Me.expenditureAccount, InPlaceSelectors.InPlaceExpenditureAccountSelector).ExpenditureAccountID = 0
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage = New ErrorMessage
            Dim trans As SqlTransaction = Nothing
            Dim transRolledBack As Boolean

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)
                msg = FinanceCodeBL.DeleteServiceOrderFundingDetail(trans, _fundDetailID)
                If Not msg.Success Then
                    If msg.Number = FinanceCodeBL.ERR_COULD_NOT_DELETE_DSO_FUNDING_DETAIL Then
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

                If Not transRolledBack Then
                    trans.Commit()
                    _refreshTree = True
                Else
                    e.Cancel = True
                    FindClicked(e)
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0001")     ' unexpected
                e.Cancel = True
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try

        End Sub

        Private Sub PopulateDropdowns()

            With cboExpAccountType
                With .DropDownList
                    .Items.Clear()
                    .DataSource = BindToEnum(GetType(ExpenditureAccountGroupType))
                    .DataTextField = "Key"
                    .DataValueField = "Value"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

        End Sub

        Public Shared Function BindToEnum(ByVal enumType As Type) As DataTable

            Dim names As String() = ExpenditureAccountGroupType.GetNames(enumType)
            Dim values As Array = ExpenditureAccountGroupType.GetValues(enumType)
            Dim dt As New DataTable
            dt.Columns.Add("Key", GetType(String))
            dt.Columns.Add("Value", GetType(Integer))

            Dim i As Integer = 0
            While i < names.Length
                Dim dr As DataRow = dt.NewRow
                dr("Key") = names(i)
                dr("Value") = CType(values.GetValue(i), Integer)
                dt.Rows.Add(dr)
                i = i + 1
            End While
            Return dt

        End Function

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If _refreshTree Then
                ClientScript.RegisterStartupScript(Me.GetType(), "Startup", String.Format("window.parent.RefreshTree({0}, 'd', {1});", _svcOrderID, _fundDetailID), True)
            End If

            If _zeroPercent Then
                _stdBut.AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderFunding.Delete"))
            Else
                If (Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderFunding.Delete")) And _
                                                                 Not _actualsExistForOrder) Then
                    'allow delete
                    _stdBut.AllowDelete = True
                Else
                    'Delete not allowed
                    lblWarning.Text = "Order Funding details may no longer be deleted as actual service is present for one or more week covered by the Service Order."
                    _stdBut.AllowDelete = False
                End If

            End If


        End Sub


        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim msg As ErrorMessage = New ErrorMessage
            Dim funding As DomServiceOrderFunding = New DomServiceOrderFunding(Me.DbConnection)
            msg = funding.Fetch(_fundID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            'it will not be possible to change the value in the Service User Funded 
            'check box if actuals exist for the service Order.
            If _actualsExistForOrder And _stdBut.ButtonsMode <> StdButtonsMode.AddNew Then
                cboExpAccountType.DropDownList.Enabled = False
                chkUnitsFunded.CheckBox.Enabled = False
                'chkBalancingAmount.CheckBox.Enabled = False
            End If

            ClientScript.RegisterStartupScript(Me.GetType(), "FindClicked", _
                                    String.Format("setup_expenditureSelector({0}, {1}, {2});", _
                                            Utils.ToInt32(cboExpAccountType.DropDownList.SelectedValue), funding.DomServiceTypeID, _
                                            Convert.ToInt32(_stdBut.ButtonsMode)), True)


        End Sub
    End Class

End Namespace