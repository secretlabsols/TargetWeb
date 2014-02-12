Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Abacus.Library.PaymentTolerance


Namespace Apps.PaymentTolerance

    ''' <summary>
    ''' Admin page used to maintain payment tolerance groups.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir    01/12/2011  BTI509 Delete button is still showing and functional when permission is set to not allow  
    '''     MoTahir    03/08/2011  D11766 eInvoicing - Provider Invoice Tolerances
    ''' </history>
    Partial Public Class PaymentToleranceGroups
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _pTolGroup As PaymentToleranceGroup = Nothing
        Private _existing_pTolGroup As PaymentToleranceGroup = Nothing

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.PaymentToleranceGroup"), "Payment Tolerance Groups")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentToleranceGroup.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentToleranceGroup.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentToleranceGroup.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                    .Add("Redundant", "Redundant")
                    .Add("System Type", "SystemType")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.PaymentToleranceGroup
                .AuditLogTableNames.Add("PaymentToleranceGroup")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.PaymentToleranceGroup")
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

            msg = PaymentToleranceBL.FetchPaymentToleranceGroup(Me.DbConnection, _pTolGroup, e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With _pTolGroup
                txtDescription.Text = .Description
                chkRedundant.CheckBox.Checked = .Redundant
            End With

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

            msg = PaymentToleranceBL.DeletePaymentToleranceGroup(Me.DbConnection, e.ItemID)

            If Not msg.Success Then
                e.Cancel = True
                lblError.Text = msg.Message
                FindClicked(e)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            _pTolGroup = New PaymentToleranceGroup(currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            If Me.IsValid Then

                If e.ItemID > 0 Then
                    _pTolGroup.ID = e.ItemID
                End If

                _pTolGroup.Description = txtDescription.Text
                _pTolGroup.Redundant = chkRedundant.CheckBox.Checked

                msg = PaymentToleranceBL.SavePaymentToleranceGroup(Me.DbConnection, _pTolGroup)

                If Not msg.Success Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    Exit Sub
                End If
                e.ItemID = _pTolGroup.ID
                FindClicked(e)
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            Dim msg As ErrorMessage = Nothing

            If Not _pTolGroup Is Nothing AndAlso PaymentToleranceBL.IsPaymentToleranceGroupSystemType(_pTolGroup) Then
                'set the warning for system type Payment Tolerance Groups
                msg = Utils.CatchError(Nothing, "E3088")
                lblError.Text = msg.Message
                lblError.ForeColor = Color.Orange
            End If
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            If Not _pTolGroup Is Nothing Then
                'check if PaymentToleranceGroup is system type to determine display of Edit button
                _stdBut.AllowEdit = (Not PaymentToleranceBL.IsPaymentToleranceGroupSystemType(_pTolGroup) _
                                     And Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentToleranceGroup.Edit")))

                'check if PaymentToleranceGroup is system type to determine display of Delete button
                _stdBut.AllowDelete = (Not PaymentToleranceBL.IsPaymentToleranceGroupSystemType(_pTolGroup) _
                                       And Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.PaymentToleranceGroup.Delete")))
            End If

        End Sub
    End Class

End Namespace