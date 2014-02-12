Imports System.ComponentModel
Imports System.Configuration.ConfigurationManager
Imports System.Collections.Generic
Imports System.Data.SqlClient
Imports Target.Abacus.Library
Imports Target.Abacus.Library.DataClasses
Imports Target.Abacus.Library.DataClasses.Collections
Imports Target.Library
Imports Target.Library.Web.UserControls
Imports WebUtils = Target.Library.Web.Utils
Imports Target.Web.Apps
Imports Target.Web.Apps.Security
Imports System.Reflection
Imports System.Text
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports AjaxControlToolkit


Namespace Apps.Admin.Dom
    ''' <summary>
    ''' Screen used to manage ordering of rate categories.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' Mo Tahir 10/05/2010  D11806.
    ''' </history>
    Partial Public Class RateOrdering
        Inherits BasePage

        Private _frameworkID As Integer
        Private _framework As DomRateFramework
        Private _stdBut As StdButtonsBase
        Private _ShowDragHandle As [Boolean]
        Private _rcList As DomRateCategoryCollection

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim js As StringBuilder = New StringBuilder()
            _rcList = New DomRateCategoryCollection

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("AbacusIntranet.WebNavMenuItem.RateCategories"), "Rate Category Sort Order")

            _stdBut = Me.Master.FindControl("MPContent").FindControl("stdButtons1")

            With _stdBut
                .AllowBack = True
                .AllowNew = False
                .AllowEdit = True
                .AllowDelete = False
                .AllowFind = False
                .EditableControls.Add(fsControls.Controls)
            End With
            AddHandler _stdBut.FindClicked, AddressOf FindClicked
            AddHandler _stdBut.EditClicked, AddressOf FindClicked
            AddHandler _stdBut.SaveClicked, AddressOf SaveClicked
            AddHandler _stdBut.CancelClicked, AddressOf CancelClicked

            AjaxPro.Utility.RegisterEnumForAjax(GetType(Target.Library.Web.UserControls.StdButtonsMode))
            ' add utility JS link
            Me.JsLinks.Add(Target.Library.Web.Utils.GetVirtualPath("Library/Javascript/WebSvcUtils.js"))

            Me.ClientScript.RegisterStartupScript(Me.GetType(), "Startup2", js.ToString(), True)


            Dim msg As ErrorMessage
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()

            _frameworkID = Target.Library.Utils.ToInt32(Request.QueryString("frameworkid"))

            PopulateRateFramework(_frameworkID)

            msg = DomRateCategory.FetchList(conn:=Me.DbConnection, list:=_rcList, auditUserName:=String.Empty, _
                                            auditLogTitle:=String.Empty, domRateFrameworkID:=_frameworkID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            reOrderListRateCategories.DataSource = _rcList
            reOrderListRateCategories.DataBind()

            If Not IsPostBack Then
                ShowDragHandle = True
            End If

        End Sub
        Private Sub FindClicked(ByRef e As StdButtonEventArgs)

            Dim msg As ErrorMessage
            Dim category As DomRateCategory
            Dim currentUser As WebSecurityUser = SecurityBL.GetCurrentUser()
            _rcList = New DomRateCategoryCollection

            category = New DomRateCategory(Me.DbConnection, currentUser.ExternalUsername, AuditLogging.GetAuditLogTitle(Me.PageTitle, Me.Settings))

            PopulateRateFramework(_frameworkID)

            msg = DomRateCategory.FetchList(conn:=Me.DbConnection, list:=_rcList, auditUserName:=String.Empty, _
                                            auditLogTitle:=String.Empty, domRateFrameworkID:=_frameworkID)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            If Not IsPostBack Then
                reOrderListRateCategories.DataSource = RateCategoryItems(_rcList)
                reOrderListRateCategories.DataBind()
            End If
        End Sub
        Private Sub CancelClicked(ByRef e As StdButtonEventArgs)
            ViewState.Clear()
            txtRateFramework.Text = String.Empty
            FindClicked(e)
            reOrderListRateCategories.DataSource = RateCategoryItems(_rcList)
            reOrderListRateCategories.DataBind()
        End Sub
        Private Sub SaveClicked(ByRef e As StdButtonEventArgs)
            Dim items As New DomRateCategoryCollection()
            items = RateCategoryItems(Nothing)
            Dim trans As SqlTransaction = Nothing
            Dim rateCategory As DomRateCategory = Nothing
            Dim msg As ErrorMessage = New ErrorMessage
            Try

                trans = SqlHelper.GetTransaction(Me.DbConnection)

                For Each item As DomRateCategory In items
                    rateCategory = New DomRateCategory(trans, String.Empty, String.Empty)
                    msg = rateCategory.Fetch(item.ID)
                    If Not msg.Success Then
                        trans.Rollback()
                        WebUtils.DisplayError(msg)
                    Else
                        With rateCategory
                            .SortOrder = items.IndexOf(rateCategory)
                            msg = .Save()
                            If Not msg.Success Then
                                trans.Rollback()
                                WebUtils.DisplayError(msg)
                            End If
                        End With
                    End If
                Next

                trans.Commit()

            Catch ex As Exception
                msg = Utils.CatchError(ex, "E0001 Rate Ordering Save")     ' unexpected
                e.Cancel = True
            Finally
                If Not msg.Success Then SqlHelper.RollbackTransaction(trans)
            End Try
        End Sub
        Private Sub PopulateRateFramework(ByVal frameworkID As Integer)

            Dim msg As ErrorMessage

            _frameworkID = frameworkID

            _framework = New DomRateFramework(Me.DbConnection, String.Empty, String.Empty)
            msg = _framework.Fetch(frameworkID)
            If Not msg.Success Then WebUtils.DisplayError(msg)
            txtRateFramework.Text = _framework.Description

        End Sub

#Region " Rate Category Order Control Code "
        Protected Property ShowDragHandle() As [Boolean]
            Get
                Return _ShowDragHandle
            End Get
            Set(ByVal value As [Boolean])
                _ShowDragHandle = value
            End Set
        End Property
        Private Property RateCategoryItems(ByVal rcCollection As DomRateCategoryCollection) As DomRateCategoryCollection
            Get
                'We assume the array of items will be small, so we use viewstate 
                ' If the array were big you may need to use session, the cache // API, or even a database or filesystem to store the items // between postbacks. 

                Dim items As DomRateCategoryCollection = ViewState("RateCategoryItems")
                If items Is Nothing Then
                    ' items are not in viewstate, read from data store 
                    items = rcCollection
                    'get values from the data store 
                    'shove into viewstate 
                    ViewState("RateCategoryItems") = items
                End If
                Return items
            End Get
            Set(ByVal value As DomRateCategoryCollection)
                ViewState("RateCategoryItems") = value
            End Set
        End Property
        Protected Sub OrderListRateCategories_ItemReorder(ByVal sender As Object, ByVal e As ReorderListItemReorderEventArgs)
            ShowDragHandle = True
            Dim items As New DomRateCategoryCollection()
            items = RateCategoryItems(Nothing)
            'using a list for the reordering (convienience) 
            Dim itemToMove As DomRateCategory = items(e.OldIndex)
            items.Remove(itemToMove)
            items.Insert(e.NewIndex, itemToMove)
            'you could save this to the DB now, but this example uses a //save button to batch up changes 
            RateCategoryItems(Nothing) = items
        End Sub
#End Region

        Private Sub RateOrdering_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Not ViewState("RateCategoryItems") Is Nothing Then
                reOrderListRateCategories.DataSource = RateCategoryItems(Nothing)
                reOrderListRateCategories.DataBind()
            End If
        End Sub
    End Class
End Namespace
