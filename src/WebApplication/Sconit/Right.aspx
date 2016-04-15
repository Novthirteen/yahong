<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Right.aspx.cs" Inherits="Right" Title="Right" %>

<head runat="server" id="righthead">
    <meta http-equiv="X-UA-Compatible" content="IE=100; IE=EDGE">
    <%--<meta charset="utf-8" />--%>
    <title>MainPage</title>
    <link href="Images/Jquery-ui/Smoothness/jquery-ui-1.10.3.custom.min.css" type="text/css" rel="stylesheet" />
    <link href="App_Themes/Base.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="Js/jquery.js"></script>
    <script type="text/javascript" src="Js/jquery-ui-1.10.3.custom.min.js"></script>
    <style>
        #tabs li .ui-icon-close {
            float: left;
            margin: 0.4em 0.2em 0 0;
            cursor: pointer;
        }

        .ui-widget-content {
            border: none;
        }

        .ui-widget {
        }

        .ui-tabs {
            padding: 0;
        }

            .ui-tabs .ui-tabs-nav li a {
                float: left;
                padding: .1em .4em;
                text-decoration: none;
            }

            .ui-tabs .ui-tabs-panel {
                padding: 0;
            }

        .ui-widget-header {
            border: 0;
            background: #fff;
            color: #222;
            font-weight: bold;
            border-bottom: solid 1px #AAA;
        }

        .ui-corner-all, .ui-corner-bottom, .ui-corner, .ui-corner-br {
            border-bottom-right-radius: 0px;
            border-bottom-left-radius: 0px;
            border-top-right-radius: 0px;
            border-top-left-radius: 0px;
        }

        .ui-tabs .ui-tabs-nav {
            margin: 0;
            padding: 0;
        }

        #main {
            padding: 0;
        }

        .liatmain {
            width: 100%;
            background-color: #ff0000;
            margin: 0 auto;
        }

        .left {
            background: #eeffee none repeat scroll 0% 0%;
            float: left;
            width: 18px;
            height: 50px;
        }

        .right {
            background: #eeeeff none repeat scroll 0% 0%;
            margin-left: 18px;
        }
    </style>

</head>
<script>
    var tabs = null;
    $(function () {
        tabs = $("#tabs").tabs();
        // close icon: removing the tab on click
        tabs.delegate("span.ui-icon-close", "click", function () {
            //debugger
            var panelId = $(this).closest("li").remove().attr("aria-controls");
            $("#" + panelId).remove();
            tabs.tabs("refresh");
            //var id = $("#tabs li.ui-state-active").attr('title');
            //parent.parent.Nav.ExpandNode(id);
        });

        tabs.bind("keyup", function (event) {
            if (event.altKey && event.keyCode === $.ui.keyCode.BACKSPACE) {
                var panelId = tabs.find(".ui-tabs-active").remove().attr("aria-controls");
                var tabs = $("#tabs").tabs();
                $("#" + panelId).remove();
                tabs.tabs("refresh");
            }
        });

        $("#tabs").height(getTotalHeight() - 29);
        addTab("<%=url %>", "<%=name %>");

        $("#tabs").tabs({
            activate: function (event, ui) {
                //debugger
                //var id = $("#tabs li.ui-state-active").attr('title');
                //parent.parent.Nav.ExpandNode(id);
                //handleOnLoad();
                //window.parent.parent.document.title = id;

                //var value = $(ui.newPanel[0]).attr("url");
                //var exp = new Date();
                //exp.setTime(exp.getTime() + 365 * 24 * 60 * 60 * 1000);
                //document.cookie = "Sconit_MainPageUrl" + "@ViewBag.CurrentUserCode" + "=" + escape(value) + ";expires=" + exp.toGMTString();
            }
        });

    });

    var tabTemplate = "<li title='#{title}'><a href='#{href}'>#{label}</a> <span class='ui-icon ui-icon-close' role='presentation'>Remove Tab</span></li>";

    // actual addTab function: adds new tab using the input from the form above
    function addTab(url, title) {
        //debugger
        if (url == undefined) {
            return;
        }
        var splittitle = title.split("-");
        var name = splittitle[splittitle.length - 1];//splittitle[splittitle.length - 2] + "-" + 
        var id = url.replace(/\./g, "_").split("=")[1];
        var li = $(tabTemplate.replace(/#\{href\}/g, "#" + id).replace(/#\{label\}/g, name).replace(/#\{title\}/g, title));
        var index = $('#tabs a[href="#' + id + '"]').parent().index();
        if (index == -1) {
            tabs.find(".ui-tabs-nav").append(li);
            tabs.append("<div id='" + id + "' url='" + url + "'><iframe src='" + url + "' width='100%' height='100%' frameborder='0' marginwidth='0' hspace='0' noresize style='border: none;' id='ifr" + id + "' /></div>");
            //<iframe src='" + url + "' width='100%' height='100%' frameborder='0' marginwidth='0' hspace='0' noresize style='border: none;' id='ifr" + id + "' />
            tabs.tabs("refresh");
            index = $('#tabs a[href="#' + id + '"]').parent().index();
            tabs.tabs("option", "active", index);
            $("#" + id).height(getTotalHeight() - 29);
            $("#" + id + " > iframe").height(getTotalHeight() - 29);
        }
        else {
            tabs.tabs("option", "active", index);
        }
    }

    function getTotalHeight() {
        if ($.browser.msie) {
            return document.compatMode == "CSS1Compat" ? document.documentElement.clientHeight : document.body.clientHeight;
        }
        else {
            return self.innerHeight;
        }
    }

    function handleOnLoad() {
        var id =getTabId();
        $("#tabs").height(getTotalHeight() - 29);
        $(id).height(getTotalHeight() - 29);
        $(id + " > iframe").height(getTotalHeight() - 29);
    }
    window.onresize = handleOnLoad;

    window.onload = function () {
        //var id = $("#tabs li.ui-state-active").attr('title');
        //parent.parent.Nav.ExpandNode(id);
        handleOnLoad();
    }

    function refresh() {
        var id =getTabId();
        $(id + " > iframe")[0].src = $(id + " > iframe")[0].src;
    }

    function getTabId() {
        var id = $("#tabs li.ui-state-active a").attr('href');
        var array = id.split("#");
        return "#" + array[1];
    }

</script>
<body style="margin-left: 5px; font-size: 12px">
    <div id="tabs">
        <ul>
        </ul>
    </div>
</body>
