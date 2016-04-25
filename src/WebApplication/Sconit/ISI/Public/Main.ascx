<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_Public_Main" %>

<%@ Register Src="~/ISI/TSK/EditMain.ascx" TagName="Edit" TagPrefix="uc2" %>
<%@ Register Src="Search.ascx" TagName="Search" TagPrefix="uc2" %>
<%@ Register Src="List.ascx" TagName="List" TagPrefix="uc2" %>


<script language="javascript">
    function showMacAddress() {
        var obj = new ActiveXObject("WbemScripting.SWbemLocator");
        var s = obj.ConnectServer(".");
        var properties = s.ExecQuery("SELECT * FROM Win32_NetworkAdapterConfiguration");
        var e = new Enumerator(properties);
        var output;
        output = '<table border="0" cellPadding="5px" cellSpacing="1px" bgColor="#CCCCCC">';
        output = output + '<tr bgColor="#EAEAEA"><td>Caption</td><td>MACAddress</td></tr>';
        while (!e.atEnd()) {
            e.moveNext(); var p = e.item();
            if (!p) continue;
            output = output + '<tr bgColor="#FFFFFF">';
            output = output + '<td>' + p.Caption; +'</td>';
            output = output + '<td>' + p.MACAddress + '</td>';
            output = output + '</tr>';
        }
        output = output + '</table>';
        document.write(output);
    }
    function GetLocalNetInfo1() {
        var locator = new ActiveXObject("WbemScripting.SWbemLocator");
        var service = locator.ConnectServer(".");
        var properties = service.ExecQuery("SELECT * FROM Win32_NetworkAdapterConfiguration");
        var e = new Enumerator(properties);
        for (; !e.atEnd() ; e.moveNext()) {
            var p = e.item();
            if (p.IPAddress == null) { continue; }
            document.write(p.IPAddress(0));
            document.write(p.MACAddress);
            document.write("<br/>");
        }
    }

    function GetLocalNetInfo() {
        var locator = new ActiveXObject("WbemScripting.SWbemLocator");
        var service = locator.ConnectServer(".");
        var properties = service.ExecQuery("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE' ");
        var e = new Enumerator(properties);

        var ip = new Array();
        var mac = new Array();
        var pcname = new Array();
        var x = 0;
        var y = 0;
        var z = 0;
        for (; !e.atEnd() ; e.moveNext()) {
            var p = e.item();
            if (p.IPAddress == null) { continue; }
            ip[x++] = p.IPAddress(0);
            mac[y++] = p.MACAddress;
            pcname[z++] = p.DNSHostName;
        }


        if (ip != '' && mac != '' && pcname != '') {
            Sys.Net.WebServiceProxy.invoke('Webservice/TaskMgrWS.asmx', 'GetLimit', false,
                        { "clientIP": ip, "clientMAC": mac, "clientPCName": pcname },
                        function OnSucceeded(result, eventArgs) {
                            if (result) {
                                //self.location = 'MainPage/NoPermission.aspx';
                                $('#MsgDiv').slideToggle();
                            } else {
                                $('#contentDiv').slideToggle();
                            }
                        },
                        function OnFailed(error) {
                            alert(error.get_message());
                        }
                       );
        }


    }

    $(document).ready(function () {
        GetLocalNetInfo();
    });




</script>


<div id="MsgDiv" style="display: none">
    <asp:Literal ID="ltlMsg" runat="server" Text="${ISI.Public.Message}" Visible="true" />
</div>
<div id="contentDiv" style="display: none">
    <uc2:Search ID="ucSearch" runat="server" Visible="true" />
    <uc2:Edit ID="ucEdit" runat="server" Visible="false" />
    <uc2:List ID="ucList" runat="server" Visible="false" />
</div>
