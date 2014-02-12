Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.CreditorPayments

Namespace Apps.WebSvc

    ''' <summary>
    ''' Class used to represent the result of a call to CreditorPayments.ToggleGenericCreditorPaymentExcludeFromCreditorsFlag
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D11874 Created 18/02/2010
    ''' </history>
    Public Class CreditorPayments_ToggleGenericCreditorPaymentExcludeFromCreditorsFlag

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrorMsg() As ErrorMessage
            Get
                Return _ErrorMsg
            End Get
            Set(ByVal value As ErrorMessage)
                _ErrorMsg = value
            End Set
        End Property

#End Region

    End Class

End Namespace


