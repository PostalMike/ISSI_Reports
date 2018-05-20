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
		<asp:ListItem Value="0">User Zero</asp:ListItem>
	</asp:DropDownList>
	<asp:Label ID="lblStartDate" runat="server" AssociatedControlID="txtStartDate" Text="Begin Date: " />
	<asp:TextBox ID="txtStartDate" runat="server"></asp:TextBox>
	<asp:Label ID="lblEndingDate" runat="server" AssociatedControlID="txtEndingDate" Text="End Date: "/>
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
	//function MyNewFunction() {

	//	var begin = $("[id$='txtStartDate']").val();
	//	var end = $("[id$='txtEndingDate']").val();

	//	//alert('now begin is ' + begin.length + ' now and end is ' + end.length);

	//	if (begin.length > 0 && end.length > 0) {
	//		window.location.href = window.location.href.split('?')[0] + '?'
	//		+ 'beginDate=' + begin + '&endDate=' + end;
	//		alert(window.location.href);
	//	}
	//	else {
	//		alert('pick both a begin and end date');
	//	}
	//}

	$(function () {
		$("#<%= txtStartDate.ClientID %>").datepicker();
		$("#<%= txtEndingDate.ClientID %>").datepicker();
	});
</script>
