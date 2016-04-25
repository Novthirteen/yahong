<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_FacilityTrans_Main" %>
<%@ Register Src="~/Facility/FacilityTrans/List.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityTrans/Search.ascx" TagName="Search" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityTrans/EditMain.ascx" TagName="Edit" TagPrefix="uc2" %>

<uc2:Search ID="ucSearch" runat="server" Visible="True" />
<uc2:List ID="ucList" runat="server" Visible="false" />
<uc2:Edit ID="ucEdit" runat="server" Visible="false" />