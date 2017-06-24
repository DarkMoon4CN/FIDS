import flightData from './flightData';
import tmplManage from './templateManagement';
import deviceManage from './deviceManagement';
import logManage from './logManagement';
import permissManage from './permissManagement';
import msgRelease from './messageRelease';

let indexRender = (function ($, util) {
    let form = layui.form();
    //leftSideBar click
    const leftSideBarEvent = () => {
        let $mainContent = $('.js-mainContent');
        $('.js-template').on('click','li',function(){
            let $this = $(this);
                $this.addClass('is-active').siblings().removeClass('is-active');
                //切换页面删除loadding层，以免造成切换页面之后loading层还在显示的问题
                $('.layui-layer-shade').remove();
                $('.layui-layer-loading').remove();
                //跳转页面
                let curAttr = $this.attr('data-href');
                switch (curAttr) {
                    case 'flightData':
                        util.loadJsCss('Content/flightData.css');
                        util.loadJsCss('Content/modal-flightData.css');
                        util.bindHtmlToIndex('./mainTmpl/flightData.html', function (data) {
                            $mainContent.html(data);
                            flightData.render();
                        });
                        break;
                    case 'tmplManage':
                        util.loadJsCss('Content/templateManagement.css');
                        util.loadJsCss('Content/modal-tmplManage-big.css');
                        util.bindHtmlToIndex('./mainTmpl/templateManagement.html', function (data) {
                            $mainContent.html(data);
                            tmplManage.render();
                        });
                        break;
                    case 'tmplEdit':
                        util.loadJsCss('Content/templateEdit.css');
                        util.bindHtmlToIndex('./mainTmpl/templateEdit.html', function (data) {
                            $mainContent.html(data);
                        });
                        break;
                    case 'deviceManage':
                        util.loadJsCss('Content/modal-device.css');
                        util.loadJsCss('Content/deviceManagement.css');
                        util.bindHtmlToIndex('./mainTmpl/deviceManagement.html', function (data) {
                            $mainContent.html(data);
                            deviceManage.render();
                        });
                        break;
                    case 'msgPub':
                        util.loadJsCss('Content/messageRelease.css');
                        util.loadJsCss('Content/modal-msg.css');
                        util.bindHtmlToIndex('./mainTmpl/messageRelease.html', function (data) {
                            $mainContent.html(data);
                            msgRelease.render();
                        });
                        break;
                    case 'perManage':
                        util.loadJsCss('Content/permissManagement.css');
                        util.loadJsCss('Content/modal-permiss.css');
                        util.bindHtmlToIndex('./mainTmpl/permissManagement.html', function (data) {
                            $mainContent.html(data);
                            permissManage.render();
                        });
                        break;
                    case 'logManage':
                        util.loadJsCss('Content/logManagement.css');
                        util.bindHtmlToIndex('./mainTmpl/logManagement.html', function (data) {
                            $mainContent.html(data);
                            logManage.render();
                        });
                        break;
                }
        });
        
    };

    const init = () => {
        leftSideBarEvent();
    };
    return {
        init: init
    }
})(jQuery, util);

$(function () {
    // computed rem
    util.dynamicCalcRem(1920); 
    $('body').show();

    // init index.html
    indexRender.init();

    // default show flightData.html
    
    util.loadJsCss('Content/flightData.css');
    util.loadJsCss('Content/modal-flightData.css');
    util.bindHtmlToIndex('./mainTmpl/flightData.html', function (data) {
        $('.js-mainContent').html(data);
        let form = layui.form();
        //首次加载获取任务代码数据->插入航班计划弹框中
        util.sendPostReq('/FlightDataApi/GetTaskCode',{pageSize:1000}).then(function(data){
            if(data.Data.List.length > 0){
                let res = doT.template($('#modal-task-code-tmpl').html())(data.Data.List);
                $('#task-code-d').html(res);   
                $('#task-code-a').html(res);
                form.render();
            }
               
        });

        flightData.render();

    });
});