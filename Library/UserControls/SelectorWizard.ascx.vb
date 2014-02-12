
Imports System.Collections.Specialized
Imports Target.Library
Imports Target.Library.Web.Adapters
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps
Imports Target.Web.Apps.SavedWizardSelections
Imports Target.Web.Apps.SavedWizardSelections.Collections
Imports Target.Web.Apps.Security
Imports WebUtils = Target.Library.Web.Utils
Imports System.ComponentModel

Namespace Library.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Web
    ''' Class	 : Library.UserControls.SelectorWizard
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Control that displays a standard wizard.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history><![CDATA[
    '''     MikeVO      13/04/2011  SDS issue #415 - added NewEnquiryDefaultQSParams property.
    '''     MikeVO      06/10/2010  Changed "selections" section to be inside a collapsible panel.
    '''     MikeVO      18/08/2010  A4WA#6444 - corrected BackUrl creation.
    '''     MikeVO      20/07/2009  D11651 - added support for saved wizard selections.
    '''     MikeVO      22/08/2007  Ported from SPWeb.
    '''     MikeVO      20/11/2006  Added CSS class for nav buttons.
    '''     MikeVO      20/10/2006  Moved wizard setup into InitControl() to allow user of validators in 
    '''                             wizard steps that use user controls. pages using SelectorWizard should now
    '''                             create the required steps in Page_Init() event then call InitControl().
    '''     MikeVO      16/10/2006  Added NewButton, BackButton, FinishButton & ShowHeaderControls properties.
    '''     MikeVO      10/10/2006  Suuport for Required property is wizard steps.
    ''' 	[Mikevo]	05/10/2006	Created
    ''' ]]></history>
    ''' -----------------------------------------------------------------------------
    Partial Class SelectorWizard
        Inherits SelectorWizardBase

#Region " Private Variables & properties "

        Private _steps As ArrayList = New ArrayList
        Private _nameValueCollection As NameValueCollection = New NameValueCollection
        Private _ShowBackButtonOnFirststep As Boolean = False


        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets the available steps in the wizard.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	06/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property Steps() As ArrayList
            Get
                Return _steps
            End Get
            Set(ByVal Value As ArrayList)
                _steps = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Providers access to the New button.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	16/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property NewButton() As HtmlInputButton
            Get
                Return btnNewEnquiry
            End Get
            Set(ByVal Value As HtmlInputButton)
                btnNewEnquiry = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Provides access to the Back button.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	16/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property BackButton() As HtmlInputButton
            Get
                Return btnBack
            End Get
            Set(ByVal Value As HtmlInputButton)
                btnBack = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Provides access to the Finish button.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	16/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property FinishButton() As HtmlInputButton
            Get
                Return btnFinish
            End Get
            Set(ByVal Value As HtmlInputButton)
                btnFinish = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets if the ehader controls are displayed.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	16/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property ShowHeaderControls() As Boolean
            Get
                Return divSelections.Visible
            End Get
            Set(ByVal Value As Boolean)
                divSelections.Visible = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets or sets default querystring parameters that are added to the url
        '''     when the "New Enquiry" button is clicked.
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[Mikevo]	13/04/2011	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Property NewEnquiryDefaultQSParams() As NameValueCollection
            Get
                Return _nameValueCollection
            End Get
            Set(ByVal Value As NameValueCollection)
                _nameValueCollection = Value
            End Set
        End Property

        ''' <summary>
        ''' In case we have to show back button on the first step of wizard.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Property ShowBackButtonOnFirststep() As Boolean
            Get
                Return _ShowBackButtonOnFirststep
            End Get
            Set(ByVal value As Boolean)
                _ShowBackButtonOnFirststep = value
            End Set
        End Property


#End Region

