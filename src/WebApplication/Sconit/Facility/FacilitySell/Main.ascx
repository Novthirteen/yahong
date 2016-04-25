<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="Facility_FacilitySell_Main" %>
<%@ Register Src="~/Facility/FacilitySell/Edit.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilitySell/List.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilitySell/Search.ascx" TagName="Search" TagPrefix="uc2" %>
<%@ Register Src="~/Facility/FacilityTrans/EditMain.ascx" TagName="Trans" TagPrefix="uc2" %>


<uc2:Search ID="ucSearch" runat="server" Visible="True" />
<uc2:Edit ID="ucEdit" runat="server" Visible="false" />
<uc2:List ID="ucList" runat="server" Visible="false" />
<uc2:Trans  ID="ucTrans" runat="server" Visible ="false" />

