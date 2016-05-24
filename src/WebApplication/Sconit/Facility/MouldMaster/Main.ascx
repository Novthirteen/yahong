<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_MouldMaster_Main" %>
<%@ Register Src="~/Facility/MouldMaster/EditMain.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/MouldMaster/List.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/MouldMaster/New.ascx" TagName="New" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/MouldMaster/Search.ascx" TagName="Search" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/MouldMaster/Import.ascx" TagName="Import" TagPrefix="uc2" %>

<uc2:Search ID="ucSearch" runat="server" Visible="True" />
<uc2:Edit ID="ucEdit" runat="server" Visible="false" />
<uc2:List ID="ucList" runat="server" Visible="false" />
<uc2:New ID="ucNew" runat="server" Visible="false" />
<uc2:Import ID="ucImport" runat="server" Visible="false" />
