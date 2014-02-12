
Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' <summary>
    ''' Control to provide access to the different Direct Payment contract reports.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' JohnF   20/10/2010   Change case of report titles (D11922)
    ''' JohnF   28/07/2010   Created (D11801)
    ''' </history>
    Partial Class DPContractReports
        Inherits System.Web.UI.UserControl

        Private _clientdetailID As Integer
        Public Property clientDetailID() As Integer
            Get
                Return _clientdetailID
            End Get
            Set(ByVal value As Integer)
                _clientdetailID = value
            End Set
        End Property

        Private _budgetHolderID As Integer
        Public Property budgetHolderID() As Integer
            Get
                Return _budgetHolderID
            End Get
            Set(ByVal value As Integer)
                _budgetHolderID = value
            End Set
        End Property

        Private _dateFrom As Date
        Public Property DateFrom() As Date
            Get
                Return _dateFrom
            End Get
            Set(ByVal value As Date)
                _dateFrom = value
            End Set
        End Property

        Private _dateTo As Date
        Public Property DateTo() As Date
            Get
                Return _dateTo
            End Get
            Set(ByVal value As Date)
                _dateTo = value
            End Set
        End Property

        Private _contractID As Integer
        Public Property contractID() As Integer
            Get
                Return _contractID
            End Get
            Set(ByVal value As Integer)
                _contractID = value
            End Set
        End Property


        Private _thePage As BasePage
        Public Property ThePage() As BasePage
            Get
                Return _thePage
            End Get
            Set(ByVal value As BasePage)
                _thePage = value
            End Set
        End Property


        Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            thePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

            With lstReports
                .Rows = 10
                .Attributes.Add("onchange", "lstReports_Change();")
                With .Items
                    .Add(New ListItem("Simple list of direct payments", divContractList.ClientID))
                    .Add(New ListItem("Simple list of direct payment periods", divPeriodList.ClientID))
                    .Add(New ListItem("Simple list of direct payment details", divDetailList.ClientID))
                    .Add(New ListItem("Simple list of direct payment breakdown details", divBreakdownList.ClientID))
                End With
            End With

            '++ List of Direct Payment contracts..
            With CType(ctlContractList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DirectPaymentContracts")
                If clientDetailID > 0 Then .Parameters.Add("intClientID", clientDetailID)
                If budgetHolderID > 0 Then .Parameters.Add("intBudgetHolderID", budgetHolderID)
                If Utils.IsDate(dateFrom) Then .Parameters.Add("dteDateFrom", dateFrom)
                If Utils.IsDate(dateTo) Then .Parameters.Add("dteDateTo", dateTo)
                If contractID > 0 Then .Parameters.Add("intContractID", contractID)
            End With

            '++ List of Direct Payment periods..
            With CType(ctlPeriodList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DirectPaymentContractPeriods")
                If clientDetailID > 0 Then .Parameters.Add("intClientID", clientDetailID)
                If budgetHolderID > 0 Then .Parameters.Add("intBudgetHolderID", budgetHolderID)
                If Utils.IsDate(dateFrom) Then .Parameters.Add("dteDateFrom", dateFrom)
                If Utils.IsDate(dateTo) Then .Parameters.Add("dteDateTo", dateTo)
                If contractID > 0 Then .Parameters.Add("intContractID", contractID)
            End With

            '++ List of Direct Payment details..
            With CType(ctlDetailList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DirectPaymentDetails")
                If clientDetailID > 0 Then .Parameters.Add("intClientID", clientDetailID)
                If budgetHolderID > 0 Then .Parameters.Add("intBudgetHolderID", budgetHolderID)
                If Utils.IsDate(dateFrom) Then .Parameters.Add("dteDateFrom", dateFrom)
                If Utils.IsDate(dateTo) Then .Parameters.Add("dteDateTo", dateTo)
                If contractID > 0 Then .Parameters.Add("intContractID", contractID)
            End With

            '++ List of Direct Payment breakdown details..
            With CType(ctlBreakdownList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebReport.DirectPaymentBreakdownDetails")
                If clientDetailID > 0 Then .Parameters.Add("intClientID", clientDetailID)
                If budgetHolderID > 0 Then .Parameters.Add("intBudgetHolderID", budgetHolderID)
                If Utils.IsDate(dateFrom) Then .Parameters.Add("dteDateFrom", dateFrom)
                If Utils.IsDate(dateTo) Then .Parameters.Add("dteDateTo", dateTo)
                If contractID > 0 Then .Parameters.Add("intContractID", contractID)
            End With


            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Apps.UserControls.ServiceOrderReports", _
           Target.Library.Web.Utils.WrapClientScript(String.Format("Report_lstReportlistId='{0}';", _
                                                                   lstReports.ClientID _
                                                      )) _
                                              )
        End Sub
    End Class

End Namespace

