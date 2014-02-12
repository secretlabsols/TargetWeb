
Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.RequestPayments

Namespace Apps.WebSvc

    Public Class NonDelivery

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

    End Class

End Namespace