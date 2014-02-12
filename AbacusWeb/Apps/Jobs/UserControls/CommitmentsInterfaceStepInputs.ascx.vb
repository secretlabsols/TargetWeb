
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Abacus.Jobs.Core
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.Controls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.Jobs.UserControls

    ''' <summary>
    ''' User control that provides custom inputs for the Service Commitment Interface initial job step.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	John Finch   16/10/2012  Created (D12376A)
    ''' </history>
    Partial Public Class CommitmentsInterfaceStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            '++ Add date utility javascript..
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            '++ Add utility javascript link..
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            '++ Add user control javascript..
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Jobs/UserControls/CommitmentsInterfaceStepInputs.js"))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Const SCRIPT_STARTUP As String = "Startup"
            Dim js As StringBuilder = New StringBuilder()
            Dim msg As New ErrorMessage
            Dim dtEndDate As Date = Nothing
            Dim isSDSv2Licensed As Boolean = False, isDPLicensed As Boolean = False

            '++ Default field values..
            If Request.Form(String.Format("{0}$txtTextBox", dteCommitmentDate.UniqueID)) Is Nothing Then
                '++ Set to the next 31st March (nominal financial year-end)..
                dtEndDate = DateSerial(Today.Year, 3, 31)
                If Today.Month > 3 Then
                    dtEndDate = DateAdd(DateInterval.Year, 1, dtEndDate)
                End If
                dteCommitmentDate.TextBox.Text = dtEndDate.ToString("dd/MM/yyyy")
            End If
            If Request.Form(String.Format("{0}$chkCheckBox", chkResidential.UniqueID)) Is Nothing Then
                chkResidential.CheckBox.Checked = True
            End If
            If Request.Form(String.Format("{0}$chkCheckBox", chkDP.UniqueID)) Is Nothing Then
                msg = Licensing.ModuleLicence.AreAnyModulesLicensed(CType(Me.Page, BasePage).DbConnection, _
                      New Licensing.ModuleLicenses() {Licensing.ModuleLicenses.IntranetSDSv2}, _
                      isSDSv2Licensed)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                msg = Licensing.ModuleLicence.AreAnyModulesLicensed(CType(Me.Page, BasePage).DbConnection, _
                      New Licensing.ModuleLicenses() {Licensing.ModuleLicenses.IntranetDirectPayments}, _
                      isDPLicensed)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                chkDP.CheckBox.Checked = (isSDSv2Licensed Or isDPLicensed)
            End If
            If Request.Form(String.Format("{0}$chkCheckBox", chkIncludeLastRun.UniqueID)) Is Nothing Then
                chkIncludeLastRun.CheckBox.Checked = True
            End If

            chkIncludeLastRun.CheckBox.Attributes.Add("onclick", "chkIncludeLastRun_Click();")

            '++ Output the control IDs to the startup script..
            js.AppendFormat("dteCommitmentDateID='{0}';", dteCommitmentDate.ClientID)
            js.AppendFormat("chkResidentialID='{0}';", chkResidential.ClientID)
            js.AppendFormat("chkDPID='{0}';", chkDP.ClientID)
            js.AppendFormat("chkIncludeLastRunID='{0}';", chkIncludeLastRun.ClientID)

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                    js.ToString, _
                    True _
                )
            End If
        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim value As String

            '++ Commitment up-to date..
            value = Request.Form(String.Format("{0}$txtTextBox", dteCommitmentDate.UniqueID))
            If Not value Is Nothing AndAlso value.Trim().Length > 0 Then
                result.Add(New Triplet(jobStepTypeID, "FilterCommitmentDate", value))
            End If

            '++ Residential..?
            value = Request.Form(String.Format("{0}$chkCheckBox", chkResidential.UniqueID))
            If value Is Nothing Then
                value = Boolean.FalseString
            ElseIf value = "on" Then
                value = Boolean.TrueString
            Else
                value = Boolean.FalseString
            End If
            result.Add(New Triplet(jobStepTypeID, "FilterResidential", value))

            '++ Direct Payments..?
            value = Request.Form(String.Format("{0}$chkCheckBox", chkDP.UniqueID))
            If value Is Nothing Then
                value = Boolean.FalseString
            ElseIf value = "on" Then
                value = Boolean.TrueString
            Else
                value = Boolean.FalseString
            End If
            result.Add(New Triplet(jobStepTypeID, "FilterDirectPayments", value))

            '++ Include Negated Commitment from previous run..?
            value = Request.Form(String.Format("{0}$chkCheckBox", chkIncludeLastRun.UniqueID))
            If value Is Nothing Then
                value = Boolean.FalseString
            ElseIf value = "on" Then
                value = Boolean.TrueString
            Else
                value = Boolean.FalseString
            End If
            result.Add(New Triplet(jobStepTypeID, "FilterIncludeLastRun", value))

            Return result

        End Function
    End Class

End Namespace

