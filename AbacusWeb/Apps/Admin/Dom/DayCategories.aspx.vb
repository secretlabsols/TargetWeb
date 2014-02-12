
Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.Admin.Dom

    ''' <summary>
	''' Admin page used to maintain domiciliary day categories.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      12/05/2009  D11549 - added reporting support.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    ''' </history>
	Partial Public Class DayCategories
        Inherits BasePage

        Private _inUseFlags As DayCategoryInUseFlags
        Private _stdBut As StdButtonsBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DayCategories"), "Day Categories")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowNew = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCategories.AddNew"))
                .AllowEdit = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCategories.Edit"))
                .AllowDelete = Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.DayCategories.Delete"))
                With .SearchBy
                    .Add("Description", "Description")
                    .Add("Abbreviation", "Abbreviation")
                End With
                .EditableControls.Add(fsControls.Controls)
                .GenericFinderTypeID = GenericFinderType.DomDayCategory
                .AuditLogTableNames.Add("DomDayCategory")
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DayCategories")
            End With
            AddHandler _stdBut.NewClicked, AddressOf NewClicked
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf EditClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked
            AddHandler _stdBut.DeleteClicked, AddressOf DeleteClicked

        End Sub

		Private Sub NewClicked(ByRef e As StdButtonEventArgs)
			PopulateTimeBandGroups()
			PopulateStandardEnhanced()
		End Sub

		Private Sub FindClicked(ByRef e As StdButtonEventArgs)

			Dim msg As ErrorMessage
			Dim ddc As DomDayCategory
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

			PopulateStandardEnhanced()

            ddc = New DomDayCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
            With ddc
                msg = .Fetch(e.ItemID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                txtDescription.Text = .Description
                txtAbbreviation.Text = .Abbreviation
                chkRedundant.CheckBox.Checked = .Redundant
                PopulateTimeBandGroups(.DomTimeBandGroupID)
                'cboTimeBandGroupID.DropDownList.SelectedValue = .DomTimeBandGroupID
                If .Standard And .Enhanced Then
                    cboStandardEnhanced.DropDownList.SelectedValue = StandardEnhanced.Both.ToString()
                ElseIf .Standard Then
                    cboStandardEnhanced.DropDownList.SelectedValue = StandardEnhanced.Standard.ToString()
                ElseIf .Enhanced Then
                    cboStandardEnhanced.DropDownList.SelectedValue = StandardEnhanced.Enhanced.ToString()
                End If
                chkMonday.CheckBox.Checked = .Monday
                chkTuesday.CheckBox.Checked = .Tuesday
                chkWednesday.CheckBox.Checked = .Wednesday
                chkThursday.CheckBox.Checked = .Thursday
                chkFriday.CheckBox.Checked = .Friday
                chkSaturday.CheckBox.Checked = .Saturday
                chkSunday.CheckBox.Checked = .Sunday
            End With

        End Sub

        Private Sub EditClicked(ByRef e As StdButtonEventArgs)
            FindClicked(e)
            DetermineInUse(e.ItemID)
        End Sub

        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            If e.ItemID = 0 Then
                txtDescription.Text = String.Empty
                txtAbbreviation.Text = String.Empty
                chkRedundant.CheckBox.Checked = TriState.False
                cboTimeBandGroupID.DropDownList.SelectedValue = String.Empty
                cboStandardEnhanced.DropDownList.SelectedValue = String.Empty
                chkMonday.CheckBox.Checked = TriState.False
                chkTuesday.CheckBox.Checked = TriState.False
                chkWednesday.CheckBox.Checked = TriState.False
                chkThursday.CheckBox.Checked = TriState.False
                chkFriday.CheckBox.Checked = TriState.False
                chkSaturday.CheckBox.Checked = TriState.False
                chkSunday.CheckBox.Checked = TriState.False
            Else
                FindClicked(e)
            End If
        End Sub

        Private Sub DeleteClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = DomDayCategory.Delete(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), e.ItemID)
            If Not msg.Success Then
                lblError.Text = msg.Message
                e.Cancel = True
                FindClicked(e)
            End If

        End Sub

        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim ddc As DomDayCategory
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim stdEnhValue As String

            ' as we are not using viewstate because it fecks other things up, dropdowns need a bit of extra work to pass validation
            PopulateTimeBandGroups()
            PopulateStandardEnhanced()

            DetermineInUse(e.ItemID)

            If _inUseFlags.InUseOnRateCat Then
                cboTimeBandGroupID.RequiredValidator.Enabled = False
            Else
                cboTimeBandGroupID.SelectPostBackValue()
            End If
            cboStandardEnhanced.SelectPostBackValue()
            stdEnhValue = cboStandardEnhanced.DropDownList.SelectedValue

            Me.Validate("Save")

            If Me.IsValid Then

                ddc = New DomDayCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))
                If e.ItemID > 0 Then
                    ' update
                    With ddc
                        msg = .Fetch(e.ItemID)
                        If Not msg.Success Then WebUtils.DisplayError(msg)
                    End With
                End If
                With ddc
                    .Description = txtDescription.Text
                    .Abbreviation = txtAbbreviation.Text
                    .Redundant = chkRedundant.CheckBox.Checked

                    ' cannot change time band group if day cat is in use on one or more rate categories
                    If _inUseFlags.InUseOnRateCat Then
                        cboTimeBandGroupID.DropDownList.SelectedValue = .DomTimeBandGroupID
                    Else
                        .DomTimeBandGroupID = cboTimeBandGroupID.DropDownList.SelectedValue
                    End If

                    If _inUseFlags.InUseOnStandardRFD Then
                        ' cannot change Standard from True to False if in use on an standard rate fromaework detail record
                        If .Standard And Not (stdEnhValue = StandardEnhanced.Standard.ToString() Or stdEnhValue = StandardEnhanced.Both.ToString()) Then
                            lblError.Text = "You cannot removed the Standard flag from this day category as it is in use by one or more Standard Rate Framework records."
                            e.Cancel = True
                        End If
                    End If
                    If _inUseFlags.InUseOnEnhancedRFD Then
                        ' cannot change Enhanced from True to False if in use on an enhaned rate fromaework detail record
                        If .Enhanced And Not (stdEnhValue = StandardEnhanced.Enhanced.ToString() Or stdEnhValue = StandardEnhanced.Both.ToString()) Then
                            lblError.Text = "You cannot removed the Enhanced flag from this day category as it is in use by one or more Enhanced Rate Framework records."
                            e.Cancel = True
                        End If
                    End If
                    .Standard = (stdEnhValue = StandardEnhanced.Standard.ToString() Or stdEnhValue = StandardEnhanced.Both.ToString())
                    .Enhanced = (stdEnhValue = StandardEnhanced.Enhanced.ToString() Or stdEnhValue = StandardEnhanced.Both.ToString())

                    ' cannot change DoW indicators if they are ticked and the day cat is in use one one or more 
                    ' rate framework details with the same DoW
                    If .Sunday And _inUseFlags.InUseOnSundayRFD Then
                        chkSunday.CheckBox.Checked = .Sunday
                    Else
                        .Sunday = chkSunday.CheckBox.Checked
                    End If
                    If .Monday And _inUseFlags.InUseOnMondayRFD Then
                        chkMonday.CheckBox.Checked = .Monday
                    Else
                        .Monday = chkMonday.CheckBox.Checked
                    End If
                    If .Tuesday And _inUseFlags.InUseOnTuesdayRFD Then
                        chkTuesday.CheckBox.Checked = .Tuesday
                    Else
                        .Tuesday = chkTuesday.CheckBox.Checked
                    End If
                    If .Wednesday And _inUseFlags.InUseOnWednesdayRFD Then
                        chkWednesday.CheckBox.Checked = .Wednesday
                    Else
                        .Wednesday = chkWednesday.CheckBox.Checked
                    End If
                    If .Thursday And _inUseFlags.InUseOnThursdayRFD Then
                        chkThursday.CheckBox.Checked = .Thursday
                    Else
                        .Thursday = chkThursday.CheckBox.Checked
                    End If
                    If .Friday And _inUseFlags.InUseOnFridayRFD Then
                        chkFriday.CheckBox.Checked = .Friday
                    Else
                        .Friday = chkFriday.CheckBox.Checked
                    End If
                    If .Saturday And _inUseFlags.InUseOnSaturdayRFD Then
                        chkSaturday.CheckBox.Checked = .Saturday
                    Else
                        .Saturday = chkSaturday.CheckBox.Checked
                    End If

                    ' validation error higher up so bail out
                    If e.Cancel Then Return

                    ' validate/save
                    msg = DomContractBL.SaveDayCategory(Me.DbConnection, ddc)
                    If Not msg.Success Then
                        If msg.Number = "E3004" Or msg.Number = "E3003" Then
                            ' rate category abbrev invalid or could not save day category
                            lblError.Text = msg.Message
                            e.Cancel = True
                        Else
                            WebUtils.DisplayError(msg)
                        End If
                    End If
                    e.ItemID = ddc.ID
                End With
            Else
                e.Cancel = True
            End If

        End Sub

        Private Sub PopulateTimeBandGroups(Optional ByVal timeBandGroupID As Integer = 0)

            Dim msg As ErrorMessage
            Dim tbGroups As DomTimeBandGroupCollection = Nothing
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            msg = DomTimeBandGroup.FetchList(Me.DbConnection, tbGroups, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings), TriState.False)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            With cboTimeBandGroupID.DropDownList
                .Items.Clear()
                .DataSource = tbGroups
                .DataTextField = "Description"
                .DataValueField = "ID"
                .DataBind()
                .Items.Insert(0, New ListItem(String.Empty))
            End With

            If timeBandGroupID <> 0 Then
                Dim itemFound As Boolean = False
                For Each tBand As DomTimeBandGroup In tbGroups
                    If tBand.ID = timeBandGroupID Then
                        cboTimeBandGroupID.DropDownList.SelectedValue = timeBandGroupID
                        itemFound = True
                        Exit For
                    End If
                Next

                If Not itemFound Then
                    Dim tBand As New DomTimeBandGroup(Me.DbConnection, String.Empty, String.Empty)
                    msg = tBand.Fetch(timeBandGroupID)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    cboTimeBandGroupID.DropDownList.Items.Insert(cboTimeBandGroupID.DropDownList.Items.Count, New ListItem(tBand.Description, tBand.ID))
                    cboTimeBandGroupID.DropDownList.SelectedValue = timeBandGroupID
                End If

            End If

        End Sub

		Private Sub PopulateStandardEnhanced()

			With cboStandardEnhanced.DropDownList
                .Items.Clear()
                .DataSource = [Enum].GetNames(GetType(StandardEnhanced))
                .DataBind()
                .Items.Insert(0, New ListItem(String.Empty))
            End With

        End Sub

        Private Sub DetermineInUse(ByVal dayCategoryID As Integer)

            Dim msg As ErrorMessage

            If dayCategoryID = 0 Then
                _inUseFlags = New DayCategoryInUseFlags()
                Return
            End If

            If _inUseFlags Is Nothing Then
                msg = DomContractBL.DetermineDayCategoryInUseFlags(Me.DbConnection, dayCategoryID, _inUseFlags)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End If

        End Sub

		Private Enum StandardEnhanced As Byte
			Standard = 1
			Enhanced = 2
			Both = 3
        End Enum

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            If Not _inUseFlags Is Nothing Then
                With _inUseFlags
                    ' cannot change time band group if day cat is in use on one or more rate categories
                    If _stdBut.ButtonsMode = StdButtonsMode.Edit Then
                        cboTimeBandGroupID.DropDownList.Enabled = Not .InUseOnRateCat
                    End If
                    ' cannot change DoW indicators if they are ticked and the day cat is in use one one or more 
                    ' rate framework details with the same DoW
                    If chkSunday.CheckBox.Checked And .InUseOnSundayRFD Then chkSunday.CheckBox.Enabled = False
                    If chkMonday.CheckBox.Checked And .InUseOnMondayRFD Then chkMonday.CheckBox.Enabled = False
                    If chkTuesday.CheckBox.Checked And .InUseOnTuesdayRFD Then chkTuesday.CheckBox.Enabled = False
                    If chkWednesday.CheckBox.Checked And .InUseOnWednesdayRFD Then chkWednesday.CheckBox.Enabled = False
                    If chkThursday.CheckBox.Checked And .InUseOnThursdayRFD Then chkThursday.CheckBox.Enabled = False
                    If chkFriday.CheckBox.Checked And .InUseOnFridayRFD Then chkFriday.CheckBox.Enabled = False
                    If chkSaturday.CheckBox.Checked And .InUseOnSaturdayRFD Then chkSaturday.CheckBox.Enabled = False
                End With
            End If

        End Sub

    End Class

End Namespace