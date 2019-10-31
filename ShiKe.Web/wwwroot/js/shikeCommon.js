/*
 *  根据指定的地址直接链接跳转回退
 * @param {} urlString 
 * @returns {} 
 */
function shikeGotoNewPage(urlString) {
    window.location.href = urlString;
}

/*
 *  根据指定的地址 urlString 访问控制器方法，然后根据返回的局部页刷新指定的 targetDiv 区域
 * @param {} urlString 
 * @param {} targetDivElelmentID 
 * @returns {} 
 */
function shikeGotoNewPartial(urlString, targetDivElelmentID) {
    shikeGotoNewPartialByJsonAndShowStatus(urlString, "", targetDivElelmentID, "", true);
}


/*
 * 根据指定的地址 urlString 访问控制器方法，
 * 执行访问时，在指定的位置呈现状态信息，
 * 然后根据返回的局部页刷新指定的 targetDiv 区域
 * @param {} urlString 
 * @param {} targetDivElelmentID 
 * @param {} statucMessage 
 * @returns {} 
 */
function shikeGotoNewPartialAndShowStatus(urlString, targetDivElelmentID, statucMessage) {
    shikeGotoNewPartialByJsonAndShowStatus(urlString, "", targetDivElelmentID, statucMessage, true);
}

/*
 * 根据指定的地址 urlString 和 jsonData 访问控制器方法，
 * 
 * @param {} urlString 
 * @param {} jsonData 
 * @param {} targetDivElelmentID 
 * @returns {} 
 */
function shikeGotoNewPartialByJson(urlString, jsonData, targetDivElelmentID) {
    shikeGotoNewPartialByJsonAndShowStatus(urlString, jsonData, targetDivElelmentID, "", true);
}

/*
 * 根据指定的地址 urlString 和 jsonData 访问控制器方法,
 * 执行访问时，在指定的位置呈现状态信息，
 * 然后根据返回的局部页刷新指定的 targetDivElelmentID 区域
 * 
 * @param {} urlString 
 * @param {} jsonData 
 * @param {} targetDivElelmentID 
 * @param {} statusMessage
 * * @param {} isAsync 
 * @returns {} 
 */
function shikeGotoNewPartialByJsonAndShowStatus(urlString, jsonData, targetDivElelmentID, statusMessage, isAsync) {
    $.ajax({
        cache: false,
        type: "POST",
        async: isAsync,
        url: urlString,
        data: jsonData,
        beforeSend: function () {
            if (statusMessage !== "") {
                $("#" + targetDivElelmentID).html(statusMessage);
            }
        }
    }).done(function (data) {
        var reg = /^<script>.*<\/script>$/;
        if (reg.test(data)) {
            // 这句是为了响应后台返回的js
            $('body').append("<span id='responseJs'>" + data + "</span>").remove("#responseJs");
            return;
        } else {
            if (targetDivElelmentID !== '') {
                $("#" + targetDivElelmentID).html(data);
            }
        }
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.error("调试错误:" + errorThrown);
    }).always(function () {
    });
}

/*
 * 创建新的 ListParaJson 对象
 * @param {} typeID 
 * @returns {} 
 */
function shikeCreateListParaJson(typeID) {
    shikeIntializationListPageParameter(typeID);
    var listParaJson = shikeGetListParaJson();
    return listParaJson;
}

/*
 *  重新初始化页面规格参数
 * @param {} typeID 
 * @returns {} 
 */
function shikeIntializationListPageParameter(typeID) {
    $("#shikeTypeID").val(typeID);
    $("#shikePageIndex").val("1");
    $("#shikePageSize").val("18");
    $("#shikePageAmount").val("0");
    $("#shikeObjectAmount").val("0");
    $("#shikeKeyword").val("");
    $("#shikeSortProperty").val("SortCode");
    $("#shikeSortDesc").val("default");
    $("#shikeSelectedObjectID").val("");
    $("#shikeIsSearch").val("False");
}

/*
 * 提取页面分页规格数据,构建 ListParaJson 对象
 * @returns {} 
 */
function shikeGetListParaJson() {
    // 提取缺省的页面规格参数
    var shikePageTypeID = $("#shikeTypeID").val();
    var shikePagePageIndex = $("#shikePageIndex").val();
    var shikePagePageSize = $("#shikePageSize").val();
    var shikePagePageAmount = $("#shikePageAmount").val();
    var shikePageObjectAmount = $("#shikeObjectAmount").val();
    var shikePageKeyword = $("#shikeKeyword").val();
    var shikePageSortProperty = $("#shikeSortProperty").val();
    var shikePageSortDesc = $("#shikeSortDesc").val();
    var shikePageSelectedObjectID = $("#shikeSelectedObjectID").val();
    var shikePageIsSearch = $("#shikeIsSearch").val();
    // 创建前端 json 数据对象
    var listParaJson = "{" +
        "ObjectTypeID:\"" + shikePageTypeID + "\", " +
        "PageIndex:\"" + shikePagePageIndex + "\", " +
        "PageSize:\"" + shikePagePageSize + "\", " +
        "PageAmount:\"" + shikePagePageAmount + "\", " +
        "ObjectAmount:\"" + shikePageObjectAmount + "\", " +
        "Keyword:\"" + shikePageKeyword + "\", " +
        "SortProperty:\"" + shikePageSortProperty + "\", " +
        "SortDesc:\"" + shikePageSortDesc + "\", " +
        "IsSearch:\"" + shikePageIsSearch + "\", " +
        "SelectedObjectID:\"" + shikePageSelectedObjectID + "\"" +
        "}";

    return listParaJson;
}