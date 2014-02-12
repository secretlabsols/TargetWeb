Imports System.Text
Imports System.Configuration.ConfigurationSettings
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.Controls
Imports System.Data.SqlClient
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps.Security
Imports AjaxControlToolkit

Namespace Apps.Res

    ''' <summary>
    ''' Screen to allow a user to view the details of a service user.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      28/04/2011  SDS issue #613 - correct issue with collapsible panel.
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ViewServiceUser
        Inherits Target.Web.Apps.BasePage

        Private Assessments As Collection
        Private CareCostElements As Collection

#Region " Page Load "

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Me.InitPage(ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ViewServiceUser"), "View Service User")
            Me.UseJQuery = True

            Dim serviceUserID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim user As WebSecurityUser = SecurityBL.GetCurrentUser()
            Dim strStyle As New StringBuilder

            Me.JsLinks.Add("TargetWeb\Library\JavaScript\Utils.js")
            Me.JsLinks.Add("ViewServiceUser.js")

            strStyle.Append("label.label { float:left; width:9.5em; font-weight:bold; }")
            strStyle.Append("span.label { float:left; width:9.5em; padding-right:1em; font-weight:bold; }")
            strStyle.Append("fieldset.resSU { padding-top:0em; margin-bottom:1em; }")
            strStyle.Append("fieldset.resSU legend { font-size:1.1em; font-weight:bold; }")
            Me.AddExtraCssStyle(strStyle.ToString)

            '*** The followig 2 lines have been comented out because currently we are not doing amendment requests
            'PopulateTitleCombo()
            'PopulateGendersCombo()

            PopulateBasicDetails(serviceUserID)

            CreateResCareCollections(serviceUserID)

            For Each item As Assessment In Assessments

                CreateAssessmentDetail(divResCare, item)

            Next

        End Sub

#End Region

#Region " PopulateTitleCombo "

        Private Sub PopulateTitleCombo()
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage

            Const SP_NAME_FETCH_TITLES As String = "pr_FetchTitles"
            ' grab the list of titles
            Try
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_TITLES)

                With cboTitle.DropDownList
                    .DataSource = reader
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    .Items.Insert(0, New ListItem("", ""))
                End With

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_TITLES, "ViewServiceUser.PopulateTitleCombo")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub

#End Region

#Region " PopulateGendersCombo "

        Private Sub PopulateGendersCombo()
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage

            Const SP_NAME_FETCH_GENDERS As String = "pr_FetchGenders"
            ' grab the list of titles
            Try
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_GENDERS)

                With cboGender.DropDownList
                    .DataSource = reader
                    .DataTextField = "Description"
                    .DataValueField = "ID"
                    .DataBind()
                    .Items.Insert(0, New ListItem("", ""))
                End With

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_GENDERS, "ViewServiceUser.PopulateGendersCombo")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub

#End Region

#Region " PopulateBasicDetails "

        Private Sub PopulateBasicDetails(ByVal ServiceUserID As Integer)
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage

            Const SP_NAME_FETCH_SERVICEUSER As String = "spxServiceUser_Fetch"
            ' grab the list of titles
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_SERVICEUSER, False)
                spParams(0).Value = SecurityBL.GetCurrentUser().ExternalUserID
                spParams(1).Value = ServiceUserID
                spParams(2).Value = False
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_SERVICEUSER, spParams)

                While reader.Read
                    'Reference
                    lblReference.Text = WebUtils.EncodeOutput(reader("Reference"))
                    'Title
                    If Not Convert.IsDBNull(reader("Title")) Then cboTitle.Text = reader("Title") 'cboTitle.DropDownList.SelectedValue = reader("TitleID")
                    cboTitle.RecordID = ServiceUserID
                    'Gender
                    If Not Convert.IsDBNull(reader("Gender")) Then cboGender.Text = reader("Gender") 'cboGender.DropDownList.SelectedValue = reader("GenderID")
                    cboGender.RecordID = ServiceUserID
                    cboGender.Visible = True
                    'Firstnames
                    txtFirstName.Text = WebUtils.EncodeOutput(reader("FirstNames"), txtFirstName.IsReadOnly)
                    txtFirstName.RecordID = ServiceUserID
                    'Surname
                    txtSurname.Text = WebUtils.EncodeOutput(reader("LastName"), txtSurname.IsReadOnly)
                    txtSurname.RecordID = ServiceUserID

                    ' reset page title
                    Me.PageTitle = String.Format("{0} - {1} {2} ({3})", Me.PageTitle, txtSurname.Text, txtFirstName.Text, lblReference.Text)

                    'Date of Birth
                    If Not IsDBNull(reader("BirthDate")) Then
                        txtDateOfBirth.Text = WebUtils.EncodeOutput(Convert.ToDateTime(reader("BirthDate")).ToShortDateString(), txtDateOfBirth.IsReadOnly)
                        txtDateOfBirth.RecordID = ServiceUserID
                    End If
                    If Not IsDBNull(reader("DeathDate")) Then
                        'Date of death
                        txtDateOfDeath.Text = WebUtils.EncodeOutput(Convert.ToDateTime(reader("DeathDate")).ToShortDateString(), txtDateOfDeath.IsReadOnly)
                        txtDateOfDeath.RecordID = ServiceUserID
                    End If
                    'National Ins No
                    txtNINo.Text = WebUtils.EncodeOutput(reader("NINO"), txtNINo.IsReadOnly)
                    txtNINo.RecordID = ServiceUserID

                    'Care Manager
                    'Name
                    txtCareManagerName.Text = WebUtils.EncodeOutput(reader("Name"), txtCareManagerName.IsReadOnly)
                    txtCareManagerName.RecordID = ServiceUserID
                    'Phone No
                    txtCareManagerPhone.Text = WebUtils.EncodeOutput(reader("Phone"), txtCareManagerPhone.IsReadOnly)
                    txtCareManagerPhone.RecordID = ServiceUserID
                    'Fax No
                    txtCareManagerFax.Text = WebUtils.EncodeOutput(reader("Fax"), txtCareManagerFax.IsReadOnly)
                    txtCareManagerFax.RecordID = ServiceUserID
                End While

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_SERVICEUSER, "ViewServiceUser.PopulateDasicDetails")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub

