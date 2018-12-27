$(function () {
    $("#DepartmentList").DataTable();
    $("#ProductList").DataTable();
    $("#BrandList").DataTable();
    $("#ProductRegister").DataTable();
    $("#UserList").DataTable();
    $("#ProductTypeList").DataTable();

    //Get Active Page
    var url = window.location.href;
    $('.navbar-nav li a[href="' + url + '"]').parent().addClass('active');
    $('.navbar-nav li a').filter(function () { 
            return this.href == url;
    }).parent().addClass('active');
}); 
