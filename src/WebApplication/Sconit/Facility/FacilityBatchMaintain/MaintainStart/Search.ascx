<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Search.ascx.cs" Inherits="Facility_FacilityMaintain_StartSearch" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>

<fieldset>
    <table class="mtable">
        <tr>
            
            <td class="td01">
                <asp:Literal ID="lblMaintainGroup" runat="server" Text="${Facility.FacilityMaster.MaintainGroup}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbMaintainGroup" runat="server" Visible="true" DescField="MaintainGroup" ValueField="MaintainGroup"
                    ServicePath="FacilityMasterMgr.service" AutoPostBack="true" MustMatch="true"
                    Width="250"  ServiceMethod="GetMaintainGroupList" OnTextChanged="tbMaintainGroup_TextChanged" />
            </td>
        </tr>
    </table>
</fieldset>