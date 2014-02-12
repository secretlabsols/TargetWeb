
Imports System.Collections.Generic
Imports System.Configuration.ConfigurationManager
Imports System.Data.SqlClient
Imports System.Text
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Library.Web.Controls
Imports Target.Web.Apps.Security

Namespace Apps.Dom.ProviderInvoice

    ''' <summary>
    ''' Screen to allow a user to view the lines on a domiciliary provider invoice.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir     09/03/2011  A4WA6695 - leaking connections
    '''     MikeVO      16/10/2009  D11546 - created.
    ''' </history>
    Partial Class ViewInvoiceLines
        Inherits Target.Web.Apps.BasePage

        Private _dpi As DomProviderInvoiceBL
        Private _InvoiceHasNotes As Boolean = False
        Private _pScheduleId As Integer

        Private _ShowNonDelivery As Boolean = True
        Public Property ShowNonDelivery() As Boolean
            Get
                Return _ShowNonDelivery
            End Get
            Set(ByVal value As Boolean)
                _ShowNonDelivery = value
            End Set
        End Property




        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusExtranet.WebNavMenuItem.ProviderInvoiceEnquiry"), "Provider Invoice - Invoice Lines")
            Me.UseJQuery = True
            Me.UseJqueryUI = True
            Dim invoiceID As Integer = Utils.ToInt32(Request.QueryString("id"))
            If invoiceID > 0 Then
                CType(ctrlInvoiceNotes, Apps.UserControls.ViewProviderInvoiceNotes).InvoiceID = invoiceID
                _InvoiceHasNotes = CType(ctrlInvoiceNotes, Apps.UserControls.ViewProviderInvoiceNotes).InvoiceHasNotes
            End If

            _pScheduleId = Utils.ToInt32(Request.QueryString("pScheduleId"))
            Dim estabID As Integer = Utils.ToInt32(Request.QueryString("estabID"))
            Dim style As New StringBuilder
            Dim sysInfo As SystemInfo
            Dim currentUser As WebSecurityUser
            Dim msg As ErrorMessage

            If Not Me.IsPostBack Then
                Me.CustomNavAdd(False)
            End If

            CType(headerDetails, UserControls.DomProviderInvoiceHeaderDetails).InvoiceID = invoiceID

            With style
                .Append("label.label { float:left; width:10em; font-weight:bold; }")
                .Append("span.label { float:left; width:10em; padding-right:1em; font-weight:bold; }")
                .Append("td.detailCell {vertical-align:top;} ")
                .Append("td.headerGroup {text-align:center;border-width:0px;} ")
                .Append("td.plannedCell {background-color: #DFEAED !important;} ")
                .Append("td.otherInvoiceCell {background-color:#fffea6;} ")
                .Append("th.header {padding-bottom:0px;} ")
                .Append("th.right, td.right {text-align:right;} ")
                Me.AddExtraCssStyle(.ToString())
            End With



            sysInfo = DataCache.SystemInfo(Me.DbConnection, Nothing, False)
            currentUser = SecurityBL.GetCurrentUser()

            ' fetch the invoice and render the invoice lines
            _dpi = New DomProviderInvoiceBL( _
                ConnectionStrings("Abacus").ConnectionString, _
                sysInfo.LicenceNo, _
                currentUser.ExternalUsername, _
                currentUser.ExternalUserID, _
                AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings) _
            )
            msg = _dpi.Fetch(invoiceID, estabID)
            If Not msg.Success Then
                WebUtils.DisplayError(msg)
                Exit Sub
            End If
            SetNondeliveryColumnVisibility(_dpi)

            msg = PopulateDetailLines(_dpi.InvoiceStyle, _
                                      _dpi.DomContractID, _
                                      _dpi.DetailLines, _
                                      _dpi.ID _
                                      )

            If Not msg.Success Then WebUtils.DisplayError(msg)

            IncludeJavaScript()

        End Sub

#Region " Include Javascript "
        Private Sub IncludeJavaScript()

            ' add in the jquery library
            UseJQuery = True

            ' add in the jquery ui library for popups and table filtering etc
            UseJqueryUI = True

            ' add in the table filter library 
            UseJqueryTableFilter = True

            ' add the table scroller library as we might have large aCustomNavGoBackunts of data
            UseJqueryTableScroller = True

            ' add the searchable menu
            UseJquerySearchableMenu = True

            ' add the jquery tooltip
            UseJqueryTooltip = True

            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/NonDelivery/NonDeliveryUnitBasedDialog.js"))

            JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Apps/NonDelivery/NonDeliveryVisitBasedDialog.js"))

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomProviderInvoice))

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.DomProfomaInvoice))

            ' add the ajax web svc tools
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.PaymentSchedule))

            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/Dom/ProviderInvoice/ViewInvoiceLines.js"))
            Dim js As String
            js = String.Format("InvoiceHasNotes=""{0}"";", _InvoiceHasNotes.ToString().ToLower())

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

