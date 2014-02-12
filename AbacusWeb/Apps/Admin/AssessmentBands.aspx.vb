Imports Target.Web.Apps
Imports Target.Library
Imports Target.Web.Apps.Security
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library
Imports System.Collections.Generic

Namespace Apps.Admin

    Partial Public Class Assessmentbands
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _FetchedAssessmentBands As Dictionary(Of Integer, SDSAssessmentBand)

        Private Const _GeneralErrorNumber As String = ErrorMessage.GeneralErrorNumber

#Region " AuditLogTitle "
        ''' <summary>
        ''' Gets the audit log title.
        ''' </summary>
        ''' <value>The audit log title.</value>
        Private ReadOnly Property AuditLogTitle() As String
            Get
                Return AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings)
            End Get
        End Property
#End Region

#Region " AuditLogUserName "
        ''' <summary>
        ''' Gets the name of the audit log user.
        ''' </summary>
        ''' <value>The name of the audit log user.</value>
        Private ReadOnly Property AuditLogUserName() As String
            Get
                Return CurrentUser.ExternalUsername
            End Get
        End Property
#End Region

#Region " FetchedAssessmentBands "
        ''' <summary>
        ''' Gets or sets the fetched Assessment Bands.
        ''' </summary>
        ''' <value>The fetched Assessment Bands.</value>
        Private ReadOnly Property FetchedAssessmentBands() As Dictionary(Of Integer, SDSAssessmentBand)
            Get
                If _FetchedAssessmentBands Is Nothing Then
                    ' always init the dictionary if null
                    _FetchedAssessmentBands = New Dictionary(Of Integer, SDSAssessmentBand)
                End If
                Return _FetchedAssessmentBands
            End Get
        End Property
#End Region

#Region " CurrentUser "
        ''' <summary>
        ''' Gets the current user.
        ''' </summary>
        ''' <value>The current user.</value>
        Private ReadOnly Property CurrentUser() As WebSecurityUser
            Get
                If _CurrentUser Is Nothing Then
                    ' if current user not fetched then get current user
                    _CurrentUser = SecurityBL.GetCurrentUser()
                End If
                Return _CurrentUser
            End Get
        End Property
#End Region

#Region " Page_Load "
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Income.ReferenceData.AssessmentBands"), "Assessment Bands")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ReferenceData.AssessmentBands.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ReferenceData.AssessmentBands.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ReferenceData.AssessmentBands.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                    .Add("Band", "Band")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.AssessmentBands
                .AuditLogTableNames.Add("SDSAssessmentBand")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.SDSAssessmentBand")
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf EditClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked
            AddHandler _stdBut.NewClicked, AddressOf NewClicked


        End Sub
#End Region

#Region " CancelClicked "
        ''' <summary>
        ''' EventHandler for the Standard Buttons CancelClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)

            FindClicked(e)

        End Sub

#End Region

#Region " DeleteClicked "
        ''' <summary>
        ''' EventHandler for the Standard Buttons DeleteClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            Try
                ' try and delete the rate, throw an error if cant
                msg = AssessmentBL.DeleteAssessmentBand(DbConnection, e.ItemID, AuditLogUserName, AuditLogTitle)

                ' if deletion successful then exit routine
                If msg.Success Then Exit Sub

                If msg.Number = AssessmentBL.ErrorCannotDeleteAssessmentBandRate Then
                    ' a validation error of some sort occurred so display it
                    lblError.Text = msg.Message

                    e.Cancel = True

                    FindClicked(e)
                Else
                    ' another type of error occurred so display it hard
                    WebUtils.DisplayError(msg)
                End If

            Catch ex As Exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)

                If Not msg.Success Then
                    e.Cancel = True
                    WebUtils.DisplayError(msg)
                End If

            End Try

        End Sub

#End Region

#Region " EditClicked "
        ''' <summary>
        ''' EventHandler for the Standard Buttons EditClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub EditClicked(ByRef e As StdButtonEventArgs)

            FindClicked(e)

        End Sub
#End Region

#Region " FindClicked "
        ''' <summary>
        ''' EventHandler for the Standard Buttons FindClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            PopulateAssessmentBand(e)

        End Sub
#End Region

#Region " NewClicked "
        ''' <summary>
        ''' EventHandler for the Standard Buttons NewClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub NewClicked(ByRef e As StdButtonEventArgs)

            FindClicked(e)

        End Sub
#End Region

