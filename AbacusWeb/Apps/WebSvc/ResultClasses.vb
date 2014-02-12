
Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DomProviderInvoice
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.DataClasses

Namespace Apps.WebSvc

#Region " FetchEstablishmentsListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchEstablishmentsList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	08/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchEstablishmentsListResult
        Public ErrMsg As ErrorMessage
        Public Establishments As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchClientsListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchClientsListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	09/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchClientsListResult
        Public ErrMsg As ErrorMessage
        Public Clients As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchRateCategoryListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchRateCategoryListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	09/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchRateCategoryListResult
        Public ErrMsg As ErrorMessage
        Public Categories As vwDomRateCategoryCollection
    End Class

#End Region

#Region " StringResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.StringResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for methods that return a single string.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	10/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class StringResult
        Public ErrMsg As ErrorMessage
        Public Value As String
    End Class

#End Region

#Region " FetchDomContractListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDomContractList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomContractListResult
        Public ErrMsg As ErrorMessage
        Public Contracts As List(Of ViewableDomContract)
        Public PagingLinks As String
    End Class

#End Region

#Region " ViewablePairListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.ViewablePairListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for methods that return a List(Of ViewablePair).
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	23/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ViewablePairListResult
        Public ErrMsg As ErrorMessage
        Public List As List(Of ViewablePair)
    End Class

#End Region

#Region " FetchDomServiceOrderListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDomServiceOrderList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomServiceOrderListResult
        Public ErrMsg As ErrorMessage
        Public Orders As List(Of ViewableDomServiceOrder)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDomProviderInvoiceBatchListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDomProviderInvoiceBatchList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProviderInvoiceBatchListResult
        Public ErrMsg As ErrorMessage
        Public Batches As List(Of ViewableDomProviderInvoiceBatch)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDomProviderContractsByBatchResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDomProviderContractsByBatchResult() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProviderContractsByBatchResult
        Public ErrMsg As ErrorMessage
        Public Contracts As List(Of ViewableDomProviderInvoiceBatchContracts)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDomProviderInvoiceListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDomProviderInvoiceList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProviderInvoiceListResult
        Public ErrMsg As ErrorMessage
        Public Invoices As List(Of ViewableDomProviderInvoice)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDomProviderInvoiceListCountResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDomProviderInvoiceList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProviderInvoiceListCountResult
        Public ErrMsg As ErrorMessage
        Public Invoices As List(Of ViewableDomProviderInvoiceListCount)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDebtorInvoiceBatchListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDebtorInvoiceBatchList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDebtorInvoiceBatchListResult
        Public ErrMsg As ErrorMessage
        Public Batches As List(Of ViewableDebtorInvoiceBatch)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDebtorInvoiceListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDebtorInvoiceList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDebtorInvoiceListResult
        Public ErrMsg As ErrorMessage
        Public Invoices As List(Of ViewableDebtorInvoice)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDebtorInvoiceListCountResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDebtorInvoiceListCount() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDebtorInvoiceListCountResult
        Public ErrMsg As ErrorMessage
        Public Invoices As List(Of ViewableDebtorInvoiceListCount)
        Public PagingLinks As String
    End Class

#End Region

#Region " DebtorInvoiceToggleExcludeResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDebtorInvoiceList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DebtorInvoiceToggleExcludeResult
        Public ErrMsg As ErrorMessage
        Public Invoices As List(Of ViewableDebtorInvoiceToggleExclude)
        Public PagingLinks As String
    End Class

#End Region

#Region " DomProviderInvoiceToggleExcludeResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDomProviderInvoiceList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DomProviderInvoiceToggleExcludeResult
        Public ErrMsg As ErrorMessage
        Public Invoices As List(Of ViewableDomProviderInvoiceToggleExclude)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchPctListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchPctListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchPctList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	28/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchPctListResult
        Public ErrMsg As ErrorMessage
        Public Pcts As List(Of ViewablePct)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchRegisterListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchRegisterList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchRegisterListResult
        Public ErrMsg As ErrorMessage
        Public Registers As List(Of ViewableRegister)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchVwRegisterWeekListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchRegisterList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchVwRegisterWeekListResult
        Public ErrMsg As ErrorMessage
        Public vwRegisterWeekRows As List(Of ViewableVwRegisterWeek)
    End Class

#End Region

