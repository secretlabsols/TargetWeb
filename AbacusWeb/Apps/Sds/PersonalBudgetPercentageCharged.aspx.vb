
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
Imports System.Data.SqlClient
Imports Target.Library.ApplicationBlocks.DataAccess

Namespace Apps.Sds

    ''' <summary>
    ''' Admin page used to maintain personal budget percentage charged figures.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD       04/02/2011    D12008 - Changes to save routine to ensure that sucw records are flagged when saving and that the user is notified when they are.
    '''     JohnF        08/09/2010    Convert to use of dedicated BL (D11794)
    '''     JohnF        09/07/2010    Created (D11794)
    ''' </history>
    Partial Public Class PersonalBudgetPercentageCharged
        Inherits BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.PersonalBudgetPercentageCharged"), "Personal Budget % Charged")

            Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PersonalBudgetPercentageCharged.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PersonalBudgetPercentageCharged.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PersonalBudgetPercentageCharged.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.SDS_PersonalBudgetPercentageCharged
                .AuditLogTableNames.Add("PersonalBudgetPercentageCharged")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.PersonalBudgetPercentageCharged")
            End With
            AddHandler stdBut.FindClicked, AddressOf FindClicked
            AddHandler stdBut.EditClicked, AddressOf FindClicked
            AddHandler stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler stdBut.DeleteClicked, AddressOf DeleteClicked
        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)
            Dim msg As ErrorMessage
            Dim charged As Target.Abacus.Library.DataClasses.PersonalBudgetPercentageCharged
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            charged = New Target.Abacus.Library.DataClasses.PersonalBudgetPercentageCharged(Me.DbConnection, _
                                                                                            currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With charged
                msg = PersonalBudgetPercentageChargedBL.Fetch(e.ItemID, charged)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                dteDateFrom.Text = .DateFrom.ToString("dd/MM/yyyy")
                If Utils.IsDate(.DateTo) AndAlso (.DateTo <> DataUtils.MAX_DATE) Then
                    dteDateTo.Text = .DateTo.ToString("dd/MM/yyyy")
                Else
                    dteDateTo.Text = String.Empty
                End If
                valPercentCharged.Text = .PercentageCharged.ToString("0")
            End With
        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                dteDateFrom.Text = String.Empty
                dteDateTo.Text = String.Empty
                valPercentCharged.Text = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

        ''' <summary>
        ''' Deletes the record
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim numberOfReconsiderationRecordsAffected As Integer = 0

            msg = PersonalBudgetPercentageChargedBL.Delete(Me.DbConnection, _
                                                           currentUser.ExternalUsername, _
                                                           AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                                           e.ItemID, _
                                                           numberOfReconsiderationRecordsAffected)
            If Not msg.Success Then
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If

        End Sub

        ''' <summary>
        ''' Saves the record
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            Try

                If Me.IsValid Then

                    Dim changedCharged As Target.Abacus.Library.DataClasses.PersonalBudgetPercentageCharged
                    Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
                    Dim numberOfReconsiderationRecordsAffected As Integer = 0

                    Using transaction As SqlTransaction = SqlHelper.GetTransaction(DbConnection)

                        changedCharged = New Target.Abacus.Library.DataClasses.PersonalBudgetPercentageCharged(transaction, _
                                                                                                                currentUser.ExternalUsername, _
                                                                                                                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                        If e.ItemID > 0 Then
                            ' if we have an id then fetch the item from the db

                            With changedCharged
                                msg = PersonalBudgetPercentageChargedBL.Fetch(e.ItemID, changedCharged)
                                If Not msg.Success Then
                                    lblError.Text = msg.Message
                                    e.Cancel = True
                                    Exit Sub
                                End If
                            End With

                        End If

                        ' set the changed objects properties to that of controls on screen
                        With changedCharged
                            .DateFrom = Convert.ToDateTime(dteDateFrom.Text)
                            If dteDateTo.Text <> String.Empty Then
                                .DateTo = Convert.ToDateTime(dteDateTo.Text)
                            Else
                                .DateTo = DataUtils.MAX_DATE
                            End If
                            Dim tempInt As Integer = valPercentCharged.Text
                            .PercentageCharged = tempInt
                            msg = PersonalBudgetPercentageChargedBL.Save(transaction, changedCharged, numberOfReconsiderationRecordsAffected)
                            If Not msg.Success Then
                                SqlHelper.RollbackTransaction(transaction)
                                lblError.Text = msg.Message
                                e.Cancel = True
                                Exit Sub
                            End If
                            e.ItemID = .ID
                        End With

                        transaction.Commit()

                        If numberOfReconsiderationRecordsAffected > 0 Then
                            ' if we have at least one record affected then display message

                            With lblError
                                .Text = "Changes made to the Personal Budget %age Charged may result in adjustments to historic charges for one or more Service User.<br /><br />"
                                .CssClass = "warningText"
                            End With

                        End If

                        msg = New ErrorMessage()
                        msg.Success = True

                    End Using

                Else

                    e.Cancel = True

                End If

            Catch ex As Exception
                ' catch and wrap error

                msg = Utils.CatchError(ex, ErrorMessage.GeneralErrorNumber)
                WebUtils.DisplayError(msg)

            Finally

                If Not msg.Success Then
                    ' if we have an error then setup error label

                    With lblError
                        .Text = msg.Message
                        .CssClass = "errorText"
                    End With

                End If

            End Try

        End Sub

    End Class

End Namespace