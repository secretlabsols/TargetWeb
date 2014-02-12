Imports Target.Web.Apps
Imports Target.Library
Imports Target.Web.Apps.Security
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library
Imports System.Collections.Generic

Namespace Apps.Admin

    Partial Public Class AssessmentBandRates
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _CurrentUser As WebSecurityUser = Nothing
        Private _FetchedAssessmentBandRates As Dictionary(Of Integer, SDSAssessmentBandRates)

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

#Region " FetchedAssessmentBandRates "
        ''' <summary>
        ''' Gets or sets the fetched Assessment Band Rates.
        ''' </summary>
        ''' <value>The fetched Assessment Band Rates.</value>
        Private ReadOnly Property FetchedAssessmentBandRates() As Dictionary(Of Integer, SDSAssessmentBandRates)
            Get
                If _FetchedAssessmentBandRates Is Nothing Then
                    ' always init the dictionary if null
                    _FetchedAssessmentBandRates = New Dictionary(Of Integer, SDSAssessmentBandRates)
                End If
                Return _FetchedAssessmentBandRates
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

#Region " AssessmentBand "
        ''' <summary>
        ''' Gets or sets the Assessment Band from the aspx page.
        ''' </summary>
        ''' <value>The Assessment Band.</value>
        Private Property AssessmentBand() As String
            Get
                Dim band As SDSAssessmentBand = Nothing
                Dim msg As ErrorMessage

                msg = AssessmentBL.GetAssessmentBand(Me.DbConnection, Utils.ToInt32(cboAssessmentBand.GetPostBackValue()), AuditLogUserName, AuditLogTitle, band)
                If Not msg.Success Then Return String.Empty

                Return band.Band
            End Get
            Set(ByVal value As String)

                Dim bands As SDSAssessmentBandCollection = Nothing
                Dim msg As ErrorMessage

                If cboAssessmentBand.DropDownList.SelectedItem IsNot Nothing Then
                    ' if we have a selected item then unselect it

                    cboAssessmentBand.DropDownList.SelectedItem.Selected = False

                End If

                msg = SDSAssessmentBand.FetchList(Me.DbConnection, bands, AuditLogUserName, AuditLogTitle, value)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If value <> "" Then
                    If cboAssessmentBand.DropDownList.Items.FindByValue(bands(0).ID) Is Nothing AndAlso Not bands Is Nothing Then
                        ' if we cant find the item in the list then it is most probably redundant so populate the combo with the redundant record


                        ' add the item into the list
                        cboAssessmentBand.DropDownList.Items.Add(New ListItem(String.Format("{0} - {1}", bands(0).Band, bands(0).Description), bands(0).ID))

                    End If

                    If cboAssessmentBand.DropDownList.Items.FindByValue(bands(0).ID) IsNot Nothing Then
                        ' if the item exists

                        cboAssessmentBand.DropDownList.Items.FindByValue(bands(0).ID).Selected = True

                    Else
                        ' else the item doesn't exist so select default value...should always exist

                        cboAssessmentBand.DropDownList.Items.FindByValue(0).Selected = True

                    End If
                End If

            End Set
        End Property
#End Region

#Region " Page_Load "
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Income.ReferenceData.AssessmentBandRates"), "Assessment Band Rates")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ReferenceData.AssessmentBandRates.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ReferenceData.AssessmentBandRates.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ReferenceData.AssessmentBandRates.Delete"))
                With .SearchBy
                    .Add("Band", "Band")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.AssessmentBandRates
                .AuditLogTableNames.Add("SDSAssessmentBandRates")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.SDSAssessmentBandRates")
                .ReportButtonWidth = "5em"
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf EditClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked
            AddHandler _stdBut.NewClicked, AddressOf NewClicked

            AddJQuery()

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
                msg = AssessmentBL.DeleteAssessmentBandRate(DbConnection, e.ItemID, AuditLogUserName, AuditLogTitle)
                If Not msg.Success Then

                    e.Cancel = True
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

            PopulateAssessmentBandRate(e)

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

                Dim assessmentBandRate As SDSAssessmentBandRates = GetAssessmentBandRateFromScreen()

                If IsPostBack Then
                    ' disable the assessment band validator

                    cboAssessmentBand.RequiredValidator.Enabled = False

                End If

                If IsValid Then
                    ' if this screen has valid controls continue

                    ' save the benefit rate
                    msg = AssessmentBL.SaveAssessmentBandRate(DbConnection, assessmentBandRate)

                    If Not msg.Success Then
                        ' if didnt succeed then check why and display error

                        If msg.Number = AssessmentBL.ErrorCannotSaveAssessmentBandRate Then
                            ' a validation error of some sort occurred so display it

                            lblError.Text = msg.Message

                        Else
                            ' another type of error occurred so display it hard

                            WebUtils.DisplayError(msg)

                        End If

                        ' cancel remaining processing
                        e.Cancel = True

                    Else
                        ' save succeeded so set the id and find again from db

                        e.ItemID = assessmentBandRate.ID
                        FindClicked(e)

                    End If

                Else
                    ' else screen is not valid

                    e.Cancel = True

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

