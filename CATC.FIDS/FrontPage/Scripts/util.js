var util = (function ($) {
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
            if (parScrollHeight > parOffsetHeight) {
                var str1 = $item.html();
                str1 = '<div>' + str1 + '</div>';
                str1 += str1;
                $item.html(str1);
                $item.css({
                    overflow: 'hidden'
                });
                $item.children().css({
                    paddingBottom: num
                });
                var ChildBox1 = $item.children().eq(0);
                var childHeight = parseFloat(ChildBox1.height()) + num;
                $item.curH = 0;
                $item.childHeight = childHeight;
                moveHeight($item, speed);
            }
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
            var arr = [];
            $(option).each(function (index, item) {
                var obj = new IScroll(item, {
                    scrollbars: 'custom',
                    fadeScrollbars: false,
                    mouseWheel: true,
                    click: false,
                    interactiveScrollbars: true,
                    bounce: false,
                });
                arr[arr.length] = obj;
            });
            return arr;
        }
    }
    /**
     * 水平滚动
     * @param {*} option String类型 滚动对象的selector,比如'.js-scroll'
     */
    function myHScroll(option) {
        if (option) {
            var arr = [];
            $(option).each(function (index, item) {
                var obj = new IScroll(item, {
                    scrollbars: 'custom',
                    fadeScrollbars: false,
                    mouseWheel: true,
                    click: false,
                    interactiveScrollbars: true,
                    bounce: false,
                    scrollY: false,
                    scrollX: true,
                    startX: 0,

                });
                arr[arr.length] = obj;
            });
            return arr;
        }
    }
    // 销毁iscroll实例
    function destroyScroll(scrollArr) {
        if (scrollArr.length > 0) {
            scrollArr.forEach(function (item) {
                item.destroy();
            });
        }
    }

    /**
     * 选项卡切换
     *
     * @param selector String类型 例如 '.map'
     * @param tabClass String类型 例如 'is-click'
     * @param conClass String类型 例如 'show'
     */
    function tabChange(selector, tabClass, conClass) {
        tabClass = tabClass || 'is-click';
        conClass = conClass || 'show';
        $(selector).each(function (index, item) {
            var tabFirst = $(item).children().eq(0);
            var aLis = tabFirst.children();
            aLis.each(function (index, li) {
                li.index = index;
                li.onclick = function () {
                    $(this).addClass(tabClass).siblings().removeClass(tabClass);

                    var flag = $(this).index();
                    var pList = $(this).parent().nextAll();
                    pList.each(function (index, p) {
                        index === flag ? $(p).addClass(conClass) : $(p).removeClass(conClass);
                    });
                }
            });
        });

    }
    /**
     * 获取本地模板页面绑定到首页
     * @param {*} url 
     * @param {*} callback 成功后的回调
     */
    function bindHtmlToIndex(url, callback) {
        $.ajax({
            url: url,
            cache: false
        }).then(function (data) {
            callback(data);
        });
    }
    /**
     * 发送post请求
     * @param {*} url  请求链接
     * @param {*} data 请求主体
     * @param {*} type 请求类型
     */
    function sendPostReq(url, data, type) {
        data = data || {};
        type = type || 'POST';
        return $.ajax({
            url: 'http://192.9.200.61' + url,
            type: type,
            data: data,
            dataType: 'json',
            cache: false,
            /*timeout: 30000*/
        });
    }
    /**
     * 函数节流
     * @param {*} delay 延时
     * @param {*} method 执行方法
     * @param {*} args 方法所需参数
     */
    function throttle(delay, method, ...args) {
        clearTimeout(method.tId);
        method.tId = setTimeout(function () {
            method.apply(null, args);
        }, delay);
    }
    /**
     * 按需加载js和vss
     * @param {*} url 文件路径
     * @param {*} callback 加载成功的回调函数
     */
    function loadJsCss(url, callback) { // 非阻塞的加载 后面的js会先执行
        var isJs = /\/.+\.js($|\?)/i.test(url) ? true : false;

        function onloaded(script, callback) { //绑定加载完的回调函数
            if (script.readyState) { //ie
                script.attachEvent('onreadystatechange', function () {
                    if (script.readyState == 'loaded' || script.readyState == 'complete') {
                        script.className = 'loaded';
                        callback && callback.constructor === Function && callback();
                    }
                });
            } else {
                script.addEventListener('load', function () {
                    script.className = "loaded";
                    callback && callback.constructor === Function && callback();
                }, false);
            }
        }
        if (!isJs) { //加载css
            var links = document.getElementsByTagName('link');
            for (var i = 0; i < links.length; i++) { //是否已加载
                if (links[i].href.indexOf(url) > -1) {
                    return;
                }
            }
            var link = document.createElement('link');
            link.type = "text/css";
            link.rel = "stylesheet";
            link.href = url;
            var head = document.getElementsByTagName('head')[0];
            head.appendChild(link);
        } else { //加载js
            var scripts = document.getElementsByTagName('script');
            for (var i = 0; i < scripts.length; i++) { //是否已加载
                if (scripts[i].src.indexOf(url) > -1 && callback && (callback.constructor === Function)) {
                    //已创建script
                    if (scripts[i].className === 'loaded') { //已加载
                        callback();
                    } else { //加载中
                        onloaded(scripts[i], callback);
                    }
                    return;
                }
            }
            var script = document.createElement('script');
            script.type = "text/javascript";
            script.src = url;
            document.body.appendChild(script);
            onloaded(script, callback);
        }
    }
    return {
        marquee: marquee,
        dynamicCalcRem: dynamicCalcRem,
        accordionPanel: accordionPanel,
        myScroll: myScroll,
        myHScroll: myHScroll,
        destroyScroll: destroyScroll,
        tabChange: tabChange,
        bindHtmlToIndex: bindHtmlToIndex,
        sendPostReq: sendPostReq,
        throttle: throttle,
        loadJsCss: loadJsCss
    }
})(jQuery);

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
            var index = arguments[1],
                content = ary[index];
            content = content || '00';
            return content;
        });
    }
    pro.formatTime = formatTime;
})(String.prototype);