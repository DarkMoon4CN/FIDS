/// <reference path="departureflight.js" />
///  FOR  /Departureflight
//start Departureflight ajax
var _defaultData;
var _isSuccess = true;

function defaultAjax(url, data, callback) {
    try {
        $.ajax({
            url: url,
            type: 'POST',
            data: data,
            async: false,
            error: function () {
                _isSuccess = false;
                toastr.info("与服务器中断了链接...");
            },
            success: defaultCallback
        });
    } catch (e)
    {
      
    }
}
function defaultCallback(data) {
    _defaultData = data;
}
//end Departureflight ajax
//中英文切换变量
var changed = 0;
//切换时间
var changeTime =5;
//创建元素时的id
var dataFlag = 1000; 
//用于操作页面头的所有自定义元素ID
var headerIDs = new Array();
//用于操作页面主体的所有自定义元素ID
var bodyIDs = new Array();
//用于操作页面页脚的所有自定义元素ID
var footerIDs = new Array();

//离港编辑JS类
var Departureflight =
{
        Init: function () { Departureflight.ListInit(); }
    }
Departureflight.ListInit = function ()
{
    //调用父窗体的方法初始化table
    if (self != top) {
        var parr = window.parent.GetTableColumn();
        Global.SetTableColumn(parr);
    }
    $(".menu-text").on("click", function () {
        Departureflight.CreateLabel();
    });
    $(".menu-pic").on("click", function () {
        Departureflight.CreateImage();
    });
    $(".menu-time").on("click", function () {
        Departureflight.CreateTime();
    });
    $(".menu-save").on("click", function () {
        Departureflight.Submit();
    });
    $(".menu-clear").on("click", function () {
        Departureflight.Reset();
    });
    $(".menu-bg").on("click", function () {
        Departureflight.CreateBgColor();
    });

    $(".menu-msg").on("click", function () {
        //Departureflight.CreateMessage();
    });
    //轮询获取消息
    setInterval(function () {
        Global.GetEventMessage("DepartureFlight");
    }, changeTime * 1000);
}
Departureflight.CreateBgColor = function ()
{
    //背景加在body上
    tag = "9";
    var bid= $("body").attr("id");
    if(bid ==null || typeof(bid)=="undefined")
    {
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
Departureflight.CreateLabel = function () {
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
Departureflight.CreateImage = function ()
{
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
}
Departureflight.CreateTime = function () {
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
Departureflight.CreateMessage = function ()
{
    //创建消息
    tag = "7";
    var url = "/Template/GetGUID";
    var data = {};
    Global.Post(url, data, Global.PostCallback);
    var divStr = Global.CreateDefDivStr(Global.PostData.Data, "tips", tag, "待编辑消息域");
    $(".footer").append(divStr);
    footerIDs.push(Global.PostData.Data);
    Global.CreateMenu(Global.PostData.Data, tag);
}
Departureflight.Submit = function ()
{
    var headerArr = new Array();
    for (var i = 0; i < headerIDs.length; i++)
    {
        //防止淡进淡出效果
        $("#" + headerIDs[i]).css("opacity", "1.0");
        headerArr.push(Global.GetStyles(headerIDs[i]));
    }
    if (headerArr.length == 0)
    {
        layer.msg('页面暂无改动,无需保存！', { time: 1000 });
        return;
    }
    //页面背景config
    var bid=$("body").attr("id");
    var config ="";
    if (bid != null)
    {
        config = Global.GetStyles(bid);
    }
    var url = "/Template/DoAddDepartureTemplate";
    var data = { header:headerArr.join("||"),body:bodyIDs.join("||"),config:config};
    _defaultData = null;
    defaultAjax(url, data, defaultCallback);
    if (_defaultData.Status == 1) {
        layer.confirm('组件已保存完毕,查看预览效果？', { icon: 1, title: '提示', time: 5000 }
            , function (index) {
               window.open("/Actual/Departureflight");
            });
    } else
    {
        layer.msg(_defaultData.Message, {time:1000});
    }
}
Departureflight.Reset = function ()
{
    layer.confirm('离港编辑模块恢复出厂设置？', { icon: 3, title: '确认' }
        , function (index) {
            var url = "/Template/Reset";
            var data = { ak: "DepartureFlight" };
            _defaultData = null;
            defaultAjax(url, data, defaultCallback);
            layer.msg(_defaultData.Message, { time: 2000 }, function () {
                window.parent.location.href = _defaultData.Data;
            });
       });  
}

//页面数据实时改变方法
Departureflight.GetDepCheckinsAndGate = function ()
{
    var url = "/Message/GetDepCheckinsAndGate";
    var fdids = $("#fdids").val();
    var data = { fdids: fdids };
    Global.Post(url, data, Global.PostCallback);
    //status=13, checkins=14,gate=16
    if (Global.PostisSuccess==false)
    {
        return;
    }
    var fdList = Global.PostData.Data;
    for (var i = 0; i < fdList.length; i++)
    {
        var fd = fdList[i];
        if (fd.Status_Code != null) {
            var status = $("#fid_13_" + fd.FDID);
            if (status.text().indexOf(fd.Status_CHINESE_NAME) ==-1 && status.text().indexOf(fd.Status_ENGLISH_NAME)==-1)
            {
                $("#fid_13_" + fd.FDID).text(fd.Status_CHINESE_NAME);
                $("#fid_13_" + fd.FDID).css("background-color",fd.Status_Color);
                //移除当前滚动效果
                Global.ClearInterval("fid_13_" + fd.FDID);
                //增加新的滚动效果
                Global.TransSferAlternately("fid_13_" + fd.FDID, fd.Status_CHINESE_NAME, fd.Status_ENGLISH_NAME);
            }
           
            var checkins = $("#fid_14_" + fd.FDID);
            if (fd.CheckIns_Display_Symbol != null) {
                if (checkins.text().indexOf(fd.CheckIns_Display_Symbol) == -1) {
                    //移除当前滚动效果
                    Global.ClearInterval("fid_14_" + fd.FDID);
                    $("#fid_14_" + fd.FDID).text(fd.CheckIns_Display_Symbol);
                }
            }

            var gate = $("#fid_16_" + fd.FDID);
            if (fd.Gate_Display_Symbol != null) {
                if (gate.text().indexOf(fd.Gate_Display_Symbol) == -1) {
                    //移除当前滚动效果
                    Global.ClearInterval("fid_16_" + fd.FDID);
                    $("#fid_16_" + fd.FDID).text(fd.Gate_Display_Symbol);
                }
            }

        }
    }
   
}
