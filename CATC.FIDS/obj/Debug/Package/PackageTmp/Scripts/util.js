(function () {
    /**
     * 动态计算REM值  不兼容IE6~8
     * @param width Number类型 ：设计稿的尺寸，也就是参考尺寸
     * @param ele String类型
     */
    function dynamicCalcRem(width, ele) {
        ele = ele || "html";
        var desW = width,
            winW = document.documentElement.clientWidth || document.body.clientWidth;
        $(ele).css({
            fontSize: winW / desW * 100 + 'px'
        });
    }

    /**
     * marquee：跑马灯效果
     * @param ele String类型 ：是要滚动的元素的父级元素 例如 传入'.tips'
     * @param speed Number类型 ：滚动速度，默认是 10
     * @param num Number类型 ：这个是两个相同滚动元素的间隔距离，要配合padding或者margin使用
     */
    var timer;
    function marquee(ele, num, speed) {
        speed = speed || 10;
        num = num || 80;
        var parBox = $(ele),
            curH = 0;
        parBox.each(function (index, item) {
            var $item = $(item);
            var parScrollWidth = item.scrollWidth,
                parScrollHeight = item.scrollHeight,
                parOffsetWidth = $item.outerWidth(),
                parOffsetHeight = $item.outerHeight();
            //水平滚动
            if (parScrollWidth > parOffsetWidth) {
                var str = $item.html();
                str = '<div>' + str + '</div>';
                str += str;
                $item.html(str);
                $item.css({
                    whiteSpace: 'nowrap',
                    overflow: 'hidden'
                });
                $item.children().css({
                    display: 'inline-block',
                    paddingRight: num
                });
              
                var ChildBox = $item.children().eq(0);
                var childWidth = parseFloat(ChildBox.width()) + num;
                $item.curW = 0;
                $item.childWidth = childWidth;
                moveWidth($item, speed);
            }
            //垂直滚动
            //if (parScrollHeight > parOffsetHeight) {
            //    var str1 = $item.html();
            //    str1 = '<div>' + str1 + '</div>';
            //    str1 += str1;
            //    $item.html(str1);
            //    $item.css({
            //        overflow: 'hidden'
            //    });
            //    $item.children().css({
            //        paddingBottom: num
            //    });
            //    var ChildBox1 = $item.children().eq(0);
            //    var childHeight = parseFloat(ChildBox1.height()) + num;
            //    $item.curH = 0;
            //    $item.childHeight = childHeight;
            //    moveHeight($item, speed);
            //}
        });

        function moveWidth(item, speed) {
            item.scrollLeft(item.curW++);
            var curLeft = item.scrollLeft();
            curLeft >= item.childWidth ? item.curW = 0 : null;
            item.timer = setTimeout(function () {
                window.clearTimeout(item.timer);
                moveWidth(item, speed);
            }, speed);
        }

        function moveHeight(item, speed) {
            item.scrollTop(curH++);
            var curLeft = item.scrollTop();
            curLeft >= item.childHeight ? curH = 0 : null;
            var timer = setTimeout(function () {
                moveWidth(item, speed);
            }, speed);
        }
    }
    /**
     * 折叠面板
     * @param ele String类型 例如'.option h3'
     */
    function accordionPanel(ele) {
        if (ele && typeof ele === 'string') {
            $(ele).each(function (index, item) {
                $(item).on('click', function (e) {
                    $(e.target).next().slideToggle("slow");
                })
            });
        }
    }

    /**
     * 用iscroll控制局部滚动
     * @param option String类型 如'.main'
     * @returns {*|f}
     */
    function myScroll(option) {
        if (option) {
            return new IScroll(option, {
                scrollbars: 'custom',
                fadeScrollbars: true,
                mouseWheel: true,
                click: true,
                interactiveScrollbars: true
            });
        }
    }

    window.util = {
        marquee: marquee,
        dynamicCalcRem: dynamicCalcRem,
        accordionPanel: accordionPanel,
        myScroll: myScroll
    }
})();

(function (pro) {
    /**
     * 格式化时间  例如：new Date().toISOString().formatTime('{3}:{4}:{5} {0}/{1}/{2}')
     * @param template String类型 例如 '{3}:{4}:{5} {0}/{1}/{2}'
     * @returns {string}
     */
    function formatTime(template) {
        template = template || '{0}年{1}月{2}日 {3}时{4}分{5}秒';
        var ary = this.match(/\d+/g);
        return template.replace(/\{(\d)\}/g, function () {
            var index = arguments[1], content = ary[index];
            content = content || '00';
            return content;
        });
    }

    pro.formatTime = formatTime;
})(String.prototype);
