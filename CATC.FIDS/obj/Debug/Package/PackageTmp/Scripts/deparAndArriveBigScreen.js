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
            layer.alert('����������ʧ�ܣ���鿴�����Ƿ�ͨ��');
        },
        success: defaultCallback
    });
}
function defaultCallback(data) {
    _defaultData = data;
}
//end arrivalflight ajax

//��Ӣ���л�����
var changed = 0;
//�л�ʱ��
var changeTime = 5;
//����Ԫ��ʱ��id
var dataFlag = 1000;
//���ڲ���ҳ��ͷ�������Զ���Ԫ��ID
var headerIDs = new Array();
//���ڲ���ҳ������������Զ���Ԫ��ID
var bodyIDs = new Array();

var DeparAndArriveBigScreen =
    {
        Init: function () { DeparAndArriveBigScreen.ListInit(); }
    }
DeparAndArriveBigScreen.ListInit = function () {
    //���ø�����ķ�����ʼ��table
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
    //��ѯ��ȡ��Ϣ
    setInterval(function () {
        Global.GetEventMessage("DeparAndArriveBigScreen");
    }, changeTime * 1000);
}

DeparAndArriveBigScreen.CreateBgColor = function () {
    //��������body��
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
    //����label
    tag = "1";
    var url = "/Template/GetGUID";
    var data = {};
    _defaultData = null;
    defaultAjax(url, data, defaultCallback);
    var divStr = Global.CreateBgDivStr(_defaultData.Data, "labelBox", tag, "���༭�ı���");
    $(".header").append(divStr);
    $("#" + _defaultData.Data).draggable({ containment: $(".header") });
    headerIDs.push(_defaultData.Data);
    Global.CreateMenu(_defaultData.Data, tag);
    toastr.info((new Date()).toLocaleTimeString() + " �������ı�Ԫ�أ�");
}
DeparAndArriveBigScreen.CreateImage = function () {
    //����image
    tag = "2";
    var url = "/Template/GetGUID";
    var data = {};
    _defaultData = null;
    defaultAjax(url, data, defaultCallback);
    var divStr = Global.CreateDefDivStr(_defaultData.Data, "labelBox", tag, "���༭ͼƬ��");
    $(".header").append(divStr);
    $("#" + _defaultData.Data).draggable({ containment: $(".header") });
    headerIDs.push(_defaultData.Data);
    Global.CreateMenu(_defaultData.Data, tag);
    //��ѯ��ȡ��Ϣ
    setInterval(function () {
        Global.GetEventMessage("BSArrivalAndArrival");
    }, changeTime * 1000);
}
DeparAndArriveBigScreen.CreateTime = function () {
    //����ʱ��
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
        //��ֹ��������Ч��
        $("#" + headerIDs[i]).css("opacity", "1.0");
        headerArr.push(Global.GetStyles(headerIDs[i]));
    }
    if (headerArr.length == 0) {
        layer.msg('ҳ�����޸Ķ�,���豣�棡', { time: 1000 });
        return;
    }
    //ҳ�汳��config
    var bid = $("body").attr("id");
    var config = "";
    if (bid != null) {
        config = Global.GetStyles(bid);
    }
    var url = "/Template/DoAddCurrentTemplate";//ͨ��ģ��
    var data = { header: headerArr.join("||"), body: bodyIDs.join("||"), config: config };
    _defaultData = null;
    defaultAjax(url, data, defaultCallback);
    if (_defaultData.Status == 1) {
        layer.confirm('����ѱ������,�鿴Ԥ��Ч����', { icon: 1, title: '��ʾ', time: 5000 }
            , function (index) {
                window.open("/Actual/BSArrivalAndArrival");
            });
    } else {
        layer.msg(_defaultData.Message, { time: 1000 });
    }
}
DeparAndArriveBigScreen.Reset = function () {

    layer.confirm('���۱༭ģ��ָ��������ã�', { icon: 3, title: 'ȷ��' }
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
