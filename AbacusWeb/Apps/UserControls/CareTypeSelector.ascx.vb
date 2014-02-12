Imports Target.Web.Apps

Namespace Apps.UserControls

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Target.Abacus.Web
    ''' Class	 : Abacus.Web.Apps.UserControls.CareTypeSelector
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    '''     User control to encapsulate the Selection of a care type.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[PaulW]	    06/07/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class CareTypeSelector
        Inherits System.Web.UI.UserControl

        Private _enableRes As Boolean
        Private _enableNonRes As Boolean
        Private _enableDP As Boolean
        Private _showRes As Boolean
        Private _showNonRes As Boolean
        Private _showDP As Boolean
        Private _defaultValue As Byte

        Public Enum CareTypeType As Byte
            Residential = 1
            NonResidential = 2
            DirectPayment = 3
        End Enum

        Public Property enableRes() As Boolean
            Get
                Return _enableRes
            End Get
            Set(ByVal value As Boolean)
                _enableRes = value
            End Set
        End Property

        Public Property showRes() As Boolean
            Get
                Return _showRes
            End Get
            Set(ByVal value As Boolean)
                _showRes = value
            End Set
        End Property

        Public Property enableNonRes() As Boolean
            Get
                Return _enableNonRes
            End Get
            Set(ByVal value As Boolean)
                _enableNonRes = value
            End Set
        End Property

        Public Property showNonRes() As Boolean
            Get
                Return _showNonRes
            End Get
            Set(ByVal value As Boolean)
                _showNonRes = value
            End Set
        End Property

        Public Property enableDP() As Boolean
            Get
                Return _enableDP
            End Get
            Set(ByVal value As Boolean)
                _enableDP = value
            End Set
        End Property

        Public Property showDP() As Boolean
            Get
                Return _showDP
            End Get
            Set(ByVal value As Boolean)
                _showDP = value
            End Set
        End Property

        Public Property defaultValue() As CareTypeType
            Get
                Return _defaultValue
            End Get
            Set(ByVal value As CareTypeType)
                _defaultValue = value
            End Set
        End Property


        Public Sub InitControl(ByVal thePage As BasePage)
            ' add dialog JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/Dialog.js"))
            ' add page JS
            thePage.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusWeb/Apps/UserControls/CareTypeSelector.js"))
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Dim js As String
            Dim thePage As BasePage = CType(Me.Page, BasePage)

            js = String.Format( _
             "CareTypeSelector_showRes={0};CareTypeSelector_enableRes={1};CareTypeSelector_showNonRes={2};CareTypeSelector_enableNonRes={3};CareTypeSelector_showDP={4};CareTypeSelector_enableDP={5};CareTypeSelector_optResID='{6}';CareTypeSelector_optNonResID='{7}';CareTypeSelector_optDPID='{8}';divResID='{9}';divNonResID='{10}';divDPID='{11}';defaultValue={12};", _
             _showRes.ToString.ToLower, _enableRes.ToString.ToLower, _showNonRes.ToString.ToLower, _enableNonRes.ToString.ToLower, _showDP.ToString.ToLower, _enableDP.ToString.ToLower, optRes.ClientID, optNonRes.ClientID, optDP.ClientID, divRes.ClientID, divNonRes.ClientID, divDP.ClientID, _defaultValue)

            thePage.ClientScript.RegisterStartupScript(Me.GetType(), _
             "Target.Abacus.Web.Apps.UserControls.CareTypeSelector.Startup", _
             Target.Library.Web.Utils.WrapClientScript(js) _
            )
        End Sub
    End Class

End Namespace