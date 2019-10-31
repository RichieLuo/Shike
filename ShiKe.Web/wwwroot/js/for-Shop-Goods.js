function gotoList(pageIndex) {
    var Stype= $("#SelectType").val();
    $.ajax({
        cache: false,
        type: 'post',
        async: false,
        url: '../../BusinessBG/List/?pageIndex=' + pageIndex + "&objectTypeID=" + Stype,
        beforeSend: function () {
        }
    }).done(function (data) {
            document.getElementById("mainWorkPlaceArea").innerHTML = data;
        
        

    }).fail(function () {
        alert("连接后台失败！");
    }).always(function () {
    });
}

function gotoCreateOrEdit(id) {
    $('#createOrEditModal').modal({
        show: true,
        backdrop: 'static'
    });
    // 访问后台 CreateOrEdit 方法，获取新建或者编辑数据的呈现 imports page 页面内容
    $.ajax({
        cache: false,
        type: 'post',
        async: false,
        url: '../../BusinessBG/EditForGoods/' + id,
        beforeSend: function () {
            // alert("开始访问");
        }
    }).done(function (data) {
        document.getElementById("createOrEditArea").innerHTML = data;
    }).fail(function () {
        alert("连接后台失败！");
    }).always(function () {
    });
}

function postGoodsEditForm() {
    var systemWorkPlaceFormOptions = {
        dataType: 'json',
        success: function (validateMessage) {
            if (validateMessage.isOK === true) {
                $('#createOrEditModal').modal('hide');
                var id = document.getElementById('HiddenID').value;
                gotoList("1");
            } else {
                $.each(validateMessage.validateMessageItems, function (i, item) {
                    activeErrStatus(item.messageName, item.message);
                });
            }
        }
    };
    $('#CreateOrEditGoodsForm').ajaxSubmit(systemWorkPlaceFormOptions);
}

function Settled() {
    if (!$('#ckRead').is(':checked')) {
        alert('请阅读服务条款后并勾选"已阅读并同意相关服务条款和隐私政策"项');
        return;
    }
    var systemWorkPlaceFormOptions = {
        dataType: 'json',
        success: function (validateMessage) {
            if (validateMessage.isOK === true) {
                ////gotoList(validateMessage.validateMessageItems[0].message);
                //window.location.href = '../../BusinessBG/Index'
            }
            else {
                $.each(validateMessage.validateMessageItems, function (i, item) {
                    activeErrStatus(item.messageName, item.message);
                });
            }
        }
    };
    $('#SettledForm').ajaxSubmit(systemWorkPlaceFormOptions);
}

// 呈现错误消息
function activeErrStatus(idName, msg) {
    document.getElementById(idName + "_DIV").classList.add("has-error", "has-feedback");
    document.getElementById(idName + "_Help").innerHTML = '<span class="help-block small" >' + msg + '</span>';
}

// 清理错误消息，输入域失去焦点的时候使用
function clearErrStyle(idName) {
    document.getElementById(idName + "_DIV").classList.remove("has-error", "has-feedback");
    document.getElementById(idName + "_Help").innerHTML = "";
}

// 失去焦点的时候进行处理的内容
function checkIfValidate(idName) {
    var result = document.getElementById(idName).value;
    alert(result);
}

// 打开删除操作会话框
function openDeleteModal(id, alertMessage, deleteType) {
    document.getElementById("modelID").value = id;
    document.getElementById("deleteModalMessage").innerText = alertMessage;
    document.getElementById("DeleteType").value = deleteType;
    $('#deleteConfirmModal').modal({
        show: true,
        backdrop: 'static'
    });
}

// 执行删除操作
function gotoDelete() {
    var pid = document.getElementById("modelID").value;
    var type = document.getElementById("DeleteType").value;
    $.ajax({
        cache: false,
        type: 'post',
        async: false,
        url: '../../BusinessBG/Delete/' + pid,
        beforeSend: function () {
        }
    })
        .done(function (data) {
            if (data.message == "删除操作成功！") {
                $('#deleteConfirmModal').modal('hide');
                if (type == 'deleteImg') {
                    alert(data.message);
                    document.getElementById(pid + "_box").remove();
                }
                else {
                    gotoList("1");
                }
            } else {
                $('#deleteConfirmModal').modal('hide');
                if (type == 'deleteImg') {
                    alert(data.message);
                }
                else {
                    gotoList("1");
                }
            }

        })
        .fail(function () {
            alert("连接后台失败！");
        })
        .always(function () {
        });
}

function gotoDetail(id) {
    $.ajax({
        cache: false,
        type: 'post',
        async: false,
        url: '../../BusinessBG/Detail/' + id,
        beforeSend: function () {
        }
    }).done(function (data) {
        document.getElementById("mainWorkPlaceArea").innerHTML = data;
    }).fail(function () {
        alert("连接后台失败！");
    }).always(function () {
    });
}


function saveCover(imgid) {
    $.ajax({
        cache: false,
        type: 'post',
        async: false,
        url: '../../BusinessBG/SaveCover/' + imgid,
        beforeSend: function () {
         
        }
    }).done(function (data) {
        if (data.isOK == true) {
            alert("设为封面成功");
            var Istitle = document.getElementsByClassName('IsTitle')[0];
            Istitle.classList.remove('IsTitle');
            document.getElementById(imgid + "_DIV").classList.add('IsTitle');
            
        }
    }).fail(function () {
        alert("连接后台失败！");
    }).always(function () {
    });
}

 