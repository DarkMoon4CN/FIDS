$(function (){
    util.dynamicCalcRem(1920);
    $('.js-templateStyle').children().each(function (index, item){
        //设置点击背景
        $(item).on('click', function (e){
            $(this).addClass('is-active').siblings().removeClass('is-active');
            //改变风格颜色
            if($(this).attr('data-style') === 'blue'){
                $('.js-category').removeClass('blackStyle classicStyle').addClass('blueStyle');
            }
            if($(this).attr('data-style') === 'black'){
                $('.js-category').removeClass('blueStyle classicStyle').addClass('blackStyle');
            }
            if($(this).attr('data-style') === 'classic'){
                $('.js-category').removeClass('blueStyle blackStyle').addClass('classicStyle');
            }
        })
    });
});