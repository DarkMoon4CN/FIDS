/// <reference path="global.js" />
//公共方法
var Global = {
    Init: function () { }
};
Global.PostData;
Global.PostisSuccess = true;
Global.Post = function (url, data, callback) {
    try {
        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            async: false,
            error: function () {
                Global.PostisSuccess = false;
                toastr.info("与服务器中断了链接...");
            },
            success: Global.PostCallback
        });
    } catch (e)
    {
        Global.PostData = null;
        Global.PostisSuccess = false;
    }
}
Global.PostCallback = function (data) {
    Global.PostData = null;
    Global.PostData = data;
}
//换算RGB变量
var reg = /^#([0-9a-fA-f]{3}|[0-9a-fA-f]{6})$/;
//滚动的字符串每秒距离
Global.MoveSize = 80;
var intervalList = new Array(); //用于操作页面淡进单出效果的集合
var timeAxisList = new Array(); //用于记录页面上时间轴集合
Global.TransSferAlternately = function (guid, content, otherContent) {
    var arr = new Array();
    arr.push(content);
    if (otherContent != null && otherContent.length > 0) {
        if (otherContent.indexOf('_') != -1) {
            var split = otherContent.split('_');
            for (var i = 0; i < split.length ; i++) {
                if (split[i].length > 0) {
                    arr.push(split[i]);
                }
            }
        }
        else {
            arr.push(otherContent);
        }
    }
    Global.Alternately(guid, arr, changeTime * 1000);
};
//定时淡进淡出
//guid 唯一的ID
//arr Array<string>类型
//time 切换的时间
Global.Alternately = function (guid, arr, time) {
    //移除已存在的定时器
    Global.ClearInterval(guid);
    var i = 1;
    if (arr.length < 2) {
        return;
    }
    var timer = setInterval(function () {
        if (i > arr.length - 1) {
            i = 0;
        }
        $("#" + guid).fadeOut(1000, function () {
            $("#" + guid).text(arr[i]);
            i++;
        });
        $("#" + guid).fadeIn(1000);
    }, time);
    //管理定时器
    var map = { key: guid, value: timer };
    intervalList.push(map);
};
Global.TransSferAlternatelyLorR = function (guid, content, otherContent) {
    var arr = new Array();
    arr.push(content);
    if (otherContent != null && otherContent.length > 0) {
        if (otherContent.indexOf('_') != -1) {
            var split = otherContent.split('_');
            for (var i = 0; i < split.length ; i++) {
                if (split[i].length > 0) {
                    arr.push(split[i]);
                }
            }
        }
        else {
            arr.push(otherContent);
        }
    }
    Global.AlternatelyLorR(guid, arr, changeTime * 1000);
};

//定时左至右
//guid 唯一的ID
//arr Array<string>类型
//time 切换的时间
Global.AlternatelyLorR = function (guid, arr, time)
{
    Global.ClearInterval(guid);
    var i = 1;
    if (arr.length < 2) {
        return;
    }
    var timer = setInterval(function () {
        if (i > arr.length - 1) {
            i = 0;
        }
        Global.fadeOut(guid, changeTime,arr[i]);
        i++;
    }, time);
    //管理定时器
    var map = { key: guid, value: timer };
    intervalList.push(map);
}
//右侧进入
Global.fadeIn = function (id, time) {
    time = time || 1;
    var elem = $('#' + id);
    elem.removeClass('animated fadeOutLeft').css('animation-duration', 0.5 + 's').addClass('animated fadeInRight');
    //监听动画结束事件
    var animatEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationEnd animationend';
    elem.on(animatEnd, function () {
    });
};
//左侧退出
Global.fadeOut = function (id, time, value) {
    time = time || 1;
    var elem = $('#' + id);
    elem.removeClass('animated fadeInRight').css('animation-duration', 0.5 + 's').addClass('animated fadeOutLeft');
    //监听动画结束事件
    var animatEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationEnd animationend';
    elem.on(animatEnd, function () {
        $("#" + id).text(value);
        Global.fadeIn(id, time);
    });
};
Global.fadeInV2 = function (id, time) {
    time = time || 1;
    var elem = $('#' + id);
    elem.removeClass('animated fadeOutLeft').css('animation-duration', 1 + 's').addClass('animated fadeInRight');
    //监听动画结束事件
    var animatEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationEnd animationend';
    elem.on(animatEnd, function () {
    });
};
Global.fadeOutV2 = function (id, time) {
    time = time || 1;
    var elem = $('#' + id);
    elem.removeClass('animated fadeInRight').css('animation-duration', 1 + 's').addClass('animated fadeOutLeft');
    //监听动画结束事件
    var animatEnd = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationEnd animationend';
    elem.on(animatEnd, function () {
        Global.fadeInV2(id, time);
    });
};
Global.fadeInV3 = function (id, time) {
    $("#" + id).fadeIn(1000);
};
Global.fadeOutV3 = function (id, time) {
    $("#" + id).fadeOut(1000, function () {
        Global.fadeInV3(id, 0);
    });
};

