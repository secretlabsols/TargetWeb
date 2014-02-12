
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

#Region " DomContractPeriodSysAccFilterStep "

    ''' <summary>
    ''' Implements a wizard step to select Suspension End Reason and period.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    '''     MikeVO      25/09/2009  D11546 - changes to "selections" section links
    ''' </history>
    Friend Class DomContractPeriodSysAccFilterStep
        Inherits DateRangeStep

        Const QS_CURRENTSTEP As String = "currentStep"
        Const QS_CONTRACTID As String = "contractID"
        Const QS_PERIODID As String = "periodID"
        Const QS_SYSACCID As String = "sysAccID"
        Const QS_ESTABLISHMENTID As String = "estabID"

        Private _description As String
        Private _queryString As NameValueCollection
        Private _currentStep As Integer
        Private _contractID As Integer
        Private _sysAccID As Integer
        Private _providerID As Integer
        Private _cboPeriod As DropDownListEx
        Private _cboSysAcc As DropDownListEx

        Sub New()
            MyBase.New()
            _description = "Please select a system account to filter the results on."
            'Me.DateFrom = Date.Today
            Me.ShowHeaderLink = False
            Me.HeaderLabelWidth = New Unit(10, UnitType.Em)
            Me.ContentLabelWidth = New Unit(10, UnitType.Em)
        End Sub

        Public Overrides Property QueryString() As System.Collections.Specialized.NameValueCollection
            Get
                Return MyBase.QueryString
            End Get
            Set(ByVal Value As System.Collections.Specialized.NameValueCollection)
                MyBase.QueryString = Value
                _queryString = Value
                _contractID = Utils.ToInt32(_queryString(QS_CONTRACTID))
                _sysAccID = Utils.ToInt32(_queryString(QS_SYSACCID))
                _providerID = Utils.ToInt32(_queryString(QS_ESTABLISHMENTID))
                _currentStep = Utils.ToInt32(_queryString(QS_CURRENTSTEP))
            End Set
        End Property

        Public Overrides Function RenderContentControls(ByVal wizard As SelectorWizardBase, ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage = New ErrorMessage
            Dim spacerBr As Literal
            Dim periods As DomContractPeriodCollection = Nothing

            msg = MyBase.RenderContentControls(wizard, controls)
            If Not msg.Success Then Return msg

            ' get periods
            msg = DomContractPeriod.FetchList(Me.DbConnection, periods, String.Empty, String.Empty, _contractID)
            If Not msg.Success Then Return msg
            periods.Sort(New CollectionSorter("DateFrom", SortDirection.Descending))

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' system account (populated from client-side call to web svc)
            _cboSysAcc = New DropDownListEx()
            With _cboSysAcc
                .LabelText = "System Account"
                .LabelWidth = "10em"
                .LabelBold = True
                .DropDownList.Enabled = True
            End With
            controls.Add(_cboSysAcc)

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            msg.Success = True
            Return msg

        End Function

        Public Overrides Function RenderHeaderControls(ByRef controls As System.Web.UI.ControlCollection) As ErrorMessage

            Dim msg As ErrorMessage
            Dim spacerBr As Literal
            Dim lbl As Label
            Dim text As Label
            Dim link As HyperLink
            Dim contractGroup As GenericContractGroup = Nothing
            Dim endReason As ServiceOrderSuspensionReason = Nothing
            Dim qs As NameValueCollection
            Dim accounts As vwDomContractPeriodSystemAccountCollection = Nothing

            ' date range
            msg = MyBase.RenderHeaderControls(controls)
            If Not msg.Success Then Return msg

            spacerBr = New Literal
            spacerBr.Text = "<br />"
            controls.Add(spacerBr)

            ' system account
            lbl = New Label()
            lbl.Text = "System Account"
            lbl.Style.Add("float", "left")
            lbl.Style.Add("font-weight", "bold")
            lbl.Style.Add("width", "10em")
            controls.Add(lbl)

            text = New Label()
            text.Style.Add("float", "left")
            If _sysAccID > 0 Then
                ' get sys acc
                msg = vwDomContractPeriodSystemAccount.FetchList(Me.DbConnection, accounts, 0, _sysAccID)
                If Not msg.Success Then Return msg
                text.Text = accounts(0).ClientName
            Else
                text.Text = "All"
            End If
            controls.Add(text)

            ' add the link
            link = New HyperLink()
            qs = Me.QueryString
            If Me.IsCurrentStep AndAlso _sysAccID > 0 Then
                ' all
                link.Text = "All System Accounts"
                qs.Remove(QS_SYSACCID)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            ElseIf _currentStep > Me.StepIndex Then
                ' change
                link.Text = "Change System Account"
                qs.Remove(QS_CURRENTSTEP)
                qs.Add(QS_CURRENTSTEP, Me.StepIndex)
                link.NavigateUrl = String.Format("{0}?{1}", Me.BaseUrl, WebUtils.GetNameValueCollectionAsString(qs))
            End If
            If Not String.IsNullOrEmpty(link.Text) Then
                link.Style.Add("float", "right")
                controls.Add(link)
            End If

            msg = New ErrorMessage
            msg.Success = True

            Return msg

        End Function

        Public Overrides ReadOnly Property Title() As String
            Get
                Return "Select a System Account"
            End Get
        End Property

        Public Overrides Sub PreRender(ByVal wizard As SelectorWizardBase)
            Dim dateFrom As Date = Date.MinValue
            Dim dateTo As Date = Date.MaxValue

            If Utils.IsDate(MyBase.DateFromControl.Text) Then dateFrom = MyBase.DateFromControl.Text
            If Utils.IsDate(MyBase.DateToControl.Text) Then dateTo = MyBase.DateToControl.Text

            wizard.Page.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
             Target.Library.Web.Utils.WrapClientScript( _
              String.Format(";DomContractPeriodSysAccFilterStep_systemAccountID='{0}';DomContractPeriodSysAccFilterStep_providerID={1};DomContractPeriodSysAccFilterStep_selectedSystemAccount={2};DomContractPeriodSysAccFilterStep_dateFrom={3};DomContractPeriodSysAccFilterStep_dateTo={4};DomContractPeriodSysAccFilterStep_contractID={5};", _
               _cboSysAcc.ClientID, _providerID, _sysAccID, WebUtils.GetDateAsJavascriptString(dateFrom), WebUtils.GetDateAsJavascriptString(dateTo), _contractID) _
             ) _
            )

        End Sub

        Public Overrides Sub InitStep(ByVal wizard As SelectorWizardBase)
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/DomContractPeriodSysAccFilterStep.js"))
            wizard.AddBasePageJsLink(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/WebSvcUtils.js"))
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.DomContract))
        End Sub

        Public Overrides ReadOnly Property BeforeNavigateJS() As String
            Get
                Return "DomContractPeriodSysAccFilterStep_BeforeNavigate()"
            End Get
        End Property

    End Class

#End Region

End Namespace