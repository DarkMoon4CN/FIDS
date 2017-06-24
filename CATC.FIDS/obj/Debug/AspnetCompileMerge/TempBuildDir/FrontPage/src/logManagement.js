export default (function logManage($, util){

    let scrollCount;
    //公共方法
    let pub = {
        setModalCon: function (){
            var a = [];
            $('.show .js-tableCon').find('.checkbox.checkbox-checked').parent().next().each(function (index, item){
                var cur = item.innerHTML;
                a.push(cur);
            });
            return a.join(', ');
        }
    };
    

    //航班数据表tableCon中的事件
    let logTableEvent = function (){
        let $tableCon = $('.js-tableCon');
        //tabCtrl-> 选项卡切换时iscroll初始化
        $('.js-tabCtrl').on('click', 'div',function (){
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
    let render =  function (){
        //局部滚动
        scrollCount = util.myScroll('.js-tableCon');
        //选项卡
        util.tabChange('.js-dataTable');
        //日志列表绑定事件
        logTableEvent();

    };
    return {
        render: render
    }
})(jQuery, util);

