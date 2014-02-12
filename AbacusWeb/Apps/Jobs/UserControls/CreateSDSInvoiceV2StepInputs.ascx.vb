
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
    ''' User control that provides custom inputs for the create SDS invoice v2 job step.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     Iftikhar      01/03/2011  D11966 - priming control with ClientDetailID
    ''' </history>
    Partial Public Class CreateSDSInvoiceV2StepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Protected Const SETTING_INVOICED_UPTO As String = "SDSInvoicingUpTo"

        Const QS_CLIENT_ID As String = "clientID"
        Private _clientID As Integer = Utils.ToInt32(HttpContext.Current.Request.QueryString(QS_CLIENT_ID))


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            Dim lastInvDate As Nullable(Of DateTime)
            Dim payUpToDate As Date
            Dim msg As ErrorMessage

            If Not IsPostBack AndAlso _clientID > 0 Then
                CType(client, InPlaceClientSelector).ClientDetailID = _clientID
            End If

            If Request.Form(String.Format("{0}$txtTextBox", dteInvUpTo.UniqueID)) Is Nothing Then
                msg = SdsInvoicingV2.GetLastPaidUpToDate(thePage.DbConnection, lastInvDate)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If Not lastInvDate Is Nothing Then
                    dteLastInvDate.Text = lastInvDate
                End If

                msg = SdsInvoicingV2.GetDefaultPayUpToDate(thePage.DbConnection, lastInvDate, payUpToDate)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                dteInvUpTo.Text = payUpToDate

            Else
                dteInvUpTo.Text = Request.Form(String.Format("{0}$txtTextBox", dteInvUpTo.UniqueID))
                optGenerateOrReport.SelectedValue = Request.Form(optGenerateOrReport.UniqueID)
                CType(client, InPlaceClientSelector).ClientDetailID = Utils.ToInt32(CType(client, InPlaceClientSelector).GetPostBackValue())
            End If

        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim batchTypesDesc As StringBuilder = New StringBuilder()
            Dim msg As ErrorMessage
            Dim serviceUserID As Integer
            Dim serviceUsr As ClientDetail = Nothing

            'Are we reporting only or generating?
            If optGenerateOrReport.SelectedItem.Text = "Report Only" Then
                result.Add(New Triplet(jobStepTypeID, "Provisional", Boolean.TrueString))
            ElseIf optGenerateOrReport.SelectedItem.Text = "Generate Invoices" Then
                result.Add(New Triplet(jobStepTypeID, "Provisional", Boolean.FalseString))
            End If

            'How long to invoice for
            result.Add(New Triplet(jobStepTypeID, "InvoiceUpTo", dteInvUpTo.Text))

            ' Service User
            serviceUserID = Utils.ToInt32(CType(client, InPlaceClientSelector).ClientDetailID)
            result.Add(New Triplet(jobStepTypeID, "FilterServiceUserID", serviceUserID))
            If serviceUserID > 0 Then
                If Not trans Is Nothing Then
                    serviceUsr = New ClientDetail(trans, String.Empty, String.Empty)
                Else
                    serviceUsr = New ClientDetail(conn, String.Empty, String.Empty)
                End If
                msg = serviceUsr.Fetch(serviceUserID)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                result.Add(New Triplet(jobStepTypeID, "FilterServiceUserName", serviceUsr.Name))
            End If

            Return result

        End Function

    End Class

End Namespace

