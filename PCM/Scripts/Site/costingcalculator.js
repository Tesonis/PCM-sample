//Returns true if all items are equal
function diffItemsWarning(arr) {
    var allequal = arr.every(v => v === arr[0]);
    return allequal;
}
function similarShelflife(arr) {
    if (parseInt(arr[0]) > 180) {
        for (i = 1; i < arr.length; i++) {
            if (parseInt(arr[i]) <= 180) {
                return false;
            }
        }
        return true;
    }
    else {
        for (i = 1; i < arr.length; i++) {
            if (parseInt(arr[i]) > 180) {
                return false;
            }
        }
        return true;
    }
    
}
function loadCosting() {
    var unitsize = [];
    var unitspercase = [];
    var items = $("#table_groupitems").DataTable().rows(".table-info").data();
    for (i = 0; i < items.length; i++) {
        unitsize.push(items[i][3]);
        unitspercase.push(parseInt(items[i][4]));
    }
    if (items.length > 0) {
        //Proposed placeholder values
        if (!diffItemsWarning(unitspercase)) {
            $("#data-unitspercase").text("Unequal Values");
        }
        else {
            $("#data-unitspercase").val(unitspercase[0].toString());
        }
        if (!diffItemsWarning(unitsize)) {
            $("#data-unitsize").val("Various");
        }
        else {
            $("#data-unitsize").val(unitsize[0].toString());
        }
    }
    $("#data-currrevenueFormat").val(numberWithCommas($("#data-currrevenue").val()));
    calculateCostingAllItems("");
    
}

function createupdateCosting() {
    var items = $("#table_groupitems").DataTable().rows(".table-info").data();
    var unitsize = [];
    var unitspercase = [];
    var itemspurchaseprice = [];
    var itemswholesaleprice = [];
    var itemsdsdprice = [];
    var mac = [];
    var unitssold = [];
    var revenue = [];
    var suggestedretailprice = [];
    var duty = [];
    for (i = 0; i < items.length; i++) {
        unitsize.push(items[i][3]);
        unitspercase.push(parseInt(items[i][4]));
        itemspurchaseprice.push(parseFloat(items[i][5].split('$')[1]));
        itemswholesaleprice.push(parseFloat(items[i][6].split('$')[1]));
        itemsdsdprice.push(parseFloat(items[i][9]));
        mac.push(parseFloat(items[i][10]));
        revenue.push(parseFloat(items[i][11]));
        unitssold.push(parseInt(items[i][12]));
        suggestedretailprice.push(parseFloat(items[i][14].split('$')[1]));
    }
    if (!diffItemsWarning(unitsize) || !diffItemsWarning(unitspercase)) {
        $("#div-diffItemsWarning").show();
    }
    else {
        $("#div-diffItemsWarning").hide();
    }
    //Proposed placeholder values
    if (!diffItemsWarning(unitspercase)) {
        $("#data-unitspercase").val("Various");
    }
    else {
        $("#data-unitspercase").val(unitspercase[0].toString());
    }
    if (!diffItemsWarning(unitsize)) {
        $("#data-unitsize").val("Various");
    }
    else {
        $("#data-unitsize").val(unitsize[0].toString());
    }
    var currwholesaleppc = Number(average(itemswholesaleprice)).toFixed(2);
    var currsuggestedppc = Number(average(suggestedretailprice)).toFixed(2);
    var currwholesaletrademargin = currwholesaleppc / (currsuggestedppc) / parseInt(unitspercase[0]);
    currwholesaletrademargin = ((100 - (currwholesaletrademargin) * 100)).toFixed(2);

    var currdsdppc = Number(average(itemsdsdprice)).toFixed(2);
    var currdsdtrademargin = currdsdppc / (currsuggestedppc) / parseInt(unitspercase[0]);
    currdsdtrademargin = ((100 - (currdsdtrademargin) * 100)).toFixed(2);
    //Put values into page
    
    
    $("#data-currppc").val(Number(average(itemspurchaseprice)).toFixed(2));
    $("#data-currwholesaleppc").val(currwholesaleppc);
    $("#data-currdsdppc").val(average(itemsdsdprice).toString());
    $("#data-currlandedcost").val(average(mac).toString());
    $("#data-duty").val(parseFloat(items[0][13].split('%')[0]));
    $("#data-propppc").val($("#tempitemgroup #data-currppc").val());
    $("#data-propwholesaleppc").val($("#tempitemgroup #data-currwholesaleppc").val());
    $("#data-propdsdppc").val($("#tempitemgroup #data-currdsdppc").val());
    $("#data-cashterms").val("1.5");
    $("#data-dealaccrual").val("0");
    $("#data-currwholesalesuggestedppc").val(currsuggestedppc);
    
    $("#data-currwholesaletrademargin").val(currwholesaletrademargin);
    $("#data-propwholesaletrademargin").val("35.00");
    $("#data-currdsdtrademargin").val(currdsdtrademargin);
    $("#data-propdsdtrademargin").val("35.00");
    //SalesMarginAnalysis
    $("#data-currcasevolume").val(total(unitssold).toString());
    $("#data-propcasevolume").val($("#data-currcasevolume").val());
    var currrev = total(revenue);
    var testrev = numberWithCommas(currrev.toFixed(2));
    $("#data-currrevenueFormat").val(numberWithCommas(currrev.toFixed(2)));
    $("#data-proprevenueFormat").val(numberWithCommas(currrev.toFixed(2)));
    $("#data-currrevenue").val(currrev.toFixed(2));
    $("#data-proprevenue").val(currrev.toFixed(2));
    $("#data-affectedl12cases").val(total(unitssold).toString());
    displaycosting();
    calculateCostingAllItems("");
}
function updateItemList() {
    var itemlist = [];
    var items = $("#table_groupitems").DataTable().rows(".table-info").data();
    for (i = 0; i < items.length; i++) {
        var itemid = items[i][1];
        itemlist.push(itemid);
    }
    $("#data-itemlist").val(itemlist.join(','));
    
}

