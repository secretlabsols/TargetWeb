Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.ServiceOrder
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.RequestPayments

Namespace Apps.WebSvc

#Region " BooleanResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.WebSvc.StringResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for methods that return a lone boolean value.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	JohnF	05/12/2013	Created (D12524A)
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class BooleanResult
        Public ErrMsg As ErrorMessage
        Public Value As Boolean
    End Class

#End Region

#Region " FetchEstablishmentsListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchEstablishmentsList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	07/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchEstablishmentsListResult
        Public ErrMsg As ErrorMessage
        Public Establishments As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchRemittancesListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchRemittancesListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchRemittancesListResult().
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	07/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchRemittancesListResult
        Public ErrMsg As ErrorMessage
        Public Remittances As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchClientsListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchClientsListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	07/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchClientsListResult
        Public ErrMsg As ErrorMessage
        Public Clients As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchRemittancesListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchRemittanceDetailForUserList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchRemittanceDetailForUserList().
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Mikevo]	13/09/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchRemittanceDetailForUserListResult
        Public ErrMsg As ErrorMessage
        Public DetailLines As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchResOccupancyListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchResOccupancyList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Paul]	15/10/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchResOccupancyListResult
        Public ErrMsg As ErrorMessage
        Public Occupancy As ArrayList
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchResServiceUserListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchResServiceUserList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Paul]	29/10/2007	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchResServiceUserListResult
        Public ErrMsg As ErrorMessage
        Public ServiceUsers As ArrayList
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
    ''' 	[PaulW]	30/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchRateCategoryListResult
        Public ErrMsg As ErrorMessage
        Public Categories As DomRateCategoryCollection
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

#Region " FetchServiceDeliveryFileListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchServiceDeliveryFileList
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Paul]	14/02/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchServiceDeliveryFileListResult
        Public ErrMsg As ErrorMessage
        Public ServiceDeliveryFile As ArrayList
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
    ''' 	[PaulW]	30/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ViewablePairListResult
        Public ErrMsg As ErrorMessage
        Public List As List(Of ViewablePair)
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
    ''' 	[PaulW]	30/01/2008	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class StringResult
        Public ErrMsg As ErrorMessage
        Public Value As String
    End Class

#End Region

#Region " FetchManualDomProformaInvoiceEnquiryListResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchManualDomProformaInvoiceEnquiryResults() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchManualDomProformaInvoiceEnquiryListResult
        Public ErrMsg As ErrorMessage
        Public Invoices As List(Of ManualDomProformaInvoiceEnquiryResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDomProformaInvoiceEnquiryListResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchDomProformaInvoiceEnquiryResults() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProformaInvoiceEnquiryListResult
        Public ErrMsg As ErrorMessage
        Public Batches As List(Of DomProformaInvoiceEnquiryResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDomProformaInvoiceListResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchDomProformaInvoiceResults() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProformaInvoiceListResult
        Public ErrMsg As ErrorMessage
        Public Invoices As List(Of DomProformaInvoiceResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDomProformaInvoiceLinesResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchDomProformaInvoiceLines() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProformaInvoiceLinesResult
        Public ErrMsg As ErrorMessage
        Public InvoiceLines As List(Of DomProformaInvoiceLineResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDomProformaInvoiceCostedVisitsResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchDomProformaInvoiceCostedVisits() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProformaInvoiceCostedVisitsResult
        Public ErrMsg As ErrorMessage
        Public InvoiceCostedVisits As List(Of DomProformaInvoiceCostedVisitsResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDomProviderInvoiceCostedVisitsResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchDomProviderInvoiceCostedVisits() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProviderInvoiceCostedVisitsResult
        Public ErrMsg As ErrorMessage
        Public InvoiceCostedVisits As List(Of DomProviderInvoiceCostedVisitsResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchVisitCodesAvailableForVisitResult "

    ''' <summary>
    ''' Simple class used to store the results of a call to the FetchVisitCodesAvailableForVisit() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchVisitCodesAvailableForVisitResult
        Inherits ViewablePairListResult
        Public UniqueID As String
        Public DefaultCodeID As Integer
    End Class

#End Region

#Region " FetchPaymenstScheduleEnquiryListResult "
    ''' <summary>
    ''' Simple classs to hold the results of the FetchPaymenstScheduleEnquiryListResult
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchPaymenstScheduleEnquiryListResult
        Public ErrMsg As ErrorMessage
        Public PaymentSchedules As List(Of PaymentScheduleEnquiryResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchDomProviderInvoiceEnquiryListResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchDomProviderInvoiceEnquiryResults() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProviderInvoiceEnquiryListResult
        Public ErrMsg As ErrorMessage
        Public RetractVisible As Boolean
        Public RetractSuccessful As Boolean
        Public Invoices As List(Of DomProviderInvoiceEnquiryResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchRetractProviderInvoiceVisibilityResults "
    ''' <summary>
    ''' Simple class to hold the results of the FetchRetractProviderInvoiceVisibilityResults() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchRetractProviderInvoiceVisibilityResults
        Public ErrMsg As ErrorMessage
        Public Visibility As Boolean
    End Class

#End Region

#Region " FetchDomProviderInvoiceVisitsEnquiryListResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchDomProviderInvoiceVisitEnquiryResults() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDomProviderInvoiceVisitsEnquiryListResult
        Public ErrMsg As ErrorMessage
        Public Visits As List(Of DomProviderInvoiceVisitsEnquiryResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchRetractProviderInvoiceVisibilityResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchDomProviderInvoiceVisitEnquiryResults() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchRetractProviderInvoiceVisibilityResult
        Public ErrMsg As ErrorMessage
        Public Visibility As Boolean
    End Class

#End Region

#Region " FetchCareWorkerListResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchCareWorkerListResult() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchCareWorkerListResult
        Public ErrMsg As ErrorMessage
        Public CareWorkers As List(Of CareWorkerResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchVisitAmendmentRequestEnquiryListResult "

    ''' <summary>
    ''' Simple class to hold the results of the FetchVisitAmendmentRequestEnquiryResult() method.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchVisitAmendmentRequestEnquiryListResult
        Public ErrMsg As ErrorMessage
        Public Amendments As List(Of VisitAmendmentRequestEnquiryResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " DurationClaimedRoundingListResult "

    ''' <summary>
    ''' simple class to hold results of the FetchDurationClaimedRoundingEnquiryResult
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchDurationClaimedRoundingEnquiryListResult
        Public ErrMsg As ErrorMessage
        Public DcrItems As List(Of DurationClaimedRoundingEnquiryResult)
        Public PagingLinks As String
    End Class

#End Region

#Region " InPlaceExternalAccountEnquiryResults "

    ''' <summary>
    ''' simple class to hold results of the FetchDurationClaimedRoundingEnquiryResult
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchInPlaceExternalAccountEnquiryListResults
        Public ErrMsg As ErrorMessage
        Public EAItems As List(Of InPlaceExternalAccountEnquiryResults)
        Public PagingLinks As String
    End Class

#End Region

#Region " FetchContractListResult "
    ''' <summary>
    ''' Simple classs to hold the results of the FetchContractListResult
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchContractListResult
        Public ErrMsg As ErrorMessage
        Public ContractList As List(Of ViewableContractList)
    End Class

#End Region

#Region " CreatePaymentRequestResult "
    ''' <summary>
    ''' Simple classs to hold the results of the CreatePaymentRequestResult
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CreatePaymentRequestResult
        Public ErrMsg As ErrorMessage
        Public PaymentRequestID As Integer
    End Class

#End Region

#Region " CreatePaymentRequest_DomContractResult "
    ''' <summary>
    ''' Simple classs to hold the results of the CreatePaymentRequest_DomContractResult
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CreatePaymentRequest_DomContractResult
        Public ErrMsg As ErrorMessage
    End Class

#End Region

#Region " CreateJob_ProcessPaymentRequestResult "
    ''' <summary>
    ''' Simple classs to hold the results of the CreateJob_ProcessPaymentRequestResult
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CreateJob_ProcessPaymentRequestResult
        Public ErrMsg As ErrorMessage
        Public EmailAddress As String
    End Class

#End Region

#Region " FetchInvoiceListResult "
    Public Class FetchInvoiceReferenceListResult
        Public ErrMsg As ErrorMessage
        Public InvoiceReferences As List(Of InvoiceReferences.ViewableInvoiceReferences)
        Public PagingLinks As String
    End Class
#End Region

#Region " DomProviderInvoiceDetailNonDeliveryUnitBased "
    Public Class FetchDomProviderInvoiceDetailNonDeliveryUnitBasedListResult
        Public ErrMsg As ErrorMessage
        Public NonDeliveryUnits As List(Of ProviderInvoiceDetailNonDeliveryUnitBased)
    End Class
#End Region

#Region " DomProviderInvoiceDetailNonDeliveryVisitBased "
    Public Class FetchDomProviderInvoiceDetailNonDeliveryVisitBasedListResult
        Public ErrMsg As ErrorMessage
        Public NonDeliveryVisits As List(Of ProviderInvoiceDetailNonDeliveryVisitBased)
    End Class
#End Region

#Region " Current Proforma Detail "
    Public Class NonDeliveryProviderInvoiceResult
        Public ErrMsg As ErrorMessage
        Public CurrentProforma As NonDeliveryProviderInvoice
    End Class
#End Region

#Region " Current Proforma Detail "
    Public Class NonDeliveryProviderInvoiceDetailResult
        Public ErrMsg As ErrorMessage
        Public CurrentProformaDetail As NonDeliveryProviderInvoiceDetail
    End Class
#End Region

#Region " FetchPaymenstScheduleEnquiryResult "
    ''' <summary>
    ''' Simple classs to hold the results of the FetchPaymenstScheduleEnquiryListResult
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FetchPaymenstScheduleEnquiryResult
        Public ErrMsg As ErrorMessage
        Public PaymentSchedule As NonDeliveryPaymentSchedule
    End Class

#End Region

#Region " FetchServiceOrderListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchServiceOrderListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	18/10/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchServiceOrderListResult
        Public ErrMsg As ErrorMessage
        Public dsoList As List(Of ViewableGenericServiceOrderExtranet)
    End Class

#End Region

#Region " FetchServiceOrderResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchServiceOrderResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	18/10/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchServiceOrderResult
        Public ErrMsg As ErrorMessage
        Public dso As ViewableGenericServiceOrderExtranet
    End Class

#End Region

#Region " FetchServiceOrderDetailListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchServiceOrderDetailListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	18/10/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchServiceOrderDetailListResult
        Public ErrMsg As ErrorMessage
        Public dsodList As List(Of ViewableGenericDSODetailExtranet)
    End Class

#End Region

#Region " FetchServiceOrderSuspensionListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchServiceOrderSuspensionListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	18/10/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchServiceOrderSuspensionListResult
        Public ErrMsg As ErrorMessage
        Public suspensionList As List(Of ViewableServiceOrderSuspension)
    End Class

#End Region

#Region " FetchServiceOrderCostListResult "

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Extranet
    ''' Class	 : Abacus.Extranet.Apps.WebSvc.FetchServiceOrderCostListResult
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     Simple classs to provide results for FetchXXXList() methods.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	18/10/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchServiceOrderCostListResult
        Public ErrMsg As ErrorMessage
        Public costList As List(Of ViewableServiceOrderCost)
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
    ''' 	PaulW	19/10/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class FetchDocumentsListResult
        Public ErrMsg As ErrorMessage
        Public Documents As List(Of ViewableDocument)
        Public PagingLinks As String
        Public CurrPage As Integer
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

#Region " CreateCareWorkerResult "

    ''' <summary>
    ''' Create Care worker id
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CreateCareWorkerResult
        Public ErrMsg As ErrorMessage
        Public CareWorkerID As Integer
    End Class

#End Region

End Namespace