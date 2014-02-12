Imports System.Text
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.SDS
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Admin

	''' <summary>
	''' Admin page used to maintain contract end reasons.
	''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD      06/01/2011  SDS214 - Altered interface to automatically check and disable the 'DP Contract Payments' checkbox if the 'DP Contracts' is checked.
    '''     JohnF       06/10/2010  Ensure deletion of new end reason works OK (SDS #271)
    '''     MoTahir     06/10/2010  Issue 242 Sharepoint
    '''     JohnF       10/09/2010  D11801 - added usage 'dropdown'
    '''     MikeVO      12/05/2009  D11549 - added reporting support.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
	Partial Public Class ContractEndReasons
        Inherits BasePage

        Private _IsEditable As Boolean = False
        Private _stdBut As StdButtonsBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ContractEndReasons"), "Contract End Reasons")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ContractEndReasons.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ContractEndReasons.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ContractEndReasons.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.ContractEndReason
                .AuditLogTableNames.Add("ContractEndReason")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ContractEndReasons")
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf EditClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked
            AddHandler _stdBut.NewClicked, AddressOf NewClicked

            JsLinks.Add("ContractEndReasons.js")
            lblUsage.Style.Add("float", "left")

        End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            _IsEditable = True
            FindClicked(e)

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)

            _IsEditable = True

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim reason As ContractEndReason
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            reason = New ContractEndReason(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With reason
                msg = ContractEndReasonBL.Fetch(e.ItemID, reason)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                chkDomContracts.CheckBox.Checked = ((.Usage And ContractEndReasonUsage.DomContracts) > 0)
                chkDPContracts.CheckBox.Checked = ((.Usage And ContractEndReasonUsage.DPContracts) > 0)
                chkDPContractDetails.CheckBox.Checked = ((.Usage And ContractEndReasonUsage.DPContractDetails) > 0)
                chkRedundant.CheckBox.Checked = .Redundant
                If .SystemType > 0 Then
                    ' if system type is larger than 0 then is a system type
                    ' , systems types cannot be changed so deny access to editing buttons
                    With _stdBut
                        .AllowDelete = False
                        .AllowEdit = False
                        .ShowSave = False
                    End With
                End If
            End With
        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
                chkDomContracts.CheckBox.Checked = False
                chkDPContracts.CheckBox.Checked = False
                chkDPContractDetails.CheckBox.Checked = False
                chkRedundant.CheckBox.Checked = False
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = ContractEndReasonBL.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If msg.Success Then
                e.ItemID = 0
                _stdBut.SelectedItemID = 0
            Else
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If

        End Sub

		Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

			Dim msg As ErrorMessage
			Dim reason As ContractEndReason
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

			If Me.IsValid Then
                reason = New ContractEndReason(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
				If e.ItemID > 0 Then
                    '++ Update of existing record..
					With reason
                        msg = ContractEndReasonBL.Fetch(e.ItemID, reason)
						If Not msg.Success Then WebUtils.DisplayError(msg)
					End With
				End If
				With reason
                    .Description = txtDescription.Text
                    Dim usageVal As Integer = 0
                    If chkDomContracts.CheckBox.Checked Or (Not chkDomContracts.CheckBox.Enabled) Then usageVal += ContractEndReasonUsage.DomContracts
                    If chkDPContracts.CheckBox.Checked Or (Not chkDPContracts.CheckBox.Enabled) Then
                        usageVal += ContractEndReasonUsage.DPContracts + ContractEndReasonUsage.DPContractDetails
                    ElseIf chkDPContractDetails.CheckBox.Checked Or (Not chkDPContractDetails.CheckBox.Enabled) Then
                        usageVal += ContractEndReasonUsage.DPContractDetails
                    End If
                    .Usage = usageVal
                    .Redundant = chkRedundant.CheckBox.Checked
                    .SystemType = 0
                    msg = ContractEndReasonBL.Save(Me.DbConnection, reason)
                    If msg.Success Then
                        lblError.Text = ""
                    Else
                        lblError.Text = msg.Message
                        e.Cancel = True
                    End If
                    e.ItemID = .ID
				End With
			Else
				e.Cancel = True
			End If

		End Sub

        Private Sub ContractEndReasons_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim msg As ErrorMessage
            Dim reason As ContractEndReason
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim currentUsage As Byte
            Dim js As New StringBuilder()

            If _stdBut.SelectedItemID > 0 Then
                reason = New ContractEndReason(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                With reason
                    msg = ContractEndReasonBL.Fetch(_stdBut.SelectedItemID, reason)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    txtDescription.Text = .Description
                    chkDomContracts.CheckBox.Checked = ((.Usage And ContractEndReasonUsage.DomContracts) > 0)
                    chkDPContracts.CheckBox.Checked = ((.Usage And ContractEndReasonUsage.DPContracts) > 0)
                    chkDPContractDetails.CheckBox.Checked = ((.Usage And ContractEndReasonUsage.DPContractDetails) > 0)
                    chkRedundant.CheckBox.Checked = .Redundant

                    msg = ContractEndReasonBL.FindContractEndReasonUsage(Me.DbConnection, _stdBut.SelectedItemID, currentUsage)
                    If Not msg.Success AndAlso msg.Number = "E0503" Then
                        '++ End reason in use, so disable and tick the usage type(s) returned..
                        If ((currentUsage And ContractEndReasonUsage.DomContracts) > 0) Then
                            chkDomContracts.CheckBox.Checked = True
                            chkDomContracts.CheckBox.Enabled = False
                            chkDomContracts.Label.Enabled = False
                        End If
                        If ((currentUsage And ContractEndReasonUsage.DPContracts) > 0) Then
                            chkDPContracts.CheckBox.Checked = True
                            chkDPContracts.CheckBox.Enabled = False
                            chkDPContracts.Label.Enabled = False
                        End If
                        If ((currentUsage And ContractEndReasonUsage.DPContractDetails) > 0) Then
                            chkDPContractDetails.CheckBox.Checked = True
                            chkDPContractDetails.CheckBox.Enabled = False
                            chkDPContractDetails.Label.Enabled = False
                        End If
                    End If
                End With
            Else
                txtDescription.Text = String.Empty
                chkDPContracts.CheckBox.Checked = False
                chkDPContractDetails.CheckBox.Checked = False
                chkRedundant.CheckBox.Checked = False
            End If

            chkDPContracts.CheckBox.Attributes.Add("onClick", "chkDPContracts_OnClick(this);")

            js.AppendFormat("dpContractCheckBoxID = '{0}';", chkDPContracts.CheckBox.ClientID)
            js.AppendFormat("dpContractPaymentCheckBoxID = '{0}';", chkDPContractDetails.CheckBox.ClientID)
            js.AppendFormat("isEditable = {0};", _IsEditable.ToString().ToLower())

            ClientScript.RegisterStartupScript(Me.GetType(), "Startup", js.ToString(), True)

        End Sub
    End Class

End Namespace