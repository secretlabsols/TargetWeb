Imports Target.Abacus.Library.RequestPayments
Imports System.Collections.Generic
Imports Target.Abacus.Library.DataClasses
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web
Imports Target.Library.Web.UserControls
Imports Target.Library
Imports System.Text
Imports Target.Abacus.Library.DataClasses.Collections
Imports System.Web.Script.Serialization
Imports Target.Abacus.Library
Imports Target.Web.Apps.Security

Namespace Apps.Dom.PaymentSchedules

    Partial Public Class RequestPayment
        Inherits Target.Web.Apps.BasePage

        Private _ContractList As List(Of ViewableContractList) = Nothing

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.RequestPayments"), "Request Payments")

            ' setup js/reports
            SetupJavaScript()

            txtPayUpTo.AllowableDays = CInt(System.Enum.Parse(GetType(DayOfWeek), DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing).DayOfWeek))

            If Utils.ToBoolean(Me.Settings(ApplicationName.AbacusIntranet, "PreventEntryOfActualServiceForFuturePeriods")) Then

                txtPayUpTo.MaximumValue = DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing)

            End If

            If Not Me.IsPostBack Then
                txtPayUpTo.Text = DomContractBL.GetWeekEndingDate(Me.DbConnection, Nothing)
            End If


        End Sub

        Private Sub SetupJavaScript()

            ' add in js link for handlers
            JsLinks.Add("RequestPayment.js")

            ' add utility js link
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            ' add dialog js
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))

            ' add reports js
            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/HiddenReportStep.js"))

            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery ui library for popups and table filtering etc
            UseJqueryUI = True

            ' add in the table filter library 
            UseJqueryTableFilter = True

            ' add the table scroller library as we might have large amounts of data
            UseJqueryTableScroller = True

            ' add the searchable menu
            UseJquerySearchableMenu = True

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.RequestPayments))

        End Sub

        ''' <summary>
        ''' Gets the Payment request records.
        ''' </summary>
        ''' <value>The register client statuses.</value>
        Private ReadOnly Property RequestPayments() As List(Of ViewableContractList)
            Get
                If _ContractList Is Nothing Then
                    Dim user As WebSecurityUser
                    user = SecurityBL.GetCurrentUser()
                    ' if we havent fetched the items then do so, throw error if encountered
                    Dim msg As ErrorMessage = RequestPaymentBL.GetContractList(DbConnection, Convert.ToDateTime(txtPayUpTo.Text), user.ExternalUserID, user.ID, _ContractList)
                    If Not msg.Success Then WebUtils.Utils.DisplayError(msg)
                End If
                Return _ContractList
            End Get
        End Property

    End Class

End Namespace