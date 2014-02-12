Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.AdministrativeSector

Namespace Apps.AdministrativeSector

    ''' <summary>
    ''' Admin page used to maintain administration areas.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir    26/11/2013  D12535 Partitioning Of Data By Administrative Area
    ''' </history>
    Partial Public Class AdministrativeAreas
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _adminAreas As Abacus.Library.DataClasses.AdministrativeSector = Nothing
        Private _existingAdminAreas As Abacus.Library.DataClasses.AdministrativeSector = Nothing

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.AdministrativeAreas"), "Administrative Areas")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.AdministrativeAreas.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.AdministrativeAreas.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.AdministrativeAreas.Delete"))
                With .SearchBy
                    .Add("Title", "Title")
                    .Add("Redundant", "Redundant")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.AdministrativeAreas
                .AuditLogTableNames.Add("AdministrativeSector")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.AdministrativeAreas")
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage = Nothing

            msg = AdministrativeAreasBL.FetchAdministrativeArea(Me.DbConnection, _adminAreas, e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With _adminAreas
                txtTitle.Text = .Title
                chkRedundant.CheckBox.Checked = .redundant
            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtTitle.Text = String.Empty
                chkRedundant.CheckBox.Checked = False
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage

            msg = AdministrativeAreasBL.DeleteAdministrativeArea(Me.DbConnection, e.ItemID)

            If Not msg.Success Then
                e.Cancel = True
                lblError.Text = msg.Message
                FindClicked(e)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            _adminAreas = New Abacus.Library.DataClasses.AdministrativeSector(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            If Me.IsValid Then

                If e.ItemID > 0 Then
                    _adminAreas.ID = e.ItemID
                End If

                _adminAreas.Title = txtTitle.Text
                _adminAreas.Redundant = chkRedundant.CheckBox.Checked

                msg = AdministrativeAreasBL.SaveAdministrativeArea(Me.DbConnection, _adminAreas)

                If Not msg.Success Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    Exit Sub
                End If
                e.ItemID = _adminAreas.ID
                FindClicked(e)
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim msg As ErrorMessage = Nothing

            'If Not _adminAreas Is Nothing AndAlso PaymentToleranceBL.IsPaymentToleranceGroupSystemType(_adminAreas) Then
            '    'set the warning for system type Payment Tolerance Groups
            '    msg = Utils.CatchError(Nothing, "E3088")
            '    lblError.Text = msg.Message
            '    lblError.ForeColor = Color.Orange
            'End If
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            'If Not _adminAreas Is Nothing Then
            '    'check if PaymentToleranceGroup is system type to determine display of Edit button
            '    _stdBut.AllowEdit = (Not PaymentToleranceBL.IsPaymentToleranceGroupSystemType(_adminAreas) _
            '                         And Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentToleranceGroup.Edit")))

            '    'check if PaymentToleranceGroup is system type to determine display of Delete button
            '    _stdBut.AllowDelete = (Not PaymentToleranceBL.IsPaymentToleranceGroupSystemType(_adminAreas) _
            '                           And Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentToleranceGroup.Delete")))
            'End If

        End Sub
    End Class

End Namespace