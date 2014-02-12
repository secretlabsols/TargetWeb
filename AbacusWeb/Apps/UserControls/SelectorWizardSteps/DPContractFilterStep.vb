Imports System.Collections.Specialized
Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls
Imports Target.Library
Imports Target.Library.Web.Controls
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils

Namespace Apps.UserControls.SelectorWizardSteps

    ''' <summary>
    ''' Screen used to apply SDS filter on direct payment contracts
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO       13/04/2011  SDS issue #415 - removed QS_DATEFROM/DATETO as they are now exposed by the DateRangeStep base class.
    '''     Waqas        Created 14/02/2011  D12009 - extended the DateRangeStep class and implemented the sds filter step
    ''' </history>
    ''' 
    Public Class DPContractFilterStep
        Inherits DateRangeStep

        Const QS_SDS As String = "issds"
        Const QS_CURRENTSTEP As String = "currentStep"

        Private _queryString As NameValueCollection
        Private _sdsRadioBtnBoth As RadioButton = New RadioButton()
        Private _sdsRadioBtnSds As RadioButton = New RadioButton()
        Private _sdsRadioBtnNonSds As RadioButton = New RadioButton()
        Private _sdsFilter As String
        Private _currentStep As Integer

        Public Property SdsFilter() As String
            Get
                If SdsRadioBtnBoth.Checked = True Then
                    Return SdsRadioBtnBoth.Text
                End If
                If SdsRadioBtnSds.Checked = True Then
                    Return SdsRadioBtnSds.Text
                End If
                If SdsRadioBtnNonSds.Checked = True Then
                    Return SdsRadioBtnNonSds.Text
                End If
                Return SdsRadioBtnBoth.Text
            End Get
            Set(ByVal value As String)
                _sdsFilter = value
            End Set
        End Property

        Public ReadOnly Property SdsRadioBtnBoth() As RadioButton
            Get
                Return _sdsRadioBtnBoth
            End Get
        End Property

        Public ReadOnly Property SdsRadioBtnSds() As RadioButton
            Get
                Return _sdsRadioBtnSds
            End Get
        End Property

        Public ReadOnly Property SdsRadioBtnNonSds() As RadioButton
            Get
                Return _sdsRadioBtnNonSds
            End Get
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection, ByVal renderTopBR As Boolean) As ErrorMessage
            ' add space b/w selections and group boxes
            Dim spacerBr As Literal
            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            Dim msg As ErrorMessage
            Dim fieldset As Panel = New Panel
            fieldset.GroupingText = "Dates"
            fieldset.Attributes.Add("style", "position:relative;float:left;margin-right:10px;")
            msg = MyBase.RenderDatesPanel(wizard, fieldset.Controls, False)
            If Not msg.Success Then Return msg
            controls.Add(fieldset)
            RenderSDSPanel(controls, renderTopBR)
            Dim clearPnl As Panel = New Panel
            clearPnl.Attributes.Add("style", "clear:left;")
            controls.Add(clearPnl)

            Return msg

        End Function

        ''' <summary>
        ''' Render SDS filter
        ''' </summary>
        ''' <param name="controls">Control collection</param>
        ''' <param name="renderTopBR">bool value to render top bar</param>
        ''' <remarks></remarks>
        ''' 
        Public Sub RenderSDSPanel(ByRef controls As System.Web.UI.ControlCollection, ByVal renderTopBR As Boolean)
            Dim fieldset As Panel = New Panel
            fieldset.GroupingText = "Self Directed Support (SDS)"
            fieldset.Attributes.Add("Style", "position:relative;float:left;")

            Dim spacerBr As Literal

            With _sdsRadioBtnBoth
                .ID = "sdsFilterBoth"
                .TextAlign = TextAlign.Right
                .GroupName = QS_SDS
                .Text = GetSdsDisplayValue(TriState.UseDefault)
                .Checked = True
                fieldset.Controls.Add(_sdsRadioBtnBoth)
            End With

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fieldset.Controls.Add(spacerBr)

            With _sdsRadioBtnSds
                .ID = "sdsFilterSds"
                .TextAlign = TextAlign.Right
                .GroupName = QS_SDS
                .Text = GetSdsDisplayValue(TriState.True)
                fieldset.Controls.Add(_sdsRadioBtnSds)
            End With

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            fieldset.Controls.Add(spacerBr)

            With _sdsRadioBtnNonSds
                .ID = "sdsFilterNonSds"
                .TextAlign = TextAlign.Right
                .GroupName = QS_SDS
                .Text = GetSdsDisplayValue(TriState.False)
                fieldset.Controls.Add(_sdsRadioBtnNonSds)
            End With

            If renderTopBR Then
                spacerBr = New Literal
                spacerBr.Text = "<br/"
                fieldset.Controls.Add(spacerBr)
            End If


            If Not HttpContext.Current.Request.QueryString(QS_SDS) Is Nothing Then
                If HttpContext.Current.Request.QueryString(QS_SDS) = "-2" Then
                    _sdsRadioBtnBoth.Checked = True
                End If
                If HttpContext.Current.Request.QueryString(QS_SDS) = "-1" Then
                    _sdsRadioBtnSds.Checked = True
                End If
                If HttpContext.Current.Request.QueryString(QS_SDS) = "0" Then
                    _sdsRadioBtnNonSds.Checked = True
                End If
            Else
                _sdsRadioBtnBoth.Checked = True
            End If

            controls.Add(fieldset)

        End Sub

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)
            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Target.SP.Apps.UserControls.SelectorWizard.DPContractFilterStep.Startup", _
                Target.Library.Web.Utils.WrapClientScript( _
                    String.Format("DPContractFilterStep_dateFromID='{0}';DPContractFilterStep_dateToID='{1}';DPContractFilterStep_SdsRadioBtnBoth='{2}';DPContractFilterStep_SdsRadioBtnSds='{3}';DPContractFilterStep_SdsRadioBtnNonSds='{4}';", _
                                  MyBase.DateFromControl.ClientID, _
                                  MyBase.DateToControl.ClientID, _
                                  SdsRadioBtnBoth.ClientID, _
                                  SdsRadioBtnSds.ClientID, _
                                  SdsRadioBtnNonSds.ClientID) _
                ) _
            )
        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DPContractFilterStep.js"))
        End Sub

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "DPContractFilterStep_BeforeNavigate()"
            End Get
        End Property

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return New NameValueCollection(_queryString)
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                ' pull out the required params
                If Not _queryString(QS_SDS) Is Nothing Then
                    ' if issds is present on the querystring but is empty, default to -2 (both sds and non-sds)
                    If Convert.ToString(_queryString(QS_SDS)).Length = 0 Then
                        _sdsFilter = TriState.UseDefault
                    Else
                        _sdsFilter = _queryString(QS_SDS)
                    End If
                Else
                    _sdsFilter = TriState.UseDefault
                    _queryString.Add(QS_SDS, TriState.UseDefault)
                End If
                _currentStep = Target.Library.Utils.ToInt32(_queryString(QS_CURRENTSTEP))

            End Set
        End Property

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage
            Dim spacerBr As Literal
            Dim lbl As Label
            Dim text As Label
            Dim link As HyperLink
            Dim qs As NameValueCollection

            ' Sds Filter
            lbl = New Label()
            lbl.Text = "SDS?"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", Me.HeaderLabelWidth.ToString())
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            text.Text = GetSdsDisplayValue(_sdsFilter)
            
            controls.Add(text)

            ' add the link
            qs = Me.QueryString
            link = New HyperLink()
            If Me.IsCurrentStep Then
                ' all
                link.Text = "SDS Filter"
                qs.Remove(QS_SDS)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf MyBase.CurrentStep > Me.StepIndex Then
                ' change
                link.Text = "Change SDS Filter"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, MyBase.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' date range
            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then Return msg

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        ''' <summary>
        ''' Get display value of SDS triplet
        ''' </summary>
        ''' <param name="sdsFilterValue"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetSdsDisplayValue(ByVal sdsFilterValue As TriState) As String
            If sdsFilterValue = TriState.False Then
                Return "Non-SDS"
            ElseIf sdsFilterValue = TriState.True Then
                Return "SDS"
            Else
                Return "Both"
            End If
        End Function

    End Class


End Namespace
