/// <reference path="departuresBigScreen.js" />
///  FOR  departuresBigScreen
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

var DeparturesBigScreen =
    {
        Init: function () { DeparturesBigScreen.ListInit(); }
    }
DeparturesBigScreen.ListInit = function () {
    //���ø�����ķ�����ʼ��table
    if (self != top) {
        var parr = window.parent.GetTableColumn();
        Global.SetTableColumn(parr);
    }
    DeparturesBigScreen.CreateVideo();
}
DeparturesBigScreen.CreateVideo = function ()
{
    var tag = "8";
    var url = "/Template/GetGUID";
    var data = {};
    Global.Post(url, data, Global.PostCallback);
    $(".videoDisplay").attr("id", Global.PostData.Data);
    $(".videoDisplay").attr("tag",tag);
    Global.CreateMenu(Global.PostData.Data, tag);
}
DeparturesBigScreen.FillVideo = function (json) {
    var video = $("#" + json.ID);
    video.html("");
    video.append(Global.CreatVideoStr());
    video.children(":first").attr("src", json.Urls);
    //������Ϻ�ֱ�ӱ��棡
    var headerArr = new Array();
    var bodyArr = new Array();
    bodyIDs.push(json.ID);
    for (var i = 0; i < bodyIDs.length; i++) {
        //��ֹ��������Ч��
        $("#" + bodyIDs[i]).css("opacity", "1.0");
        bodyArr.push(Global.GetStyles(bodyIDs[i]));
    }
    var data = { header: headerArr.join("||"), body: bodyArr.join("||") };
    
    var url = "/Template/DoAddBSDepartureTemplate";
    Global.Post(url, data, Global.PostCallback);
    if (Global.PostData.Status == 1) {
        layer.confirm('����ѱ������!', { icon: 1, title: '��ʾ', time: 5000 }
            , function (index) {
                //window.open("/Actual/BSDepartureflight");
            });
    } else
    {
        layer.msg(Global.PostData.Message, { time: 1000 });
    }
}

//ҳ������ʵʱ�ı䷽��
DeparturesBigScreen.GetDepCheckinsAndGate = function () {
    var url = "/Message/GetDepCheckinsAndGate";
    var fdids = $("#fdids").val();
    var data = { fdids: fdids };
    Global.Post(url, data, Global.PostCallback);
    //status=13, checkins=14,gate=16
    if (Global.PostisSuccess == false) {
        return;
    }
    var fdList = Global.PostData.Data;
    for (var i = 0; i < fdList.length; i++) {
        var fd = fdList[i];
        if (fd.Status_Code != null) {
            var status = $("#fid_13_" + fd.FDID);
            if (status.text().indexOf(fd.Status_CHINESE_NAME) == -1 && status.text().indexOf(fd.Status_ENGLISH_NAME) == -1) {
                $("#fid_13_" + fd.FDID).text(fd.Status_CHINESE_NAME);
                $("#fid_13_" + fd.FDID).css("background-color", fd.Status_Color);
                //�Ƴ���ǰ����Ч��
                Global.ClearInterval("fid_13_" + fd.FDID);
                //�����µĹ���Ч��
                Global.TransSferAlternately("fid_13_" + fd.FDID, fd.Status_CHINESE_NAME, fd.Status_ENGLISH_NAME);
            }

            var checkins = $("#fid_14_" + fd.FDID);
            if (fd.CheckIns_Display_Symbol != null) {
                if (checkins.text().indexOf(fd.CheckIns_Display_Symbol) == -1) {
                    //�Ƴ���ǰ����Ч��
                    Global.ClearInterval("fid_14_" + fd.FDID);
                    $("#fid_14_" + fd.FDID).text(fd.CheckIns_Display_Symbol);
                }
            }

            var gate = $("#fid_16_" + fd.FDID);
            if (fd.Gate_Display_Symbol != null) {
                if (gate.text().indexOf(fd.Gate_Display_Symbol) == -1) {
                    //�Ƴ���ǰ����Ч��
                    Global.ClearInterval("fid_16_" + fd.FDID);
                    $("#fid_16_" + fd.FDID).text(fd.Gate_Display_Symbol);
                }
            }

        }
    }

}