#End Region

        ''' <summary>
        ''' the ‘Non-Delivery’ column will not be displayed if the Framework Type is ‘Community General’ 
        ''' and none of the Contract Periods (of the Contract to which the Provider Invoice relates) 
        ''' have a Service Outcome Group or a Visit Code Group recorded against them
        ''' </summary>
        ''' <param name="_dpi">object of DomProviderInvoiceBL</param>
        ''' <remarks></remarks>
        Public Sub SetNondeliveryColumnVisibility(ByVal _dpi As DomProviderInvoiceBL)
            If _dpi.FrameWorkType = FrameworkTypes.CommunityGeneral Then
                Dim dcPeriodList As DataClasses.Collections.DomContractPeriodCollection = _
                    New DataClasses.Collections.DomContractPeriodCollection()
                Dim msg As New ErrorMessage
                msg = DataClasses.DomContractPeriod.FetchList(conn:=Me.DbConnection, _
                                                        list:=dcPeriodList, _
                                                        auditUserName:=String.Empty, _
                                                        auditLogTitle:=String.Empty, _
                                                        domContractID:=_dpi.DomContractID)

                If Not msg.Success Then WebUtils.DisplayError(msg)
                Dim hasServiceOutcomeGroupOrVisitCodeGroup As Boolean = False


                For Each dcPeriod As DomContractPeriod In dcPeriodList
                    If dcPeriod.VisitCodeGroupID > 0 Or dcPeriod.ServiceOutcomeGroupID > 0 Then
                        hasServiceOutcomeGroupOrVisitCodeGroup = True
                    End If
                Next

                ShowNonDelivery = hasServiceOutcomeGroupOrVisitCodeGroup

            End If

        End Sub


#Region " PopulateDetailLines "

        Private Function PopulateDetailLines(ByVal invoiceStyle As DomProviderInvoiceStyle, _
                                            ByVal domContractID As Integer, _
                                            ByVal detailLines As List(Of DetailLine), _
                                            ByVal invoiceId As Integer) As ErrorMessage

            Dim msg As ErrorMessage
            Dim row As TableRow
            Dim cell As TableCell

            If invoiceStyle = DomProviderInvoiceStyle.ManualPayment Then
                pnlDetailsSummaryVisitLevel.Visible = False
                pnlDetailsSummaryLevelExtranet.Visible = False

                For Each dl As DetailLine In detailLines
                    With dl

                        row = New TableRow()
                        phDetailsManualPayment.Controls.Add(row)

                        ' week ending
                        cell = New TableCell()
                        row.Cells.Add(cell)
                        cell.Text = .WeekEnding.ToString("dd/MM/yyyy")

                        ' description
                        cell = New TableCell()
                        row.Cells.Add(cell)
                        cell.Text = .Comment

                        ' cost
                        cell = New TableCell()
                        row.Cells.Add(cell)
                        cell.CssClass = "right"
                        cell.Text = .ThisInvoiceCost.ToString("F2")

                    End With
                Next
            ElseIf invoiceStyle = DomProviderInvoiceStyle.SummaryLevelExtranet Then
                pnlDetailsManualPayment.Visible = False
                pnlDetailsSummaryVisitLevel.Visible = False

                For detailIndex As Integer = 0 To detailLines.Count - 1
                    OutputDetailControlsSummaryLevelExtranet(detailIndex.ToString(), detailLines(detailIndex), invoiceId)
                Next
            Else
                'invoiceStyle = DomProviderInvoiceStyle.VisitLevel 'Then
                pnlDetailsManualPayment.Visible = False
                pnlDetailsSummaryLevelExtranet.Visible = False

                For detailIndex As Integer = 0 To detailLines.Count - 1
                    OutputDetailControls(detailIndex.ToString(), detailLines(detailIndex))
                Next
            End If

            msg = New ErrorMessage()
            msg.Success = True
            Return msg

        End Function

#End Region

#Region " Output Deatil Controls For Summary Level Extranet "

        Private Sub OutputDetailControlsSummaryLevelExtranet(ByVal uniqueID As String, _
                                                             ByVal detail As DetailLine, _
                                                             ByVal invoiceId As Integer)

            Dim row As TableRow
            Dim cell As TableCell
            'Dim txt As TextBoxEx

            row = New TableRow()
            row.ID = uniqueID
            phDetailsSummaryLevelExtranet.Controls.Add(row)

            ' week ending
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell"
            cell.Controls.Add(CreateSpan(detail.WeekEnding.ToString("dd/MM/yyyy")))

            ' rate category
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell"
            cell.Controls.Add(CreateSpan(detail.RateCategoryDescription))

            ' planned units 
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell plannedCell left"
            cell.Controls.Add(CreateSpan(detail.PlannedUnits.ToString("F2")))
            cell.Controls.Add(CreateSpan(" "))
            cell.Controls.Add(CreateSpan(detail.MeasuredIn))


            ' planned rate
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell plannedCell right"
            cell.Controls.Add(CreateSpan(detail.PlannedRate.ToString("F2")))

            ' planned cost
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell plannedCell right"
            cell.Controls.Add(CreateSpan(detail.PlannedCost.ToString("F2")))



            ' this invoice delivered
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell right"
            cell.Controls.Add(CreateSpan(detail.DeliveredUnits.ToString("F2")))

            If ShowNonDelivery Then
                Dim numberOfNonDeliveryMsg As String = String.Format("{0}{1}", detail.NonDeliveryUnitsPaid.ToString("F2"), IIf(detail.NumberOfNonDeliveryItems > 0, String.Format(" ({0})", detail.NumberOfNonDeliveryItems), ""))
                ' this invoice Nondelivery
                cell = New TableCell()
                row.Cells.Add(cell)
                cell.CssClass = "detailCell right"
                ''
                If detail.NonDeliveryDataPresent Then
                    Dim hlink As New HyperLink
                    hlink.ID = "hlink_" & detail.ID
                    hlink.Text = numberOfNonDeliveryMsg
                    hlink.Attributes.Add("onclick", _
                                         String.Format("javascript:ShowDeliverydetail(""{0}"",""{1}"",""{2}"",{3},{4},""{5}"");", _
                                                       _pScheduleId, _
                                                       invoiceId, _
                                                       detail.ID, _
                                                       detail.PlannedUnits, _
                                                       detail.PlannedRate, _
                                                       detail.TimeBased.ToString().ToLower()))
                    cell.Controls.Add(hlink)
                Else
                    cell.Controls.Add(CreateSpan(numberOfNonDeliveryMsg))
                End If

            End If

            ' this invoice rate
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell right"
            cell.Controls.Add(CreateSpan(detail.ThisInvoiceRate.ToString("F2")))

            ' this invoice cost
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell right"
            cell.Controls.Add(CreateSpan(detail.ThisInvoiceCost.ToString("F2")))

        End Sub

#End Region

#Region " OutputDetailControls "

        Private Sub OutputDetailControls(ByVal uniqueID As String, ByVal detail As DetailLine)

            Dim row As TableRow
            Dim cell As TableCell
            Dim txt As TextBoxEx

            row = New TableRow()
            row.ID = uniqueID
            phDetailsSummaryVisitLevel.Controls.Add(row)

            ' week ending
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell"
            cell.Controls.Add(CreateSpan(detail.WeekEnding.ToString("dd/MM/yyyy")))

            ' rate category
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell"
            cell.Controls.Add(CreateSpan(detail.RateCategoryDescription))

            ' planned units 
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell plannedCell left"
            cell.Controls.Add(CreateSpan(detail.PlannedUnits.ToString("F2")))
            cell.Controls.Add(CreateSpan(" "))
            cell.Controls.Add(CreateSpan(detail.MeasuredIn))


            ' planned rate
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell plannedCell right"
            cell.Controls.Add(CreateSpan(detail.PlannedRate.ToString("F2")))

            ' planned cost
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell plannedCell right"
            cell.Controls.Add(CreateSpan(detail.PlannedCost.ToString("F2")))

            ' other invoice units
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell otherInvoiceCell right"
            cell.Controls.Add(CreateSpan(detail.OtherInvoiceUnits.ToString("F2")))

            ' other invoice cost
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell otherInvoiceCell right"
            cell.Controls.Add(CreateSpan(detail.OtherInvoiceCost.ToString("F2")))

            ' this invoice units
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell right"
            txt = CreateUnitsField(uniqueID, detail.MeasuredInDisplayAsHoursMins, detail.ThisInvoiceUnits, detail.MeasuredInMinutesPerUnit)
            cell.Controls.Add(txt)

            ' this invoice rate
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell right"
            cell.Controls.Add(CreateSpan(detail.ThisInvoiceRate.ToString("F2")))

            ' this invoice cost
            cell = New TableCell()
            row.Cells.Add(cell)
            cell.CssClass = "detailCell right"
            cell.Controls.Add(CreateSpan(detail.ThisInvoiceCost.ToString("F2")))

        End Sub

#End Region

#Region " CreateSpan "

        Private Function CreateSpan(ByVal value As String) As HtmlGenericControl

            Dim span As HtmlGenericControl
            span = New HtmlGenericControl("span")
            span.InnerText = value
            Return span

        End Function

#End Region

#Region " CreateUnitsField "

        Private Function CreateUnitsField(ByVal uniqueID As String, _
                                          ByVal displayAsHoursMins As Boolean, _
                                          ByVal units As Decimal, _
                                          ByVal minutes As Integer) As Control

            Dim txt As TextBoxEx


            'If displayAsHoursMins Then

            'Else

            txt = New TextBoxEx()
            With txt
                .ID = uniqueID
                .Format = TextBoxEx.TextBoxExFormat.CurrencyFormat
                .Width = New Unit(3, UnitType.Em)
                .Text = units.ToString("F2")
                .IsReadOnly = True
            End With
            Return txt

            'End If

        End Function

#End Region

#Region " Page_Unload "

        Private Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
            If Not _dpi Is Nothing Then
                _dpi.Dispose()
            End If
        End Sub

#End Region

        Private Sub btnBack_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnBack.Click
            Me.CustomNavRemoveLast()
            Me.CustomNavGoBack()
        End Sub

     
    End Class

End Namespace