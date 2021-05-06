
var count = 1;
$(".group-item").click(function () {
    if (!$(this).children().hasClass('fa-check-circle-o')) {
        $(this).parent().parent().toggleClass("table-info");
        $(this).children().toggleClass("fa-minus-circle");
        $("#numitemsselected").text($("#table_groupitems").DataTable().rows(".table-info").data().length);
    }
    if ($(".fa-minus-circle").length > 0){
        //Run Calculations
        $("#btnNewItemGroup").prop('disabled', false);
        $("#btnStep3").prop('disabled', false);
        if (BrandItemRestrictionPass()) {
            
            $("#btnCostingAnalysis").prop('disabled', false);
            if ($("#tempitemgroup").is(":visible")) {
                $("#btnCostingAnalysis").text("Update Items");
            }
            else {
                $("#btnCostingAnalysis").text("Create Costing Analysis For Selected");
            }
        }
        else {
            $("#btnCostingAnalysis").text("Selected Items Cannot Be In The Same Group");
            $("#btnCostingAnalysis").prop('disabled', true);
        }
        
    }
    if ($("#new-itemgroup-name").val() == "") {
        $("#new-itemgroup-name").val($(this).parent().parent().find("#table-itemdesc").text());
    }

});
function BrandItemRestrictionPass() {
    var warehousecat = [];
    var shelflife = [];
    var dutypct = [];
    var items = $("#table_groupitems").DataTable().rows(".table-info").data();
    for (i = 0; i < items.length; i++) {
        warehousecat.push(items[i][7]);
        shelflife.push(items[i][8]);
        dutypct.push(items[i][13]);
    }
    if (diffItemsWarning(warehousecat) && similarShelflife(shelflife)) {
        switch (warehousecat[0]) {
            case "Frozen":
                $("#data-warehousecat").val("F");
                break;
            case "Perishable":
                $("#data-warehousecat").val("P");
                break;
            default:
                $("#data-warehousecat").val("");
                break;
        }
        for (i = 0; i < shelflife.length; i++) {
            if (shelflife[i] <= 180) {
                $("#data-shortshelflife").prop("checked", "true");
                break;
            }
            else {
                $("#data-shortshelflife").prop("checked", "false");
            }
        }
        return true;
    }
    else {
        return false;
    }
    
}
$("#btnCostingAnalysis").click(function(){
    updateItemList();
    $("#div-progressbtn").show();
    $("#tempitemgroup").show();
    createupdateCosting();
});

function saveItemGroup(brand) {
    var name = $("#new-itemgroup-name").val();
    var sname = name.replace(/ /g, '');
    $('<section id="tempitemgroup" class="itemgroupform"></section>').insertAfter($("#tempitemgroup"));
    var form = $("#tempitemgroup").html();
    $("#tempitemgroup").prop("id", sname);
    $("#" + sname).prop('hidden', true);
    $("#tempitemgroup").html(form);
    var numofitems = $(".fa-minus-circle").length;
    var newitemgroup = '<a class="dropdown-item" href="#?" onclick=\"loadItemGroup(\''+sname+'\');\"><button class="btn btn-sm btn-danger mr-2" onclick="warningDelete()"><i class="fa fa-trash text-white m-0"></i></button>' + name + ' <span class="badge badge-primary badge-pill ml-3" style="position: static;">' + numofitems + '</span> </a> '
    $("#sel-itemgroups").append(newitemgroup);
    AddNewGroup(brand);
    $("#btnCostingAnalysis").prop('hidden', false);

}
function loadItemGroup(id) {
    $(".itemgroupform").prop('hidden', true);
    $("#" + id).prop('hidden', false);
    
}
function displaycosting() {
    $("#tempitemgroup").prop('hidden', false);
    $("#btnCostingAnalysis").text("Update Items");
    $(".skipbtn").prop('hidden', true);
    document.getElementById('tempitemgroup').scrollIntoView();

    //Highlight Editable Fields
    $("#tempitemgroup input:not([readonly])").css("color", "black");
    $("#tempitemgroup input:not([readonly])").css("font-weight", "600");
    $("#tempitemgroup input:not([readonly])").css("box-shadow", "0 0 0 0.05rem black");
}
function scrolltotop() {
    $(window).scrollTop(0);
}
function togglepnltable() {
    $("#pnltable").slideToggle();
}


function warningSave() {
    $("#modalWarningSave").modal('toggle');
}
function warningDelete() {
    $("#modalDeleteGroup").modal('toggle');
}
$("#btn-FinancialAllItems").click(function () {
    $("#div-FinancialAllItems").removeClass("d-none");
    $("#div-FinancialAffectedItems").addClass("d-none");
    $("#btn-FinancialAllItems").addClass("active");
    $("#btn-FinancialAffectedItems").removeClass("active");
});
$("#btn-FinancialAffectedItems").click(function () {
    $("#div-FinancialAllItems").addClass("d-none");
    $("#div-FinancialAffectedItems").removeClass("d-none");
    $("#btn-FinancialAllItems").removeClass("active");
    $("#btn-FinancialAffectedItems").addClass("active");
});



function resizeIframe(obj) {
    obj.style.height = obj.contentWindow.document.documentElement.scrollHeight + 'px';
}


function changecolor() {
    $(".changenum").each(function () {
        if (parseFloat($(this).text()) > 0) {
            $(this).removeClass('text-danger');
            $(this).addClass('text-success');
        }
        else if (parseFloat($(this).text()) < 0) {
            $(this).removeClass('text-success');
            $(this).addClass('text-danger');
        }
    });
}
function getUrlVars() {
    var vars = {};
    var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
        vars[key] = value;
    });
    return vars;
}
function Step1Submitted() {
    var selectedbrand = getUrlVars()["brand"];
    if (selectedbrand != "" && selectedbrand != undefined) {
        Step2();
    }
}
function average(numlist) {
    var total = 0;
    for (var i = 0; i < numlist.length; i++) {
        total += numlist[i];
    }
    var average = (total / numlist.length).toFixed(2);
    return average;
}
function total(numlist) {
    var total = 0;
    for (var i = 0; i < numlist.length; i++) {
        total += numlist[i];
    }
    return total;
}
function loaderFadeout() {
    $("#div-loader").fadeOut();
}
function loaderFadein() {
    $("#div-loader").fadeIn();
}
function validate() {
    var validated;
    $(".form-control").filter('[required]').each(function () {
        if ($(this).val() === '') {
            return false;
        }
        else {
            validated = true;
        }
    })
    if (validated) {
        $("#div-loader").fadeIn();
    }
    return validated;
}
function validateStep2() {
    var validated;
    $(".form-control").each(function () {
        if ($(this).val() === '') {
            validated = false;
        }
        else {
            validated = true;
        }
    })
    if (validated) {
        loaderFadein();
    }
    else {
        loaderFadeout();
        alert();
        event.preventDefault();
        
    }

}
function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}