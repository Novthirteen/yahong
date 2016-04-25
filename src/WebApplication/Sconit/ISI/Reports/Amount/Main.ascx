<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Main.ascx.cs" Inherits="ISI_Reports_Amount_Main" %>
<%@ Register Src="~/Controls/TextBox.ascx" TagName="textbox" TagPrefix="uc3" %>
<%@ Register Assembly="com.Sconit.Control" Namespace="com.Sconit.Control" TagPrefix="cc1" %>
<script language="javascript" type="text/javascript" src="Js/Highcharts/js/highcharts.js"></script>
<script language="javascript" type="text/javascript" src="Js/Highcharts/js/modules/exporting.js"></script>
<script language="javascript" type="text/javascript" src="Js/Highcharts/js/themes/grid.js"></script>
<script language="javascript" type="text/javascript">
    var chart;
    function ShowChart() {
        $("#fs").slideDown();
        Sys.Net.WebServiceProxy.invoke('Webservice/CheckupMgrWS.asmx', 'GetCheckups', false,
            { "startDate": $('#<%=tbStartDate.ClientID%>').val(), "endDate": $('#<%=tbEndDate.ClientID%>').val(), "type": $('#<%=tbFlow.ClientID%>').val() },
            function OnSucceeded(result, eventArgs) {
                chart = new Highcharts.Chart({
                    chart: {
                        renderTo: 'container'
                    },
                    title: {
                        text: ''
                    },
                    xAxis: {
                        categories: result.categories,
                        labels: {
                            rotation: -45,
                            align: 'right',
                            style: {
                                fontSize: '13px',
                                fontFamily: 'Verdana, sans-serif'
                            }
                        }
                    },
                    yAxis: {
                        title: {
                            text: '人次'
                        },
                        labels: {
                            formatter: function () {
                                return this.value;
                            }
                        }
                    },
                    tooltip: {
                        formatter: function () {
                            var s;
                            if (this.point.name) { // the pie chart
                                s = '' +
                                    this.point.name + ': ' + this.y + '  元';
                            } else {
                                s = '' +
                                    this.series.name + this.x + ': ' + this.y + '人次';
                            }
                            return s;
                        }
                    },
                    plotOptions: {
                        column: {
                            dataLabels: {
                                enabled: true,
                                color: '#FFFFFF',
                                align: 'center',
                                x: -3,
                                y: 10,
                                formatter: function () {
                                    return this.y;
                                },
                                style: {
                                    fontSize: '13px',
                                    fontFamily: 'Verdana, sans-serif'
                                }
                            }
                        }
                    },
                    labels: {
                        items: [{
                            html: '金额对比',
                            style: {
                                left: '60px',
                                top: '8px',
                                color: 'black'
                            }
                        }]
                    },
                    series: [{
                        type: 'column',
                        name: '增款',
                        data: result.data1,
                        color: '#4572A7'
                    }, {
                        type: 'column',
                        name: '扣款',
                        data: result.data2,
                        color: '#89A54E'
                    }, {
                        type: 'spline',
                        name: '平均数',
                        data: result.data3,
                        marker: {
                            lineWidth: 2,
                            lineColor: Highcharts.getOptions().colors[3],
                            fillColor: 'white'
                        }
                    }, {
                        type: 'pie',
                        name: 'Total consumption',
                        data: [{
                            name: '增款',
                            y: result.p1,
                            color: '#4572A7' // Jane's color
                        }, {
                            name: '扣款',
                            y: result.p2,
                            color: '#89A54E' // John's color
                        }],
                        center: [100, 80],
                        size: 100,
                        showInLegend: false,
                        dataLabels: {
                            enabled: true,
                            color: '#000000',
                            connectorColor: '#000000',
                            formatter: function () {
                                return '<b>' + this.point.name + '</b>: ' + Highcharts.numberFormat(this.percentage, 0) + ' %';
                            }
                        }
                    }]
                });
            },
            function OnFailed(error) {

                alert(error.get_message());
            }
          );
        }
</script>
<fieldset>
    <table class="mtable">

        <tr>
            <td class="td01">
                <asp:Literal ID="lblPartyFrom" runat="server" Text="${MasterData.Order.OrderHead.PartyFrom.Region}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbPartyFrom" runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" ServicePath="PartyMgr.service" ServiceMethod="GetOrderFromParty" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblPartyTo" runat="server" Text="${MasterData.Order.OrderHead.PartyTo}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbPartyTo" runat="server" Visible="true" Width="250" DescField="Name"
                    ValueField="Code" MustMatch="true" ServicePath="PartyMgr.service" ServiceMethod="GetOrderToParty" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblFlow" runat="server" Text="${MasterData.Order.OrderHead.Flow}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbFlow" runat="server" Visible="true" DescField="Description" ValueField="Code"
                    ServicePath="FlowMgr.service" MustMatch="true" Width="250" ServiceMethod="GetFlowList" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblCode" runat="server" Text="${MasterData.Item.Code}:" />
            </td>
            <td class="td02">
                <uc3:textbox ID="tbItem" runat="server" Visible="true" Width="250" DescField="Description"
                    ValueField="Code" ServicePath="ItemMgr.service" ServiceMethod="GetCacheAllItem" />
            </td>
        </tr>
        <tr>
            <td class="td01">
                <asp:Literal ID="lblStartDate" runat="server" Text="${Common.Business.StartDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbStartDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
            <td class="td01">
                <asp:Literal ID="lblEndDate" runat="server" Text="${Common.Business.EndDate}:" />
            </td>
            <td class="td02">
                <asp:TextBox ID="tbEndDate" runat="server" onClick="WdatePicker({dateFmt:'yyyy-MM-dd'})" />
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02">
                <asp:RadioButtonList runat="server" ID="rblType" RepeatDirection="Horizontal">
                    <asp:ListItem runat="server" Text="${ISI.Reports.Amount.Mouth}" Value="mouth" />
                    <asp:ListItem runat="server" Text="${ISI.Reports.Amount.Week}" Value="week"
                        Selected="True" />
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td class="td01"></td>
            <td class="td02"></td>
            <td class="td01"></td>
            <td class="t02">
                <div class="buttons">
                    <button type="button" id="btnSearch" class="button2" name="btnSearch" onclick="ShowChart()">
                        ${Common.Button.Search}
                    </button>
                </div>
            </td>
        </tr>
    </table>
</fieldset>
<fieldset id="fs" style="display: none">
    <div id="container" style="min-width: 400px; height: 400px; margin: 0 auto"></div>
</fieldset>
