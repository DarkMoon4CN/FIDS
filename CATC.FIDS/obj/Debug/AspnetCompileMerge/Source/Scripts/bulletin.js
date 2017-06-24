/// <reference path="Bulletin.js" />
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

var Bulletin =
    {
        Init: function () { Bulletin.ListInit(); }
    }
Bulletin.ListInit = function () {
    $(".menu-text").on("click", function () {
        Bulletin.CreateLabel();
    });

    $(".menu-pic").on("click", function () {
        Bulletin.CreateImage();
    });
    $(".menu-time").on("click", function () {
        Bulletin.CreateTime();
    });
    $(".menu-save").on("click", function () {
        Bulletin.Submit();
    });
    $(".menu-clear").on("click", function () {
        Bulletin.Reset();
    });
    $(".menu-bg").on("click", function () {
        Bulletin.CreateBgColor();
    });
    $(".bulletinCon").dblclick(function () {
        //创建一个大型的描述文本，生成后必须有一个父节点
        Bulletin.CreateContent($(this));//self
    });
}
Bulletin.CreateContent = function (obj)
{
    tag = "10";
    var childerCount = $(obj).children().length;
    var id;
    if (childerCount == 0) {
        var url = "/Template/GetGUID";
        var data = {};
        Global.Post(url, data, Global.PostCallback);
        id = Global.PostData.Data;
        var cdiv = $("<div id='" + id + "'  tag='" + tag + "'></div>");
        obj.append(cdiv);
        bodyIDs.push(id);
    }
    else
    {
        id = obj.children(":first").attr("id");
    }
    Global.ToEdit(id,tag);
}
Bulletin.CreateBgColor = function () {
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
Bulletin.CreateLabel = function () {
    //创建label
    tag = "1";
    var url = "/Template/GetGUID";
    var data = {};
    Global.Post(url, data, Global.PostCallback);
    var divStr = Global.CreateBgDivStr(Global.PostData.Data, "labelBox", tag, "待编辑文本域");
    $(".other").append(divStr);
    $("#" + Global.PostData.Data).draggable({ containment: $("#" + Global.PostData.Data).parent() });
    otherIDs.push(Global.PostData.Data);
    Global.CreateMenu(Global.PostData.Data, tag);
    toastr.info((new Date()).toLocaleTimeString() + " 创建了文本元素！");
}
Bulletin.CreateImage = function () {
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
Bulletin.CreateTime = function () {
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
Bulletin.Submit = function () {
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
  
    var bodyArr = new Array();
    for (var i = 0; i < bodyIDs.length; i++) {
        bodyArr.push(Global.GetStyles(bodyIDs[i]));
    }

    var url = "/Template/DoAddBulletinTemplate";
    var data = { header: headerArr.join("||"), body: bodyArr.join("||"), config: config, other: otherArr.join("||") };
    Global.Post(url, data, Global.PostCallback);
    if (Global.PostData.Status == 1) {
        layer.confirm('组件已保存完毕,查看预览效果？', { icon: 1, title: '提示', time: 5000 }
            , function (index) {
                window.open("/Actual/Bulletin");
            });
    } else {
        layer.msg(Global.PostData.Message, { time: 1000 });
    }
}
Bulletin.Reset = function () {
    layer.confirm('公告信息编辑模块恢复出厂设置？', { icon: 3, title: '确认' }
    , function (index) {
        var url = "/Template/Reset";
        var data = { ak: "Bulletin" };
        Global.Post(url, data, Global.PostData);
        layer.msg(Global.PostData.Message, { time: 2000 }, function () {
            window.parent.location.href = Global.PostData.Data;
        });
    });
}