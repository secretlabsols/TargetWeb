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
Imports Target.Web.Apps.Licensing
Imports System.Reflection
Imports System.Text
Imports Target.Library.ApplicationBlocks.DataAccess
Imports Target.Library.Web.Controls
Imports AjaxControlToolkit


Namespace Apps.Admin.About
    ''' <summary>
    ''' Screen used to manage licences.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' MikeVO   13/07/2010  Tidied up CSS, spellings and GetVersionInformation() calls.
    ''' Mo Tahir 15/06/2010  D11829.
    ''' </history>
    Partial Public Class Info
        Inherits BasePage

        Const CTRL_ROW_SUFFIX As String = "_Row"

        Private _stdBut As StdButtonsBase
        Private _appInfo As ApplicationInfo = Nothing

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim js As StringBuilder = New StringBuilder()
            Dim msg As ErrorMessage
            Dim list As New List(Of ViewableLicence)

            Me.InitPage(Target.Library.Web.ConstantsManager.GetConstant("All.WebNavMenuItem.FreeView"), "About")
            Me.RenderMenu = SecurityBL.IsUserLoggedOn()

            msg = ModuleLicence.GetLicenceModules(Me.DbConnection, list)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = SecurityBL.GetVersionInformation(Me.DbConnection, Me.Settings, _appInfo)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            OutputSiteHeaders()
            OutputSiteInfo()

            OutputLicenceHeaders()
            OutputLicenceInfo(list)

            OutputVersionHeaders()
            OutputVersionInfo()

            OutputAssemblyHeaders()
            OutputAssemblyInfo()

        End Sub

        Private Sub OutputLicenceHeaders()
            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell

            phLicencesHeading.Controls.Clear()

            row = New HtmlTableRow()
            row.ID = "LicenceHeader"

            phLicencesHeading.Controls.Add(row)

            ' Module Name
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Module<br/>Name"

            ' Module Reference
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Module<br/>Reference"

            ' Description
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Description"

            ' Licenced
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Licenced ?"

            ' Expiry Date
            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Expiry<br/>Date"

        End Sub

        Private Sub OutputLicenceInfo(ByVal list As List(Of ViewableLicence))

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell

            phLicences.Controls.Clear()

            For Each webMod As ViewableLicence In list
                row = New HtmlTableRow()
                row.ID = webMod.ID & CTRL_ROW_SUFFIX
                phLicences.Controls.Add(row)

                ' Module Name
                cell = New HtmlTableCell()
                cell.InnerHtml = webMod.Name
                row.Controls.Add(cell)

                ' Module Reference
                cell = New HtmlTableCell()
                cell.InnerHtml = webMod.Reference
                row.Controls.Add(cell)

                ' Module Description
                cell = New HtmlTableCell()
                cell.InnerHtml = webMod.Description
                row.Controls.Add(cell)

                ' Licenced
                cell = New HtmlTableCell()
                row.Controls.Add(cell)
                If webMod.Licenced Then
                    cell.InnerHtml = "YES"
                Else
                    cell.InnerHtml = "NO"
                End If

                cell = New HtmlTableCell()
                With cell
                    If Not webMod.ExpiryDate Is Nothing Then
                        .InnerHtml = ModuleLicence.FormatLicenceDate(webMod.ExpiryDate)
                        .BgColor = ModuleLicence.GetExpiryDateColour(webMod.ExpiryDate)
                    Else
                        .InnerHtml = "&nbsp"
                    End If
                End With
                row.Controls.Add(cell)
            Next

        End Sub

        Private Sub OutputSiteHeaders()
            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell

            phSiteInfoHeading.Controls.Clear()

            row = New HtmlTableRow()
            row.ID = "SiteInfoHeader"

            phSiteInfoHeading.Controls.Add(row)

            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Licenced To"

            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Licence No"

            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Licenced Users"
        End Sub

        Private Sub OutputSiteInfo()

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim sysInfo As New SystemInfoCollection
            Dim msg As ErrorMessage
            Dim concurrentUserLimit As Integer = 0

            msg = SystemInfo.FetchList(conn:=Me.DbConnection, list:=sysInfo)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            msg = SecurityBL.GetConcurrentUserLoginLimit(ConnectionStrings("Abacus").ConnectionString, concurrentUserLimit)
            If Not msg.Success Then WebUtils.DisplayError(msg)

            phSiteInfo.Controls.Clear()

            row = New HtmlTableRow()
            row.ID = sysInfo(0).SiteName & CTRL_ROW_SUFFIX
            phSiteInfo.Controls.Add(row)

            cell = New HtmlTableCell()
            cell.InnerHtml = sysInfo(0).SiteName
            row.Controls.Add(cell)

            cell = New HtmlTableCell()
            cell.InnerHtml = sysInfo(0).LicenceNo
            row.Controls.Add(cell)

            cell = New HtmlTableCell()
            cell.InnerHtml = concurrentUserLimit
            row.Controls.Add(cell)

        End Sub

        Private Sub OutputVersionHeaders()
            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell

            phVersionInfoHeading.Controls.Clear()

            row = New HtmlTableRow()
            row.ID = "VerisonInfoHeader"

            phVersionInfoHeading.Controls.Add(row)

            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Database Version"

            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = ".NET Framework Version"

            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Compiler Version"

        End Sub

        Private Sub OutputVersionInfo()

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim section As ApplicationInfoSection

            phVersionInfo.Controls.Clear()

            row = New HtmlTableRow()
            row.ID = "test" & CTRL_ROW_SUFFIX
            phVersionInfo.Controls.Add(row)

            'database info
            section = _appInfo.Sections(0)

            cell = New HtmlTableCell()
            cell.InnerHtml = section.Details(0).ToString
            row.Controls.Add(cell)

            section = _appInfo.Sections(1)

            cell = New HtmlTableCell()
            cell.InnerHtml = section.Details(0).ToString
            row.Controls.Add(cell)

            cell = New HtmlTableCell()
            cell.InnerHtml = section.Details(1).ToString
            row.Controls.Add(cell)

        End Sub

        Private Sub OutputAssemblyHeaders()
            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell

            phAssemblyInfoHeading.Controls.Clear()

            row = New HtmlTableRow()
            row.ID = "AssemblyHeader"

            phAssemblyInfoHeading.Controls.Add(row)

            cell = New HtmlTableCell("th")
            row.Controls.Add(cell)
            cell.Style.Add("vertical-align", "top")
            cell.InnerHtml = "Assembly Information"

        End Sub

        Private Sub OutputAssemblyInfo()

            Dim row As HtmlTableRow
            Dim cell As HtmlTableCell
            Dim section As ApplicationInfoSection
            Dim assemblyDetails As ArrayList
            Dim assemlyInfo As Object

            phAssemblyInfo.Controls.Clear()

            section = _appInfo.Sections(2)
            assemblyDetails = section.Details

            For Each assemlyInfo In assemblyDetails
                row = New HtmlTableRow()
                row.ID = assemblyDetails.IndexOf(assemlyInfo) & CTRL_ROW_SUFFIX
                phAssemblyInfo.Controls.Add(row)

                cell = New HtmlTableCell()
                cell.InnerHtml = assemlyInfo.ToString
                row.Controls.Add(cell)
            Next
        End Sub

    End Class
End Namespace
