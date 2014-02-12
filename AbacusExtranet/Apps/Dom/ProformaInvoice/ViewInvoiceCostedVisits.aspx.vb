Imports Target.Web.Apps.Security
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library.DataClasses
Imports System.Text
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Abacus.Library
Imports WebUtils = Target.Library.Web.Utils
Namespace Apps.Dom.ProformaInvoice

    ''' <summary>
    ''' Screen to allow a user to view the breakdown of visit cost for an individual domiciiary pro forma invoice.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      01/12/2008  D11444 - security overhaul.
    '''     MikeVO      ??/??/????  Created.
    ''' </history>
    Partial Class ViewInvoiceCostedVisits
        Inherits Target.Web.Apps.BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.PaymentSchedules"), "Pro forma Invoice - Costed Visits")
            Me.UseJQuery = True

            Dim visitID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("visitID"))
            Dim batchID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("batchID"))
            Dim invoiceID As Integer = Target.Library.Utils.ToInt32(Request.QueryString("id"))
            Dim pScheduleId As Integer = Target.Library.Utils.ToInt32(Request.QueryString("pScheduleId"))
            Dim invFilterAwait As String = Target.Library.Utils.ToString(Request.QueryString("await"))
            Dim invFilterVer As String = Target.Library.Utils.ToString(Request.QueryString("ver"))
            Dim backUrl As String = Target.Library.Utils.ToString(Request.QueryString("backUrl"))
            Dim contractHasRoundingRules As Boolean = ContractHasRounding(pScheduleId)

            Dim js As String
            Dim style As New StringBuilder
            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))

            If currentPage <= 0 Then currentPage = 1

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ProformaInvoice/ViewInvoiceCostedVisits.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomContract))
            style.Append("label.label { float:left; width:9em; font-weight:bold; }")
            style.Append("span.label { float:left; width:9em; padding-right:1em; font-weight:bold; }")
            Me.AddExtraCssStyle(style.ToString)

            If Not Me.IsPostBack Then
                Me.CustomNavAdd(False)
            End If

            js = String.Format("currentPage={0};" & _
                               "invoiceID={1};" & _
                               "selectedVisitID={2}; " & _
                               "batchID={3};" & _
                               "contractHasRoundingRules={4};", _
                               currentPage, _
                               invoiceID, _
                               visitID, _
                               batchID, _
                               contractHasRoundingRules.ToString().ToLower() _
                               )

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Me.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", _
                String.Format("dteCopyWeekEndingID=""{0}"";", dteCopyWeekEnding.ClientID), True)
        End Sub

        Private Function ContractHasRounding(ByVal pscheduleId As Integer) As Boolean
            ' if contract has rounding rules
            Dim msg As New ErrorMessage
            Dim pSchedule As DataClasses.PaymentSchedule = _
            New DataClasses.PaymentSchedule(Me.DbConnection, String.Empty, String.Empty)
            msg = pSchedule.Fetch(pscheduleId)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            Dim dcrContractList As DataClasses.Collections.DurationClaimedRoundingDomContractCollection = _
            New DataClasses.Collections.DurationClaimedRoundingDomContractCollection()
            Dim contractHasRoundingRules As Boolean = False
            msg = New ErrorMessage
            msg = DataClasses.DurationClaimedRoundingDomContract.FetchList(Me.DbConnection, dcrContractList, String.Empty, String.Empty, Nothing, pSchedule.DomContractID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If dcrContractList.Count > 0 Then
                contractHasRoundingRules = True
            End If

            Return contractHasRoundingRules
        End Function


        Private Sub btnBack_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.ServerClick
            Me.CustomNavRemoveLast()
            Me.CustomNavGoBack()
        End Sub


    End Class

End Namespace