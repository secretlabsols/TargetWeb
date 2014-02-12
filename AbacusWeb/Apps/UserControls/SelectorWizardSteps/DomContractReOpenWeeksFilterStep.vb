
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Abacus.Library.DebtorInvoices
Imports Target.Library
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports System.Text
Imports Target.Abacus.Web.Apps.General.DebtorInvoices
Imports Target.Web.Apps.Security
Imports Target.Web.Apps.Security.Collections


Namespace Apps.UserControls.SelectorWizardSteps

#Region " DomContractReOpenWeeksFilterStep "

    ''' <summary>
    ''' Wizard step that allows the filtering of re-opened weeks on week-ending and expected closure dates.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' </history>
    Friend Class DomContractReOpenWeeksFilterStep
        Implements ISelectorWizardStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_WE_DATEFROM As String = "weDateFrom"
        Const QS_WE_DATETO As String = "weDateTo"
        Const QS_CLOSURE_DATEFROM As String = "closureDateFrom"
        Const QS_CLOSURE_DATETO As String = "closureDateTo"

        Private _dbConnection As SqlConnection
        Private _baseUrl As String
        Private _isCurrentStep As Boolean
        Private _queryString As NameValueCollection
        Private _stepIndex As Integer
        Private _required As Boolean
        Private _description As String = "Please enter the date ranges below to filter the list of re-opened weeks."
        Private _weDateFrom As Date, _weDateTo As Date, _closureDateFrom As Date, _closureDateTo As Date
        Private _weDateFromControl As TextBoxEx
        Private _weDateToControl As TextBoxEx
        Private _closureDateFromControl As TextBoxEx
        Private _closureDateToControl As TextBoxEx
        Private _currentStep As Integer

        Public Property BaseUrl() As String Implements ISelectorWizardStep.BaseUrl
            Get
                Return _baseUrl
            End Get
            Set(ByVal value As String)
                _baseUrl = value
            End Set
        End Property

        Public ReadOnly Property BeforeNavigateJS() As String Implements ISelectorWizardStep.BeforeNavigateJS
            Get
                Return "DomContractReOpenWeeksFilterStep_BeforeNavigate()"
            End Get
        End Property

        Public Property DbConnection() As SqlConnection Implements ISelectorWizardStep.DbConnection
            Get
                Return _dbConnection
            End Get
            Set(ByVal value As SqlConnection)
                _dbConnection = value
            End Set
        End Property

        Public Property Description() As String Implements ISelectorWizardStep.Description
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Public Sub InitStep(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.InitStep
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DomContractReOpenWeeksFilterStep.js"))
        End Sub

        Public Property IsCurrentStep() As Boolean Implements ISelectorWizardStep.IsCurrentStep
            Get
                Return _isCurrentStep
            End Get
            Set(ByVal value As Boolean)
                _isCurrentStep = value
            End Set
        End Property

        Public Sub PreRender(ByVal wizard As SelectorWizardBase) Implements ISelectorWizardStep.PreRender

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.DateRangeStep.Startup", _
                WebUtils.WrapClientScript( _
                    String.Format("{0}_weDateFromID='{1}';{0}_weDateToID='{2}';{0}_closureDateFromID='{3}';{0}_closureDateToID='{4}';", _
                        "DomContractReOpenWeeksFilterStep", _weDateFromControl.ClientID, _weDateToControl.ClientID, _
                        _closureDateFromControl.ClientID, _closureDateToControl.ClientID) _
                ) _
            )

        End Sub

        Public Property QueryString() As NameValueCollection Implements ISelectorWizardStep.QueryString
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal value As NameValueCollection)
                _queryString = value
                ' pull out the required params
                If Utils.IsDate(_queryString(QS_WE_DATEFROM)) Then _weDateFrom = _queryString(QS_WE_DATEFROM)
                If Utils.IsDate(_queryString(QS_WE_DATETO)) Then _weDateTo = _queryString(QS_WE_DATETO)
                If Utils.IsDate(_queryString(QS_CLOSURE_DATEFROM)) Then _closureDateFrom = _queryString(QS_CLOSURE_DATEFROM)
                If Utils.IsDate(_queryString(QS_CLOSURE_DATETO)) Then _closureDateTo = _queryString(QS_CLOSURE_DATETO)
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
            End Set
        End Property

        Public Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderContentControls

            Dim msg As ErrorMessage = New ErrorMessage()
            Dim fs As HtmlGenericControl
            Dim legend As HtmlGenericControl
            Dim lit As Literal

            lit = New Literal
            lit.Text = "<br />"
            controls.Add(lit)

            ' W/E date range
            fs = New HtmlGenericControl("FIELDSET")
            controls.Add(fs)
            fs.Style.Add("width", "40%")
            legend = New HtmlGenericControl("LEGEND")
            fs.Controls.Add(legend)
            legend.InnerText = "Re-Opened Week Ending Date"

            _weDateFromControl = New TextBoxEx()
            With _weDateFromControl
                .ID = "dteWEDateFrom"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "Between"
                .LabelWidth = "6em"
                If Target.Library.Utils.IsDate(_weDateFrom) Then .Text = _weDateFrom
                fs.Controls.Add(_weDateFromControl)
            End With

            _weDateToControl = New TextBoxEx()
            With _weDateToControl
                .ID = "dteWEDateTo"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "And"
                .LabelWidth = "6em"
                If Target.Library.Utils.IsDate(_weDateTo) Then .Text = _weDateTo
                fs.Controls.Add(_weDateToControl)
            End With

            lit = New Literal
            lit.Text = "<br /><br />"
            controls.Add(lit)

            ' expected closure date range
            fs = New HtmlGenericControl("FIELDSET")
            controls.Add(fs)
            fs.Style.Add("width", "40%")
            legend = New HtmlGenericControl("LEGEND")
            fs.Controls.Add(legend)
            legend.InnerText = "Expected Closure Date"

            _closureDateFromControl = New TextBoxEx()
            With _closureDateFromControl
                .ID = "dteClosureDateFrom"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "Between"
                .LabelWidth = "6em"
                If Target.Library.Utils.IsDate(_closureDateFrom) Then .Text = _closureDateFrom
                fs.Controls.Add(_closureDateFromControl)
            End With

            _closureDateToControl = New TextBoxEx()
            With _closureDateToControl
                .ID = "dteClosureDateTo"
                .Format = TextBoxEx.TextBoxExFormat.DateFormat
                .LabelBold = True
                .LabelText = "And"
                .LabelWidth = "6em"
                If Target.Library.Utils.IsDate(_closureDateTo) Then .Text = _closureDateTo
                fs.Controls.Add(_closureDateToControl)
            End With

            lit = New Literal
            lit.Text = "<br /><br />"
            controls.Add(lit)

            msg.Success = True
            Return msg

        End Function

        Public Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage Implements ISelectorWizardStep.RenderHeaderControls

            Dim msg As ErrorMessage
            Dim lbl As Label
            Dim text As Label
            Dim link As HyperLink
            Dim lit As Literal
            Dim qs As NameValueCollection

            ' w/e dates
            lbl = New Label()
            lbl.Text = "Re-Opened Week Ending Date"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "18em")
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            If Not Utils.IsDate(_weDateFrom) And Not Utils.IsDate(_weDateTo) Then
                text.Text = "All"
            Else
                If Utils.IsDate(_weDateFrom) Then
                    text.Text = String.Format("From {0}", _weDateFrom.ToString("dd/MM/yyyy"))
                End If
                If Utils.IsDate(_weDateTo) Then
                    If Utils.IsDate(_weDateFrom) Then text.Text &= " "
                    text.Text &= String.Format("To {0}", _weDateTo.ToString("dd/MM/yyyy"))
                End If
            End If
            controls.Add(text)


            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso (Utils.IsDate(_weDateFrom) Or _
                                         Utils.IsDate(_weDateTo) Or _
                                         Utils.IsDate(_closureDateFrom) Or _
                                         Utils.IsDate(_closureDateTo)) Then
                ' all
                link.Text = "All Dates"
                qs.Remove(QS_WE_DATEFROM)
                qs.Remove(QS_WE_DATETO)
                qs.Remove(QS_CLOSURE_DATEFROM)
                qs.Remove(QS_CLOSURE_DATETO)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change Date Ranges"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, Me.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            lit = New Literal
            lit.Text = "<br />"
            controls.Add(lit)

            ' expected closure dates
            lbl = New Label()
            lbl.Text = "Expected Closure Date"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "18em")
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            If Not Utils.IsDate(_closureDateFrom) And Not Utils.IsDate(_closureDateTo) Then
                text.Text = "All"
            Else
                If Utils.IsDate(_closureDateFrom) Then
                    text.Text = String.Format("From {0}", _closureDateFrom.ToString("dd/MM/yyyy"))
                End If
                If Utils.IsDate(_closureDateTo) Then
                    If Utils.IsDate(_closureDateTo) Then text.Text &= " "
                    text.Text &= String.Format("To {0}", _closureDateTo.ToString("dd/MM/yyyy"))
                End If
            End If
            controls.Add(text)

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        Public Property Required() As Boolean Implements ISelectorWizardStep.Required
            Get
                Return _required
            End Get
            Set(ByVal value As Boolean)
                _required = value
            End Set
        End Property

        Public Property StepIndex() As Integer Implements ISelectorWizardStep.StepIndex
            Get
                Return _stepIndex
            End Get
            Set(ByVal value As Integer)
                _stepIndex = value
            End Set
        End Property

        Public ReadOnly Property Title() As String Implements ISelectorWizardStep.Title
            Get
                Return "Enter Date Ranges"
            End Get
        End Property
    End Class

#End Region

End Namespace