function search() {//搜索查询
    var searchInput = document.getElementById("searchInput").value;
    //window.event.returnValue = false
    //这个属性放到提交表单中的onclick事件中在这次点击事件不会提交表单，如果放到超链接中则在这次点击事件不执行超链接href属性。
    window.location.href = '../../User/Query?keyword=' + searchInput;
    window.event.returnValue = false;
}

function search_a()
{
    var vra = document.getElementById("ViewLayout_Recommend").getElementsByClassName("vra");
    console.log(vra);
    for (var i = 0; i < vra.length; i++) {
        vra[i].Input = i;
        vra[i].onclick = function () {
            window.location.href = '../../User/Query?keyword=' + vra[this.Input].innerHTML;
        }
    }
}

function select_List()
{
    var select_a = document.getElementById("select").getElementsByClassName("a_sele");
    
    for (var i = 0; i < select_a.length; i++) {
        console.log(select_a[i]);
        select_a[i].Input = i;
        select_a[i].onclick = function ()
        {
            var se_innerHtml = select_a[this.Input].innerHTML;
            if (se_innerHtml == "全部")
            {
                se_innerHtml = "";
            }
            window.location.href = '../../User/Query?keyword=' + se_innerHtml;
        }
    }
}