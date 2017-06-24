export default (function msgRelease($, util) {

    let scrollCount = null,
        scrollDevCount = null;

    //公共方法
    let pub = {
        setModalCon: function () {
            var a = [];
            $('.show .js-tableCon').find('.checkbox.checkbox-checked').parent().next().each(function (index, item) {
                var cur = item.innerHTML;
                a.push(cur);
            });
            return a.join(', ');
        },
        //根据一级切换显示不同的弹框和不同的menu
        showDiffModalAndMenu: function (ele) {
            let $menuAdd = $('.js-menu-add'),
                $menuPub = $('.js-msg-pub');

            let curAttr = $(ele).attr('data-flag');
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
        backInitState: function(ele){
            if ($(ele).closest('.js-modal-area')) {
                util.destroyScroll(scrollDevCount);
                //清除选中
                let $checkContain = $('.js-checkbox-list .checkContain');
                $checkContain.children().removeClass('checkbox-checked');
                $checkContain.removeClass('is-active');
            }
        },
        //改变设备列表多选框样式
        changeDeviceState: function (weeks) {
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
        //获取设备列表选中的设备值
        getDeviceVal: function () {
            let arr = [];
            let $list = $('.js-checkbox-list .checkContain').not($('.js-checkAll')).find('.checkbox-checked').parent();
            $list.each(function (i, item) {
                arr.push(item.getAttribute('data-name'));
            });

            return arr.join();
        },
    };

    //数据表tableCon中的事件
    let logTableEvent = function () {
        let $tableCon = $('.js-dataTable .js-tableCon');

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
            let $this = $(this),
                $row = $this.closest('.row');
            let flag = $(this).hasClass('checkbox-checked');
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
    let modalBtnEvent = function () {
        var $resetBtn = $('.modal-msg .js-reset');
        let $boxMask = $('.modal-msg .mod-boxMask');

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
    let menuEvent = function () {
        let $boxMask = $('.modal-msg .mod-boxMask');
        let $modalCon = $('.confirm .js-modalCon'),
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
            let tar = e.target,
                menuCon = $(tar).text();

        });
    };

    let dataTabsEvent = function () {

        $('.js-dataTabs .dataTabs').on('click', 'li', function (e) {
            let $this = $(this),
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
    let jeDateInit = function () {
        /*let date = {
            format: 'YYYY-MM-DD',
            // minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            zIndex: 9999,
            maxDate: '2099-06-30 23:59:59',
        };*/
        let dateTime = {
            format: 'YYYY-MM-DD hh:mm:ss',
            // minDate: $.nowDate(0),
            isinitVal: false,
            festival: false,
            ishmsVal: false,
            zIndex: 9999,
            maxDate: '2099-06-30 23:59:59',
        }
        $('.js-date-time').jeDate(dateTime);
        // $('.js-date').jeDate(date);
    };

    let render = function () {
        //使用layui's form
        let form = layui.form();
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
    }
})(jQuery, util);