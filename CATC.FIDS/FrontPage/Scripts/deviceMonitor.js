var deviceMonitor = (function ($, util, layer) {
    var scrollCount = null, scrollLSCount = null;
    let $monitorCount = null;
    var pub = {
        timer: null,
        /** 输入框提示信息
         * @param selector String类型 如'#displayMonitor'
         * @param con String类型 如'1min/次'
         */
        formTips: function (selector, con) {
            let tips;
            $(selector).off();
            $(selector).on('focus', function () {
                //同时多次点击执行一次
                tips = layer.tips(con, this,{time:0});
            });
            $(selector).on('blur', function () {
                //同时多次点击执行一次
                layer.close(tips);
            });
        },
        /** 验证规则&
         *  val String类型 Number类型
         */
        verifyRules: function(val){
            //非空验证
            if(!val){
                layer.alert('值不能为空');
                return false;
            }
            //必须是整数
            if(/(\.)|(-)/.test(val)){
                layer.alert('值必须是正整数');
                return false;
            }
            return true;
        },
        /** 列表初始化请求
         * @param obj.reqData Object类型  ajax请求主体
         * @param obj.time Number类型  ajax请求主体
         */
        tableConDataInit: function (obj) {
            obj = obj || {isPrimary_select: 1};
            //循环执行->清除上一个定时器
            window.clearTimeout(pub.timer);
            //实时获取监控频率的值
            var tempVal = $monitorCount.html();
            var time = ~~tempVal;
            // get Device list
            /*var loadTips = layer.load(2, {
                shade: 0.02
            });*/
            util.sendPostReq('/DisplayInfoes/RefreshDeviceState', obj)
                .then(function (data) {
                    if (data.length > 0) {
                        var res = doT.template($('#device-monitor-tmpl').html())(data);
                        $('#device-list').html(res);                        
                    } else {$('#device-list').html('');}
                })
                .then(function(){
                    // layer.close(loadTips);
                    //必须先销毁上次的scroll实例，再初始化
                    scrollCount ? util.destroyScroll(scrollCount) : null;
                    scrollCount = util.myScroll('.js-tableCon');
                    //绑定成功之后->根据监控频率循环执行
                    pub.timer = setTimeout(function(){
                        pub.tableConDataInit(obj);
                    }, (time + 7)*1000);
                })
                .fail(function (err) {
                    layer.msg('请求发送错误');
                    // layer.close(loadTips);
                });
        },
    }

    //左侧边栏事件
    var leftSideBarEvent = function(){
        var $mainContent = $('.js-mainContent');
        $('.js-template').on('click','li',function(){
            var $this = $(this);
                $this.addClass('is-active').siblings().removeClass('is-active');
                var flag = $this.attr('data-groupid');
                //切换页面删除loadding层，以免造成切换页面之后loading层还在显示的问题
                $('.layui-layer-shade').remove();
                $('.layui-layer-loading').remove();
                if(flag === 'all'){
                    pub.tableConDataInit();
                } else {
                    pub.tableConDataInit({isPrimary_select:1, displayGroup_name: flag});
                }
        });

    };
    //设备监控按钮事件
    var deviceMonitorBtnEvent = function(){
        var $modalMonitor = $('.js-modal-monitor .js-boxMask');
        $('#menu-monitor').on('click', function(){
            var temp = $(this).find('.js-monitor-count').html();
            $('#displayMonitor').val(temp);
            $modalMonitor.show();
        });
    }
    var modalBtnEvent = function(){
        let $resetBtn = $('.js-modal-device .js-reset');
        pub.formTips('#displayMonitor', '最小值1s，单位: s/次');
        //确认按钮
        $('.js-modal-device .js-btnEnsure').on('click', function () {
            var $curboxMask = $(this).closest('.mod-boxMask');
            var tempVal = $('#displayMonitor').val();
            var isRight = pub.verifyRules(tempVal);
            //非空验证
            if(isRight){
                var loadTips = layer.load(2, {shade: 0.02});
                util.sendPostReq('/DisplayInfoes/UpdateConnectTimeALL',{request_time: tempVal}).then(function(data){
                    if(data.Status){
                        layer.msg(data.Message);
                    } else {
                        layer.msg(data.Message);
                    }
                 })
                 .then(function(){                    
                    $resetBtn.trigger('click');
                    $curboxMask.hide(); 
                    $('#menu-monitor .js-monitor-count').html(tempVal);
                 })
                 .fail(function(err){
                    layer.alert('请求发送错误');
                 })
                 .always(function(){
                     layer.close(loadTips);
                 });
            }                 
        });
        //取消按钮
        $('.js-modal-device .js-btnCancel').on('click', function () {
            var $curboxMask = $(this).closest('.mod-boxMask');
            $resetBtn.trigger('click');
            $curboxMask.hide();
        });
        //关闭按钮
        $('.js-modal-device .js-btnClose').on('click', function () {
            var $curboxMask = $(this).closest('.mod-boxMask');
            $resetBtn.trigger('click');
            $curboxMask.hide();
        });
    }
    var init = function() {
        //缓存监控设备次数值
        $monitorCount = $('#menu-monitor .js-monitor-count');
        //首次加载->请求全部设备列表
        pub.tableConDataInit();
        
        //左侧边栏->请求设备组列表
        util.sendPostReq('/DisplayInfoes/SelectGroupInfor')
                .then(function (data) {
                    if (data.length > 0) {
                        // console.log(JSON.parse(data));  
                        let res = doT.template($('#device-group-tmpl').html())(data);
                        $('.js-template').html(res);

                    } else {$('.js-template').html('');}
                })
                .then(function(){
                    leftSideBarEvent();
                    //必须先销毁上次的scroll实例，再初始化
                    scrollLSCount ? util.destroyScroll(scrollCount) : null;
                    scrollLSCount = util.myScroll('.js-leftSildeBar');
                })
                .fail(function (err) {
                    layer.msg('请求发送错误');
                });

        
        
        
        deviceMonitorBtnEvent();
        modalBtnEvent();

    };




    return {
        init: init
    }
})(jQuery, util, layer);

$(function () {
    // computed rem
    util.dynamicCalcRem(1920);
    $('body').show();

    // init index.html
    deviceMonitor.init();

});