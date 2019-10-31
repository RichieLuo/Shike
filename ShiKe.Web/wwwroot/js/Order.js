$(function () {
    $(".or_u li").click(function () {
        var index = $(this).index();
        $(this).addClass("order_ul_li")
            .siblings().removeClass("order_ul_li");
        $(".products .or_d").eq(index)
            .addClass("selected")
            .siblings().removeClass("selected");
    });
});

$(function () {
    $(".se_nav_tabs li").click(function () {
        var index = $(this).index();
        $(this).addClass("se_active")
            .siblings().removeClass("se_active");

        $(".se_tabs_bd .se_in").eq(index)
            .addClass("selected")
            .siblings().removeClass("selected");
    });
});
