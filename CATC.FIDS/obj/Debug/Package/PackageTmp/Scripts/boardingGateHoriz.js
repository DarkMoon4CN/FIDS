/// <reference path="BoardingGateHoriz.js" />
///  FOR  BoardingGateHoriz
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
var BoardingGateHoriz =
    {
        Init: function () { BoardingGateHoriz.ListInit(); }
    }
BoardingGateHoriz.ListInit = function () {
    $(".menu-text").on("click", function () {
        BoardingGateHoriz.CreateLabel();
    });

    $(".menu-pic").on("click", function () {
        BoardingGateHoriz.CreateImage();
    });
    $(".menu-time").on("click", function () {
        BoardingGateHoriz.CreateTime();
    });
    $(".menu-save").on("click", function () {
        BoardingGateHoriz.Submit();
    });
    $(".menu-clear").on("click", function () {
        BoardingGateHoriz.Reset();
    });
    $(".menu-bg").on("click", function () {
        BoardingGateHoriz.CreateBgColor();
    });
}
BoardingGateHoriz.CreateBgColor = function () {
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
BoardingGateHoriz.CreateLabel = function () {
    //创建label
    tag = "1";
    var url = "/Template/GetGUID";
    var data = {};
    Global.Post(url, data, Global.PostCallback);
    var divStr = Global.CreateBgDivStr(Global.PostData.Data, "labelBox", tag, "待编辑文本域");
    $(".header").append(divStr);
    $("#" + Global.PostData.Data).draggable({ containment: $(".header") });
    headerIDs.push(Global.PostData.Data);
    Global.CreateMenu(Global.PostData.Data, tag);
    toastr.info((new Date()).toLocaleTimeString() + " 创建了文本元素！");
}
BoardingGateHoriz.CreateImage = function () {
    //创建image
    tag = "2";
    var url = "/Template/GetGUID";
    var data = {};
    _defaultData = null;
    Global.Post(url, data, Global.PostCallback);
    var divStr = Global.CreateDefDivStr(Global.PostData.Data, "labelBox", tag, "待编辑图片域");
    $(".header").append(divStr);
    $("#" + Global.PostData.Data).css("min-width", "1.2rem");
    $("#" + Global.PostData.Data).css("width", "3.2rem");
    $("#" + Global.PostData.Data).draggable({ containment: $(".header") });
    headerIDs.push(Global.PostData.Data);
    Global.CreateMenu(Global.PostData.Data, tag);
}
BoardingGateHoriz.CreateTime = function () {
    //创建时钟
    tag = "3";
    var url = "/Template/GetGUID";
    var data = {};
    _defaultData = null;
    Global.Post(url, data, Global.PostCallback);
    var divStr = Global.CreateBcTimeDivStr(Global.PostData.Data, "labelBox", tag);
    $(".header").append(divStr);
    $("#" + Global.PostData.Data).draggable({ containment: $(".header") });
    headerIDs.push(Global.PostData.Data);
    Global.CreateMenu(Global.PostData.Data, tag);
}
BoardingGateHoriz.Submit = function () {
    var headerArr = new Array();
    for (var i = 0; i < headerIDs.length; i++) {
        //防止淡进淡出效果
        $("#" + headerIDs[i]).css("opacity", "1.0");
        headerArr.push(Global.GetStyles(headerIDs[i]));
    }
    if (headerArr.length == 0) {
        layer.msg('页面暂无改动,无需保存！', { time: 1000 });
        return;
    }
    //页面背景config
    var bid = $("body").attr("id");
    var config = "";
    if (bid != null) {
        config = Global.GetStyles(bid);
    }
    var url = "/Template/DoAddBoardingGateHorizTemplate";
    var data = { header: headerArr.join("||"), body: bodyIDs.join("||"), config: config };
    Global.Post(url, data, Global.PostCallback);
    if (Global.PostData.Status == 1) {
        layer.confirm('组件已保存完毕,查看预览效果？', { icon: 1, title: '提示', time: 5000 }
            , function (index) {
                window.open("/Actual/BoardingGateHoriz");
            });
    } else {
        layer.msg(Global.PostData.Message, { time: 1000 });
    }
}
BoardingGateHoriz.Reset = function () {
    layer.confirm('登机口(横版、含气象)模块恢复出厂设置？', { icon: 3, title: '确认' }
    , function (index) {
        var url = "/Template/Reset";
        var data = { ak: "BoardingGateHoriz" };
        Global.Post(url, data, Global.PostData);
        layer.msg(Global.PostData.Message, { time: 2000 }, function () {
            window.parent.location.href = Global.PostData.Data;
        });
    });
}