
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
    ''' User control that provides custom inputs for the create domiciliary provider invoice job step.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class CreateDomProvInvFromAttendanceRegisterStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            thePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/Jobs/UserControls/CreateDomProvInvFromAttendanceRegisterStepInputs.js"))
            'If Not Me.IsPostBack Then
            '    chkAmendedRegisters.CheckBox.Checked = True
            '    chkNewRegisters.CheckBox.Checked = True
            'End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_STARTUP As String = "Startup"

            Dim thePage As BasePage = CType(Me.Page, BasePage)

            thePage.AddExtraCssStyle(".chkBoxStyle { float:left; margin-right:2em; }")

            PopulateScreen()

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                    String.Format("Edit_domContractID='{0}';Edit_NewRegID='{1}';Edit_AmendedRegID='{2}';", _
                        domContract.ClientID, chkNewRegisters.ClientID, chkAmendedRegisters.ClientID), _
                    True _
                )
            End If

        End Sub

        Private Sub PopulateScreen()

            ' checkboxes - default to ticked if not already submitted
            If Request.Form(String.Format("{0}$chkCheckBox", chkNewRegisters.UniqueID)) Is Nothing Then
                chkNewRegisters.CheckBox.Checked = True
            End If
            If Request.Form(String.Format("{0}$chkCheckBox", chkAmendedRegisters.UniqueID)) Is Nothing Then
                chkAmendedRegisters.CheckBox.Checked = True
            End If

            ' provider
            With CType(Provider, InPlaceEstablishmentSelector)
                .Required = False
            End With

            ' contract
            With CType(DomContract, InPlaceDomContractSelector)
                .Required = False
            End With

        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim batchTypesDesc As StringBuilder = New StringBuilder()
            Dim msg As ErrorMessage
            Dim estab As Establishment
            Dim contract As DomContract
            Dim providerID As Integer, contractID As Integer
            Dim value As String


            value = Request.Form(String.Format("{0}$chkCheckBox", chkNewRegisters.UniqueID))
            If value Is Nothing Then
                value = Boolean.FalseString
            ElseIf value = "on" Then
                value = Boolean.TrueString
            Else
                value = Boolean.FalseString
            End If
            result.Add(New Triplet(jobStepTypeID, "NewRegisters", value))


            value = Request.Form(String.Format("{0}$chkCheckBox", chkAmendedRegisters.UniqueID))
            If value Is Nothing Then
                value = Boolean.FalseString
            ElseIf value = "on" Then
                value = Boolean.TrueString
            Else
                value = Boolean.FalseString
            End If
            result.Add(New Triplet(jobStepTypeID, "AmendedRegisters", value))

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

            Return result

        End Function

    End Class

End Namespace

