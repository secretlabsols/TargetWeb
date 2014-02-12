
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library
Imports Target.Abacus.Library.SDS

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Admin page used to maintain domiciliary contract groups.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     06/10/2010  SDS Issue - 162 Generic Contractor Groups require usage indicator
    '''     Paul W      29/06/2010  D11795 - SDS, Generic Contracts and Service Orders
    '''     MikeVO      12/05/2009  D11549 - added reporting support.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Public Class ContractGroups
        Inherits BasePage
        Private _stdBut As StdButtonsBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ContractGroups"), "Contract Groups")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ContractGroups.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ContractGroups.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ContractGroups.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.DomContractGroup
                .AuditLogTableNames.Add("GenericContractGroup")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.ContractGroups")
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim group As GenericContractGroup
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            group = New GenericContractGroup(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With group
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                chkDomContracts.CheckBox.Checked = ((.Usage And ContractEndReasonUsage.DomContracts) > 0)
                chkDPContracts.CheckBox.Checked = ((.Usage And ContractEndReasonUsage.DPContracts) > 0)
                chkRedundant.CheckBox.Checked = .Redundant
            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
                chkDomContracts.CheckBox.Checked = False
                chkDPContracts.CheckBox.Checked = False
                chkRedundant.CheckBox.Checked = False
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = GenericContractGroup.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If Not msg.Success Then
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If



        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim group As GenericContractGroup
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If Me.IsValid Then
                group = New GenericContractGroup(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With group
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                End If
                With group
                    .Description = txtDescription.Text
                    Dim usageVal As Integer = 0
                    If chkDomContracts.CheckBox.Checked Or (Not chkDomContracts.CheckBox.Enabled) Then usageVal += ContractEndReasonUsage.DomContracts
                    If chkDPContracts.CheckBox.Checked Or (Not chkDPContracts.CheckBox.Enabled) Then usageVal += ContractEndReasonUsage.DPContracts
                    .Usage = usageVal
                    .Redundant = chkRedundant.CheckBox.Checked
                    msg = .Save()
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    e.ItemID = .ID
                End With
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub ContractGroups_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim msg As ErrorMessage
            Dim group As GenericContractGroup
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim currentUsage As Byte

            If _stdBut.SelectedItemID > 0 Then
                group = New GenericContractGroup(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                With group
                    msg = GenericContractGroupBL.Fetch(_stdBut.SelectedItemID, group)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    txtDescription.Text = .Description
                    chkDomContracts.CheckBox.Checked = ((.Usage And ContractEndReasonUsage.DomContracts) > 0)
                    chkDPContracts.CheckBox.Checked = ((.Usage And ContractEndReasonUsage.DPContracts) > 0)
                    chkRedundant.CheckBox.Checked = .Redundant

                    msg = GenericContractGroupBL.CheckIfGenericContractGroupInUse(Me.DbConnection, _stdBut.SelectedItemID, currentUsage)
                    If Not msg.Success AndAlso msg.Number = "E0503" Then
                        ' in use, so disable and tick the usage type(s) returned..
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
                    End If
                End With
            Else
                txtDescription.Text = String.Empty
                chkDPContracts.CheckBox.Checked = False
                chkDPContracts.CheckBox.Checked = False
                chkRedundant.CheckBox.Checked = False
            End If
        End Sub
    End Class

End Namespace