//给元素绑定事件
//id 元素ID
//pclass 父类的class
//isDraggable 是否可以移动
//isResizable 是否可以放大缩小
//isMenu      是否允许拥有菜单选项
Global.BindElementEvent = function (id, pclass, tag, isDraggable, isResizable, isMenu) {
    if (isDraggable == 1) {
        $("#" + id).draggable({ containment: $("." + pclass) });
    }
    if (isMenu == 1) {
        Global.CreateMenu(id, tag);
    }
    if (isResizable == 1) {
        $("#" + id).resizable({ containment: $("." + pclass) });
    }
    headerIDs.push(id);
};

//给元素绑定事件,重构
//id 元素ID
//eleName     可被jquery识别的父类节点
//isDraggable 是否可以移动
//isResizable 是否可以放大缩小
//isMenu      是否允许拥有菜单选项
//arrayName   被哪个自定义ID集合push
Global.BindElementEventV2 = function (id, eleName, tag, isDraggable, isResizable, isMenu, arrayName)
{
    if (isDraggable == 1) {
        $("#" + id).draggable({ containment: $(eleName) });
    }
    if (isMenu == 1) {
        Global.CreateMenu(id, tag);
    }
    if (isResizable == 1) {
        $("#" + id).resizable({ containment: $(eleName) });
    }
    if (arrayName == "header")
    {
        headerIDs.push(id);
    }
    else if (arrayName == "body")
    {
        bodyIDs.push(id);
    }
    else if (arrayName == "other")
    {
        otherIDs.push(id);
    }
}


//设置时间样式
// id 元素ID
// timeType 时间类型
Global.ServiceTime = function (id, timeType) {
    var timeTypeStr = "{0}年{1}月{2}日 {3}时{4}分{5}秒";
    if (timeType == "2") {
        timeTypeStr = "{0}/{1}/{2} {3}:{4}:{5}";
    }
    else if (timeType == "3") {
        timeTypeStr = "{3}:{4}:{5} {0}/{1}/{2}";
    }
    else if (timeType == "4") {
        timeTypeStr = "{3}:{4} {0}/{1}/{2}";
    }
    else if (timeType == "5") {
        timeTypeStr = "{3}:{4} {0}/{1}/{2}";
    }
    var time = new Date(+new Date() + 8 * 3600 * 1000).toISOString().formatTime(timeTypeStr);
    $("#" + id).children(":first").html(time);
}
Global.Marquee = function (id, num)
{
    window.clearInterval(timer);
    var parBox = $("#"+id);
    var parScrollWidth = parBox[0].scrollWidth,
        parScrollHeight = parBox[0].scrollHeight;
    var parOffsetWidth = parBox.outerWidth(),
        parOffsetHeight = parBox.outerHeight();
    //水平滚动
    if (parScrollWidth > parOffsetWidth) {
        var str = parBox.html();
        str = '<div>' + str + '</div>';
        str += str;
        parBox.html(str);
        parBox.css({
            whiteSpace: 'nowrap',
            overflow: 'hidden'
        });
        parBox.children().css({
            display: 'inline-block',
            paddingRight: num
        });
        var ChildBox = parBox.children().eq(0);
        var childWidth = parseFloat(ChildBox.width()) + num;
        var cur = 0;
        var timer = window.setInterval(moveWidth, 10);
        function moveWidth() {
            parBox.scrollLeft(cur++);
            var curLeft = parBox.scrollLeft();
            if (curLeft >= childWidth) {
                cur = 0;
            }
        }
    }
    //垂直滚动
    if (parScrollHeight > parOffsetHeight) {
        var str1 = parBox.html();
        str1 = '<div>' + str1 + '</div>';
        str1 += str1;
        parBox.html(str1);
        parBox.css({
            overflow: 'hidden'
        });
        parBox.children().css({
            paddingBottom: num
        });
        var ChildBox1 = parBox.children().eq(0);
        var childWidth1 = parseFloat(ChildBox1.height()) + num;
        var cur1 = 0;
        var timer1 = window.setInterval(moveHeight, 10);
        function moveHeight() {
            parBox.scrollTop(cur1++);
            var curLeft = parBox.scrollTop();
            if (curLeft >= childWidth1) {
                cur1 = 0;
            }
        }
    }
}

