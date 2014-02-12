Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    ''' <summary>
    ''' Control to provide access to the different service order reports.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' PaulW  21/10/2011  D11945A - Initial Version.
    ''' </history>
    Partial Class ServiceOrderReports
        Inherits System.Web.UI.UserControl

        Private _weekEndingDate As DateTime
        Public Property weekEndingDate() As DateTime
            Get
                Return _weekEndingDate
            End Get
            Set(ByVal value As DateTime)
                _weekEndingDate = value
            End Set
        End Property

        Private _establishmentID As Integer
        Public Property EstablishmentID() As Integer
            Get
                Return _establishmentID
            End Get
            Set(ByVal value As Integer)
                _establishmentID = value
            End Set
        End Property

        Private _contractID As Integer
        Public Property ContractID() As Integer
            Get
                Return _contractID
            End Get
            Set(ByVal value As Integer)
                _contractID = value
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

        Private _movement As Int16
        Public Property Movement() As Int16
            Get
                Return _movement
            End Get
            Set(ByVal value As Int16)
                _movement = value
            End Set
        End Property

        Private _dsoId As Integer
        Public Property DsoID() As Integer
            Get
                Return _dsoId
            End Get
            Set(ByVal value As Integer)
                _dsoId = value
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

            ThePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusExtranet/Apps/UserControls/HiddenReportStep.js"))
            ThePage.JsLinks.Add(WebUtils.GetVirtualPath("AbacusExtranet/Apps/UserControls/ServiceOrderReports.js"))
            ThePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/WebSvcUtils.js"))


            weekEndingDate = Target.Abacus.Library.DomContractBL.GetWeekEndingDate(ThePage.DbConnection, Nothing, DateTime.Now)
            detailWeekEndingDate.Text = weekEndingDate.ToShortDateString()
            detailWeekEndingDate.AllowableDays = weekEndingDate.DayOfWeek

            With lstReports
                .Rows = 10
                .Attributes.Add("onchange", "lstReports_Change();")
                With .Items
                    '.Add(New ListItem("Simple list of service orders", divSoList.ClientID))
                    .Add(New ListItem("Simple list of orders", divDsoList.ClientID))
                    .Add(New ListItem("Simple list of order details", divDsoDetailList.ClientID))
                    .Add(New ListItem("Simple list of order suspensions", divDsoSuspensionList.ClientID))
                End With
            End With


            ' dso list
            With CType(ctlDsoList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebReport.ServiceOrders")
                If EstablishmentID > 0 Then .Parameters.Add("EstablishmentID", EstablishmentID)
                If ContractID > 0 Then .Parameters.Add("ContractID", ContractID)
                If Utils.IsDate(DateFrom) Then .Parameters.Add("DateFrom", DateFrom)
                If Utils.IsDate(DateTo) Then .Parameters.Add("DateTo", DateTo)
                .Parameters.Add("Movement", Movement)
                If DsoID > 0 Then .Parameters.Add("intDsoID", DsoID)
            End With

            ' DSO detail list
            With CType(ctlDsoDetailList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebReport.ServiceOrderDetails")
                If EstablishmentID > 0 Then .Parameters.Add("EstablishmentID", EstablishmentID)
                If ContractID > 0 Then .Parameters.Add("ContractID", ContractID)
                If Utils.IsDate(DateFrom) Then .Parameters.Add("DateFrom", DateFrom)
                If Utils.IsDate(DateTo) Then .Parameters.Add("DateTo", DateTo)
                .Parameters.Add("Movement", Movement)
                .Parameters.Add("WED", weekEndingDate.ToShortDateString())
                .Parameters.Add("Filter", Not chkDonotfilter.Checked)
                If DsoID > 0 Then .Parameters.Add("intDsoID", DsoID)
            End With

            ' DSO visit list
            With CType(ctlDsoSuspensionList, IReportsButton)
                .ReportID = Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebReport.ServiceOrderSuspensions")
                If EstablishmentID > 0 Then .Parameters.Add("EstablishmentID", EstablishmentID)
                If ContractID > 0 Then .Parameters.Add("ContractID", ContractID)
                If Utils.IsDate(DateFrom) Then .Parameters.Add("DateFrom", DateFrom)
                If Utils.IsDate(DateTo) Then .Parameters.Add("DateTo", DateTo)
                .Parameters.Add("Movement", Movement)
                If DsoID > 0 Then .Parameters.Add("intDsoID", DsoID)
            End With


            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Extranet.Apps.Apps.UserControls.ServiceOrderReports", _
             Target.Library.Web.Utils.WrapClientScript(String.Format("Report_lstReportlistId='{0}';" & _
                                                                     "reportButtonID='{1}';", _
                                                                     lstReports.ClientID, _
                                                                     ctlDsoDetailList.ClientID _
                                                        )) _
                                                )

          




        End Sub
    End Class

End Namespace