function freightswitch(x) {
    if (x == "num") {
        if (!$("#btn-freightnum").hasClass("active")) {
            $("#btn-freightnum").addClass("active");
            $("#btn-freightperc").removeClass("active");
            $("#data-freightcosting").removeClass("d-none");
            $("#data-freightcostingperc").addClass("d-none");
        }
    }
    else {
        if (!$("#btn-freightperc").hasClass("active")) {
            $("#btn-freightperc").addClass("active");
            $("#btn-freightnum").removeClass("active");
            $("#data-freightcosting").addClass("d-none");
            $("#data-freightcostingperc").removeClass("d-none");
        }
    }
}
function calculateFinancialAllItems() {
    //CASES
    var L12affectedCases = parseInt($("#val-Affectedl12cases").val());
    var N12affectedCases = parseInt($("#val-Affectedn12cases").val());
    var L12unaffectedCases = parseInt($("#val-Unaffectedl12cases").val());
    var N12unaffectedCases = parseInt($("#val-Unaffectedn12cases").val());
    var L12totalcases = L12affectedCases + L12unaffectedCases;
    var N12totalcases = N12affectedCases + N12unaffectedCases;
    var Changetotalcases = N12totalcases - L12totalcases;
    //update
    $("#val-L12totalcases").text(L12totalcases.toString());
    $("#val-N12totalcases").text(N12totalcases.toString());
    $("#val-Changetotalcases").text(Changetotalcases.toString());
    //GROSS SALES
    var L12affectedgrosssales = parseFloat($("#val-Affectedl12price").val());
    var N12affectedgrosssales = parseFloat($("#val-Affectedn12price").val());
    var L12unaffectedgrosssales = parseFloat($("#val-Unaffectedl12price").val());
    var N12unaffectedgrosssales = parseFloat($("#val-Unaffectedn12price").val());
    var L12totalgrosssales = L12affectedgrosssales + L12unaffectedgrosssales;
    var N12totalgrosssales = N12affectedgrosssales + N12unaffectedgrosssales;
    var Changegrosssales = N12totalgrosssales - L12totalgrosssales;
    //update
    $("#val-L12totalgrosssales").text(numberWithCommas(L12totalgrosssales.toFixed(2)));
    $("#val-N12totalgrosssales").text(numberWithCommas(N12totalgrosssales.toFixed(2)));
    $("#val-Changegrosssales").text(numberWithCommas(Changegrosssales.toFixed(2)));
    //AVERAGE PRICE / CASE
    var L12affectedappc = parseFloat((L12affectedgrosssales / L12affectedCases).toFixed(2));
    var N12affectedappc = parseFloat((N12affectedgrosssales / N12affectedCases).toFixed(2));
    if (L12unaffectedgrosssales == 0) {
        var L12unaffectedappc = 0;
    }
    else {
        var L12unaffectedappc = parseFloat((L12unaffectedgrosssales / L12unaffectedCases).toFixed(2));
    }
    if (N12unaffectedgrosssales == 0) {
        var N12unaffectedappc = 0;
    }
    else {
        var N12unaffectedappc = parseFloat((N12unaffectedgrosssales / N12unaffectedCases).toFixed(2));
    }
    
    var L12totalappc = (L12affectedappc * L12affectedCases / L12totalcases) + (L12unaffectedappc * L12unaffectedCases / L12totalcases);
    var N12totalappc = (N12affectedappc * N12affectedCases / N12totalcases) + (N12unaffectedappc * N12unaffectedCases / N12totalcases);
    var Changeappc = N12totalappc - L12totalappc;
    //update
    $("#val-L12affectedappc").text(L12affectedappc);
    $("#val-N12affectedappc").text(N12affectedappc);
    $("#val-L12unaffectedappc").text(L12unaffectedappc);
    $("#val-N12unaffectedappc").text(N12unaffectedappc);
    $("#val-L12appc").text(L12totalappc.toFixed(2).toString());
    $("#val-N12appc").text(N12totalappc.toFixed(2).toString());
    $("#val-Changeappc").text(Changeappc.toFixed(2).toString());
    //Terms
    var L12affectedterms = parseFloat($("#val-Affectedl12terms").val());
    var N12affectedterms = parseFloat($("#val-Affectedn12terms").val());
    var L12unaffectedterms = parseFloat($("#val-Affectedl12terms").val());
    var N12unaffectedterms = parseFloat($("#val-Affectedl12terms").val());
    var L12terms = L12affectedterms + L12unaffectedterms;
    var N12terms = N12affectedterms + N12unaffectedterms;
    var Changeterms = N12terms - L12terms;
    //update
    $("#val-L12terms").text(L12terms.toFixed(2).toString());
    $("#val-N12terms").text(N12terms.toFixed(2).toString());
    $("#val-Changeterms").text(Changeterms.toFixed(2).toString());
    //Volume Rebates
    var L12affectedvolumerebates = parseFloat($("#val-Affectedl12volumerebates").val());
    var N12affectedvolumerebates = parseFloat($("#val-Affectedn12volumerebates").val());
    var L12unaffectedvolumerebates = parseFloat($("#val-Affectedl12volumerebates").val());
    var N12unaffectedvolumerebates = parseFloat($("#val-Affectedl12volumerebates").val());
    var L12volumerebates = L12affectedvolumerebates + L12unaffectedvolumerebates;
    var N12volumerebates = N12affectedvolumerebates + N12unaffectedvolumerebates;
    var Changevolumerebates = N12volumerebates - L12volumerebates;
    //update
    $("#val-L12volumerebates").text(L12volumerebates.toFixed(2).toString());
    $("#val-N12volumerebates").text(N12volumerebates.toFixed(2).toString());
    $("#val-Changevolumerebates").text(Changevolumerebates.toFixed(2).toString());

    //Customer Programs
    var L12affectedcustomerprograms = parseFloat($("#val-Affectedl12customerprograms").val());
    var N12affectedcustomerprograms = parseFloat($("#val-Affectedn12customerprograms").val());
    var L12unaffectedcustomerprograms = parseFloat($("#val-Affectedl12customerprograms").val());
    var N12unaffectedcustomerprograms = parseFloat($("#val-Affectedl12customerprograms").val());
    var L12customerprograms = L12affectedcustomerprograms + L12unaffectedcustomerprograms;
    var N12customerprograms = N12affectedcustomerprograms + N12unaffectedcustomerprograms;
    var Changecustomerprograms = N12customerprograms - L12customerprograms;
    //update
    $("#val-L12customerprograms").text(L12customerprograms.toFixed(2).toString());
    $("#val-N12customerprograms").text(N12customerprograms.toFixed(2).toString());
    $("#val-Changecustomerprograms").text(Changecustomerprograms.toFixed(2).toString());
    
    //GROSS LESS PROGRAMS
    var L12affectedgrosslessprograms = L12affectedgrosssales - L12affectedcustomerprograms - L12affectedterms - L12affectedvolumerebates;
    var N12affectedgrosslessprograms = N12affectedgrosssales - N12affectedcustomerprograms - N12affectedterms - N12affectedvolumerebates;
    var L12unaffectedgrosslessprograms = L12unaffectedgrosssales - L12unaffectedcustomerprograms - L12unaffectedterms - L12unaffectedvolumerebates;
    var N12unaffectedgrosslessprograms = N12unaffectedgrosssales - N12unaffectedcustomerprograms - N12unaffectedterms - N12unaffectedvolumerebates;
    var L12grosslessprograms = L12affectedgrosslessprograms + L12unaffectedgrosslessprograms;
    var N12grosslessprograms = N12affectedgrosslessprograms + N12unaffectedgrosslessprograms;
    var Changegrosslessprograms = L12grosslessprograms - N12grosslessprograms;
    //update
    $("#val-L12affectedgrosslessprograms").text(numberWithCommas(L12affectedgrosslessprograms.toFixed(2)));
    $("#val-N12affectedgrosslessprograms").text(numberWithCommas(N12affectedgrosslessprograms.toFixed(2)));
    $("#val-L12unaffectedgrosslessprograms").text(numberWithCommas(L12unaffectedgrosslessprograms.toFixed(2)));
    $("#val-N12unaffectedgrosslessprograms").text(numberWithCommas(N12unaffectedgrosslessprograms.toFixed(2)));
    $("#val-L12grosslessprograms").text(numberWithCommas(L12grosslessprograms.toFixed(2)));
    $("#val-N12grosslessprograms").text(numberWithCommas(N12grosslessprograms.toFixed(2)));
    $("#val-Changegrosslessprograms").text(numberWithCommas(Changegrosslessprograms.toFixed(2)));
    //Off Invoice
    var L12affectedoffinvoice = parseFloat($("#val-Affectedl12offinvoice").val());
    var N12affectedoffinvoice = parseFloat($("#val-Affectedn12offinvoice").val());
    var L12unaffectedoffinvoice = parseFloat($("#val-Affectedl12offinvoice").val());
    var N12unaffectedoffinvoice = parseFloat($("#val-Affectedl12offinvoice").val());
    var L12offinvoice = L12affectedoffinvoice + L12unaffectedoffinvoice;
    var N12offinvoice = N12affectedoffinvoice + N12unaffectedoffinvoice;
    var Changeoffinvoice = N12offinvoice - L12offinvoice;
    //update
    $("#val-L12offinvoice").text(L12offinvoice.toFixed(2).toString());
    $("#val-N12offinvoice").text(N12offinvoice.toFixed(2).toString());
    $("#val-Changeoffinvoice").text(Changeoffinvoice.toFixed(2).toString());
    //Credits
    var L12affectedcredits = parseFloat($("#val-Affectedl12credits").val());
    var N12affectedcredits = parseFloat($("#val-Affectedn12credits").val());
    var L12unaffectedcredits = parseFloat($("#val-Affectedl12credits").val());
    var N12unaffectedcredits = parseFloat($("#val-Affectedl12credits").val());
    var L12credits = L12affectedcredits + L12unaffectedcredits;
    var N12credits = N12affectedcredits + N12unaffectedcredits;
    var Changecredits = N12credits - L12credits;
    //update
    $("#val-L12credits").text(L12credits.toFixed(2).toString());
    $("#val-N12credits").text(N12credits.toFixed(2).toString());
    $("#val-Changecredits").text(Changecredits.toFixed(2).toString());
    //Marketing Spend
    var L12affectedmarketingspend = parseFloat($("#val-Affectedl12marketingspend").val());
    var N12affectedmarketingspend = parseFloat($("#val-Affectedn12marketingspend").val());
    var L12unaffectedmarketingspend = parseFloat($("#val-Affectedl12marketingspend").val());
    var N12unaffectedmarketingspend = parseFloat($("#val-Affectedl12marketingspend").val());
    var L12marketingspend = L12affectedmarketingspend + L12unaffectedmarketingspend;
    var N12marketingspend = N12affectedmarketingspend + N12unaffectedmarketingspend;
    var Changemarketingspend = N12marketingspend - L12marketingspend;
    //update
    $("#val-L12marketingspend").text(L12marketingspend.toFixed(2).toString());
    $("#val-N12marketingspend").text(N12marketingspend.toFixed(2).toString());
    $("#val-Changemarketingspend").text(Changemarketingspend.toFixed(2).toString());
    //NET SALES
    var L12affectednetsales = L12affectedgrosslessprograms - L12affectedoffinvoice - L12affectedcredits - L12affectedmarketingspend;
    var N12affectednetsales = N12affectedgrosslessprograms - N12affectedoffinvoice - N12affectedcredits - N12affectedmarketingspend;
    var L12unaffectednetsales = L12unaffectedgrosslessprograms - L12unaffectedoffinvoice - L12unaffectedcredits - L12unaffectedmarketingspend;
    var N12unaffectednetsales = N12unaffectedgrosslessprograms - N12unaffectedoffinvoice - N12unaffectedcredits - N12unaffectedmarketingspend;
    var L12netsales = L12affectednetsales + L12unaffectednetsales;
    var N12netsales = N12affectednetsales + N12unaffectednetsales;
    var Changenetsales = L12netsales - N12netsales;
    //update
    $("#val-L12affectednetsales").text(numberWithCommas(L12affectednetsales.toFixed(2)));
    $("#val-N12affectednetsales").text(numberWithCommas(N12affectednetsales.toFixed(2)));
    $("#val-L12unaffectednetsales").text(numberWithCommas(L12unaffectednetsales.toFixed(2)));
    $("#val-N12unaffectednetsales").text(numberWithCommas(N12unaffectednetsales.toFixed(2)));
    $("#val-L12netsales").text(numberWithCommas(L12netsales.toFixed(2)));
    $("#val-N12netsales").text(numberWithCommas(N12netsales.toFixed(2)));
    $("#val-Changenetsales").text(numberWithCommas(Changenetsales.toFixed(2)));
    //COGS
    var L12affectedcogs = parseFloat($("#val-L12affectedcogs").text());
    var N12affectedcogs = L12affectedcogs / L12affectedgrosssales * N12affectedgrosssales;
    if (!isFinite(N12affectedcogs)) {
        N12affectedcogs = 0;
    }
    var L12unaffectedcogs = parseFloat($("#val-L12unaffectedcogs").text());
    var N12unaffectedcogs = L12unaffectedcogs / L12unaffectedgrosssales * N12unaffectedgrosssales;
    if (!isFinite(N12unaffectedcogs)) {
        N12unaffectedcogs = 0;
    }
    var L12totalcogs = L12affectedcogs + L12unaffectedcogs;
    var N12totalcogs = N12affectedcogs + N12unaffectedcogs;
    var Changecogs = N12totalcogs - L12totalcogs;
    //update
    $("#val-N12affectedcogs").text(N12affectedcogs.toFixed(2).toString());
    $("#val-N12unaffectedcogs").text(N12unaffectedcogs.toFixed(2).toString());
    $("#val-L12totalcogs").text(L12totalcogs.toFixed(2).toString());
    $("#val-L12totalcogs").text(L12totalcogs.toFixed(2).toString());
    $("#val-N12totalcogs").text(N12totalcogs.toFixed(2).toString());
    $("#val-Changecogs").text(Changecogs.toFixed(2).toString());
    //GROSS MARGIN
    var L12affectedgrossmargin = L12affectednetsales - L12affectedcogs;
    var N12affectedgrossmargin = N12affectednetsales - N12affectedcogs;
    var L12unaffectedgrossmargin = L12unaffectednetsales - L12unaffectedcogs;
    var N12unaffectedgrossmargin = N12unaffectednetsales - N12unaffectedcogs;
    var L12totalgrossmargin = L12affectedgrossmargin + L12unaffectedgrossmargin;
    var N12totalgrossmargin = N12affectedgrossmargin + N12unaffectedgrossmargin;
    var Changegrossmargin = N12totalgrossmargin - L12totalgrossmargin;
    //update
    $("#val-L12affectedgrossmargin").text(L12affectedgrossmargin.toFixed(2).toString());
    $("#val-N12affectedgrossmargin").text(N12affectedgrossmargin.toFixed(2).toString());
    $("#val-L12unaffectedgrossmargin").text(L12unaffectedgrossmargin.toFixed(2).toString());
    $("#val-N12unaffectedgrossmargin").text(N12unaffectedgrossmargin.toFixed(2).toString());
    $("#val-L12totalgrossmargin").text(L12totalgrossmargin.toFixed(2).toString());
    $("#val-N12totalgrossmargin").text(N12totalgrossmargin.toFixed(2).toString());
    $("#val-Changegrossmargin").text(Changegrossmargin.toFixed(2).toString());
    //GROSS MARGIN
    var L12affectedgrossmargincase = L12affectedgrossmargin / L12affectedCases;
    var N12affectedgrossmargincase = N12affectedgrossmargin / N12affectedCases;
    var L12unaffectedgrossmargincase = L12unaffectedgrossmargin / L12unaffectedCases;
    if (!isFinite(L12unaffectedgrossmargincase)) {
        L12unaffectedgrossmargincase = 0;
    }
    var N12unaffectedgrossmargincase = N12unaffectedgrossmargin / N12unaffectedCases;
    if (!isFinite(N12unaffectedgrossmargincase)) {
        N12unaffectedgrossmargincase = 0;
    }
    var L12totalgrossmargincase = L12totalgrossmargin / L12totalcases;
    var N12totalgrossmargincase = N12totalgrossmargin / N12totalcases;
    var Changegrossmargincase = N12totalgrossmargincase - L12totalgrossmargincase;
    //update
    $("#val-L12affectedgrossmargincase").text(L12affectedgrossmargincase.toFixed(2).toString());
    $("#val-N12affectedgrossmargincase").text(N12affectedgrossmargincase.toFixed(2).toString());
    $("#val-L12unaffectedgrossmargincase").text(L12unaffectedgrossmargincase.toFixed(2).toString());
    $("#val-N12unaffectedgrossmargincase").text(N12unaffectedgrossmargincase.toFixed(2).toString());
    $("#val-L12totalgrossmargincase").text(L12totalgrossmargincase.toFixed(2).toString());
    $("#val-N12totalgrossmargincase").text(N12totalgrossmargincase.toFixed(2).toString());
    $("#val-Changegrossmargincase").text(Changegrossmargincase.toFixed(2).toString());
    //VAR OPEX
    var L12affectedvaropex = parseFloat($("#val-Affectedl12varopex").val());
    var N12affectedvaropex = parseFloat($("#val-Affectedn12varopex").val());
    var L12unaffectedvaropex = parseFloat($("#val-Unaffectedl12varopex").val());
    var N12unaffectedvaropex = parseFloat($("#val-Unaffectedn12varopex").val());
    var L12totalvaropex = L12affectedvaropex + L12unaffectedvaropex;
    var N12totalvaropex = N12affectedvaropex + N12unaffectedvaropex;
    var Changevaropex = N12totalvaropex - L12totalvaropex;
    //update
    $("#val-L12affectedvaropex").text(L12affectedvaropex.toFixed(2).toString());
    $("#val-N12affectedvaropex").text(N12affectedvaropex.toFixed(2).toString());
    $("#val-L12unaffectedvaropex").text(L12unaffectedvaropex.toFixed(2).toString());
    $("#val-N12unaffectedvaropex").text(N12unaffectedvaropex.toFixed(2).toString());
    $("#val-L12totalvaropex").text(L12totalvaropex.toFixed(2).toString());
    $("#val-N12totalvaropex").text(N12totalvaropex.toFixed(2).toString());
    $("#val-Changevaropex").text(Changevaropex.toFixed(2).toString());
    //VAR OPEX
    var L12affectedvaropex = parseFloat($("#val-Affectedl12varopex").val());
    var N12affectedvaropex = parseFloat($("#val-Affectedn12varopex").val());
    var L12unaffectedvaropex = parseFloat($("#val-Unaffectedl12varopex").val());
    var N12unaffectedvaropex = parseFloat($("#val-Unaffectedn12varopex").val());
    var L12totalvaropex = L12affectedvaropex + L12unaffectedvaropex;
    var N12totalvaropex = N12affectedvaropex + N12unaffectedvaropex;
    var Changevaropex = N12totalvaropex - L12totalvaropex;
    //update
    $("#val-L12affectedvaropex").text(L12affectedvaropex.toFixed(2).toString());
    $("#val-N12affectedvaropex").text(N12affectedvaropex.toFixed(2).toString());
    $("#val-L12unaffectedvaropex").text(L12unaffectedvaropex.toFixed(2).toString());
    $("#val-N12unaffectedvaropex").text(N12unaffectedvaropex.toFixed(2).toString());
    $("#val-L12totalvaropex").text(L12totalvaropex.toFixed(2).toString());
    $("#val-N12totalvaropex").text(N12totalvaropex.toFixed(2).toString());
    $("#val-Changevaropex").text(Changevaropex.toFixed(2).toString());
    //CONTRIBUTION MARGIN
    var L12affectedconmargin = L12affectedgrossmargin - L12affectedvaropex;
    var N12affectedconmargin = N12affectedgrossmargin - N12affectedvaropex;
    var L12unaffectedconmargin = L12unaffectedgrossmargin - L12unaffectedvaropex;
    var N12unaffectedconmargin = N12unaffectedgrossmargin - N12unaffectedvaropex;
    var L12totalconmargin = L12affectedconmargin + L12unaffectedconmargin;
    var N12totalconmargin = N12affectedconmargin + N12unaffectedconmargin;
    var Changeconmargin = N12totalconmargin - L12totalconmargin;
    //update
    $("#val-L12affectedconmargin").text(L12affectedconmargin.toFixed(2).toString());
    $("#val-N12affectedconmargin").text(N12affectedconmargin.toFixed(2).toString());
    $("#val-L12unaffectedconmargin").text(L12unaffectedconmargin.toFixed(2).toString());
    $("#val-N12unaffectedconmargin").text(N12unaffectedconmargin.toFixed(2).toString());
    $("#val-L12totalconmargin").text(L12totalconmargin.toFixed(2).toString());
    $("#val-N12totalconmargin").text(N12totalconmargin.toFixed(2).toString());
    $("#val-Changeconmargin").text(Changeconmargin.toFixed(2).toString());
    //CONTRIBUTION MARGIN / CASE
    var L12affectedconmargincase = L12affectedconmargin / L12affectedCases;
    var N12affectedconmargincase = N12affectedconmargin / N12affectedCases;
    var L12unaffectedconmargincase = L12unaffectedconmargin / L12unaffectedCases;
    if (!isFinite(L12unaffectedconmargincase)) {
        L12unaffectedconmargincase = 0;
    }
    var N12unaffectedconmargincase = N12unaffectedconmargin / N12unaffectedCases;
    if (!isFinite(N12unaffectedconmargincase)) {
        N12unaffectedconmargincase = 0;
    }
    var L12totalconmargincase = L12totalconmargin / L12totalcases;
    var N12totalconmargincase = N12totalconmargin / N12totalcases;
    var Changeconmargincase = N12totalconmargincase - L12totalconmargincase;
    //update
    $("#val-L12affectedconmargincase").text(L12affectedconmargincase.toFixed(2).toString());
    $("#val-N12affectedconmargincase").text(N12affectedconmargincase.toFixed(2).toString());
    $("#val-L12unaffectedconmargincase").text(L12unaffectedconmargincase.toFixed(2).toString());
    $("#val-N12unaffectedconmargincase").text(N12unaffectedconmargincase.toFixed(2).toString());
    $("#val-L12totalconmargincase").text(L12totalconmargincase.toFixed(2).toString());
    $("#val-N12totalconmargincase").text(N12totalconmargincase.toFixed(2).toString());
    $("#val-Changeconmargincase").text(Changeconmargincase.toFixed(2).toString());
    //DIRECT OPEX
    var L12totaldirectopex = parseFloat($("#val-L12directopex").val());
    var N12totaldirectopex = parseFloat($("#val-N12directopex").val());
    var Changedirectopex = N12totaldirectopex - L12totaldirectopex;
    //CONTRIBUTION AFTER OPEX MARGIN

    var L12totalconmarginA = L12totalconmargin - L12totaldirectopex;
    var N12totalconmarginA = N12totalconmargin - N12totaldirectopex;
    var ChangeconmarginA = N12totalconmarginA - L12totalconmarginA
    //update
    $("#val-L12totalconmarginA").text(L12totalconmarginA.toFixed(2).toString());
    $("#val-N12totalconmarginA").text(N12totalconmarginA.toFixed(2).toString());
    $("#val-ChangeconmarginA").text(ChangeconmarginA.toFixed(2).toString());
    //CONTRIBUTION AFTER OPEX MARGIN / CASE
    var L12totalconmarginAcase = L12totalconmarginA / L12totalcases;
    var N12totalconmarginAcase = N12totalconmarginA / N12totalcases;
    var ChangeconmarginAcase = N12totalconmarginAcase - L12totalconmarginAcase;
    //update
    $("#val-L12totalconmarginAcase").text(L12totalconmarginAcase.toFixed(2).toString());
    $("#val-N12totalconmarginAcase").text(N12totalconmarginAcase.toFixed(2).toString());
    $("#val-ChangeconmarginAcase").text(ChangeconmarginAcase.toFixed(2).toString());

    //Change Color
    changecolor();
}