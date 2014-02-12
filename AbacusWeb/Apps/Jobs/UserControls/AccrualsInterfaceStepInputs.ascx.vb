
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
    ''' User control that provides custom inputs for the purge service delivery data amendments job step.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 	John Finch   28/01/2011  Created (D11932)
    ''' </history>
    Partial Public Class AccrualsInterfaceStepInputs
        Inherits System.Web.UI.UserControl
        Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs

        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            '++ Add date utility javascript..
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            '++ Add utility javascript link..
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            '++ Add dialog javascript..
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            '++ Add list filter javascript..
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            '++ Add user control javascript..
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/Jobs/UserControls/AccrualsInterfaceStepInputs.js"))
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Const SCRIPT_STARTUP As String = "Startup"

            Dim js As StringBuilder = New StringBuilder()
            Dim msg As New ErrorMessage
            Dim lookupList As LookupCollection = Nothing
            Dim lastAccrualDate As Date = Date.Today
            Dim lastPeriodNumID As Integer = 0
            Dim lastFinancialYearID As Integer = 0
            Dim lastFilePath As String = ""

            cboFinancialYear.DropDownList.Attributes.Add("onchange", "cboFinancialYear_OnChange();")

            PopulateScreen()

            '++ Get the LOOKUP entries for the most recent entries used on an accruals run,
            '++ and use these to prime the field options..
            msg = Lookup.FetchList(CType(Me.Page, BasePage).DbConnection, lookupList, "ACCRUALRUN")
            If Not msg.Success Then WebUtils.DisplayError(msg)
            If msg.Success AndAlso lookupList.Count > 0 Then
                For Each lookupRec As Lookup In lookupList
                    Select Case lookupRec.Description
                        Case "LASTACCRUALDATE"
                            If Utils.IsDate(lookupRec.InfoString) Then
                                lastAccrualDate = Convert.ToDateTime(lookupRec.InfoString)
                            End If
                        Case "LASTPERIODNUM"
                            lastPeriodNumID = Utils.ToInt32(lookupRec.InfoValue)
                        Case "LASTFINANCIALYEAR"
                            lastFinancialYearID = Utils.ToInt32(lookupRec.InfoValue)
                        Case "LASTFILEPATH"
                            lastFilePath = Utils.ToString(lookupRec.Comment)
                    End Select
                Next
            End If

            '++ Add AJAX-generated javascript to the page..
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.Lookups))
            '++ Output the control IDs to the startup script..
            js.AppendFormat("dteAccrualDateID='{0}';", dteAccrualDate.ClientID)
            js.AppendFormat("cboPeriodNumID='{0}';", cboPeriodNum.ClientID)
            js.AppendFormat("cboFinancialYearID='{0}';", cboFinancialYear.ClientID)
            js.AppendFormat("txtFilePathID='{0}';", txtFilePath.ClientID)
            '++ Add the default fields values (if any)..
            js.AppendFormat("defAccrualDate='{0}';", lastAccrualDate.ToString("dd/MM/yyyy"))
            js.AppendFormat("defFinancialYear='{0}';", lastFinancialYearID.ToString)
            js.AppendFormat("defPeriodNum='{0}';", lastPeriodNumID.ToString)
            txtFilePath.Text = lastFilePath

            If Not Page.ClientScript.IsStartupScriptRegistered(Me.GetType(), SCRIPT_STARTUP) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType(), SCRIPT_STARTUP, _
                    js.ToString, _
                    True _
                )
            End If
        End Sub

        Private Sub PopulateScreen()
            Dim msg As New ErrorMessage
            Dim budgetYearList As BudgetYearCollection = Nothing
            Dim budgetPeriodList As BudgetPeriodCollection = Nothing

            '++ Populate a list of budget (i.e. financial) years..
            With cboFinancialYear
                msg = BudgetYear.FetchList(CType(Me.Page, BasePage).DbConnection, budgetYearList)
                If Not msg.Success Then WebUtils.DisplayError(msg)
                With .DropDownList
                    .Items.Clear()
                    For Each bYear As BudgetYear In budgetYearList
                        If bYear.IsFinancialYear = TriState.True Then
                            .Items.Add(New ListItem(bYear.Description, bYear.ID))
                        End If
                    Next
                    .Items.Insert(0, New ListItem(String.Empty, 0))
                End With
            End With
            cboFinancialYear.SelectPostBackValue()
        End Sub

        Public Function GetCustomInputs(ByVal conn As SqlConnection, ByVal trans As SqlTransaction, _
                ByVal jobStepTypeID As Integer) As List(Of Triplet) Implements Target.Abacus.Jobs.Core.ICustomJobStepInputs.GetCustomInputs

            Dim msg As New ErrorMessage
            Dim result As List(Of Triplet) = New List(Of Triplet)
            Dim txtValue As String, dtValue As String
            Dim financialYearID As Integer = 0
            Dim periodNumID As Integer = 0
            Dim lookupRec As Lookup = Nothing
            Dim lookupList As LookupCollection = Nothing
            Dim lookupAccrualDateID As Integer = 0
            Dim lookupFinancialYearID As Integer = 0
            Dim lookupPeriodNumberID As Integer = 0
            Dim lookupFilePathID As Integer = 0

            '++ Accrual date..
            dtValue = Request.Form(String.Format("{0}$txtTextBox", dteAccrualDate.UniqueID))
            result.Add(New Triplet(jobStepTypeID, "FilterAccrualDate", dtValue))
            '++ Financial Year..
            financialYearID = Utils.ToInt32(cboFinancialYear.GetPostBackValue())
            result.Add(New Triplet(jobStepTypeID, "FilterFinancialYearID", financialYearID))
            '++ Period number..
            periodNumID = Utils.ToInt32(cboPeriodNum.GetPostBackValue())
            result.Add(New Triplet(jobStepTypeID, "FilterPeriodNumberID", periodNumID))
            '++ File Path..
            txtValue = Request.Form(String.Format("{0}$txtTextBox", txtFilePath.UniqueID))
            result.Add(New Triplet(jobStepTypeID, "FilterFilePath", txtValue))

            '++ Write this custom control's settings back to the respective LOOKUP stores..
            If trans IsNot Nothing Then
                lookupRec = New Lookup(trans)
                msg = Lookup.FetchList(trans, lookupList, "ACCRUALRUN")
            ElseIf conn IsNot Nothing Then
                lookupRec = New Lookup(conn)
                msg = Lookup.FetchList(conn, lookupList, "ACCRUALRUN")
            Else
                lookupRec = New Lookup(CType(Me.Page, BasePage).DbConnection)
                msg = Lookup.FetchList(CType(Me.Page, BasePage).DbConnection, lookupList, "ACCRUALRUN")
            End If
            lookupRec.Type = "ACCRUALRUN"

            If msg.Success Then
                '++ Grab the existing LOOKUP IDs (if any)..
                If lookupList.Count > 0 Then
                    For Each lkRec As Lookup In lookupList
                        Select Case lkRec.Description
                            Case "LASTACCRUALDATE"
                                lookupAccrualDateID = lkRec.ID
                            Case "LASTPERIODNUM"
                                lookupPeriodNumberID = lkRec.ID
                            Case "LASTFINANCIALYEAR"
                                lookupFinancialYearID = lkRec.ID
                            Case "LASTFILEPATH"
                                lookupFilePathID = lkRec.ID
                        End Select
                    Next
                End If

                msg = lookupRec.Fetch(lookupAccrualDateID)
                With lookupRec
                    .Description = "LASTACCRUALDATE"
                    .ID = lookupAccrualDateID
                    .InfoString = dtValue
                    .InfoValue = 0
                    msg = .Save()
                End With
                With lookupRec
                    .Description = "LASTFILEPATH"
                    .ID = lookupFilePathID
                    .InfoString = ""
                    .InfoValue = 0
                    .Comment = txtValue
                    msg = .Save()
                End With
                With lookupRec
                    .Description = "LASTFINANCIALYEAR"
                    .ID = lookupFinancialYearID
                    .InfoString = ""
                    .InfoValue = financialYearID
                    .Comment = ""
                    msg = .Save()
                End With
                With lookupRec
                    .Description = "LASTPERIODNUM"
                    .ID = lookupPeriodNumberID
                    .InfoString = ""
                    .InfoValue = periodNumID
                    .Comment = ""
                    msg = .Save()
                End With
            End If

            Return result

        End Function
    End Class

End Namespace

