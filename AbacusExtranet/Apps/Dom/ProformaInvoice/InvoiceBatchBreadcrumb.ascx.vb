
Imports System.Text
Imports System.Web
Imports Target.Web.Apps


Namespace Apps.Dom.ProformaInvoice
    Public Class InvoiceBatchBreadcrumb
        Inherits System.Web.UI.UserControl

#Region " Properties "
        Private _batchID As Integer
        Public Property BatchID() As Integer
            Get
                Return _batchID
            End Get
            Set(ByVal value As Integer)
                _batchID = value
            End Set
        End Property

        Private _invoiceID As Integer
        Public Property InvoiceID() As Integer
            Get
                Return _invoiceID
            End Get
            Set(ByVal value As Integer)
                _invoiceID = value
            End Set
        End Property

        Private _visitID As Integer
        Public Property VisitID() As Integer
            Get
                Return _visitID
            End Get
            Set(ByVal value As Integer)
                _visitID = value
            End Set
        End Property


        Private _paymentScheduleId As Integer
        Public Property PaymentScheduleId() As Integer
            Get
                Return _paymentScheduleId
            End Get
            Set(ByVal value As Integer)
                _paymentScheduleId = value
            End Set
        End Property


        Private _invFilterAwait As String
        Public Property InvFilterAwait() As String
            Get
                Return _invFilterAwait
            End Get
            Set(ByVal value As String)
                _invFilterAwait = value
            End Set
        End Property


        Private _invFilterVer As String
        Public Property InvFilterVer() As String
            Get
                Return _invFilterVer
            End Get
            Set(ByVal value As String)
                _invFilterVer = value
            End Set
        End Property


        Private _backUrl As String
        Public Property backUrl() As String
            Get
                Return _backUrl
            End Get
            Set(ByVal value As String)
                _backUrl = value
            End Set
        End Property

#End Region
        
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load



            Dim backJs As StringBuilder

            batchLink.Visible = False
            invoiceLink.Visible = False
            visitLink.Visible = False


            If _visitID > 0 Then
                'batchLink.Visible = True
                'viewBatch.HRef = String.Format("ViewInvoiceBatch.aspx?id={0}", _batchID)
                invoiceLink.Visible = True
                'viewInvoice.HRef = HttpUtility.UrlDecode(Request.QueryString("invoices"))
                ' the id is basically ps id
                'viewInvoice.HRef = String.Format("ViewInvoices.aspx?id={0}" & _
                '                                 "&pScheduleId={1}" & _
                '                                 "&await={2}" & _
                '                                 "&ver={3}" & _
                '                                 "&selectedInvoiceID={4}" & _
                '                                 "&backUrl={5}", _
                '                                PaymentScheduleId, _
                '                                PaymentScheduleId, _
                '                                InvFilterAwait, _
                '                                InvFilterVer, _
                '                                InvoiceID, _
                '                                HttpUtility.UrlEncode(backUrl) _
                '                                )
                visitLink.Visible = True
                'viewCostedVisit.HRef = HttpUtility.UrlDecode(Request.QueryString("costed"))
                'viewCostedVisit.HRef = String.Format("ViewInvoiceCostedVisits.aspx?id={0}" & _
                '                                     "&pScheduleId={1}" & _
                '                                     "&await={2}" & _
                '                                     "&ver={3}" & _
                '                                     "&selectedInvoiceID={4}" & _
                '                                     "&backUrl={5}", _
                '                                     InvoiceID, _
                '                                     PaymentScheduleId, _
                '                                     InvFilterAwait, _
                '                                     InvFilterVer, _
                '                                     InvoiceID, _
                '                                     HttpUtility.UrlEncode(backUrl) _
                '                                    )
            End If

            backJs = New StringBuilder()
            If Request.QueryString("backUrl") Is Nothing Then
                backJs.Append("history.go(-1);")
            Else
                backJs.AppendFormat("var backUrl=""{0}"";", Request.QueryString("backUrl"))
                backJs.Append("document.location.href=backUrl;")
            End If
            btnBack.Attributes.Add("onclick", backJs.ToString())


            Dim thePage As BasePage = CType(Me.Page, BasePage)
            thePage.JsLinks.Add("InvoiceBatchBreadcrumb.js")
            Dim js As String = String.Empty
            js = String.Format("viewInvoiceId=""{0}"";" & _
                              "viewCostedVisitId=""{1}"";" & _
                              "find={2}", _
                              viewInvoice.ClientID, _
                              viewCostedVisit.ClientID, _
                              IIf(invoiceLink.Visible And visitLink.Visible, True.ToString().ToLower(), False.ToString().ToLower()))

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", Target.Library.Web.Utils.WrapClientScript(js))

        End Sub

    End Class
End Namespace