
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Admin.Dom

    ''' <summary>
	''' Admin page used to maintain domiciliary enhanced rate days.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MvO  12/05/2009  D11549 - added reporting support.
    ''' MvO  01/12/2008  D11444 - security overhaul.
    ''' </history>
	Partial Public Class EnhancedRateDays
		Inherits BasePage

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.EnhancedRateDays"), "Enhanced Rate Days")

			Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.EnhancedRateDays.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.EnhancedRateDays.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.EnhancedRateDays.Delete"))
                With .SearchBy
                    .Add("Date", "Date")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.DomEnhancedRateDay
                .AuditLogTableNames.Add("DomEnhancedRateDay")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.EnhancedRateDays")
            End With
			AddHandler stdBut.FindClicked, AddressOf FindClicked
			AddHandler stdBut.EditClicked, AddressOf FindClicked
			AddHandler stdBut.SaveClicked, AddressOf SaveClicked
			AddHandler stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler stdBut.DeleteClicked, AddressOf DeleteClicked

            AddJQuery()
		End Sub

		Private Sub FindClicked(ByRef e As StdButtonEventArgs)

			Dim msg As ErrorMessage
			Dim erd As DomEnhancedRateDay
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            erd = New DomEnhancedRateDay(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With erd
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                dteDate.Text = .EnhancedDay
            End With

        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                dteDate.Text = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = DomEnhancedRateDay.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim erd As DomEnhancedRateDay
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If Me.IsValid Then
                erd = New DomEnhancedRateDay(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With erd
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                End If
                With erd
                    .EnhancedDay = dteDate.Text
                    msg = .Save()
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    e.ItemID = .ID
                End With
            Else
                e.Cancel = True
            End If

        End Sub

#Region " Use JQuery "
        Private Sub AddJQuery()

            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery ui library for popups and table filtering etc
            UseJqueryUI = True

            ' add in the table filter library 
            UseJqueryTableFilter = True

            ' add the table scroller library as we might have large amounts of data
            UseJqueryTableScroller = True

            ' add the searchable menu
            UseJquerySearchableMenu = True

            ' add the jquery tooltip
            UseJqueryTooltip = True

            UseJqueryTemplates = True
        End Sub
#End Region
	End Class

End Namespace