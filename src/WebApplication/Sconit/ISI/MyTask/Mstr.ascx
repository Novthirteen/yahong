<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Mstr.ascx.cs" Inherits="ISI_Mstr_Main" %>
<%@ Register Src="~/ISI/TSK/EditMain.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="Search.ascx" TagName="Search" TagPrefix="uc2" %>
<%@ Register Src="List.ascx" TagName="List" TagPrefix="uc2" %>
<uc2:Search ID="ucSearch" runat="server" Visible="true" />
<uc2:Edit ID="ucEdit" runat="server" Visible="false" />
<uc2:List ID="ucList" runat="server" Visible="false" />
