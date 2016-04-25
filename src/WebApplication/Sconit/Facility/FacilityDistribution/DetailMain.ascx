<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DetailMain.ascx.cs" Inherits="Facility_FacilityDistributionDetail_Main" %>
<%@ Register Src="DetailEdit.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="DetailList.ascx" TagName="List" TagPrefix="uc2" %>
<%@ Register Src="DetailNew.ascx" TagName="New" TagPrefix="uc2" %>

<uc2:Edit ID="ucEdit" runat="server" Visible="false" />
<uc2:List ID="ucList" runat="server" Visible="false" />
<uc2:New ID="ucNew" runat="server" Visible="false" />
