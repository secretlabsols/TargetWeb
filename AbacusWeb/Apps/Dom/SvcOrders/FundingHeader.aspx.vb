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
Imports Target.Library.ApplicationBlocks.DataAccess
Imports System.Data.SqlClient

Namespace Apps.Dom.SvcOrder

    ''' <summary>
    ''' Screen used to display Funding header Info on the DSO funding screen.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO   07/04/2009  D11537 - need to suppress Csrf check due to use of iframe.
    ''' Paul  17/02/2009  D11491 - Initial Version.
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Public Class FundingHeader
        Inherits BasePage

        Const VIEWSTATE_KEY_DATA As String = "DataList"
        Const VIEWSTATE_KEY_COUNTER As String = "NewCounter"

        Const ROW_PREFIX As String = "row"

        Const CTRL_PREFIX_NUMERATOR As String = "numerator"
        Const CTRL_PREFIX_DENOMINATOR As String = "denominator"
        Const CTRL_PREFIX_EXPENDITURE As String = "expenditure"
        Const CTRL_PREFIX_CALL_OFF_ORDER As String = "callofforder"
        Const CTRL_PREFIX_REMOVED As String = "remove"

        Const UNIQUEID_PREFIX_NEW As String = "N"
        Const UNIQUEID_PREFIX_UPDATE As String = "U"
        Const UNIQUEID_PREFIX_DELETE As String = "D"

        Private _svcOrderID As Integer
        Private _stdBut As StdButtonsBase
        Private _fundID As Integer
        Private _refreshTree As Boolean
        Private _newPropIDCounter As Integer
        Private _actualsExistForOrder As Boolean
        Private _btnAuditDetails As HtmlInputButton = New HtmlInputButton("button")

#Region " Stuff "

#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                AddHandler .AddCustomControls, AddressOf StdButtons_AddCustomControls
            End With

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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceOrderFunding"), "Service Order Funding")
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim msg As ErrorMessage = New ErrorMessage
            Dim detailList As DomServiceOrderFundingDetailCollection = New DomServiceOrderFundingDetailCollection
            Dim propList As Collection = New Collection

            _svcOrderID = Utils.ToInt32(Request.QueryString("id"))
            _fundID = Utils.ToInt32(Request.QueryString("fundingID"))

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            'Check if actuals exist for this order
            msg = DomContractBL.DomServiceOrder_ActualsExist(Me.DbConnection, _svcOrderID, _actualsExistForOrder)
            If Not msg.Success Then WebUtils.DisplayError(msg)


            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderFunding.AddNew"))
                .ShowNew = False
                .AllowFind = False
                .AllowEdit = (Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderFunding.Edit")) And _
                              Not _actualsExistForOrder)
                .AllowDelete = (Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderFunding.Delete")) And _
                                Not _actualsExistForOrder)
                .EditableControls.Add(fsControls.Controls)
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            Me.JsLinks.Add("FundingHeader.js")

            'Get a list of all the detail rows
            If _fundID > 0 Then
                msg = DomServiceOrderFundingDetail.FetchList(Me.DbConnection, detailList, _fundID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                For Each fd As DomServiceOrderFundingDetail In detailList
                    Dim prop As FundingProportion = New FundingProportion
                    prop.FundingDetailID = fd.ID
                    prop.CallOffOrder = fd.Callofforder
                    prop.Denominator = fd.Denominator
                    prop.Numerator = fd.Numerator

                    Dim expAccount As ExpenditureAccount = New ExpenditureAccount(Me.DbConnection)
                    msg = expAccount.Fetch(fd.ExpenditureAccountID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    Dim financeCode As FinanceCode = New FinanceCode(Me.DbConnection)
                    msg = financeCode.Fetch(expAccount.FinanceCodeID_Expenditure)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                    prop.ExpenditureCode = financeCode.Code

                    propList.Add(prop)
                Next

                ' refresh the list of existing bands and save in view state
                phProportions.Controls.Clear()
                For Each funProp As FundingProportion In propList
                    OutputProportionControls(funProp, propList.Count, False)
                Next
            End If

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage = New ErrorMessage

            PopulateDropdowns()

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim funding As DomServiceOrderFunding = New DomServiceOrderFunding(Me.DbConnection)
            Dim detailList As DomServiceOrderFundingDetailCollection = New DomServiceOrderFundingDetailCollection
            Dim propList As Collection = New Collection
            Dim msg As ErrorMessage = New ErrorMessage

            PopulateDropdowns()
            cboServiceType.SelectPostBackValue()

            msg = funding.Fetch(_fundID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With funding
                cboServiceType.DropDownList.SelectedValue = .DomServiceTypeID
                If .Type = 1 Then
                    optApportion.Checked = True
                ElseIf .Type = 2 Then
                    optOffPlan.Checked = True
                End If
                chkDefault.CheckBox.Checked = .UseAsDefaultForOtherServiceTypes

                CType(auditDetails, IBasicAuditDetails).EnteredBy = .CreatedBy
                CType(auditDetails, IBasicAuditDetails).DateEntered = .DateCreated.ToString("dd MMM yyyy")
                If Utils.IsDate(.DateAmended) Then
                    CType(auditDetails, IBasicAuditDetails).DateLastAmended = .DateAmended.ToString("dd MMM yyyy")
                    CType(auditDetails, IBasicAuditDetails).LastAmendedBy = .AmendedBy
                End If
            End With

            'Get a list of all the detail rows
            msg = DomServiceOrderFundingDetail.FetchList(Me.DbConnection, detailList, _fundID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            For Each fd As DomServiceOrderFundingDetail In detailList
                Dim prop As FundingProportion = New FundingProportion
                prop.FundingDetailID = fd.ID
                prop.CallOffOrder = fd.Callofforder
                prop.Denominator = fd.Denominator
                prop.Numerator = fd.Numerator

                Dim expAccount As ExpenditureAccount = New ExpenditureAccount(Me.DbConnection)
                msg = expAccount.Fetch(fd.ExpenditureAccountID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                Dim financeCode As FinanceCode = New FinanceCode(Me.DbConnection)
                msg = financeCode.Fetch(expAccount.FinanceCodeID_Expenditure)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                prop.ExpenditureCode = financeCode.Code

                propList.Add(prop)
            Next

            ' refresh the list of existing bands and save in view state
            phProportions.Controls.Clear()
            For Each funProp As FundingProportion In propList
                OutputProportionControls(funProp, propList.Count, False)
            Next

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim funding As DomServiceOrderFunding = New DomServiceOrderFunding(Me.DbConnection)
            Dim fundingID As Integer = Utils.ToInt32(Request.QueryString("fundingID"))
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim detailList As DomServiceOrderFundingDetailCollection = New DomServiceOrderFundingDetailCollection
            Dim msg As ErrorMessage = New ErrorMessage
            Dim trans As SqlTransaction = Nothing
            Dim transRolledBack As Boolean
            Dim propList As Collection = New Collection

            PopulateDropdowns()
            cboServiceType.SelectPostBackValue()

            If fundingID > 0 Then
                'Existing record
                msg = funding.Fetch(fundingID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            Else
                'New record
            End If
            With funding
                .DomServiceTypeID = cboServiceType.DropDownList.SelectedValue
                If optApportion.Checked = True Then
                    .Type = 1
                ElseIf optOffPlan.Checked = True Then
                    .Type = 2
                End If
                .UseAsDefaultForOtherServiceTypes = chkDefault.CheckBox.Checked
                'TODO D12199
                '.DomServiceOrderID = _svcOrderID
            End With

            'Get a list of all the detail rows
            If _fundID > 0 Then
                msg = DomServiceOrderFundingDetail.FetchList(Me.DbConnection, detailList, _fundID)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                'Build up Proportions collection from Proportions table
                propList = New Collection
                For Each fd As DomServiceOrderFundingDetail In detailList
                    ' create the empty fundingProportion
                    Dim prop As FundingProportion = New FundingProportion
                    ' set the Funding Proportion Properties
                    With prop
                        .FundingDetailID = fd.ID
                        .ExpenditureCode = CType(phProportions.FindControl(CTRL_PREFIX_EXPENDITURE & fd.ID), TextBoxEx).Text
                        'validate user input
                        If CType(phProportions.FindControl(CTRL_PREFIX_DENOMINATOR & fd.ID), TextBoxEx).Text > Integer.MaxValue Then
                            lblError.Text = String.Format("Invalid denominator for income code {0}", .ExpenditureCode)
                            e.Cancel = True
                            Exit Sub
                        End If
                        If CType(phProportions.FindControl(CTRL_PREFIX_NUMERATOR & fd.ID), TextBoxEx).Text > Integer.MaxValue Then
                            lblError.Text = String.Format("Invalid numerator for income code {0}", .ExpenditureCode)
                            e.Cancel = True
                            Exit Sub
                        End If
                        .Denominator = Utils.ToInt32(CType(phProportions.FindControl(CTRL_PREFIX_DENOMINATOR & fd.ID), TextBoxEx).Text)
                        .Numerator = Utils.ToInt32(CType(phProportions.FindControl(CTRL_PREFIX_NUMERATOR & fd.ID), TextBoxEx).Text)

                        If optOffPlan.Checked = True Then
                            .CallOffOrder = Utils.ToInt32(CType(phProportions.FindControl(CTRL_PREFIX_CALL_OFF_ORDER & fd.ID), DropDownListEx).DropDownList.SelectedValue)
                        Else
                            .CallOffOrder = 0
                        End If
                        .ID = propList.Count + 1
                    End With
                    ' add to the collection
                    propList.Add(prop)
                Next
            End If

            Try
                trans = SqlHelper.GetTransaction(Me.DbConnection)
                msg = FinanceCodeBL.SaveDomServiceOrderFunding(trans, funding, currentUser.ExternalUsername, fundingID)
                If Not msg.Success Then
                    If msg.Number = FinanceCodeBL.ERR_COULD_NOT_SAVE_DSO_FUNDING Or msg.Number = FinanceCodeBL.ERR_COULD_NOT_SAVE_DSO_FUNDING_DETAIL Then
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
                    If propList.Count > 0 Then
                        msg = FinanceCodeBL.UpdateServiceOrderFundingDetailProportions(trans, propList)
                        If Not msg.Success Then
                            If msg.Number = FinanceCodeBL.ERR_COULD_NOT_SAVE_DSO_FUNDING Or msg.Number = FinanceCodeBL.ERR_COULD_NOT_SAVE_DSO_FUNDING_DETAIL Then
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
                    'Store ID for Use Later
                    _fundID = fundingID
                    e.ItemID = fundingID
                    _refreshTree = True
                    Response.Redirect(String.Format("FundingHeader.aspx?id={0}&fundingID={1}&mode={2}", _svcOrderID, _fundID, Convert.ToInt32(StdButtonsMode.Fetched)))
                Else
                    e.Cancel = True
                End If

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0001")     ' unexpected
                e.Cancel = True
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try
        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                phProportions.Controls.Clear()
                PopulateDropdowns()
                'cboServiceType.DropDownList.SelectedValue = ""
                CType(auditDetails, IBasicAuditDetails).EnteredBy = String.Empty
                CType(auditDetails, IBasicAuditDetails).DateEntered = Nothing
                CType(auditDetails, IBasicAuditDetails).DateLastAmended = Nothing
                CType(auditDetails, IBasicAuditDetails).LastAmendedBy = String.Empty
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
                msg = FinanceCodeBL.DeleteServiceOrderFunding(trans, _fundID)
                If Not msg.Success Then
                    If msg.Number = FinanceCodeBL.ERR_COULD_NOT_DELETE_DSO_FUNDING Then
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

            Dim msg As ErrorMessage
            Dim serviceTypes As List(Of ViewablePair) = New List(Of ViewablePair)
            Dim domServiceOrder As DomServiceOrder = New DomServiceOrder(Me.DbConnection, String.Empty, String.Empty)
            msg = domServiceOrder.Fetch(_svcOrderID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            Dim contractID As Integer = domServiceOrder.DomContractID

            With cboServiceType
                ' get a list of non-redundant groups
                msg = DomContractBL.FetchServiceTypesAvailableToContract(Me.DbConnection, contractID, serviceTypes)
                'msg = DomContractGroup.FetchList(Me.DbConnection, groups, String.Empty, String.Empty, TriState.False)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    .DataSource = serviceTypes
                    .DataTextField = "Text"
                    .DataValueField = "Value"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty))
                End With
            End With

        End Sub

        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            'If _refreshTree Then
            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", String.Format("window.parent.RefreshTree({0}, 'h', {1});", _svcOrderID, _fundID), True)
            'End If

        End Sub

#End Region

#Region " Proportions Table Code "

#Region "           OutputProportionControls "

        Private Sub OutputProportionControls(ByVal prop As FundingProportion, ByVal noDetailRows As Integer, ByVal selectPostBackValue As Boolean)

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim txtExpenditure As TextBoxEx
            Dim txtNumerator As TextBoxEx
            Dim txtDenominator As TextBoxEx
            Dim cboCallOffOrder As DropDownListEx
            'Dim selectedID As Integer

            row = New HtmlTableRow()
            row.ID = ROW_PREFIX & prop.FundingDetailID
            phProportions.Controls.Add(row)

            'Expenditure Code
            cell = New HtmlTableCell()
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            txtExpenditure = New TextBoxEx()
            With txtExpenditure
                .ID = CTRL_PREFIX_EXPENDITURE & prop.FundingDetailID

                .IsReadOnly = True
            End With
            cell.Controls.Add(txtExpenditure)
            If selectPostBackValue Then
                txtExpenditure.Text = txtExpenditure.GetPostBackValue
            Else
                txtExpenditure.Text = prop.ExpenditureCode
            End If

            'Numerator
            cell = New HtmlTableCell()
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            txtNumerator = New TextBoxEx()
            With txtNumerator
                .ID = CTRL_PREFIX_NUMERATOR & prop.FundingDetailID
                .TextBox.Style.Add("width", "4em")
                .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
            End With
            cell.Controls.Add(txtNumerator)
            If selectPostBackValue Then
                txtNumerator.Text = txtNumerator.GetPostBackValue
            Else
                txtNumerator.Text = prop.Numerator
            End If

            'Denominator
            cell = New HtmlTableCell()
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            txtDenominator = New TextBoxEx()
            With txtDenominator
                .ID = CTRL_PREFIX_DENOMINATOR & prop.FundingDetailID
                .TextBox.Style.Add("width", "4em")
                .Format = TextBoxEx.TextBoxExFormat.IntegerFormat
            End With
            cell.Controls.Add(txtDenominator)
            If selectPostBackValue Then
                txtDenominator.Text = txtDenominator.GetPostBackValue
            Else
                txtDenominator.Text = prop.Denominator
            End If


            ' Call Off Order
            cell = New HtmlTableCell()
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cboCallOffOrder = New DropDownListEx()
            With cboCallOffOrder
                .ID = CTRL_PREFIX_CALL_OFF_ORDER & prop.FundingDetailID
                LoadCallOffOrderDropdown(cboCallOffOrder, noDetailRows, prop.CallOffOrder)
                If selectPostBackValue Then
                    'LoadCallOffOrderDropdown(cboCallOffOrder, noDetailRows, .SelectPostBackValue())
                    .SelectPostBackValue()
                Else

                    If Not prop Is Nothing AndAlso prop.CallOffOrder > 0 Then .DropDownList.SelectedValue = prop.CallOffOrder
                End If

                '.DropDownList.Attributes.Add("onchange", "cboExpenditure_Change();")
            End With
            cell.Controls.Add(cboCallOffOrder)

        End Sub

#End Region

#Region "           LoadCallOffOrderDropdown "

        Private Sub LoadCallOffOrderDropdown(ByVal dropdown As DropDownListEx, ByVal numDetailRows As Integer, ByVal actualCallOffOrderValue As Integer)

            With dropdown.DropDownList
                .Items.Insert(0, "")
                For index As Integer = 1 To numDetailRows
                    .Items.Insert(index, index.ToString)
                Next
                If actualCallOffOrderValue > numDetailRows Then
                    'for some reason Call Off orders value is out of sync, most likely because a detail row has been deleted.
                    .Items.Insert(actualCallOffOrderValue - 1, actualCallOffOrderValue.ToString)
                End If
            End With

        End Sub

#End Region

#End Region

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            'the Proportions of details rows can not be amended once actuals exist for the Service Order.
            'If _actualsExistForOrder Or _stdBut.ButtonsMode <> StdButtonsMode.Edit Then
            '    WebUtils.RecursiveDisable(phProportions.Controls, True)
            'End If

            If _actualsExistForOrder And _stdBut.ButtonsMode <> StdButtonsMode.AddNew Then
                lblWarning.Text = "Order Funding details may no longer be amended as actual service is present for one or more week covered by the Service Order."
            End If

        End Sub
    End Class

End Namespace