
Imports System.Collections.Specialized
Imports System.Text
Imports System.Web
Imports Target.Library

Namespace Apps.Dom.PaymentSchedules

    ''' <summary>
    ''' Class to parse and process the Payment Schedules wizard screen.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' IHS     24/05/2011  D12084 - Created.
    ''' </history>
    Public Class WizardScreenParameters

#Region " Consts "

        ' default start and end date
        Public Const NULL_DATE_TO As String = "9999-12-31"
        Public Const NULL_DATE_FROM As String = "1900-01-01"

        ' Provider and Contract
        Public Const QS_ESTAB_ID As String = "estabID"
        Public Const QS_CONTRACT_ID As String = "contractID"

        ' Header Details
        Public Const QS_REFERENCE As String = "ref"
        Public Const QS_PERIOD_FROM As String = "periodFrom"
        Public Const QS_PERIOD_TO As String = "periodTo"
        Public Const QS_VISIT_YES As String = "visitYes"
        Public Const QS_VISIT_NO As String = "visitNo"

        ' Unprocessed Pro forma Invoices
        Public Const QS_PROFORMA_INVOICES_NONE As String = "pfNone"
        Public Const QS_PROFORMA_INVOICES_AWAITING As String = "pfAwait"
        Public Const QS_PROFORMA_INVOICES_VERIFIED As String = "pfVer"

        ' Provider Invoices
        Public Const QS_INVOICES_UNPAID As String = "invUnpaid"
        Public Const QS_INVOICES_SUSPENDED As String = "invSusp"
        Public Const QS_INVOICES_AUTHORISED As String = "invAuth"
        Public Const QS_INVOICES_PAID As String = "invPaid"

        ' Unprocessed Visit Amendment Requests
        Public Const QS_VAR_AWAITING As String = "varAwait"
        Public Const QS_VAR_VERIFIED As String = "varVer"
        Public Const QS_VAR_DECLINED As String = "varDec"

#End Region

#Region " Private variables & properties "

        Public ReadOnly Property SelectedPeriodDesc() As String
            Get
                Dim result As StringBuilder = New StringBuilder()
                With result

                    If Me.PeriodFrom = NULL_DATE_FROM AndAlso Me.PeriodTo = NULL_DATE_TO Then
                        .Append("Any Period")
                    Else
                        If Not String.IsNullOrEmpty(Me.PeriodFrom) AndAlso Me.PeriodFrom <> NULL_DATE_FROM Then
                            .Append(String.Format("From: {0}", Me.PeriodFrom))
                        End If

                        If Not String.IsNullOrEmpty(Me.PeriodTo) AndAlso Me.PeriodTo <> NULL_DATE_TO Then
                            .Append(String.Format(" To: {0}", Me.PeriodTo))
                        End If
                    End If

                    ' remove leading ", "
                    If .Length > 0 Then
                        .Remove(0, 2)
                    Else
                        .Append("Any Period")
                    End If
                End With
                Return result.ToString().Trim()
            End Get
        End Property

        Public ReadOnly Property SelectedVisitBasedDesc() As String
            Get
                Dim result As StringBuilder = New StringBuilder()
                With result

                    If Me.VisitYes = "true" AndAlso Me.VisitNo = "true" Then
                        .Append(", All")
                    Else
                        If Me.VisitYes = "true" Then .Append(", Yes")
                        If Me.VisitNo = "true" Then .Append(", No")
                    End If

                    ' remove leading ", "
                    If .Length > 0 Then
                        .Remove(0, 2)
                    Else
                        .Append("All")
                    End If
                End With
                Return result.ToString()
            End Get
        End Property

        Public ReadOnly Property SelectedUnprocessedProformaInvoicesDesc() As String
            Get
                Dim result As StringBuilder = New StringBuilder()
                With result

                    If Me.ProformaInvoicesNone = "true" AndAlso Me.ProformaInvoicesAwaitingVerification = "true" _
                       AndAlso Me.ProformaInvoicesVerified = "true" Then
                        .Append(", All")
                    Else
                        If Me.ProformaInvoicesNone = "true" Then .Append(", Having no Pro forma Invoices")
                        If Me.ProformaInvoicesAwaitingVerification = "true" Then .Append(", Having Pro forma Invoices that are 'Awaiting Verification'")
                        If Me.ProformaInvoicesVerified = Boolean.TrueString.ToLower() Then .Append(", Having 'verified' Pro forma Invoices")
                    End If

                    ' remove leading ", "
                    If .Length > 0 Then
                        .Remove(0, 2)
                    Else
                        .Append("All")
                    End If
                End With
                Return result.ToString()
            End Get
        End Property

        Public ReadOnly Property SelectedProviderInvoicesDesc() As String
            Get
                Dim result As StringBuilder = New StringBuilder()
                With result

                    If Me.InvoicesUnpaid = "true" AndAlso Me.InvoicesSuspended = "true" _
                       AndAlso Me.InvoicesAuthorised = "true" AndAlso Me.InvoicesPaid = "true" Then
                        .Append(", All")
                    Else
                        If Me.InvoicesUnpaid = "true" Then .Append(", Unpaid")
                        If Me.InvoicesSuspended = "true" Then .Append(", Suspended")
                        If Me.InvoicesAuthorised = "true" Then .Append(", Authorised")
                        If Me.InvoicesPaid = "true" Then .Append(", Paid")
                    End If

                    ' remove leading ", "
                    If .Length > 0 Then
                        .Remove(0, 2)
                    Else
                        .Append("All")
                    End If
                End With
                Return result.ToString()
            End Get
        End Property

        Public ReadOnly Property SelectedUnprocessedVARsDesc() As String
            Get
                Dim result As StringBuilder = New StringBuilder()
                With result

                    If Me.VAR_Awaiting = "true" AndAlso Me.VAR_Verified = "true" _
                       AndAlso Me.VAR_Declined = "true" Then
                        .Append(", All")
                    Else
                        If Me.VAR_Awaiting = "true" Then .Append(", Awaiting Verification")
                        If Me.VAR_Verified = "true" Then .Append(", Verified")
                        If Me.VAR_Declined = "true" Then .Append(", Declined")
                    End If

                    ' remove leading ", "
                    If .Length > 0 Then
                        .Remove(0, 2)
                    Else
                        .Append("All")
                    End If
                End With
                Return result.ToString()
            End Get
        End Property

#End Region

#Region " Public Fields "

        ' Provider and Contract
        Public EstabID As Integer
        Public ContractID As Integer

        ' Header Details
        Public Reference As String
        Public PeriodFrom As String
        Public PeriodTo As String
        Public VisitYes As String
        Public VisitNo As String

        ' Unprocessed Pro forma Invoices
        Public ProformaInvoicesNone As String
        Public ProformaInvoicesAwaitingVerification As String
        Public ProformaInvoicesVerified As String

        ' Provider Invoices
        Public InvoicesUnpaid As String
        Public InvoicesSuspended As String
        Public InvoicesAuthorised As String
        Public InvoicesPaid As String

        ' Unprocessed Visit Amendment Requests
        Public VAR_Awaiting As String
        Public VAR_Verified As String
        Public VAR_Declined As String

#End Region

#Region " Ctor "

        ''' <summary>
        ''' Ctor.
        ''' </summary>
        ''' <remarks></remarks>
        Sub New()
            Initialise()
        End Sub

        ''' <summary>
        ''' Ctor. Primes with values from the supplied string.
        ''' </summary>
        ''' <param name="qs">The querystring suitable for parsing by HttpUtility.ParseQueryString().</param>
        ''' <remarks></remarks>
        Sub New(ByVal qs As String)
            Me.New(HttpUtility.ParseQueryString(qs))
        End Sub

        ''' <summary>
        ''' Ctor. Primes with values from the supplied name/value colleciton.
        ''' </summary>
        ''' <param name="qs">The querystring, i.e. Request.QueryString.</param>
        ''' <remarks></remarks>
        Sub New(ByVal qs As NameValueCollection)
            Me.New()
            ParseQS(qs)
        End Sub

#End Region

#Region " Initialise "

        ''' <summary>
        ''' Initialises all fields with initial values.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Initialise()
            Me.EstabID = 0
            Me.ContractID = 0

            Me.Reference = String.Empty
            Me.PeriodFrom = String.Empty
            Me.PeriodTo = String.Empty
            Me.VisitYes = String.Empty
            Me.VisitNo = String.Empty

            Me.ProformaInvoicesNone = String.Empty
            Me.ProformaInvoicesAwaitingVerification = String.Empty
            Me.ProformaInvoicesVerified = String.Empty

            Me.InvoicesUnpaid = String.Empty
            Me.InvoicesSuspended = String.Empty
            Me.InvoicesAuthorised = String.Empty
            Me.InvoicesPaid = String.Empty

            Me.VAR_Awaiting = String.Empty
            Me.VAR_Verified = String.Empty
            Me.VAR_Declined = String.Empty
        End Sub

#End Region

#Region " BuildUrl "

        ''' <summary>
        ''' Builds an absolute url, priming the querystring from the field values.
        ''' </summary>
        ''' <param name="url">The starting url.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function BuildUrl(ByVal url As Uri) As String

            Dim builder As Target.Library.Web.UriBuilder = New Target.Library.Web.UriBuilder(url)

            With builder.QueryItems

                .Set(QS_ESTAB_ID, Me.EstabID.ToString())
                .Set(QS_CONTRACT_ID, Me.ContractID)

                .Set(QS_REFERENCE, Me.Reference)
                .Set(QS_PERIOD_FROM, Me.PeriodFrom)
                .Set(QS_PERIOD_TO, Me.PeriodTo)
                .Set(QS_VISIT_YES, Me.VisitYes)
                .Set(QS_VISIT_NO, Me.VisitNo)

                .Set(QS_PROFORMA_INVOICES_NONE, Me.ProformaInvoicesNone)
                .Set(QS_PROFORMA_INVOICES_AWAITING, Me.ProformaInvoicesAwaitingVerification)
                .Set(QS_PROFORMA_INVOICES_VERIFIED, Me.ProformaInvoicesVerified)

                .Set(QS_INVOICES_UNPAID, Me.InvoicesUnpaid)
                .Set(QS_INVOICES_SUSPENDED, Me.InvoicesSuspended)
                .Set(QS_INVOICES_AUTHORISED, Me.InvoicesAuthorised)
                .Set(QS_INVOICES_PAID, Me.InvoicesPaid)

                .Set(QS_VAR_AWAITING, Me.VAR_Awaiting)
                .Set(QS_VAR_VERIFIED, Me.VAR_Verified)
                .Set(QS_VAR_DECLINED, Me.VAR_Declined)

            End With

            Return builder.Uri.AbsoluteUri

        End Function

#End Region

#Region " ParseQS "

        ''' <summary>
        ''' Parses the querystring and primes the fields.
        ''' </summary>
        ''' <param name="qs">The querystring.</param>
        ''' <remarks></remarks>
        Private Sub ParseQS(ByVal qs As NameValueCollection)

            Me.EstabID = Utils.ToInt32(qs(QS_ESTAB_ID))
            Me.ContractID = Utils.ToInt32(qs(QS_CONTRACT_ID))

            Me.Reference = Utils.ToString(qs(QS_REFERENCE))
            Me.PeriodFrom = Utils.ToString(qs(QS_PERIOD_FROM))
            Me.PeriodTo = Utils.ToString(qs(QS_PERIOD_TO))
            Me.VisitYes = Utils.ToString(qs(QS_VISIT_YES))
            Me.VisitNo = Utils.ToString(qs(QS_VISIT_NO))

            Me.ProformaInvoicesNone = Utils.ToString(qs(QS_PROFORMA_INVOICES_NONE))
            Me.ProformaInvoicesAwaitingVerification = Utils.ToString(qs(QS_PROFORMA_INVOICES_AWAITING))
            Me.ProformaInvoicesVerified = Utils.ToString(qs(QS_PROFORMA_INVOICES_VERIFIED))

            Me.InvoicesUnpaid = Utils.ToString(qs(QS_INVOICES_UNPAID))
            Me.InvoicesSuspended = Utils.ToString(qs(QS_INVOICES_SUSPENDED))
            Me.InvoicesAuthorised = Utils.ToString(qs(QS_INVOICES_AUTHORISED))
            Me.InvoicesPaid = Utils.ToString(qs(QS_INVOICES_PAID))

            Me.VAR_Awaiting = Utils.ToString(qs(QS_VAR_AWAITING))
            Me.VAR_Verified = Utils.ToString(qs(QS_VAR_VERIFIED))
            Me.VAR_Declined = Utils.ToString(qs(QS_VAR_DECLINED))

            ' if value not in Query String then set it to default value

            If Me.PeriodFrom = String.Empty OrElse Me.PeriodFrom = "null" OrElse _
                                            Not Utils.IsDate(Me.PeriodFrom) Then
                Me.PeriodFrom = "null"
            End If

            If Me.PeriodTo = String.Empty OrElse Me.PeriodTo = "null" OrElse _
                                            Not Utils.IsDate(Me.PeriodTo) Then
                Me.PeriodTo = "null"
            End If

            If Me.VisitYes = String.Empty Then Me.VisitYes = "true"
            If Me.VisitNo = String.Empty Then Me.VisitNo = "true"

            If Me.ProformaInvoicesNone = String.Empty Then Me.ProformaInvoicesNone = "true"
            If Me.ProformaInvoicesAwaitingVerification = String.Empty Then Me.ProformaInvoicesAwaitingVerification = "true"
            If Me.ProformaInvoicesVerified = String.Empty Then Me.ProformaInvoicesVerified = "true"

            If Me.InvoicesUnpaid = String.Empty Then Me.InvoicesUnpaid = "true"
            If Me.InvoicesSuspended = String.Empty Then Me.InvoicesSuspended = "true"
            If Me.InvoicesAuthorised = String.Empty Then Me.InvoicesAuthorised = "true"
            If Me.InvoicesPaid = String.Empty Then Me.InvoicesPaid = "true"

            If Me.VAR_Awaiting = "" Then Me.VAR_Awaiting = "true"
            If Me.VAR_Verified = "" Then Me.VAR_Verified = "true"
            If Me.VAR_Declined = "" Then Me.VAR_Declined = "true"

        End Sub

#End Region

    End Class

End Namespace