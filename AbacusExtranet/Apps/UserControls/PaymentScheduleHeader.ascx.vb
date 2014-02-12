Imports Target.Abacus.Library
Imports Target.Abacus.Extranet.Apps.UserControls.SelectorWizardSteps
Imports Target.Web.Apps.Security
Imports Target.Library.Web.Utils
Imports Target.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Web.Apps

Namespace Apps.UserControls

    Partial Public Class PaymentScheduleHeader
        Inherits System.Web.UI.UserControl

        Private _singleLiner As Boolean = False
        Public Property SingleLiner() As Boolean
            Get
                Return _singleLiner
            End Get
            Set(ByVal value As Boolean)
                _singleLiner = value
            End Set
        End Property

        Private _paymentScheduleId As Integer = 0
        Public Property PaymentScheduleId() As Integer
            Get
                Return _paymentScheduleId
            End Get
            Set(ByVal value As Integer)
                _paymentScheduleId = value
                PopulatePaymentScheduleHeader()
                setVisibility()
            End Set
        End Property


        Private _showPaymentschedule As Boolean = True
        Public Property showPaymentSchedule() As Boolean
            Get
                Return _showPaymentschedule
            End Get
            Set(ByVal value As Boolean)
                _showPaymentschedule = value
            End Set
        End Property


        Private _estabId As Integer
        Public Property EstabId() As Integer
            Get
                Return _estabId
            End Get
            Set(ByVal value As Integer)
                _estabId = value
            End Set
        End Property

        Private _contractId As Integer
        Public Property ContractId() As Integer
            Get
                Return _contractId
            End Get
            Set(ByVal value As Integer)
                _contractId = value
            End Set
        End Property

        Public WriteOnly Property BoldLabels() As Boolean
            Set(ByVal value As Boolean)
                SetBoldLabels(value)
            End Set
        End Property

        Public WriteOnly Property LabelWidth() As String
            Set(ByVal value As String)
                SetLabelwidth(value)
            End Set
        End Property

        Public Property GroupingText() As String
            Get
                Return pnlfieldset.GroupingText
            End Get
            Set(ByVal value As String)
                pnlfieldset.GroupingText = value
            End Set
        End Property

        Public Property ExternalControls() As Panel
            Get
                Return ExtraControls
            End Get
            Set(ByVal value As Panel)
                ExtraControls = value
            End Set
        End Property

        Public WriteOnly Property SingleLineLabelText() As String
            Set(ByVal value As String)
                lblReferenceSingleLine.Text = value
            End Set
        End Property

        ''' <summary>
        ''' Default: True
        ''' This property enables the hyperlink on the payment schedule
        ''' information in single line.
        ''' if passed true it will turn the hyperlink on the payment schedule and if 
        ''' passed false then it will change the hyperlink to a simple label
        ''' </summary>
        ''' <remarks></remarks>
        Private _enablePaymentScheduleHyperlink As Boolean = True
        Public WriteOnly Property EnablePaymentScheduleHyperlink() As Boolean
            Set(ByVal value As Boolean)
                _enablePaymentScheduleHyperlink = value
                ToggleSingleLinkLinkAndLabel()
            End Set
        End Property

        Private _pnlPaymentFromToVisibility As Boolean = True
        Public WriteOnly Property hidePaymentFromTo() As Boolean
            Set(ByVal value As Boolean)
                pnlPaymentFromTo.Visible = value
            End Set
        End Property

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Request.QueryString("psview") Is Nothing Then
                showPaymentSchedule = Boolean.Parse(Request.QueryString("psview").ToString())
            End If
            PopulatePaymentScheduleHeader()
            setVisibility()
        End Sub

        Private Sub setVisibility()
            If _paymentScheduleId = 0 Or Not showPaymentSchedule Then
                pnlPaymentSchedule.Visible = False
                pnlsingleLine.Visible = False
            Else
                '' display only single line 
                If SingleLiner Then
                    pnlsingleLine.Visible = True
                    pnlPaymentSchedule.Visible = False
                Else
                    pnlPaymentSchedule.Visible = True
                    pnlsingleLine.Visible = False
                End If
            End If
        End Sub

        Private Sub PopulatePaymentScheduleHeader()
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            Dim msg As New Target.Library.ErrorMessage

            ' either set the query string or set the public property.
            If _paymentScheduleId = 0 Then
                _paymentScheduleId = Utils.ToInt32(Request.QueryString(ClientStep.QS_PSCHEDULE_ID))
            End If

            Dim pSchedule As DataClasses.PaymentSchedule = New DataClasses.PaymentSchedule(thePage.DbConnection, _
                                                                                           String.Empty, _
                                                                                           String.Empty)
            msg = pSchedule.Fetch(_paymentScheduleId)
            If Not msg.Success Then
                Return
            End If

            ''''''''''''''' set estab id and contract id ''''''''''''''''''''
            ContractId = pSchedule.DomContractID
            EstabId = pSchedule.ProviderID
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            lnkPaymentSchedile.Text = pSchedule.Reference
            lnkPaymentSchedile.NavigateUrl = "~/AbacusExtranet/Apps/Dom/PaymentSchedules/PaymentSchedules.aspx?mode=1&id=" & pSchedule.ID

            lblDateFrom.Text = pSchedule.DateFrom.ToShortDateString()
            lblDateTo.Text = pSchedule.DateTo.ToShortDateString()

            Dim provider As DataClasses.Establishment = New DataClasses.Establishment(thePage.DbConnection)
            msg = provider.Fetch(pSchedule.ProviderID)
            If Not msg.Success Then
                Return
            End If

            lblProviderRef.Text = provider.AltReference
            lblProviderName.Text = provider.Name

            Dim contract As DataClasses.DomContract = New DataClasses.DomContract(thePage.DbConnection, "", "")
            msg = contract.Fetch(pSchedule.DomContractID)
            If Not msg.Success Then
                Return
            End If

            lblContractNumber.Text = contract.Number
            lblContractTitle.Text = contract.Title

            ''''' single line information 
            lnksingleLineLink.Text = String.Format("{0} ({1} to {2})", _
                                                   pSchedule.Reference, _
                                                   pSchedule.DateFrom.ToShortDateString(), _
                                                   pSchedule.DateTo.ToShortDateString())
            lnksingleLineLink.NavigateUrl = "~/AbacusExtranet/Apps/Dom/PaymentSchedules/PaymentSchedules.aspx?mode=1&id=" & pSchedule.ID
            '''''''''''''''''''''''''''''''''
            '' set single line information as label text
            lnksingleLineLabel.Text = String.Format("{0} ({1} to {2})", _
                                                   pSchedule.Reference, _
                                                   pSchedule.DateFrom.ToShortDateString(), _
                                                   pSchedule.DateTo.ToShortDateString())
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ToggleSingleLinkLinkAndLabel()

            ' display payment from payment to '''''''''''''
            pnlPaymentFromTo.Visible = _pnlPaymentFromToVisibility

            'add external controls
            'ExtraControls.Controls.Add(externalControls)

            '' if no payment scheduleid then disable control 


        End Sub

        Private Sub SetBoldLabels(ByVal isBold As Boolean)
            lblReferenceSingleLine.Font.Bold = isBold
            lblReference.Font.Bold = isBold
            lblProvider.Font.Bold = isBold
            lblcontract.Font.Bold = isBold
            lblPaymentFrom.Font.Bold = isBold
            lblPaymentTo.Font.Bold = isBold
        End Sub

        Private Sub SetLabelwidth(ByVal unit As String)
            lblReferenceSingleLine.Width = System.Web.UI.WebControls.Unit.Parse(unit)
            lblReference.Width = System.Web.UI.WebControls.Unit.Parse(unit)
            lblProvider.Width = System.Web.UI.WebControls.Unit.Parse(unit)
            lblcontract.Width = System.Web.UI.WebControls.Unit.Parse(unit)
            lblPaymentFrom.Width = System.Web.UI.WebControls.Unit.Parse(unit)
        End Sub

        Private Sub ToggleSingleLinkLinkAndLabel()
            lnksingleLineLink.Visible = _enablePaymentScheduleHyperlink
            lnksingleLineLabel.Visible = Not _enablePaymentScheduleHyperlink
        End Sub

    End Class

End Namespace