#Region " Properties "
        ''' <summary>
        ''' The _button width
        ''' </summary>
        Private _nextButtonWidth As String
        ''' <summary>
        ''' Gets or sets the width of the button.
        ''' </summary>
        ''' <value>
        ''' The width of the button.
        ''' </value>
        <Browsable(True)> _
        Public Property NextButtonWidth() As String
            Get
                Return _nextButtonWidth
            End Get
            Set(ByVal value As String)
                _nextButtonWidth = value
            End Set
        End Property

        ''' <summary>
        ''' The _back button width
        ''' </summary>
        Private _backButtonWidth As String

        ''' <summary>
        ''' Gets or sets the width of the back button.
        ''' </summary>
        ''' <value>
        ''' The width of the back button.
        ''' </value>
        Public Property BackButtonWidth() As String
            Get
                Return _backButtonWidth
            End Get
            Set(ByVal value As String)
                _backButtonWidth = value
            End Set
        End Property

        ''' <summary>
        ''' The _new button width
        ''' </summary>
        Private _newButtonWidth As String
        ''' <summary>
        ''' Gets or sets the new width of the button.
        ''' </summary>
        ''' <value>
        ''' The new width of the button.
        ''' </value>
        Public Property NewButtonWidth() As String
            Get
                Return _newButtonWidth
            End Get
            Set(ByVal value As String)
                _newButtonWidth = value
            End Set
        End Property

#End Region

#Region " InitControl "

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Initialises the wizard and creates the specified steps.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     MikeVO      02/07/2009  A4WA#5562 - support for "BeforeNavigateJS" in hidden wizard steps.
        '''     MikeVO      19/05/2009  D11549 - added support for hidden end steps.
        '''     MikeVO      13/11/2006  Fix to JS reference so it will work for pages in different folders.
        ''' 	[Mikevo]	20/10/2006	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub InitControl()

            Dim currentStep As Integer = Utils.ToInt32(Request.QueryString("currentStep"))
            Dim thePage As BasePage = DirectCast(Me.Page, BasePage)
            Dim spacerBr As Literal
            Dim msg As ErrorMessage
            Dim showFinishButton As Boolean
            Dim headerControlCountBefore As Integer, headerControlCountAfter As Integer
            Dim normalStepCount As Integer
            Dim hiddenEndStepButton As HtmlInputButton
            Dim prog As SteppedProgress
            Dim currentStepBeforeNavigateJS As String = "void(0)"

            ' add page JS
            thePage.UseJQuery = True

            thePage.JsLinks.Add(WebUtils.GetVirtualPath("Library/UserControls/SelectorWizard.js"))

            ' get the count of "normal" steps
            For stepIndex As Integer = 0 To Me.Steps.Count - 1
                If Not TypeOf Me.Steps(stepIndex) Is ISelectorWizardHiddenEndStep Then normalStepCount += 1
            Next

            phHeader = Me.FindControl("cpSelections").FindControl("phHeader")

            ' setup each of the steps
            For Each s As ISelectorWizardStep In Me.Steps
                s.DbConnection = thePage.DbConnection
                s.BaseUrl = Request.Url.AbsolutePath
                s.QueryString = New NameValueCollection(Request.QueryString)
                s.StepIndex = Me.Steps.IndexOf(s)
                s.IsCurrentStep = (Me.Steps.IndexOf(s) = currentStep)
                headerControlCountBefore = phHeader.Controls.Count
                msg = s.RenderHeaderControls(phHeader.Controls)
                headerControlCountAfter = phHeader.Controls.Count
                If Not msg.Success Then WebUtils.DisplayError(msg)
                ' if its not the last step, render a spacer after the header controls for each step, if header controls were added by the step
                If s.StepIndex < normalStepCount - 1 AndAlso headerControlCountBefore <> headerControlCountAfter Then
                    spacerBr = New Literal
                    spacerBr.Text = "<br />"
                    phHeader.Controls.Add(spacerBr)
                End If
                If s.IsCurrentStep Then
                    ' for the current step
                    s.IsCurrentStep = True
                    If TypeOf s Is ISelectorWizardHiddenEndStep Then
                        lblTitle.Text = s.Title
                    Else
                        lblTitle.Text = String.Format("Step {0} of {1}: {2}", currentStep + 1, normalStepCount, s.Title)
                        ' render progress bar
                        prog = New SteppedProgress(currentStep + 1, normalStepCount)
                        phProgress.Controls.Add(prog)
                    End If
                    msg = s.RenderContentControls(Me, phContent.Controls)
                    If Not msg.Success Then WebUtils.DisplayError(msg)
                    ' setup before navigate javascript
                    btnNext.Attributes.Add("onclick", String.Format("if({0}) SelectorWizard_Navigate();", s.BeforeNavigateJS))
                    btnFinish.Attributes.Add("onclick", String.Format("if({0}) SelectorWizard_Finish();", s.BeforeNavigateJS))
                    If Not String.IsNullOrEmpty(s.BeforeNavigateJS) Then currentStepBeforeNavigateJS = s.BeforeNavigateJS
                End If
            Next

            ' setup the buttons
            ' if show back button on first step is tru and current step is first step then 
            ' javascript function call should be sent to SelectorWizard_FirstBack
            If ShowBackButtonOnFirststep And currentStep = 0 Then
                btnBack.Attributes.Add("onclick", "SelectorWizard_CustomBack();")
            Else
                btnBack.Attributes.Add("onclick", "SelectorWizard_Back();")
            End If

            ' show the Back button if it isn't already hidden and it isn't the first step
            ' also if user has not forced to display the back button.
            btnBack.Visible = (btnBack.Visible AndAlso (currentStep > 0 Or ShowBackButtonOnFirststep))
            btnNext.Visible = (currentStep < normalStepCount - 2)

            ' the finish button is visible when NOT on the last step and none of the steps after the current step are required
            If btnFinish.Visible Then
                showFinishButton = True
                For stepIndex As Integer = currentStep + 1 To normalStepCount - 1
                    If DirectCast(Me.Steps(stepIndex), ISelectorWizardStep).Required Then
                        showFinishButton = False
                        Exit For
                    End If
                Next
                If showFinishButton Then showFinishButton = (currentStep < normalStepCount - 1)
                btnFinish.Visible = showFinishButton
            End If

            ' output buttons for hidden end steps when the last "normal" wizard step is the current step
            If currentStep = normalStepCount - 1 Then
                For stepIndex As Integer = currentStep + 1 To Me.Steps.Count - 1
                    If TypeOf Me.Steps(stepIndex) Is ISelectorWizardHiddenEndStep Then
                        hiddenEndStepButton = New HtmlInputButton()
                        With hiddenEndStepButton
                            '.Attributes.Add("class", "selectorWizardNavButton")
                            .Attributes.Add("onclick", String.Format("{0}; SelectorWizard_ShowHiddenStep({1});", currentStepBeforeNavigateJS, stepIndex))
                            .Value = CType(Me.Steps(stepIndex), ISelectorWizardHiddenEndStep).ButtonText
                        End With
                        phHiddenEndStepButtons.Controls.Add(hiddenEndStepButton)
                    End If
                Next
            End If

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("SelectorWizard_currentStep={0};SelectorWizard_lastStep={1};SelectorWizard_cboSavedSelections='{2}';", _
                        currentStep, _
                        normalStepCount - 1, _
                        cboSavedSelections.ClientID) _
                ) _
            )

            ' init the current step
            For Each s As ISelectorWizardStep In Me.Steps
                If s.IsCurrentStep Then
                    s.InitStep(Me)
                    Exit For
                End If
            Next

            msg = SetupSavedWizardSelections()
            If Not msg.Success Then WebUtils.DisplayError(msg)

        End Sub

