/// <reference path="BoardingGate.js" />
///  FOR  BoardingGate
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

var BoardingGate =
    {
        Init: function () { BoardingGate.ListInit(); }
    }
BoardingGate.ListInit = function () {
   
    $(".menu-text").on("click", function () {
        BoardingGate.CreateLabel();
    });

    $(".menu-pic").on("click", function () {
        BoardingGate.CreateImage();
    });
    $(".menu-time").on("click", function () {
        BoardingGate.CreateTime();
    });
    $(".menu-save").on("click", function () {
        BoardingGate.Submit();
    });
    $(".menu-clear").on("click", function () {
        BoardingGate.Reset();
    });
}
BoardingGate.CreateLabel = function () {
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
BoardingGate.CreateImage = function () {
    //创建image
    console.log("begin creating image");
    tag = "2";
    var url = "/Template/GetGUID";
    var data = {};
    _defaultData = null;
    Global.Post(url, data, Global.PostCallback);
    var divStr = Global.CreateDefDivStr(Global.PostData.Data, "labelBox", tag, "待编辑图片域");
    console.log("after creating def div");
    $(".header").append(divStr);
    console.log("after append div str");
    $("#" + Global.PostData.Data).css("min-width", "1.2rem");
    $("#" + Global.PostData.Data).css("width", "3.2rem");
    $("#" + Global.PostData.Data).draggable({ containment: $(".header") });

    headerIDs.push(Global.PostData.Data);
    console.log("after push");
    Global.CreateMenu(Global.PostData.Data, tag);
    console.log("end");
}
BoardingGate.CreateTime = function () {
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
BoardingGate.Submit = function () {
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
    var url = "/Template/DoAddBoardingGateTemplate";
    var data = { header: headerArr.join("||"), body: bodyIDs.join("||") };
    Global.Post(url, data, Global.PostCallback);
    if (Global.PostData.Status == 1) {
        layer.confirm('组件已保存完毕,查看预览效果？', { icon: 1, title: '提示', time: 5000 }
            , function (index) {
                window.open("/Actual/BoardingGate");
            });
    } else {
        layer.msg(_defaultData.Message, { time: 1000 });
    }
}
BoardingGate.Reset = function () {
    layer.confirm('登机口竖版（含气象）模块恢复出厂设置？', { icon: 3, title: '确认' }
    , function (index) {
        var url = "/Template/Reset";
        var data = { ak: "BoardingGate" };
        Global.Post(url, data, Global.PostData);
        layer.msg(Global.PostData.Message, { time: 2000 }, function () {
            window.parent.location.href = Global.PostData.Data;
        });
    });
}