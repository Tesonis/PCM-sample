function calculateCostingAllItems(entryfield, cmtype) {

    //Pulled Numbers
    var unitspercase = parseInt($("#data-unitspercase").val());
    var currppc = parseFloat($("#data-currppc").val());
    var currlandedcost = parseFloat($("#data-currlandedcost").val());
    var currwholesaleppc = parseFloat($("#data-currwholesaleppc").val());
    var currdsdppc = parseFloat($("#data-currdsdppc").val());
    var propwholesalesuggestedppc = parseFloat($("#data-currwholesalesuggestedppc").val());
    var currdsdsuggestedppc = parseFloat($("#data-currwholesalesuggestedppc").val());
    //Input Values
    var propppc = parseFloat($("#data-propppc").val());
    var exchangerate = parseFloat($("#data-exchangerate").val());
    var duty = parseFloat($("#data-duty").val());
    var dealaccrual = parseFloat($("#data-dealaccrual").val());
    var cashterms = parseFloat($("#data-cashterms").val());
    var other = parseFloat($("#data-other").val());
    var spoilage = parseFloat($("#data-spoilage").val());
    var branddevelopmentfund = parseFloat($("#data-branddevelopmentfund").val());
    var billback = parseFloat($("#data-billback").val());
    //Wholesale Trade Margin input
    var currwholesaletrademargin = parseFloat($("#data-currwholesaletrademargin").val());
    var propwholesaletrademargin = parseFloat($("#data-propwholesaletrademargin").val());
    //DSD Trade Margin input
    var currdsdtrademargin = parseFloat($("#data-currdsdtrademargin").val());
    var propdsdtrademargin = parseFloat($("#data-propdsdtrademargin").val());
    //Wholesale SRP
    var propwholesalesuggestedppc = parseFloat($("#data-propwholesalesuggestedppc").val());
    //DSD Suggested SRP
    var propdsdsuggestedppc = parseFloat($("#data-propdsdsuggestedppc").val());


    //Cad Equiv
    var propcadequivalent = Number((propppc * exchangerate).toFixed(2));

    //Freight Costing
    if (entryfield == "freightperc") {
        var freightcostingperc = parseFloat($("#data-freightcostingperc").val());
        var freightcosting = parseFloat((propcadequivalent * (freightcostingperc / 100)).toFixed(2))
        $("#data-freightcosting").val(freightcosting.toFixed(2))
    }
    else {
        var freightcosting = parseFloat($("#data-freightcosting").val());
        var freightcostingperc = parseFloat(((freightcosting / propcadequivalent) * 100).toFixed(2))
        $("#data-freightcostingperc").val(freightcostingperc.toFixed(2))
    }

    //Prop Duty & Landed
    var propduty = parseFloat((duty / 100 * propcadequivalent).toFixed(2));
    var proplandedcost = parseFloat((propcadequivalent + propduty + freightcosting).toFixed(2));

    //New Item without current pricing
    if ($("#data-currwholesaleppc").val() == null || $("#data-currwholesaleppc").val() == "") {
        if (entryfield == "wholesale") {
            var propwholesaleppc = parseFloat($("#data-propwholesaleppc").val());
        }
        else {
            var propwholesaleppc = (proplandedcost / currlandedcost * currwholesaleppc)
            $("#data-propwholesaleppc").val(propwholesaleppc.toFixed(2));
        }
    }
    else {
        var propwholesaleppc = parseFloat($("#data-propwholesaleppc").val());
    }
    if (cmtype != "baseline") {
        var propdsdppc = parseFloat(($("#data-propwholesaleppc").val() / 0.95 / unitspercase).toFixed(2)) * unitspercase;
        if (entryfield == "wholesale") {
            //Calc Wholesale SRP
            var propwholesalesuggestedppc = parseFloat((propwholesaleppc / unitspercase / ((100 - propwholesaletrademargin) / 100)).toFixed(1)) - 0.01;
            propwholesalesuggestedppc = parseFloat(propwholesalesuggestedppc.toFixed(2));
            //Calc DSD SRP
            var propdsdsuggestedppc = parseFloat((propdsdppc / unitspercase / ((100 - propdsdtrademargin) / 100)).toFixed(1)) - 0.01;
            propdsdsuggestedppc = parseFloat(propdsdsuggestedppc.toFixed(2));
        }
    }
    //Calculate DSD Price from Wholesale if type is not baseline, if baseline, then match it
    
    else {
        var propdsdppc = parseFloat($("#data-propdsdppc").val());
    }

    //DSD PPC input calc
    if (entryfield == "dsd") {
        //var propwholesaleppc = parseFloat(($("#data-propdsdppc").val() * 0.95 / unitspercase).toFixed(2)) * unitspercase;
        //Calc Wholesale SRP
        var propwholesalesuggestedppc = parseFloat((propwholesaleppc / unitspercase / ((100 - propwholesaletrademargin) / 100)).toFixed(1)) - 0.01;
        propwholesalesuggestedppc = parseFloat(propwholesalesuggestedppc.toFixed(2));
        //Calc DSD SRP
        var propdsdsuggestedppc = parseFloat((propdsdppc / unitspercase / ((100 - propdsdtrademargin) / 100)).toFixed(1)) - 0.01;
        propdsdsuggestedppc = parseFloat(propdsdsuggestedppc.toFixed(2));
    }

    //deal accrual from dsd price
    var currdealaccrual = parseFloat((dealaccrual / 100 * currdsdppc).toFixed(2));
    var propdealaccrual = parseFloat((dealaccrual / 100 * propdsdppc).toFixed(2));
    
    //Suggested Retail Price -> Trade Margin Calc
    if (entryfield == "wholesalesuggested") {
        var propwholesaletrademargin = ((100 - (propwholesaleppc / (propwholesalesuggestedppc) / unitspercase) * 100));
        var propwholesalesuggestedppc = parseFloat($("#data-propwholesalesuggestedppc").val());
        var propdsdsuggestedppc = propwholesalesuggestedppc;
        var propdsdtrademargin = ((100 - (propdsdppc / propdsdsuggestedppc / unitspercase) * 100));
    }
    else if (entryfield == "wholesaletrade") {
        //Calc Wholesale SRP
        var propwholesalesuggestedppc = parseFloat((propwholesaleppc / unitspercase / ((100 - propwholesaletrademargin) / 100)).toFixed(1)) - 0.01;
        propwholesalesuggestedppc = parseFloat(propwholesalesuggestedppc.toFixed(2));
    }
    else if (entryfield == "dsdtrade") {
        //Calc DSD SRP
        var propdsdsuggestedppc = parseFloat((propdsdppc / unitspercase / ((100 - propdsdtrademargin) / 100)).toFixed(1)) - 0.01;
        propdsdsuggestedppc = parseFloat(propdsdsuggestedppc.toFixed(2));
    }
    else {
        //Calc Wholesale SRP
        var propwholesalesuggestedppc = parseFloat((propwholesaleppc / unitspercase / ((100 - propwholesaletrademargin) / 100)).toFixed(1)) - 0.01;
        propwholesalesuggestedppc = parseFloat(propwholesalesuggestedppc.toFixed(2));
        //Calc DSD SRP
        var propdsdsuggestedppc = parseFloat((propdsdppc / unitspercase / ((100 - propdsdtrademargin) / 100)).toFixed(2)) - 0.01;
        propdsdsuggestedppc = parseFloat(propdsdsuggestedppc.toFixed(2));
    }

    //Rest of the form calculations
    //Wholesale section
    var currwholesaledivisibility = parseFloat(((currwholesaleppc) / unitspercase).toFixed(4));
    var propwholesaledivisibility = parseFloat(((propwholesaleppc) / unitspercase).toFixed(4));
    var currwholesalerecommendedprice = parseFloat((parseFloat(currwholesaledivisibility.toFixed(2)) * unitspercase).toFixed(2));
    var propwholesalerecommendedprice = parseFloat((parseFloat(propwholesaledivisibility.toFixed(2)) * unitspercase).toFixed(2));
    var currwholesalenetinvoice = parseFloat((currwholesaleppc - currdealaccrual).toFixed(2));
    var propwholesalenetinvoice = parseFloat((propwholesaleppc - propdealaccrual).toFixed(2));
    var currwholesalegrossmarginpercase = parseFloat((currwholesalenetinvoice - currlandedcost).toFixed(2));
    var propwholesalegrossmarginpercase = parseFloat((propwholesalenetinvoice - proplandedcost).toFixed(2));
    var currwholesalegrossmarginperc = (currwholesalegrossmarginpercase / currwholesalenetinvoice);
    var propwholesalegrossmarginperc = (propwholesalegrossmarginpercase / propwholesalenetinvoice);

    //Extra Components
    var currcashterms = parseFloat((cashterms / 100 * currwholesalenetinvoice).toFixed(2));
    var propcashterms = parseFloat((cashterms / 100 * propwholesalenetinvoice).toFixed(2));
    var propother = parseFloat((other / 100 * propwholesalenetinvoice).toFixed(2));
    var propspoilage = parseFloat((spoilage / 100 * propwholesalenetinvoice).toFixed(2));
    var propbranddevelopmentfund = parseFloat((branddevelopmentfund / 100 * propwholesalenetinvoice).toFixed(2));
    var propbillback = parseFloat((billback / 100 * propwholesalenetinvoice).toFixed(2));

    var currother = parseFloat((other / 100 * currwholesalenetinvoice).toFixed(2));
    var currspoilage = parseFloat((spoilage / 100 * currwholesalenetinvoice).toFixed(2));
    var currbranddevelopmentfund = parseFloat((branddevelopmentfund / 100 * currwholesalenetinvoice).toFixed(2));
    var currbillback = parseFloat((billback / 100 * currwholesalenetinvoice).toFixed(2));

    //Wholesale Margin
    var currwholesalenetmarginpercase = parseFloat((currwholesalegrossmarginpercase - currcashterms - currother - currspoilage - currbranddevelopmentfund - currbillback).toFixed(2));
    var propwholesalenetmarginpercase = parseFloat((propwholesalegrossmarginpercase - propcashterms - propother - propspoilage - propbranddevelopmentfund - propbillback).toFixed(2));
    var currwholesalenetmarginperc = (currwholesalenetmarginpercase / (currwholesalenetinvoice - currcashterms - currother - currspoilage - currbranddevelopmentfund - currbillback));
    var propwholesalenetmarginperc = (propwholesalenetmarginpercase / (propwholesalenetinvoice - propcashterms - propother - propspoilage - propbranddevelopmentfund - propbillback));


    //DSD rest of calculations
    var currdsdnetinvoice = parseFloat((currdsdppc - currdealaccrual).toFixed(2));
    var propdsdnetinvoice = parseFloat((propdsdppc - propdealaccrual).toFixed(2));
    var currdsdgrossmarginpercase = parseFloat((currdsdnetinvoice - currlandedcost).toFixed(2));
    var propdsdgrossmarginpercase = parseFloat((propdsdnetinvoice - proplandedcost).toFixed(2));
    var currdsdgrossmarginperc = parseFloat((currdsdgrossmarginpercase / currdsdnetinvoice).toFixed(2));
    var propdsdgrossmarginperc = parseFloat((propdsdgrossmarginpercase / propdsdnetinvoice).toFixed(2));

    var currdsdnetmarginpercase = parseFloat((currdsdgrossmarginpercase - currcashterms - currother - currspoilage - currbranddevelopmentfund - currbillback).toFixed(2));
    var propdsdnetmarginpercase = parseFloat((propdsdgrossmarginpercase - propcashterms - propother - propspoilage - propbranddevelopmentfund - propbillback).toFixed(2));
    var currdsdnetmarginperc = parseFloat((currdsdnetmarginpercase / (currdsdnetinvoice - currcashterms - currother - currspoilage - currbranddevelopmentfund - currbillback)).toFixed(2));
    var propdsdnetmarginperc = parseFloat((propdsdnetmarginpercase / (propdsdnetinvoice - propcashterms - propother - propspoilage - propbranddevelopmentfund - propbillback)).toFixed(2));


    //Plug numbers back into view
    $("#data-currcashterms").text(currcashterms.toFixed(2));
    $("#data-currother").text(currother.toFixed(2));
    $("#data-currspoilage").text(currspoilage.toFixed(2));
    $("#data-currbranddevelopmentfund").text(currbranddevelopmentfund.toFixed(2));
    $("#data-currbillback").text(currbillback.toFixed(2));

    //Proposed Column General
    $("#data-propppc").text(propppc.toFixed(2));
    $("#data-cadequivalent").text(propcadequivalent.toFixed(2));
    $("#data-propduty").text(propduty.toFixed(2));
    $("#data-propfreightcosting").text(freightcosting.toFixed(2));
    $("#data-proplandedcost").text(proplandedcost.toFixed(2));
    $("#data-propcashterms").text(propcashterms.toFixed(2));
    $("#data-propother").text(propother.toFixed(2));
    $("#data-propspoilage").text(propspoilage.toFixed(2));
    $("#data-propbranddevelopmentfund").text(propbranddevelopmentfund.toFixed(2));
    $("#data-propbillback").text(propbillback.toFixed(2));
    $("#data-currwholesaleppc").text(currwholesaleppc.toFixed(2));
    $("#data-currdsdppc").text(currdsdppc.toFixed(2));
    $("#data-propwholesaleppc").val(Number(propwholesaleppc).toFixed(2));
    $("#data-propdsdppc").val(Number(propdsdppc).toFixed(2));
    $("#data-currdealaccrual").text(currdealaccrual.toFixed(2));
    $("#data-propdealaccrual").text(propdealaccrual.toFixed(2));
    $("#data-currwholesaledivisibility").text(currwholesaledivisibility.toFixed(4));
    $("#data-propwholesaledivisibility").text(propwholesaledivisibility.toFixed(4));
    $("#data-propwholesalesuggestedppc").val(propwholesalesuggestedppc.toFixed(2));
    $("#data-propwholesaletrademargin").val(Number(propwholesaletrademargin).toFixed(2));
    $("#data-propdsdtrademargin").val(Number(propdsdtrademargin).toFixed(2));
    $("#data-currwholesalenetinvoice").text(currwholesalenetinvoice.toFixed(2));
    $("#data-propwholesalenetinvoice").text(propwholesalenetinvoice.toFixed(2));
    $("#data-currwholesalegrossmarginpercase").text(currwholesalegrossmarginpercase.toFixed(2));
    $("#data-propwholesalegrossmarginpercase").text(propwholesalegrossmarginpercase.toFixed(2));
    $("#data-currwholesalegrossmarginperc").text((currwholesalegrossmarginperc * 100).toFixed(2));
    $("#data-propwholesalegrossmarginperc").text((propwholesalegrossmarginperc * 100).toFixed(2));
    $("#data-currwholesalenetmarginpercase").text(currwholesalenetmarginpercase.toFixed(2));
    $("#data-propwholesalenetmarginpercase").text(propwholesalenetmarginpercase.toFixed(2));
    $("#data-currwholesalenetmarginperc").text((currwholesalenetmarginperc * 100).toFixed(2));
    $("#data-propwholesalenetmarginperc").text((propwholesalenetmarginperc * 100).toFixed(2));
    $("#data-currwholesalerecommendedppc").text(currwholesalerecommendedprice.toFixed(2));
    $("#data-propwholesalerecommendedppc").text(propwholesalerecommendedprice.toFixed(2));
    $("#data-currdsdsuggestedppc").val(currdsdsuggestedppc.toFixed(2));
    $("#data-propdsdsuggestedppc").val(propdsdsuggestedppc.toFixed(2));
    $("#data-currdsdnetinvoice").text(currdsdnetinvoice.toFixed(2));
    $("#data-propdsdnetinvoice").text(propdsdnetinvoice.toFixed(2));
    $("#data-currdsdgrossmarginpercase").text(currdsdgrossmarginpercase.toFixed(2));
    $("#data-propdsdgrossmarginpercase").text(propdsdgrossmarginpercase.toFixed(2));
    $("#data-currdsdgrossmarginperc").text((currdsdgrossmarginperc * 100).toFixed(2));
    $("#data-propdsdgrossmarginperc").text((propdsdgrossmarginperc * 100).toFixed(2));
    $("#data-currdsdnetmarginpercase").text(currdsdnetmarginpercase.toFixed(2));
    $("#data-propdsdnetmarginpercase").text(propdsdnetmarginpercase.toFixed(2));
    $("#data-currdsdnetmarginperc").text((currdsdnetmarginperc * 100).toFixed(2));
    $("#data-propdsdnetmarginperc").text((propdsdnetmarginperc * 100).toFixed(2));

    //MarginAnalysis

    var currcasevolume = parseInt($("#data-currcasevolume").val());
    var currrevenueFormat = parseFloat($("#data-currrevenueFormat").val())
    if (currcasevolume == 0 || currrevenueFormat == 0) {
        $("#data-proprevenue").val(0);
        $("#data-curraveragesellingppc").text("0");
        $("#data-propaveragesellingppc").text("0");
        $("#data-currmarginpercase").text("0");
        $("#data-propmarginpercase").text("0");
        $("#data-currmargintotal").text("0");
        $("#data-propmargintotal").text("0");
        $("#data-currmarginperc").text("0");
        $("#data-propmarginperc").text("0");
    }
    else {
        var propcasevolume = parseInt($("#data-propcasevolume").val());
        var currtotalsales = parseFloat($("#data-currrevenue").val().replace(/,/g, ''));
        var pricemultiplier = Number(((propwholesaleppc - currwholesaleppc) / currwholesaleppc) + 1);
        var curraveragesellingprice = Number((currtotalsales / currcasevolume).toFixed(2));
        var propaveragesellingprice = Number((curraveragesellingprice * pricemultiplier).toFixed(2));
        var proptotalsales = Number((propcasevolume * propaveragesellingprice).toFixed(2));
        var currmarginpercase = Number((curraveragesellingprice - currlandedcost).toFixed(2));
        var propmarginpercase = Number((propaveragesellingprice - proplandedcost).toFixed(2));
        var currmargintotal = Number((currmarginpercase * currcasevolume).toFixed(2));
        var propmargintotal = Number((propmarginpercase * propcasevolume).toFixed(2));
        var currmarginperc = Number((currmargintotal / currtotalsales * 100).toFixed(2));
        var propmarginperc = Number((propmargintotal / proptotalsales * 100).toFixed(2));
        $("#data-proprevenue").val(numberWithCommas(proptotalsales.toFixed(2)));
        $("#data-curraveragesellingppc").text(curraveragesellingprice.toFixed(2));
        $("#data-propaveragesellingppc").text(propaveragesellingprice.toFixed(2));
        $("#data-currmarginpercase").text(currmarginpercase.toFixed(2));
        $("#data-propmarginpercase").text(propmarginpercase.toFixed(2));
        $("#data-currmargintotal").text(numberWithCommas(currmargintotal.toFixed(2)));
        $("#data-propmargintotal").text(numberWithCommas(propmargintotal.toFixed(2)));
        $("#data-currmarginperc").text(currmarginperc.toFixed(2));
        $("#data-propmarginperc").text(propmarginperc.toFixed(2));
    }
}
