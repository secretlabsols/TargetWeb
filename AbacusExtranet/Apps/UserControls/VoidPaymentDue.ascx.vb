Imports Target.Abacus.Library
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Web.Apps.Security
Imports webUtils = Target.Library.Web.Utils
Imports Target.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Web.Apps
Imports Target.Abacus.Library.PaymentSchedules

Namespace Apps.UserControls

    Partial Public Class VoidPaymentDue
        Inherits System.Web.UI.UserControl

        Private _pScheduleId As Integer = 0
        Public Property pScheduleId() As Integer
            Get
                Return _pScheduleId
            End Get
            Set(ByVal value As Integer)
                _pScheduleId = value
            End Set
        End Property


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim thePage As BasePage = CType(Me.Page, BasePage)
            Dim msg As New ErrorMessage()

            pScheduleId = Utils.ToInt32(Request.QueryString("pScheduleId"))

            Dim pschedule As PaymentSchedule = New PaymentSchedule(thePage.DbConnection, String.Empty, String.Empty)
            If (pScheduleId <> 0) Then

                msg = pschedule.Fetch(pScheduleId)
                If Not msg.Success Then webUtils.DisplayError(msg)

                ShowHideVoidPaymentDetails(thePage.DbConnection, pschedule)
            End If

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Dom.PaymentSchedules.PaymentSchedules.Startup", _
             Target.Library.Web.Utils.WrapClientScript(String.Format("pScheduleId={0};", _
                                                                     Utils.ToInt32(pschedule.ID) _
                                                        )) _
                                                   )

        End Sub

        Private Sub ShowHideVoidPaymentDetails(conn As SqlClient.SqlConnection, Optional pschedule As PaymentSchedule = Nothing)
            '' if no contract then just hide it
            If pschedule Is Nothing Then
                pnlVoidPaymentdue.Visible = False
                Return
            End If

            'If Weak have contract then process as below 

            Dim msg As New ErrorMessage()
            Dim contract As DomContract = New DomContract(conn, String.Empty, String.Empty)
            msg = contract.Fetch(pschedule.DomContractID)
            If Not msg.Success Then
                webUtils.DisplayError(msg)
            End If

            If contract.ContractType <> [Enum].GetName(GetType(DomContractType), DomContractType.BlockGuarantee) Then
                pnlVoidPaymentdue.Visible = False
            Else
                ' populate information of void Payment
                Dim voidDetail As New VoidPaymentdetail()
                msg = VoidPaymentsBL.GetVoidPaymentDetails(conn, pschedule.ID, voidDetail)
                lnkPaymentDue.Text = voidDetail.VoidPaymentDue.ToString("C")
            End If

        End Sub

    End Class

End Namespace