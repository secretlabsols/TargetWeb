Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Admin.ServiceUsers

    ''' <summary>
    ''' Admin page used to maintain domiciliary service order end reasons.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' PaulW     09/07/2010   D11796 SDS - Spend Plans
    ''' </history>
    Partial Public Class SpendPlanEndReasons
        Inherits BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.SpendPlanEndResons"), "Spend Plan End Reasons")

            Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.SpendPlanEndReason.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.SpendPlanEndReason.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.SpendPlanEndReason.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.SpendPlanEndReason
                .AuditLogTableNames.Add("SpendPlanEndReason")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.SpendPlanEndReasons")
            End With
            AddHandler stdBut.FindClicked, AddressOf FindClicked
            AddHandler stdBut.EditClicked, AddressOf EditClicked
            AddHandler stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler stdBut.NewClicked, AddressOf NewClicked
            AddHandler stdBut.DeleteClicked, AddressOf DeleteClicked
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim reason As New SpendPlanEndReason(String.Empty, String.Empty)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = SpendPlanBL.FetchSpendPlanEndReason(Me.DbConnection, e.ItemID, _
                                                currentUser.ExternalUsername, _
                                                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), reason)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            With reason
                txtDescription.Text = .Description
                chkRedundant.CheckBox.Checked = .Redundant
            End With

        End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim reason As New SpendPlanEndReason(String.Empty, String.Empty)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = SpendPlanBL.FetchSpendPlanEndReason(Me.DbConnection, e.ItemID, _
                                                currentUser.ExternalUsername, _
                                                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), reason)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If reason.System = True Then
                lblError.Text = "You are unable to edit this 'System' End Reason."
                e.Cancel = True
                FindClicked(e)
            Else
                FindClicked(e)
            End If

        End Sub

        Private Sub NewClicked(ByRef e As StdButtonEventArgs)
            txtDescription.Text = String.Empty
            chkRedundant.CheckBox.Checked = False
        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
                chkRedundant.CheckBox.Checked = False
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim reason As New SpendPlanEndReason(String.Empty, String.Empty)
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If e.ItemID > 0 Then
                msg = SpendPlanBL.FetchSpendPlanEndReason(Me.DbConnection, e.ItemID, currentUser.ExternalUsername, _
                                                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), reason)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If
            With reason
                .Description = txtDescription.Text
                .Redundant = chkRedundant.CheckBox.Checked
            End With

            msg = SpendPlanBL.SaveSpendPlanEndReason(Me.DbConnection, _
                                                currentUser.ExternalUsername, _
                                                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), reason)
            If Not msg.Success Then
                If msg.Number = SpendPlanBL.ERR_COULD_NOT_SAVE_SPENDREASON Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                Else
                    WebUtils.DisplayError(msg)
                End If
            Else
                e.ItemID = reason.ID
                FindClicked(e)
            End If

        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = SpendPlanBL.DeleteSpendPlanEndReason(Me.DbConnection, _
                                                currentUser.ExternalUsername, _
                                                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If Not msg.Success Then
                If msg.Number = SpendPlanBL.ERR_COULD_NOT_DELETE_SPENDREASON Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    FindClicked(e)
                Else
                    WebUtils.DisplayError(msg)
                End If

            End If


        End Sub

    End Class

End Namespace