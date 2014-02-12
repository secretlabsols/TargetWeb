Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.CreditorPayments
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections

Namespace Apps.WebSvc

    ''' <summary>
    ''' Class used to represent the result of a call to ServiceRegisters.PrimeServiceRegister
    ''' </summary>
    ''' <history>
    ''' ColinDaly   D12140 07/07/2011 Created
    ''' </history>
    Public Class ServiceRegisters_PrimeServiceRegisterResult

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the error message.
        ''' </summary>
        ''' <value>The error message.</value>
        Public Property ErrMsg() As ErrorMessage
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


