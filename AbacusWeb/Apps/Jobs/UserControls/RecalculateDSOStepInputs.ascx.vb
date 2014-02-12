
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
    ''' User control that provides custom inputs for the 'Recalculate DSO' job step.
    ''' </summary>
    ''' <remarks></remarks>
    Partial Public Class RecalculateDSOStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            thePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/Jobs/UserControls/RecalculateDSOStepInputs.js"))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Const SCRIPT_STARTUP As String = "Startup"

            Dim thePage As BasePage = CType(Me.Page, BasePage)
            Dim script As StringBuilder
            Dim scriptString As String

            thePage.AddExtraCssStyle(".chkBoxStyle { float:left; margin-right:2em; }")

            PopulateScreen()

            '++ Output the contract control ID to the startup script..
            script = New StringBuilder()
            scriptString = script.ToString()
            If scriptString.Length > 0 Then scriptString = scriptString.Substring(0, scriptString.Length - 1)

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                    String.Format("Edit_domContractID='{0}';", domContract.ClientID), _
                    True _
                )
            End If

        End Sub

        Private Sub PopulateScreen()
            Dim contractID As Integer

            '++ Set the need for screen validation based on a contract being specified..
            With CType(domContract, InPlaceDomContractSelector)
                contractID = Utils.ToInt32(.GetPostBackValue())
                .Required = (contractID = 0)
            End With
        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim msg As ErrorMessage
            Dim contract As DomContract
            Dim contractID As Integer

            '++ Set the contract triplet items..
            contractID = Utils.ToInt32(CType(DomContract, InPlaceDomContractSelector).GetPostBackValue())
            result.Add(New Triplet(jobStepTypeID, "ContractID", contractID))
            If contractID > 0 Then
                If Not trans Is Nothing Then
                    contract = New DomContract(trans, String.Empty, String.Empty)
                Else
                    contract = New DomContract(conn, String.Empty, String.Empty)
                End If
                msg = contract.Fetch(contractID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "ContractDesc", String.Format("{0}/{1}", contract.Number, contract.Title)))
            End If

            Return result

        End Function

    End Class

End Namespace

