/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};

/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {

/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId])
/******/ 			return installedModules[moduleId].exports;

/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};

/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);

/******/ 		// Flag the module as loaded
/******/ 		module.l = true;

/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}


/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;

/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;

/******/ 	// identity function for calling harmony imports with the correct context
/******/ 	__webpack_require__.i = function(value) { return value; };

/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, {
/******/ 				configurable: false,
/******/ 				enumerable: true,
/******/ 				get: getter
/******/ 			});
/******/ 		}
/******/ 	};

/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};

/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };

/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";

/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 6);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony default export */ __webpack_exports__["a"] = (function deviceManage($, util, layui, layer, doT) {
    var $boxMask = null,
        $cfBoxMask = null;
    var scrollCount = null;
    //初始化layui表单
    var form = layui.form();

    var getElem = function getElem() {
        $boxMask = $('.js-modal-edit .js-boxMask'), $cfBoxMask = $('.confirm .mod-boxMask');
    };
    //公共方法
    var pub = {
        timer: null,
        getDeviceName: function getDeviceName() {
            var arrCon = [];
            $('.js-tableCon').find('.checkbox.checkbox-checked').parent().next().each(function (index, item) {
                var cur = item.innerHTML;
                arrCon.push(cur);
            });
            return arrCon.join(', ');
        },
        //获取设备ID
        getDeviceId: function getDeviceId() {
            var curIds = [];
            $('.js-tableCon').find('.checkbox.checkbox-checked').closest('.row').each(function (i, item) {
                var cur = item.getAttribute('data-deviceid');
                curIds.push(cur);
            });
            return curIds.join(',');
        },
        //发送控制命令
        sendCtrlDirect: function sendCtrlDirect(directName, deviceIds) {
            util.sendPostReq('/DisplayInfoes/ControlMethod', {
                "Message": directName,
                "Data": deviceIds
            }).then(function (data) {
                layer.msg(data.Message);
                pub.onStartChangeCon();
            }).fail(function (err) {
                layer.msg('请求发送错误');
            });
        },
        //首次加载->获取列表
        tableConDataInit: function tableConDataInit(obj) {
            obj = obj || { isPrimary_select: 1 };

            // get Device list
            var loadTips = layer.load(2, {
                shade: 0.02
            });
            util.sendPostReq('/DisplayInfoes/SelectAllRecord', obj).then(function (data) {
                if (data.length > 0) {
                    // console.log(JSON.parse(data));  
                    var res = doT.template($('#device-list-tmpl').html())(data);
                    $('#device-list').html(res);

                    //必须先销毁上次的scroll实例，再初始化
                    scrollCount ? util.destroyScroll(scrollCount) : null;
                    scrollCount = util.myScroll('.js-tableCon');
                } else {
                    $('#device-list').html('');
                }
            }).fail(function (err) {
                layer.msg('请求发送错误');
            }).always(function () {
                layer.close(loadTips);
            });
        },
        //获取选中行数据，只能获取单行
        getDeviceAttr: function getDeviceAttr() {
            var $deviceList = $('#device-list').children('.is-active');
            var $deviceListChilds = $deviceList.children();
            var obj = {};
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
        bindDataToModal: function bindDataToModal(data) {
            clearTimeout(pub.timer);
            var $modalDevice = $('.js-modal-device');
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
                $modalDevice.find('select[name="deviceGroup"] option[value="' + data.displayGroup + '"]').prop('selected', 'selected');
                form.render();
            }, 0);

            //启动类型 1为自动  0为手动
            data.bootType === '1' ? $('#startAuto').prop('checked', 'checked') : $('#startHand').prop('checked', 'checked');

            form.render();
        },
        //发送表单数据
        bindLayuiEvent: function bindLayuiEvent() {
            var obj = {};
            //获取下拉选择框的value
            form.on('select(modal-device-sel)', function (data) {
                if (data.elem.name === 'deviceGroup') {
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
                if (formObj.deviceGroup === 'all' || !formObj.deviceGroup) {
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
                var isEmptyObject = $.isEmptyObject(obj);
                if (!isEmptyObject) {
                    var loadTips = layer.load(2, {
                        shade: 0.02
                    });
                    //发送请求
                    util.sendPostReq('/DisplayInfoes/UpdateOneRecord', obj).then(function (data) {
                        layer.msg(data.Message + ' ' + data.Data);
                        $boxMask.hide();
                        $('.js-reset').trigger('click');
                        //发送成功之后刷新页面，因为发送命令之后设备返回反馈信息需要一点时间，所以用定时器
                        pub.timer = setTimeout(function () {
                            //根据设备选择框的值来判定当前刷新的数据列表
                            pub.deviceSelStateChangeCon();
                        }, 2000);
                    }).fail(function (err) {
                        layer.msg(err);
                    }).always(function () {
                        layer.close(loadTips);
                    });
                }
                return false;
            });
        },
        //清除列表选中样式
        clearChellAllEffect: function clearChellAllEffect() {
            var $tableCon = $('.js-equipTable .js-tableCon');
            //清除复选框选中样式
            $tableCon.find('.checkbox-checked').removeClass('checkbox-checked');
            //清除全选按钮样式
            $('.js-allEquip').removeClass('checkbox-checked');
            //清除行选中效果
            $tableCon.find('.is-active').removeClass('is-active');
        },
        //清除选项卡选中效果
        backToTabDefault: function backToTabDefault() {
            $('.js-tabCheck').children().eq(0).addClass('is-active').siblings().removeClass('is-active');
        },
        //输入框提示信息
        formTips: function formTips(selector, con) {
            var tips = void 0;
            $(selector).off();
            $(selector).on('focus', function () {
                //同时多次点击执行一次
                tips = layer.tips(con, this, { time: 0 });
            });
            $(selector).on('blur', function () {
                //同时多次点击执行一次
                layer.close(tips);
            });
        },
        //ajax获取值并绑定到弹框
        getModalData: function getModalData(url, dotTmplID, tmplID, data) {
            //loading
            var loadTips = layer.load(2, {
                shade: 0.02
            });
            //tmpl's list
            util.sendPostReq(url, data).then(function (data) {
                if (data.length > 0) {
                    var tmplVal = $('#' + dotTmplID).html();
                    var res = doT.template(tmplVal)(data);
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
        getDeviceGroId: function getDeviceGroId() {
            var cur = $('#deviceGroupSelect').val();
            if (cur === 'all') {
                return;
            } else {
                return cur;
            }
        },
        //根据设备选择框的值和选项卡来判定当前刷新的数据列表
        onStartChangeCon: function onStartChangeCon() {
            var curState = $('#deviceGroupSelect').val();
            var curFlag = $('.js-tabCheck').children('.is-active').attr('data-flag');
            var id = ~~curState;
            switch (curFlag) {
                case 'monitor':
                    if (curState === 'all') {
                        pub.tableConDataInit({ isPrimary_select: 1 });
                    } else {
                        pub.tableConDataInit({
                            isPrimary_select: 1,
                            displayGroup_name: id
                        });
                    }
                    break;
                case 'valid':
                    if (curState === 'all') {
                        pub.tableConDataInit({ isPrimary_select: 1 });
                    } else {
                        pub.tableConDataInit({
                            isPrimary_select: 1,
                            displayGroup_name: id
                        });
                    }
                    break;
                case 'invalid':
                    if (curState === 'all') {
                        pub.tableConDataInit({ isPrimary_select: 0 });
                    } else {
                        pub.tableConDataInit({
                            isPrimary_select: 0,
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
        changeCfModalCon: function changeCfModalCon(menuCon, str) {
            $('.js-modal-device .js-modalTitle').html(menuCon);
            $('.confirm .js-modalCon').html('是否' + menuCon + ':' + str);
            $cfBoxMask.show();
        }
    };

    //设备列表事件
    var tableConEvent = function tableConEvent() {
        var $tableCon = $('.js-equipTable .js-tableCon');
        var $menuDisplay = $('.js-menuDisplay');
        var $menuDisable = $('.menu-disable');
        //监控设备、有效设备和无效设备选项卡
        $('.js-tabCheck').on('click', 'div', function (e) {
            var $this = $(this);
            $this.addClass('is-active').siblings().removeClass('is-active');
            var curFlag = $this.attr('data-flag');
            //清除上一选项卡页面的选中效果
            pub.clearChellAllEffect();
            var deviceGroId = pub.getDeviceGroId();
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
            var $this = $(this),
                $row = $this.closest('.row');
            var flag = $this.hasClass('checkbox-checked');
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
    var menuEvent = function menuEvent() {
        var $modalTitle = $('.js-modal-device .js-modalTitle');
        $('.js-menu').on('click', function (e) {
            var tar = e.target,
                menuCon = $(tar).text(),
                str = void 0;
            //获取设备列表被选中的设备名称
            str = pub.getDeviceName();

            var checkLength = $('.js-tableCon').find('.checkbox.checkbox-checked').length;
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
                        var deviceData = pub.getDeviceAttr();
                        //绑定到弹框表单
                        pub.bindDataToModal(deviceData);
                        $boxMask.show();
                    }
                    break;
                case 'menu-boot':
                    if (checkLength === 0) {
                        layer.alert('未选中设备');
                    } else {
                        pub.changeCfModalCon(menuCon, str);
                    }
                    break;
                case 'menu-stop':
                    if (checkLength === 0) {
                        layer.alert('未选中设备');
                    } else {
                        pub.changeCfModalCon(menuCon, str);
                    }
                    break;
                case 'menu-reboot':
                    if (checkLength === 0) {
                        layer.alert('未选中设备');
                    } else {
                        pub.changeCfModalCon(menuCon, str);
                    }
                    break;
                case 'menu-enabled':
                    if (checkLength === 0) {
                        layer.alert('未选中设备');
                    } else {
                        pub.changeCfModalCon(menuCon, str);
                    }

                    break;
                case 'menu-disable':
                    if (checkLength === 0) {
                        layer.alert('未选中设备');
                    } else {
                        pub.changeCfModalCon(menuCon, str);
                    }
                    break;
            }
        });
    };

    //弹框中的事件
    var modalBtnEvent = function modalBtnEvent() {
        var $resetBtn = $('.js-modal-device .js-reset'),
            $cfResetBtn = $('.js-modal-device .confirm .js-cfReset');
        //显示属性提示单位
        pub.formTips('#displayProp', '最大值150，最小值0，单位:(%)');
        /***** modal btnEvent *****/
        //确认按钮
        /*$('.js-modal-device .js-btnEnsure').on('click', function () {
                      
        });*/
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

        /***** confirm's modal btnEvent *****/
        //确认按钮
        $('.js-modal-device .js-btnEnsure1').on('click', function () {
            var curFlag = $(this).closest('.mod-popupBox').children('.js-modalTitle').text();
            var deviceId = pub.getDeviceId();
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
    var layuiInit = function layuiInit() {
        //使用layui's form
        var form = layui.form();
        form.render();
        //监听设备组选择框
        form.on('select(deviceGroupSelect)', function (data) {
            if (data.elem.value === 'all') {
                pub.tableConDataInit();
            } else {
                //请求单个设备组的设备列表
                pub.tableConDataInit({
                    isPrimary_select: 1,
                    displayGroup_name: data.elem.value
                });
            }
            pub.backToTabDefault();
        });
    };

    var render = function render() {
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

/***/ }),
/* 1 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony default export */ __webpack_exports__["a"] = (function flightData($, util, layer, layui, doT) {
    //四个弹框
    var $planBoxMask = null,
        //航班计划弹框
    $resBoxMask = null,
        //资源分配弹框
    $baseBoxMask = null;
    //iscroll实例->代表水平和垂直实例
    var scrollCount = null,
        scrollHCount = null;

    //layui's form初始化
    var form = layui.form();

    var getElem = function getElem() {
        $planBoxMask = $('.js-modal-fliData .js-flight-plan'), //航班计划弹框
        $resBoxMask = $('.js-modal-fliData .js-resource-allo'), //资源分配弹框
        $baseBoxMask = $('.js-modal-fliData .js-base-data'); //基础数据弹框
    };

    //公共方法
    var pub = {
        setModalCon: function setModalCon() {
            var a = [];
            $('.show .js-tableCon').find('.checkbox.checkbox-checked').parent().next().each(function (index, item) {
                var cur = item.innerHTML;
                a.push(cur);
            });
            return a.join(', ');
        },
        //绑定飞机数据
        bindPlaneData: function bindPlaneData() {
            var $planeDataList = $('#plane-data').children('.is-active');
            var $planeDataChilds = $planeDataList.children();
            var $planeData = $baseBoxMask.find('.plane-data');
            //id
            var id = $planeDataList.attr('data-id');
            $planeData.find('.js-flag-id').val(id);
            //飞机注册号
            var reg = $planeDataChilds.map(function (index, item) {
                return item.getAttribute('data-reg-no');
            })[0];
            $planeData.find('input[name="ac_reg_no"]').val(reg);
            //机型IATA代码
            var type_iata = $planeDataChilds.map(function (index, item) {
                return item.getAttribute('data-type-iata');
            })[0];
            $planeData.find('input[name="ac_type_iata"]').val(type_iata);
            //航空公司IATA代码
            var airline_iata = $planeDataChilds.map(function (index, item) {
                return item.getAttribute('data-airline-iata');
            })[0];
            $planeData.find('input[name="airline_iata"]').val(airline_iata);
            //删除标志
            var flg_del = $planeDataChilds.map(function (index, item) {
                return item.getAttribute('data-flg-del');
            })[0];
            $planeData.find('select[name="flag_del"]').find('option[value="' + flg_del + '"]').prop('selected', 'selected');
            form.render();
            //附加代码
            var ext_code = $planeDataChilds.map(function (index, item) {
                return item.getAttribute('data-ext-code');
            })[0];
            $planeData.find('input[name="ext_code"]').val(ext_code);
        },
        //机型数据
        bindModelData: function bindModelData() {
            var $curList = $('#model-data').children('.is-active');
            var $curChilds = $curList.children();
            var $modalData = $baseBoxMask.find('.model-data');
            //id
            var id = $curList.attr('data-id');
            $modalData.find('.js-model-id').val(id);
            //中文名称
            var name_cn = $curChilds.map(function (index, item) {
                return item.getAttribute('data-name-cn');
            })[0];
            $modalData.find('input[name="cn_name"]').val(name_cn);
            //英文名称
            var name_en = $curChilds.map(function (index, item) {
                return item.getAttribute('data-name-en');
            })[0];
            $modalData.find('input[name="en_name"]').val(name_en);
            //机型IATA代码
            var iata_code = $curChilds.map(function (index, item) {
                return item.getAttribute('data-iata-code');
            })[0];
            $modalData.find('input[name="ac_type_iata"]').val(iata_code);
            //机型IACO代码
            var icao_code = $curChilds.map(function (index, item) {
                return item.getAttribute('data-icao-code');
            })[0];
            $modalData.find('input[name="ac_type_iaco"]').val(icao_code);
        },
        //航空公司
        bindAirlineData: function bindAirlineData() {
            var $curList = $('#airline-data').children('.is-active');
            var $curChilds = $curList.children();
            var $modalData = $baseBoxMask.find('.airline');

            //id
            var id = $curList.attr('data-id');
            $modalData.find('.js-airline-id').val(id);
            //简称
            var short_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-short-name');
            })[0];
            $modalData.find('input[name="short_name"]').val(short_name);
            //中文名称
            var cn_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            var en_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //国内国际属性
            var dori = $curChilds.map(function (index, item) {
                return item.getAttribute('data-dori');
            })[0];
            $modalData.find('select[name="dori"] option[value="' + dori + '"]').prop('selected', 'selected');
            form.render();
            //航空公司IATA代码
            var airline_iata = $curChilds.map(function (index, item) {
                return item.getAttribute('data-airline-iata');
            })[0];
            $modalData.find('input[name="airline_iata"]').val(airline_iata);
            //航空公司IACO代码
            var airline_icao = $curChilds.map(function (index, item) {
                return item.getAttribute('data-airline-icao');
            })[0];
            $modalData.find('input[name="airline_icao"]').val(airline_icao);
            //基地机场IACO代码
            var airport_iata = $curChilds.map(function (index, item) {
                return item.getAttribute('data-airport-iata');
            })[0];
            $modalData.find('input[name="airport_iata"]').val(airport_iata);
            //航空公司联盟代码
            var alliance = $curChilds.map(function (index, item) {
                return item.getAttribute('data-alliance-code');
            })[0];
            $modalData.find('input[name="alliance_code"]').val(alliance);
        },
        //航空联盟
        bindAllianceData: function bindAllianceData() {
            var $curList = $('#air-alliance').children('.is-active');
            var $curChilds = $curList.children();
            var $modalData = $baseBoxMask.find('.air-alliance');

            //id
            var id = $curList.attr('data-id');
            $modalData.find('.js-alliance-id').val(id);

            //联盟代码
            var all_code = $curChilds.map(function (index, item) {
                return item.getAttribute('data-alliance-name');
            })[0];
            $modalData.find('input[name="alliance_code"]').val(all_code);
            //中文名称
            var cn_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            var en_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //备注
            var remark = $curChilds.map(function (index, item) {
                return item.getAttribute('data-remark');
            })[0];
            $modalData.find('input[name="remark"]').val(remark);
        },
        //机场
        bindAirportData: function bindAirportData() {
            var $curList = $('#airport-data').children('.is-active');
            var $curChilds = $curList.children();
            var $modalData = $baseBoxMask.find('.airport');

            //id
            var id = $curList.attr('data-id');
            $modalData.find('.js-airport-id').val(id);

            //简称
            var short_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-short-name');
            })[0];
            $modalData.find('input[name="short_name"]').val(short_name);
            //中文名称
            var cn_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            var en_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //国内国际属性
            var dori = $curChilds.map(function (index, item) {
                return item.getAttribute('data-dori');
            })[0];
            $modalData.find('select[name="dori"] option[value="' + dori + '"]').prop('selected', 'selected');
            form.render();
            //机场IATA代码
            var airport_iata = $curChilds.map(function (index, item) {
                return item.getAttribute('data-airport-iata');
            })[0];
            $modalData.find('input[name="airport_iata"]').val(airport_iata);
            //机场ICAO代码
            var airport_icao = $curChilds.map(function (index, item) {
                return item.getAttribute('data-airport-icao');
            })[0];
            $modalData.find('input[name="airport_icao"]').val(airport_icao);
            //城市IATA代码
            var city_iata = $curChilds.map(function (index, item) {
                return item.getAttribute('data-city-iata');
            })[0];
            $modalData.find('input[name="city_iata"]').val(city_iata);
            //区域代码
            var area_code = $curChilds.map(function (index, item) {
                return item.getAttribute('data-area-code');
            })[0];
            $modalData.find('input[name="area_code"]').val(area_code);
        },
        //城市
        bindCityData: function bindCityData() {
            var $curList = $('#city-data').children('.is-active');
            var $curChilds = $curList.children();
            var $modalData = $baseBoxMask.find('.city');
            //id
            var id = $curList.attr('data-id');
            $modalData.find('.js-city-id').val(id);
            //简称
            var short_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-short-name');
            })[0];
            $modalData.find('input[name="short_name"]').val(short_name);
            //中文名称
            var cn_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            var en_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //所属省份标志
            var province_flag = $curChilds.map(function (index, item) {
                return item.getAttribute('data-province');
            })[0];
            $modalData.find('input[name="province_flag"]').val(province_flag);
            //城市IATA代码
            var city_iata = $curChilds.map(function (index, item) {
                return item.getAttribute('data-city-iata');
            })[0];
            $modalData.find('input[name="city_iata"]').val(city_iata);
            //城市ICAO代码
            var city_icao = $curChilds.map(function (index, item) {
                return item.getAttribute('data-city-icao');
            })[0];
            $modalData.find('input[name="city_icao"]').val(city_icao);
            //国际IATA代码
            var country_iata = $curChilds.map(function (index, item) {
                return item.getAttribute('data-country-iata');
            })[0];
            $modalData.find('input[name="con_iata"]').val(country_iata);
        },
        //省份
        bindProvinceData: function bindProvinceData() {
            var $curList = $('#province-data').children('.is-active');
            var $curChilds = $curList.children();
            var $modalData = $baseBoxMask.find('.province');
            //id
            var id = $curList.attr('data-id');
            $modalData.find('.js-province-id').val(id);
            //简称
            var short_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-short-name');
            })[0];
            $modalData.find('input[name="short_name"]').val(short_name);
            //中文名称
            var cn_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            var en_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //国内国际属性
            var dori = $curChilds.map(function (index, item) {
                return item.getAttribute('data-dori');
            })[0];
            $modalData.find('select[name="dori"] option[value="' + dori + '"]').prop('selected', 'selected');
            form.render();
        },
        //国家
        bindCountryData: function bindCountryData() {
            var $curList = $('#country-data').children('.is-active');
            var $curChilds = $curList.children();
            var $modalData = $baseBoxMask.find('.country');
            //id
            var id = $curList.attr('data-id');
            $modalData.find('.js-country-id').val(id);
            //中文名称
            var cn_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            var en_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //国家IATA代码
            var country_iata = $curChilds.map(function (index, item) {
                return item.getAttribute('data-country-iata');
            })[0];
            $modalData.find('input[name="country_iata"]').val(country_iata);
            //国家ICAO代码
            var country_icao = $curChilds.map(function (index, item) {
                return item.getAttribute('data-country-icao');
            })[0];
            $modalData.find('input[name="country_icao"]').val(country_icao);
        },
        //任务代码
        bindTaskCodeData: function bindTaskCodeData() {
            var $curList = $('#task-code').children('.is-active');
            var $curChilds = $curList.children();
            var $modalData = $baseBoxMask.find('.task-code');
            //id
            var id = $curList.attr('data-id');
            $modalData.find('.js-task-id').val(id);
            //中文名称
            var cn_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            var en_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //任务代码
            var task_code = $curChilds.map(function (index, item) {
                return item.getAttribute('data-task-code');
            })[0];
            $modalData.find('input[name="task_code"]').val(task_code);
            //描述
            var desc = $curChilds.map(function (index, item) {
                return item.getAttribute('data-desc');
            })[0];
            $modalData.find('input[name="desc"]').val(desc);
        },
        //延误代码
        bindDelayCodeData: function bindDelayCodeData() {
            var $curList = $('#delay-code').children('.is-active');
            var $curChilds = $curList.children();
            var $modalData = $baseBoxMask.find('.delay-code');
            //id
            var id = $curList.attr('data-id');
            $modalData.find('.js-delay-id').val(id);
            //中文名称
            var cn_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-cn-name');
            })[0];
            $modalData.find('input[name="cn_name"]').val(cn_name);
            //英文名称
            var en_name = $curChilds.map(function (index, item) {
                return item.getAttribute('data-en-name');
            })[0];
            $modalData.find('input[name="en_name"]').val(en_name);
            //延误代码
            var delay_code = $curChilds.map(function (index, item) {
                return item.getAttribute('data-delay-code');
            })[0];
            $modalData.find('input[name="delay_code"]').val(delay_code);
            //延误类型
            var delay_type = $curChilds.map(function (index, item) {
                return item.getAttribute('data-delay-type');
            })[0];
            $modalData.find('input[name="delay_type"]').val(delay_type);
            //描述
            var desc = $curChilds.map(function (index, item) {
                return item.getAttribute('data-desc');
            })[0];
            $modalData.find('input[name="desc"]').val(desc);
        },
        /**离港航班->绑定单条数据到弹框
         * @param id Number类型 单条数据的FDID
         */
        bindDeparFlightData: function bindDeparFlightData(id) {
            util.sendPostReq('/FlightDataApi/GetFlightData', { FDID: id }).then(function (data) {
                if (data.Status) {
                    bindData(data.Data);
                } else {
                    layer.msg(data.Message);
                }
            }).fail(function () {
                layer.msg('获取单条数据失败');
            });

            function bindData(data) {
                //FDID
                $planBoxMask.find('.js-fdid').val(id);
                //运营日
                if (data.OPERATION_DATE) {
                    var oper_day = new Date(Number(/(\d+)/.exec(data.OPERATION_DATE)[1])).toISOString().formatTime('{0}-{1}-{2}');
                    $('#oper-day-dep').val(oper_day);
                }
                //航班号
                $('#flight-num-dep').val(data.FLIGHT_NO);
                //国内国际标识
                $('#dori-dep').find('option[value="' + data.DORI + '"]').prop('selected', "selected");
                //任务代码
                $('#task-code-d').find('option[value="' + data.TASK_CODE + '"]').prop('selected', "selected");
                //航站楼
                $('#term-floor-dep').val(data.TERMINAL_NO);
                //航空公司
                $('#airline-dep').val(data.AIRLINE_IATA);
                //机号
                $('#plane-num-dep').val(data.AC_REG_NO);
                //机型
                $('#plane-type-dep').val(data.AIRCRAFT_TYPE_IATA);
                //贵客标识
                $('#flag-vip-dep').find('option[value="' + data.FLG_VIP + '"]').prop('selected', "selected");
                //本场起飞
                $('#take-off-dep').val(data.ORIGIN_AIRPORT_IATA);
                //后场到达
                $('#arrive-dep').val(data.DEST_AIRPORT_IATA);
                //计划本场离港时间
                if (data.STD) {
                    var std = new Date(Number(/(\d+)/.exec(data.STD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#std-dep').val(std);
                }

                //计划后场到港时间
                if (data.STA) {
                    var sta = new Date(Number(/(\d+)/.exec(data.STA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#sta-dep').val(sta);
                }

                //预计离港时间
                if (data.ETD) {
                    var etd = new Date(Number(/(\d+)/.exec(data.ETD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#etd-dep').val(etd);
                }
                //预计到岗时间
                if (data.ETA) {
                    var eta = new Date(Number(/(\d+)/.exec(data.ETA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#eta-dep').val(eta);
                }
                //实际离港时间
                if (data.ATD) {
                    var atd = new Date(Number(/(\d+)/.exec(data.ATD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#atd-dep').val(atd);
                }
                //实际到港时间
                if (data.ATA) {
                    var ata = new Date(Number(/(\d+)/.exec(data.ATA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#ata-dep').val(ata);
                }

                //航线起始
                $('#airline-start-dep').val(data.AIRPORT1);
                //航显结束
                $('#airline-end-dep').val(data.AIRPORT4);
                //经停1
                $('#airline-1-dep').val(data.AIRPORT2);
                //经停2
                $('#airline-2-dep').val(data.AIRPORT3);
                //共享主航班号
                $('#share-main-dep').val(data.CODE_SHARE1);
                //共享航班号
                $('#share-2-dep').val(data.CODE_SHARE2);
                //链接航班
                //$('#link-flight').val();
                //链接运营日
                //$('#link-day').val();
                //合并航班号
                $('#merge-dep').val(data.CODE_SHARE3);
                //被合并航班号
                $('#be-merge-dep').val(data.CODE_SHARE4);

                form.render();
            }
        },
        /**到港航班->绑定单条数据到弹框
         * @param id Number类型 单条数据的FDID
         */
        bindArvFlightData: function bindArvFlightData(id) {
            util.sendPostReq('/FlightDataApi/GetFlightData', { FDID: id }).then(function (data) {
                if (data.Status) {
                    bindData(data.Data);
                } else {
                    layer.msg(data.Message);
                }
            }).fail(function () {
                layer.msg('获取单条数据失败');
            });
            function bindData(data) {
                //FDID
                $planBoxMask.find('.js-fdid').val(id);
                //运营日
                if (data.OPERATION_DATE) {
                    var oper_day = new Date(Number(/(\d+)/.exec(data.OPERATION_DATE)[1])).toISOString().formatTime('{0}-{1}-{2}');
                    $('#oper-day-arr').val(oper_day);
                }
                //航班号
                $('#flight-num-arr').val(data.FLIGHT_NO);
                //国内国际标识
                $('#dori-arr').find('option[value="' + data.DORI + '"]').prop('selected', "selected");
                //任务代码
                $('#task-code-a').find('option[value="' + data.TASK_CODE + '"]').prop('selected', "selected");
                //航站楼
                $('#term-floor-arr').val(data.TERMINAL_NO);
                //航空公司
                $('#airline-arr').val(data.AIRLINE_IATA);
                //机号
                $('#plane-num-arr').val(data.AC_REG_NO);
                //机型
                $('#plane-type-arr').val(data.AIRCRAFT_TYPE_IATA);
                //贵客标识
                $('#flag-vip-arr').find('option[value="' + data.FLG_VIP + '"]').prop('selected', "selected");
                //本场起飞
                $('#take-off-arr').val(data.ORIGIN_AIRPORT_IATA);
                //后场到达
                $('#arrive-arr').val(data.DEST_AIRPORT_IATA);
                //计划本场离港时间
                if (data.STD) {
                    var std = new Date(Number(/(\d+)/.exec(data.STD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#std-arr').val(std);
                }

                //计划后场到港时间
                if (data.STA) {
                    var sta = new Date(Number(/(\d+)/.exec(data.STA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#sta-arr').val(sta);
                }

                //预计离港时间
                if (data.ETD) {
                    var etd = new Date(Number(/(\d+)/.exec(data.ETD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#etd-arr').val(etd);
                }
                //预计到岗时间
                if (data.ETA) {
                    var eta = new Date(Number(/(\d+)/.exec(data.ETA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#eta-arr').val(eta);
                }
                //实际离港时间
                if (data.ATD) {
                    var atd = new Date(Number(/(\d+)/.exec(data.ATD)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#atd-arr').val(atd);
                }
                //实际到港时间
                if (data.ATA) {
                    var ata = new Date(Number(/(\d+)/.exec(data.ATA)[1])).toISOString().formatTime('{0}-{1}-{2} {3}:{4}:{5}');
                    $('#ata-arr').val(ata);
                }

                //航线起始
                $('#airline-start-arr').val(data.AIRPORT1);
                //航显结束
                $('#airline-end-arr').val(data.AIRPORT4);
                //经停1
                $('#airline-1-arr').val(data.AIRPORT2);
                //经停2
                $('#airline-2-arr').val(data.AIRPORT3);
                //共享主航班号
                $('#share-main-arr').val(data.CODE_SHARE1);
                //共享航班号
                $('#share-2-arr').val(data.CODE_SHARE2);
                //链接航班
                //$('#link-flight').val();
                //链接运营日
                //$('#link-day').val();
                //合并航班号
                $('#merge-arr').val(data.CODE_SHARE3);
                //被合并航班号
                $('#be-merge-arr').val(data.CODE_SHARE4);

                form.render();
            }
        },
        /** 分页 
         *  @param option Object类型
         *  option.pageNum Number类型 总页数
         *  option.url String类型 请求链接
         *  option.pageSize Number类型 一页显示多少条
         *  option.tmplId  String类型 模版ID
         *  option.targetId String类型 目标ID
         */
        laypageEvent: function laypageEvent(option) {
            layui.use(['laypage'], function () {
                var laypage = layui.laypage;
                laypage({
                    cont: 'log-view-paging',
                    pages: option.pageNum,
                    groups: 4,
                    skin: '#e6e6e6',
                    last: '尾页',
                    prev: '<em><i class="icon-prev"></i></em>',
                    next: '<em><i class="icon-next"></i></em>',
                    jump: function jump(obj, first) {
                        if (!first) {
                            var loadTips = layer.load(2, {
                                shade: 0.02
                            });
                            //根据页数obj.curr发送对应的请求
                            util.sendPostReq(option.url, {
                                startRow: (obj.curr - 1) * option.pageSize,
                                pageSize: option.pageSize
                            }).then(function (data) {
                                if (data.Status) {
                                    var res = doT.template($('#' + option.tmplId).html())(data.Data.List);
                                    $('#' + option.targetId).html(res);
                                } else {
                                    layer.msg(data.Message);
                                }
                            }).then(function () {
                                //内容列表添加局部滚动
                                pub.removeScrollbar();
                                scrollHCount = util.myHScroll('.js-tabCtrl-con');
                                scrollCount = util.myScroll('.js-tableCon');
                            }).always(function () {
                                layer.close(loadTips);
                            });
                        }
                    }
                });
            });
        },
        //切换弹框类型
        changeModalType: function changeModalType(attr) {
            switch (attr) {
                //航班计划弹框
                case 'flight-plan':
                    $planBoxMask.show();
                    break;
                //资源分配弹框
                case 'res-allo':
                    $resBoxMask.show();
                    break;
                //基础数据弹框
                case 'base-data':
                    $baseBoxMask.show();
                    break;
            }
        },

        /** 航班数据->改变弹框内容
         * @param attr String类型 根据自定义属性的值来切换不同的弹框内容
         * @param id String类型 点击编辑按钮，给对应的弹框加上对应的行id
         */
        changeModalCon: function changeModalCon(attr, id) {
            id = id || '';
            switch (attr) {
                //航班计划
                case 'depart':
                    $planBoxMask.find('.depart-form').addClass('show').siblings('.arrive-form').removeClass('show').addClass('hide');
                    //如果id存在，则是编辑->获取单条航班数据
                    id ? pub.bindDeparFlightData(id) : null;
                    break;
                case 'arrive':
                    $planBoxMask.find('.arrive-form').addClass('show').siblings('.depart-form').removeClass('show').addClass('hide');
                    id ? pub.bindArvFlightData(id) : null;
                    break;
                //资源分配
                case 'board-gate':
                    $resBoxMask.find('.board-gate-res').addClass('show').siblings('.board-check-bag').removeClass('show').addClass('hide');
                    break;
                case 'check-in':
                    $resBoxMask.find('.check-in-res').addClass('show').siblings('.board-check-bag').removeClass('show').addClass('hide');
                    break;
                case 'baggage':
                    $resBoxMask.find('.baggage-res').addClass('show').siblings('.board-check-bag').removeClass('show').addClass('hide');
                    break;
                //基础数据
                case 'plane-data':
                    $baseBoxMask.find('.plane-data').addClass('show').siblings().removeClass('show').addClass('hide');
                    //id有值则是编辑，把选中行的数据绑定到弹框
                    id ? pub.bindPlaneData() : null;
                    break;
                case 'model-data':
                    $baseBoxMask.find('.model-data').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindModelData() : null;
                    break;
                case 'airline':
                    $baseBoxMask.find('.airline').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindAirlineData() : null;
                    break;
                case 'air-alliance':
                    $baseBoxMask.find('.air-alliance').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindAllianceData() : null;
                    break;
                case 'airport':
                    $baseBoxMask.find('.airport').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindAirportData() : null;
                    break;
                case 'city':
                    $baseBoxMask.find('.city').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindCityData() : null;
                    break;
                case 'province':
                    $baseBoxMask.find('.province').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindProvinceData() : null;
                    break;
                case 'country':
                    $baseBoxMask.find('.country').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindCountryData() : null;
                    break;
                case 'task-code':
                    $baseBoxMask.find('.task-code').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindTaskCodeData() : null;
                    break;
                case 'delay-code':
                    $baseBoxMask.find('.delay-code').addClass('show').siblings().removeClass('show').addClass('hide');
                    id ? pub.bindDelayCodeData() : null;
                    break;
            }
        },

        //删除横向竖向滚动条
        removeScrollbar: function removeScrollbar() {
            $('.iScrollHorizontalScrollbar.iScrollLoneScrollbar').remove();
            $('.iScrollVerticalScrollbar.iScrollLoneScrollbar').remove();
        },
        //次日航班、资源分配等切换一级切换
        dataTabsChangeCon: function dataTabsChangeCon(elem) {
            var $changeMenuCon = $('.js-changeMenuCon'),
                $menuDisplay = $('.js-menuDisplay'),
                curAttr = $(elem).attr('data-flag');

            switch (curAttr) {
                case 'morrow-plan':
                    $menuDisplay.removeClass('menuHide');
                    $changeMenuCon.html('发布数据');
                    //获取初始数据
                    pub.tableConDataInit({
                        reqData: {
                            aord: 'D',
                            planType: 0
                        },
                        tmplId: 'flight-plan-tmpl',
                        elemId: 'flight-plan-d'
                    });
                    break;
                case 'today-plan':
                    $changeMenuCon.html('次日数据转当日数据');
                    $menuDisplay.addClass('menuHide');
                    pub.tableConDataInit({
                        reqData: {
                            aord: 'D',
                            planType: 1
                        },
                        tmplId: 'flight-plan-tmpl',
                        elemId: 'flight-plan-td'
                    });
                    break;
                case 'morrow-res':
                    $changeMenuCon.html('发布数据');
                    $menuDisplay.addClass('menuHide');
                    break;
                case 'today-res':
                    $changeMenuCon.html('次日资源转当日资源');
                    $menuDisplay.addClass('menuHide');
                    break;
                case 'base-data':
                    $changeMenuCon.html('发布数据');
                    $menuDisplay.addClass('menuHide');
                    pub.tableConDataInit({
                        url: '/FlightDataApi/GetAirCraftData',
                        tmplId: 'plane-data-tmpl',
                        elemId: 'plane-data'
                    });
                    break;
            }
        },
        //离港航班、进港航班、登机口资源等二级切换
        ctrlTabChangeCon: function ctrlTabChangeCon(elem) {
            var curAttr = $(elem).attr('data-menu');
            //一级切换元素的flag属性
            var curFlag = $('.js-dataTabs').children().eq(0).children('.is-active').attr('data-flag');
            switch (curAttr) {
                case 'depart':
                    if (curFlag === 'morrow-plan') {
                        pub.tableConDataInit({
                            reqData: {
                                aord: 'D',
                                planType: 0
                            },
                            tmplId: 'flight-plan-tmpl',
                            elemId: 'flight-plan-d'
                        });
                    }
                    if (curFlag === 'today-plan') {
                        pub.tableConDataInit({
                            reqData: {
                                aord: 'D',
                                planType: 1
                            },
                            tmplId: 'flight-plan-tmpl',
                            elemId: 'flight-plan-td'
                        });
                    }
                    break;
                case 'arrive':
                    if (curFlag === 'morrow-plan') {
                        pub.tableConDataInit({
                            reqData: {
                                aord: 'A',
                                planType: 0
                            },
                            tmplId: 'flight-plan-tmpl',
                            elemId: 'flight-plan-a'
                        });
                    }
                    if (curFlag === 'today-plan') {
                        pub.tableConDataInit({
                            reqData: {
                                aord: 'A',
                                planType: 1
                            },
                            tmplId: 'flight-plan-tmpl',
                            elemId: 'flight-plan-ta'
                        });
                    }
                    break;
                case 'plane-data':
                    pub.tableConDataInit({
                        url: '/FlightDataApi/GetAirCraftData',
                        tmplId: 'plane-data-tmpl',
                        elemId: 'plane-data'
                    });
                    break;
                case 'model-data':
                    pub.tableConDataInit({
                        url: '/FlightDataApi/GetAirCraftType',
                        tmplId: 'model-data-tmpl',
                        elemId: 'model-data'
                    });
                    break;
                case 'airline':
                    pub.tableConDataInit({
                        url: '/FlightDataApi/GetAirLineData',
                        tmplId: 'airline-data-tmpl',
                        elemId: 'airline-data'
                    });
                    break;
                case 'air-alliance':
                    pub.tableConDataInit({
                        url: '/FlightDataApi/GetAllianceData',
                        tmplId: 'air-alliance-tmpl',
                        elemId: 'air-alliance'
                    });
                    break;
                case 'airport':
                    pub.tableConDataInit({
                        url: '/FlightDataApi/GetAirportData',
                        tmplId: 'airport-data-tmpl',
                        elemId: 'airport-data'
                    });
                    break;
                case 'city':
                    pub.tableConDataInit({
                        url: '/FlightDataApi/GetCity',
                        tmplId: 'city-data-tmpl',
                        elemId: 'city-data'
                    });
                    break;
                case 'province':
                    pub.tableConDataInit({
                        url: '/FlightDataApi/GetProvince',
                        tmplId: 'province-data-tmpl',
                        elemId: 'province-data'
                    });
                    break;
                case 'country':
                    pub.tableConDataInit({
                        url: '/FlightDataApi/GetCountry',
                        tmplId: 'country-data-tmpl',
                        elemId: 'country-data'
                    });
                    break;
                case 'task-code':
                    pub.tableConDataInit({
                        url: '/FlightDataApi/GetTaskCode',
                        tmplId: 'task-code-tmpl',
                        elemId: 'task-code'
                    });
                    break;
                case 'delay-code':
                    pub.tableConDataInit({
                        url: '/FlightDataApi/GetDelayCode',
                        tmplId: 'delay-code-tmpl',
                        elemId: 'delay-code'
                    });
                    break;
            }
        },
        sendFlightDataFormVal: function sendFlightDataFormVal() {
            form.on('submit(flight-data)', function (data) {
                var formObj = data.field;
                var obj = {};
                //如果fdid有值，则是编辑，不是添加
                if (formObj.fdid) {
                    obj.FDID = ~~formObj.fdid;
                }
                //运营日
                obj.OPERATION_DATE = formObj.oper_day;
                //航班号
                obj.FLIGHT_NO = formObj.flight_num;
                //到离港标识
                if ($(this).closest('.layui-form').hasClass('depart-form')) {
                    obj.AORD = 'D';
                }
                if ($(this).closest('.layui-form').hasClass('arrive-form')) {
                    obj.AORD = 'A';
                }
                //国内国际标识
                obj.DORI = formObj.dori;
                //任务代码
                obj.TASK_CODE = formObj.task_code;
                //航站楼
                obj.TERMINAL_NO = ~~formObj.term_floor;
                //航空公司
                obj.AIRLINE_IATA = formObj.airline;
                //机号
                obj.AC_REG_NO = formObj.plane_num;
                //机型
                obj.AIRCRAFT_TYPE_IATA = formObj.plane_type;
                //贵宾标识
                obj.FLG_VIP = formObj.flg_vip;
                //起飞
                obj.ORIGIN_AIRPORT_IATA = formObj.take_off;
                //到达
                obj.DEST_AIRPORT_IATA = formObj.arrive;
                //计划离港
                obj.STD = formObj.std;
                //预计离港
                obj.ETD = formObj.etd;
                //实际离港
                obj.ATD = formObj.atd;
                //计划到港
                obj.STA = formObj.sta;
                //预计到港
                obj.ETA = formObj.eta;
                //实际到港
                obj.ATA = formObj.ata;
                //航显起始
                obj.AIRPORT1 = formObj.airline_start;
                //航显结束
                obj.AIRPORT4 = formObj.airline_end;
                //经停1
                obj.AIRPORT2 = formObj.air_line_1;
                //经停2
                obj.AIRPORT3 = formObj.air_line_2;
                //共享主航班号
                obj.CODE_SHARE1 = formObj.share_main;
                //共享航班号
                obj.CODE_SHARE2 = formObj.share_code2;
                //合并航班号
                obj.CODE_SHARE3 = formObj.merge_flight;
                //被合并航班号
                obj.CODE_SHARE4 = formObj.be_merge_fli;

                console.log('formObj', formObj);
                console.log('obj', obj);
            });
        },
        //基础数据弹框->发送表单信息
        sendBaseDataFormVal: function sendBaseDataFormVal() {
            //submit提交
            form.on('submit(base-data)', function (data) {
                var formObj = data.field;
                //飞机数据
                if ($(this).closest('.layui-form').hasClass('plane-data')) {
                    var obj = {};
                    //如果id有值，则是编辑，不是添加
                    if (formObj.id) {
                        obj.ID = ~~formObj.id;
                    }
                    obj.AC_REG_NO = formObj.ac_reg_no;
                    obj.AC_TYPE_IATA = formObj.ac_type_iata;
                    obj.AIRLINE_IATA = formObj.airline_iata;
                    obj.FLG_DELETED = formObj.flag_del;
                    obj.EXT_CODE = formObj.ext_code;
                    // 发送数据前检测obj是否为空
                    var isEmptyObject = $.isEmptyObject(obj);
                    if (!isEmptyObject) {
                        var loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddairCraftData', obj).then(function (data) {
                            console.log('1', data);
                            if (data.Status) {
                                layer.msg(data.Message);
                                $baseBoxMask.hide();
                                $('.js-reset').trigger('click');
                                pub.tableConDataInit({
                                    url: '/FlightDataApi/GetAirCraftData',
                                    tmplId: 'plane-data-tmpl',
                                    elemId: 'plane-data'
                                });
                            } else {
                                layer.msg(data.Message);
                            }
                        }).fail(function (err) {
                            layer.msg('发送请求失败');
                        }).always(function () {
                            layer.close(loadTips);
                        });
                    }
                }
                //机型数据
                if ($(this).closest('.layui-form').hasClass('model-data')) {
                    var _obj = {};
                    //如果id有值，则是编辑，不是添加
                    if (formObj.id) {
                        _obj.ID = ~~formObj.id;
                    }
                    _obj.name_chinese = formObj.cn_name;
                    _obj.name_english = formObj.en_name;
                    _obj.iataCode = formObj.ac_type_iata;
                    _obj.icaoCode = formObj.ac_type_iaco;
                    // 发送数据前检测obj是否为空
                    var _isEmptyObject = $.isEmptyObject(_obj);
                    if (!_isEmptyObject) {
                        var _loadTips = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddairCraftType', _obj).then(function (data) {
                            if (data.Status) {
                                layer.msg(data.Message);
                                $baseBoxMask.hide();
                                $('.js-reset').trigger('click');
                                pub.tableConDataInit({
                                    url: '/FlightDataApi/GetAirCraftType',
                                    tmplId: 'model-data-tmpl',
                                    elemId: 'model-data'
                                });
                            } else {
                                layer.msg(data.Message);
                            }
                        }).fail(function (err) {
                            layer.msg('发送请求失败');
                        }).always(function () {
                            layer.close(_loadTips);
                        });
                    }
                }
                //航空公司
                if ($(this).closest('.layui-form').hasClass('airline')) {
                    var _obj2 = {};
                    //如果id有值，则是编辑，不是添加
                    if (formObj.id) {
                        _obj2.ID = ~~formObj.id;
                    }
                    _obj2.Airline_IATA = formObj.airline_iata;
                    _obj2.Airline_ICAO = formObj.airline_icao;
                    _obj2.Short_Name = formObj.short_name;
                    _obj2.Host_AirPort_IATA = formObj.airport_iata;
                    _obj2.DORI = formObj.dori;
                    _obj2.NAME_ENGLISH = formObj.en_name;
                    _obj2.NAME_CHINESE = formObj.cn_name;
                    _obj2.ALLIANCE_CODE = formObj.alliance_code;
                    // 发送数据前检测obj是否为空
                    var _isEmptyObject2 = $.isEmptyObject(_obj2);
                    if (!_isEmptyObject2) {
                        var _loadTips2 = layer.load(2, {
                            shade: 0.02
                        });

                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddairLineData', _obj2).then(function (data) {
                            if (data.Status) {
                                layer.msg(data.Message);
                                $baseBoxMask.hide();
                                $('.js-reset').trigger('click');
                                pub.tableConDataInit({
                                    url: '/FlightDataApi/GetAllianceData',
                                    tmplId: 'airline-data-tmpl',
                                    elemId: 'airline-data'
                                });
                            } else {
                                layer.msg(data.Message);
                            }
                        }).fail(function (err) {
                            layer.msg('发送请求失败');
                        }).always(function () {
                            layer.close(_loadTips2);
                        });
                    }
                }
                //航空联盟
                if ($(this).closest('.layui-form').hasClass('air-alliance')) {
                    var _obj3 = {};
                    //如果id有值，则是编辑，不是添加
                    if (formObj.id) {
                        _obj3.ID = ~~formObj.id;
                    }
                    _obj3.ALLIANCE_NAME = formObj.alliance_code;
                    _obj3.NAME_CHINESE = formObj.cn_name;
                    _obj3.NAME_ENGLISH = formObj.en_name;
                    _obj3.REMARK = formObj.remark;
                    // 发送数据前检测obj是否为空
                    var _isEmptyObject3 = $.isEmptyObject(_obj3);
                    if (!_isEmptyObject3) {
                        var _loadTips3 = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddallianceData', _obj3).then(function (data) {
                            if (data.Status) {
                                layer.msg(data.Message);
                                $baseBoxMask.hide();
                                $('.js-reset').trigger('click');
                                pub.tableConDataInit({
                                    url: '/FlightDataApi/GetAllianceData',
                                    tmplId: 'air-alliance-tmpl',
                                    elemId: 'air-alliance'
                                });
                            } else {
                                layer.msg(data.Message);
                            }
                        }).fail(function (err) {
                            layer.msg('发送请求失败');
                        }).always(function () {
                            layer.close(_loadTips3);
                        });
                    }
                }
                //机场
                if ($(this).closest('.layui-form').hasClass('airport')) {
                    var _obj4 = {};
                    //如果id有值，则是编辑，不是添加
                    if (formObj.id) {
                        _obj4.ID = ~~formObj.id;
                    }
                    _obj4.AIRPORT_IATA = formObj.airport_iata;
                    _obj4.AIRPORT_ICAO = formObj.airport_icao;
                    _obj4.SHORT_NAME = formObj.short_name;
                    _obj4.CITY_IATA = formObj.city_iata;
                    _obj4.DORI = formObj.dori;
                    _obj4.NAME_ENGLISH = formObj.en_name;
                    _obj4.NAME_CHINESE = formObj.cn_name;
                    _obj4.REGION_CODE = formObj.area_code;
                    // 发送数据前检测obj是否为空
                    var _isEmptyObject4 = $.isEmptyObject(_obj4);
                    if (!_isEmptyObject4) {
                        var _loadTips4 = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddairPortData', _obj4).then(function (data) {
                            if (data.Status) {
                                layer.msg(data.Message);
                                $baseBoxMask.hide();
                                $('.js-reset').trigger('click');
                                pub.tableConDataInit({
                                    url: '/FlightDataApi/GetAirportData',
                                    tmplId: 'airport-data-tmpl',
                                    elemId: 'airport-data'
                                });
                            } else {
                                layer.msg(data.Message);
                            }
                        }).fail(function (err) {
                            layer.msg('发送请求失败');
                        }).always(function () {
                            layer.close(_loadTips4);
                        });
                    }
                }
                //城市
                if ($(this).closest('.layui-form').hasClass('city')) {
                    var _obj5 = {};
                    //如果id有值，则是编辑，不是添加
                    if (formObj.id) {
                        _obj5.ID = ~~formObj.id;
                    }
                    _obj5.City_IATA = formObj.city_iata;
                    _obj5.Country_IATA = formObj.con_iata;
                    _obj5.City_ICAO = formObj.city_icao;
                    _obj5.Name_Chinese = formObj.cn_name;
                    _obj5.Name_English = formObj.en_name;
                    _obj5.Short_Chinese = formObj.short_name;
                    _obj5.Province_IS = formObj.province_flag ? ~~formObj.province_flag : formObj.province_flag;
                    // 发送数据前检测obj是否为空
                    var _isEmptyObject5 = $.isEmptyObject(_obj5);
                    if (!_isEmptyObject5) {
                        var _loadTips5 = layer.load(2, {
                            shade: 0.02
                        });
                        console.log(_obj5);
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddCityData', _obj5).then(function (data) {
                            if (data.Status) {
                                layer.msg(data.Message);
                                $baseBoxMask.hide();
                                $('.js-reset').trigger('click');
                                pub.tableConDataInit({
                                    url: '/FlightDataApi/GetCity',
                                    tmplId: 'city-data-tmpl',
                                    elemId: 'city-data'
                                });
                            } else {
                                layer.msg(data.Message);
                            }
                        }).fail(function (err) {
                            layer.msg('发送请求失败');
                        }).always(function () {
                            layer.close(_loadTips5);
                        });
                    }
                }
                //省份
                if ($(this).closest('.layui-form').hasClass('province')) {
                    var _obj6 = {};
                    //如果id有值，则是编辑，不是添加
                    if (formObj.id) {
                        _obj6.Province_ID = ~~formObj.id;
                    }
                    _obj6.Short_Name = formObj.short_name;
                    _obj6.Name_Chinese = formObj.cn_name;
                    _obj6.Name_English = formObj.en_name;
                    _obj6.DORI = formObj.dori;
                    // 发送数据前检测obj是否为空
                    var _isEmptyObject6 = $.isEmptyObject(_obj6);
                    if (!_isEmptyObject6) {
                        var _loadTips6 = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddProvinceData', _obj6).then(function (data) {
                            if (data.Status) {
                                layer.msg(data.Message);
                                $baseBoxMask.hide();
                                $('.js-reset').trigger('click');
                                pub.tableConDataInit({
                                    url: '/FlightDataApi/GetProvince',
                                    tmplId: 'province-data-tmpl',
                                    elemId: 'province-data'
                                });
                            } else {
                                layer.msg(data.Message);
                            }
                        }).fail(function (err) {
                            layer.msg('发送请求失败');
                        }).always(function () {
                            layer.close(_loadTips6);
                        });
                    }
                }
                //国家
                if ($(this).closest('.layui-form').hasClass('country')) {
                    var _obj7 = {};
                    //如果id有值，则是编辑，不是添加
                    if (formObj.id) {
                        _obj7.ID = ~~formObj.id;
                    }
                    _obj7.Country_IATA = formObj.country_iata;
                    _obj7.Name_Chinese = formObj.cn_name;
                    _obj7.Name_English = formObj.en_name;
                    _obj7.Country_ICAO = formObj.country_icao;
                    // 发送数据前检测obj是否为空
                    var _isEmptyObject7 = $.isEmptyObject(_obj7);
                    if (!_isEmptyObject7) {
                        var _loadTips7 = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddCountryData', _obj7).then(function (data) {
                            if (data.Status) {
                                layer.msg(data.Message);
                                $baseBoxMask.hide();
                                $('.js-reset').trigger('click');
                                pub.tableConDataInit({
                                    url: '/FlightDataApi/GetCountry',
                                    tmplId: 'country-data-tmpl',
                                    elemId: 'country-data'
                                });
                            } else {
                                layer.msg(data.Message);
                            }
                        }).fail(function (err) {
                            layer.msg('发送请求失败');
                        }).always(function () {
                            layer.close(_loadTips7);
                        });
                    }
                }
                //任务代码
                if ($(this).closest('.layui-form').hasClass('task-code')) {
                    var _obj8 = {};
                    //如果id有值，则是编辑，不是添加
                    if (formObj.id) {
                        _obj8.ID = ~~formObj.id;
                    }
                    _obj8.Task_Code = formObj.task_code;
                    _obj8.Name_Chinese = formObj.cn_name;
                    _obj8.Name_English = formObj.en_name;
                    _obj8.Description = formObj.desc;
                    // 发送数据前检测obj是否为空
                    var _isEmptyObject8 = $.isEmptyObject(_obj8);
                    if (!_isEmptyObject8) {
                        var _loadTips8 = layer.load(2, {
                            shade: 0.02
                        });
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddTaskCode', _obj8).then(function (data) {
                            if (data.Status) {
                                layer.msg(data.Message);
                                $baseBoxMask.hide();
                                $('.js-reset').trigger('click');
                                pub.tableConDataInit({
                                    url: '/FlightDataApi/GetFlightBasicsData',
                                    tmplId: 'task-code-tmpl',
                                    elemId: 'task-code'
                                });
                            } else {
                                layer.msg(data.Message);
                            }
                        }).fail(function (err) {
                            layer.msg('发送请求失败');
                        }).always(function () {
                            layer.close(_loadTips8);
                        });
                    }
                }
                //延误代码
                if ($(this).closest('.layui-form').hasClass('delay-code')) {
                    var _obj9 = {};
                    //如果id有值，则是编辑，不是添加
                    if (formObj.id) {
                        _obj9.ID = ~~formObj.id;
                    }
                    _obj9.Delay_Code = formObj.delay_code;
                    _obj9.Type = formObj.delay_type;
                    _obj9.Code_Chinese = formObj.cn_name;
                    _obj9.Code_English = formObj.en_name;
                    _obj9.Description = formObj.desc;
                    // 发送数据前检测obj是否为空
                    var _isEmptyObject9 = $.isEmptyObject(_obj9);
                    if (!_isEmptyObject9) {
                        var _loadTips9 = layer.load(2, {
                            shade: 0.02
                        });
                        console.log(_obj9);
                        //发送添加请求
                        util.sendPostReq('/FlightDataApi/AddDelayCode', _obj9).then(function (data) {
                            if (data.Status) {
                                layer.msg(data.Message);
                                $baseBoxMask.hide();
                                $('.js-reset').trigger('click');
                                pub.tableConDataInit({
                                    url: '/FlightDataApi/GetDelayCode',
                                    tmplId: 'delay-code-tmpl',
                                    elemId: 'delay-code'
                                });
                            } else {
                                layer.msg(data.Message);
                            }
                        }).fail(function (err) {
                            layer.msg('发送请求失败');
                        }).always(function () {
                            layer.close(_loadTips9);
                        });
                    }
                }
                return false;
            });
        },
        /** 航班数据列表初始化
         * @param option Object类型
         * option.url: 请求链接
         * option.reqDate: 请求主体
         * option.tmplId: doT模版ID
         * option.elemId: 目标ID
         */
        tableConDataInit: function tableConDataInit(option) {
            option.reqData = option.reqData || {};
            option.url = option.url || '/FlightDataApi/GetFlightDataList';
            option.reqData.pageSize = 12;
            //如果选中了全选按钮，每次初始化应该去掉选中效果
            $('.js-allEquip').hasClass('checkbox-checked') ? $('.js-allEquip').removeClass('checkbox-checked') : null;

            //请求列表
            var loadTips = layer.load(2, {
                shade: 0.03
            });
            util.sendPostReq(option.url, option.reqData).then(function (data) {
                if (data.Status) {
                    if (data.Data.List.length > 0) {
                        var str = $('#' + option.tmplId).html();
                        var res = doT.template(str)(data.Data.List);
                        $('#' + option.elemId).html(res);
                        //总数据count->作为第二个then的参数
                        return data.Data.Count;
                    } else {
                        $('#' + option.elemId).html('');
                    }
                } else {
                    layer.msg(data.Message);
                }
            }).then(function (count) {

                //内容列表添加局部滚动
                pub.removeScrollbar();
                scrollHCount = util.myHScroll('.js-tabCtrl-con');
                scrollCount = util.myScroll('.js-tableCon');
                //分页
                pub.laypageEvent({
                    pageNum: Math.ceil(count / option.reqData.pageSize),
                    url: option.url,
                    pageSize: option.reqData.pageSize,
                    tmplId: option.tmplId,
                    targetId: option.elemId
                });
            }).fail(function (err) {
                layer.msg('请求发生错误');
            }).always(function () {
                layer.close(loadTips);
            });
        }
    };
    //航班数据表tableCon中的事件
    var flightDataTableEvent = function flightDataTableEvent() {

        //tabCtrl-> 切换到/离港航班
        $('.js-tabCtrl').on('click', 'div', function (e) {
            var $dataTable = $(this).closest('.js-dataTable');
            //不同选项发送不同请求 
            pub.ctrlTabChangeCon(this);
            //TAB切换清除选中效果
            $dataTable.find('.row.is-active').removeClass('is-active');
            $dataTable.find('.checkbox.checkbox-checked').removeClass('checkbox-checked');
            e.stopPropagation();
        });

        var $tableCon = $('.js-dataTable .js-tableCon');

        //tableCon列表点击效果
        $tableCon.on('click', function (e) {
            var tar = e.target,
                $parTag = $(tar.parentNode),
                flag = $parTag.hasClass('row');
            //由于iscoll点击一次会触发两次，所以先销毁，在函数尾部再添加上
            /* scrollCount ? util.destroyScroll(scrollCount) : null;
             scrollHCount ? util.destroyScroll(scrollHCount) : null;*/
            if (flag) {
                //点击当前row时->移除全部row复选框的效果
                $(this).find('.checkbox').removeClass('checkbox-checked');
                $('.allEquip').removeClass('checkbox-checked');
                //点击时背景改变
                if ($parTag.attr('switch')) {
                    $parTag.removeAttr('switch');
                    $parTag.removeClass('is-active');
                    $parTag.find('.checkbox').removeClass('checkbox-checked');
                } else {
                    $parTag.attr('switch', 'on').siblings().removeAttr('switch');
                    $parTag.addClass('is-active').siblings().removeClass('is-active');
                    $parTag.find('.checkbox').addClass('checkbox-checked');
                }
            }
            /*//添加上iscroll实例
            scrollHCount = util.myHScroll('.js-tabCtrl-con');
            scrollCount = util.myScroll('.js-tableCon');*/
        });
        //列表的每个复选框 & row背景色改变
        $tableCon.on('click', '.checkbox', function (e) {
            $('.js-allEquip').removeClass('checkbox-checked');
            var $this = $(this),
                $row = $this.closest('.row');
            var flag = $this.hasClass('checkbox-checked');
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
                //$this-> 控制的是当前按钮的开关效果
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

    //弹框中的事件
    var modalEvent = function modalEvent() {
        var $resetBtn = $('.js-modal-fliData .js-reset');
        //确认按钮
        $('.js-modal-fliData .ensure-btn').on('click', function () {
            var $this = $(this);
            var $boxMask = $this.closest('.mod-boxMask');

            //点击上传
            var $gar = $this.parent().parent();
            if ($gar.hasClass('importFile')) {
                $.ajaxFileUpload({
                    url: '/FlightDataApi/UploadFile',
                    fileElementId: 'path',
                    secureuri: false,
                    dataType: 'json',
                    success: function success(data) {
                        console.log('data', data);
                    },
                    error: function error(err) {
                        console.log('err', err);
                    }
                });
            }
            // $boxMask.hide();
            // $resetBtn.trigger('click');
        });
        //取消
        $('.js-modal-fliData .cancel-btn').on('click', function () {
            var $boxMask = $(this).closest('.mod-boxMask');
            $resetBtn.trigger('click');
            $boxMask.hide();
        });
        //关闭按钮
        $('.js-modal-fliData .close-btn').on('click', function () {
            var $boxMask = $(this).closest('.mod-boxMask');
            $resetBtn.trigger('click');
            $boxMask.hide();
        });

        //上传按钮
        $(".upload").on("change", "input[type='file']", function (e) {
            var filePath = $(this).val();
            if (filePath.indexOf("xlsx") != -1 || filePath.indexOf("xls") != -1) {
                $(".input-tips").html("").hide();
                var arr = filePath.split('\\');
                var fileName = arr[arr.length - 1];
                $(".show-file-name").val(fileName);
            } else {
                $(".show-file-name").val("");
                $(".input-tips").html("您未上传文件，或者您上传文件类型有误！").show();
                return false;
            }
        });
    };

    //菜单menu中的事件
    var menuEvent = function menuEvent() {
        var $title = $('.js-modal-fliData .js-modalBigTitle');
        var $importTitle = $('.js-import-data .js-modalBigTitle');
        //添加数据
        $('.js-menu-add').on('click', function () {
            var cur = $('.js-dataTable.show .js-tabCtrl').find('.is-click');
            var curText = cur.text();
            $title.html('添加' + curText);
            //根据不同的列表类型改变弹框部分内容
            var tabAttr = cur.attr('data-menu');
            pub.changeModalCon(tabAttr);
            //根据dataTabs的不同显示对应的弹框
            var curAttr = $('.js-dataTabs .dataTabs').find('.is-active').attr('data-modal');
            pub.changeModalType(curAttr);
        });
        //编辑数据
        $('.js-menu-edit').on('click', function () {
            var checkLength = $('.js-tableCon').find('.checkbox.checkbox-checked').length;
            if (checkLength === 0) {
                layer.alert('未选中数据');
            } else if (checkLength > 1) {
                layer.alert('只能编辑单条数据');
            } else {
                //选中行的id
                var dataId = $('.js-tableCon').find('.row.is-active').attr('data-id');
                //当前二级切换的元素
                var $curEle = $('.js-dataTable.show .js-tabCtrl').find('.is-click');
                var curText = $curEle.text();
                $title.html('编辑' + curText);
                //根据不同的列表类型改变弹框部分内容
                var tabAttr = $curEle.attr('data-menu');
                pub.changeModalCon(tabAttr, dataId);
                //根据dataTabs的不同显示对应的弹框
                var curAttr = $('.js-dataTabs .dataTabs').find('.is-active').attr('data-modal');
                pub.changeModalType(curAttr);
            }
        });
        //导入数据
        $('.js-menu-save').on('click', function () {
            $importTitle.html('上传文件');
            $('.js-import-data').show();
        });
        //发布数据
        $('.js-menu-send').on('click', function () {});
        //删除按钮
        $('.js-menu-del').on('click', function (e) {

            //获取列表选中行
            var $tableCon = $('.js-tableCon').find('.checkbox.checkbox-checked');
            var checkLength = $tableCon.length;
            var rows = $tableCon.closest('.row').get();
            //选中的行的id集合
            var ids = rows.map(function (row) {
                return row.getAttribute('data-rowid');
            }).join(',');
            if (checkLength === 0) {
                layer.alert('未选中数据');
            } else {
                // pub.delData(ids);
            }
        });
    };

    //多种数据切换事件:次日数据、资源分配、基础...
    var dataTabsEvent = function dataTabsEvent() {

        $('.js-dataTabs .dataTabs').on('click', 'li', function (e) {
            //根据航班数据、资源分配...不同改变列表内容
            pub.dataTabsChangeCon(this);

            //数据切换->清除选中行和全选btn样式
            var $dataTabs = $(this).closest('.js-dataTabs');
            $dataTabs.find('.row.is-active').removeClass('is-active');
            $dataTabs.find('.checkbox.checkbox-checked').removeClass('checkbox-checked');
            e.stopPropagation();
        });
    };
    //日期插件初始化
    var jeDateInit = function jeDateInit() {
        var date = {
            format: 'YYYY-MM-DD',
            // minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            zIndex: 9999,
            maxDate: '2099-06-30 23:59:59'
        };
        var dateTime = {
            format: 'YYYY-MM-DD hh:mm:ss',
            // minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            zIndex: 9999,
            maxDate: '2099-06-30 23:59:59'
        };
        $('.js-date-time').jeDate(dateTime);
        $('.js-date').jeDate(date);
    };

    var render = function render() {
        form.render();
        //之所以这么写是为了在flightData页面插入index页之后再获取弹框元素
        getElem();

        //首次加载获取->次日计划-离港航班数据
        pub.tableConDataInit({
            reqData: {
                aord: 'D',
                planType: 0
            },
            tmplId: 'flight-plan-tmpl',
            elemId: 'flight-plan-d'
        });

        //选项卡
        util.tabChange('.js-dataTabs', 'is-active');
        util.tabChange('.js-dataTable');

        //绑定事件和插件初始化

        flightDataTableEvent();
        modalEvent();
        menuEvent();
        dataTabsEvent();
        jeDateInit();
        //给弹框绑定layui监听事件
        pub.sendBaseDataFormVal();
        pub.sendFlightDataFormVal();
    };
    return {
        render: render
    };
})(jQuery, util, layer, layui, doT);

/***/ }),
/* 2 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony default export */ __webpack_exports__["a"] = (function logManage($, util) {

    var scrollCount = void 0;
    //公共方法
    var pub = {
        setModalCon: function setModalCon() {
            var a = [];
            $('.show .js-tableCon').find('.checkbox.checkbox-checked').parent().next().each(function (index, item) {
                var cur = item.innerHTML;
                a.push(cur);
            });
            return a.join(', ');
        }
    };

    //航班数据表tableCon中的事件
    var logTableEvent = function logTableEvent() {
        var $tableCon = $('.js-tableCon');
        //tabCtrl-> 选项卡切换时iscroll初始化
        $('.js-tabCtrl').on('click', 'div', function () {
            util.destroyScroll(scrollCount);
            scrollCount = util.myScroll('.js-tableCon');
            //TAB切换清除选中效果
            var $dataTable = $(this).closest('.js-dataTable');
            $dataTable.find('.row.is-active').removeClass('is-active');
            $dataTable.find('.checkbox.checkbox-checked').removeClass('checkbox-checked');
        });
        //tableCon列表点击效果
        $tableCon.on('click', function (e) {
            var tar = e.target,
                parTag = tar.parentNode,
                flag = $(parTag).hasClass('row');
            if (flag) {
                //点击当前row时->移除全部row复选框的效果
                $(this).find('.checkbox').removeClass('checkbox-checked');
                $('.allEquip').removeClass('checkbox-checked');
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
        $tableCon.on('click', '.checkbox', function () {
            $('.js-allEquip').removeClass('checkbox-checked');
            var $this = $(this),
                $row = $this.closest('.row');
            var flag = $(this).hasClass('checkbox-checked');
            if (!flag) {
                $this.addClass('checkbox-checked');
                $row.addClass('is-active');
            } else {
                $this.removeClass('checkbox-checked');
                $row.removeClass('is-active');
                // $('.js-allEquip').removeClass('checkbox-checked');
            }
        });
        //全选按钮
        $('.js-allEquip').on('click', function (e) {
            var tar = e.target,
                par = tar.parentNode;
            var $tableCon = $(par).siblings('.js-tableCon');
            var $checkboxs = $tableCon.find('.checkbox');
            var $rows = $tableCon.find('.row');
            var flag = $(this).hasClass('checkbox-checked');
            if (!flag) {
                //$(this)-> 控制的是当前按钮的开关效果
                $(this).addClass('checkbox-checked');
                $rows.addClass('is-active');
                $checkboxs.addClass('checkbox-checked');
                $rows.removeAttr('switch');
            } else {
                $(this).removeClass('checkbox-checked');
                $checkboxs.removeClass('checkbox-checked');
                $rows.removeClass('is-active');
            }
        });
    };
    var render = function render() {
        //局部滚动
        scrollCount = util.myScroll('.js-tableCon');
        //选项卡
        util.tabChange('.js-dataTable');
        //日志列表绑定事件
        logTableEvent();
    };
    return {
        render: render
    };
})(jQuery, util);

/***/ }),
/* 3 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony default export */ __webpack_exports__["a"] = (function msgRelease($, util) {

    var scrollCount = null,
        scrollDevCount = null;

    //公共方法
    var pub = {
        setModalCon: function setModalCon() {
            var a = [];
            $('.show .js-tableCon').find('.checkbox.checkbox-checked').parent().next().each(function (index, item) {
                var cur = item.innerHTML;
                a.push(cur);
            });
            return a.join(', ');
        },
        //根据一级切换显示不同的弹框和不同的menu
        showDiffModalAndMenu: function showDiffModalAndMenu(ele) {
            var $menuAdd = $('.js-menu-add'),
                $menuPub = $('.js-msg-pub');

            var curAttr = $(ele).attr('data-flag');
            switch (curAttr) {
                case 'msg-pub':
                    $menuAdd.addClass('hide');
                    $menuPub.removeClass('hide');
                    $('.js-modal-pub').removeClass('hide').siblings('.layui-form').addClass('hide');
                    break;
                case 'define-msg':
                    $menuAdd.removeClass('hide');
                    $menuPub.addClass('hide');
                    $('.js-modal-define').removeClass('hide').siblings('.layui-form').addClass('hide');
                    break;
                case 'msg-area':
                    $menuAdd.removeClass('hide');
                    $menuPub.addClass('hide');
                    $('.js-modal-area').removeClass('hide').siblings('.layui-form').addClass('hide');
                    break;
            }
        },
        //点击确认，取消，close按钮把多选设备列表恢复到初始状态
        backInitState: function backInitState(ele) {
            if ($(ele).closest('.js-modal-area')) {
                util.destroyScroll(scrollDevCount);
                //清除选中
                var $checkContain = $('.js-checkbox-list .checkContain');
                $checkContain.children().removeClass('checkbox-checked');
                $checkContain.removeClass('is-active');
            }
        },
        //改变设备列表多选框样式
        changeDeviceState: function changeDeviceState(weeks) {
            var $checkboxList = $('.js-checkbox-list');
            //首先全部清除样式
            $checkboxList.find('.checkContain').children().eq(0).removeClass('checkbox-checked');
            //然后再添加相应样式
            weeks.forEach(function (num) {
                $checkboxList.find('.checkContain[data-name=' + num + ']').children().eq(0).addClass('checkbox-checked');
            });
            if (weeks.length == 7) {
                $('.js-checkbox-list').find('.js-checkAll').children().eq(0).addClass('checkbox-checked');
            }
        },
        //获取设备列表选中的设备值
        getDeviceVal: function getDeviceVal() {
            var arr = [];
            var $list = $('.js-checkbox-list .checkContain').not($('.js-checkAll')).find('.checkbox-checked').parent();
            $list.each(function (i, item) {
                arr.push(item.getAttribute('data-name'));
            });

            return arr.join();
        }
    };

    //数据表tableCon中的事件
    var logTableEvent = function logTableEvent() {
        var $tableCon = $('.js-dataTable .js-tableCon');

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
        $tableCon.on('click', '.checkbox', function () {
            $('.js-allEquip').removeClass('checkbox-checked');
            var $this = $(this),
                $row = $this.closest('.row');
            var flag = $(this).hasClass('checkbox-checked');
            if (!flag) {
                $this.addClass('checkbox-checked');
                $row.addClass('is-active');
            } else {
                $this.removeClass('checkbox-checked');
                $row.removeClass('is-active');
                // $('.js-allEquip').removeClass('checkbox-checked');
            }
        });
        //全选按钮
        $('.js-allEquip').on('click', function (e) {
            var tar = e.target,
                par = tar.parentNode;
            var $tableCon = $(par).siblings('.js-tableCon');
            var $checkboxs = $tableCon.find('.checkbox');
            var $rows = $tableCon.find('.row');
            var flag = $(this).hasClass('checkbox-checked');
            if (!flag) {
                //$(this)-> 控制的是当前按钮的开关效果
                $(this).addClass('checkbox-checked');
                $rows.addClass('is-active');
                $checkboxs.addClass('checkbox-checked');
                $rows.removeAttr('switch');
            } else {
                $(this).removeClass('checkbox-checked');
                $checkboxs.removeClass('checkbox-checked');
                $rows.removeClass('is-active');
            }
        });
    };
    //弹框中的事件
    var modalBtnEvent = function modalBtnEvent() {
        var $resetBtn = $('.modal-msg .js-reset');
        var $boxMask = $('.modal-msg .mod-boxMask');

        //确认按钮
        $('.modal-msg .ensure-btn').on('click', function () {
            $resetBtn.trigger('click');
            $boxMask.hide();
            pub.backInitState(this);
        });
        //取消
        $('.modal-msg .cancel-btn').on('click', function () {
            $resetBtn.trigger('click');
            $boxMask.hide();
            pub.backInitState(this);
        });
        //关闭按钮
        $('.modal-msg .close-btn').on('click', function () {
            $resetBtn.trigger('click');
            $boxMask.hide();
            pub.backInitState(this);
        });
        //多选设备
        $('.js-checkbox-list').on('click', 'div', function (e) {
            var $this = $(this),
                $selfChild = $this.children(),
                $checkAllChild = $this.siblings('.js-checkAll').children(),
                $sibChilds = $this.siblings().children(),
                isExist = $selfChild.hasClass('checkbox-checked'),
                isCheckAll = $this.hasClass('js-checkAll');
            console.log(111);
            //普通
            if (isExist) {
                console.log(2222);
                $checkAllChild.removeClass('checkbox-checked');
                $selfChild.removeClass('checkbox-checked');
                $this.removeClass('is-active');
            } else {
                $selfChild.addClass('checkbox-checked');
                $this.addClass('is-active');
            }
            //全选
            if (isCheckAll && !isExist) {
                console.log(333);
                $sibChilds.addClass('checkbox-checked');
                $this.parent().children().addClass('is-active');
            } else if (isCheckAll && isExist) {
                $sibChilds.removeClass('checkbox-checked');
                $this.parent().children().removeClass('is-active');
            }
            e.stopPropagation();
        });
    };

    //菜单menu中的事件
    var menuEvent = function menuEvent() {
        var $boxMask = $('.modal-msg .mod-boxMask');
        var $modalCon = $('.confirm .js-modalCon'),
            $modalTitle = $('.confirm .js-modalTitle');
        //添加数据
        $('.js-msg-pub').on('click', function () {
            $boxMask.show();
        });
        //添加数据
        $('.js-menu-add').on('click', function () {
            $boxMask.show();
            //如果是添加消息区域，则初始化iscroll实例            
            if ($('.js-modal-area').hasClass('hide') == false) {
                scrollDevCount = util.myScroll('.js-scroll-device');
            }
        });
        //编辑数据
        $('.js-menu-edit').on('click', function () {
            $boxMask.show();
        });
        //删除按钮
        $('.js-menu-del').on('click', function (e) {
            var tar = e.target,
                menuCon = $(tar).text();
        });
    };

    var dataTabsEvent = function dataTabsEvent() {

        $('.js-dataTabs .dataTabs').on('click', 'li', function (e) {
            var $this = $(this),
                $dataTabs = $this.closest('.js-dataTabs');
            //切换页面时清除选中效果
            $dataTabs.find('.row.is-active').removeClass('is-active');
            $dataTabs.find('.checkbox.checkbox-checked').removeClass('checkbox-checked');

            //change menu & 弹框类型
            pub.showDiffModalAndMenu(this);
            //scroll初始化
            util.destroyScroll(scrollCount);
            scrollCount = util.myScroll('.js-tableCon');
        });
    };

    //日期插件初始化
    var jeDateInit = function jeDateInit() {
        /*let date = {
            format: 'YYYY-MM-DD',
            // minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            zIndex: 9999,
            maxDate: '2099-06-30 23:59:59',
        };*/
        var dateTime = {
            format: 'YYYY-MM-DD hh:mm:ss',
            // minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            zIndex: 9999,
            maxDate: '2099-06-30 23:59:59'
        };
        $('.js-date-time').jeDate(dateTime);
        // $('.js-date').jeDate(date);
    };

    var render = function render() {
        //使用layui's form
        var form = layui.form();
        form.render();
        //局部滚动
        scrollCount = util.myScroll('.js-tableCon');
        //选项卡
        util.tabChange('.js-dataTabs', 'is-active');

        logTableEvent();
        modalBtnEvent();
        menuEvent();
        dataTabsEvent();
        jeDateInit();
    };
    return {
        render: render
    };
})(jQuery, util);

/***/ }),
/* 4 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony default export */ __webpack_exports__["a"] = (function permissManage($, util) {

    var scrollCount = null,
        scrollDevCount = null;
    //公共方法
    var pub = {
        setModalCon: function setModalCon() {
            var a = [];
            $('.show .js-tableCon').find('.checkbox.checkbox-checked').parent().next().each(function (index, item) {
                var cur = item.innerHTML;
                a.push(cur);
            });
            return a.join(', ');
        },
        //根据一级切换显示不同的弹框和不同的menu
        showDiffModalAndMenu: function showDiffModalAndMenu(ele) {
            var curAttr = $(ele).attr('data-flag');
            switch (curAttr) {
                case 'limit-manage':
                    $('.modal-permiss').find('.mod-popupBox').removeClass('bigWidth');
                    $('.js-limit-man').removeClass('hide').siblings('.layui-form').addClass('hide');
                    break;
                case 'user-manage':
                    $('.modal-permiss').find('.mod-popupBox').addClass('bigWidth');
                    $('.js-user-man').removeClass('hide').siblings('.layui-form').addClass('hide');
                    break;
            }
        },
        //点击确认，取消，close按钮把多选设备列表恢复到初始状态
        backInitState: function backInitState(ele) {
            if ($(ele).closest('.js-modal-area')) {
                util.destroyScroll(scrollDevCount);
                //清除选中
                var $checkContain = $('.js-checkbox-list .checkContain');
                $checkContain.children().removeClass('checkbox-checked');
                $checkContain.removeClass('is-active');
            }
        },
        //改变设备列表多选框样式
        changeDeviceState: function changeDeviceState(weeks) {
            var $checkboxList = $('.js-checkbox-list');
            //首先全部清除样式
            $checkboxList.find('.checkContain').children().eq(0).removeClass('checkbox-checked');
            //然后再添加相应样式
            weeks.forEach(function (num) {
                $checkboxList.find('.checkContain[data-name=' + num + ']').children().eq(0).addClass('checkbox-checked');
            });
            if (weeks.length == 7) {
                $('.js-checkbox-list').find('.js-checkAll').children().eq(0).addClass('checkbox-checked');
            }
        },
        //获取设备列表选中的设备值
        getDeviceVal: function getDeviceVal() {
            var arr = [];
            var $list = $('.js-checkbox-list .checkContain').not($('.js-checkAll')).find('.checkbox-checked').parent();
            $list.each(function (i, item) {
                arr.push(item.getAttribute('data-name'));
            });

            return arr.join();
        }
    };

    //数据表tableCon中的事件
    var logTableEvent = function logTableEvent() {
        var $tableCon = $('.js-dataTable .js-tableCon');

        //tableCon列表点击效果
        $tableCon.on('click', function (e) {
            var tar = e.target,
                parTag = tar.parentNode,
                flag = $(parTag).hasClass('row');
            if (flag) {
                //点击当前row时->移除全部row复选框的效果
                $(this).find('.checkbox').removeClass('checkbox-checked');
                $('.allEquip').removeClass('checkbox-checked');
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
        $tableCon.on('click', '.checkbox', function () {
            $('.js-allEquip').removeClass('checkbox-checked');
            var $this = $(this),
                $row = $this.closest('.row');
            var flag = $(this).hasClass('checkbox-checked');
            if (!flag) {
                $this.addClass('checkbox-checked');
                $row.addClass('is-active');
            } else {
                $this.removeClass('checkbox-checked');
                $row.removeClass('is-active');
                // $('.js-allEquip').removeClass('checkbox-checked');
            }
        });
        //全选按钮
        $('.js-allEquip').on('click', function (e) {
            var tar = e.target,
                par = tar.parentNode;
            var $tableCon = $(par).siblings('.js-tableCon');
            var $checkboxs = $tableCon.find('.checkbox');
            var $rows = $tableCon.find('.row');
            var flag = $(this).hasClass('checkbox-checked');
            if (!flag) {
                //$(this)-> 控制的是当前按钮的开关效果
                $(this).addClass('checkbox-checked');
                $rows.addClass('is-active');
                $checkboxs.addClass('checkbox-checked');
                $rows.removeAttr('switch');
            } else {
                $(this).removeClass('checkbox-checked');
                $checkboxs.removeClass('checkbox-checked');
                $rows.removeClass('is-active');
            }
        });
    };
    //弹框中的事件
    var modalBtnEvent = function modalBtnEvent() {
        var $resetBtn = $('.modal-log .js-reset'),
            $cfResetBtn = $('.confirm .js-cfReset');
        var $boxMask = $('.modal-permiss .mod-boxMask');
        //确认按钮
        $('.modal-permiss .ensure-btn').on('click', function () {
            $boxMask.hide();
            pub.backInitState(this);
        });
        //取消
        $('.modal-permiss .cancel-btn').on('click', function () {
            $resetBtn.trigger('click');
            $boxMask.hide();
            pub.backInitState(this);
        });
        //关闭按钮
        $('.modal-permiss .close-btn').on('click', function () {
            $resetBtn.trigger('click');
            $boxMask.hide();
            pub.backInitState(this);
        });
        //多选设备
        $('.js-checkbox-list').on('click', 'div', function (e) {
            var $this = $(this),
                $selfChild = $this.children(),
                $checkAllChild = $this.siblings('.js-checkAll').children(),
                $sibChilds = $this.siblings().children(),
                isExist = $selfChild.hasClass('checkbox-checked'),
                isCheckAll = $this.hasClass('js-checkAll');
            console.log(111);
            //普通
            if (isExist) {
                console.log(2222);
                $checkAllChild.removeClass('checkbox-checked');
                $selfChild.removeClass('checkbox-checked');
                $this.removeClass('is-active');
            } else {
                $selfChild.addClass('checkbox-checked');
                $this.addClass('is-active');
            }
            //全选
            if (isCheckAll && !isExist) {
                console.log(333);
                $sibChilds.addClass('checkbox-checked');
                $this.parent().children().addClass('is-active');
            } else if (isCheckAll && isExist) {
                $sibChilds.removeClass('checkbox-checked');
                $this.parent().children().removeClass('is-active');
            }
            e.stopPropagation();
        });
    };

    //菜单menu中的事件
    var menuEvent = function menuEvent() {
        var $boxMask = $('.modal-permiss .mod-boxMask');
        var $modalCon = $('.confirm .js-modalCon'),
            $modalTitle = $('.confirm .js-modalTitle');
        //添加数据
        $('.js-menu-add').on('click', function () {
            $boxMask.show();
            scrollDevCount = util.myScroll('.js-scroll-device');
        });
        //编辑数据
        $('.js-menu-edit').on('click', function () {
            $boxMask.show();
            //如果是添加消息区域，则初始化iscroll实例  
            scrollDevCount = util.myScroll('.js-scroll-device');
        });
        //删除按钮
        $('.js-menu-del').on('click', function (e) {
            var tar = e.target,
                menuCon = $(tar).text();
            var str = pub.setModalCon();
            $('.js-btnEnsure1').css('background-color', '#f26455');
            $cfBoxMask.show();
            $modalTitle.html(menuCon);
            $modalCon.html('是否' + menuCon + ': ' + str);
        });
    };

    var dataTabsEvent = function dataTabsEvent() {
        $('.js-dataTabs .dataTabs').on('click', 'li', function (e) {
            util.destroyScroll(scrollCount);
            scrollCount = util.myScroll('.js-tableCon');

            var $dataTabs = $(this).closest('.js-dataTabs');
            $dataTabs.find('.row.is-active').removeClass('is-active');
            $dataTabs.find('.checkbox.checkbox-checked').removeClass('checkbox-checked');

            //change menu & 弹框类型
            pub.showDiffModalAndMenu(this);
        });
    };

    var render = function render() {
        //使用layui's form
        var form = layui.form();
        form.render();
        //局部滚动
        scrollCount = util.myScroll('.js-tableCon');
        //选项卡
        util.tabChange('.js-dataTabs', 'is-active');

        logTableEvent();
        modalBtnEvent();
        menuEvent();
        dataTabsEvent();
    };
    return {
        render: render
    };
})(jQuery, util);

/***/ }),
/* 5 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
/* harmony default export */ __webpack_exports__["a"] = (function templateManage($, util, layui, layer, doT) {

    //为了不重复执行myScroll方法，因为多次执行myScroll会创建多个滚动条
    var tableConCount = void 0;
    var $boxMaskBig = null,
        $modTitleBig = null;

    var form = layui.form();

    var getElem = function getElem() {
        $boxMaskBig = $('.modal-tmplManage-big .mod-boxMask');
        $modTitleBig = $boxMaskBig.find('.js-modalBigTitle');
    };

    //公共方法
    var pub = {
        templateData: null,
        timer: null,
        //格式化时间
        formatDate: function formatDate(dataStr, mode) {
            var cur = new Date(Number(/(-?\d+)/.exec(dataStr)[1]));
            if (mode === 'date') {
                return cur.toLocaleDateString().formatTime('{0}-{1}-{2}');
            }
            if (mode === 'time') {
                return (/(\d{2}:\d{2}):\d{2}/.exec(cur)[1]
                );
            }
        },
        //确认弹框中的内容
        setModalCon: function setModalCon() {
            var a = [];
            $('.show .js-tableCon').find('.checkbox.checkbox-checked').parent().next().each(function (index, item) {
                var cur = item.innerHTML;
                a.push(cur);
            });
            return a.join(', ');
        },
        //模版分配弹框发送表单信息
        sendLayuiFormVal: function sendLayuiFormVal(form, flagId) {
            var obj = {};
            //获取下拉选择框的value
            form.on('select(modal-tmpl-sel)', function (data) {
                switch (data.elem.id) {
                    case 'deviceName':
                        obj.displayID = data.elem.value;
                        pub.getOccupiedMaxIndex(data.elem.value);
                        pub.getOccupiedIndex(data.elem.value);
                        break;
                    case 'templateName':
                        obj.templateID = parseInt(data.elem.value);
                        //绑定模板对应的子模版
                        pub.bindToChildTmpl(data.elem.value);
                        break;
                }
            });
            //submit提交
            form.on('submit', function (data) {
                var formObj = data.field;
                //flagId->标识是添加还是更新数据
                if (flagId) {
                    obj.ID = flagId;
                    obj.displayID = formObj.deviceName;
                    obj.templateID = formObj.templateName;
                }
                //开始时间和结束时间
                obj.startTime = formObj.startDate;
                obj.endTime = formObj.endDate;
                obj.spaceStartTime = '1970-01-01 ' + formObj.startTime;
                obj.spaceEndTime = '1970-01-01 ' + formObj.endTime;
                if (formObj.startTime >= formObj.endTime) {
                    layer.alert('开始时间不能大于结束时间');
                    return;
                }
                //循环次数
                obj.count = ~~formObj.loopCount;
                //页面名称
                obj.pageName = formObj.pageName;
                //排序
                obj.index = ~~formObj.taxis;
                //上屏代码
                obj.topScreenCode = ~~formObj.topScreenCode;
                //星期
                var weekVal = pub.getWeeksVal();
                if (weekVal.length > 0) {
                    obj.weeks = weekVal;
                } else {
                    layer.alert('周：请至少选中一个');
                    return;
                }
                //持续时间
                // obj.intervalSecond = parseFloat(formObj.holdTime) * 60; 
                obj.intervalSecond = ~~formObj.holdTime;
                //广告
                formObj.ad ? obj.isAdvert = 1 : obj.isAdvert = 0;
                //是否覆盖
                formObj.cover ? obj.isCover = 1 : obj.isCover = 0;
                //是否发布
                formObj.isPublish ? obj.isSend = 1 : obj.isSend = 0;
                //子模版
                obj.dataValue = formObj.displaySymbol;
                //other
                obj.sort = 0;
                obj.advertUrl = 'http://';
                // 发送数据前检测obj是否为空
                var isEmptyObject = $.isEmptyObject(obj);
                if (!isEmptyObject) {
                    var loadTips = layer.load(2, {
                        shade: 0.02
                    });
                    //发送添加请求
                    util.sendPostReq('/TemplateApi/AddTemplateDisplay', obj).then(function (data) {
                        if (data.Status) {
                            layer.msg(data.Message);
                            $boxMaskBig.hide();
                            $('.js-reset').trigger('click');
                            //根据设备选择框的值来判定当前刷新的数据
                            pub.deviceSelStateChangeCon();
                            // pub.tableConDataInit();
                        } else {
                            layer.msg(data.Message);
                        }
                    }).fail(function (err) {
                        layer.msg(err);
                    }).always(function () {
                        layer.close(loadTips);
                    });
                }
                return false;
            });
        },
        //模版管理列表初始化
        tableConDataInit: function tableConDataInit(reqData) {
            reqData = reqData || {};
            reqData.pageSize = 10;
            //如果选中了全选按钮，每次初始化应该去掉选中效果
            $('.js-allEquip').hasClass('checkbox-checked') ? $('.js-allEquip').removeClass('checkbox-checked') : null;
            var loadTips = layer.load(2, {
                shade: 0.02
            });
            //请求列表
            util.sendPostReq('/TemplateApi/GetTemplateDisplayList', reqData).then(function (data) {
                if (data.Status) {
                    if (data.Data.List.length > 0) {
                        //根据Index排正序
                        data.Data.List.sort(function (a, b) {
                            return a.Index - b.Index;
                        });
                        // console.log(data);
                        var res = doT.template($('#template-list').html())(data.Data.List);
                        $('#tmplTableCon').html(res);
                        //避免iscroll重复加载
                        tableConCount ? util.destroyScroll(tableConCount) : null;
                        //局部滚动
                        tableConCount = util.myScroll('.js-tableCon');
                        //分页 & laypageNum页数
                        var laypageNum = Math.ceil(data.Data.Count / reqData.pageSize);
                        pub.laypageEvent(laypageNum, reqData.displayId, reqData.pageSize);
                    } else {
                        $('#tmplTableCon').html('');
                    }
                } else {
                    layer.msg(data.Message);
                }
            }).fail(function (err) {
                layer.msg('请求发送错误');
            }).always(function () {
                layer.close(loadTips);
            });
        },
        //ajax获取值并绑定到弹框
        getModalData: function getModalData(url, dotTmplID, tmplID, reqData) {
            //loading
            var loadTips = layer.load(2, {
                shade: 0.02
            });
            //tmpl's list
            util.sendPostReq(url, reqData).then(function (data) {
                if (data.Status === 1 && data.Data) {
                    var tmplVal = $('#' + dotTmplID).html();
                    var res = doT.template(tmplVal)(data.Data);
                    $('#' + tmplID).html(res);
                    form.render();
                    //如果是获取模版列表，赋值给templateData
                    /GetTemplateList/.test(url) ? pub.templateData = data.Data : null;
                }
            }).fail(function (err) {
                layer.msg('请求发送错误');
            }).always(function () {
                layer.close(loadTips);
            });
        },
        //编辑按钮->获取数据并绑定到弹框
        bindValueToModal: function bindValueToModal(data) {
            window.clearTimeout(pub.timer);
            //有极小的概率发生绑定数据到弹框之前获取不到设备列表，所以先form.render
            pub.timer = setTimeout(function () {
                //模版名称
                $('#templateName option[value="' + data.TemplateID + '"]').prop('selected', 'selected');
                //设备名称
                $('#deviceName option[value=' + data.DisplayID + ']').prop('selected', 'selected');
                //子模版
                $('#childTmplName').find('option[value="' + data.DataValue + '"]').prop('selected', 'selected');
                form.render();
            }, 5);

            //页面名称
            $('#pageName').val(data.PageName);
            //排序
            //to be continue
            $('#taxis').val(data.Index);
            //持续时间
            $('#holdTime').val(data.IntervalSecond);
            //上屏代码
            //to be continue
            //循环次数
            $('#loopCount').val(data.Count);
            //日期
            $('#startDate').val(pub.formatDate(data.StartTime, 'date'));
            $('#endDate').val(pub.formatDate(data.EndTime, 'date'));
            //时分
            $('#startTime').val(pub.formatDate(data.SpaceStartTime, 'time'));
            $('#endTime').val(pub.formatDate(data.SpaceEndTime, 'time'));
            //周
            //data.weeks
            var weeks = data.Weeks.split(',');
            pub.changeWeekState(weeks);
            //覆盖和广告
            !!data.IsAdvert ? $('#ad').prop('checked', 'checked') : null;
            !!data.IsCover ? $('#cover').prop('checked', 'checked') : null;
            !!data.IsSend ? $('#publish').prop('checked', 'checked') : null;

            form.render();
        },
        //点击确认，取消，close按钮->弹框重置到默认值
        backToDefaultVal: function backToDefaultVal() {
            //weeks周
            $('.js-checkbox-list .checkContain').children().addClass('checkbox-checked');
            //taxis排序
            $('#taxis').css('border', '');
            //弹框title
            $modTitleBig.html('模版分配');
        },
        //改变'周'复选框样式
        changeWeekState: function changeWeekState(weeks) {
            var $checkboxList = $('.js-checkbox-list');
            //首先全部清除样式
            $checkboxList.find('.checkContain').children().eq(0).removeClass('checkbox-checked');
            //然后再添加相应样式
            weeks.forEach(function (num) {
                $checkboxList.find('.checkContain[data-name=' + num + ']').children().eq(0).addClass('checkbox-checked');
            });
            if (weeks.length == 7) {
                $('.js-checkbox-list').find('.js-checkAll').children().eq(0).addClass('checkbox-checked');
            }
        },
        //获取选中的week值
        getWeeksVal: function getWeeksVal() {
            var arr = [];
            var $list = $('.js-checkbox-list .checkContain').not($('.js-checkAll')).find('.checkbox-checked').parent();
            $list.each(function (i, item) {
                arr.push(item.getAttribute('data-name'));
            });

            return arr.join();
        },
        //实时比较输入框的值并显示不同样式
        compareVal: function compareVal(ele, data) {
            var $ele = $(ele);
            data = data.split(',');
            var curVal = $ele.value;
            if (data.indexOf(curVal) >= 0) {
                $ele.css('border', '1px solid #f00');
                $('.input-tips').css('display', 'block');
            } else {
                $ele.css('border', '');
                $('.input-tips').css('display', 'none');
            }
            //实时输入值也进行比较
            $ele.on('input', function () {
                var val = this.value;
                if (data.indexOf(val) >= 0) {
                    this.style.border = '1px solid #f00';
                    $('.input-tips').css('display', 'block');
                } else {
                    this.style.border = '';
                    $('.input-tips').css('display', 'none');
                }
            });
        },
        //删除数据
        delData: function delData(ids) {
            layer.confirm('确定删除吗', {
                btn: ['取消', '确定']
            }, function () {
                layer.msg('不删除');
            }, function () {
                util.sendPostReq('/TemplateApi/DelTemplateDisplay', {
                    ids: ids
                }).then(function (data) {
                    if (data.Status === 1) {
                        layer.msg('删除成功');
                        pub.deviceSelStateChangeCon();
                    } else {
                        layer.msg('删除失败');
                    }
                }).fail(function (err) {
                    layer.msg(err);
                });
            });
        },
        //模版发布
        templatePub: function templatePub(ids) {
            layer.confirm('确定发布吗', {
                btn: ['取消', '确定']
            }, function () {
                layer.msg('不发布');
            }, function () {
                util.sendPostReq('/TemplateApi/SendCommand', {
                    ids: ids
                }).then(function (data) {
                    if (data.Status === 1) {
                        layer.alert(data.Message);
                        pub.deviceSelStateChangeCon();
                    } else {
                        layer.alert(data.Message);
                    }
                }).fail(function (err) {
                    layer.msg(err);
                });
            });
        },
        //模版撤销
        backoutPub: function backoutPub(ids) {
            layer.confirm('确定撤销发布吗', {
                btn: ['取消', '确定']
            }, function () {
                layer.msg('不撤销');
            }, function () {
                util.sendPostReq('/TemplateApi/SendCommand', {
                    sendType: 2,
                    ids: ids
                }).then(function (data) {
                    if (data.Status === 1) {
                        layer.alert(data.Message);
                        pub.deviceSelStateChangeCon();
                    } else {
                        layer.alert(data.Message);
                    }
                }).fail(function (err) {
                    layer.msg(err);
                });
            });
        },
        //获取被占用的最大排序
        getOccupiedMaxIndex: function getOccupiedMaxIndex(displayID) {
            util.sendPostReq('/TemplateApi/GetMaxIndex', {
                displayID: displayID
            }).then(function (data) {
                if (data.Status) {
                    //排序输入框->设置该为最大值
                    $('#taxis').val(data.Data);
                } else {
                    layer.msg(data.Message);
                }
            });
        },
        //获取被占用的设备的排序val
        getOccupiedIndex: function getOccupiedIndex(displayID) {
            util.sendPostReq('/TemplateApi/GetUseIndexs', {
                displayID: displayID
            }).then(function (data) {
                if (data.Status) {

                    //触发taxisVal事件
                    pub.compareVal('#taxis', data.Data);
                    //$('#taxis').trigger('taxisVal', data.Data);
                } else {
                    layer.msg(data.Message);
                }
            });
        },
        //分页
        laypageEvent: function laypageEvent(pageNum, displayId, pageSize) {
            layui.use(['laypage'], function () {
                var laypage = layui.laypage;
                laypage({
                    cont: 'log-view-paging',
                    pages: pageNum,
                    groups: 4,
                    skin: '#e6e6e6',
                    last: '尾页',
                    prev: '<em><i class="icon-prev"></i></em>',
                    next: '<em><i class="icon-next"></i></em>',
                    jump: function jump(obj, first) {
                        if (!first) {
                            var loadTips = layer.load(2, {
                                shade: 0.02
                            });
                            //根据页数obj.curr发送对应的请求
                            util.sendPostReq('/TemplateApi/GetTemplateDisplayList', {
                                startRow: (obj.curr - 1) * pageSize,
                                pageSize: pageSize,
                                displayId: displayId
                            }).then(function (data) {
                                // console.log('data',data);
                                if (data.Status) {
                                    var res = doT.template($('#template-list').html())(data.Data.List);
                                    $('#tmplTableCon').html(res);
                                } else {
                                    layer.msg(data.Message);
                                }
                            }).always(function () {
                                layer.close(loadTips);
                            });
                        }
                    }
                });
            });
        },
        //输入框提示信息
        formTips: function formTips(selector, con) {
            var tips = void 0;
            $(selector).off();
            $(selector).on('focus', function () {
                //同时多次点击执行一次
                tips = layer.tips(con, this, { time: 0 });
            });
            $(selector).on('blur', function () {
                //同时多次点击执行一次
                layer.close(tips);
            });
        },
        //绑定对应模版的子模版
        bindToChildTmpl: function bindToChildTmpl(id) {
            // $('.js-child-tmpl').hide();
            if (id && pub.templateData) {
                var $childTmpl = $('.js-child-tmpl'),
                    $childTmplName = $('#childTmplName');

                var arr = pub.templateData.filter(function (item) {
                    return item.templateID == id;
                });
                if (arr[0].value) {
                    $childTmplName.attr('lay-verify', 'required');
                    var str = $('#select-child-tmpl').html();
                    var res = doT.template(str)(arr[0]);
                    $childTmplName.html(res);
                    $childTmpl.show();
                } else {
                    $childTmplName.removeAttr('lay-verify');
                    $childTmplName.val('');
                    $childTmpl.hide();
                }
                form.render();
            }
        },
        //获取单个设备和模版信息,并绑定到表单
        getInfOToModal: function getInfOToModal(rowId) {
            var loadTips = layer.load(2, { shade: 0.02 });
            util.sendPostReq('/TemplateApi/GetTemplateDisplay', {
                ID: rowId
            }).then(function (data) {
                if (data.Status === 1) {
                    //绑定数据到弹框
                    pub.bindValueToModal(data.Data);
                    //设置排序输入框的值不能重复提示功能
                    pub.getOccupiedIndex(data.Data.DisplayID);
                    //rowId：设置点击确定是更新数据
                    pub.sendLayuiFormVal(form, rowId);
                    //根据当前模版ID来判断是否有子模版
                    pub.bindToChildTmpl(data.Data.TemplateID);
                } else {
                    layer.msg(data.Message + ',未获取到当前设备信息');
                }
            }).fail(function (err) {
                layer.msg(err);
            }).always(function () {
                layer.close(loadTips);
            });
        },
        //根据设备选择框的值来判定当前刷新的数据列表
        deviceSelStateChangeCon: function deviceSelStateChangeCon() {
            var curState = $('#deviceSelect').val();
            if (curState === 'all') {
                pub.tableConDataInit();
            } else {
                var id = ~~curState;
                pub.tableConDataInit({
                    displayId: id
                });
            }
        },
        //动态显示撤销发布按钮
        changeMenu: function changeMenu() {
            var $tableCon = $('.js-tableCon').find('.checkbox.checkbox-checked');
            var $backoutPub = $('.js-backoutPub');
            var rows = $tableCon.closest('.row').get();
            var sendArr = rows.map(function (row) {
                return row.getAttribute('data-is-send');
            });
            if (sendArr.length === 0) {
                $backoutPub.addClass('hide').removeClass('show');
                return;
            }
            //如果0不存在，则都是已发布状态
            if (sendArr.indexOf('0') == -1) {
                $backoutPub.addClass('show').removeClass('hide');
            } else {
                $backoutPub.addClass('hide').removeClass('show');
            }
        }

    };
    //列表中的事件
    var tableConEvent = function tableConEvent() {
        var $tableCon = $('.js-equipTable .js-tableCon');
        //tableCon列表点击效果
        $tableCon.on('click', function (e) {
            var tar = e.target,
                $parTag = $(tar.parentNode),
                flag = $parTag.hasClass('row');
            if (flag) {
                //点击当前row时->移除全部row复选框的效果
                $(this).find('.checkbox').removeClass('checkbox-checked');
                $('.js-allEquip').removeClass('checkbox-checked');
                //点击时背景改变
                if ($parTag.attr('switch')) {
                    //取消选中
                    $parTag.removeAttr('switch');
                    $parTag.removeClass('is-active').siblings().removeClass('is-active');
                    $parTag.find('.checkbox').removeClass('checkbox-checked');
                } else {
                    //选中
                    $parTag.attr('switch', 'on').siblings().removeAttr('switch');
                    $parTag.addClass('is-active').siblings().removeClass('is-active');
                    $parTag.find('.checkbox').addClass('checkbox-checked');
                }
            }
            //不管是选中还是取消选中都要判定是否显示或隐藏
            pub.changeMenu();
        });

        //列表的每个复选框 & row背景色改变
        $tableCon.on('click', '.checkbox', function (e) {
            $('.js-allEquip').removeClass('checkbox-checked');
            var $this = $(this),
                $row = $this.closest('.row');
            var flag = $(this).hasClass('checkbox-checked');
            if (!flag) {
                //选中
                $this.addClass('checkbox-checked');
                $row.addClass('is-active');
            } else {
                //取消
                $this.removeClass('checkbox-checked');
                $row.removeClass('is-active');
            }
            pub.changeMenu();
            e.stopPropagation();
        });
        //全选按钮
        $('.js-allEquip').on('click', function (e) {
            var tar = e.target,
                par = tar.parentNode;
            var $tableCon = $(par).siblings('.js-tableCon');
            var $checkboxs = $tableCon.find('.checkbox');
            var $rows = $tableCon.find('.row');
            var flag = $(this).hasClass('checkbox-checked');
            if (!flag) {
                //$(this)-> 控制的是当前按钮的开关效果
                $(this).addClass('checkbox-checked');
                $rows.addClass('is-active');
                $checkboxs.addClass('checkbox-checked');
                $rows.removeAttr('switch');
            } else {
                $(this).removeClass('checkbox-checked');
                $checkboxs.removeClass('checkbox-checked');
                $rows.removeClass('is-active');
            }
        });

        //列表中的编辑按钮
        $tableCon.on('click', '.js-edit', function (e) {
            var curElem = $(this).closest('.row');
            var rowId = parseInt(curElem.attr('data-rowid'));
            $modTitleBig.html('模版编辑');
            //获取单个设备和模版信息,并绑定到表单
            //pub.getInfOToModal(rowId);
            //loading
            var loadTips = layer.load(2, { shade: 0.02 });
            //get模版列表
            util.sendPostReq('/TemplateApi/GetTemplateList', {
                pageSize: 1000
            }).then(function (data) {
                if (data.Status === 1 && data.Data) {
                    //doT模板引擎绑定数据
                    var tmplVal = $('#select-tmpl').html();
                    var res = doT.template(tmplVal)(data.Data);
                    $('#templateName').html(res);
                    //表单更新
                    form.render();
                    //赋值给templateData
                    pub.templateData = data.Data;
                } else {
                    layer.msg('获取模版列表失败,请刷新');
                }
            }).then(function () {
                //获取设备列表
                pub.getModalData('/TemplateApi/GetDisplayList', 'select-device', 'deviceName');
            }).then(function () {
                //获取单个设备和模版信息,并绑定到表单
                pub.getInfOToModal(rowId);
            }).fail(function (err) {
                layer.msg(err);
            }).always(function () {
                layer.close(loadTips);
            });
            $boxMaskBig.show();
            e.stopPropagation();
        });
        //列表中的删除按钮
        $tableCon.on('click', '.js-del', function (e) {
            var curElem = $(this).closest('.row');
            var tmplId = curElem.attr('data-rowid');
            pub.delData(tmplId);
            e.stopPropagation();
        });
    };
    //menu事件
    var menuEvent = function menuEvent() {
        //menu->模板分配
        $('.js-templateAllow').on('click', function () {
            //get template list
            pub.getModalData('/TemplateApi/GetTemplateList', 'select-tmpl', 'templateName', {
                pageSize: 1000
            });
            //get device list
            util.sendPostReq('/DisplayInfoes/SelectAllRecord', { isPrimary_select: 1 }).then(function (data) {
                if (data.length > 0) {
                    var tmplVal = $('#select-device').html();
                    var res = doT.template(tmplVal)(data);
                    $('#deviceName').html(res);
                    form.render();
                }
            }).fail(function (err) {
                layer.msg('' + err);
            });

            //给弹框的表单绑定layui事件->为了给后台发送表单数据
            pub.sendLayuiFormVal(form);
            $boxMaskBig.show();
        });
        //模版发布
        $('.js-templatePub').on('click', function () {
            //获取列表选中行
            var $tableCon = $('.js-tableCon').find('.checkbox.checkbox-checked');
            var checkLength = $tableCon.length;
            var rows = $tableCon.closest('.row').get();
            //选中的行的id集合
            var ids = rows.map(function (row) {
                return row.getAttribute('data-rowid');
            }).join(',');
            if (checkLength === 0) {
                layer.alert('未选中模版');
            } else {
                pub.templatePub(ids);
            }
        });

        $('.js-backoutPub').on('click', function () {
            var $tableCon = $('.js-tableCon').find('.checkbox.checkbox-checked');
            var checkLength = $tableCon.length;
            var rows = $tableCon.closest('.row').get();
            //选中的行的id集合
            var ids = rows.map(function (row) {
                return row.getAttribute('data-rowid');
            }).join(',');
            if (checkLength === 0) {
                layer.alert('未选中模版');
            } else {
                pub.backoutPub(ids);
            }
        });
        //menu->删除按钮
        $('.js-menuDel').on('click', function (e) {
            //获取列表选中行
            var $tableCon = $('.js-tableCon').find('.checkbox.checkbox-checked');
            var checkLength = $tableCon.length;
            var rows = $tableCon.closest('.row').get();
            //选中的行的id集合
            var ids = rows.map(function (row) {
                return row.getAttribute('data-rowid');
            }).join(',');
            if (checkLength === 0) {
                layer.alert('未选中行');
            } else {
                pub.delData(ids);
            }
        });
    };
    //弹框中的事件
    var modalEvent = function modalEvent() {
        var $reset = $('.js-modal-tmplManage .js-reset'),
            $checkboxList = $('.js-checkbox-list');

        //多选框->周几
        $checkboxList.on('click', 'div', function (e) {
            var $this = $(this),
                $selfChild = $this.children(),
                $checkAllChild = $this.siblings('.js-checkAll').children(),
                $sibChilds = $this.siblings().children(),
                isExist = $selfChild.hasClass('checkbox-checked'),
                isCheckAll = $this.hasClass('js-checkAll');
            console.log(111);
            //普通
            if (isExist) {
                console.log(2222);
                $checkAllChild.removeClass('checkbox-checked');
                $selfChild.removeClass('checkbox-checked');
            } else {
                $selfChild.addClass('checkbox-checked');
            }
            //全选
            if (isCheckAll && !isExist) {
                console.log(333);
                $sibChilds.addClass('checkbox-checked');
            } else if (isCheckAll && isExist) {
                $sibChilds.removeClass('checkbox-checked');
            }
            e.stopPropagation();
        });
        //排序输入框提示
        pub.formTips('#taxis', '最大值为255');
        //持续时间输入框提示
        pub.formTips('#holdTime', '单位：秒(/s)');
        //reset按钮
        $reset.on('click', function () {
            pub.backToDefaultVal();
        });
        //取消按钮
        $('.js-modal-tmplManage .cancel-btn').on('click', function () {
            var $curboxMask = $(this).closest('.mod-boxMask');
            $reset.trigger('click'); //触发重置事件
            $curboxMask.hide();
        });
        //确认按钮
        /*$('.js-modal-tmplManage .ensure-btn').on('click', function () {
            let $curboxMask = $(this).closest('.mod-boxMask');
            $curboxMask.hide();
        });*/
        //关闭按钮
        $('.js-modal-tmplManage .close-btn').on('click', function () {
            var $curboxMask = $(this).closest('.mod-boxMask');
            /*util.destroyScroll(radioScrollCount);*/
            $reset.trigger('click');
            // $curboxMask.find('.js-boxShadow').addClass('boxShadow');
            $curboxMask.hide();
        });
    };
    //日期插件初始化
    var jeDateInit = function jeDateInit() {
        //日期控件初始化 js-time-> 时分 js-times->时分秒  js-date->年月日
        $(".js-time").jeDate({
            format: "hh:mm"
        });

        var dateStart = {
            format: 'YYYY-MM-DD',
            minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            maxDate: '2099-06-30 23:59:59',
            choosefun: function choosefun(elem, datas) {
                dateEnd.minDate = datas; //开始日选好后，重置结束日的最小日期
            }
        };
        var dateEnd = {
            format: 'YYYY-MM-DD',
            minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            maxDate: '2099-06-30 23:59:59',
            choosefun: function choosefun(elem, datas) {
                dateStart.maxDate = datas; //开始日选好后，重置结束日的最小日期
            }
        };
        $('.js-dateStart').jeDate(dateStart);
        $('.js-dateEnd').jeDate(dateEnd);
    };
    //layui初始化
    var layuiInit = function layuiInit() {
        //使用layui's form
        var layUiform = layui.form();
        //设备选择的下拉框显示出来
        layUiform.render();
        //设备选择
        layUiform.on('select(deviceSelect)', function (data) {
            if (data.elem.value === 'all') {
                pub.tableConDataInit();
            } else {
                //请求单个设备的列表
                var id = ~~data.elem.value;
                pub.tableConDataInit({
                    displayId: id
                });
            }
        });
    };

    var render = function render() {
        getElem();
        //加载页面时初始化
        pub.tableConDataInit();
        //设备选择->请求设备列表
        util.sendPostReq('/DisplayInfoes/SelectAllRecord', { isPrimary_select: 1 }).then(function (data) {
            if (data.length > 0) {
                var tmplVal = $('#device-tmpl').html();
                var res = doT.template(tmplVal)(data);
                $('#deviceSelect').html(res);
                form.render();
            }
        }).fail(function (err) {
            layer.msg('' + err);
        });

        //绑定事件
        tableConEvent();
        menuEvent();
        layuiInit();
        modalEvent();
        jeDateInit();
    };

    return {
        render: render
    };
})(jQuery, util, layui, layer, doT);

/***/ }),
/* 6 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
Object.defineProperty(__webpack_exports__, "__esModule", { value: true });
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_0__flightData__ = __webpack_require__(1);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_1__templateManagement__ = __webpack_require__(5);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_2__deviceManagement__ = __webpack_require__(0);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_3__logManagement__ = __webpack_require__(2);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_4__permissManagement__ = __webpack_require__(4);
/* harmony import */ var __WEBPACK_IMPORTED_MODULE_5__messageRelease__ = __webpack_require__(3);







var indexRender = function ($, util) {
    var form = layui.form();
    //leftSideBar click
    var leftSideBarEvent = function leftSideBarEvent() {
        var $mainContent = $('.js-mainContent');
        $('.js-template').on('click', 'li', function () {
            var $this = $(this);
            $this.addClass('is-active').siblings().removeClass('is-active');
            //切换页面删除loadding层，以免造成切换页面之后loading层还在显示的问题
            $('.layui-layer-shade').remove();
            $('.layui-layer-loading').remove();
            //跳转页面
            var curAttr = $this.attr('data-href');
            switch (curAttr) {
                case 'flightData':
                    util.loadJsCss('Content/flightData.css');
                    util.loadJsCss('Content/modal-flightData.css');
                    util.bindHtmlToIndex('./mainTmpl/flightData.html', function (data) {
                        $mainContent.html(data);
                        __WEBPACK_IMPORTED_MODULE_0__flightData__["a" /* default */].render();
                    });
                    break;
                case 'tmplManage':
                    util.loadJsCss('Content/templateManagement.css');
                    util.loadJsCss('Content/modal-tmplManage-big.css');
                    util.bindHtmlToIndex('./mainTmpl/templateManagement.html', function (data) {
                        $mainContent.html(data);
                        __WEBPACK_IMPORTED_MODULE_1__templateManagement__["a" /* default */].render();
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
                        __WEBPACK_IMPORTED_MODULE_2__deviceManagement__["a" /* default */].render();
                    });
                    break;
                case 'msgPub':
                    util.loadJsCss('Content/messageRelease.css');
                    util.loadJsCss('Content/modal-msg.css');
                    util.bindHtmlToIndex('./mainTmpl/messageRelease.html', function (data) {
                        $mainContent.html(data);
                        __WEBPACK_IMPORTED_MODULE_5__messageRelease__["a" /* default */].render();
                    });
                    break;
                case 'perManage':
                    util.loadJsCss('Content/permissManagement.css');
                    util.loadJsCss('Content/modal-permiss.css');
                    util.bindHtmlToIndex('./mainTmpl/permissManagement.html', function (data) {
                        $mainContent.html(data);
                        __WEBPACK_IMPORTED_MODULE_4__permissManagement__["a" /* default */].render();
                    });
                    break;
                case 'logManage':
                    util.loadJsCss('Content/logManagement.css');
                    util.bindHtmlToIndex('./mainTmpl/logManagement.html', function (data) {
                        $mainContent.html(data);
                        __WEBPACK_IMPORTED_MODULE_3__logManagement__["a" /* default */].render();
                    });
                    break;
            }
        });
    };

    var init = function init() {
        leftSideBarEvent();
    };
    return {
        init: init
    };
}(jQuery, util);

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
        var form = layui.form();
        //首次加载获取任务代码数据->插入航班计划弹框中
        util.sendPostReq('/FlightDataApi/GetTaskCode', { pageSize: 1000 }).then(function (data) {
            if (data.Data.List.length > 0) {
                var res = doT.template($('#modal-task-code-tmpl').html())(data.Data.List);
                $('#task-code-d').html(res);
                $('#task-code-a').html(res);
                form.render();
            }
        });

        __WEBPACK_IMPORTED_MODULE_0__flightData__["a" /* default */].render();
    });
});

/***/ })
/******/ ]);