#End Region

#Region " CreateResCareCollections "

        Private Sub CreateResCareCollections(ByVal ServiceUserID As Integer)
            Dim reader As SqlDataReader = Nothing
            Dim msg As ErrorMessage

            Const SP_NAME_FETCH_SURESCARE As String = "spxServiceUserResCare_Fetch"
            ' grab the list of titles
            Try
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, SP_NAME_FETCH_SURESCARE, False)
                spParams(0).Value = SecurityBL.GetCurrentUser().ExternalUserID
                spParams(1).Value = ServiceUserID
                reader = SqlHelper.ExecuteReader(Me.DbConnection, CommandType.StoredProcedure, SP_NAME_FETCH_SURESCARE, spParams)

                Assessments = New Collection
                CareCostElements = New Collection
                While reader.Read
                    Dim newItem As New Assessment
                    With newItem
                        If Not Convert.IsDBNull(reader("DateFrom")) Then .DateFrom = reader("DateFrom")
                        If Not Convert.IsDBNull(reader("DateTo")) Then .DateTo = reader("DateTo")
                        If Not Convert.IsDBNull(reader("Home")) Then .Home = reader("Home")
                        If Not Convert.IsDBNull(reader("prvPaymentTo")) Then .PaidTo = reader("prvPaymentTo")
                        If Not Convert.IsDBNull(reader("prvPaidGross")) Then .PaidGross = reader("prvPaidGross")
                        If Not Convert.IsDBNull(reader("prvPaymentSuppress")) Then .PaymentSuppress = reader("prvPaymentSuppress")
                        If Not Convert.IsDBNull(reader("HomeCost")) Then .Homecost = reader("HomeCost")
                        If Not Convert.IsDBNull(reader("Assessment")) Then .Assessment = reader("Assessment")
                        If Not Convert.IsDBNull(reader("AssessmentID")) Then .AssessmentID = reader("AssessmentID")
                        If Not Convert.IsDBNull(reader("PaymentSuppressReason")) Then .PaymentSuppressReason = reader("PaymentSuppressReason")
                        If Not Convert.IsDBNull(reader("prvPaymentDays")) Then .PaymentDays = reader("prvPaymentDays")
                        If Not Convert.IsDBNull(reader("PrvLastPaymentDays")) Then .LastPaymentDays = reader("PrvLastPaymentDays")
                        If Not Convert.IsDBNull(reader("prvPayment")) Then .Payment = reader("prvPayment")
                        If Not Convert.IsDBNull(reader("prvLastPayment")) Then .LastPayment = reader("prvLastPayment")
                        If Not Convert.IsDBNull(reader("LastClientIncomeRate")) Then .LastClientIncomeRate = reader("LastClientIncomeRate")
                    End With
                    Assessments.Add(newItem)
                End While

                reader.NextResult()

                While reader.Read
                    Dim newItem As New CareCostElement
                    With newItem
                        If Not Convert.IsDBNull(reader("Amount")) Then .Amount = reader("Amount")
                        If Not Convert.IsDBNull(reader("AssessmentID")) Then .AssessmentID = reader("AssessmentID")
                        If Not Convert.IsDBNull(reader("Description")) Then .Description = reader("Description")
                        If Not Convert.IsDBNull(reader("InHomeCost")) Then .InHomeCost = reader("InHomeCost")
                        If Not Convert.IsDBNull(reader("PayOnBehalf")) Then .PayOnBehalf = reader("PayOnBehalf")
                        If Not Convert.IsDBNull(reader("PCTPayOnBehalf")) Then .PCTPayOnBehalf = reader("PCTPayOnBehalf")
                        If Not Convert.IsDBNull(reader("PrimaryCareTrustID")) Then .PCTFunded = True
                        If Not Convert.IsDBNull(reader("LastRate")) Then .LastRate = reader("LastRate")
                        If Not Convert.IsDBNull(reader("PaidDays")) Then .PaidDays = reader("PaidDays")
                        If Not Convert.IsDBNull(reader("LastPaidDays")) Then .LastPaidDays = reader("LastPaidDays")
                    End With
                    CareCostElements.Add(newItem)
                End While

            Catch ex As Exception
                msg = Target.Library.Utils.CatchError(ex, "E0501", SP_NAME_FETCH_SURESCARE, "ViewServiceUser.CreateResCareCollection")   ' could not read data
                Target.Library.Web.Utils.DisplayError(msg)
            Finally
                If Not reader Is Nothing Then reader.Close()
            End Try
        End Sub

