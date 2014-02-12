
Imports System.Data.SqlClient
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.GenericFinder

    ''' <summary>
    ''' Displays a generic record finder popup.
    ''' </summary>
    ''' <history>
    ''' MikeVO   31/10/2011 BTI128 - added support for dynamic column widths
    ''' Mo Tahir 04/08/2011 D11766 - Provider Invoice Tolerances
    ''' Mo Tahir 13/07/2010 D11798 - Budget Holders
    ''' JohnF   09/07/2010  D11794 - Added PersonalBudgetPercentageCharged
    ''' JohnF   30/06/2010  D11794 - Added BudgetCategory and BudgetCategoryGroup
    ''' Mo Tahir28/06/2010  D11829 - Licensing
    ''' ColinD  05/05/2010  D11756 - added ability to handle project codes GenericFinderType.ProjectCode
    ''' </history>
    <Idunno.AntiCsrf.SuppressCsrfCheck()> _
    Partial Class GenericFinder
        Inherits Target.Web.Apps.BasePage

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim msg As ErrorMessage
            Dim securityItemID As Integer = Utils.ToInt32(Request.QueryString("siID"))
            Dim genericFinderTypeID As GenericFinderType = [Enum].Parse(GetType(GenericFinderType), Utils.ToInt32(Request.QueryString("gftID")))
            Dim searchBy As String = Request.QueryString("searchBy")
            Dim searchFor As String = Request.QueryString("searchFor")
            Dim extraParams As String() = Nothing

            If Not Request.QueryString("param") Is Nothing Then
                extraParams = Request.QueryString("param").Split(",")
            End If

            Me.EnableTimeout = False
            Me.InitPage(securityItemID, "Find")
            Me.JsLinks.Add("GenericFinder.js")

            Dim finder As GenericFinderResultsBase = Me.Master.FindControl("MPContent").FindControl("genericFinder1")
            With finder
                msg = GetResults(genericFinderTypeID, searchBy, searchFor, finder, extraParams)
                If Not msg.Success Then WebUtils.DisplayError(msg)
            End With

        End Sub

        Private Function GetResults(ByVal genericFinderTypeID As GenericFinderType, _
                                    ByVal searchBy As String, _
                                    ByVal searchFor As String, _
                                    ByRef finder As GenericFinderResultsBase, _
                                    ByVal extraParams As String()) As ErrorMessage

            Const PAGING_LINK_TEMPLATE As String = "<a href=""javascript:GenericFinder_FetchPage({0})"" title=""{2}"">{1}</a>&nbsp;"
            Const PAGE_SIZE As Integer = 10

            Dim msg As ErrorMessage = New ErrorMessage
            Dim totalRecords As Integer
            Dim currentPage As Integer = Utils.ToInt32(Request.QueryString("page"))
            Dim spName As String

            If currentPage = 0 Then currentPage = 1

            Select Case genericFinderTypeID
                Case GenericFinderType.DomServiceType
                    spName = "spxDomServiceType_FetchListWithPaging"
                    With finder.ColumnWidths
                        .Add("Category", 25.0)
                        .Add("Classification", 17.0)
                        .Add("Service Group", 23.0)
                        .Add("Service Type", 25.0)
                        .Add("Redundant", 10.0)
                    End With

                Case GenericFinderType.DomTimeBandGroup
                    spName = "spxDomTimeBandGroup_FetchListWithPaging"
                Case GenericFinderType.DomVisitCodeGroup
                    spName = "spxDomVisitCodeGroup_FetchListWithPaging"
                Case GenericFinderType.DomEnhancedRateDay
                    spName = "spxDomEnhancedRateDay_FetchListWithPaging"
                Case GenericFinderType.DomDayCategory
                    spName = "spxDomDayCategory_FetchListWithPaging"
                Case GenericFinderType.DomRateFramework
                    spName = "spxDomRateFramework_FetchListWithPaging"
                    With finder.ColumnWidths
                        .Add("Description", 45.0)
                        .Add("Framework Type", 20.0)
                        .Add("Abbreviation", 15.0)
                        .Add("Enhanced", 10.0)
                        .Add("Redundant", 10.0)
                    End With

                Case GenericFinderType.DomRateCategory
                    spName = "spxDomRateCategory_FetchListWithPaging"
                Case GenericFinderType.DomContractGroup
                    spName = "spxGenericContractGroup_FetchListWithPaging"
                    With finder.ColumnWidths
                        .Add("Description", 40.0)
                        .Add("Usage", 50.0)
                        .Add("Redundant", 10.0)
                    End With

                Case GenericFinderType.ContractEndReason
                    spName = "spxContractEndReason_FetchListWithUsageDesc"
                Case GenericFinderType.DomManuallyAmendedIndicatorGroup
                    spName = "spxDomManuallyAmendedIndicatorGroup_FetchListWithPaging"
                Case GenericFinderType.DomContractReOpenedWeek
                    spName = "spxDomContractReOpenedWeek_FetchListWithPaging"
                Case GenericFinderType.DomServiceOrderEndReason
                    spName = "spxDomServiceOrderEndReason_FetchListWithPaging"
                Case GenericFinderType.RASOverrideReason
                    spName = "spxRASOverrideReason_FetchListWithPaging"
                Case GenericFinderType.RASType
                    spName = "spxRASType_FetchListWithPaging"
                Case GenericFinderType.DomUnitsOfMeasure
                    spName = "spxDomUnitsOfMeasure_FetchListWithPaging"
                    With finder.ColumnWidths
                        .Add("Description", 12.0)
                        .Add("Comment", 44.0)
                        .Add("Service Registers?", 17.0)
                        .Add("Minutes Per Unit", 15.0)
                        .Add("System Type", 12.0)
                    End With

                Case GenericFinderType.ServiceOrderSuspensionReason
                    spName = "spxServiceOrderSuspensionReason_FetchListWithPaging"
                Case GenericFinderType.OtherFundingOrganization
                    spName = "spxOtherFundingOrganization_FetchListWithPaging"
                Case GenericFinderType.ExpenditureAccount
                    spName = "spxExpenditureAccountGroup_FetchListWithPaging"
                Case GenericFinderType.AvailableJobType
                    spName = "spxvwAvailableJobType_FetchListWithPaging"
                Case GenericFinderType.ServiceGroup
                    spName = "spxServiceGroup_FetchListWithPaging"
                    With finder.ColumnWidths
                        .Add("Service Group", 43.0)
                        .Add("Service Category", 26.0)
                        .Add("Classification", 18.0)
                        .Add("Redundant", 13.0)
                    End With

                Case GenericFinderType.ServiceOutcomes
                    spName = "spxServiceOutcomeGroup_FetchListWithPaging"
                Case GenericFinderType.ReportCategories
                    spName = "spxWebReportCategory_FetchListWithPaging"
                Case GenericFinderType.ReportConfiguration
                    spName = "spxWebReport_FetchListWithPaging"
                Case GenericFinderType.ProjectCode
                    spName = "spxProjectCodes_FetchListWithPaging"
                Case GenericFinderType.WebModule
                    spName = "spxWebModule_FetchListWithPaging"
                Case GenericFinderType.SDS_BudgetCategory
                    spName = "spxBudgetCategory_FetchListWithPaging"
                Case GenericFinderType.SDS_BudgetCategoryGroup
                    spName = "spxBudgetCategoryGroup_FetchListWithPaging"
                Case GenericFinderType.SDS_PersonalBudgetPercentageCharged
                    spName = "spxPersonalBudgetPercentageCharged_FetchListWithPaging"
                Case GenericFinderType.BudgetHolder
                    spName = "spxBudgetHolder_FetchListWithPaging"
                Case GenericFinderType.ServiceUserBudgetPeriod
                    spName = "spxServiceUserBudgetPeriod_FetchListWithPaging"
                Case GenericFinderType.DirectPaymentContract
                    spName = "spxDPContract_FetchListWithPaging"
                Case GenericFinderType.SpendPlanEndReason
                    spName = "spxSpendPlanEndReason_FetchListWithPaging"
                Case GenericFinderType.BenefitRates
                    spName = "spxBenefitRates_FetchListWithPaging"
                Case GenericFinderType.NonResidentialParameters
                    spName = "spxNonResidentialParameters_FetchListWithPaging"
                Case GenericFinderType.NonResidentialIsAllowanceRates
                    spName = "spxNonResidentialIsAllowanceRates_FetchListWithPaging"
                Case GenericFinderType.NonResidentialIsAllowanceComponents
                    spName = "spxNonResidentialIsAllowanceComponents_FetchListWithPaging"
                Case GenericFinderType.OutgoingMaximumAmounts
                    spName = "spxOutgoingMaximumAmounts_FetchListWithPaging"
                Case GenericFinderType.DocumentType
                    spName = "spxDocumentTypes_FetchListWithPaging"
                Case GenericFinderType.AssessmentBands
                    spName = "spxSDSAssessmentBand_FetchListWithPaging"
                Case GenericFinderType.AssessmentBandRates
                    spName = "spxSDSAssessmentBandRates_FetchListWithPaging"
                Case GenericFinderType.DurationClaimedRounding
                    spName = "spxDurationClaimedRoundingFind_FetchListWithPaging"
                Case GenericFinderType.DocumentPrinters
                    spName = "spxDocumentPrintersFind_FetchListWithPaging"
                Case GenericFinderType.NoteCategory
                    spName = "spxNoteCategory_FetchListWithPaging"
                Case GenericFinderType.EmailTemplates
                    spName = "spxEmailTemplate_FetchListWithPaging"
                Case GenericFinderType.PaymentToleranceGroup
                    spName = "spxPaymentToleranceGroup_FetchListWithPaging"
                Case GenericFinderType.MovementRequestRejectionReason
                    spName = "spxMovementRequestRejectionReason_FetchListWithPaging"
                Case GenericFinderType.ProcessSubStatuses
                    spName = "spxProcessSubStatus_FetchListWithPaging"
                Case GenericFinderType.AdministrativeAreas
                    spName = "spxAdministrativeArea_FetchListWithPaging"
                Case Else
                    Throw New ArgumentOutOfRangeException("GenericFinderType", genericFinderTypeID, "Unsupported finder type specified.")

            End Select

            msg = GetData(spName, searchBy, searchFor, currentPage, PAGE_SIZE, finder.Results, totalRecords, extraParams)
            If Not msg.Success Then Return msg
            finder.PagingLinks = WebUtils.BuildPagingLinks(PAGING_LINK_TEMPLATE, currentPage, Math.Ceiling(totalRecords / PAGE_SIZE))

            msg.Success = True
            Return msg

        End Function

        Private Function GetData(ByVal spName As String, ByVal searchBy As String, ByVal searchFor As String, _
              ByVal currentPage As Integer, ByVal pageSize As Integer, _
              ByRef dt As DataTable, ByRef totalRecords As Integer, ByVal extraParams As String()) As ErrorMessage

            Const EXTRA_PARAM_INDEX_START As Integer = 5

            Dim msg As ErrorMessage = New ErrorMessage
            Dim ds As DataSet = Nothing

            Try
                ' create SP parameters
                Dim spParams As SqlParameter() = SqlHelperParameterCache.GetSpParameterSet(Me.DbConnection, spName, False)
                spParams(0).Value = searchBy
                spParams(1).Value = searchFor
                spParams(2).Value = currentPage
                spParams(3).Value = 10
                If Not extraParams Is Nothing Then
                    For index As Integer = 0 To extraParams.Length - 1
                        spParams(EXTRA_PARAM_INDEX_START + index).Value = extraParams(index)
                    Next
                End If

                ' execute
                ds = SqlHelper.ExecuteDataset(Me.DbConnection, CommandType.StoredProcedure, spName, spParams)
                dt = ds.Tables(0)

                ' get record count
                totalRecords = Utils.ToInt32(spParams(4).Value)

                msg.Success = True

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0501", spName, "GetData()")     ' error reading data
            End Try

            Return msg

        End Function

    End Class

End Namespace