//获取元素下的所有样式
//id 节点id
Global.GetStyles = function (id) {
    var tag = $("#" + id).attr("tag");
    if (typeof(tag) == "undefined")
    {
        return;
    }
    var attArr = $("#" + id).get(0).attributes;
    var arr = new Array();

    var overflowX = $("body").css("overflow-x");
    var overflowY = $("body").css("overflow-y");
    var ox = 0;
    var oy = 0;
    if (overflowX != "hidden")
    {
        oy = 0.5;//如果有滚动条的情况下
    }
    if (overflowY != "hidden")
    {
        ox = 0.5;//如果有滚动条的情况下
    }
    //坐标x位置换算成百分比
    var bWidth = parseInt($(document.body).width());
    var eLeft = parseInt($("#" + id).css("left"));
    var pLeft = parseFloat(((eLeft / bWidth) * 100)+ox).toFixed(5) + "%";
    $("#" + id).css("left", pLeft);

    //坐标y位置换算成百分比
    var bHeight = parseInt($("#"+id).parent().height());
    var eTop = parseInt($("#" + id).css("top"));
    var pTop = parseFloat(((eTop / bHeight) * 100)+oy).toFixed(5) + "%";
    $("#" + id).css("top", pTop);

    for (var i = 0; i < attArr.length; i++) {
        if (!isNaN(parseInt(i))) {
            var value = attArr[i].value;
            if (value.indexOf("url") > -1 && value.indexOf("\"") >-1)
            {
                value = value.replace(/\"/g, "");
            }
            arr.push("\"" + attArr[i].name + "\"" + ":" + "\"" + value + "\"");
        }
    }
    var tmp = "{" + arr.join(",") + "}";
    //将去掉的左边和下边的滚动条还原
    return tmp;
};
//用于全局根据GUID清理定时器
//guid 元素ID
Global.ClearInterval = function (id)
{
    for (var i = intervalList.length - 1; i > 0; i--) {
        if (intervalList[i].key==id)
        {
            clearInterval(intervalList[i].value);
            intervalList.splice(i, 1);
        }
    }
};
//初始化页面上 body 标签
//id  唯一ID
//tag 元素标记
//style 样式
Global.BodyElementInit = function (id,tag,style)
{
    $("body").attr("id", id);
    $("body").attr("tag", tag);
    $("body").attr("style",style);
}
//创建label
//guid 需要创建的ID
//classname label 的 class 
//tag 创建的元素类型
//msg 显示描述
Global.CreateDefDivStr = function (guid, className, tag, msg)
{
    return $("<div class='" + className + "' id='" + guid + "'  tag='" + tag + "' style='border:3px solid #000;'>" + msg + "</div>");
}
Global.CreateBgDivStr = function (guid, className, tag, msg) {
    return $("<div class='" + className + "' id='" + guid + "' tag='" + tag + "' style='border:3px solid #000;'><div id='msg_"+guid+"'>" + msg + "</div></div>");
}
Global.CreateTimeDivStr = function (guid, className, tag) {
    return $("<div class='" + className + "' id='" + guid + "'  tag='" + tag + "' style='border:3px solid #000;'><div class='timeBox'>mm yyy:MM:dd</div></div>");
}
Global.CreatVideoStr = function ()
{
    return $("<video controls='controls' src='' autoplay='autoplay' loop='loop' width='100%'></video>");
}
//BaggageClaim编辑模块的时间div 特殊处理
Global.CreateBcTimeDivStr = function (guid, className, tag) {
    return $("<div class='" + className + "' id='" + guid + "'  tag='" + tag + "' style='border:3px solid #000;'><div class='timeBox'>mm yyy:MM:dd</div></div>");
}

Global.ColorRgba = function (color, opacity) {
    var sColor = color.toLowerCase();
    if (sColor && reg.test(sColor)) {
        if (sColor.length === 4) {
            var sColorNew = "#";
            for (var i = 1; i < 4; i += 1) {
                sColorNew += sColor.slice(i, i + 1).concat(sColor.slice(i, i + 1));
            }
            sColor = sColorNew;
        }
        //处理六位的颜色值
        var sColorChange = [];
        for (var i = 1; i < 7; i += 2) {
            sColorChange.push(parseInt("0x" + sColor.slice(i, i + 2)));
        }

        return "rgba(" + sColorChange.join(",") + "," + opacity + ")";
    } else {
        return sColor;
    }
}

//用于背景色遮罩
Global.CreatMaskDiv= function (guid,bgcolor)
{
    $("#mask_" + guid).remove();
    var t = $("#" + guid).clone(true);
    t.attr("id", "mask_" + guid);
    t.removeClass("background-color");
    t.css("background-color", bgcolor);
    t.css("z-index","0");
    t.html("");
    $("#" + guid).parent().append(t);
}

Global.FormatTime = function (template)
{
    return template.replace(/\{(\d)\}/g, function () {
        var index = arguments[1], content = ary[index];
        content = content || '00';
        return content;
    });
}

/*RGB颜色转换为16进制*/
String.prototype.colorHex = function () {
    var that = this;
    if (/^(rgb|RGB)/.test(that)) {
        var aColor = that.replace(/(?:\(|\)|rgb|RGB)*/g, "").split(",");
        var strHex = "#";
        for (var i = 0; i < aColor.length; i++) {
            var hex = Number(aColor[i]).toString(16);
            if (hex === "0") {
                hex += hex;
            }
            strHex += hex;
        }
        if (strHex.length !== 7) {
            strHex = that;
        }
        return strHex;
    } else if (reg.test(that)) {
        var aNum = that.replace(/#/, "").split("");
        if (aNum.length === 6) {
            return that;
        } else if (aNum.length === 3) {
            var numHex = "#";
            for (var i = 0; i < aNum.length; i += 1) {
                numHex += (aNum[i] + aNum[i]);
            }
            return numHex;
        }
    } else {
        return that;
    }
};
//-------------------------------------------------
/*16进制颜色转为RGB格式*/
String.prototype.colorRgb = function () {
    var sColor = this.toLowerCase();
    if (sColor && reg.test(sColor)) {
        if (sColor.length === 4) {
            var sColorNew = "#";
            for (var i = 1; i < 4; i += 1) {
                sColorNew += sColor.slice(i, i + 1).concat(sColor.slice(i, i + 1));
            }
            sColor = sColorNew;
        }
        //处理六位的颜色值
        var sColorChange = [];
        for (var i = 1; i < 7; i += 2) {
            sColorChange.push(parseInt("0x" + sColor.slice(i, i + 2)));
        }
        return "RGB(" + sColorChange.join(",") + ")";
    } else {
        return sColor;
    }
};
//-------------------------------------------------
/*16进制颜色转为RGBA格式*/
String.prototype.colorRgba = function (opacity) {
    var sColor = this.toLowerCase();
    if (sColor && reg.test(sColor)) {
        if (sColor.length === 4) {
            var sColorNew = "#";
            for (var i = 1; i < 4; i += 1) {
                sColorNew += sColor.slice(i, i + 1).concat(sColor.slice(i, i + 1));
            }
            sColor = sColorNew;
        }
        //处理六位的颜色值
        var sColorChange = [];
        for (var i = 1; i < 7; i += 2) {
            sColorChange.push(parseInt("0x" + sColor.slice(i, i + 2)));
        }
        return "RGBA(" + sColorChange.join(",") + ","+opacity+")";
    } else {
        return sColor;
    }
};

Global.FillLabel = function (json) {
    //用于由子窗口向父窗口进行填充label数据
    var arr = new Array();
    var timeStr = "";
    if (json.IsSETime) {
        timeStr = ":00:00--24:00";
    }
    arr.push(json.Content + timeStr);
    if (json.OtherContent != null && json.OtherContent.length > 0) {
        if (json.OtherContent.indexOf('_') != -1) {
            var split = json.OtherContent.split('_');
            for (var i = 0; i < split.length ; i++) {
                if (split[i].length > 0) {
                    arr.push(split[i] + timeStr);
                }
            }
        }
        else {
            arr.push(json.OtherContent + timeStr);
        }
    }
    $("#" + json.ID).css("fontSize", json.FontSize);
    $("#" + json.ID).css("color", json.Color);

    var bgcolor = Global.ColorRgba(json.BgColor, (parseInt(json.Opacity) / 100).toFixed(2));
    $("#" + json.ID).css("background-color", bgcolor);
    $("#" + json.ID).css("border", "0px");
    var cDivID = $("#" + json.ID).children(":first").attr("id");//child div id
    $("#" + cDivID).text(arr[0]);
    if (json.IsBold == true) {
        $("#" + json.ID).css("font-weight", "bold");
    }
    toastr.info((new Date()).toLocaleTimeString() + " 对文本进行了编辑！");
    $("#" + json.ID).resizable({ containment: $("#" + json.ID).parent() });
    //global.js
    Global.Alternately(cDivID, arr, changeTime * 1000);
}
Global.FillImage = function (json) {
    var imgStr = $("<img src='" + json.Url + "'/>");
    $("#" + json.ID).html(imgStr);
    $("#" + json.ID).css("line-height", "0");
    $("#" + json.ID).css("border", "0px");
    $("#" + json.ID).resizable({ containment: $("#" + json.ID).parent() });
    toastr.info((new Date()).toLocaleTimeString() + " 对图片进行了编辑！");
}
Global.FillTime = function (json, msg) {
    var cDiv = $("#" + json.ID).children(":first");
   
    cDiv.css("fontSize", json.FontSize);
    cDiv.css("color", json.Color);
    $("#" + json.ID).css("border", "0px");
    $("#" + json.ID).css("z-index", "1");
    Global.ServiceTime(json.ID, json.TimeType);
    if (json.IsBold == true) {
        cDiv.css("font-weight", "bold");
    }
    
    toastr.info((new Date()).toLocaleTimeString() + " 对时钟进行了编辑！");
}
Global.FillMessage = function (json)
{
    var div = $("#" + json.ID);
    var arr = new Array();
    arr.push(json.Content);
    if (json.OtherContent != null && json.OtherContent.length > 0) {
        if (json.OtherContent.indexOf('_') != -1) {
            var split = json.OtherContent.split('_');
            for (var i = 0; i < split.length ; i++) {
                if (split[i].length > 0) {
                    arr.push(split[i]);
                }
            }
        }
        else {
            arr.push(json.OtherContent);
        }
    }
    $("#" + json.ID).css("border", "0px");
    $("#" + json.ID).html(arr[0]);
    Global.Alternately(json.ID, arr, changeTime * 1000);
    Global.Marquee(json.ID, Global.MoveSize);
    toastr.info((new Date()).toLocaleTimeString() + " 对文本进行了编辑！");
}
Global.FillVideo = function (json)
{
    var video = $("#" + json.ID);
    video.html("");
    video.append(Global.CreatVideoStr());
    video.children(":first").attr("src", json.Urls);
}
Global.FillBgColor = function (json)
{
    if (json.BgType == 1) {
        $("#" + json.ID).css("background", 'none !important');
        var bgcolor = Global.ColorRgba(json.BgColor, (parseInt(json.Opacity) / 100).toFixed(2));
        $("#" + json.ID).css("background", bgcolor);
    } else
    {
        $("#" + json.ID).css("background", "url(" + json.BgImageUrl + ") no-repeat");
        $("#" + json.ID).css("background-size", "100% 100%");
    }
}
Global.FillContent = function (json)
{
    $("#" + json.ID).html("");
    $("#" + json.ID).append(json.Content);
}

Global.ToEdit = function (id, tag) {
    var methodName = "";
    var width = '650px';
    var height = '650px';
    if (tag == "1") {
        methodName = "EditLabel";
    } else if (tag == "2") {
        methodName = "EditImage";
    }
    else if (tag == "3") {
        methodName = "EditTime";
    }
    else if (tag == "7")
    {
        methodName = "EditMessage";
    }
    else if (tag == "8")
    {
        methodName = "EditVideo";
    }
    else if (tag == 9)
    {
        methodName = "EditBgcolor"
    }
    else if (tag == 10)
    {
        methodName = "EditContent";
        width = '900px';
        height = '770px';
    }
    var url = "/Template/" + methodName + "?id=" + id + "&tag=" + tag;
    layer.open({
        type: 2,
        title: '设置 元素 属性',
        shadeClose: false,
        shade: [0.7, '#999'],
        maxmin: false,
        area: [width, height],
        content: [url, 'no']
    });
}
Global.CreateMenu = function (id, tag) {
        if (true) {
            //创建元素右键属性
            var menu = new BootstrapMenu('#' + id, {
                actions: [{
                    name: ' 编 辑 [Edit]',
                    onClick: function () {
                        Global.ToEdit(id, tag);
                    }
                }, {
                    name: ' 删 除 [Del]',
                    onClick: function () {
                        Global.DelElement(id, tag);
                    }
                }]
            });
        }
    }
Global.SetTableColumn = function (arr) {
        $(".col-2").hide();
        for (var i = 0; i < arr.length; i++) {
            var t = $("#t_" + arr[i]).clone(true);
            t.css("display", "block");
            $(".mainCon-table").append(t);
        }
        bodyIDs = arr;
    }
Global.DelElement = function (id, tag) {

        var url = "/Template/DoDelElement";
        var data = { dataid: id };
        var index = layer.msg("正在删除元素...");
        Global.Post(url, data, Global.PostCallback);
        if (Global.PostData.Status == 1) {
            layer.close(index);
            layer.msg("删除完成！");
            $("#" + id).remove();
        }
    }

//EventMessage临时变量
var emid = 0;
Global.GetEventMessage = function (actionKey) {
        var arr = new Array();
        var url = "/Message/GetEventMessage";
        var data = { ak: actionKey };
        Global.Post(url, data, Global.PostCallback);
    
        if (Global.PostData.Data == null)
        {
            $("#msg_00001").remove();
            emid = 0;
        }
        else if (Global.PostData.Data != null && Global.PostData.Data.emid != emid) {
            emid = Global.PostData.Data.emid;
            $("#00001").html();
            var cid = $("<div id='msg_00001' style='display: block;'>" + Global.PostData.Data.chineseContent + "</div>");
            $("#00001").append(cid);
            arr.push(Global.PostData.Data.chineseContent);
            arr.push(Global.PostData.Data.englishContent);
            arr.push(Global.PostData.Data.otherContent);
            Global.Alternately("msg_00001", arr, changeTime * 1000 * 2);
        }
}

//初始化时将时间轴写入，后台调用 GUID_中文_英文
Global.SetTimeAxis = function (data)
{
    timeAxisList.push(data);
}
Global.GetTimeAxis = function () {
    return timeAxisList;
}
Global.GetUrlParam = function (name)
{
    var reg = new RegExp("(^|#)" + name + "=([^&]*)(&|$)");
    var r = window.location.href.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}
//新版定时器中英文切换 2017/03/24
//guid GUID
//content 内容
//otherContent 其他内容
Global.TransSferAlternatelyV2=function (guid, content, otherContent) {
    var arr = new Array();
    arr.push(content);
    if (otherContent != null && otherContent.length > 0) {
        if (otherContent.indexOf('_') != -1) {
            var split = otherContent.split('_');
            for (var i = 0; i < split.length ; i++) {
                if (split[i].length > 0) {
                    arr.push(split[i]);
                }
            }
        }
        else {
            arr.push(otherContent);
        }
    }
    Global.SetCustomTimeOut(guid, arr, changeTime * 1000);
};

//TimeOut自定定时器
//id 元素ID
//arr  需要展示的字符串
//time间隔时间
var timeOutList = new Array();
Global.SetCustomTimeOut = function (id, arr, time)
{
    if (arr.length < 2) {
        return;
    }
    var flag_arr = arr.slice();
    var lastData = arr[0];
    arr.splice(0, 1);
    arr.push(lastData);
    for (var i = 0; i < arr.length; i++) {
        (function (i) {
            if ($("#" + id).length > 0)
            {
              var timer = null;
              clearTimeout(timer);
              timer = setTimeout(function () {
                    $("#" + id).fadeOut(1000, function () {
                        $("#" + id).text(arr[i]);
                    });
                    $("#" + id).fadeIn(1000);
                    if ((i + 1) == arr.length)
                    { 
                        Global.SetCustomTimeOut(id, flag_arr, time);
                        flag_arr = null;
                    }
                    clearTimeout(timer);
              }, (i + 1) * time);
            }
        })(i);
    }
}

//气象设置 Start
Global.WeatherPhenomenaModel = function (wpName)
{
    var model = { Image: null, Chinese: null, English: null };
  
    if (wpName.indexOf("晴") > -1)
    {
        model.Image = "/Images/Weather/qing.png";
        model.Chinese = "晴";
        model.English = "Sun";
        return model;
    }
    if (wpName.indexOf("多云") > -1)
    {
        model.Image = "/Images/Weather/duoyun.png";
        model.Chinese = "多云";
        model.English = "Cloud";
        return model;
    }
    if (wpName.indexOf("小雨") > -1)
    {
        model.Image = "/Images/Weather/xiaoyu.png";
        model.Chinese = "小雨";
        model.English = "Light Rain";
        return model;
    }
    if (wpName.indexOf("中雨") > -1)
    {
        model.Image = "/Images/Weather/zhongyu.png";
        model.Chinese = "中雨";
        model.English = "Moderate Rain";
        return model;
    }
    if (wpName.indexOf("大雨") > -1)
    {
        model.Image = "/Images/Weather/dayu.png";
        model.Chinese = "大雨";
        model.English = "Heavy Rain";
        return model;
    }
    if (wpName.indexOf("雨") > -1)
    {
        model.Image = "/Images/Weather/zhongyu.png";
        model.Chinese = "雨";
        model.English = "Rain";
        return model;
    }
    if (wpName.indexOf("雾") > -1)
    {
        model.Image = "/Images/Weather/wu.png";
        model.Chinese = "雾";
        model.English = "Fog";
        return model;
    }
    if (wpName.indexOf("小雪") > -1)
    {
        model.Image = "/Images/Weather/xiaoxue.png";
        model.Chinese = "小雪";
        model.English = "Light Snow";
        return model;
    }
    if (wpName.indexOf("中雪") > -1)
    {
        model.Image = "/Images/Weather/zhongxue.png";
        model.Chinese = "中雪";
        model.English = "Moderate Snow";
        return model;
    }
    if (wpName.indexOf("大雪") > -1) {
        model.Image = "/Images/Weather/daxue.png";
        model.Chinese = "大雪";
        model.English = "Heavy Snow";
        return model;
    }
    if (wpName.indexOf("雪") > -1) {
        model.Image = "/Images/Weather/xiaoxue.png";
        model.Chinese = "雪";
        model.English = "Snow";
        return model;
    }
    return null;
}
Global.SetWeatherPhenomena = function (id, imgid,wpName)
{
    Global.ClearInterval(id);
    var model = Global.WeatherPhenomenaModel(wpName);
    if (model != null) {
        $("#" + id).text(model.Chinese);
        $("#" + imgid).attr("src", model.Image);
        Global.TransSferAlternately(id, model.Chinese, model.English);
    } 
}
Global.WeatherWindDirModel = function (degree)
{
    var model = { Degree: degree, Chinese: null, English: null };
    if (degree!=null)
    {
         //348.76-11.25
        if ((degree > 348.75 && degree < 361) || ( degree >-0.99 && degree < 11.26))
        {
            model.Chinese = "北";
            model.English = "N";
            return model;
        }

        //11.26-33.75
        if (degree >11.25 && degree<33.76)
        {
            model.Chinese = "东北东";
            model.English = "NNE";
            return model;
        }

        //33.76-56.25
        if (degree > 33.75 && degree < 56.26) {
            model.Chinese = "东北";
            model.English = "NE";
            return model;
        }

        //56.26-78.75
        if (degree > 56.25 && degree < 78.76) {
            model.Chinese = "东东北";
            model.English = "ENE";
            return model;
        }

        //78.76-101.25
        if (degree > 78.75 && degree < 101.26) {
            model.Chinese = "东";
            model.English = "E";
            return model;
        }

        //101.26-123.75
        if (degree > 101.25 && degree < 123.76) {
            model.Chinese = "东东南";
            model.English = "ESE";
            return model;
        }

        //123.76-146.25
        if (degree > 123.75 && degree < 146.26) {
            model.Chinese = "东南";
            model.English = "SE";
            return model;
        }

        //146.26-168.75
        if (degree > 146.25 && degree < 168.76) {
            model.Chinese = "南东南";
            model.English = "SSE";
            return model;
        }

        //168.76-191.25
        if (degree > 168.75 && degree < 191.26) {
            model.Chinese = "南";
            model.English = "S";
            return model;
        }

        //191.26-213.75
        if (degree > 191.25 && degree < 213.76) {
            model.Chinese = "南西南";
            model.English = "SSW";
            return model;
        }

        //213.76-236.25
        if (degree > 213.75 && degree < 236.26) {
            model.Chinese = "西南";
            model.English = "SW";
            return model;
        }

        //236.26-258.75
        if (degree > 236.25 && degree < 258.76) {
            model.Chinese = "西西南";
            model.English = "WSW";
            return model;
        }

        //258.76-281.25
        if (degree > 258.75 && degree < 281.26) {
            model.Chinese = "西";
            model.English = "W";
            return model;
        }

        //281.26 - 303.75
        if (degree > 281.25 && degree < 303.76) {
            model.Chinese = "西西北";
            model.English = "WNW";
            return model;
        }


        //303.76-326.25
        if (degree > 303.75 && degree < 326.26) {
            model.Chinese = "西北";
            model.English = "NW";
            return model;
        }

        //326.26-348.75
        if (degree > 326.25 && degree < 348.76) {
            model.Chinese = "西北";
            model.English = "NW";
            return model;
        }

        return null;
    }
}
Global.SetWeatherWindDir = function (id, degree)
{
        Global.ClearInterval(id);
        var model = Global.WeatherWindDirModel(degree);
        if (model != null) {
            $("#" + id).text(model.Chinese);
            Global.TransSferAlternately(id, model.Chinese, model.English);
        }
}
//气象设置 End