#End Region

#Region " CreateAssessmentDetail "

        Private Sub CreateAssessmentDetail(ByVal tabControl As Control, ByVal Assessment As Assessment)

            Dim newPanel As New Target.Library.Web.Controls.CollapsiblePanel

            newPanel.HeaderLinkText = String.Format("{0} to {1} - {2}", Assessment.DateFrom.ToString("dd/MM/yyyy"), DataUtils.DisplayEndDate(Assessment.DateTo), Assessment.Home)
            newPanel.ID = "Assessment" & Assessment.AssessmentID
            tabControl.Controls.Add(newPanel)
            newPanel.EnsureChildControls()

            CreateAssessmentDetailTab(newPanel.ContentPanel, Assessment)

        End Sub

#End Region

#Region " CreateAssessmentDetailTab "

        Private Sub CreateAssessmentDetailTab(ByVal panel As Panel, ByVal Assessment As Assessment)
            Dim tabControl As New TabContainer
            Dim newFieldset As New HtmlGenericControl
            Dim legend As New HtmlGenericControl
            Dim spacerBr As Literal

            Dim paymentInfoTab As New TabPanel
            Dim weeklyFiguresTab As New TabPanel
            Dim figuresToDateTab As New TabPanel

            paymentInfoTab.HeaderText = "Payment Information"
            weeklyFiguresTab.HeaderText = "Weekly Figures"
            figuresToDateTab.HeaderText = "Figures To Date"

            With tabControl
                .ID = String.Format("tab{0}", Assessment.AssessmentID)
                .Tabs.Add(paymentInfoTab)
                .Tabs.Add(weeklyFiguresTab)
                .Tabs.Add(figuresToDateTab)
            End With
            panel.Controls.Add(tabControl)

            'Tab 1 - Payment Information
            PopulatePaymentInformationTab(paymentInfoTab, Assessment)

            'Tab 2 - Weekly Figures
            newFieldset = New HtmlGenericControl("fieldset")
            newFieldset.Attributes.Add("class", "resSU")
            legend = New HtmlGenericControl("legend")
            legend.InnerText = "Cost of Care"
            newFieldset.Controls.Add(legend)
            weeklyFiguresTab.Controls.Add(newFieldset)
            CreateCostOfCare(newFieldset, Assessment)

            newFieldset = New HtmlGenericControl("fieldset")
            newFieldset.Attributes.Add("class", "resSU")
            legend = New HtmlGenericControl("legend")
            legend.InnerText = "Income You Collect"
            newFieldset.Controls.Add(legend)
            weeklyFiguresTab.Controls.Add(newFieldset)
            CreateIncomeYouCollect(newFieldset, Assessment)

            newFieldset = New HtmlGenericControl("fieldset")
            newFieldset.Attributes.Add("class", "resSU")
            legend = New HtmlGenericControl("legend")
            legend.InnerText = "Council Payment"
            newFieldset.Controls.Add(legend)
            weeklyFiguresTab.Controls.Add(newFieldset)
            CreateCouncilPayment(newFieldset, Assessment)

            'Tab 3 - Figures To Date
            AddTextBox(figuresToDateTab, "Qualifying Period", "txtQualifyingDays", _
                PrintWeeksDays(Assessment.PaymentDays, "{0} Weeks, {1} Days"), _
                "11em")
            spacerBr = New Literal
            spacerBr.Text = "<br />"
            figuresToDateTab.Controls.Add(spacerBr)

            newFieldset = New HtmlGenericControl("fieldset")
            newFieldset.Attributes.Add("class", "resSU")
            legend = New HtmlGenericControl("legend")
            legend.InnerText = "Income You Have Collected"
            newFieldset.Controls.Add(legend)
            figuresToDateTab.Controls.Add(newFieldset)
            CreateIncomeYouHaveCollected(newFieldset, Assessment)

            newFieldset = New HtmlGenericControl("fieldset")
            newFieldset.Attributes.Add("class", "resSU")
            legend = New HtmlGenericControl("legend")
            legend.InnerText = "Payments You Have Received"
            newFieldset.Controls.Add(legend)
            figuresToDateTab.Controls.Add(newFieldset)
            CreatePaymentsYouHaveReceived(newFieldset, Assessment)

        End Sub

