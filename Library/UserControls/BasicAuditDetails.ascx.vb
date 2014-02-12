
Imports AjaxControlToolkit
Imports Target.Library.Web.UserControls

Namespace Library.UserControls

    ''' <summary>
    ''' User control to display basic audit details.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' ColinD  04/10/2010  D11802 - added AmendedByCaption/EnteredByCaption/DateLastAmendedCaption/DateEnteredCaption
    ''' MikeVO  09/12/2009  A4WA#5946 - added EnteredVisible/AmendedVisible properties.
    ''' MikeVO  20/02/2009  D11492 - created.
    ''' </history>
    Partial Public Class BasicAuditDetails
        Inherits System.Web.UI.UserControl
        Implements IBasicAuditDetails


#Region " IBasicAuditDetails "

        Public Property ToggleControlID() As String Implements IBasicAuditDetails.ToggleControlID
            Get
                Return cpe.CollapseControlID
            End Get
            Set(ByVal value As String)
                cpe.CollapseControlID = value
                cpe.ExpandControlID = value
            End Set
        End Property

        Public Property Collapsed() As Boolean Implements IBasicAuditDetails.Collapsed
            Get
                Return cpe.Collapsed
            End Get
            Set(ByVal value As Boolean)
                cpe.Collapsed = value
            End Set
        End Property

        Public Property DateEntered() As Date Implements IBasicAuditDetails.DateEntered
            Get
                Return txtDateEntered.Text
            End Get
            Set(ByVal value As Date)
                txtDateEntered.Text = value
            End Set
        End Property

        Public Property DateLastAmended() As Date Implements IBasicAuditDetails.DateLastAmended
            Get
                Return txtDateLastAmended.Text
            End Get
            Set(ByVal value As Date)
                txtDateLastAmended.Text = value
            End Set
        End Property

        Public Property EnteredBy() As String Implements IBasicAuditDetails.EnteredBy
            Get
                Return txtEnteredBy.Text
            End Get
            Set(ByVal value As String)
                txtEnteredBy.Text = value
            End Set
        End Property

        Public Property LastAmendedBy() As String Implements IBasicAuditDetails.LastAmendedBy
            Get
                Return txtLastAmendedBy.Text
            End Get
            Set(ByVal value As String)
                txtLastAmendedBy.Text = value
            End Set
        End Property

        Public Property AmendedVisible() As Boolean Implements IBasicAuditDetails.AmendedVisible
            Get
                Return pnlAmended.Visible
            End Get
            Set(ByVal value As Boolean)
                pnlAmended.Visible = value
            End Set
        End Property

        Public Property EnteredVisible() As Boolean Implements IBasicAuditDetails.EnteredVisible
            Get
                Return pnlEntered.Visible
            End Get
            Set(ByVal value As Boolean)
                pnlEntered.Visible = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the amended by caption.
        ''' </summary>
        ''' <value>The amended by caption.</value>
        Public Property AmendedByCaption() As String Implements IBasicAuditDetails.LastAmendedByCaption
            Get
                Return txtLastAmendedBy.LabelText
            End Get
            Set(ByVal value As String)
                txtLastAmendedBy.LabelText = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the entered by caption.
        ''' </summary>
        ''' <value>The entered by caption.</value>
        Public Property EnteredByCaption() As String Implements IBasicAuditDetails.EnteredByCaption
            Get
                Return txtEnteredBy.LabelText
            End Get
            Set(ByVal value As String)
                txtEnteredBy.LabelText = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the date last amended caption.
        ''' </summary>
        ''' <value>The date last amended caption.</value>
        Public Property DateLastAmendedCaption() As String Implements IBasicAuditDetails.DateLastAmendedCaption
            Get
                Return txtDateLastAmended.LabelText
            End Get
            Set(ByVal value As String)
                txtDateLastAmended.LabelText = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the date entered caption.
        ''' </summary>
        ''' <value>The date entered caption.</value>
        Public Property DateEnteredCaption() As String Implements IBasicAuditDetails.DateEnteredCaption
            Get
                Return txtDateEntered.LabelText
            End Get
            Set(ByVal value As String)
                txtDateEntered.LabelText = value
            End Set
        End Property

#End Region

    End Class

End Namespace

