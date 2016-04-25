<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_Approve_Main" %>
<%@ Register Src="List.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="Search.ascx" TagName="Search" TagPrefix="uc2" %>
<%@ Register Src="~/ISI/Checkup/Edit.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="~/ISI/Summary/EditMain.ascx" TagName="Summary" TagPrefix="uc2" %>
<uc2:Search ID="ucSearch" runat="server" Visible="True" />
<uc2:List ID="ucList" runat="server" Visible="false" />
<uc2:Edit ID="ucEdit" runat="server" Visible="false" />
<uc2:Summary ID="ucSummary" runat="server" Visible="false" />