#End Region

#Region " OnPreRender "

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)

            MyBase.OnPreRender(e)

            ' construct New Enquiry default params javascript
            Dim defaultNewEnquiryParamsJS As String = WebUtils.GetNameValueCollectionAsString(Me.NewEnquiryDefaultQSParams)
            Me.Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), _
                                                           "ScriptBlock", _
                                                           String.Format("SelectorWizard_defaultNewEnquiryParamsJS='{0}';", defaultNewEnquiryParamsJS), _
                                                           True _
            )

            For Each s As ISelectorWizardStep In Me.Steps
                If s.IsCurrentStep Then
                    s.PreRender(Me)
                    Exit For
                End If
            Next

        End Sub

#End Region

#Region " AddBasePageJsLink "

        Public Overrides Sub AddBasePageJsLink(ByVal url As String)
            DirectCast(Me.Page, BasePage).JsLinks.Add(url)
        End Sub

#End Region

#Region " SetupSavedWizardSelections "

        Private Function SetupSavedWizardSelections() As ErrorMessage

            Const OPTGROUP_MY As String = "My Saved Selections"
            Const OPTGROUP_GLOBAL As String = "Global Selections"
            Const OPTGROUP_ACTIONS As String = "Actions"

            Dim msg As ErrorMessage
            Dim thePage As BasePage = DirectCast(Me.Page, BasePage)
            Dim currentUser As websecurityUser = Security.SecurityBL.GetCurrentUser()
            Dim mySelections As vwWebSavedWizardSelectionCollection = Nothing
            Dim globalSelections As vwWebSavedWizardSelectionCollection = Nothing

            If thePage.Settings.CurrentApplicationID = ApplicationName.AbacusIntranet Then
                ' get my selections
                msg = SavedWizardSelectionsBL.FetchSavedSelectionsForUI(thePage.DbConnection, _
                                                                        thePage.Settings.CurrentApplicationID, _
                                                                        thePage.MenuItemID, _
                                                                        currentUser, _
                                                                        False, _
                                                                        mySelections)
                If Not msg.Success Then Return msg
                ' get global selections
                msg = SavedWizardSelectionsBL.FetchSavedSelectionsForUI(thePage.DbConnection, _
                                                                        thePage.Settings.CurrentApplicationID, _
                                                                        thePage.MenuItemID, _
                                                                        currentUser, _
                                                                        True, _
                                                                        globalSelections)
                If Not msg.Success Then Return msg

                With cboSavedSelections.Items
                    ' add my selections
                    AddSavedSelections(OPTGROUP_MY, mySelections)
                    ' add globals
                    AddSavedSelections(OPTGROUP_GLOBAL, globalSelections)
                    ' add actions
                    If SecurityBL.UserHasMenuItemCommand(thePage.DbConnection, _
                                 currentUser.ID, _
                                 Target.Library.Web.ConstantsManager.GetConstant( _
                                    String.Format("{0}.WebNavMenuItemCommand.SavedWizardSelectionsEdit.Add", thePage.Settings.CurrentApplicationID) _
                                 ), _
                                 Convert.ToInt32(thePage.Settings.CurrentApplicationID)) Then
                        AddSavedSelectionItem("Save current selections...", _
                                              WebUtils.GetVirtualPath(String.Format("Apps/SavedWizardSelections/Edit.aspx?mi={0}&mode=2&", thePage.MenuItemID)), _
                                              OPTGROUP_ACTIONS, _
                                              True)
                    End If
                    If SecurityBL.UserHasMenuItem(thePage.DbConnection, _
                                 currentUser.ID, _
                                 Target.Library.Web.ConstantsManager.GetConstant( _
                                    String.Format("{0}.WebNavMenuItem.SavedWizardSelections", thePage.Settings.CurrentApplicationID) _
                                 ), _
                                 Convert.ToInt32(thePage.Settings.CurrentApplicationID)) Then
                        AddSavedSelectionItem( _
                            "Manage selections...", _
                            String.Format( _
                                "{0}?backURL={1}", _
                                WebUtils.GetVirtualPath("Apps/SavedWizardSelections/List.aspx"), _
                                HttpUtility.UrlEncode(Request.Url.PathAndQuery) _
                            ), _
                            OPTGROUP_ACTIONS, _
                            False _
                        )
                    End If
                End With

                With cboSavedSelections
                    If .Items.Count > 0 Then
                        pnlSavedSelections.Visible = True
                        ' add blank
                        .Items.Insert(0, String.Empty)
                        .Attributes.Add("onchange", "SelectorWizard_JumpToSavedSelection();")
                    End If
                End With

            End If

            msg = New ErrorMessage()
            msg.Success = True

            Return msg

        End Function

        Private Sub AddSavedSelections(ByVal groupName As String, _
                                       ByVal selections As vwWebSavedWizardSelectionCollection)

            Dim url As String

            With cboSavedSelections.Items
                For Each sel As vwWebSavedWizardSelection In selections
                    ' build url
                    url = sel.ScreenUrl
                    If url.Contains("?") Then url = url.Split("?")(0)
                    url = String.Format("{0}?{1}", url, sel.QueryString)
                    url = WebUtils.GetVirtualPath(url)
                    ' create item
                    AddSavedSelectionItem(sel.Name, url, groupName, False)
                Next
            End With

        End Sub

        Private Sub AddSavedSelectionItem(ByVal text As String, _
                                          ByVal value As String, _
                                          ByVal groupName As String, _
                                          ByVal appendCurrentUrl As Boolean)
            Dim newOpt As ListItem
            newOpt = New ListItem(text, value)
            With newOpt.Attributes
                .Add(DropDownListAdapter.OPTGROUP_ATTRIBUTE_NAME, groupName)
                If appendCurrentUrl Then .Add("tag", "appendUrl")
            End With
            cboSavedSelections.Items.Add(newOpt)
        End Sub

#End Region

        Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

            If Not String.IsNullOrEmpty(NextButtonWidth) Then
                btnNext.Attributes.Add("style", "width:" & NextButtonWidth & ";")
            End If
            If Not String.IsNullOrEmpty(BackButtonWidth) Then
                btnBack.Attributes.Add("style", "width:" & BackButtonWidth & ";")
            End If

            If Not String.IsNullOrEmpty(NewButtonWidth) Then
                btnNewEnquiry.Attributes.Add("style", "width:" & NewButtonWidth & ";")
            End If
        End Sub
    End Class

End Namespace