#Region " GetAssessmentBandRate "
        ''' <summary>
        ''' Gets the Assessment band Rate.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetAssessmentBandRate(ByVal id As Integer) As SDSAssessmentBandRates

            Dim item As SDSAssessmentBandRates = Nothing
            Dim msg As New ErrorMessage()

            If FetchedAssessmentBandRates.ContainsKey(id) Then
                ' if already fetched then return that object

                item = FetchedAssessmentBandRates(id)

            Else
                ' else not already fetched so get the item from db

                msg = AssessmentBL.GetAssessmentBandRate(DbConnection, id, AuditLogUserName, AuditLogTitle, item)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                ' add the item to the dictionary for further use
                FetchedAssessmentBandRates.Add(id, item)

            End If

            Return item

        End Function
#End Region

#Region " GetAssessmentBandRateFromScreen "
        ''' <summary>
        ''' Gets the Assessment band rate from screen.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetAssessmentBandRateFromScreen() As SDSAssessmentBandRates

            Dim msg As New ErrorMessage()
            Dim bandRate As SDSAssessmentBandRates = Nothing
            Dim itemId As Integer = _stdBut.SelectedItemID

            ' populate assessment bands first so we can select as required in properties
            msg = PopulateAssessmentBands()
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If itemId > 0 Then
                ' if we are modifying an existing assessment band rate then fetch db copy and make changes to that

                bandRate = GetAssessmentBandRate(itemId)

            Else
                ' else item is new so create new object

                bandRate = New SDSAssessmentBandRates(DbConnection, AuditLogUserName, AuditLogTitle)

            End If

            ' setup values on the assessment band from the screen
            With bandRate
                .SDSAssessmentBand = AssessmentBand
                .DateFrom = txtFromDate.Value
                If txtToDate.Value <> String.Empty Then
                    .DateTo = txtToDate.Value
                Else
                    .DateTo = Date.MaxValue.Date
                End If

                If txtAssessedCharge.Value <> String.Empty Then .AssessedCharge = txtAssessedCharge.Value
            End With

            Return bandRate

        End Function
#End Region

#Region " PopulateAssessmentBandRate "
        ''' <summary>
        ''' Populates the screen with a Assessment band rate using its id.
        ''' </summary>
        ''' <param name="e">The id.</param>
        Private Sub PopulateAssessmentBandRate(ByRef e As StdButtonEventArgs)

            Dim msg As New ErrorMessage()

            If e.ItemID > 0 Then
                ' if we have an id to work with then do so

                ' populate the item on screen
                msg = PopulateAssessmentBandRate(GetAssessmentBandRate(e.ItemID))

            Else
                ' else no id so throw an error

                msg = PopulateAssessmentBandRate(CType(Nothing, SDSAssessmentBandRates))

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
        ''' <param name="bandRate">The assessment band rate.</param>
        Private Function PopulateAssessmentBandRate(ByVal bandRate As SDSAssessmentBandRates) As ErrorMessage

            Dim msg As New ErrorMessage()

            Try

                ' populate assessment bands first so we can select as required in properties
                msg = PopulateAssessmentBands()
                If Not msg.Success Then Return msg

                ' set properties on controls using properties of form
                ' they will handle all formatting etc
                If Not bandRate Is Nothing Then
                    ' if we have a band then we are working with an existing one
                    AssessmentBand = bandRate.SDSAssessmentBand
                    txtFromDate.Text = bandRate.DateFrom
                    If bandRate.DateTo <> Date.MaxValue.Date Then
                        txtToDate.Text = bandRate.DateTo
                    End If
                    txtAssessedCharge.Text = bandRate.AssessedCharge.ToString("c")
                Else
                    ' else we have no rate so we are working with a new/deleteed one, so setup defaults
                    AssessmentBand = String.Empty
                    txtFromDate.Text = String.Empty
                    txtToDate.Text = String.Empty
                    txtAssessedCharge.Text = String.Empty
                End If

                ' setup success error message
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)

            End Try

            Return msg

        End Function
#End Region

#Region " PopulateAssessmentBands "
        ''' <summary>
        ''' Populates the assessment bands dropdown.
        ''' </summary>
        Private Function PopulateAssessmentBands() As ErrorMessage

            Dim items As SDSAssessmentBandCollection = Nothing
            Dim msg As New ErrorMessage()

            Try

                ' always clear all items from the list first
                cboAssessmentBand.DropDownList.Items.Clear()

                ' get non redundant items from db
                msg = AssessmentBL.GetAssessmentBands(connection:=DbConnection, items:=items, redundant:=TriState.False)
                If Not msg.Success Then Return msg

                If Not items Is Nothing AndAlso items.Count > 0 Then
                    'if we have some items to add to drop down then do so

                    For Each band As SDSAssessmentBand In items
                        With cboAssessmentBand.DropDownList
                            .Items.Add(New ListItem(String.Format("{0} - {1}", band.Band, band.Description), band.ID))
                        End With
                    Next

                End If

                ' add default item in to dd
                cboAssessmentBand.DropDownList.Items.Insert(0, New ListItem("", 0))

                ' setup success error message
                msg = New ErrorMessage()
                msg.Success = True

            Catch ex As Exception

                msg = Target.Library.Utils.CatchError(ex, _GeneralErrorNumber)

            End Try

            Return msg

        End Function

#End Region


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