#End Region

#Region " PopulatePaymentInformationTab "

        Private Sub PopulatePaymentInformationTab(ByVal PaymentInformation As Control, ByVal Assessment As Assessment)

            'Start Date
            AddTextBox(PaymentInformation, "Start Date", "txtStartDate", Assessment.DateFrom.ToString("dd/MM/yyyy"), "14em")
            'Date Paid Up To
            AddTextBox(PaymentInformation, "Date Paid Up To", "txtDatePaidUpTo", IIf(Target.Library.Utils.IsDate(Assessment.PaidTo), Assessment.PaidTo.ToString("dd/MM/yyyy"), String.Empty), "14em")
            'Weeks/Days Paid
            AddTextBox(PaymentInformation, "Weeks/Days Paid", "txtWeeksDaysPaid", _
                PrintWeeksDays(Assessment.PaymentDays, "{0} Weeks, {1} Days"), _
                "14em")
            'Gross/Net
            AddTextBox(PaymentInformation, "Gross/Net", "txtGrossNet", IIf(Assessment.PaidGross, "Gross", "Net"), "14em")
            'Payment Suppressed
            AddTextBox(PaymentInformation, "Payment Suppressed?", "txtPaymentSuppress", IIf(Assessment.PaymentSuppress, "Yes", "No"), "14em")
            'Suppression Reason
            AddTextBox(PaymentInformation, "Suppression Reason", "txtSuppressionReason", Assessment.PaymentSuppressReason, "14em")

        End Sub

#End Region

#Region " AddTextBox "

        Private Sub AddTextBox(ByVal ParentControl As Control, ByVal caption As String, ByVal ControlName As String, ByVal Value As String, Optional ByVal labelWidth As String = "9.5em")
            Dim textBox As New Target.Library.Web.Controls.TextBoxEx
            Dim spacerBr As Literal
            With textBox
                .ID = ControlName
                .LabelText = caption
                .LabelWidth = labelWidth
                .LabelBold = True
                .ReadOnlyContentCssClass = "content"
                .IsReadOnly = True
                .Text = Value
            End With
            ParentControl.Controls.Add(textBox)
            spacerBr = New Literal
            spacerBr.Text = "<br />"
            ParentControl.Controls.Add(spacerBr)
        End Sub

#End Region

#Region " CreateCostOfCare "

        Private Sub CreateCostOfCare(ByVal COCFieldset As HtmlGenericControl, ByVal Assessment As Assessment)
            Dim Table As New Table
            Dim Cell As TableCell
            Dim row As New TableRow
            Dim Head As TableHeaderCell

            With Table
                .Attributes.Add("class", "listTable sortable")
                .ID = "tblCreateCostOfCare" & Assessment.AssessmentID
                .CellPadding = 2
                .CellSpacing = 0
                .Attributes.Add("style", "table-layout:fixed; float:left; margin-top:0em;")
                'Table Header
                Head = New TableHeaderCell
                Head.Text = "Description"
                Head.ID = "thRef"
                Head.Attributes.Add("style", "width:40%;")
                row.Cells.Add(Head)
                Head = New TableHeaderCell
                Head.Text = "Weekly Rate"
                Head.ID = "thRate"
                Head.Attributes.Add("style", "width:20%;")
                row.Cells.Add(Head)
                .Rows.Add(row)

                row = New TableRow
                Cell = New TableCell
                Cell.Text = "Accommodation"
                row.Cells.Add(Cell)


                Dim sumElements As Decimal = 0
                Dim accommodationCost As Decimal = 0
                For Each Element As CareCostElement In CareCostElements
                    If Element.AssessmentID = Assessment.AssessmentID And Element.InHomeCost = True Then
                        sumElements = sumElements + Element.Amount
                    End If
                Next
                Cell = New TableCell
                accommodationCost = Assessment.Homecost - sumElements
                Cell.Text = accommodationCost.ToString("c")
                row.Cells.Add(Cell)
                .Rows.Add(row)


                For Each Element As CareCostElement In CareCostElements
                    If Element.AssessmentID = Assessment.AssessmentID And (Element.InHomeCost = True Or _
                            (Element.InHomeCost = False And Element.PayOnBehalf = True And _
                            (Element.PCTFunded And Element.PCTPayOnBehalf = True) Or Element.PCTFunded = False)) Then
                        row = New TableRow
                        Cell = New TableCell
                        Cell.Text = Element.Description
                        row.Cells.Add(Cell)
                        Cell = New TableCell
                        Cell.Text = Element.Amount.ToString("c")
                        row.Cells.Add(Cell)
                        .Rows.Add(row)
                    End If
                Next

                row = New TableRow
                Cell = New TableCell
                Cell.Text = "Total Cost Of Care"
                Cell.Attributes.Add("style", "font-weight:bold;")
                row.Cells.Add(Cell)
                sumElements = 0
                For Each Element As CareCostElement In CareCostElements
                    If Element.AssessmentID = Assessment.AssessmentID And (Element.InHomeCost = True Or _
                            (Element.InHomeCost = False And Element.PayOnBehalf = True And _
                            (Element.PCTFunded And Element.PCTPayOnBehalf = True) Or Element.PCTFunded = False)) Then
                        sumElements = sumElements + Element.Amount
                    End If
                Next
                Cell = New TableCell
                Cell.Text = (accommodationCost + sumElements).ToString("c")
                Cell.Attributes.Add("style", "font-weight:bold;")
                row.Cells.Add(Cell)
                .Rows.Add(row)
            End With

            COCFieldset.Controls.Add(Table)
        End Sub

