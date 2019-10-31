

//全站顶部搜索框的查询
function search() {
    var ShopOrGoods = $("#shopOrGoods option:selected").val();
    if (ShopOrGoods == "全部") {
        var searchInput = document.getElementById("searchInput").value;
        if ( searchInput != "") {
            alert("暂时不支持全部搜索！功能正在开发中...");
        } else {
            location.href = '../../GoodsManager/Search';
        }
        event.returnValue = false;
    }
    else if (ShopOrGoods == "商品") {
        var searchInput = document.getElementById("searchInput").value;
        //window.event.returnValue = false
        //这个属性放到提交表单中的onclick事件中在这次点击事件不会提交表单，如果放到超链接中则在这次点击事件不执行超链接href属性。
        location.href = '../../GoodsManager/Search?keyword=' + searchInput;
        event.returnValue = false;
    }
    else if (ShopOrGoods == "店铺") {
        var searchInput = document.getElementById("searchInput").value;
        location.href = '../../GoodsManager/ShopSearch?keyword=' + searchInput;
        event.returnValue = false;
    }

}

//首页查询
function CateSearch(nameVal) {
    location.href = '../../GoodsManager/CateSearch?keyword=' + nameVal;
}

//首页更多
function SearchMore() {
     location.href = '../../GoodsManager/Search';
}
$(function () {

    //按类别查询  searchValCate
    $(".pSearch .searchValCate").click(function () {
        var val = $(this).text();
        if (val == "全部") {
            location.href = '../../GoodsManager/Search';
        } else {
            location.href = '../../GoodsManager/CateSearch?keyword=' + val;
        }
    })

    //按商品查询  searchValGoods
    $(".pSearch .searchValGoods").click(function () {
        var val = $(this).text();
        location.href = '../../GoodsManager/Search?keyword=' + val;
    })
    //按商品查询（更多类型 未完成）  searchValGoods
    $(".pSearch .searchValCateMore").click(function () {
        location.href = ' ../../Home/Unfinished';
    })

    //按地区查询（未完成）  searchValArea
    $(".searchValArea").click(function () {
        location.href = ' ../../Home/Unfinished';
    })
    //按地区查询（其他 未完成） searchValArea
    $(".searchValAreaMore").click(function () {
        location.href = ' ../../Home/Unfinished';
    })

    //按商品查询（位置：搜索框底部）  searchValGoods
    $(".searchValGoodsForSearchBottom").click(function () {
        var val = $(this).text();
        location.href = '../../GoodsManager/Search?keyword=' + val;
    })


})
