
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

Namespace Apps.Admin.Dom

    ''' <summary>
    ''' Screen used to maintain the domiciliary provider invoice weeks closed to date.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     IHS         03/05/2011  A4W Action Change #6385.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
    Partial Public Class CloseWeek
        Inherits BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.CloseWeek"), "Close Weeks")

            Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With stdBut
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.CloseWeek.Edit"))
                .EditableControls.Add(fsControls.Controls)
                .AllowBack = False
                .AllowDelete = False
                .AllowFind = False
                .AllowNew = False
            End With
            AddHandler stdBut.FindClicked, AddressOf FindClicked
            AddHandler stdBut.EditClicked, AddressOf FindClicked
            AddHandler stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler stdBut.CancelClicked, AddressOf FindClicked

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim sysInfo As SystemInfo

            sysInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, True)
            If Utils.IsDate(sysInfo.DPIWeeksClosedTo) Then dteDate.Text = sysInfo.DPIWeeksClosedTo

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim sysInfo As SystemInfo

            If Me.IsValid Then
                sysInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, True)
                With sysInfo
                    If Not Utils.IsDate(dteDate.Text) Then
                        .DPIWeeksClosedTo = Nothing
                    Else
                        ' validate week ending date
                        msg = DomContractBL.ValidateWeekEndingDate(Me.DbConnection, dteDate.Text)
                        If Not msg.Success Then
                            e.Cancel = True
                            lblError.Text = msg.Message
                            Return
                        End If
                        .DPIWeeksClosedTo = dteDate.Text
                    End If
                    .DbConnection = Me.DbConnection
                    msg = .Save()
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End With
            Else
                e.Cancel = True
            End If

        End Sub

    End Class

End Namespace