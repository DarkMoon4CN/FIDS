/// <reference path="SecurityBulletin.js" />
///  FOR  Bulletin
//中英文切换变量
var changed = 0;
//切换时间
var changeTime = 5;
//创建元素时的id
var dataFlag = 1000;
//用于操作页面头的所有自定义元素ID
var headerIDs = new Array();
//用于操作页面主体的所有自定义元素ID
var bodyIDs = new Array();
//用于页面上其他的自定义元素ID
var otherIDs = new Array();

var SecurityBulletin =
    {
        Init: function () { SecurityBulletin.ListInit(); }
    }
SecurityBulletin.ListInit = function () {
    $(".menu-bg").on("click", function () {
        SecurityBulletin.CreateBgColor();
    });

    $(".menu-save").on("click", function () {
        SecurityBulletin.Submit();
    });
    $(".menu-clear").on("click", function () {
        SecurityBulletin.Reset();
    });
    $(".img-secBulletin").dblclick(function () {
        tag = "2";
        var childerCount = $(this).children().length;
        var id;
        if (childerCount == 0) {
            var url = "/Template/GetGUID";
            var data = {};
            Global.Post(url, data, Global.PostCallback);
            id = Global.PostData.Data;
            var cdiv = $("<div id='" + id + "'  tag='" + tag + "'></div>");
            $(this).append(cdiv);
            bodyIDs.push(id);
        }
        else {
            id = $(this).children(":first").attr("id");
        }
         Global.ToEdit(id, tag);
    });
}
SecurityBulletin.CreateBgColor = function () {
    //背景加在body上
    tag = "9";
    var bid = $("body").attr("id");
    if (bid == null || typeof (bid) == "undefined") {
        var url = "/Template/GetGUID";
        var data = {};
        Global.Post(url, data, Global.PostCallback);
        $("body").attr("id", Global.PostData.Data);
        $("body").attr("tag", tag);
        bid = Global.PostData.Data;
    }
    Global.ToEdit(bid, tag);
}
SecurityBulletin.Submit = function () {
    var headerArr = new Array();
    for (var i = 0; i < headerIDs.length; i++) {
        //防止淡进淡出效果
        $("#" + headerIDs[i]).css("opacity", "1.0");
        headerArr.push(Global.GetStyles(headerIDs[i]));
    }

    var otherArr = new Array();
    for (var i = 0; i < otherIDs.length; i++) {
        $("#" + otherIDs[i]).css("opacity", "1.0");
        otherArr.push(Global.GetStyles(otherIDs[i]));
    }

    //页面背景config
    var bid = $("body").attr("id");
    var config = "";
    if (bid != null) {
        config = Global.GetStyles(bid);
    }
    console.log(bodyIDs);
    var bodyArr = new Array();
    for (var i = 0; i < bodyIDs.length; i++) {
        bodyArr.push(Global.GetStyles(bodyIDs[i]));
    }

    var url = "/Template/DoAddCurrentTemplate";
    var data = { actionKey: "SecurityBulletin", header: headerArr.join("||"), body: bodyArr.join("||"), config: config, other: otherArr.join("||") };
    Global.Post(url, data, Global.PostCallback);
    if (Global.PostData.Status == 1) {
        layer.confirm('组件已保存完毕,查看预览效果？', { icon: 1, title: '提示', time: 5000 }
            , function (index) {
                window.open("/Actual/SecurityBulletin");
            });
    } else {
        layer.msg(Global.PostData.Message, { time: 1000 });
    }
}
SecurityBulletin.Reset = function () {
    layer.confirm('公告信息编辑模块恢复出厂设置？', { icon: 3, title: '确认' }
    , function (index) {
        var url = "/Template/Reset";
        var data = { ak: "SecurityBulletin" };
        Global.Post(url, data, Global.PostData);
        layer.msg(Global.PostData.Message, { time: 2000 }, function () {
            window.parent.location.href = Global.PostData.Data;
        });
    });
}