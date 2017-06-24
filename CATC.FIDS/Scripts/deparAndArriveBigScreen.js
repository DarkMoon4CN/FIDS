/// <reference path="deparAndArriveBigScreen.js" />
///  FOR deparAndArriveBigScreen

//start arrivalflight ajax
var _defaultData;
var _isSuccess = true;

function defaultAjax(url, data, callback) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        async: false,
        error: function () {
            _isSuccess = false;
            layer.alert('服务器访问失败，请查看网络是否畅通！');
        },
        success: defaultCallback
    });
}
function defaultCallback(data) {
    _defaultData = data;
}
//end arrivalflight ajax

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

var DeparAndArriveBigScreen =
    {
        Init: function () { DeparAndArriveBigScreen.ListInit(); }
    }
DeparAndArriveBigScreen.ListInit = function () {
    //调用父窗体的方法初始化table
    //if (self != top) {
    //    var parr = window.parent.GetTableColumn();
    //    Global.SetTableColumn(parr);
    //}
    $(".menu-text").on("click", function () {
        DeparAndArriveBigScreen.CreateLabel();
    });

    $(".menu-pic").on("click", function () {
        DeparAndArriveBigScreen.CreateImage();
    });
    $(".menu-time").on("click", function () {

        DeparAndArriveBigScreen.CreateTime();
    });
    $(".menu-save").on("click", function () {
        DeparAndArriveBigScreen.Submit();
    });
    $(".menu-clear").on("click", function () {
        DeparAndArriveBigScreen.Reset();
    });
    $(".menu-bg").on("click", function () {
        DeparAndArriveBigScreen.CreateBgColor();
    });
    //轮询获取消息
    setInterval(function () {
        Global.GetEventMessage("DeparAndArriveBigScreen");
    }, changeTime * 1000);
}

DeparAndArriveBigScreen.CreateBgColor = function () {
    //背景加在body上
    tag = "9";
    var bid = $("body").attr("id");
    if (bid == null || typeof (bid) == "undefined") {
        var url = "/Template/GetGUID";
        var data = {};
        _defaultData = null;
        defaultAjax(url, data, defaultCallback);
        $("body").attr("id", _defaultData.Data);
        $("body").attr("tag", tag);
        bid = _defaultData.Data;
    }
    Global.ToEdit(bid, tag);
}
DeparAndArriveBigScreen.CreateLabel = function () {
    //创建label
    tag = "1";
    var url = "/Template/GetGUID";
    var data = {};
    _defaultData = null;
    defaultAjax(url, data, defaultCallback);
    var divStr = Global.CreateBgDivStr(_defaultData.Data, "labelBox", tag, "待编辑文本域");
    $(".header").append(divStr);
    $("#" + _defaultData.Data).draggable({ containment: $(".header") });
    headerIDs.push(_defaultData.Data);
    Global.CreateMenu(_defaultData.Data, tag);
    toastr.info((new Date()).toLocaleTimeString() + " 创建了文本元素！");
}
DeparAndArriveBigScreen.CreateImage = function () {
    //创建image
    tag = "2";
    var url = "/Template/GetGUID";
    var data = {};
    _defaultData = null;
    defaultAjax(url, data, defaultCallback);
    var divStr = Global.CreateDefDivStr(_defaultData.Data, "labelBox", tag, "待编辑图片域");
    $(".header").append(divStr);
    $("#" + _defaultData.Data).draggable({ containment: $(".header") });
    headerIDs.push(_defaultData.Data);
    Global.CreateMenu(_defaultData.Data, tag);
    //轮询获取消息
    setInterval(function () {
        Global.GetEventMessage("BSArrivalAndArrival");
    }, changeTime * 1000);
}
DeparAndArriveBigScreen.CreateTime = function () {
    //创建时钟
    tag = "3";
    var url = "/Template/GetGUID";
    var data = {};
    _defaultData = null;
    defaultAjax(url, data, defaultCallback);
    var divStr = Global.CreateTimeDivStr(_defaultData.Data, "labelBox", tag);
    $(".header").append(divStr);
    $("#" + _defaultData.Data).draggable({ containment: $(".header") });
    headerIDs.push(_defaultData.Data);
    Global.CreateMenu(_defaultData.Data, tag);
}
DeparAndArriveBigScreen.Submit = function () {
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
    var url = "/Template/DoAddCurrentTemplate";//通用模板
    var data = { header: headerArr.join("||"), body: bodyIDs.join("||"), config: config };
    _defaultData = null;
    defaultAjax(url, data, defaultCallback);
    if (_defaultData.Status == 1) {
        layer.confirm('组件已保存完毕,查看预览效果？', { icon: 1, title: '提示', time: 5000 }
            , function (index) {
                window.open("/Actual/BSArrivalAndArrival");
            });
    } else {
        layer.msg(_defaultData.Message, { time: 1000 });
    }
}
DeparAndArriveBigScreen.Reset = function () {

    layer.confirm('进港编辑模块恢复出厂设置？', { icon: 3, title: '确认' }
    , function (index) {
        var url = "/Template/Reset";
        var data = { ak: "BSArrivalAndArrival" };
        _defaultData = null;
        defaultAjax(url, data, defaultCallback);
        layer.msg(_defaultData.Message, { time: 2000 }, function () {
            window.parent.location.href = _defaultData.Data;
        });
    });
}
