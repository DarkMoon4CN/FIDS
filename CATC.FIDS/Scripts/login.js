

function password_true(that) {
    //密码显示、隐藏
    var str = $(that).css('background-position');
    var oY = str.substring(str.length - 3);
    if (oY == '0px') {
        $(that).css('background-position', '0px -21px');
        $(that).parent().children('input').attr('type', 'text');
    } else {
        $(that).css('background-position', '0px 0px');
        $(that).parent().children('input').attr('type', 'password');
    }
}


function login_sys(operation, t_sex)
{
    var strUrl = "/FrontPage/index.html";
    var logintext = $("#t_logintext").val();
    var loginpwd = $("#t_loginpwd").val();
    if (logintext == "") {
        alert("请输入账号");
        return;
    }
    if (loginpwd == "") {
        alert("请输入密码");
        return;
    }

    $.ajax({
        url: "/Home/LoginSys?user_name=" + logintext + "&user_pwd=" + loginpwd,
        type: "post",
        success: function (msg) {
            if (msg.resault == "1") {

                window.location = strUrl;
            }
            else {
                alert("用户名或密码有误");
            }
        },
    });
}

