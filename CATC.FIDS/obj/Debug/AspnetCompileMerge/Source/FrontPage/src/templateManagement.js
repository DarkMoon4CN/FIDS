export default (function templateManage($, util, layui, layer, doT) {

    
    //为了不重复执行myScroll方法，因为多次执行myScroll会创建多个滚动条
    let tableConCount;
    let $boxMaskBig  = null, $modTitleBig = null;
    

    let form = layui.form();

    let getElem = function(){
        $boxMaskBig  = $('.modal-tmplManage-big .mod-boxMask');
        $modTitleBig = $boxMaskBig.find('.js-modalBigTitle');
    }

    //公共方法
    let pub = {
        templateData: null,
        timer: null,
        //格式化时间
        formatDate: function (dataStr, mode) {
            let cur = new Date(Number(/(-?\d+)/.exec(dataStr)[1]));
            if (mode === 'date') {
                return cur.toLocaleDateString().formatTime('{0}-{1}-{2}');
            }
            if (mode === 'time') {
                return /(\d{2}:\d{2}):\d{2}/.exec(cur)[1];
            }
        },
        //确认弹框中的内容
        setModalCon: function () {
            var a = [];
            $('.show .js-tableCon').find('.checkbox.checkbox-checked').parent().next().each(function (index, item) {
                var cur = item.innerHTML;
                a.push(cur);
            });
            return a.join(', ');
        },
        //模版分配弹框发送表单信息
        sendLayuiFormVal: function (form, flagId) {
            let obj = {};
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
                let weekVal = pub.getWeeksVal();
                if (weekVal.length > 0) {
                    obj.weeks = weekVal;
                } else {
                    layer.alert('周：请至少选中一个');
                    return;
                }
                //持续时间
                // obj.intervalSecond = parseFloat(formObj.holdTime) * 60; 
                obj.intervalSecond = ~~(formObj.holdTime);
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
                let isEmptyObject = $.isEmptyObject(obj);
                if (!isEmptyObject) {
                    let loadTips = layer.load(2, {
                        shade: 0.02
                    });
                    //发送添加请求
                    util.sendPostReq('/TemplateApi/AddTemplateDisplay', obj)
                        .then(function (data) {
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
        //模版管理列表初始化
        tableConDataInit: function (reqData) {
            reqData = reqData || {};
            reqData.pageSize = 10;
            //如果选中了全选按钮，每次初始化应该去掉选中效果
            $('.js-allEquip').hasClass('checkbox-checked') ? $('.js-allEquip').removeClass('checkbox-checked'):null;
            let loadTips = layer.load(2, {
                shade: 0.02
            });
            //请求列表
            util.sendPostReq('/TemplateApi/GetTemplateDisplayList', reqData)
                .then(function (data) {
                    if (data.Status) {
                        if (data.Data.List.length > 0) {
                            //根据Index排正序
                            data.Data.List.sort(function(a,b){
                                return a.Index - b.Index;
                            });
                            // console.log(data);
                            let res = doT.template($('#template-list').html())(data.Data.List);
                            $('#tmplTableCon').html(res);
                            //避免iscroll重复加载
                            tableConCount ? util.destroyScroll(tableConCount) : null;
                            //局部滚动
                            tableConCount = util.myScroll('.js-tableCon');
                            //分页 & laypageNum页数
                            let laypageNum = Math.ceil(data.Data.Count / reqData.pageSize);
                            pub.laypageEvent(laypageNum, reqData.displayId,reqData.pageSize);
                        } else {
                            $('#tmplTableCon').html('');

                        }

                    } else {
                        layer.msg(data.Message);
                    }
                })
                .fail(function (err) {
                    layer.msg('请求发送错误');
                })
                .always(function () {
                    layer.close(loadTips);
                });
        },
        //ajax获取值并绑定到弹框
        getModalData: function (url, dotTmplID, tmplID, reqData) {
            //loading
            let loadTips = layer.load(2, {
                shade: 0.02
            });
            //tmpl's list
            util.sendPostReq(url, reqData)
                .then(function (data) {
                    if (data.Status === 1 && data.Data) {
                        let tmplVal = $('#' + dotTmplID).html();
                        let res = doT.template(tmplVal)(data.Data);
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
        bindValueToModal: function (data) {
            window.clearTimeout(pub.timer);
            //有极小的概率发生绑定数据到弹框之前获取不到设备列表，所以先form.render
            pub.timer = setTimeout(function () {
                //模版名称
                $('#templateName option[value="' + data.TemplateID + '"]').prop('selected', 'selected');
                //设备名称
                $('#deviceName option[value=' + data.DisplayID + ']').prop('selected', 'selected');
                //子模版
                $('#childTmplName').find('option[value="'+data.DataValue+'"]').prop('selected','selected'); 
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
            let weeks = data.Weeks.split(',');
            pub.changeWeekState(weeks);
            //覆盖和广告
            !!data.IsAdvert ? $('#ad').prop('checked','checked') : null;
            !!data.IsCover ? $('#cover').prop('checked','checked') : null;
            !!data.IsSend ? $('#publish').prop('checked','checked') : null;
      
            form.render();
        },
        //点击确认，取消，close按钮->弹框重置到默认值
        backToDefaultVal: function () {
            //weeks周
            $('.js-checkbox-list .checkContain').children().addClass('checkbox-checked');
            //taxis排序
            $('#taxis').css('border', '');
            //弹框title
            $modTitleBig.html('模版分配');
        },
        //改变'周'复选框样式
        changeWeekState: function (weeks) {
            let $checkboxList = $('.js-checkbox-list');
            //首先全部清除样式
            $checkboxList.find('.checkContain').children().eq(0).removeClass('checkbox-checked');
            //然后再添加相应样式
            weeks.forEach(function (num) {
                $checkboxList.find('.checkContain[data-name=' + num + ']').children().eq(0).addClass('checkbox-checked');
            });
            if(weeks.length == 7) {
                $('.js-checkbox-list').find('.js-checkAll').children().eq(0).addClass('checkbox-checked');
            }
        },
        //获取选中的week值
        getWeeksVal: function () {
            let arr = [];
            let $list = $('.js-checkbox-list .checkContain').not($('.js-checkAll')).find('.checkbox-checked').parent();
            $list.each(function (i, item) {
                arr.push(item.getAttribute('data-name'));
            });

            return arr.join();
        },
        //实时比较输入框的值并显示不同样式
        compareVal: function (ele, data) {
            let $ele = $(ele);
            data = data.split(',');
            let curVal = $ele.value;
            if (data.indexOf(curVal) >= 0) {
                $ele.css('border', '1px solid #f00');
                $('.input-tips').css('display', 'block');
            } else {
                $ele.css('border', '');
                $('.input-tips').css('display', 'none');
            }
            //实时输入值也进行比较
            $ele.on('input', function () {
                let val = this.value;
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
        delData: function (ids) {
            layer.confirm('确定删除吗', {
                btn: ['取消', '确定']
            }, function () {
                layer.msg('不删除');
            }, function () {
                util.sendPostReq('/TemplateApi/DelTemplateDisplay', {
                        ids: ids
                    })
                    .then(function (data) {
                        if (data.Status === 1) {
                            layer.msg('删除成功');
                            pub.deviceSelStateChangeCon();
                        } else {
                            layer.msg('删除失败');
                        }

                    })
                    .fail(function (err) {
                        layer.msg(err);
                    });
            });
        },
        //模版发布
        templatePub: function (ids) {
            layer.confirm('确定发布吗', {
                btn: ['取消', '确定']
            }, function () {
                layer.msg('不发布');
            }, function () {
                util.sendPostReq('/TemplateApi/SendCommand', {
                        ids: ids
                    })
                    .then(function (data) {
                        if (data.Status === 1) {
                            layer.alert(data.Message);
                            pub.deviceSelStateChangeCon();
                        } else {
                            layer.alert(data.Message);
                        }
                    })
                    .fail(function (err) {
                        layer.msg(err);
                    });
            });
        },
        //模版撤销
        backoutPub: function(ids){
            layer.confirm('确定撤销发布吗', {
                btn: ['取消', '确定']
            }, function () {
                layer.msg('不撤销');
            }, function () {
                util.sendPostReq('/TemplateApi/SendCommand', {
                        sendType: 2,
                        ids: ids
                    })
                    .then(function (data) {
                        if (data.Status === 1) {
                            layer.alert(data.Message);
                            pub.deviceSelStateChangeCon();
                        } else {
                            layer.alert(data.Message);
                        }
                    })
                    .fail(function (err) {
                        layer.msg(err);
                    });
            });
        },
        //获取被占用的最大排序
        getOccupiedMaxIndex: function (displayID) {
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
        getOccupiedIndex: function (displayID) {
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
        laypageEvent: function (pageNum, displayId, pageSize) {
            layui.use(['laypage'], function () {
                let laypage = layui.laypage;
                laypage({
                    cont: 'log-view-paging',
                    pages: pageNum,
                    groups: 4,
                    skin: '#e6e6e6',
                    last: '尾页',
                    prev: '<em><i class="icon-prev"></i></em>',
                    next: '<em><i class="icon-next"></i></em>',
                    jump: function (obj, first) {
                        if (!first) {
                            let loadTips = layer.load(2, {
                                shade: 0.02
                            });
                            //根据页数obj.curr发送对应的请求
                            util.sendPostReq('/TemplateApi/GetTemplateDisplayList', {
                                    startRow: (obj.curr - 1) * pageSize,
                                    pageSize: pageSize,
                                    displayId: displayId
                                })
                                .then(function (data) {
                                    // console.log('data',data);
                                    if (data.Status) {
                                        let res = doT.template($('#template-list').html())(data.Data.List);
                                        $('#tmplTableCon').html(res);
                                    } else {
                                        layer.msg(data.Message);
                                    }
                                })
                                .always(function () {
                                    layer.close(loadTips);
                                });
                        }
                    }
                });

            });
        },
        //输入框提示信息
        formTips: function (selector,con) {
            let tips;
            $(selector).off();
            $(selector).on('focus', function () {
                //同时多次点击执行一次
                tips = layer.tips(con,this,{time:0});
            });
            $(selector).on('blur', function () {
                //同时多次点击执行一次
                layer.close(tips);
            });
        },
        //绑定对应模版的子模版
        bindToChildTmpl: function(id){
            // $('.js-child-tmpl').hide();
            if(id && pub.templateData){
                let $childTmpl     = $('.js-child-tmpl'),
                    $childTmplName = $('#childTmplName');

                let arr = pub.templateData.filter(function(item){
                    return item.templateID == id;
                });
                if(arr[0].value){
                    $childTmplName.attr('lay-verify','required');
                    let str = $('#select-child-tmpl').html();
                    let res = doT.template(str)(arr[0]);
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
        getInfOToModal: function (rowId) {
                let loadTips = layer.load(2, {shade: 0.02});
                util.sendPostReq('/TemplateApi/GetTemplateDisplay', {
                        ID: rowId
                    })
                    .then(function (data) {
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
                    })
                    .fail(function (err) {
                        layer.msg(err);
                    })
                    .always(function(){
                        layer.close(loadTips);
                    });

        },
        //根据设备选择框的值来判定当前刷新的数据列表
        deviceSelStateChangeCon: function(){
            let curState = $('#deviceSelect').val();
            if(curState === 'all'){
                pub.tableConDataInit();
            } else {
                let id = ~~curState;
                pub.tableConDataInit({
                    displayId: id
                });
            }
        },
        //动态显示撤销发布按钮
        changeMenu: function(){
            let $tableCon = $('.js-tableCon').find('.checkbox.checkbox-checked');
            let $backoutPub = $('.js-backoutPub');
            let rows = $tableCon.closest('.row').get();
            let sendArr = rows.map(function (row) {
                return row.getAttribute('data-is-send');
            });
            if(sendArr.length === 0){
                $backoutPub.addClass('hide').removeClass('show');
                return;
            }
            //如果0不存在，则都是已发布状态
            if(sendArr.indexOf('0') == -1){
                $backoutPub.addClass('show').removeClass('hide');
            } else {
                $backoutPub.addClass('hide').removeClass('show');
            }
        }

    };
    //列表中的事件
    let tableConEvent = function () {
        let $tableCon = $('.js-equipTable .js-tableCon');
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
            let $this = $(this),
                $row = $this.closest('.row');
            let flag = $(this).hasClass('checkbox-checked');
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
            let curElem = $(this).closest('.row');
            let rowId   = parseInt(curElem.attr('data-rowid'));
            $modTitleBig.html('模版编辑');
            //获取单个设备和模版信息,并绑定到表单
            //pub.getInfOToModal(rowId);
            //loading
            let loadTips = layer.load(2, {shade: 0.02});
            //get模版列表
            util.sendPostReq('/TemplateApi/GetTemplateList', {
                    pageSize: 1000
                })
                .then(function (data) {
                    if (data.Status === 1 && data.Data) {
                        //doT模板引擎绑定数据
                        let tmplVal = $('#select-tmpl').html();
                        let res = doT.template(tmplVal)(data.Data);
                        $('#templateName').html(res);
                        //表单更新
                        form.render();
                        //赋值给templateData
                        pub.templateData = data.Data;
                    } else {
                        layer.msg('获取模版列表失败,请刷新');
                    }
                })
                .then(function () {
                    //获取设备列表
                    pub.getModalData('/TemplateApi/GetDisplayList', 'select-device', 'deviceName');
                })
                .then(function () {
                    //获取单个设备和模版信息,并绑定到表单
                    pub.getInfOToModal(rowId);
                })
                .fail(function (err) {
                    layer.msg(err);
                }).always(function () {
                    layer.close(loadTips);
                });
            $boxMaskBig.show();
            e.stopPropagation();
        });
        //列表中的删除按钮
        $tableCon.on('click', '.js-del', function (e) {
            let curElem = $(this).closest('.row');
            let tmplId  = curElem.attr('data-rowid');
            pub.delData(tmplId);
            e.stopPropagation();
        });
    };
    //menu事件
    let menuEvent = function () {
        //menu->模板分配
        $('.js-templateAllow').on('click', function () {
            //get template list
            pub.getModalData('/TemplateApi/GetTemplateList', 'select-tmpl', 'templateName', {
                pageSize: 1000
            });
            //get device list
            util.sendPostReq('/DisplayInfoes/SelectAllRecord', {isPrimary_select: 1})
                .then(function (data) {
                    if (data.length > 0) {
                        let tmplVal = $('#select-device').html();
                        let res = doT.template(tmplVal)(data);
                        $('#deviceName').html(res);
                        form.render();
                    }
                }).fail(function (err) {
                    layer.msg(''+ err);
                });

            //给弹框的表单绑定layui事件->为了给后台发送表单数据
            pub.sendLayuiFormVal(form);
            $boxMaskBig.show();
        });
        //模版发布
        $('.js-templatePub').on('click', function () {
            //获取列表选中行
            let $tableCon = $('.js-tableCon').find('.checkbox.checkbox-checked');
            let checkLength = $tableCon.length;
            let rows = $tableCon.closest('.row').get();
            //选中的行的id集合
            let ids = rows.map(function (row) {
                return row.getAttribute('data-rowid')
            }).join(',');
            if (checkLength === 0) {
                layer.alert('未选中模版');
            } else {
                pub.templatePub(ids);
            }
        });

        $('.js-backoutPub').on('click', function(){
            let $tableCon = $('.js-tableCon').find('.checkbox.checkbox-checked');
            let checkLength = $tableCon.length;
            let rows = $tableCon.closest('.row').get();
            //选中的行的id集合
            let ids = rows.map(function (row) {
                return row.getAttribute('data-rowid')
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
            let $tableCon = $('.js-tableCon').find('.checkbox.checkbox-checked');
            let checkLength = $tableCon.length;
            let rows = $tableCon.closest('.row').get();
            //选中的行的id集合
            let ids = rows.map(function (row) {
                return row.getAttribute('data-rowid')
            }).join(',');
            if (checkLength === 0) {
                layer.alert('未选中行');
            } else {
                pub.delData(ids);
            }
        });
    };
    //弹框中的事件
    let modalEvent = function () {
        let $reset = $('.js-modal-tmplManage .js-reset'),
            $checkboxList = $('.js-checkbox-list');
                
        //多选框->周几
        $checkboxList.on('click', 'div', function (e) {
            let $this = $(this),
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
        pub.formTips('#taxis','最大值为255');
        //持续时间输入框提示
        pub.formTips('#holdTime','单位：秒(/s)');
        //reset按钮
        $reset.on('click', function () {
            pub.backToDefaultVal();
        });
        //取消按钮
        $('.js-modal-tmplManage .cancel-btn').on('click', function () {
            let $curboxMask = $(this).closest('.mod-boxMask');
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
    let jeDateInit = function () {
        //日期控件初始化 js-time-> 时分 js-times->时分秒  js-date->年月日
        $(".js-time").jeDate({
            format: "hh:mm",
        });

        let dateStart = {
            format: 'YYYY-MM-DD',
            minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            maxDate: '2099-06-30 23:59:59',
            choosefun: function (elem, datas) {
                dateEnd.minDate = datas; //开始日选好后，重置结束日的最小日期
            }
        };
        let dateEnd = {
            format: 'YYYY-MM-DD',
            minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            maxDate: '2099-06-30 23:59:59',
            choosefun: function (elem, datas) {
                dateStart.maxDate = datas; //开始日选好后，重置结束日的最小日期
            }
        };
        $('.js-dateStart').jeDate(dateStart);
        $('.js-dateEnd').jeDate(dateEnd);
    };
    //layui初始化
    let layuiInit = function () {
        //使用layui's form
        let layUiform = layui.form();
        //设备选择的下拉框显示出来
        layUiform.render();
        //设备选择
        layUiform.on('select(deviceSelect)', function (data) {
            if (data.elem.value === 'all') {
                pub.tableConDataInit();
            } else {
                //请求单个设备的列表
                let id = ~~data.elem.value;
                pub.tableConDataInit({
                    displayId: id
                });
            }
        });
    };

    let render = function () {
        getElem();
        //加载页面时初始化
        pub.tableConDataInit();
        //设备选择->请求设备列表
        util.sendPostReq('/DisplayInfoes/SelectAllRecord', {isPrimary_select: 1})
                .then(function (data) {
                    if (data.length > 0) {
                        let tmplVal = $('#device-tmpl').html();
                        let res = doT.template(tmplVal)(data);
                        $('#deviceSelect').html(res);
                        form.render();
                    }
                }).fail(function (err) {
                    layer.msg(''+ err);
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
    }

})(jQuery, util, layui, layer, doT);