#End Region

#Region " CreateIncomeYouCollect "

        Private Sub CreateIncomeYouCollect(ByVal COCFieldset As HtmlGenericControl, ByVal Assessment As Assessment)
            Dim Table As New Table
            Dim Cell As TableCell
            Dim row As New TableRow
            Dim Head As TableHeaderCell

            With Table
                .Attributes.Add("class", "listTable sortable")
                .ID = "tblCreateCostOfCare" & Assessment.AssessmentID
                .CellPadding = 2
                .CellSpacing = 0
                .Attributes.Add("style", "table-layout:fixed; float:left; margin-top:0em;")
                'Table Header
                Head = New TableHeaderCell
                Head.Text = "Description"
                Head.ID = "thRef"
                Head.Attributes.Add("style", "width:40%;")
                row.Cells.Add(Head)
                Head = New TableHeaderCell
                Head.Text = "Weekly Rate"
                Head.ID = "thRate"
                Head.Attributes.Add("style", "width:20%;")
                row.Cells.Add(Head)
                .Rows.Add(row)


                If Not Assessment.PaidGross Then
                    row = New TableRow
                    Cell = New TableCell
                    Cell.Text = "Service User Contribution"
                    row.Cells.Add(Cell)

                    Cell = New TableCell
                    Cell.Text = Assessment.Assessment.ToString("c")
                    row.Cells.Add(Cell)
                    .Rows.Add(row)
                End If

                For Each Element As CareCostElement In CareCostElements
                    If Element.AssessmentID = Assessment.AssessmentID And _
                            Element.PayOnBehalf = False Or _
                            (Element.PCTFunded And Element.PCTPayOnBehalf = False) Then
                        row = New TableRow
                        Cell = New TableCell
                        Cell.Text = Element.Description
                        row.Cells.Add(Cell)
                        Cell = New TableCell
                        Cell.Text = Element.Amount.ToString("c")
                        row.Cells.Add(Cell)
                        .Rows.Add(row)
                    End If
                Next

                row = New TableRow
                Cell = New TableCell
                Cell.Text = "Total Income"
                Cell.Attributes.Add("style", "font-weight:bold;")
                row.Cells.Add(Cell)
                Dim sumElements As Decimal = 0
                For Each Element As CareCostElement In CareCostElements
                    If Element.AssessmentID = Assessment.AssessmentID And _
                            Element.PayOnBehalf = False Or _
                            (Element.PCTFunded And Element.PCTPayOnBehalf = False) Then
                        sumElements = sumElements + Element.Amount
                    End If
                Next
                Cell = New TableCell
                If Not Assessment.PaidGross Then
                    Cell.Text = (Assessment.Assessment + sumElements).ToString("c")
                Else
                    Cell.Text = sumElements.ToString("c")
                End If
                Cell.Attributes.Add("style", "font-weight:bold;")
                row.Cells.Add(Cell)
                .Rows.Add(row)
            End With

            COCFieldset.Controls.Add(Table)
        End Sub

#End Region

