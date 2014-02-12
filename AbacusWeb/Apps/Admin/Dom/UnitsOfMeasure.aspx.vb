
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
    ''' Admin page used to maintain domiciliary units of measure.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     PaulW       06/07/2012  A4WA#7247 - Minutes per unit not being maintained on a new record or where the value vas zero
    '''     ColinD      09/08/2011  D11965 - changes to handle removal of VisitBased flag.
    '''     ColinD      20/07/2010  D11893 - added SystemType column to data class, controls ability to edit/delete uoms
    '''     MikeVO      12/05/2009  D11549 - added reporting support.
    '''     MikeVO      06/01/2009  D11469 - created.
    ''' </history>
    Partial Public Class UnitsOfMeasure
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _inUseByRateCategory As Boolean
        Private _inUseByDSO As Boolean

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.UnitsOfMeasure"), "Units Of Measure")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.UnitsOfMeasure.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.UnitsOfMeasure.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.UnitsOfMeasure.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                    .Add("Comment", "Comment")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.DomUnitsOfMeasure
                .AuditLogTableNames.Add("DomUnitsOfMeasure")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.UnitsOfMeasure")
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

            Me.JsLinks.Add("UnitsOfMeasure.js")
            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))

            chkAllowUseServiceRegisters.CheckBox.Attributes.Add("onclick", "chkAllowUseServiceRegisters_Click();")
            chkUnitsDisplayedAsHoursMins.CheckBox.Attributes.Add("onclick", "chkUnitsDisplayedAsHoursMins_Click();")

        End Sub

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim uom As DomUnitsOfMeasure
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            uom = New DomUnitsOfMeasure(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With uom
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                chkAllowUseServiceRegisters.CheckBox.Checked = .AllowUseWithServiceRegisters
                txtComment.Text = .Comment
                chkUnitsDisplayedAsHoursMins.CheckBox.Checked = .UnitsDisplayedAsHoursMins
                txtMinutesPerUnit.Text = IIf(.MinutesPerUnit <> 0, .MinutesPerUnit, String.Empty)
                If uom.SystemType > 0 Then
                    ' if system type is larger than 0 then is a system type
                    ' , systems types cannot be changed so deny access to editing buttons
                    With _stdBut
                        .AllowDelete = False
                        .AllowEdit = False
                        .ShowSave = False
                    End With
                End If
            End With

            ' get "in use" flags
            msg = DomContractBL.UnitOfMeasureInUseByRateCategory(Me.DbConnection, uom.ID, _inUseByRateCategory)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            msg = DomContractBL.UnitOfMeasureInUseByDSO(Me.DbConnection, uom.ID, False, _inUseByDSO)
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
                txtComment.Text = String.Empty
                chkAllowUseServiceRegisters.CheckBox.Checked = False
                chkUnitsDisplayedAsHoursMins.CheckBox.Checked = False
                txtMinutesPerUnit.Text = False
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = DomContractBL.DeleteUnitOfMeasure(Me.DbConnection, _
                                                    e.ItemID, _
                                                    currentUser.ExternalUsername, _
                                                    AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings) _
            )
            If Not msg.Success Then
                If msg.Number = DomContractBL.ERR_COULD_NOT_DELETE_UOM Then
                    lblError.Text = msg.Message
                    e.Cancel = True
                    FindClicked(e)
                Else
                    WebUtils.DisplayError(msg)
                End If
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim uom As DomUnitsOfMeasure
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If Me.IsValid Then
                uom = New DomUnitsOfMeasure(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With uom
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With

                    ' get "in use" flags
                    msg = DomContractBL.UnitOfMeasureInUseByRateCategory(Me.DbConnection, uom.ID, _inUseByRateCategory)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    msg = DomContractBL.UnitOfMeasureInUseByDSO(Me.DbConnection, uom.ID, False, _inUseByDSO)
                    If Not msg.Success Then WebUtils.DisplayError(msg)

                End If

                With uom
                    .Description = txtDescription.Text
                    .Comment = txtComment.Text
                    .UnitsDisplayedAsHoursMins = chkUnitsDisplayedAsHoursMins.CheckBox.Checked
                    If .UnitsDisplayedAsHoursMins Then
                        .MinutesPerUnit = 60
                    Else
                        .MinutesPerUnit = Utils.ToInt32(txtMinutesPerUnit.Text)
                    End If
                    ' visit based checkbox cannot be changed if the UOM is in use by a rate category
                    If _inUseByRateCategory Then
                        chkAllowUseServiceRegisters.CheckBox.Checked = .AllowUseWithServiceRegisters
                    Else
                        .AllowUseWithServiceRegisters = chkAllowUseServiceRegisters.CheckBox.Checked
                    End If
                    If (.MinutesPerUnit > 0) Then

                        ' minutes per unit value cannot be changed if the UOM is in use by a summary-level DSO
                        If _inUseByDSO Then
                            txtMinutesPerUnit.Text = .MinutesPerUnit
                        Else
                            If .UnitsDisplayedAsHoursMins Then
                                .MinutesPerUnit = 60
                            Else
                                .MinutesPerUnit = Utils.ToInt32(txtMinutesPerUnit.Text)
                            End If
                        End If
                    Else
                        .UnitsDisplayedAsHoursMins = False
                        .MinutesPerUnit = 0
                    End If
                    msg = DomContractBL.SaveUnitOfMeasure(Me.DbConnection, uom)
                    If Not msg.Success Then
                        If msg.Number = DomContractBL.ERR_COULD_NOT_SAVE_UOM Then
                            lblError.Text = msg.Message
                            e.Cancel = True
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    Else
                        e.ItemID = .ID
                        FindClicked(e)
                    End If
                End With
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            ' visit based checkbox cannot be changed if it is in use by a rate category
            If _inUseByRateCategory Then
                chkAllowUseServiceRegisters.CheckBox.Enabled = False
            End If

            ' minutes per unit value cannot be changed if the UOM is in use by a summary-level DSO
            If _inUseByDSO Then
                chkUnitsDisplayedAsHoursMins.CheckBox.Enabled = False
                txtMinutesPerUnit.Enabled = False
            End If

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
                                                String.Format("uom_Mode={0};uom_inUseByDSO={1};", _
                                                    Convert.ToInt32(_stdBut.ButtonsMode), _
                                                    _inUseByDSO.ToString().ToLower() _
                                                ), _
                                                True)

        End Sub

    End Class

End Namespace