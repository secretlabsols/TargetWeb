Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.ServiceOrder
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Admin page used to maintain Other Funding Organization.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO   12/05/2009  D11549 - added reporting support.
    ''' Paul  04/02/2009  D11491 - Original Version.
    ''' </history>
    Partial Public Class OtherFundingOrganizationMaintenance
        Inherits Target.Web.Apps.BasePage

        Private _orgID As Integer
        Private _stdBut As StdButtonsBase
        Private _btnAuditDetails As HtmlInputButton = New HtmlInputButton("button")

#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                AddHandler .AddCustomControls, AddressOf StdButtons_AddCustomControls
            End With

        End Sub

#End Region

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.OtherFundingOrganization"), "Other Funding Organisation")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.OtherFundingOrganization.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.OtherFundingOrganization.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.OtherFundingOrganization.Delete"))
                With .SearchBy
                    .Add("Name", "Name")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.OtherFundingOrganization
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.OtherFundingOrganizations")
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            With txtAddress
                .TextBox.TextMode = TextBoxMode.MultiLine
                .TextBox.Rows = 3
                .SetupRegEx()
            End With
            
        End Sub

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

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim Organisation As OtherFundingOrganization
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Organisation = New OtherFundingOrganization(Me.DbConnection)
            With Organisation
                _orgID = e.ItemID
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                If .Type = 4 Then 'OLA
                    optOLA.Checked = True
                ElseIf .Type = 5 Then 'Other organization
                    optOther.Checked = True
                End If
                txtName.Text = .Name
                txtDebtorRef.Text = .DebtorRef
                txtAddress.Text = .Address
                txtPostCode.Text = .Postcode
                txtPhone.Text = .Phone
                txtFax.Text = .Fax
                txtEmail.Text = .Email
                txtContactTitleAndInitials.Text = .ContactTitleAndInitials
                txtContactSurname.Text = .ContactSurname

                CType(auditDetails, IBasicAuditDetails).EnteredBy = .CreatedBy
                CType(auditDetails, IBasicAuditDetails).DateEntered = .DateCreated.ToString("dd MMM yyyy")
                If Utils.IsDate(.DateAmended) Then
                    CType(auditDetails, IBasicAuditDetails).DateLastAmended = .DateAmended.ToString("dd MMM yyyy")
                    CType(auditDetails, IBasicAuditDetails).LastAmendedBy = .AmendedBy
                End If

                chkRedundant.CheckBox.Checked = .Redundant

            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                optOLA.Checked = True
                txtName.Text = String.Empty
                txtDebtorRef.Text = String.Empty
                txtAddress.Text = String.Empty
                txtPostCode.Text = String.Empty
                txtPhone.Text = String.Empty
                txtFax.Text = String.Empty
                txtEmail.Text = String.Empty
                txtContactTitleAndInitials.Text = String.Empty
                txtContactSurname.Text = String.Empty
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
            Dim msg As ErrorMessage
            Dim inUse As Boolean

            'Check if in use
            msg = serviceorderbl.IsOtherFundingOrganizationInUse(Me.DbConnection, e.ItemID, inUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If inUse Then
                lblError.Text = "Cannot delete this record, it is already in use on an Expenditure Account."
                e.Cancel = True
                FindClicked(e)
            Else
                msg = OtherFundingOrganization.Delete(Me.DbConnection, e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim Organisation As OtherFundingOrganization
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If Me.IsValid Then
                Organisation = New OtherFundingOrganization(Me.DbConnection)
                If e.ItemID > 0 Then
                    ' update
                    With Organisation
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                End If

                With Organisation
                    If optOLA.Checked Then
                        .Type = 4
                    ElseIf optOther.Checked Then
                        .Type = 5
                    End If
                    .Name = txtName.Text
                    .DebtorRef = txtDebtorRef.Text
                    .Address = txtAddress.Text
                    .Postcode = txtPostCode.Text
                    .Phone = txtPhone.Text
                    .Fax = txtFax.Text
                    .Email = txtEmail.Text
                    .ContactTitleAndInitials = txtContactTitleAndInitials.Text
                    .ContactSurname = txtContactSurname.Text
                    .Redundant = chkRedundant.CheckBox.Checked
                    If e.ItemID > 0 Then
                        .AmendedBy = currentUser.ExternalUsername
                        .DateAmended = Date.Now
                    Else
                        .CreatedBy = currentUser.ExternalUsername
                        .DateCreated = Date.Now
                    End If

                    ' save
                    msg = .Save()
                    If Not msg.Success Then
                        WebUtils.DisplayError(msg)
                    End If
                    e.ItemID = .ID
                End With
            Else
                e.Cancel = True
            End If

            FindClicked(e)
        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim msg As ErrorMessage
            Dim inUse As Boolean

            'Check if in use
            msg = ServiceOrderBL.IsOtherFundingOrganizationInUse(Me.DbConnection, _orgID, inUse)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If inUse Then
                optOLA.Disabled = True
                optOther.Disabled = True
            End If

            If _stdBut.ButtonsMode <> StdButtonsMode.Edit And _stdBut.ButtonsMode <> StdButtonsMode.Fetched Then
                CType(auditDetails, IBasicAuditDetails).Collapsed = True
                CType(_btnAuditDetails, HtmlInputButton).Disabled = True
            Else
                CType(auditDetails, IBasicAuditDetails).Collapsed = True
                CType(_btnAuditDetails, HtmlInputButton).Disabled = False
            End If

        End Sub
    End Class

End Namespace