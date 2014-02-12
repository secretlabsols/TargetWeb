Imports System.Collections.Generic
Imports Target.Library
Imports Target.Abacus.Library
Imports Target.Abacus.Library.CreditorPayments
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections

Namespace Apps.WebSvc

    ''' <summary>
    ''' Class used to represent the result of a call to CreditorPayments.GetGenericCreditorPaymentSuspensionCommentsResult
    ''' it populates from vwSuspensionComment.
    ''' </summary>
    ''' <history>
    ''' Waqas       A7461 amended 29/09/2012
    ''' ColinDaly   D11874 Created 18/02/2010
    ''' </history>
    Public Class CreditorPayments_GetvwSuspensionCommentCollectionCommentsResult

#Region "Fields"

        Private _ErrorMsg As ErrorMessage = Nothing
        Private _Items As vwSuspensionCommentCollection = Nothing

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

        ''' <summary>
        ''' Gets or sets the items.
        ''' </summary>
        ''' <value>The items.</value>
        Public Property Items() As vwSuspensionCommentCollection
            Get
                Return _Items
            End Get
            Set(ByVal value As vwSuspensionCommentCollection)
                _Items = value
            End Set
        End Property

#End Region

    End Class

End Namespace


