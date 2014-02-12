
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.SDS
Imports Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Sds

    ''' <summary>
    ''' Admin page used to maintain budget category groups.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     ColinD       07/12/2010    Updated D11964A - Added grouping of units of service functionality 
    '''     JohnF        05/07/2010    Created (D11794)
    ''' </history>
    Partial Public Class BudgetCategoryGroup
        Inherits BasePage

#Region "Fields"

        Private _CurrentSdsInvoicingMethod As SdsBL.SDSInvoicingMethod

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Load event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.BudgetCategoryGroups"), "Budget Category Groups")

            Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.BudgetCategoryGroups.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.BudgetCategoryGroups.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.BudgetCategoryGroups.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .EditableControls.Add(fsSummarisationControls.Controls)
                .GenericFinderTypeID = GenericFinderType.SDS_BudgetCategoryGroup
                .AuditLogTableNames.Add("BudgetCategoryGroup")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.BudgetCategoryGroups")
            End With
            AddHandler stdBut.FindClicked, AddressOf FindClicked
            AddHandler stdBut.EditClicked, AddressOf EditClicked
            AddHandler stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler stdBut.DeleteClicked, AddressOf DeleteClicked

        End Sub

        ''' <summary>
        ''' Handles the PreRenderComplete event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            If CurrentSdsInvoicingMethod = SdsBL.SDSInvoicingMethod.CapActualServiceCostAtAssessedCharge Then
                ' only allow editing if sds invoicing method is CapActualServiceCostAtAssessedCharge
                chkGroupUnitsOfServiceOnInvoice.Enabled = True
            Else
                ' else invalid sds invoicing methodf so disable controls
                chkGroupUnitsOfServiceOnInvoice.Enabled = False
            End If

            lblUnitOfMeasure.Style.Add("margin-left", "1.65em")

        End Sub

        ''' <summary>
        ''' Handles the EditClicked Event Handler of the Standard Buttons Control
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            ' now find the record to populate controls on screen
            FindClicked(e)

        End Sub

        ''' <summary>
        ''' Handles the FindClicked Event Handler of the Standard Buttons Control
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim group As Target.Abacus.Library.DataClasses.BudgetCategoryGroup
            Dim dum As DomUnitsOfMeasure = Nothing
            Dim dumDescription As String = "Not Set"
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            group = New Target.Abacus.Library.DataClasses.BudgetCategoryGroup(Me.DbConnection, _
                                                                              currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With group
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                chkRedundant.CheckBox.Checked = .Redundant
                chkGroupUnitsOfServiceOnInvoice.Checked = .GroupUnitsOfServiceOnServiceUserStatement
                DisplayUnitOfMeasure(.UnitOfMeasureID)
            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
                chkRedundant.CheckBox.Checked = False
                DisplayUnitOfMeasure(0)
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = BudgetCategoryGroupBL.Delete(Me.DbConnection, _
                                               currentUser.ExternalUsername, _
                                               AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), _
                                               e.ItemID)
            If Not msg.Success Then
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If
        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage
            Dim group As Target.Abacus.Library.DataClasses.BudgetCategoryGroup
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            Try
                If Me.IsValid Then
                    group = New Target.Abacus.Library.DataClasses.BudgetCategoryGroup(Me.DbConnection, _
                                                                                      currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                    If e.ItemID > 0 Then
                        ' update
                        With group
                            msg = BudgetCategoryGroupBL.Fetch(e.ItemID, group)
                            If Not msg.Success Then
                                lblError.Text = msg.Message
                                e.Cancel = True
                                Exit Sub
                            End If
                        End With
                    End If
                    With group
                        .Description = txtDescription.Text
                        .Redundant = chkRedundant.CheckBox.Checked
                        If CurrentSdsInvoicingMethod = SdsBL.SDSInvoicingMethod.CapActualServiceCostAtAssessedCharge Then
                            .GroupUnitsOfServiceOnServiceUserStatement = chkGroupUnitsOfServiceOnInvoice.Checked
                        End If
                        msg = BudgetCategoryGroupBL.Save(Me.DbConnection, group)
                        If Not msg.Success Then
                            lblError.Text = msg.Message
                            e.Cancel = True
                            Exit Sub
                        End If
                        e.ItemID = .ID
                        FindClicked(e)
                    End With
                Else
                    e.Cancel = True
                End If
            Catch ex As Exception
                msg.Success = False
                msg.Message = ex.Message
                msg.ExMessage = String.Concat(ex.Message, ex.StackTrace)
            Finally
                If Not msg.Success Then
                    lblError.Text = msg.Message
                Else
                    lblError.Text = ""
                End If
            End Try

        End Sub

#End Region

#Region "Methods"

        ''' <summary>
        ''' Displays the unit of measure.
        ''' </summary>
        ''' <param name="id">The id.</param>
        Private Sub DisplayUnitOfMeasure(ByVal id As Integer)

            Dim msg As ErrorMessage = Nothing
            Dim dum As DomUnitsOfMeasure = Nothing
            Dim dumDescription As String = "Not Set"

            If id > 0 Then
                ' if we have a dom units of measure record to fetch then do so

                dum = New DomUnitsOfMeasure(DbConnection, String.Empty, String.Empty)

                ' fetch the dom unit of measure and throw error if not found
                msg = dum.Fetch(id)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                dumDescription = dum.Description

            End If

            lblUnitOfMeasure.Text = String.Format("Unit of Measure : {0}", dumDescription)

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the current SDS invoicing method.
        ''' </summary>
        ''' <value>The current SDS invoicing method.</value>
        Public ReadOnly Property CurrentSdsInvoicingMethod() As SdsBL.SDSInvoicingMethod
            Get
                If CType(_CurrentSdsInvoicingMethod, Integer) = 0 Then
                    ' if havent fetched the invoicing method then do so
                    Dim msg As ErrorMessage = Nothing
                    msg = SdsBL.GetSDSInvoicingMethodSetting(DbConnection, _
                                                             Nothing, _
                                                             _CurrentSdsInvoicingMethod)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                End If
                Return _CurrentSdsInvoicingMethod
            End Get
        End Property

#End Region

    End Class

End Namespace