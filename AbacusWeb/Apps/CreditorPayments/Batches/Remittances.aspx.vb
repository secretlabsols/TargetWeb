Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports Target.Library
Imports Target.Library.Web
Imports Target.Library.Web.UserControls

Namespace Apps.CreditorPayments.Batches

    ''' <summary>
    ''' Screen used to recreate batches of generic creditor payments.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''   ColinD   16/02/2010 D11874 - Created
    ''' </history>

    Partial Public Class Remittances
        Inherits BasePage

        Private Const _PageTitle As String = "Creditor Payment Batch - Remittances"
        Private Const _QsBatchID As String = "bid"

        ''' <summary>
        ''' Gets the standard buttons control.
        ''' </summary>
        ''' <value>The standard buttons control.</value>
        Private ReadOnly Property StandardButtonsControl() As StdButtonsBase
            Get
                Return CType(stdButtons1, StdButtonsBase)
            End Get
        End Property

        ''' <summary>
        ''' Gets the creditor payment batch criteria control.
        ''' </summary>
        ''' <value>The creditor payment batch criteria control.</value>
        Private ReadOnly Property CreditorPaymentBatchCriteriaControl() As ucCreditorPaymentBatchCriteria
            Get
                Return CType(FilterCriteria1, ucCreditorPaymentBatchCriteria)
            End Get
        End Property

        ''' <summary>
        ''' Gets the Remittance Selector control.
        ''' </summary>
        ''' <value>The creditor payment batch criteria control.</value>
        Private ReadOnly Property RemittanceSelectorControl() As RemittanceSelector
            Get
                Return CType(RemittanceSelector1, RemittanceSelector)
            End Get
        End Property

        ''' <summary>
        ''' Gets the batch id to filter on from the query string, 0 if not specified.
        ''' </summary>
        ''' <value>The filter batch ID.</value>
        Private ReadOnly Property FilterBatchID() As Integer
            Get
                Return Target.Library.Utils.ToInt32(Request.QueryString(_QsBatchID))
            End Get
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            ' init the page
            InitPage(ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.Payment.CreditorPayments.CreditorPaymentBatches"), _PageTitle)

            ' setup std buttons so only the back button is present
            With StandardButtonsControl
                .AllowBack = True
                .AllowDelete = False
                .AllowEdit = False
                .AllowFind = False
                .AllowNew = False
                .ShowNew = False
                .ShowSave = False
            End With

            SetupCreditorPaymentBatchCriteriaControl()

            RemittanceSelectorControl.InitControl(Me, FilterBatchID)

        End Sub

        ''' <summary>
        ''' Setups the creditor payment batch criteria control.
        ''' </summary>
        Private Sub SetupCreditorPaymentBatchCriteriaControl()

            With CreditorPaymentBatchCriteriaControl
                .FilterCreditorPaymentBatchID = FilterBatchID
            End With

        End Sub

    End Class

End Namespace