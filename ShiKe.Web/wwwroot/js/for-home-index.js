
// 处理登录系统操作
function logon(role) {
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
            url: "../Account/Logon",
            data: logonJsonModel,
            dataType: 'json',
            beforeSend: function () {
            }
        }).done(function (logonStatus) {
            if (logonStatus.result === true) {
                //$('#logonModal').modal('hide'); // 关闭对话框
                //document.getElementById("logonModalErrMessage").href = "../Account/EditProfile"               
               
                location.href = "../Home/Index";
              
            } else {
                document.getElementById("logonModalErrMessage").innerText = logonStatus.message;
            }
        }).fail(function () {
            alert("这个连接后台失败！");
        }).always(function () {
        });
    }
}

// 处理注册用户操作
//function register() {
//    // 提取所需要的注册数据
//    var regUserName = document.getElementById('UserName').value;
//    var regFullName = document.getElementById('Name').value;
//    var regEmail = document.getElementById('EMail').value;
//    var regMobile = document.getElementById('MobileNumber').value;
//    var regPassword = document.getElementById('Password').value;
//    var regPasswordConfirm = document.getElementById('ConfirmPassword').value;
//    // 处理校验 TBD

//    if (regPassword !== regPasswordConfirm) {
//        document.getElementById('registerModalErrMessage').innerText = "密码和重复密码不一致，请重新输入。";
//    } else {
//        // 创建登录资料数据模型
//        var registerDataModel = "{" +
//            "UserName:'" + regUserName + "'," +
//            "Name:'" + regFullName + "'," +
//            "EMail:'" + regEmail + "'," +
//            "MobileNumber:'" + regMobile + "'," +
//            "Password:'" + regPassword + "'" +
//            "}";
//        // 转换为 Json 模型
//        var registerJsonModel = { 'jsonRegisterInformation': registerDataModel };
//        // 提交数据
//        $.ajax({
//            cache: false,
//            type: 'POST',
//            async: false,
//            url: "../Account/Register",
//            data: registerJsonModel,
//            dataType: 'json',
//            beforeSend: function () {
//            }
//        }).done(function (registerStatus) {
//            if (registerStatus.result === true) {
//                //alert("注册成功，您现在可以登录进行购物了！");
//                document.getElementById("registerModalErrMessage").innerText = registerStatus.message;
//            } else {
//                document.getElementById("registerModalErrMessage").innerText = registerStatus.message;
//            }
//        }).fail(function () {
//            alert("这个连接后台失败！");
//        }).always(function () {
//        });
//    }
//}

//// 获取用户登录名称
//function getUserName() {
//    $.ajax({
//        cache: false,
//        type: 'post',
//        async: false,
//        url: '../../Account/GetUserName',
//        beforeSend: function () {
//            // alert("开始访问");
//        }
//    }).done(function (data) {
//        document.getElementById("userLogonInfo").innerHTML = ' <a href="../../Account/EditProfile" style="padding:0 5px">'
//            + '<span class="glyphicon glyphicon-asterisk"></span>'
//            + '  欢迎你：' + data.userName
//            + '</a>';
//    }).fail(function () {
//        alert("连接后台失败！");
//    }).always(function () {
//        });
//}