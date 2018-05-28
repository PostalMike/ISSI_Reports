'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2006
' by Perpetual Motion Interactive Systems Inc. ( http://www.perpetualmotion.ca )
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'
Imports DotNetNuke

Imports System.IO
Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Reflection

Imports UISkin = DotNetNuke.UI.Skins.Skin
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security
Imports DotNetNuke.Modules.DTSReports.Exceptions
Imports System.Web.Compilation
Imports DotNetNuke.Framework
Imports DotNetNuke.Modules.DTSReports.Visualizers
Imports DotNetNuke.Modules.DTSReports.Extensions
Imports DotNetNuke.Entities.Modules

Namespace DotNetNuke.Modules.DTSReports

	''' -----------------------------------------------------------------------------
	''' <summary>
	''' The ViewReports class displays the content
	''' </summary>
	''' <remarks>
	''' </remarks>
	''' <history>
	''' </history>
	''' -----------------------------------------------------------------------------
	Partial Class ViewReports
		Inherits Entities.Modules.PortalModuleBase
		Implements Entities.Modules.IActionable

		Private Report As ReportInfo
		Private beenHere As Boolean = False

#Region " Error Handlers "

		Private Sub HandleVisualizerException(ByVal vex As VisualizerException)
			If IsEditable Then
				DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me,
											String.Format(Localization.GetString("VisualizerError.Message",
																				 Me.LocalResourceFile),
														  vex.LocalizedMessage),
											DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
			End If
		End Sub

		Private Sub HandleDataSourceException(ByVal ex As DataSourceException)
			If Me.UserInfo.IsSuperUser Then
				UISkin.AddModuleMessage(Me,
				String.Format(Localization.GetString("HostDSError.Message", Me.LocalResourceFile), ex.LocalizedMessage),
					DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
			ElseIf IsEditable Then
				UISkin.AddModuleMessage(Me, Localization.GetString("AdminDSError.Message", Me.LocalResourceFile),
					DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
			End If
		End Sub

		Private Sub HandleMissingVisualizerError()
			If IsEditable Then
				DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me,
					Localization.GetString("VisualizerDoesNotExist.Text",
					Me.LocalResourceFile),
					DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
			End If
		End Sub

		Private Sub HandleVisualizerLoadError()
			DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me,
										Localization.GetString("VisualizerLoadError.Text",
										Me.LocalResourceFile),
										DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
		End Sub

#End Region


#Region " Helper Methods "

		Private Function VisualizerFolderExists(ByVal visualizerFolder As String) As Boolean

			Return Directory.Exists(Server.MapPath(Me.ResolveUrl(String.Format("Visualizers/{0}", visualizerFolder))))

		End Function

		Private Function GetVisualizerFolder() As String
			Dim sVisualizerFolder As String = Convert.ToString(Settings.Item(ReportsController.SETTING_Visualizer))
			If String.IsNullOrEmpty(sVisualizerFolder) OrElse Not VisualizerFolderExists(sVisualizerFolder) Then
				If VisualizerFolderExists("Grid") Then
					' Default to grid if its installed, otherwise default to none selected
					' This should cover most upgrades from pre-Visualizer versions as long as the
					' user sticks with the default and installs Grid, if not, then they are advanced
					' enough to know what to do.

					sVisualizerFolder = "Grid"
				ElseIf IsEditable Then
					DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me,
						Localization.GetString("VisualizerNotConfigured.Text",
						Me.LocalResourceFile),
						DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
				End If
			End If
			Return sVisualizerFolder
		End Function

		Private Function LoadVisualizerControl(ByVal sVisualizerControl As String, ByVal sVisualizerName As String) As VisualizerControlBase
			Dim ctrlVisualizer As VisualizerControlBase = Nothing
			Try
				' Load the visualizer control
				ctrlVisualizer = TryCast(Me.LoadControl(sVisualizerControl), VisualizerControlBase)
				ctrlVisualizer.Initialize(New ExtensionContext(Me.TemplateSourceDirectory, "Visualizer", sVisualizerName))
			Catch vex As VisualizerException
				HandleVisualizerException(vex)
				DotNetNuke.Services.Exceptions.Exceptions.LogException(vex)
			Catch ex As HttpCompileException
				DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
			End Try
			Return ctrlVisualizer
		End Function

		Private Function AutoExecuteReport(ByVal ctlVisualizer As VisualizerControlBase, ByVal report As ReportInfo, ByRef results As DataTable, ByVal fromCache As Boolean) As Boolean
			Try
				results = ReportsController.ExecuteReport(report, String.Concat(ReportsController.CACHEKEY_Reports, ModuleId), report.CacheDuration <= 0, Me, fromCache)

			Catch ex As DataSourceException
				' Display the error message to host users only
				If Me.UserInfo.IsSuperUser Then
					DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me,
						String.Format(Localization.GetString("HostExecuteError.Message",
															 Me.LocalResourceFile),
									  ex.LocalizedMessage),
						DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
				ElseIf IsEditable Then
					DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me,
						Localization.GetString("AdminExecuteError.Message",
						Me.LocalResourceFile),
						DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
				End If
				DotNetNuke.Services.Exceptions.Exceptions.LogException(ex)
				ctlVisualizer.Visible = False
				Return False
			End Try
			Return True
		End Function

		Private Sub RunReport()

			' Check for a visualizer
			Dim sVisualizerFolder As String = GetVisualizerFolder()

			' If the visualizer folder is now non-empty
			If Not String.IsNullOrEmpty(sVisualizerFolder) Then
				' Find the visualizer control
				Dim sVisualizerControl As String = Me.ResolveUrl(String.Format("Visualizers/{0}/{1}", sVisualizerFolder, VisualizerControlBase.FILENAME_VisualizerASCX))
				If Not File.Exists(Server.MapPath(sVisualizerControl)) Then
					HandleMissingVisualizerError()
				Else
					Dim ctlVisualizer As VisualizerControlBase = LoadVisualizerControl(sVisualizerControl, sVisualizerFolder)
					If ctlVisualizer IsNot Nothing Then
						ctlVisualizer.ID = "Visualizer"
						ctlVisualizer.ParentModule = Me
						Dim results As DataTable = Nothing
						Dim fromCache As Boolean = False
						If ctlVisualizer.AutoExecuteReport AndAlso
						   Not AutoExecuteReport(ctlVisualizer, Report, results, fromCache) Then
							Return
						End If
						ctlVisualizer.SetReport(Report, results, fromCache)
						Me.VisualizerSection.Controls.Clear()
						Me.VisualizerSection.Controls.Add(ctlVisualizer)
					ElseIf IsEditable Then
						HandleVisualizerLoadError()
					End If
				End If
			End If
		End Sub

#End Region

#Region " Event Handlers "

		Protected Sub Page_Error(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Error
			Dim ex As Exception = Server.GetLastError()
			If TypeOf ex Is DataSourceException Then
				HandleDataSourceException(DirectCast(ex, DataSourceException))
			ElseIf TypeOf ex Is VisualizerException Then
				HandleVisualizerException(DirectCast(ex, VisualizerException))
			Else
				MyBase.OnError(e)
			End If
		End Sub

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
			If UserInfo.IsSuperUser AndAlso Directory.Exists(Server.MapPath("App_Code/Reports")) Then
				UISkin.AddModuleMessage(Me, Localization.GetString("CleanUpOldAppCode.Text", Me.LocalResourceFile),
										UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
			End If
			Report = ReportsController.GetReport(ModuleId, TabModuleId)
			InfoPane.Visible = Report.ShowInfoPane
			ControlsPane.Visible = Report.ShowControls
			If Report.ShowInfoPane Then
				TitleLiteral.Text = Report.Title
				DescriptionLiteral.Text = Report.Description
			End If
			'durthaler added code
			If Not IsPostBack Then
				LoadUsers()
			End If

			'modify the query setting to use the parameters.
			Dim mc As New ModuleController
			Dim sqlUser As String = ""

			If ddlUserName.SelectedIndex > 0 Then
				sqlUser = " AND USER_NAME =  " + "'" + ddlUserName.SelectedItem.Text + "'"
			End If

			Dim sqlSelect As String = "
					SELECT 
						USER_NAME
						, COUNT(1) 

					FROM [dbo].[vw_UserSearches] "

			Dim sqlWhere As String = "
					WHERE insertedon BETWEEN" + "'" + txtStartDate.Text + "'" + " AND " + "'" + txtEndingDate.Text + "'" + " AND USER_ID > 2 "

			Dim sqlGroupBy As String = "

					GROUP BY 
						USER_NAME

				"
			Dim sqlResult As String = sqlSelect + sqlWhere + sqlUser + sqlGroupBy
			mc.UpdateModuleSetting(ModuleId, "dnn_ReportsDS_DotNetNuke_Query", sqlResult)
			'end durthaler added code
			If ClearReportButton.Visible = False Then
				RunReport()
				'ClearReportButton.Visible = True
				If Report.AutoRunReport Then
					RunReport()
				End If
			End If

		End Sub
		Dim selUser As String = ""
		'Dim isUsersLoaded As Boolean = False
		Private Sub LoadUsers()
			For Each ui As Entities.Users.UserInfo In Entities.Users.UserController.GetUsers(PortalId)
				'If ui.IsInRole("Registered Users") And ui.UserID > 2 Then
				If ui.UserID > 2 Then
					Dim li As ListItem = New ListItem
					li.Text = ui.FullName
					li.Value = ui.UserID.ToString()
					ddlUserName.Items.Add(li)
				End If
			Next
			ddlUserName.DataBind()
			'isUsersLoaded = True
		End Sub
		Private Sub RunReportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles RunReportButton.Click
			If ClearReportButton.Visible = True Then
				'Exit Sub
			End If
			RunReport()
		End Sub
		Protected Sub ddlUserName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlUserName.SelectedIndexChanged
			Session.Add("selectedUser", ddlUserName.SelectedItem.Text)
			Session.Add("selectedVallue", ddlUserName.SelectedValue)
		End Sub
		Private Sub ClearReportButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles ClearReportButton.Click
			Me.VisualizerSection.Controls.Clear()
			ClearReportButton.Visible = False
			'todo: get the page name dynamically
			'Response.Redirect("VIS/Reports/SearchesbyUser.aspx")
		End Sub


#End Region

#Region " IActionable Members "

		Public ReadOnly Property ModuleActions() As Entities.Modules.Actions.ModuleActionCollection Implements Entities.Modules.IActionable.ModuleActions
			Get
				Dim actions As New Entities.Modules.Actions.ModuleActionCollection
				actions.Add(Me.GetNextActionID(),
					Localization.GetString("ManagePackages.Action", Me.LocalResourceFile),
					"", "", Me.ResolveUrl("images/ManagePackages.gif"),
					EditUrl("ManagePackages"), False, SecurityAccessLevel.Host, True, False)
				Return actions
			End Get
		End Property

#End Region

	End Class

End Namespace
