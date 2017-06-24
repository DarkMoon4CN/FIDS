export default (function deviceManage($, util, layui, layer, doT) {
    let $boxMask    = null,
        $cfBoxMask  = null;
    let scrollCount = null;
    //初始化layui表单
    let form = layui.form();

    let getElem = function(){
        $boxMask    = $('.js-modal-edit .js-boxMask'),
        $cfBoxMask  = $('.confirm .mod-boxMask');
    }
    //公共方法
    let pub = {
        timer:null,
        getDeviceName: function () {
            var arrCon = [];
            $('.js-tableCon').find('.checkbox.checkbox-checked').parent().next().each(function (index, item) {
                var cur = item.innerHTML;
                arrCon.push(cur);
            });
            return arrCon.join(', ');
        },
        //获取设备ID
        getDeviceId: function () {
            let curIds = [];
            $('.js-tableCon').find('.checkbox.checkbox-checked').closest('.row').each(function (i, item) {
                let cur = item.getAttribute('data-deviceid');
                curIds.push(cur);
            });
            return curIds.join(',');
        },
        //发送控制命令
        sendCtrlDirect: function (directName, deviceIds) {
            util.sendPostReq('/DisplayInfoes/ControlMethod', {
                    "Message": directName,
                    "Data": deviceIds
            })
            .then(function (data) {
                layer.msg(data.Message);
                pub.onStartChangeCon();
            }).fail(function (err) {
                layer.msg('请求发送错误');
            });
        },
        //首次加载->获取列表
        tableConDataInit: function (obj) {
            obj = obj || {isPrimary_select: 1};

            // get Device list
            let loadTips = layer.load(2, {
                shade: 0.02
            });
            util.sendPostReq('/DisplayInfoes/SelectAllRecord', obj)
                .then(function (data) {
                    if (data.length > 0) {
                        // console.log(JSON.parse(data));  
                        let res = doT.template($('#device-list-tmpl').html())(data);
                        $('#device-list').html(res);

                        //必须先销毁上次的scroll实例，再初始化
                        scrollCount ? util.destroyScroll(scrollCount) : null;
                        scrollCount = util.myScroll('.js-tableCon');
                    } else {$('#device-list').html('');}
                }).fail(function (err) {
                    layer.msg('请求发送错误');
                }).always(function () {
                    layer.close(loadTips);
                });
        },
        //获取选中行数据，只能获取单行
        getDeviceAttr: function () {
            const $deviceList = $('#device-list').children('.is-active');
            const $deviceListChilds = $deviceList.children();
            let obj = {};
            obj.displayId = $deviceList.attr('data-deviceid');
            obj.displayName = $deviceListChilds.map(function (i, item) {
                return item.getAttribute('data-displayname');
            })[0];
            obj.displayGroup = $deviceListChilds.map(function (i, item) {
                return item.getAttribute('data-displaygroup');
            })[0];
            obj.displayMark = $deviceListChilds.map(function (i, item) {
                return item.getAttribute('data-displaymark');
            })[0];
            obj.ip = $deviceListChilds.map(function (i, item) {
                return item.getAttribute('data-ip');
            })[0];
            obj.displayLumi = $deviceListChilds.map(function (i, item) {
                return item.getAttribute('data-displaylumi');
            })[0];
            obj.bootType = $deviceListChilds.map(function (i, item) {
                return item.getAttribute('data-boottype');
            })[0];
            obj.isPrimary = $deviceList.attr('data-primary');

            return obj;
        },
        //绑定数据到弹框
        bindDataToModal: function (data) {
            clearTimeout(pub.timer);
            let $modalDevice = $('.js-modal-device');
            //设备组名称
            $modalDevice.find('input[name="deviceName"]').val(data.displayName);
            //设备ID
            $modalDevice.find('input[name="deviceId"]').val(data.displayId);
            //是否有效
            $modalDevice.find('input[name="isPrimary"]').val(data.isPrimary);
            //设备IP
            $modalDevice.find('input[name="deviceIp"]').val(data.ip);
            //设备描述
            $modalDevice.find('input[name="deviceDesc"]').val(data.displayMark);
            //设备亮度
            $modalDevice.find('input[name="displayProp"]').val(data.displayLumi);
            //设备组选择   有极小的概率发生绑定数据到弹框之前获取不到设备列表，所以先form.render
            pub.timer = setTimeout(function () {
                $modalDevice.find('select[name="deviceGroup"] option[value="'+data.displayGroup+'"]').prop('selected', 'selected');
                form.render();
            }, 0);
           

            //启动类型 1为自动  0为手动
            data.bootType === '1' ? $('#startAuto').prop('checked', 'checked') : $('#startHand').prop('checked', 'checked');

            form.render();
        },
        //发送表单数据
        bindLayuiEvent: function () {
            let obj = {};
            //获取下拉选择框的value
            form.on('select(modal-device-sel)', function (data) {
                if(data.elem.name === 'deviceGroup'){
                    obj.displayGroup_name = data.elem.value;
                }
            });
            //submit提交
            form.on('submit', function (data) {
                //先清空定时器
                clearTimeout(pub.timer);
                var formObj = data.field;
                //设备ID
                obj.displayID_name = formObj.deviceId;
                //设备名称
                obj.displayName_name = formObj.deviceName;
                //设备组名
                if(formObj.deviceGroup === 'all'||!formObj.deviceGroup) {
                    layer.alert('设备组不能为空');
                    return;
                }
                obj.displayGroup_name = formObj.deviceGroup;
                //设备描述
                obj.displayMark_name = formObj.deviceDesc;
                //设备亮度
                obj.displayLuminance_name = formObj.displayProp;
                //是否是设备有效
                obj.isPrimary_name = formObj.isPrimary;
                if (obj.displayLuminance_name > 150) {
                    layer.alert('亮度最大值为150');
                    return;
                }
                if (obj.displayLuminance_name < 0) {
                    layer.alert('亮度最小值为0');
                    return;
                }
                // 发送数据前检测obj是否为空  
                let isEmptyObject = $.isEmptyObject(obj);
                if (!isEmptyObject) {
                    let loadTips = layer.load(2, {
                        shade: 0.02
                    });
                    //发送请求
                    util.sendPostReq('/DisplayInfoes/UpdateOneRecord', obj)
                        .then(function (data) {
                            layer.msg(data.Message+ ' '+ data.Data);
                            $boxMask.hide();
                            $('.js-reset').trigger('click');
                            //发送成功之后刷新页面，因为发送命令之后设备返回反馈信息需要一点时间，所以用定时器
                            pub.timer = setTimeout(function(){
                                //根据设备选择框的值来判定当前刷新的数据列表
                                pub.deviceSelStateChangeCon();
                            },2000);
                            
                        })
                        .fail(function (err) {
                            layer.msg(err);
                        })
                        .always(function () {
                            layer.close(loadTips);
                        });

                }
                return false;
            });
        },
        //清除列表选中样式
        clearChellAllEffect: function () {
            let $tableCon = $('.js-equipTable .js-tableCon');
            //清除复选框选中样式
            $tableCon.find('.checkbox-checked').removeClass('checkbox-checked');
            //清除全选按钮样式
            $('.js-allEquip').removeClass('checkbox-checked');
            //清除行选中效果
            $tableCon.find('.is-active').removeClass('is-active');
        },
        //清除选项卡选中效果
        backToTabDefault: function(){
            $('.js-tabCheck').children().eq(0).addClass('is-active').siblings().removeClass('is-active');
        },
        //输入框提示信息
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
        //ajax获取值并绑定到弹框
        getModalData: function (url, dotTmplID, tmplID, data) {
            //loading
            let loadTips = layer.load(2, {
                shade: 0.02
            });
            //tmpl's list
            util.sendPostReq(url, data)
                .then(function (data) {                   
                    if (data.length > 0) {
                        let tmplVal = $('#' + dotTmplID).html();
                        let res = doT.template(tmplVal)(data);           
                        $('#' + tmplID).html(res);
                        form.render();
                    }
                }).fail(function (err) {
                    layer.msg(err);
                }).always(function () {
                    layer.close(loadTips);
                });
        },
        //获取设备组ID
        getDeviceGroId: function(){
           let cur = $('#deviceGroupSelect').val();
           if(cur === 'all') {
               return;
           } else {
               return cur;
           }
           
        },
        //根据设备选择框的值和选项卡来判定当前刷新的数据列表
        onStartChangeCon: function(){
            let curState = $('#deviceGroupSelect').val();
            let curFlag = $('.js-tabCheck').children('.is-active').attr('data-flag');
            let id = ~~curState;
            switch(curFlag){
                case 'monitor':
                    if(curState === 'all'){
                        pub.tableConDataInit({isPrimary_select: 1 });
                    } else {
                        pub.tableConDataInit({
                            isPrimary_select:1,
                            displayGroup_name: id
                        });
                    }
                    break;
                case 'valid':
                    if(curState === 'all'){
                        pub.tableConDataInit({isPrimary_select: 1 });
                    } else {
                        pub.tableConDataInit({
                            isPrimary_select:1,
                            displayGroup_name: id
                        });
                    }
                    break;
                case 'invalid':
                    if(curState === 'all'){
                        pub.tableConDataInit({isPrimary_select: 0 });
                    } else {
                        pub.tableConDataInit({
                            isPrimary_select:0,
                            displayGroup_name: id
                        });
                    }
                    break;
            }
            
            /*//获取当前设备组的值
            function getCurDevSelVal(){

            }*/
        },
        //动态改变确认框显示内容
        changeCfModalCon: function(menuCon, str){
            $('.js-modal-device .js-modalTitle').html(menuCon);
            $('.confirm .js-modalCon').html('是否'+ menuCon+ ':' + str);
            $cfBoxMask.show();
        }
    };

    //设备列表事件
    let tableConEvent = function () {
        let $tableCon    = $('.js-equipTable .js-tableCon');
        let $menuDisplay = $('.js-menuDisplay');
        let $menuDisable = $('.menu-disable');
        //监控设备、有效设备和无效设备选项卡
        $('.js-tabCheck').on('click', 'div',function (e) {
            let $this = $(this);
            $this.addClass('is-active').siblings().removeClass('is-active');
            let curFlag = $this.attr('data-flag');
            //清除上一选项卡页面的选中效果
            pub.clearChellAllEffect();
            let deviceGroId = pub.getDeviceGroId();
            switch (curFlag) {
                case 'valid':
                    $menuDisplay.addClass('menuHide');
                    //改变menu->监控设备和有效及无效设备效果不同
                    $menuDisable.show();
                    //请求有效设备列表
                    pub.tableConDataInit({
                        isPrimary_select: 1,
                        displayGroup_name: deviceGroId
                    });
                    break;
                case 'invalid':
                    $menuDisplay.removeClass('menuHide');
                    //改变menu->监控设备和有效及无效设备效果不同
                    $menuDisable.hide();
                    //请求无效设备列表
                    pub.tableConDataInit({
                        isPrimary_select: 0,
                        displayGroup_name: deviceGroId
                    });
                    break;

            }
             e.stopPropagation();
        });

        //tableCon列表点击效果
        $tableCon.on('click', function (e) {
            var tar = e.target,
                parTag = tar.parentNode,
                flag = $(parTag).hasClass('row');
            if (flag) {
                //点击当前row时->移除全部row复选框的效果
                $(this).find('.checkbox').removeClass('checkbox-checked');
                $('.js-allEquip').removeClass('checkbox-checked');
                //点击时背景改变
                if ($(parTag).attr('switch')) {
                    $(parTag).removeAttr('switch');
                    $(parTag).removeClass('is-active');
                    $(parTag).find('.checkbox').removeClass('checkbox-checked');
                } else {
                    $(parTag).attr('switch', 'on').siblings().removeAttr('switch');
                    $(parTag).addClass('is-active').siblings().removeClass('is-active');
                    $(parTag).find('.checkbox').addClass('checkbox-checked');
                }
            }
        });
        //列表的每个复选框 & row背景色改变
        $tableCon.on('click', '.checkbox', function (e) {
            $('.js-allEquip').removeClass('checkbox-checked');
            let $this = $(this),
                $row = $this.closest('.row');
            let flag = $this.hasClass('checkbox-checked');
            if (!flag) {
                $this.addClass('checkbox-checked');
                $row.addClass('is-active');
            } else {
                $this.removeClass('checkbox-checked');
                $row.removeClass('is-active');
                // $('.js-allEquip').removeClass('checkbox-checked');
            }
             e.stopPropagation();
        });
        //全选按钮
        $('.js-allEquip').on('click', function (e) {
            var tar = e.target,
                gar = tar.parentNode.parentNode,
                $this = $(this);
            var $tableCon = $(gar).siblings('.js-tableCon');
            var $checkboxs = $tableCon.find('.checkbox');
            var $rows = $tableCon.find('.row');
            var flag = $this.hasClass('checkbox-checked');
            if (!flag) {
                //$(this)-> 控制的是当前按钮的开关效果
                $this.addClass('checkbox-checked');
                $rows.addClass('is-active');
                $checkboxs.addClass('checkbox-checked');
                $rows.removeAttr('switch');
            } else {
                $this.removeClass('checkbox-checked');
                $checkboxs.removeClass('checkbox-checked');
                $rows.removeClass('is-active');
            }

        });
    };
    //菜单按钮事件
    let menuEvent = function () {
        let $modalTitle = $('.js-modal-device .js-modalTitle');
        $('.js-menu').on('click', function (e) {
            let tar = e.target,
                menuCon = $(tar).text(),     
                str;
            //获取设备列表被选中的设备名称
            str = pub.getDeviceName();

            let checkLength = $('.js-tableCon').find('.checkbox.checkbox-checked').length;
            switch (tar.className) {
                case 'menu-set':
                    if (checkLength === 0) {
                        layer.alert('未选中设备');
                    } else if (checkLength > 1) {
                        layer.alert('只能设置单个设备的属性');
                    } else {
                        $modalTitle.html(menuCon);
                        pub.bindLayuiEvent();
                        //获取弹框的设备组信息
                        pub.getModalData('/DisplayInfoes/SelectGroupInfor', 'deviceGro-tmpl', 'deviceGroup');
                        //获取选中行数据
                        let deviceData = pub.getDeviceAttr();
                        //绑定到弹框表单
                        pub.bindDataToModal(deviceData);
                        $boxMask.show();
                    }
                    break;
                case 'menu-boot':
                    if (checkLength === 0) {
                        layer.alert('未选中设备');
                    } else {
                        pub.changeCfModalCon(menuCon,str);
                    }
                    break;
                case 'menu-stop':
                    if (checkLength === 0) {
                        layer.alert('未选中设备');
                    } else {
                        pub.changeCfModalCon(menuCon,str);
                    }
                    break;
                case 'menu-reboot':
                    if (checkLength === 0) {
                        layer.alert('未选中设备');
                    } else {
                        pub.changeCfModalCon(menuCon,str);
                    }
                    break;
                case 'menu-enabled':
                    if (checkLength === 0) {
                        layer.alert('未选中设备');
                    } else {
                        pub.changeCfModalCon(menuCon,str);
                    }

                    break;
                case 'menu-disable':
                    if (checkLength === 0) {
                        layer.alert('未选中设备');
                    } else {
                        pub.changeCfModalCon(menuCon,str);
                    }
                    break;
            }
        });

    };

    //弹框中的事件
    let modalBtnEvent = function () {
        let $resetBtn = $('.js-modal-device .js-reset'),
            $cfResetBtn = $('.js-modal-device .confirm .js-cfReset');
        //显示属性提示单位
        pub.formTips('#displayProp', '最大值150，最小值0，单位:(%)');
        /***** modal btnEvent *****/
        //确认按钮
        /*$('.js-modal-device .js-btnEnsure').on('click', function () {
                      
        });*/
        //取消按钮
        $('.js-modal-device .js-btnCancel').on('click', function () {
            let $curboxMask = $(this).closest('.mod-boxMask');
            $resetBtn.trigger('click');
            $curboxMask.hide();
        });
        //关闭按钮
        $('.js-modal-device .js-btnClose').on('click', function () {
            let $curboxMask = $(this).closest('.mod-boxMask');
            $resetBtn.trigger('click');
            $curboxMask.hide();
        });

        /***** confirm's modal btnEvent *****/
        //确认按钮
        $('.js-modal-device .js-btnEnsure1').on('click', function () {
            let curFlag = $(this).closest('.mod-popupBox').children('.js-modalTitle').text();
            let deviceId = pub.getDeviceId();
            switch (curFlag) {
                case '启动设备':
                    pub.sendCtrlDirect('start_on', deviceId);
                    break;
                case '关闭设备':
                    pub.sendCtrlDirect('shut_down', deviceId);
                    break;
                case '重启设备':
                    pub.sendCtrlDirect('do_reboot', deviceId);
                    break;
                case '启用设备':
                    pub.sendCtrlDirect('valid', deviceId);
                    break;
                case '停用设备':
                    pub.sendCtrlDirect('invalid', deviceId);
                    break;
            }
            $cfBoxMask.hide();
        });
        //取消
        $('.js-modal-device .js-btnCancel1').on('click', function () {
            $cfResetBtn.trigger('click');
            $cfBoxMask.hide();
        });
        //关闭按钮
        $('.js-modal-device .js-btnClose1').on('click', function () {
            $cfResetBtn.trigger('click');
            $cfBoxMask.hide();
        });
    };

    //layuiInit
    let layuiInit = function () {
        //使用layui's form
        let form = layui.form();
        form.render();
        //监听设备组选择框
        form.on('select(deviceGroupSelect)', function (data) {
            if (data.elem.value === 'all') {
                pub.tableConDataInit();
            } else {
                //请求单个设备组的设备列表
                pub.tableConDataInit({
                    isPrimary_select:1,
                    displayGroup_name: data.elem.value
                });
            }
            pub.backToTabDefault();
        });
    }

    let render = function () {
        //获取弹框元素
        getElem();
        //首次加载页面获取数据
        pub.tableConDataInit();
        //获取设备组列表
        pub.getModalData('/DisplayInfoes/SelectGroupInfor', 'deviceGroSel-tmpl', 'deviceGroupSelect');
        //获取数据之后绑定事件
        
        tableConEvent();
        menuEvent();
        modalBtnEvent();
        layuiInit();
    };

    return {
        render: render
    };

})(jQuery, util, layui, layer, doT);