#Region " SaveClicked "
        ''' <summary>
        ''' EventHandler for the Standard Buttons SaveClicked Event.
        ''' </summary>
        ''' <param name="e">The <see cref="Target.Library.Web.UserControls.StdButtonEventArgs" /> instance containing the event data.</param>
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            Try
                Dim assessmentBand As SDSAssessmentBand = GetAssessmentBandFromScreen()

                ' screen is not valid
                If Not IsValid Then
                    e.Cancel = True
                    Exit Sub
                End If

                ' save the benefit rate
                msg = AssessmentBL.SaveAssessmentBand(DbConnection, assessmentBand)

                If Not msg.Success Then
                    ' if didnt succeed then check why and display error
                    If msg.Number = AssessmentBL.ErrorCannotSaveAssessmentBand Then
                        ' a validation error of some sort occurred so display it
                        lblError.Text = msg.Message

                        ' cancel remaining processing
                        e.Cancel = True
                    Else
                        ' another type of error occurred so display it hard
                        WebUtils.DisplayError(msg)
                    End If
                Else
                    ' save succeeded so set the id and find again from db
                    e.ItemID = assessmentBand.ID
                    FindClicked(e)
                End If

            Catch ex As Exception
                ' catch the exception
                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)

                If Not msg.Success Then
                    e.Cancel = True
                    WebUtils.DisplayError(msg)
                End If

            End Try

        End Sub
#End Region

#Region " GetAssessmentBand "
        ''' <summary>
        ''' Gets the Assessment band.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetAssessmentBand(ByVal id As Integer) As SDSAssessmentBand

            Dim item As SDSAssessmentBand = Nothing
            Dim msg As New ErrorMessage()

            If FetchedAssessmentBands.ContainsKey(id) Then
                ' if already fetched then return that object

                item = FetchedAssessmentBands(id)

            Else
                ' else not already fetched so get the item from db

                msg = AssessmentBL.GetAssessmentBand(DbConnection, id, AuditLogUserName, AuditLogTitle, item)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' add the item to the dictionary for further use
                FetchedAssessmentBands.Add(id, item)

            End If

            Return item

        End Function
#End Region

#Region " GetAssessmentBandFromScreen "
        ''' <summary>
        ''' Gets the Assessment band from screen.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetAssessmentBandFromScreen() As SDSAssessmentBand

            Dim msg As New ErrorMessage()
            Dim band As SDSAssessmentBand = Nothing
            Dim itemId As Integer = _stdBut.SelectedItemID


            If itemId > 0 Then
                ' if we are modifying an existing assessment band then fetch db copy and make changes to that

                band = GetAssessmentBand(itemId)

            Else
                ' else item is new so create new object

                band = New SDSAssessmentBand(DbConnection, AuditLogUserName, AuditLogTitle)

            End If

            ' setup values on the assessment band from the screen
            With band
                ' Band value is not changed if editing existing Band
                If Not itemId > 0 Then .Band = txtBand.Value

                .Description = txtDescription.Text
                .Redundant = chkRedundant.CheckBox.Checked
            End With

            Return band

        End Function
#End Region

#Region " PopulateAssessmentBand "
        ''' <summary>
        ''' Populates the screen with a Assessment band using its id.
        ''' </summary>
        ''' <param name="e">The id.</param>
        Private Sub PopulateAssessmentBand(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            If e.ItemID > 0 Then
                ' if we have an id to work with then do so

                ' populate the item on screen
                msg = PopulateAssessmentBand(GetAssessmentBand(e.ItemID))

            Else
                ' else no id so throw an error

                msg = PopulateAssessmentBand(CType(Nothing, SDSAssessmentBand))

            End If

            If Not msg.Success Then
                ' if errored populating screen display error

                e.Cancel = True
                WebUtils.DisplayError(msg)

            End If

        End Sub


        ''' <summary>
        ''' Populates the screen with the assessment band.
        ''' </summary>
        ''' <param name="band">The assessment band.</param>
        Private Function PopulateAssessmentBand(ByVal band As SDSAssessmentBand) As ErrorMessage

            Dim msg As New ErrorMessage()

            Try

                ' set properties on controls using properties of form
                ' they will handle all formatting etc
                If Not band Is Nothing Then
                    ' if we have a band then we are working with an existing one
                    txtBand.Text = band.Band
                    txtDescription.Text = band.Description
                    chkRedundant.CheckBox.Checked = band.Redundant
                Else
                    ' else we have no rate so we are working with a new/deleteed one, so setup defaults
                    txtBand.Text = String.Empty
                    txtDescription.Text = String.Empty
                    chkRedundant.CheckBox.Checked = False
                End If

                ' set message to success
                msg.Success = True

            Catch ex As Exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)

            End Try

            Return msg

        End Function
#End Region

        ''' <summary>
        ''' Using Pre-Render event to set field focus
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            'The Assessment band field will only be enabled when entering a new record.
            If _stdBut.ButtonsMode <> StdButtonsMode.AddNew Then
                txtDescription.SetFocus = True
            Else
                txtBand.SetFocus = True
            End If
        End Sub

        ''' <summary>
        ''' Using Pre-Render Complete event to enable/disable txtBand
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete
            'The Assessment band field will only be enabled when entering a new record.
            txtBand.Enabled = (_stdBut.ButtonsMode = StdButtonsMode.AddNew)
        End Sub
    End Class

End Namespace