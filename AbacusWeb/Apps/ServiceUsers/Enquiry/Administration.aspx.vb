Imports Target.Web.Apps
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.SDS
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports WebUtils = Target.Library.Web.Utils
Imports System.Collections.Generic
Imports Target.Library.Web.Controls
Imports System.Text
Imports System.Web.Script.Serialization

Namespace Apps.ServiceUsers.Enquiry
    ''' <summary>
    ''' Screen used to maintain a service user admin information.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Waqas         21/11/2012  D12420 - Added NHS Number field
    '''     MikeVO        07/09/2011  D12112 - support for summarised statement formats.
    '''     MikeVO        13/06/2011  SDS issue #633 - stop display of date picker.
    '''     MikeVO        27/04/2011  SDS issue #567 - corrected display of "Suspend invoices where actual service exceeds planned".
    '''     ColinD        29/03/2011  D12057 - Added Personal Budget Statement functionality.
    '''     ColinD        07/01/2011  Updated CreateMemoIndicators to pass client id to ServiceUserBL.GetServiceUserMemoIndicators.
    '''     ColinD        22/12/2010  Removed SDS Billing frame.
    '''     MikeVO        15/10/2010  More UI tidy up.
    '''     MikeVO        13/10/2010  More UI tidy up.
    '''     MikeVO        05/10/2010  UI tidy up.
    '''     Mo Tahir      27/08/2010  D11814 - Service User Enquiry.
    ''' </history>
    Partial Public Class Administration
        Inherits BasePage

        Private _clientID As Integer
        Private _clientDetail As ClientDetail
        Private _vwTitle As vwTitle
        Private _systemInfo As SystemInfo
        Private _statementFrequencies As LookupCollection = Nothing

        Const MEMO_CTRL_PREFIX As String = "MEMO_"
        Const MEMO_DIV_LIMIT As Integer = 10

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim msg As ErrorMessage

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.ServiceUserEnquiry"), "Service User")
            Session(AuditLogging.SESSION_KEY_MENU_ITEM_ID) = Me.MenuItemID
            If Utils.ToInt32(Request.QueryString("clientID")) > 0 Then
                _clientID = Utils.ToInt32(Request.QueryString("clientID"))
            End If

            Me.UseJQuery = True
            Me.JsLinks.Add(WebUtils.GetVirtualPath("Library/Javascript/date.js"))
            Me.JsLinks.Add("Administration.js")

            ServiceUserBL.GetServiceUser(conn:=Me.DbConnection, cDetail:=_clientDetail, itemID:=_clientID)

            _systemInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)

            ' fetch all frequencies
            msg = Lookup.FetchList(conn:=DbConnection, list:=_statementFrequencies, type:="FREQUENCY")
            If Not msg.Success Then WebUtils.DisplayError(msg)

            LoadDetails()
            LoadAdministrativeDetails()
            LoadOtherInformation()
            LoadLegacyDetails()

            ExpandPanel(cpDetail)

        End Sub

        Private Sub LoadDetails()

            Dim msg As ErrorMessage

            ' FIND
            ' personal
            txtReference = cpDetail.FindControl("txtReference")
            txtNHSNumber = cpDetail.FindControl("txtNHSNumber")
            txtForenames = cpDetail.FindControl("txtForenames")
            txtSurname = cpDetail.FindControl("txtSurname")
            txtTitle = cpDetail.FindControl("txtTitle")
            txtNiNo = cpDetail.FindControl("txtNiNo")
            txtDOB = cpDetail.FindControl("txtDOB")
            txtDOD = cpDetail.FindControl("txtDOD")
            txtGender = cpDetail.FindControl("txtGender")
            txtEthnicity = cpDetail.FindControl("txtEthnicity")
            txtEmail = cpDetail.FindControl("txtEmail")

            ' general
            txtAlternativeRef = cpDetail.FindControl("txtAlternativeRef")
            txtDebtorReference = cpDetail.FindControl("txtDebtorReference")
            txtCreditorReference = cpDetail.FindControl("txtCreditorReference")
            txtCurrentTeam = cpDetail.FindControl("txtCurrentTeam")
            txtCurrentCareManager = cpDetail.FindControl("txtCurrentCareManager")
            txtDateofLastAssessment = cpDetail.FindControl("txtDateofLastAssessment")
            txtDateofNextAssessment = cpDetail.FindControl("txtDateofNextAssessment")
            txtUserInitials = cpDetail.FindControl("txtUserInitials")
            txtFinanceCode = cpDetail.FindControl("txtFinanceCode")

            ' CONFIGURE
            ' personal
            ConfigureTextbox(txtReference)
            ConfigureTextbox(txtNHSNumber)
            ConfigureTextbox(txtForenames)
            ConfigureTextbox(txtSurname)
            ConfigureTextbox(txtTitle)
            ConfigureTextbox(txtNiNo)
            ConfigureTextbox(txtDOB)
            ConfigureTextbox(txtDOD)
            ConfigureTextbox(txtGender)
            ConfigureTextbox(txtEthnicity)
            ConfigureTextbox(txtEmail)

            ' general
            ConfigureTextbox(txtAlternativeRef)
            ConfigureTextbox(txtDebtorReference)
            ConfigureTextbox(txtCreditorReference)
            ConfigureTextbox(txtCurrentTeam)
            ConfigureTextbox(txtCurrentCareManager)
            ConfigureTextbox(txtDateofLastAssessment)
            ConfigureTextbox(txtDateofNextAssessment)
            ConfigureTextbox(txtUserInitials)
            ConfigureTextbox(txtFinanceCode)

            ' POPULATE
            ' personal
            txtReference.Text = _clientDetail.Reference
            txtNHSNumber.Text = _clientDetail.NHSNumber
            txtForenames.Text = _clientDetail.FirstNames
            txtSurname.Text = _clientDetail.LastName
            msg = ServiceUserBL.GetServiceUserTitle(Me.DbConnection, _clientDetail.Title, txtTitle.Text)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            txtNiNo.Text = _clientDetail.NINO
            If Utils.IsDate(_clientDetail.BirthDate) Then txtDOB.Text = _clientDetail.BirthDate
            If Utils.IsDate(_clientDetail.DeathDate) Then txtDOD.Text = _clientDetail.DeathDate
            msg = ServiceUserBL.GetServiceUserGenderDescription(Me.DbConnection, _clientDetail.Gender, txtGender.Text)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            msg = ServiceUserBL.GetServiceUserEthnicDescription(Me.DbConnection, _clientDetail.EthnicOriginID, txtEthnicity.Text)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            txtEmail.Text = _clientDetail.EmailAddress

            ' general
            txtAlternativeRef.Text = _clientDetail.CRISSPRef
            txtDebtorReference.Text = _clientDetail.DebtorNumber
            txtCreditorReference.Text = _clientDetail.CreditorReference
            msg = ServiceUserBL.GetServiceUserTeamName(Me.DbConnection, _clientDetail.CurrentTeamID, txtCurrentTeam.Text)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            msg = ServiceUserBL.GetServiceUserCareManagerName(Me.DbConnection, _clientDetail.CurrentCareManagerID, txtCurrentCareManager.Text)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If Utils.IsDate(_clientDetail.DateOfLastAssessment) Then txtDateofLastAssessment.Text = _clientDetail.DateOfLastAssessment
            If Utils.IsDate(_clientDetail.DateOfNextAssessment) Then txtDateofNextAssessment.Text = _clientDetail.DateOfNextAssessment
            txtUserInitials.Text = _clientDetail.UsersInitials
            txtFinanceCode.Text = _clientDetail.FinanceCode

        End Sub

        ''' <summary>
        ''' Loads the administrative details tab controls.
        ''' </summary>
        Private Sub LoadAdministrativeDetails()

            ' setup last statement produced on text box
            txtLastStatementProducedOn = cpAdministrativeDetail.FindControl("txtLastStatementProducedOn")
            txtSortCode = cpAdministrativeDetail.FindControl("txtSortCode")
            txtAccountNumber = cpAdministrativeDetail.FindControl("txtAccountNumber")
            txtPaymentRef = cpAdministrativeDetail.FindControl("txtPaymentRef")

            ConfigureTextbox(txtLastStatementProducedOn)
            ConfigureTextbox(txtSortCode)
            ConfigureTextbox(txtAccountNumber)
            ConfigureTextbox(txtPaymentRef)

            If Utils.IsDate(_clientDetail.LastPersonalBudgetStatementDate) Then txtLastStatementProducedOn.Text = _clientDetail.LastPersonalBudgetStatementDate.ToString("dd/MM/yyyy")

            If Me.UserHasMenuItemCommand(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItemCommand.ServiceUserEnquiry.ViewBankAccountDetails")) Then
                txtSortCode.Text = _clientDetail.BankSortCode
                txtAccountNumber.Text = _clientDetail.BankAccountNumber
                txtPaymentRef.Text = _clientDetail.BankPaymentRef
            Else
                txtSortCode.Text = New String("*", _clientDetail.BankSortCode.Length)
                txtAccountNumber.Text = New String("*", _clientDetail.BankAccountNumber.Length)
                txtPaymentRef.Text = New String("*", _clientDetail.BankPaymentRef.Length)
                If _clientDetail.BankSortCode.Length > 0 Or _clientDetail.BankAccountNumber.Length > 0 Or _clientDetail.BankPaymentRef.Length > 0 Then
                    txtBankAccountWarning.Text = "You do not have permission to view bank account details, and these have been hidden."
                End If
            End If


            ' setup drop down list
            PopulatePersonalBudgetStatementDropdownLists()

        End Sub

        Private Sub LoadOtherInformation()

            Dim msg As ErrorMessage

            txtDSSOffice = cpOtherInfo.FindControl("txtDSSOffice")
            txtCurrentCaseID = cpOtherInfo.FindControl("txtCurrentCaseID")
            txtMPSNumber = cpOtherInfo.FindControl("txtMPSNumber")
            chkParentalContributionRequired = cpOtherInfo.FindControl("chkParentalContributionRequired")
            txtBillingType = cpOtherInfo.FindControl("txtBillingType")
            txtRiskLevel = cpOtherInfo.FindControl("txtRiskLevel")

            ConfigureTextbox(txtDSSOffice)
            ConfigureTextbox(txtCurrentCaseID)
            ConfigureTextbox(txtMPSNumber)
            ConfigureCheckbox(chkParentalContributionRequired)
            ConfigureTextbox(txtBillingType)
            ConfigureTextbox(txtRiskLevel)

            msg = ServiceUserBL.GetDSSOfficeName(Me.DbConnection, _clientDetail.DSSOfficeID, txtDSSOffice.Text)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            txtCurrentCaseID.Text = _clientDetail.CurrentCaseID
            txtMPSNumber.Text = _clientDetail.MPSNumber
            chkParentalContributionRequired.CheckBox.Checked = _clientDetail.ParentalContribution
            If _clientDetail.ClientBillingTypeID > 0 Then txtBillingType.Text = _clientDetail.ClientBillingTypeID
            txtRiskLevel.Text = _clientDetail.RiskLevel

            CreateMemoIndicators()
        End Sub

        ''' <summary>
        ''' Loads the legacy details.
        ''' </summary>
        Private Sub LoadLegacyDetails()

            Dim suspensionSetting As Boolean

            ' res billing
            txtResInvoicedUpTo = cpLegacy.FindControl("txtResInvoicedUpTo")
            chkResSuppressInvoicing = cpLegacy.FindControl("chkResSuppressInvoicing")
            chkResSuppressInvoicePrinting = cpLegacy.FindControl("chkResSuppressInvoicePrinting")
            chkResPayByDD = cpLegacy.FindControl("chkResPayByDD")
            chkSuppressFromDebtorsInterface = cpLegacy.FindControl("chkSuppressFromDebtorsInterface")

            ' non-res billing
            txtDomInvoicedUpTo = cpLegacy.FindControl("txtDomInvoicedUpTo")
            chkDomSuppressInvoicing = cpLegacy.FindControl("chkDomSuppressInvoicing")
            chkDomSuppressInvoicePrinting = cpLegacy.FindControl("chkDomSuppressInvoicePrinting")
            chkDomPayByDD = cpLegacy.FindControl("chkDomPayByDD")
            txtSwipeCardNo = cpLegacy.FindControl("txtSwipeCardNo")
            txtCardProducedDate = cpLegacy.FindControl("txtCardProducedDate")

            chkSuppressStatements = cpLegacy.FindControl("chkSuppressStatements")
            chkSuspensionSetting = cpLegacy.FindControl("chkSuspensionSetting")

            ' CONFIGURE
            ' res billing
            ConfigureTextbox(txtResInvoicedUpTo)
            ConfigureCheckbox(chkResSuppressInvoicing)
            ConfigureCheckbox(chkResSuppressInvoicePrinting)
            ConfigureCheckbox(chkResPayByDD)
            ConfigureCheckbox(chkSuppressFromDebtorsInterface)

            ' non-res billing
            ConfigureTextbox(txtDomInvoicedUpTo)
            ConfigureCheckbox(chkDomSuppressInvoicing)
            ConfigureCheckbox(chkDomSuppressInvoicePrinting)
            ConfigureCheckbox(chkDomPayByDD)
            ConfigureTextbox(txtSwipeCardNo)
            ConfigureTextbox(txtCardProducedDate)

            ConfigureCheckbox(chkSuspensionSetting)
            ConfigureCheckbox(chkSuppressStatements)

            ' POPULATE

            ' res billing
            If Utils.IsDate(_clientDetail.ResInvoicedTo) Then txtResInvoicedUpTo.Text = _clientDetail.ResInvoicedTo
            chkResSuppressInvoicing.CheckBox.Checked = _clientDetail.ResInvoiceSuppress
            chkResSuppressInvoicePrinting.CheckBox.Checked = _clientDetail.ResInvoiceDontPrint
            chkResPayByDD.CheckBox.Checked = _clientDetail.ResPayByDD
            chkSuppressFromDebtorsInterface.CheckBox.Checked = _clientDetail.ExcludeFromDebtorsRes

            ' non-res billing
            If Utils.IsDate(_clientDetail.DomInvoicedTo) Then txtDomInvoicedUpTo.Text = _clientDetail.DomInvoicedTo
            chkDomSuppressInvoicing.CheckBox.Checked = _clientDetail.DomInvoiceSuppress
            chkDomSuppressInvoicePrinting.CheckBox.Checked = _clientDetail.DomInvoiceDontPrint
            chkDomPayByDD.CheckBox.Checked = _clientDetail.DomPayByDD
            txtSwipeCardNo.Text = _clientDetail.SwipeCardNo
            If Utils.IsDate(_clientDetail.LastSwipeCardManu) Then txtCardProducedDate.Text = _clientDetail.LastSwipeCardManu

            chkSuppressStatements.CheckBox.Checked = _clientDetail.SuppressStatements

            ' if the client indicator is set we want to reverse the system setting
            suspensionSetting = _systemInfo.DPIAutoSuspendInvoices
            If _clientDetail.NegateDefaultDPISuspension Then
                suspensionSetting = Not suspensionSetting
            End If
            chkSuspensionSetting.CheckBox.Checked = suspensionSetting

        End Sub

        Private Sub CreateMemoIndicators()

            Dim memIndicators As List(Of ViewableMemoIndicator) = Nothing
            Dim msg As ErrorMessage = Nothing
            Dim counter As Integer = 1
            Dim lbl As Label
            Dim cbk As CheckBoxEx
            Dim txt As TextBoxEx
            Dim controls As ControlCollection

            msg = ServiceUserBL.GetServiceUserMemoIndicators(Me.DbConnection, _clientID, memIndicators)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            For Each viewableIndicator As ViewableMemoIndicator In memIndicators

                If counter <= MEMO_DIV_LIMIT Then
                    controls = phMemoIndicators1.Controls
                ElseIf counter <= (MEMO_DIV_LIMIT * 2) Then
                    controls = phMemoIndicators2.Controls
                Else
                    controls = phMemoIndicators3.Controls
                End If

                If viewableIndicator.Infostring = "Y" Then
                    cbk = New CheckBoxEx()
                    With cbk
                        .ID = MEMO_CTRL_PREFIX & viewableIndicator.ID
                        .CheckBox.Checked = IIf(viewableIndicator.MemoValue = "N", False, True)
                        .Text = viewableIndicator.Description
                    End With
                    controls.Add(cbk)
                    ConfigureCheckbox(cbk)
                    AddBr(controls)
                    AddBr(controls)
                Else
                    txt = New TextBoxEx()
                    With txt
                        .ID = MEMO_CTRL_PREFIX & viewableIndicator.ID
                        .Text = viewableIndicator.MemoValue
                        .MaxLength = 1
                        .Width = New Unit(1, UnitType.Em)
                        .OutputBrAfter = False
                        ConfigureTextbox(txt)
                    End With
                    lbl = New Label()
                    With lbl
                        .Text = viewableIndicator.Description
                        .AssociatedControlID = txt.ID
                        .CssClass = "checkBoxLabel"
                    End With
                    controls.Add(txt)
                    controls.Add(lbl)
                    AddBr(controls)
                    AddBr(controls)
                End If
                counter += 1
            Next

        End Sub

        Private Sub AddBr(ByVal controls As ControlCollection)
            Dim literal1 As Literal = New Literal
            literal1.Text = "<br>"
            controls.Add(literal1)
        End Sub

        Private Sub ConfigureCheckbox(ByVal chk As CheckBoxEx)
            chk.CheckBox.TextAlign = TextAlign.Right
            'chk.Label.Style.Add("margin-top", "0.25em")
            chk.CheckBox.Attributes.Add("onclick", "return false;")
        End Sub

        Private Sub ConfigureTextbox(ByVal txt As TextBoxEx)
            txt.TextBox.ReadOnly = True
            If txt.Format = TextBoxEx.TextBoxExFormat.DateFormat Then
                txt.DatePickerButton.Visible = False
                txt.DatePicker.Enabled = False
            End If
        End Sub

        Private Sub ExpandPanel(ByVal panel As CollapsiblePanel)
            panel.Expanded = True
        End Sub

        ''' <summary>
        ''' Populates the personal budget statement drop down lists.
        ''' </summary>
        Private Sub PopulatePersonalBudgetStatementDropdownLists()
            PopulatePersonalBudgetStatementFrequencies()
            PopulatePersonalBudgetStatementLayouts()
        End Sub

        ''' <summary>
        ''' Populates the personal budget statement frequencies drop down list.
        ''' </summary>
        Private Sub PopulatePersonalBudgetStatementFrequencies()

            Dim msg As New ErrorMessage()
            Dim currentStatementFrequency As ListItem = Nothing
            Dim defaultStatementFrequency As Lookup = Nothing
            Dim defaultStatementFrequencyID As Integer = 0

            Const DropDownListOptionGroupAttribute As String = "OptionGroup"
            Const DropDownListOverrideGroup As String = "Override"

            ' setup frequencies drop down list
            cboStatementFrequency = cpAdministrativeDetail.FindControl("cboStatementFrequency")
            With cboStatementFrequency.DropDownList
                .Items.Clear()
                .EnableViewState = False
            End With

            ' get default frequency id
            msg = PersonalBudgetStatementBL.GetDefaultFrequency(Me.DbConnection, Nothing, defaultStatementFrequencyID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' get the default frequency from the list using item id stored in system settings
            defaultStatementFrequency = (From tmpStatementFrequency In _statementFrequencies.ToArray() _
                                            Where tmpStatementFrequency.ID = defaultStatementFrequencyID _
                                                Select tmpStatementFrequency).FirstOrDefault()

            If Not defaultStatementFrequency Is Nothing Then
                ' if we have a default item...

                currentStatementFrequency = New ListItem( _
                    String.Format("DEFAULT ({0})", defaultStatementFrequency.Description), _
                    defaultStatementFrequency.ID _
                )
                currentStatementFrequency.Attributes.Add(DropDownListOptionGroupAttribute, "Default")
                cboStatementFrequency.DropDownList.Items.Add(currentStatementFrequency)

                If _clientDetail.PersonalBudgetStatementFrequencyID = defaultStatementFrequencyID _
                    OrElse _clientDetail.PersonalBudgetStatementFrequencyID = 0 Then

                    currentStatementFrequency.Selected = True

                End If

            End If

            For Each statementFrequency As Lookup In _statementFrequencies
                ' loop each item and add to drop down list

                If statementFrequency.Redundant = TriState.False _
                    AndAlso statementFrequency.ID <> defaultStatementFrequencyID Then
                    ' if the item isnt redundant and its not the default frequency

                    currentStatementFrequency = New ListItem(statementFrequency.Description, statementFrequency.ID)
                    currentStatementFrequency.Attributes.Add(DropDownListOptionGroupAttribute, DropDownListOverrideGroup)
                    cboStatementFrequency.DropDownList.Items.Add(currentStatementFrequency)

                    If statementFrequency.ID = _clientDetail.PersonalBudgetStatementFrequencyID Then
                        ' select the item if required

                        currentStatementFrequency.Selected = True

                    End If

                End If

            Next

            If _clientDetail.PersonalBudgetStatementFrequencyID > 0 Then
                ' if the frequency is selected on the client record we need to check it is def listed 

                ' find the frequency in the list using the client frequency id
                currentStatementFrequency = cboStatementFrequency.DropDownList.Items.FindByValue(_clientDetail.PersonalBudgetStatementFrequencyID)

                If currentStatementFrequency Is Nothing Then
                    ' if we havent found the selected item in the list then add it in at the end

                    defaultStatementFrequency = (From tmpStatementFrequency In _statementFrequencies.ToArray() _
                                                    Where tmpStatementFrequency.ID = _clientDetail.PersonalBudgetStatementFrequencyID _
                                                        Select tmpStatementFrequency).FirstOrDefault()

                    If Not defaultStatementFrequency Is Nothing Then

                        currentStatementFrequency = New ListItem(defaultStatementFrequency.Description, defaultStatementFrequency.ID)
                        With currentStatementFrequency
                            .Attributes.Add(DropDownListOptionGroupAttribute, DropDownListOverrideGroup)
                            .Selected = True
                        End With
                        cboStatementFrequency.DropDownList.Items.Add(currentStatementFrequency)

                    End If

                End If

            End If

            If IsPostBack Then
                ' if we have posted back then select the postback value

                cboStatementFrequency.SelectPostBackValue()

            End If

        End Sub

        ''' <summary>
        ''' Populates the personal budget statement layouts drop down list.
        ''' </summary>
        Private Sub PopulatePersonalBudgetStatementLayouts()

            Dim msg As New ErrorMessage()
            Dim applicationSettingLayouts As New Target.Library.Collections.ApplicationSettingCollection()
            Dim currentStatementLayout As ListItem = Nothing
            Dim defaultStatementLayout As PersonalBudgetStatementBL.Formats

            Const DropDownListOptionGroupAttribute As String = "OptionGroup"
            Const DropDownListOverrideGroup As String = "Override"

            ' setup frequencies drop down list
            cboStatementLayout = cpAdministrativeDetail.FindControl("cboStatementLayout")
            With cboStatementLayout.DropDownList
                .Items.Clear()
                .EnableViewState = False
            End With

            ' fetch settings to determine default layout id
            msg = PersonalBudgetStatementBL.GetDefaultFormat(Me.DbConnection, Nothing, defaultStatementLayout)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' get the default layout from the list using item id stored in system settings
            currentStatementLayout = New ListItem( _
                String.Format("DEFAULT ({0})", Utils.SplitOnCapitals(defaultStatementLayout.ToString())), _
                Convert.ToInt32(defaultStatementLayout) _
            )
            currentStatementLayout.Attributes.Add(DropDownListOptionGroupAttribute, "Default")
            cboStatementLayout.DropDownList.Items.Add(currentStatementLayout)

            If _clientDetail.PersonalBudgetStatementFormat = defaultStatementLayout Then

                currentStatementLayout.Selected = True

            End If

            For Each layout As String In [Enum].GetNames(GetType(PersonalBudgetStatementBL.Formats))
                ' loop each item and add to drop down list

                Dim tmpLayout As PersonalBudgetStatementBL.Formats = [Enum].Parse(GetType(PersonalBudgetStatementBL.Formats), layout)

                If tmpLayout <> defaultStatementLayout Then
                    ' if the item isnt the default layout

                    currentStatementLayout = New ListItem(Utils.SplitOnCapitals(tmpLayout.ToString()), Convert.ToInt32(tmpLayout))
                    currentStatementLayout.Attributes.Add(DropDownListOptionGroupAttribute, DropDownListOverrideGroup)
                    cboStatementLayout.DropDownList.Items.Add(currentStatementLayout)

                    If Convert.ToInt32(tmpLayout) = _clientDetail.PersonalBudgetStatementFormat Then
                        ' select the item if required

                        currentStatementLayout.Selected = True

                    End If

                End If

            Next

            If IsPostBack Then
                ' if we have posted back then select the postback value

                cboStatementLayout.SelectPostBackValue()

            End If

        End Sub

        ''' <summary>
        ''' Handles the click event of the button btnUpdatePersonalBudgetStatements
        ''' </summary>
        ''' <param name="sender">The sender.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub btnUpdatePersonalBudgetStatements_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdatePersonalBudgetStatements.Click

            Dim msg As New ErrorMessage()
            Dim defaultStatementLayout As PersonalBudgetStatementBL.Formats
            Dim defaultStatementFrequency As Integer

            cboStatementFrequency = cpAdministrativeDetail.FindControl("cboStatementFrequency")
            cboStatementLayout = cpAdministrativeDetail.FindControl("cboStatementLayout")

            ' get default frequency
            msg = PersonalBudgetStatementBL.GetDefaultFrequency(Me.DbConnection, Nothing, defaultStatementFrequency)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' get default layout
            msg = PersonalBudgetStatementBL.GetDefaultFormat(Me.DbConnection, Nothing, defaultStatementLayout)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' save the selected frequency and layout to the client record
            If Utils.ToInt32(cboStatementFrequency.Value) = defaultStatementFrequency Then
                _clientDetail.PersonalBudgetStatementFrequencyID = Nothing
            Else
                _clientDetail.PersonalBudgetStatementFrequencyID = Utils.ToInt32(cboStatementFrequency.Value)
            End If

            If Utils.ToInt32(cboStatementLayout.Value) = Convert.ToInt32(defaultStatementLayout) Then
                _clientDetail.PersonalBudgetStatementFormat = Nothing
            Else
                _clientDetail.PersonalBudgetStatementFormat = Utils.ToInt32(cboStatementLayout.Value)
            End If

            msg = _clientDetail.Save(False)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            ' repopulate the dropdown lists
            PopulatePersonalBudgetStatementDropdownLists()

        End Sub

        Private Sub Page_PreRenderComplete(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRenderComplete

            Dim js As StringBuilder = New StringBuilder()
            Dim nextStatementStartFromDate As Nullable(Of Date)
            Dim reducedFrequencies As List(Of ReducedFrequency)
            Dim jsSerializer As JavaScriptSerializer = New JavaScriptSerializer()
            Dim jsonFrequencies As String = "[]"

            ' LAST STATEMENT DATE
            ' prime from last statement date
            nextStatementStartFromDate = _clientDetail.LastPersonalBudgetStatementDate
            ' then start of SDS
            If Not Utils.IsDate(nextStatementStartFromDate) Then nextStatementStartFromDate = _clientDetail.SDSFrom
            ' otherwise nothing
            If Not Utils.IsDate(nextStatementStartFromDate) Then nextStatementStartFromDate = Nothing
            If nextStatementStartFromDate.HasValue Then
                js.AppendFormat("nextStatementStartFromDate={0};", WebUtils.GetDateAsJavascriptString(nextStatementStartFromDate))
            End If


            ' STATEMENT FREQUENCIES
            With jsSerializer
                If Not _statementFrequencies Is Nothing Then
                    reducedFrequencies = (From f As Lookup In _statementFrequencies _
                                         Select New ReducedFrequency(f.ID, f.InfoString)).ToList()
                    jsonFrequencies = .Serialize(reducedFrequencies.ToArray())
                End If
            End With
            js.AppendFormat("statementFrequencies={0};", jsonFrequencies)


            js.AppendFormat("cboStatementFrequencyID='{0}';", cboStatementFrequency.DropDownList.ClientID)


            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", js.ToString(), True)

        End Sub

#Region " ReducedFrequency class "

        Private Class ReducedFrequency

            Private _id As Integer
            Private _infoString As String

            Public Property ID() As Integer
                Get
                    Return _id
                End Get
                Set(ByVal value As Integer)
                    _id = value
                End Set
            End Property

            Public Property InfoString() As String
                Get
                    Return _infoString
                End Get
                Set(ByVal value As String)
                    _infoString = value
                End Set
            End Property

            Sub New(ByVal id As Integer, ByVal infoString As String)
                Me.ID = id
                Me.InfoString = infoString
            End Sub

        End Class

#End Region

    End Class

End Namespace
