Imports Target.Abacus.Library
Imports Target.Library
Imports Target.Library.Web
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.ExpenditureAccountSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the listing and selecting of Expenditure Accounts.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[Paul]	18/02/2009	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ExpenditureAccountSelector
        Inherits System.Web.UI.UserControl

        Private _enableAccoutnTypeCombo As Boolean

        Public Property enableAccountTypeCombo() As Boolean
            Get
                Return _enableAccoutnTypeCombo
            End Get
            Set(ByVal value As Boolean)
                _enableAccoutnTypeCombo = value
            End Set
        End Property

        Public Sub InitControl(ByVal thePage As BasePage, ByVal selectedAccountID As Integer, ByVal selectedServiceType As Integer, ByVal selectedAccountType As Integer, ByVal description As String)

            Dim currentPage As Integer = Target.Library.Utils.ToInt32(Request.QueryString("page"))
            If currentPage <= 0 Then currentPage = 1

            ' add date utility JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))
            ' add utility JS link
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add list filter JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/ListFilter.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/ExpenditureAccountSelector.js"))
            ' add AJAX-generated javascript to the page
            AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Web.Apps.WebSvc.FinanceCodes))

            cboExpAccountType.DropDownList.Attributes.Add("onchange", "expenditureAccountSelector_cboExpaccountType_Click();")
            PopulateDropdowns()
            cboExpAccountType.Enabled = _enableAccoutnTypeCombo

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Target.Abacus.Web.Apps.UserControls.ExpenditureAccountSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(String.Format( _
              "currentPage={0};ExpenditureAccountSelector_selectedAccountID={1};ExpenditureAccountSelector_selectedServiceType={2};ExpenditureAccountSelector_selectedExpenditureType={3};ExpenditureAccountSelector_cboTypeID='{4}';listFilterName='{5}';", currentPage, selectedAccountID, selectedServiceType, selectedAccountType, cboExpAccountType.ClientID, description) _
             ) _
            )

        End Sub

        Private Sub PopulateDropdowns()

            With cboExpAccountType
                With .DropDownList
                    .Items.Clear()
                    .DataSource = BindToEnum(GetType(ExpenditureAccountGroupType))
                    .DataTextField = "Key"
                    .DataValueField = "Value"
                    .DataBind()
                    ' insert a blank at the top
                    .Items.Insert(0, New ListItem(String.Empty, 0))
                End With
            End With

        End Sub

        Public Shared Function BindToEnum(ByVal enumType As Type) As DataTable

            Dim names As String() = ExpenditureAccountGroupType.GetNames(enumType)
            Dim values As Array = ExpenditureAccountGroupType.GetValues(enumType)
            Dim dt As New DataTable
            dt.Columns.Add("Key", GetType(String))
            dt.Columns.Add("Value", GetType(Integer))

            Dim i As Integer = 0
            While i < names.Length
                Dim dr As DataRow = dt.NewRow
                dr("Key") = names(i)
                dr("Value") = CType(values.GetValue(i), Integer)
                dt.Rows.Add(dr)
                i = i + 1
            End While
            Return dt

        End Function

    End Class

End Namespace

