Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library
Imports Target.Abacus.Web.Apps.InPlaceSelectors
Imports Target.Library
Imports Target.Library.Web.Controls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control that provides custom inputs for the re-evaluation of visit based client charges job step.
    ''' </summary>
    ''' <remarks></remarks>

    Partial Public Class CreateReEvaluateVisitBasedClientChargesStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            thePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/Jobs/UserControls/CreateReEvaluateVisitBasedClientChargesStepInputs.js"))
            ' add date utility javascript
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_STARTUP As String = "Startup"
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            Dim systemCalcType As String = String.Empty
            Dim defaultDate As DateTime
            Dim minDate As DateTime = New DateTime("1753,1,1")
            Dim msg As ErrorMessage = New ErrorMessage()


            If thePage.Settings.SettingExists(DomContractBL.SETTING_SERVICE_USER_MINUTES_CALC_METHOD) Then
                systemCalcType = thePage.Settings(DomContractBL.SETTING_SERVICE_USER_MINUTES_CALC_METHOD)
            Else
                systemCalcType = "1"
            End If

            ' provider
            With CType(provider, InPlaceEstablishmentSelector)
                .Required = False
            End With

            ' contract  
            With CType(domContract, InPlaceDomContractSelector)
                .Required = False
            End With

            ' service user 
            With CType(serviceUser, InPlaceClientSelector)
                .Required = False
            End With

            '' get default Weekending Date From

            msg = DomContractBL.GetDefaultWeekEndingFromDateToReEvaluateVisitBasedClientcharges(thePage.DbConnection, defaultDate, thePage.Settings.CurrentApplicationID)
            If Not msg.Success Then WebUtils.DisplayError(msg)


            '' add on change javascript method

            rdbProvider.Attributes.Add("onclick", "selectionChanged('" & rdbProvider.ClientID & "','provider')")
            rdbServiceUser.Attributes.Add("onclick", "selectionChanged('" & rdbServiceUser.ClientID & "','serviceUser')")

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                    String.Format("Edit_domContractID='{0}';" & _
                                  "systemCalcType='{1}';" & _
                                  "dtefromID='{2}';" & _
                                  "dteToId='{3}';" & _
                                  "Edit_domProviderID='{4}';" & _
                                  "Edit_serviceUserID='{5}';" & _
                                  "radioProviderId='{6}';" & _
                                  "radioServiceUserId='{7}';" & _
                                  "defaultWEDate='{8}';" & _
                                  "minDate='{9}';", _
                        domContract.ClientID, _
                        systemCalcType, _
                        dteDateFrom.ClientID, _
                        dteDateTo.ClientID, _
                        provider.ClientID, _
                        serviceUser.ClientID, _
                        rdbProvider.ClientID, _
                        rdbServiceUser.ClientID, _
                        defaultDate.ToShortDateString(), _
                        minDate.ToShortDateString()), _
                    True _
                )
            End If

        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim msg As ErrorMessage
            Dim estab As Establishment
            Dim contract As DomContract
            Dim svcUser As ClientDetail
            Dim providerID As Integer, contractID As Integer, serviceUserID As Integer
            Dim filterByClientID As Boolean
            Dim value As String

            ' provider
            providerID = Utils.ToInt32(CType(provider, InPlaceEstablishmentSelector).GetPostBackValue())
            result.Add(New Triplet(jobStepTypeID, "FilterProviderID", providerID))
            If providerID > 0 Then
                If Not trans Is Nothing Then
                    estab = New Establishment(trans)
                Else
                    estab = New Establishment(conn)
                End If
                msg = estab.Fetch(providerID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "FilterProviderDesc", estab.Name))
            End If

            ' contract
            contractID = Utils.ToInt32(CType(domContract, InPlaceDomContractSelector).GetPostBackValue())
            result.Add(New Triplet(jobStepTypeID, "FilterContractID", contractID))
            If contractID > 0 Then
                If Not trans Is Nothing Then
                    contract = New DomContract(trans, String.Empty, String.Empty)
                Else
                    contract = New DomContract(conn, String.Empty, String.Empty)
                End If
                msg = contract.Fetch(contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "FilterContractDesc", String.Format("{0}/{1}", contract.Number, contract.Title)))
            End If

            ' service User
            serviceUserID = Utils.ToInt32(CType(serviceUser, InPlaceClientSelector).GetPostBackValue())
            result.Add(New Triplet(jobStepTypeID, "FilterServiceUserID", serviceUserID))
            If serviceUserID > 0 Then
                If Not trans Is Nothing Then
                    svcUser = New ClientDetail(trans, String.Empty, String.Empty)
                Else
                    svcUser = New ClientDetail(conn, String.Empty, String.Empty)
                End If
                msg = svcUser.Fetch(serviceUserID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "FilterServiceUserDesc", String.Format("{0}/{1}", svcUser.Reference, svcUser.Name)))
            End If

            ' filter by client id
            If serviceUserID = 0 Then
                filterByClientID = False
            Else
                filterByClientID = True
            End If


            result.Add(New Triplet(jobStepTypeID, "FilterByClientID", filterByClientID))

            ' date from
            value = Request.Form(String.Format("{0}$txtTextBox", dteDateFrom.UniqueID))
            If Not value Is Nothing AndAlso value.Trim().Length > 0 Then
                result.Add(New Triplet(jobStepTypeID, "FilterDateFrom", value))
            End If

            ' date to
            value = Request.Form(String.Format("{0}$txtTextBox", dteDateTo.UniqueID))
            If Not value Is Nothing AndAlso value.Trim().Length > 0 Then
                result.Add(New Triplet(jobStepTypeID, "FilterDateTo", value))
            End If

            Return result

        End Function


        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            Dim weekEndingDate As DateTime = Target.Abacus.Library.DomContractBL.GetWeekEndingDate(thePage.DbConnection, Nothing)

            ' setup date from date picker
            With dteDateFrom
                .AllowableDays = weekEndingDate.DayOfWeek
            End With

            ' setup date to date picker
            With dteDateTo
                .AllowableDays = weekEndingDate.DayOfWeek
            End With
        End Sub

      
    End Class


End Namespace