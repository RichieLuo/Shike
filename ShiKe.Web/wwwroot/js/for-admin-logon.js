
window.onresize = function () {
    changeBannerHeight();
}

function changeBannerHeight() {
    var h = document.documentElement.clientHeight;//获取页面可见高度
    $("#adminbody").css({ "background-size": "100%" + h + "px" })
};

function Logon()
{
    loginVerify();
}

// 处理登录系统操作
function LogonGo()
{   
    var userName = document.getElementById('UserName').value;
    var password = document.getElementById('Password').value;
    if (userName === "" || password === "") {
        document.getElementById('logonModalErrMessage').innerText = "用户名或者密码不能为空。";
    } else {
        document.getElementById('logonModalErrMessage').innerText = "正在登录系统，请稍候......";
        // 创建登录数据模型
        var logonDataModel = "{" +
            "UserName:'" + userName + "'," +
            "Password:'" + password + "'" +
            "}";
        // 转换为 Json 模型
        var logonJsonModel = { 'jsonLogonInformation': logonDataModel };
        // 执行提交
        $.ajax({
            cache: false,
            type: 'POST',
            async: false,
            url: "../Admin/Logon",
            data: logonJsonModel,
            dataType: 'json',
            beforeSend: function () {
            }
        }).done(function (logonStatus) {
            if (logonStatus.result === true) {
                if (logonStatus.isAdminRole === true) {
                    location.href = logonStatus.message;
                } else {
                    document.getElementById("logonModalErrMessage").innerHTML = logonStatus.message;
                }
              
            } else {
                document.getElementById("logonModalErrMessage").innerText = logonStatus.message;
            }
        }).fail(function () {
            alert("这个连接后台失败！");
        }).always(function () {
        });
    }
}

var code; //在全局定义验证码
window.onload = function () {
    createCode();
}

//产生验证码
function createCode() {
    code = "";
    var codeLength = 4; //验证码的长度  
    var checkCode = document.getElementById("code");
    var random = new Array(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
        'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'); //随机数  
    for (var i = 0; i < codeLength; i++) { //循环操作  
        var index = Math.floor(Math.random() * 36); //取得随机数的索引（0~35）  
        code += random[index]; //根据索引取得随机数加到code上  
    }
    checkCode.value = code; //把code值赋给验证码  
}

//校验验证码 和 登录信息
function loginVerify() {

    var inputCode = document.getElementById("codeValidValue").value.toUpperCase(); //取得输入的验证码并转化为大写        
    if (inputCode.length <= 0) { //若输入的验证码长度为0  		
        alert("请输入验证码！"); //则弹出请输入验证码  
    } else if (inputCode !== code) { //若输入的验证码与产生的验证码不一致时		
        alert("验证码输入错误，请重新输入！"); //则弹出验证码输入错误  
        createCode(); //刷新验证码  
        document.getElementById("codeValidValue").value = ""; //清空文本框  
    } else { //输入正确时

        //alert("验证成功,正在跳转页面"); //验证成功
        LogonGo();
    }
}