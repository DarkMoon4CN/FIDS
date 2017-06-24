var gulp        = require('gulp'),
    browserSync = require('browser-sync').create();
var reload      = browserSync.reload;
// 静态服务器
/*实时监控*/
gulp.task("watch", function() {
    browserSync.init({
        files: [
            "./*.html",
            "./Template/*.html",
            "./Template/css/*.css",
            "./Template/js/*.js",
            "./Content/*.css",
            "./Scripts/*.js",
            "./dist/*.js",
            "./mainTmpl/*.html"
        ],
        logLevel: "debug",
        logPrefix: "insgeek",
        server: {
            //这里写的是html文件相对于根目录所在的文件夹*!/
            baseDir: "./",
            //这里如果不写，默认启动的是index.html，如果是其他名字，需要这里写*/
            index: "index.html"
        },
        port: 8080,
        ghostMode: {
            clicks: true,
            forms: true,
            scroll: true
        },
        browser: "chrome"
    });
    gulp.watch("src/*.js").on('change', reload);
});

// 代理
/*gulp.task("watch",function(){
    browserSync.init({
        //这里的files写的是需要监控的文件的位置
        files:[
            "./!*.html",
            "./Template/!*.html",
            "./Template/css/!*.css",
            "./Template/js/!*.js",
            "./Content/!*.css",
            "./Scripts/!*.js",
            "./dist/!*.js",
            "./mainTmpl/!*.html"
        ],
        logLevel: "debug",
        logPrefix:"hx",
        //这里的proxy写的是需要代理的服务器
        proxy:"localhost:63342",
        ghostMode: {
            clicks: true,
            forms: true,
            scroll: true
        },
        //这里写的是代理后，bs在哪个端口打开
        port: 8080,

        //这里设置的是bs运行时打开的浏览器名称
        browser: "chrome"
    });
});*/

// babel转译
/*
var plumber = require('gulp-plumber');
var babel = require('gulp-babel');
gulp.task('plumber',function (){
    gulp.src('./src/!*.js').pipe(plumber()).pipe(babel({presets:['es2015']})).pipe(gulp.dest('./build/'))
});*/