#Region " CreateCouncilPayment "

        Private Sub CreateCouncilPayment(ByVal COCFieldset As HtmlGenericControl, ByVal Assessment As Assessment)
            Dim Table As New Table
            Dim Cell As TableCell
            Dim row As New TableRow
            Dim Head As TableHeaderCell

            'Work out the Accomodation Cost
            Dim sumElements As Decimal = 0
            Dim accommodationCost As Decimal = 0
            For Each Element As CareCostElement In CareCostElements
                If Element.AssessmentID = Assessment.AssessmentID And Element.InHomeCost = True Then
                    sumElements = sumElements + Element.Amount
                End If
            Next
            Cell = New TableCell
            accommodationCost = Assessment.Homecost - sumElements

            'Work Out the total Cost Of Service
            sumElements = 0
            Dim totalCostOfService As Decimal = 0
            For Each Element As CareCostElement In CareCostElements
                If Element.AssessmentID = Assessment.AssessmentID And (Element.InHomeCost = True Or _
                        (Element.InHomeCost = False And Element.PayOnBehalf = True And _
                        (Element.PCTFunded And Element.PCTPayOnBehalf = True) Or Element.PCTFunded = False)) Then
                    sumElements = sumElements + Element.Amount
                End If
            Next
            totalCostOfService = accommodationCost + sumElements

            'Work out the total Income
            sumElements = 0
            Dim totalIncome As Decimal = 0
            For Each Element As CareCostElement In CareCostElements
                If Element.AssessmentID = Assessment.AssessmentID And _
                        Element.PayOnBehalf = False Or _
                        (Element.PCTFunded And Element.PCTPayOnBehalf = False) Then
                    sumElements = sumElements + Element.Amount
                End If
            Next
            Cell = New TableCell
            If Not Assessment.PaidGross Then
                totalIncome = Assessment.Assessment + sumElements
            Else
                totalIncome = sumElements
            End If

            With Table
                .Attributes.Add("class", "listTable sortable")
                .ID = "tblCreateCostOfCare" & Assessment.AssessmentID
                .CellPadding = 2
                .CellSpacing = 0
                .Attributes.Add("style", "table-layout:fixed; float:left; margin-top:0em;")
                'Table Header
                Head = New TableHeaderCell
                Head.Text = "Description"
                Head.ID = "thRef"
                Head.Attributes.Add("style", "width:40%;")
                row.Cells.Add(Head)
                Head = New TableHeaderCell
                Head.Text = "Weekly Rate"
                Head.ID = "thRate"
                Head.Attributes.Add("style", "width:20%;")
                row.Cells.Add(Head)
                .Rows.Add(row)

                row = New TableRow
                Cell = New TableCell
                Cell.Text = "Council Payment"
                row.Cells.Add(Cell)

                Cell = New TableCell
                Dim councilPayment As Decimal = 0
                If Not Assessment.PaidGross Then
                    councilPayment = (accommodationCost - Assessment.Assessment).ToString("c")
                Else
                    councilPayment = accommodationCost.ToString("c")
                End If
                Cell.Text = councilPayment.ToString("c")
                row.Cells.Add(Cell)
                .Rows.Add(row)

                'Display all Items that the council are paying for
                For Each Element As CareCostElement In CareCostElements
                    If Element.AssessmentID = Assessment.AssessmentID And _
                        (Element.PayOnBehalf = True And _
                        ((Element.PCTFunded And Element.PCTPayOnBehalf = True) Or Element.PCTFunded = False)) Then
                        row = New TableRow
                        Cell = New TableCell
                        Cell.Text = Element.Description
                        row.Cells.Add(Cell)
                        Cell = New TableCell
                        Cell.Text = Element.Amount.ToString("c")
                        row.Cells.Add(Cell)
                        .Rows.Add(row)
                    End If
                Next

                row = New TableRow
                Cell = New TableCell
                Cell.Text = "Total Payment"
                Cell.Attributes.Add("style", "font-weight:bold;")
                row.Cells.Add(Cell)

                Cell = New TableCell
                Cell.Text = (totalCostOfService - totalIncome).ToString("c")
                Cell.Attributes.Add("style", "font-weight:bold;")
                row.Cells.Add(Cell)
                .Rows.Add(row)
            End With

            COCFieldset.Controls.Add(Table)
        End Sub

#End Region

