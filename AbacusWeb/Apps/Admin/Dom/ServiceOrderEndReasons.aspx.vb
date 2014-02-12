
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Admin.Dom

    ''' <summary>
	''' Admin page used to maintain domiciliary service order end reasons.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO  12/05/2009  D11549 - added reporting support.
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' </history>
	Partial Public Class ServiceOrderEndReasons
		Inherits BasePage

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceOrderEndReasons"), "Service Order End Reasons")

			Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderEndReasons.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderEndReasons.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceOrderEndReasons.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.DomServiceOrderEndReason
                .AuditLogTableNames.Add("DomServiceOrderEndReason")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DomServiceOrderEndReasons")
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
			Dim reason As DomServiceOrderEndReason
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            reason = New DomServiceOrderEndReason(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With reason
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                chkRedundant.CheckBox.Checked = .Redundant
            End With

        End Sub
        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim reason As DomServiceOrderEndReason
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            reason = New DomServiceOrderEndReason(Me.DbConnection, currentUser.ExternalUsername, _
                                                  AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            msg = reason.Fetch(e.ItemID)
            If msg.Success Then
                If reason.Type = 1 Then
                    lblError.Text = "You are unable to edit to 'System' End Reason."
                    e.Cancel = True
                Else
                    FindClicked(e)
                End If
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

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim reason As DomServiceOrderEndReason = Nothing

            reason = New DomServiceOrderEndReason(Me.DbConnection, currentUser.ExternalUsername, _
                                                  AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            msg = reason.Fetch(e.ItemID)
            If msg.Success Then
                If reason.Type = 1 Then
                    lblError.Text = "You are unable to delete a 'System' End Reason."
                    e.Cancel = True
                Else
                    msg = DomServiceOrderEndReason.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim reason As DomServiceOrderEndReason
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If Me.IsValid Then
                reason = New DomServiceOrderEndReason(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With reason
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                End If
                With reason
                    .Description = txtDescription.Text
                    .Redundant = chkRedundant.CheckBox.Checked
                    ' save
                    msg = .Save()
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    e.ItemID = .ID
                End With
            Else
                e.Cancel = True
            End If

        End Sub

	End Class

End Namespace