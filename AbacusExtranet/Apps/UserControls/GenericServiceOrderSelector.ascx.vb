Imports Target.Library
Imports Target.Library.Web
Imports Target.Web.Apps
Imports Target.Web.Apps.Security

Namespace Apps.UserControls

    Partial Class GenericServiceOrderSelector
        Inherits System.Web.UI.UserControl

        Private _thePage As BasePage
        Public Property thePage() As BasePage
            Get
                Return _thePage
            End Get
            Set(ByVal value As BasePage)
                _thePage = value
            End Set
        End Property

        Private _establishmentId As Integer
        Public Property establishmentId() As Integer
            Get
                Return _establishmentId
            End Get
            Set(ByVal value As Integer)
                _establishmentId = value
            End Set
        End Property

        Private _contractId As Integer
        Public Property contractId() As Integer
            Get
                Return _contractId
            End Get
            Set(ByVal value As Integer)
                _contractId = value
            End Set
        End Property

        Private _dateFrom As Date
        Public Property dateFrom() As Date
            Get
                Return _dateFrom
            End Get
            Set(ByVal value As Date)
                _dateFrom = value
            End Set
        End Property

        Private _dateTo As Date
        Public Property dateTo() As Date
            Get
                Return _dateTo
            End Get
            Set(ByVal value As Date)
                _dateTo = value
            End Set
        End Property

        Private _movement As Integer
        Public Property movement() As Integer
            Get
                Return _movement
            End Get
            Set(ByVal value As Integer)
                _movement = value
            End Set
        End Property

        Private _selectedServiceOrderId As Integer
        Public Property selectedServiceOrderId() As Integer
            Get
                Return _selectedServiceOrderId
            End Get
            Set(ByVal value As Integer)
                _selectedServiceOrderId = value
            End Set
        End Property

        Private Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
            Rendercontrols()
        End Sub

        Public Sub Rendercontrols()

            Dim documentsAccessible As Boolean = False

            Dim weekEndingDate As DateTime = Target.Abacus.Library.DomContractBL.GetWeekEndingDate(thePage.DbConnection, Nothing)
            costWeekEndingDate.AllowableDays = weekEndingDate.DayOfWeek
            detailWeekEndingDate.AllowableDays = weekEndingDate.DayOfWeek

            With thePage
                ' add in the jquery library
                .UseJQuery = True

                ' add in the jquery ui library for popups and table filtering etc
                .UseJqueryUI = True

                ' add in the table filter library 
                .UseJqueryTableFilter = True

                ' add the table scroller library as we might have large amounts of data
                .UseJqueryTableScroller = True

                ' add the searchable menu
                .UseJquerySearchableMenu = True

                ' add the templates pluggin
                .UseJqueryTemplates = True

                'add the jquery tooltip library
                .UseJqueryTooltip = True

                ' add page JS
                .JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("AbacusExtranet/Apps/UserControls/GenericServiceOrderSelector.js"))

                ' add date utility JS
                .JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/date.js"))

                ' add web service utils
                .JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/JavaScript/WebSvcUtils.js"))

                ' add AJAX-generated javascript to the page
                AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.GenericServiceOrder))
                AjaxPro.Utility.RegisterTypeForAjax(GetType(Target.Abacus.Extranet.Apps.WebSvc.Documents))


                documentsAccessible = SecurityBL.UserHasMenuItem(.DbConnection, SecurityBL.GetCurrentUser().ID, _
                                                              Target.Library.Web.ConstantsManager.GetConstant( _
                                                              "AbacusExtranet.WebNavMenuItem.Documents"), _
                                                              .Settings.CurrentApplicationID)



                thePage.ClientScript.RegisterStartupScript(Me.GetType(), "Startup", _
                                                            String.Format("establishmentID={0};domContractID={1};dateFrom=""{2}"";dateTo=""{3}"";movement={4};showDocumentTab={5};dtpDetailId='{6}'; dtpCostId='{7}'  ", _
                                                            establishmentID, contractID, dateFrom, dateTo, movement, documentsAccessible.ToString.ToLower, detailWeekEndingDate.ClientID, costWeekEndingDate.ClientID), True)


            End With





        End Sub

      
    End Class

End Namespace