#Region " CreateIncomeYouHaveCollected "

        Private Sub CreateIncomeYouHaveCollected(ByVal COCFieldset As HtmlGenericControl, ByVal Assessment As Assessment)
            Dim Table As New Table
            Dim Cell As TableCell
            Dim row As New TableRow
            Dim Head As TableHeaderCell
            Dim serviceUserPayments As Decimal
            Dim serviceUserAdjustments As Decimal

            With Table
                .Attributes.Add("class", "listTable sortable")
                .ID = "tblCreateCostOfCare" & Assessment.AssessmentID
                .CellPadding = 2
                .CellSpacing = 0
                .Attributes.Add("style", "table-layout:fixed; float:left; margin-top:0em;")
                'Table Header
                Head = New TableHeaderCell
                Head.Text = "Description"
                Head.ID = "thRef"
                Head.Attributes.Add("style", "width:35%;vertical-align:bottom;")
                row.Cells.Add(Head)
                Head = New TableHeaderCell
                Head.Text = "Weekly<br />Rate"
                Head.ID = "thRate"
                Head.Attributes.Add("style", "width:20%;")
                row.Cells.Add(Head)
                Head = New TableHeaderCell
                Head.Text = "Total<br />Income"
                Head.ID = "thPayments"
                Head.Attributes.Add("style", "width:20%;")
                row.Cells.Add(Head)
                Head = New TableHeaderCell
                Head.Text = "Pending<br />Adjustments"
                Head.ID = "thAdjustments"
                Head.Attributes.Add("style", "width:25%;")
                row.Cells.Add(Head)
                .Rows.Add(row)

                If Not Assessment.PaidGross Then
                    row = New TableRow
                    Cell = New TableCell
                    Cell.Text = "Service User Contribution"
                    row.Cells.Add(Cell)

                    Cell = New TableCell
                    Cell.Text = Assessment.Assessment.ToString("c")
                    row.Cells.Add(Cell)

                    Cell = New TableCell
                    serviceUserPayments = (Assessment.Assessment * Assessment.LastPaymentDays / 7)
                    Cell.Text = serviceUserPayments.ToString("c")
                    row.Cells.Add(Cell)

                    Cell = New TableCell
                    ' LastClientIncomeRate is stored as a negative figure so need to multiple by -1
                    serviceUserAdjustments = (Assessment.Assessment - (Assessment.LastClientIncomeRate * -1)) * (Assessment.PaymentDays / 7)
                    serviceUserAdjustments += (Assessment.PaymentDays - Assessment.LastPaymentDays) * ((Assessment.LastClientIncomeRate * -1) / 7)
                    Cell.Text = serviceUserAdjustments.ToString("c")
                    row.Cells.Add(Cell)

                    .Rows.Add(row)
                End If

                Dim sumPayments As Decimal = 0
                Dim sumAdjustments As Decimal = 0
                For Each Element As CareCostElement In CareCostElements
                    If Element.AssessmentID = Assessment.AssessmentID And _
                            Element.PayOnBehalf = False Or _
                            (Element.PCTFunded And Element.PCTPayOnBehalf = False) Then
                        row = New TableRow
                        Cell = New TableCell
                        Cell.Text = Element.Description
                        row.Cells.Add(Cell)

                        Cell = New TableCell
                        Cell.Text = Element.Amount.ToString("c")
                        row.Cells.Add(Cell)

                        Cell = New TableCell
                        Cell.Text = (Element.Amount * Element.LastPaidDays / 7).ToString("c")
                        sumPayments = sumPayments + Math.Round((Element.Amount * Element.LastPaidDays / 7), 2)
                        row.Cells.Add(Cell)

                        Cell = New TableCell
                        Cell.Text = (((Element.Amount - Element.LastRate) * (Element.PaidDays / 7)) + ((Element.PaidDays - Element.LastPaidDays) * (Element.LastRate / 7))).ToString("c")
                        sumAdjustments = sumAdjustments + Math.Round((Element.Amount - Element.LastRate) * (Element.PaidDays / 7), 2) + Math.Round((Element.PaidDays - Element.LastPaidDays) * (Element.LastRate / 7), 2)
                        row.Cells.Add(Cell)
                        .Rows.Add(row)
                    End If
                Next

                row = New TableRow
                Cell = New TableCell
                Cell.Attributes.Add("style", "font-weight:bold;")
                Cell.Text = "Total"
                row.Cells.Add(Cell)

                Cell = New TableCell
                Cell.Attributes.Add("style", "font-weight:bold;")
                Cell.Text = ""
                row.Cells.Add(Cell)

                Cell = New TableCell
                Cell.Attributes.Add("style", "font-weight:bold;")
                Cell.Text = (sumPayments + serviceUserPayments).ToString("c")
                row.Cells.Add(Cell)

                Cell = New TableCell
                Cell.Attributes.Add("style", "font-weight:bold;")
                Cell.Text = (sumAdjustments + serviceUserAdjustments).ToString("c")
                row.Cells.Add(Cell)
                .Rows.Add(row)

            End With

            COCFieldset.Controls.Add(Table)
        End Sub

#End Region

