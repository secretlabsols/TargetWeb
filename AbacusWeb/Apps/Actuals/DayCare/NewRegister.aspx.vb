
Imports Target.Abacus.Web.Apps.UserControls.SelectorWizardSteps
Imports Target.Library
Imports System.Text
Imports Target.Library.Web.UserControls
Imports Target.Web.Apps.Security
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Abacus.Library
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Abacus.Library.DomProviderInvoice
Imports Target.Library.Web.Controls


Namespace Apps.Actuals.DayCare

    ''' <summary>
    ''' Create new register screen, used for creation of new registers.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD  12/05/2010  D11746 - Prevent entry of future dates after the next Sunday if configured within the setting 'PreventEntryOfActualServiceForFuturePeriods'
    ''' MoTahir 05/11/2009  D11681
    ''' </history>
    Partial Public Class NewRegister
        Inherits BasePage

        Private _stdBut As StdButtonsBase
        Private _estabID As Integer
        Private _contractID As Integer
        Private _wkEndingDate As String
        Private createButton As Button = New Button

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")
            AddHandler _stdBut.AddCustomControls, AddressOf StdButtons_AddCustomControls
            AddHandler createButton.Click, AddressOf createButton_Clicked
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_STARTUP As String = "Startup"
            Dim sgClassification As Integer = 0
            Dim msg As ErrorMessage
            Dim preventEntryOfFutureDates As Boolean = False

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.DayCare"), "Create Register")

            'get querystring values
            _estabID = Utils.ToInt32(Request.QueryString("estabID"))
            _contractID = Utils.ToInt32(Request.QueryString("contractID"))
            _wkEndingDate = Utils.ToString(Request.QueryString("wedate"))

            If Not IsPostBack Then
                SetupProviderSelector(_estabID)
                SetupContractSelector(_contractID, _estabID)
                SetupEffectiveDateSelector(_wkEndingDate)
            End If

            ' setup buttons
            With _stdBut
                .AllowBack = True
                .AllowNew = False
                .AllowEdit = False
                .AllowDelete = False
                .AllowFind = False
            End With

            ' add utility JS link
            Me.JsLinks.Add("NewRegister.js")
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
            sgClassification = ServiceGroupClassifications.DayCare
            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                 String.Format("Edit_domContractID='{0}';InPlaceDomContractSelector_providerID={1};dteDatesEffectiveDateID='{2}';sgClassificationID='{3}';", _
                  domContract.ClientID, _estabID, dteDatesEffectiveDate.ClientID, sgClassification), True)
            End If

            ' determine whether we should allow future periods
            msg = DomProviderInvoiceBL.ShouldPreventEntryOfActualServiceForFuturePeriods(DbConnection, _
                                                                                        preventEntryOfFutureDates)

            If msg.Success Then
                ' if we fetched the setting correctly

                If preventEntryOfFutureDates Then
                    ' if we should prevent future dates then setup range validator, 
                    ' in this interface we do allow upto the next sunday to be entered

                    SetUpPreventEntryOfFutureDatesRangeValidator(rvDatesEffectiveDate, dteDatesEffectiveDate)

                Else
                    ' else we shouldnt prevent future dates so hide the validator

                    rvDatesEffectiveDate.Visible = False

                End If

            Else
                ' else we didn't fetch the setting correctly so throw an error

                WebUtils.DisplayError(msg)

            End If

        End Sub

#Region " StdButtons_AddCustomControls "
        Private Sub StdButtons_AddCustomControls(ByRef controls As ControlCollection)
            createButton.Text = "Create"
            createButton.Style.Add("padding-right", "1em")
            createButton.Style.Add("padding-left", "1em")
            createButton.ValidationGroup = "Save"
            controls.Add(createButton)
        End Sub
#End Region

#Region "createButton_Clicked"
        Protected Sub createButton_Clicked(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim providerID As String = "", contractID As String = ""
            Dim weDate As Date = dteDatesEffectiveDate.Text
            Dim result As ErrorMessage
            Dim contractBL As DomContractBL = New DomContractBL
            result = contractBL.WeekendingDateValid(weDate)

            If result.Success = False Then
                lblError.Text = "Please enter a vaild week ending date in the format dd/mm/yyyy"
            Else
                providerID = Request.Form(CType(provider, InPlaceEstablishmentSelector).HiddenFieldUniqueID)
                contractID = Request.Form(CType(domContract, InPlaceDomContractSelector).HiddenFieldUniqueID)
                If providerID <> "" And contractID <> "" Then
                    result = contractBL.PrimeRegister(providerID, contractID, weDate, Me.PageTitle, Me.Settings)
                    If result.Success = False Then
                        lblError.Text = result.Message
                    Else
                        Response.Redirect("Edit.aspx?mode=1&ID=" & result.Message & "&backUrl=" & HttpUtility.UrlEncode(Utils.ToString(Request.QueryString("backUrl"))))
                    End If
                Else
                    lblError.Text = "Unable to create register No Provider Id supplier"
                End If
            End If
        End Sub
#End Region

#Region " SetupInPlaceSelectors "
        Private Sub SetupProviderSelector(ByVal providerID As Integer)
            If providerID > 0 Then
                With CType(Me.provider, InPlaceSelectors.InPlaceEstablishmentSelector)
                    .EstablishmentID = providerID
                    .Required = True
                End With
            End If
        End Sub

        Private Sub SetupContractSelector(ByVal contractID As Integer, ByVal providerID As Integer)
            With CType(Me.domContract, InPlaceSelectors.InPlaceDomContractSelector)
                If contractID > 0 And providerID > 0 Then
                    .ContractID = contractID
                    .Required = True
                End If
                If providerID > 0 Then
                    .Enabled = True
                    .Required = True
                Else
                    .Enabled = False
                End If
                .ServiceGroupClassificationID = ServiceGroupClassifications.DayCare
            End With
        End Sub

        Private Sub SetupEffectiveDateSelector(ByVal weDate As String)
            dteDatesEffectiveDate.Text = weDate
        End Sub
#End Region

#Region "SetUpPreventEntryOfFutureDatesRangeValidator"

        ''' <summary>
        ''' Sets up prevent entry of future dates range validator.
        ''' </summary>
        ''' <param name="validator">The validator.</param>
        ''' <param name="controlToValidate">The control to validate.</param>
        Private Sub SetUpPreventEntryOfFutureDatesRangeValidator(ByVal validator As RangeValidator, _
                                                                 ByVal controlToValidate As TextBoxEx)

            With validator
                .ControlToValidate = String.Format("{0}${1}", controlToValidate.ID, controlToValidate.TextBox.ID)
                .MaximumValue = DomContractBL.GetWeekEndingDate(CType(Me.Page, BasePage).DbConnection, Nothing)
                .MinimumValue = DateTime.MinValue.ToString("dd/MM/yyyy")
                .Type = ValidationDataType.Date
                .Visible = True
            End With

        End Sub

#End Region

    End Class

End Namespace