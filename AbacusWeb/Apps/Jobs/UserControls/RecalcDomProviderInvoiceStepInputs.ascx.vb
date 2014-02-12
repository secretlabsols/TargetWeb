
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
Imports Target.Abacus.Library.AdministrativeSector
Imports Target.Web.Apps.Security

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control that provides custom inputs for the recalculate domiciliary provider invoice job step.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class RecalcDomProviderInvoiceStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            thePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/Jobs/UserControls/RecalcDomProviderInvoiceStepInputs.js"))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_STARTUP As String = "Startup"

            ' provider
            With CType(provider, InPlaceEstablishmentSelector)
                .Required = False
            End With

            ' contract
            With CType(domContract, InPlaceDomContractSelector)
                .Required = False
            End With

            ' checkboxes - default to ticked if not already submitted
            If Request.Form(String.Format("{0}$chkCheckBox", chkCreateProforma.UniqueID)) Is Nothing Then
                chkCreateProforma.CheckBox.Checked = True
            End If

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                    String.Format("Edit_domContractID='{0}';" & _
                                  "dtefromID='{1}';" & _
                                  "dteToId='{2}';", _
                        domContract.ClientID, _
                        dteDateFrom.ClientID, _
                        dteDateTo.ClientID), _
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
            Dim providerID As Integer, contractID As Integer
            Dim value As String
            Dim isPartitionDataOn As Boolean

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

            ' create proforma invoices
            value = Request.Form(String.Format("{0}$chkCheckBox", chkCreateProforma.UniqueID))
            If value Is Nothing Then
                value = Boolean.FalseString
            ElseIf value = "on" Then
                value = Boolean.TrueString
            Else
                value = Boolean.FalseString
            End If
            result.Add(New Triplet(jobStepTypeID, "FilterCreateProforma", value))

            'data partitioned ?
            msg = AdministrativeAreasBL.IsPartitionDataByAdministrativeAreaOn(trans, conn, isPartitionDataOn)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If isPartitionDataOn Then
                result.Add(New Triplet(jobStepTypeID, "FilterPartitionData", isPartitionDataOn.ToString))
                result.Add(New Triplet(jobStepTypeID, "FilterWebSecurityUserID", SecurityBL.GetCurrentUser.ID))
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