#Region " CreatePaymentsYouHaveReceived "

        Private Sub CreatePaymentsYouHaveReceived(ByVal COCFieldset As HtmlGenericControl, ByVal Assessment As Assessment)
            Dim Table As New Table
            Dim Cell As TableCell
            Dim row As New TableRow
            Dim Head As TableHeaderCell

            'Work out the Accomodation Cost
            Dim sumElements As Decimal = 0
            Dim accommodationCost As Decimal = 0
            For Each Element As CareCostElement In CareCostElements
                If Element.AssessmentID = Assessment.AssessmentID And Element.InHomeCost = True Then
                    sumElements = sumElements + Element.Amount
                End If
            Next
            Cell = New TableCell
            accommodationCost = Assessment.Homecost - sumElements

            If Not Assessment.PaidGross Then
                accommodationCost -= Assessment.Assessment
            End If

            With Table
                .Attributes.Add("class", "listTable sortable")
                .ID = "tblCreateCostOfCare" & Assessment.AssessmentID
                .CellPadding = 2
                .CellSpacing = 0
                .Attributes.Add("style", "table-layout:fixed; float:left; margin-top:0em;")
                'Table Header
                Head = New TableHeaderCell
                Head.Text = "Description"
                Head.ID = "thRef"
                Head.Attributes.Add("style", "width:35%;vertical-align:bottom;")
                row.Cells.Add(Head)
                Head = New TableHeaderCell
                Head.Text = "Weekly<br />Rate"
                Head.ID = "thRate"
                Head.Attributes.Add("style", "width:20%;")
                row.Cells.Add(Head)
                Head = New TableHeaderCell
                Head.Text = "Total<br />Payments"
                Head.ID = "thPayments"
                Head.Attributes.Add("style", "width:20%;")
                row.Cells.Add(Head)
                Head = New TableHeaderCell
                Head.Text = "Pending<br />Adjustments"
                Head.ID = "thAdjustments"
                Head.Attributes.Add("style", "width:25%;")
                row.Cells.Add(Head)
                .Rows.Add(row)

                row = New TableRow
                Cell = New TableCell
                Cell.Text = "Council Payment"
                row.Cells.Add(Cell)

                Cell = New TableCell
                Cell.Text = accommodationCost.ToString("c")
                row.Cells.Add(Cell)

                Cell = New TableCell
                Dim serviceUserPayments As Decimal = (accommodationCost * Assessment.LastPaymentDays / 7)
                Cell.Text = serviceUserPayments.ToString("c")
                row.Cells.Add(Cell)

                Cell = New TableCell
                Dim accommodationAdjustments As Decimal = 0
                accommodationAdjustments = (Assessment.Payment - Assessment.LastPayment) * (Assessment.PaymentDays / 7)
                accommodationAdjustments += (Assessment.PaymentDays - Assessment.LastPaymentDays) * (Assessment.LastPayment / 7)
                Cell.Text = accommodationAdjustments.ToString("c")
                row.Cells.Add(Cell)

                .Rows.Add(row)

                Dim sumPayments As Decimal = 0
                Dim sumAdjustments As Decimal = 0
                For Each Element As CareCostElement In CareCostElements
                    If Element.AssessmentID = Assessment.AssessmentID And _
                        (Element.PayOnBehalf = True And _
                        ((Element.PCTFunded And Element.PCTPayOnBehalf = True) Or Element.PCTFunded = False)) Then

                        row = New TableRow
                        Cell = New TableCell
                        Cell.Text = Element.Description
                        row.Cells.Add(Cell)

                        Cell = New TableCell
                        Cell.Text = Element.Amount.ToString("c")
                        row.Cells.Add(Cell)

                        Cell = New TableCell
                        Cell.Text = (Element.Amount * Element.LastPaidDays / 7).ToString("c")
                        sumPayments = sumPayments + Math.Round((Element.Amount * Element.LastPaidDays / 7), 2)
                        row.Cells.Add(Cell)

                        Cell = New TableCell
                        Cell.Text = (((Element.Amount - Element.LastRate) * (Element.PaidDays / 7)) + ((Element.PaidDays - Element.LastPaidDays) * (Element.LastRate / 7))).ToString("c")
                        sumAdjustments = sumAdjustments + Math.Round((Element.Amount - Element.LastRate) * (Element.PaidDays / 7), 2) + Math.Round((Element.PaidDays - Element.LastPaidDays) * (Element.LastRate / 7), 2)
                        row.Cells.Add(Cell)
                        .Rows.Add(row)
                    End If
                Next

                row = New TableRow
                Cell = New TableCell
                Cell.Attributes.Add("style", "font-weight:bold;")
                Cell.Text = "Total"
                row.Cells.Add(Cell)

                Cell = New TableCell
                Cell.Attributes.Add("style", "font-weight:bold;")
                Cell.Text = ""
                row.Cells.Add(Cell)

                Cell = New TableCell
                Cell.Attributes.Add("style", "font-weight:bold;")
                Cell.Text = (sumPayments + serviceUserPayments).ToString("c")
                row.Cells.Add(Cell)

                Cell = New TableCell
                Cell.Attributes.Add("style", "font-weight:bold;")
                Cell.Text = (sumAdjustments + accommodationAdjustments).ToString("c")
                row.Cells.Add(Cell)
                .Rows.Add(row)

            End With

            COCFieldset.Controls.Add(Table)
        End Sub

#End Region

#Region " PrintWeeksDays "

        Private Function PrintWeeksDays(ByVal totalDays As Integer, ByVal format As String) As String
            Dim weeks As Integer, days As Integer
            weeks = Math.DivRem(totalDays, 7, days)
            Return String.Format(format, weeks, days)
        End Function

#End Region

#Region " Assessment "
        Friend Class Assessment
            Public AssessmentID As Integer
            Public DateFrom As Date
            Public DateTo As Date
            Public Home As String
            Public PaidTo As Date
            Public PaidGross As Boolean
            Public PaymentSuppress As Boolean
            Public PaymentSuppressReason As String
            Public Homecost As Decimal
            Public Assessment As Decimal
            Public LastClientIncomeRate As Decimal
            Public PaymentDays As Integer
            Public LastPaymentDays As Integer
            Public Payment As Decimal
            Public LastPayment As Decimal
        End Class
#End Region

#Region " CareCostElement "
        Friend Class CareCostElement
            Public AssessmentID As Integer
            Public Description As String
            Public Amount As Decimal
            Public InHomeCost As Boolean
            Public PayOnBehalf As Boolean
            Public PCTPayOnBehalf As Boolean
            Public PCTFunded As Boolean
            Public LastRate As Decimal
            Public PaidDays As Decimal
            Public LastPaidDays As Decimal

            Public Sub New()

            End Sub
        End Class
#End Region

    End Class

End Namespace