#Region " FetchCareManagerListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchCareManagerListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchCareManagerList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	28/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchCareManagerListResult
        Public ErrMsg As ErrorMessage
        Public CareManagers As List(Of ViewableCareManager)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchTeamListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchTeamListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchTeamList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	28/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchTeamListResult
        Public ErrMsg As ErrorMessage
        Public Teams As List(Of ViewableTeam)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchClientGroupListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchClientGroupListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchClientGroupList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	29/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchClientGroupListResult
        Public ErrMsg As ErrorMessage
        Public ClientGroups As List(Of ViewableClientGroup)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchClientSubGroupListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchClientSubGroupListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple class to provide results for the FetchClientSubGroupList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	JohnF    16/05/2013    Initial version (D12479)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchClientSubGroupListResult
        Public ErrMsg As ErrorMessage
        Public ClientSubGroups As List(Of ViewableClientSubGroup)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchManualPaymentDomProformaListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchManualPaymentDomProformaListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchManualPaymentDomProformaList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	07/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchManualPaymentDomProformaListResult
        Public ErrMsg As ErrorMessage
        Public Invoices As List(Of ManualPaymentDomProformaInvoiceEnquiryResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " GetUnitCostResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.GetUnitCostResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for GetUnitCost() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	09/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class GetUnitCostResult
        Public ErrMsg As ErrorMessage
        Public UnitCost As Decimal
        Public RowID As String
    End Class

#End Region

#Region " FetchFinanceCodeListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchFinanceCodeListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchFinanceCodeList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	09/04/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchFinanceCodeListResult
        Public ErrMsg As ErrorMessage
        Public FinanceCodes As List(Of ViewableFinanceCode)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchServiceGroupListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchServiceGroupListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchServiceGroupList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mo Tahir]	13/08/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchServiceGroupListResult
        Public ErrMsg As ErrorMessage
        Public ServiceGroups As List(Of ViewableServiceGroup)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchRegisterGroupListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchRegisterGroupListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchRegisterGroupList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Paul Wheaver]	19/03/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchRegisterGroupListResult
        Public ErrMsg As ErrorMessage
        Public RegisterGroups As List(Of ViewableRegisterGroup)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchReOpenedWeekListResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchReOpenedWeekList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchReOpenedWeekListResult
        Public ErrMsg As ErrorMessage
        Public Weeks As List(Of ReOpenedWeekResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " BooleanResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.StringResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for methods that return a single boolean value.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	19/05/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class BooleanResult
        Public ErrMsg As ErrorMessage
        Public Value As Boolean
    End Class

#End Region

#Region " FetchPersonalBudgetEnquiryListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchPersonalBudgetEnquiryListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchPersonalBudgetEnquiryList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	10/07/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchPersonalBudgetEnquiryListResult
        Public ErrMsg As ErrorMessage
        Public Budgets As List(Of PersonalBudgetEnquiryResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " DecimalResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.DecimalResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for methods that return a decimal value.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	08/07/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DecimalResult
        Public ErrMsg As ErrorMessage
        Public Value As Decimal
    End Class

#End Region

#Region " FetchIndicativeEnquiryListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchIndicativeEnquiryListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchIndicativeEnquiryListResult() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     ColinD      11/10/2010  renamed from FetchIndicativeEnquiryListResult to FetchIndicativeEnquiryListResult
    ''' 	[Mikevo]	07/07/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchIndicativeEnquiryListResult
        Public ErrMsg As ErrorMessage
        Public IndicativeBudgets As List(Of IndicativeBudgetEnquiryResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchThirdPartyListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchThirdPartyListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Paul]	06/02/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchThirdPartyListResult
        Public ErrMsg As ErrorMessage
        Public ThirdPartys As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchServiceOrderSuspensionPeriodListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchServiceOrderSuspensionPeriodList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchServiceOrderSuspensionPeriodListResult
        Public ErrMsg As ErrorMessage
        Public Suspensions As List(Of ViewableServiceOrderSuspensionPeriod)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchOtherFundingOrganizationListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchOtherFundingOrganizationListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Paul]	12/02/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchOtherFundingOrganizationListResult
        Public ErrMsg As ErrorMessage
        Public Organizations As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchExpenditureAccountListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchExpenditureAccountListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Paul]	18/02/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchExpenditureAccountListResult
        Public ErrMsg As ErrorMessage
        Public Accounts As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " StringListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.StringListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for methods that return a list of strings.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	17/02/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class StringListResult
        Public ErrMsg As ErrorMessage
        Public Values As List(Of String)
        Public Tag As String
    End Class

#End Region

#Region " CreateDomCreditorsInterfaceResult "

    ''' <summary>
    ''' Simple class to return the results of the CreateDomCreditorsInterface() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CreateDomCreditorsInterfaceResult
        Public ErrMsg As ErrorMessage
        Public InterfaceID As Integer = -1
    End Class

#End Region

#Region " FetchDomProviderInvoiceVisitsEnquiryListResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchDomProviderInvoiceVisitEnquiryResults() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProviderInvoiceVisitsEnquiryListResult
        Public ErrMsg As ErrorMessage
        Public Visits As List(Of DomProviderInvoiceVisitsEnquiryIntranetResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDomProviderInvoiceNotesResult "
    Public Class FetchDomProviderInvoiceNotesResult
        Public errMsg As ErrorMessage
        Public Notes As List(Of DomProviderInvoiceNotesEx)
    End Class
#End Region

#Region " GetDomProviderInvoiceNoteTextResult "
    Public Class GetDomProviderInvoiceNoteTextResult
        Public errMsg As ErrorMessage
        Public note As String
    End Class
#End Region

#Region " FetchJobScheduleListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchJobScheduleListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchJobScheduleList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	23/06/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchJobScheduleListResult
        Public ErrMsg As ErrorMessage
        Public JobSchedules As List(Of ViewableJobSchedule)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchServiceOutcomeListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchServiceOutcomeListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[MoTahir]	17/11/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchServiceOutcomeListResult
        Public ErrMsg As ErrorMessage
        Public ServiceOutcomes As List(Of ViewableServiceOutcome)
    End Class

#End Region

#Region " FetchRatePreclusionListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchRatePreclusionListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[MoTahir]	20/01/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchRatePreclusionListResult
        Public ErrMsg As ErrorMessage
        Public RatePreclusions As List(Of ViewableRatePreclusion)
    End Class

#End Region

#Region " FetchRateFrameworkListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchRateFrameworkListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mo]	16/03/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchRateFrameworkListResult
        Public ErrMsg As ErrorMessage
        Public RateFrameworks As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchBudgetHolderListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchBudgetHolderList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchBudgetHolderListResult
        Public ErrMsg As ErrorMessage
        Public BudgetHolders As List(Of ViewableBudgetHolder)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDPContractListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchDPContractList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDPContractListResult
        Public ErrMsg As ErrorMessage
        Public Contracts As List(Of ViewableDPContract)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchBudgetPeriodListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchBudgetPeriodListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for the FetchBudgetPeriodList() method.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	JohnF   27/01/2011    Created (D11932)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchBudgetPeriodListResult
        Public ErrMsg As ErrorMessage
        Public BudgetPeriods As List(Of ViewableBudgetPeriod)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDocumentsListResult "
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchDocumentsListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for fetching 'ViewableDocument' methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	Iftikhar	11/05/2011	SDS Issue #659 - added CurrPage
    ''' 	Iftikhar	28/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchDocumentsListResult
        Public ErrMsg As ErrorMessage
        Public Documents As List(Of ViewableDocument)
        Public PagingLinks As String
        Public CurrPage As Integer
    End Class

#End Region

#Region " FetchDocumentPrintQueueBatchesResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchDocumentPrintQueueBatchesResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for fetching 'ViewableDocumentPrintQueueBatch' methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	IHS		13/04/2011	D11960 - Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchDocumentPrintQueueBatchesResult
        Public ErrMsg As ErrorMessage
        Public PrintQueueBatches As List(Of ViewableDocumentPrintQueueBatch)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchPersonalBudgetStatementsListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchPersonalBudgetStatementsListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for fetching PersonalBudgetStatement list.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Iftikhar]	02/03/2011	D11854 - Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchPersonalBudgetStatementsListResult
        Public ErrMsg As ErrorMessage
        Public PersonalBudgetStatements As List(Of ViewablePersonalBudgetStatement)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDocumentPrinterPaperSourceListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchDocumentPrinterPaperSourceListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for fetching DocumentPrinterPaperSource list.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[IHS]	25/03/2011	D11960 - Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchDocumentPrinterPaperSourceListResult
        Public ErrMsg As ErrorMessage
        Public PrinterCanDuplex As Boolean
        Public PrinterPaperSources As List(Of DocumentPrinterPaperSource)
    End Class

#End Region

#Region " FetchDocumentPrinterPaperSizeListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchDocumentPrinterPaperSizeListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for fetching DocumentPrinterPaperSize list.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[IHS]	25/03/2011	D11960 - Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchDocumentPrinterPaperSizeListResult
        Public ErrMsg As ErrorMessage
        Public PrinterPaperSizes As List(Of DocumentPrinterPaperSize)
    End Class

#End Region

#Region " GetWeekEndingDateResult "
    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.FetchDocumentsListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for fetching 'ViewableDocument' methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	PaulW	19/10/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class GetWeekendingDateResult
        Public ErrMsg As ErrorMessage
        Public WeekEndingDate As DateTime
    End Class

#End Region

#Region " FetchSelectorDebtorInvoiceListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchSelectorDebtorInvoiceList() method.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MoTahir       15/01/2013 D12092G - Old-style Debtor Invoices In Place Selector
    ''' </history>
    Public Class FetchSelectorDebtorInvoiceListResult
        Public ErrMsg As ErrorMessage
        Public Invoices As List(Of Selectors.Messages.Items.DebtorInvoice)
        Public PagingLinks As String
    End Class

#End Region


#Region " FetchSectorCodeListResult "

    ''' <summary>
    ''' Simple class to return the results of the FetchSectorCodeList() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchSectorCodeListResult
        Public ErrMsg As ErrorMessage
        Public SectorCodes As List(Of ViewableSectorCode)
        Public PagingLinks As String
    End Class

#End Region

End Namespace
