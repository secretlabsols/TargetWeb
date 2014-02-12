
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Reports
Imports Target.Web.Apps.Security

Namespace Apps.Reports.Admin

    ''' <summary>
    ''' Admin page used to maintain report categories.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      22/10/2009  D11710 - created.
    ''' </history>
    Partial Public Class ReportCategories
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _imgPadlock As HtmlImage = New HtmlImage
        Private _showPadlock As Boolean

#Region " Page_Init "

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            AddHandler _stdBut.AddCustomControls, AddressOf StdButtons_AddCustomControls
        End Sub

#End Region

#Region " Page_Load  "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebNavMenuItem.Reporting.ReportCategories"), "Report Categories")

            'Dim stdBut As StdButtonsBase = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Reporting.ReportCategories.AddNew"))
                With .SearchBy
                    .Add("Description", "Description")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.ReportCategories
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant(Me.CurrentApplicationFromConfig, "WebReport.ReportCategories")
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked
        End Sub

#End Region

#Region " Page_PreRender "

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            _imgPadlock.Visible = _showPadlock

            With _stdBut
                If _showPadlock Then
                    .AllowDelete = False
                    .AllowEdit = False
                Else
                    .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Reporting.ReportCategories.Edit"))
                    .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.Reporting.ReportCategories.Delete"))
                End If
            End With

        End Sub

#End Region

#Region " StdButtons_AddCustomControls "

        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)

            With _imgPadlock
                .ID = "imgPadlock"
                .Src = WebUtils.GetVirtualPath("Images/PadLock.gif")
                _imgPadlock.Visible = False
            End With
            controls.Add(_imgPadlock)

        End Sub

#End Region

#Region " FindClicked "

        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim cat As WebReportCategory
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            cat = New WebReportCategory(Me.DbConnection)
            With cat
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                _showPadlock = .SystemCategory
            End With

        End Sub

#End Region

#Region " CancelClicked "

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
            Else
                FindClicked(e)
            End If
        End Sub

#End Region

#Region " DeleteClicked "

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = WebReportCategory.Delete(Me.DbConnection, e.ItemID)
            If Not msg.Success Then
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If

        End Sub

#End Region

#Region " SaveClicked "

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim cat As WebReportCategory
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            If Me.IsValid Then
                cat = New WebReportCategory(Me.DbConnection)
                If e.ItemID > 0 Then
                    ' update
                    With cat
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                End If
                With cat
                    .Description = txtDescription.Text
                    msg = .Save()
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    e.ItemID = .ID
                End With
            Else
                e.Cancel = True
            End If

        End Sub

#End Region

    End Class

End Namespace