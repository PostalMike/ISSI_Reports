<%@ Control Language="vb" Inherits="DotNetNuke.Modules.DTSReports.ViewReports" AutoEventWireup="false"
	Explicit="True" CodeBehind="ViewReports.ascx.vb" %>
<asp:Panel ID="InfoPane" runat="server" Visible="false" EnableViewState="false">
	<h1>
		<asp:Literal ID="TitleLiteral" runat="server" Text="Title goes here" EnableViewState="false" /></h1>
	<p class="NormalBold">
		<asp:Literal ID="DescriptionLiteral" runat="server" Text="Description goes here"
			EnableViewState="false" />
	</p>
</asp:Panel>
<asp:PlaceHolder ID="ControlsPane" runat="server" Visible="false" EnableViewState="false">
	<asp:Label ID="lblUserName" runat="server" AssociatedControlID="ddlUserName" Text="User Name: " />
	<asp:DropDownList ID="ddlUserName" runat="server">
		<asp:ListItem Value="0">All Users</asp:ListItem>
	</asp:DropDownList>
	<asp:Label ID="lblStartDate" runat="server" AssociatedControlID="txtStartDate" Text="Begin Date: " />
	<asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox>
	<asp:Label ID="lblEndingDate" runat="server" AssociatedControlID="txtEndingDate" Text="End Date: " />
	<asp:TextBox ID="txtEndingDate" runat="server"></asp:TextBox>
	<p>
		<asp:LinkButton ID="RunReportButton" runat="server" ResourceKey="RunReportButton"
			CssClass="CommandButton" EnableViewState="false" OnClientClick="MyNewFunction()" />&nbsp;
        <asp:LinkButton ID="ClearReportButton" runat="server" ResourceKey="ClearReportButton"
			CssClass="CommandButton" Visible="false" EnableViewState="false" />
	</p>
</asp:PlaceHolder>
<asp:PlaceHolder ID="VisualizerSection" runat="server" />
<script type="text/javascript">
	$(function () {
		var newDate = new Date();
		var defDate = ('"' + ('0' + (newDate.getMonth() + 1)) + '/' + (newDate.getDate() - 7) + '/' + newDate.getFullYear() + '"');
		alert(defDate);
		$("#<%= txtStartDate.ClientID %>").datepicker({ dateFormat: 'dd/mm/yyyy' });
		$("#<%= txtStartDate.ClientID %>").datepicker('option', 'defaultDate', defDate); 
		$("#<%= txtEndingDate.ClientID %>").datepicker();
		$("#<%= txtEndingDate.ClientID %>").datepicker('setDate', 'today');
	});
</script>
