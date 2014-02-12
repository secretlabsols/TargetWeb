Imports System.Collections.Generic
Imports Target.Abacus.Library.InterfaceLogs
Imports Target.Library
Imports Target.Web.Apps
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.CreditorPayments.Batches

    ''' <summary>
    ''' User control used to display filter criteria for creditor payments batches
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   18/02/2010 D11874 - Created
    ''' </history>
    Partial Public Class ucCreditorPaymentBatchCriteria
        Inherits System.Web.UI.UserControl

#Region "Fields"

        ' locals
        Private _FilterCreditorPaymentBatchID As Integer = 0

        ' constants
        Private Const _CurrencyFormat As String = "c"
        Private Const _DateFormat As String = "dd MMM yyyy"
        Private Const _DateAndTimeFormat As String = "dd MMM yyyy HH:mm:ss"
        Private Const _InformationNotSpecified As String = "(not specified)"

#End Region

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the filter creditor payment batch ID.
        ''' </summary>
        ''' <value>The filter creditor payment batch ID.</value>
        Public Property FilterCreditorPaymentBatchID() As Integer
            Get
                Return _FilterCreditorPaymentBatchID
            End Get
            Set(ByVal value As Integer)
                _FilterCreditorPaymentBatchID = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the base page.
        ''' </summary>
        ''' <value>My base page.</value>
        Private ReadOnly Property MyBasePage() As BasePage
            Get
                Return CType(Page, BasePage)
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' <summary>
        ''' Handles the Init event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            MyBasePage.AddExtraCssStyle("label.ucCreditorPaymentBatchCriteriaLabel { float:left; width:14.5em; font-weight:bold; }")

        End Sub

        ''' <summary>
        ''' Handles the PreRender event of the Page control.
        ''' </summary>
        ''' <param name="sender">The source of the event.</param>
        ''' <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender

            PopulateCreditorPaymentBatchCriteria()

        End Sub

#End Region

#Region "Functions"

        ''' <summary>
        ''' Populates the creditor payment batch criteria.
        ''' </summary>
        Private Sub PopulateCreditorPaymentBatchCriteria()

            ' reset all the labels text to empty
            lblCreatedDateValue.Text = String.Empty
            lblCreatedByValue.Text = String.Empty
            lblPaymentCountValue.Text = String.Empty
            lblPaymentValueNetValue.Text = String.Empty
            lblPaymentValueVATValue.Text = String.Empty
            lblPaymentValueGrossValue.Text = String.Empty
            lblPostingDateValue.Text = String.Empty
            lblPostingYearValue.Text = String.Empty
            lblPeriodNumValue.Text = String.Empty

            If FilterCreditorPaymentBatchID > 0 Then
                ' if the batch id is larger than 0, set display

                Dim msg As New ErrorMessage()
                Dim interfaceLog As ViewableInterfaceLog = Nothing

                ' fetch the item to display
                msg = InterfaceLogsBL.GetInterfaceLog(MyBasePage.DbConnection, FilterCreditorPaymentBatchID, interfaceLog)
                If Not msg.Success Then WebUtils.DisplayError(msg)

                If Not interfaceLog Is Nothing Then
                    ' if we found a matching record

                    With interfaceLog
                        lblCreatedDateValue.Text = .DateCreated.ToString(_DateAndTimeFormat)
                        lblCreatedByValue.Text = .CreatedBy
                        lblPaymentCountValue.Text = .Entries
                        lblPaymentValueNetValue.Text = .Value.ToString(_CurrencyFormat)
                        lblPaymentValueVATValue.Text = .Vat.ToString(_CurrencyFormat)
                        lblPaymentValueGrossValue.Text = .TotalValue.ToString(_CurrencyFormat)
                        If Utils.IsDate(.PostingDate) AndAlso .PostingDate.Ticks > 0 Then
                            lblPostingDateValue.Text = .PostingDate.ToString(_DateFormat)
                        Else
                            lblPostingDateValue.Text = _InformationNotSpecified
                        End If
                        If .PostingYear > 0 Then
                            lblPostingYearValue.Text = .PostingYear
                        Else
                            lblPostingYearValue.Text = _InformationNotSpecified
                        End If
                        If .PeriodNumber > 0 Then
                            lblPeriodNumValue.Text = .PeriodNumber
                        Else
                            lblPeriodNumValue.Text = _InformationNotSpecified
                        End If
                    End With

                End If

            End If

        End Sub

#End Region

    End Class

End Namespace
