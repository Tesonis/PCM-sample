using IBM.Data.DB2.iSeries;
using Newtonsoft.Json;
using PCM.Models;
using PCM.Models.PCMViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TOLC.ERP.Application;
using CostingModel = PCM.Models.PCMViewModels.CostingModel;
using ERP = TOLC.ERP.Application;

namespace PCM.Controllers
{
    public class HomeController : Controller
    {
        
        [HttpGet]
        public ActionResult PCMMain()
        {
            try
            {
                TempData["errorcode"] = null;
                IndexViewModel vm = RetrieveUserData(Request.Cookies["SecToken"]["UserName"]);
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                vm.errorvm = new ErrorViewModel
                {
                    errorcode = TempData["errorcode"].ToString()
                };
                return View(vm);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpGet]
        public ActionResult PriceChange(string id, string cmid)
        {
            if (cmid != null)
            {
                try
                {
                    TempData["errorcode"] = 0;
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                    CMViewModel vm = new CMViewModel
                    {
                        Brand = GetBrands(),
                        PriceZone = PriceZone(),
                        CMType = CMType(),
                        errorvm = new ErrorViewModel()
                    };
                    if (id == null)
                    {
                        //Get Costing Model Data and Stuff
                        vm.Costingmodel = new CostingModel
                        {
                            //load in data
                            ModelID = cmid
                        };
                        //Get CM From DB
                        CostingModelPayload cmpayload = new CostingModelPayload();
                        ReturnValue rv = new ReturnValue();
                        try
                        {
                            rv = new ERP.CostingModel().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(cmid), ref cmpayload);
                            TempData["errorcode"] = rv.Number;
                            //Get modelid of model
                            if (rv.Number == 0 && cmpayload != null)
                            {
                                vm.Costingmodel.ModelID = cmpayload.ID.ToString();
                                vm.Costingmodel.Brand = cmpayload.brand;
                                switch (cmpayload.priceZone)
                                {
                                    case Pricing.PriceZone.Zone1:
                                        vm.Costingmodel.Zone = "Zone 1 Only";
                                        break;
                                    case Pricing.PriceZone.Zone3:
                                        vm.Costingmodel.Zone = "Zone 3 Only";
                                        break;
                                    case Pricing.PriceZone.ZoneAll:
                                        vm.Costingmodel.Zone = "National";
                                        break;
                                }
                                vm.Costingmodel.Modelname = cmpayload.name;
                                ViewBag.modelID = vm.Costingmodel.ModelID;

                            }

                            //get brand list of items from brand
                            vm.Costingmodel.Data = new CostingModelData();
                            vm.Costingmodel.Data.Itemgroups = GetGroups(cmpayload.ID.ToString());


                        }
                        catch (InvalidOperationException e)
                        {
                        }

                    }
                    //CostingModel id = null
                    else
                    {

                    }
                    return View(vm);
                }
                catch
                {
                    return RedirectToAction("PCMMain", "Home");
                }
            }
            else
            {
                return RedirectToAction("PCMMain", "Home");
            }
        }
        [HttpGet]
        public ActionResult CostingModelReview(string id, string gid)
        {
            try
            {
                var secToken = Request.Cookies["SecToken"]["SecurityKey"];
                CMViewModel vm = new CMViewModel
                {
                    Brand = GetBrands(),
                    PriceZone = PriceZone(),
                    CMType = CMType(),
                    errorvm = new ErrorViewModel()
                };

                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                //Check for id

                if (id != null)
                {
                    //Get Costing Model Data and Stuff
                    vm.Costingmodel = new CostingModel
                    {
                        //load in data
                        ModelID = id
                    };
                    //Get CM From DB
                    CostingModelPayload cmpayload = new CostingModelPayload();
                    ReturnValue rv = new ReturnValue();
                    try
                    {
                        rv = new ERP.CostingModel().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(id), ref cmpayload);
                        TempData["errorcode"] = rv.Number;
                            //Get modelid of model
                            if (rv.Number == 0 && cmpayload != null)
                            {
                                vm.Costingmodel.ModelID = cmpayload.ID.ToString();
                                vm.Costingmodel.Brand = cmpayload.brand;
                                vm.Costingmodel.Vendor = cmpayload.vendor;
                                vm.Costingmodel.Zone = cmpayload.priceZone.ToString();
                                vm.Costingmodel.Type = cmpayload.type.ToString();
                                vm.Costingmodel.Modelname = cmpayload.name;
                            vm.Costingmodel.Status = cmpayload.Status;
                                //add zone
                                ViewBag.modelID = vm.Costingmodel.ModelID;
                            
                        }
                            //Get Vendor
                            vm.Vendor = GetAllVendors();

                        //get brand list of items from brand
                        vm.Costingmodel.Data = new CostingModelData();
                        vm.Costingmodel.Data.Comment = cmpayload.comment;
                        vm.Costingmodel.LastUpdated = cmpayload.LastModifiedDate;
                        vm.Costingmodel.Bdm = cmpayload.CreatedUser;
                        vm.Costingmodel.Data.Itemgroups = GetGroups(cmpayload.ID.ToString());
                        var currgroup = vm.Costingmodel.Data.Itemgroups.First();
                        string groupid;
                        if (gid == "" || gid == null)
                        {
                            groupid = currgroup.id;
                        }
                        else
                        {
                            groupid = gid;
                        }
                        //get brand list of items from brand
                        vm.Branditems = GetBrandItems(id, cmpayload.brand, cmpayload.vendor, cmpayload.priceZone.ToString());
                        vm.currItemgroup = GetCurrGroup(groupid, vm);

                        //Get All Used items from Groups
                        vm.RestItemsList = GetUsedItems(vm);


                        //if (vm.Costingmodel.Data.Itemgroups.Count() == 0)
                        //{
                        //    vm.currItemgroup = new Itemgroup("GroupID", "")
                        //}
                        //else
                        //{
                        //    vm.currItemgroup = GetCurrGroup(gid, vm);
                        //}

                        //else if (step == 3)
                        //{
                        //    if (rv.Number == 0 && cmpayload != null)
                        //    {
                        //        vm.Costingmodel.ModelID = cmpayload.ID.ToString();
                        //        vm.Costingmodel.Modelname = cmpayload.name;
                        //        vm.Costingmodel.Brand = cmpayload.brand;
                        //        vm.Costingmodel.Vendor = cmpayload.vendor;
                        //        vm.Costingmodel.Zone = cmpayload.priceZone.ToString();
                        //        vm.Costingmodel.Type = cmpayload.type.ToString();
                        //        vm.Costingmodel.Rejectcomment = cmpayload.rejectedComment;
                        //        vm.Costingmodel.Data = new CostingModelData();
                        //        vm.Costingmodel.Data.Comment = cmpayload.comment;
                        //        vm.Costingmodel.LastUpdated = cmpayload.LastModifiedDate;
                        //        vm.Costingmodel.Bdm = cmpayload.CreatedUser;
                        //    }

                        //}
                    }
                    catch (InvalidOperationException e)
                    {
                    }

                }
                //CostingModel id = null
                else
                {
                    vm.Costingmodel = new CostingModel
                    {
                        Data = new CostingModelData(),
                    };
                    vm.Branditems = new List<Branditem>();
                    vm.Costingmodel.Data.Itemgroups = new List<Itemgroup>();
                    vm.Costingmodel.Type = "-1";
                    vm.errorvm.errorcode = null;

                }
                return View(vm);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
        [HttpGet]
        public ActionResult CostingModel(string id, int? step, string p, string gid)
        {
            try
            {
                var secToken = Request.Cookies["SecToken"]["SecurityKey"];
                if (p == "delete")
                {
                    DeleteCostingModel(id);
                    return RedirectToAction("PCMMain", "Home");
                }
                CMViewModel vm = new CMViewModel
                {
                    Brand = GetBrands(),
                    PriceZone = PriceZone(),
                    CMType = CMType(),
                    errorvm = new ErrorViewModel()
                };
                
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                
                    //Check for id
                    if (step == null) { step = 1; }
                
                    if (id != null)
                    {
                        //Get Costing Model Data and Stuff
                        vm.Costingmodel = new CostingModel
                        {
                            //load in data
                            ModelID = id
                        };
                        vm.Step = step.GetValueOrDefault(1);
                        //Get CM From DB
                        CostingModelPayload cmpayload = new CostingModelPayload();
                        ReturnValue rv = new ReturnValue();
                        try
                        {
                            rv = new ERP.CostingModel().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(id), ref cmpayload);
                            TempData["errorcode"] = rv.Number;
                            if (step == 1)
                            {
                                //Get modelid of model
                                if (rv.Number == 0 && cmpayload != null)
                                {
                                    vm.Costingmodel.ModelID = cmpayload.ID.ToString();
                                    vm.Costingmodel.Brand = cmpayload.brand;
                                    vm.Costingmodel.Vendor = cmpayload.vendor;
                                    vm.Costingmodel.Zone = cmpayload.priceZone.ToString();
                                    vm.Costingmodel.Type = cmpayload.cmType.ToString();
                                    vm.Costingmodel.Modelname = cmpayload.name;
                                    //add zone
                                    ViewBag.modelID = vm.Costingmodel.ModelID;
                                }
                                //Get Vendor
                                vm.Vendor = GetAllVendors();
                            }
                            else if (step == 2)
                            {
                                if (rv.Number == 0 && cmpayload != null)
                                {
                                    vm.Costingmodel.ModelID = cmpayload.ID.ToString();
                                    vm.Costingmodel.Brand = cmpayload.brand;
                                    vm.Costingmodel.Vendor = cmpayload.vendor;
                                    vm.Costingmodel.Type = cmpayload.cmType.ToString();
                                    ViewBag.cmtype = vm.Costingmodel.Type;
                            }

                                //get brand list of items from brand
                                vm.Branditems = GetBrandItems(id, cmpayload.brand, cmpayload.vendor, cmpayload.priceZone.ToString());
                                vm.Costingmodel.Data = new CostingModelData();
                                vm.Costingmodel.Data.Itemgroups = GetGroups(cmpayload.ID.ToString());
                                if (gid == null || gid == "")
                                {
                                    vm.currItemgroup = new Itemgroup("GroupID", "");
                                }
                                else
                                {

                                    if (vm.Costingmodel.Data.Itemgroups.Count() == 0)
                                    {
                                        vm.currItemgroup = new Itemgroup("GroupID", "");

                                    }
                                    else
                                    {
                                        vm.currItemgroup = GetCurrGroup(gid, vm);
                                    }


                                }
                                //Get All Used items from Groups
                                vm.RestItemsList = GetUsedItems(vm);
                            }
                            //else if (step == 3)
                            //{
                            //    if (rv.Number == 0 && cmpayload != null)
                            //    {
                            //        vm.Costingmodel.ModelID = cmpayload.ID.ToString();
                            //        vm.Costingmodel.Brand = cmpayload.brand;
                                    
                            //    }
                            //    if (TempData["step3data"] != null)
                            //{
                            //    string[] arr = TempData["step3data"] as string[];
                            //    vm.Costingmodel.Data = new CostingModelData
                            //    {
                            //        Affectedl12cases = Int32.Parse(arr[0]),
                            //        Affectedl12price = Decimal.Parse(arr[1]),
                            //        Unaffectedl12cases = Int32.Parse(arr[2]),
                            //        Unaffectedl12price = Decimal.Parse(arr[3]),
                            //        Affectedn12cases = Int32.Parse(arr[4]),
                            //        Affectedn12price = Decimal.Parse(arr[5]),
                            //        Unaffectedn12cases = Int32.Parse(arr[6]),
                            //        Unaffectedn12price = Decimal.Parse(arr[7]),
                            //        Affectedl12varopex = 0,
                            //        Affectedn12varopex = 0,
                            //        Unaffectedl12varopex = 0,
                            //        Unaffectedn12varopex = 0
                            //    };
                            //}
                            //else
                            //{
                            //    vm.Costingmodel.Data = new CostingModelData
                            //    {
                            //        Affectedl12cases = 0,
                            //        Affectedl12price = 0,
                            //        Affectedl12varopex = 0,
                            //        Affectedn12cases = 0,
                            //        Affectedn12price = 0,
                            //        Affectedn12varopex = 0,
                            //        Unaffectedl12cases = 0,
                            //        Unaffectedl12price = 0,
                            //        Unaffectedl12varopex = 0,
                            //        Unaffectedn12cases = 0,
                            //        Unaffectedn12price = 0,
                            //        Unaffectedn12varopex = 0
                            //    };
                            //}
                            //}
                            else if (step == 3)
                            {
                                if (rv.Number == 0 && cmpayload != null)
                                {
                                    vm.Costingmodel.ModelID = cmpayload.ID.ToString();
                                    vm.Costingmodel.Modelname = cmpayload.name;
                                    vm.Costingmodel.Brand = cmpayload.brand;
                                    vm.Costingmodel.Vendor = cmpayload.vendor;
                                    vm.Costingmodel.Zone = cmpayload.priceZone.ToString();
                                    vm.Costingmodel.Type = cmpayload.type.ToString();
                                    vm.Costingmodel.Rejectcomment = cmpayload.rejectedComment;
                                    vm.Costingmodel.Data = new CostingModelData();
                                    vm.Costingmodel.Data.Comment = cmpayload.comment;
                                    vm.Costingmodel.LastUpdated = cmpayload.LastModifiedDate;
                                    vm.Costingmodel.Bdm = cmpayload.CreatedUser;
                                }
                                
                            }
                        }
                        catch (InvalidOperationException e)
                        {
                        }
                        
                    }
                    //CostingModel id = null
                    else
                    {
                        vm.Costingmodel = new CostingModel
                        {
                            Data = new CostingModelData(),
                        };
                        vm.Branditems = new List<Branditem>();
                        vm.Costingmodel.Data.Itemgroups = new List<Itemgroup>();
                        vm.Costingmodel.Type = "-1";
                        vm.Step = step.GetValueOrDefault(1);
                        vm.errorvm.errorcode = null;
                        
                    }
                    return View(vm);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
        public ActionResult CostingModelCreate(string brand, string vendor, int zone, int type, string name)
        {
            ReturnValue rv = new ReturnValue();
            CostingModelPayload payload = new CostingModelPayload
            {
                brand = brand,
                vendor = vendor,
                name = name
            };
            switch (zone)
            {
                case 1:
                    payload.priceZone = Pricing.PriceZone.Zone1;
                    break;
                case 2:
                    payload.priceZone = Pricing.PriceZone.Zone3;
                    break;
                default:
                    payload.priceZone = Pricing.PriceZone.ZoneAll;
                    break;
            }
            switch (type)
            {
                case 1:
                    payload.type = ERP.CostingModel.Type.Baseline;
                    break;
                case 2:
                    payload.type = ERP.CostingModel.Type.Planning;
                    break;
                default:
                    payload.type = ERP.CostingModel.Type.PriceChange;
                    break;
            }
            rv = new ERP.CostingModel().Create(Request.Cookies["SecToken"]["SecurityKey"], ref payload);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int cmid = payload.ID;
                return Json(new { url = Url.Action("CostingModel", "Home", new { id = cmid.ToString(), step = 2 }) });
            }
            
        }
        public ActionResult CostingModelUpdateName(string cmid, string brand, string vendor, int zone, int type, string name)
        {
            ReturnValue rv = new ReturnValue();
            CostingModelPayload payload = new CostingModelPayload
            {
                ID = int.Parse(cmid),
                brand = brand,
                vendor = vendor,
                name = name
            };
            switch (zone)
            {
                case 1:
                    payload.type = ERP.CostingModel.Type.Baseline;
                    break;
                case 2:
                    payload.type = ERP.CostingModel.Type.Planning;
                    break;
                case 3:
                    payload.type = ERP.CostingModel.Type.PriceChange;
                    break;
            }
            switch (type)
            {
                case 1:
                    payload.type = ERP.CostingModel.Type.Baseline;
                    break;
                case 2:
                    payload.type = ERP.CostingModel.Type.Planning;
                    break;
                case 3:
                    payload.type = ERP.CostingModel.Type.PriceChange;
                    break;
            }
            rv = new ERP.CostingModel().Update(Request.Cookies["SecToken"]["SecurityKey"], payload);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { url = Url.Action("CostingModel", "Home", new { id = cmid.ToString(), step = 2 }) });
            }
        }
        public ActionResult CostingModelUpdateComment(string cmid, string brand, string vendor, string zone, int type, string name, string comment)
        {
            ReturnValue rv = new ReturnValue();
            CostingModelPayload payload = new CostingModelPayload
            {
                ID = int.Parse(cmid),
                brand = brand,
                vendor = vendor,
                name = name,
                comment = comment
            };
            if (zone == "Zone1")
            {
                payload.priceZone = Pricing.PriceZone.Zone1;
            }
            else if (zone == "Zone3")
            {
                payload.priceZone = Pricing.PriceZone.Zone3;
            }
            else
            {
                payload.priceZone = Pricing.PriceZone.ZoneAll;
            }
            switch (type)
            {
                case 1:
                    payload.type = ERP.CostingModel.Type.Planning;
                    break;
                case 2:
                    payload.type = ERP.CostingModel.Type.PriceChange;
                    break;
                default:
                    payload.type = ERP.CostingModel.Type.Baseline;
                    break;
            }
            rv = new ERP.CostingModel().Update(Request.Cookies["SecToken"]["SecurityKey"], payload);
            if (rv.Number != 0)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { url = Url.Action("PCMMain", "Home") });
            }
        }
        

        private IEnumerable<Branditem> GetUsedItems(CMViewModel vm)
        {
            var restgroup = vm.Costingmodel.Data.Itemgroups;
            List<Branditem> restgroupitems = new List<Branditem>();
            DataSet DBSetUseditems = null;
            ReturnValue rv = new ReturnValue();

            foreach (var grp in restgroup)
            {
                rv = new CostingModelGroupItem().List(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(grp.id), ref DBSetUseditems);
                if (DBSetUseditems != null && rv.Number == 0)
                {
                    foreach (DataTable table in DBSetUseditems.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            Branditem itm = new Branditem();
                            itm.itemID = row["ITEM"].ToString();
                            restgroupitems.Add(itm);
                        }
                    }
                }
            }
            return restgroupitems;
        }

        private Itemgroup GetCurrGroup(string gid, CMViewModel vm)
        {
            var group = vm.Costingmodel.Data.Itemgroups.Where(m => m.id == gid).First();

            //Retrieve items list for currgroup
            List<Branditem> curritemlist = new List<Branditem>();
            CostingModelGroupPayload grouppayload = null;
            CostingModelGroupPricePayload wholesalepl = null;
            CostingModelGroupPricePayload dsdpl = null;
            DataSet DBSetCurrgroup = null;
            ReturnValue rv = new ReturnValue();
            try
            {
                rv = new CostingModelGroupItem().List(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(group.id), ref DBSetCurrgroup);
                TempData["errorcode"] = rv.Number;
                //Current Group
                if (DBSetCurrgroup != null && rv.Number == 0)
                {
                    foreach (DataTable table in DBSetCurrgroup.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            Branditem itm = new Branditem();
                            itm.itemID = row["ITEM"].ToString();
                            curritemlist.Add(itm);
                        }
                    }
                    rv = new CostingModelGroup().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(gid), ref grouppayload);
                    if (grouppayload != null)
                    {
                        rv = new CostingModelGroupPrice().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(group.id), CostingModelGroupPrice.CostingModelGroupPriceType.Wholesale, ref wholesalepl);
                        rv = new CostingModelGroupPrice().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(group.id), CostingModelGroupPrice.CostingModelGroupPriceType.DSD, ref dsdpl);
                    }
                    TempData["errorcode"] = rv.Number;
                }

                
                vm.Costingmodel = vm.Costingmodel;
                vm.currItemgroup = new Itemgroup(group.id, group.groupname)
                {
                    items = curritemlist,
                    l12monthsalesrevenue = grouppayload.actualSales,
                    l12monthcasevolume = grouppayload.actualVolume,
                    n12monthcasevolume = grouppayload.estimatedVolume,
                    warehousecat = grouppayload.warehouseCategory,
                    shortshelflife = (grouppayload.shortShelfLife == 1),
                    billback = grouppayload.billbackPercent,
                    branddevelopmentfund = grouppayload.brandDevelopmentPercent,
                    Cashterms = grouppayload.cashTermsPercent,
                    currlandedcost = grouppayload.currentLandedPricePerCase,
                    currentpurchasepricepercase = grouppayload.currentPurchasePricePerCase,
                    dealaccruals = grouppayload.dealAccrualPercent,

                    Exchangerate = grouppayload.exchangeRate,
                    freightcosting = grouppayload.freightPricePerCase,
                    otherperc = grouppayload.otherPercent,
                    proposedpurchasepricepercase = grouppayload.proposedPurchasePricePerCase,
                    spoilagecredits = grouppayload.spoilageCreditsPercent,
                    duty = grouppayload.tariffPercent,
                    currwholesaleppc = wholesalepl.currentPricePerCase,
                    currwholesalesuggestedppc = wholesalepl.currentSuggestedRetailPricePerUnit,
                    currwholesaletrademargin = wholesalepl.currentMarginPercent,
                    propwholesaleppc = wholesalepl.proposedPricePerCase,
                    propwholesalesuggestedppc = wholesalepl.proposedSuggestedRetailPricePerUnit,
                    propwholesaletrademargin = wholesalepl.proposedMarginPercent,
                    currdsdppc = dsdpl.currentPricePerCase,
                    currdsdsuggestedppc = dsdpl.currentSuggestedRetailPricePerUnit,
                    currdsdtrademargin = dsdpl.currentMarginPercent,
                    propdsdppc = dsdpl.proposedPricePerCase,
                    propdsdsuggestedppc = dsdpl.proposedSuggestedRetailPricePerUnit,
                    propdsdtrademargin = dsdpl.proposedMarginPercent
                };
            }
            catch (InvalidOperationException e)
            {
            }
            finally
            {
                grouppayload = null;
                wholesalepl = null;
                dsdpl = null;
            }
            
            return vm.currItemgroup;
        }


        private List<Itemgroup> GetGroups(string v)
        {
            List<Itemgroup> itemgroups = new List<Itemgroup>();
            DataSet DBSetItemgroup = null;
            ReturnValue rv = new ReturnValue();
            try
            {
                rv = new ERP.CostingModelGroup().List(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(v), ref DBSetItemgroup);
                TempData["errorcode"] = rv.Number;
                if (DBSetItemgroup != null && rv.Number == 0)
                {
                    foreach (DataTable table in DBSetItemgroup.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            Itemgroup group = new Itemgroup();
                            group.id = row["ROW_ID"].ToString();
                            group.groupname = row["Name"].ToString();
                            itemgroups.Add(group);
                        }
                    }
                }
            }
            catch (InvalidOperationException e)
            {
            }
            return itemgroups;
        }

        [HttpPost]
        public ActionResult CostingModel(CMViewModel vm, string id)
        {
            var newid = 0;
                if (id == null)
                {
                    newid = CreateCostingModel(vm);
                }
                else
                {
                    UpdateCostingModel(vm, id);
                }
            //Get id return value
            CMViewModel newvm = new CMViewModel();
            newvm.errorvm = new ErrorViewModel();
            newvm.errorvm.errorcode = TempData["errorcode"].ToString();
            if (newvm.errorvm.errorcode != "0" && newvm.errorvm.errorcode != null)
            {
                return PartialView("_ErrorCodeMessage", newvm.errorvm);
            }
            else
            {
                if (id == null)
                {
                    return Json(new { url = Url.Action("CostingModel", "Home", new { id = newid.ToString(), step = 2 }) });
                }
                else
                {
                    return Json(new { url = Url.Action("CostingModel", "Home", new { id, step = 2 }) });
                }
                
            }
            
        }
        [HttpGet]
        public ActionResult CostingAnalysis(string id, string p, string gid)
        {
            if (p == "d")
            {
                DeleteCostingModelGroup(gid);
            }
            return RedirectToAction("CostingModel",new { id = id, step = 2});
            //return Json(new { url = Url.Action("CostingModel","Home", new { id = id, step = 2 }) }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult CostingAnalysis(CMViewModel vm)
        {
            var grouppost = vm.currItemgroup;

            CostingModelGroupPayload grouppayload = new CostingModelGroupPayload();
            List<ItemPayload> items = new List<ItemPayload>();
            List<string> itemstring = grouppost.itemlist.Split(',').ToList();
            for(int i = 0; i < itemstring.Count; i ++)
            {
                ItemPayload itm = new ItemPayload();
                itm.ItemNumber = itemstring[i];
                items.Add(itm);
            }
            grouppayload.Items = items;
            grouppayload.ID = int.Parse(vm.currItemgroup.id);
            grouppayload.name = grouppost.groupname;
            grouppayload.actualSales = grouppost.l12monthsalesrevenue;//
            grouppayload.actualVolume = grouppost.l12monthcasevolume;//
            grouppayload.billbackPercent = grouppost.billback;
            grouppayload.brandDevelopmentPercent = grouppost.branddevelopmentfund;
            grouppayload.cashTermsPercent = grouppost.Cashterms;
            grouppayload.costingModelID = Int32.Parse(vm.Costingmodel.ModelID);
            grouppayload.currentLandedPricePerCase = grouppost.currlandedcost;//
            grouppayload.currentPurchasePricePerCase = grouppost.currentpurchasepricepercase;//
            grouppayload.proposedPurchasePricePerCase = grouppost.proposedpurchasepricepercase;
            grouppayload.spoilageCreditsPercent = grouppost.spoilagecredits;
            grouppayload.tariffPercent = grouppost.Tariff;
            grouppayload.dealAccrualPercent = grouppost.dealaccruals;
            grouppayload.estimatedVolume = grouppost.n12monthcasevolume;
            grouppayload.exchangeRate = grouppost.Exchangerate;
            grouppayload.freightPricePerCase = grouppost.freightcosting;
            grouppayload.otherPercent = grouppost.otherperc;
            grouppayload.warehouseCategory = grouppost.warehousecat;
            if (grouppost.shortshelflife)
            {
                grouppayload.shortShelfLife = 1;
            }
            else
            {
                grouppayload.shortShelfLife = 0;
            }
            //Wholesale Price Group
            CostingModelGroupPricePayload pricepayloadW = new CostingModelGroupPricePayload
            {
                currentPricePerCase = grouppost.currwholesaleppc,//
                proposedPricePerCase = grouppost.propwholesaleppc,
                currentMarginPercent = grouppost.currwholesaletrademargin,
                proposedMarginPercent = grouppost.propwholesaletrademargin,
                currentSuggestedRetailPricePerUnit = grouppost.currwholesalesuggestedppc,//
                proposedSuggestedRetailPricePerUnit = grouppost.propwholesalesuggestedppc
            };
            //DSD Price Group
            CostingModelGroupPricePayload pricepayloadDSD = new CostingModelGroupPricePayload
            {
                currentPricePerCase = grouppost.currdsdppc,//
                proposedPricePerCase = grouppost.propdsdppc,
                currentMarginPercent = grouppost.currdsdtrademargin,
                proposedMarginPercent = grouppost.propdsdtrademargin,
                currentSuggestedRetailPricePerUnit = grouppost.currdsdsuggestedppc,//
                proposedSuggestedRetailPricePerUnit = grouppost.propdsdsuggestedppc//
            };

            grouppayload.PriceTypeDSD = pricepayloadDSD;
            grouppayload.PriceTypeWholesale = pricepayloadW;
            if (grouppayload.ID == 0 )
            {
                CreateCostingModelGroup(grouppayload);
            }
            else
            {
                UpdateCostingModelGroup(grouppayload);
            }
            //Retrive Costing Model and call update with new values from financial P&L
            //Add n12 values from grouppost (vm.currItemgroup.n12monthcasevolume, etc)
            //CostingModelPayload cmpayload = new CostingModelPayload();

            TempData["Affectedl12cases"] = vm.Costingmodel.Data.Affectedl12cases;
            TempData["Affectedl12price"] = vm.Costingmodel.Data.Affectedl12price;
            TempData["Unaffectedl12cases"] = vm.Costingmodel.Data.Unaffectedl12cases;
            TempData["Unaffectedl12price"] = vm.Costingmodel.Data.Unaffectedl12price;

            TempData["Affectedn12cases"] = vm.Costingmodel.Data.Affectedn12cases;
            TempData["Affectedn12price"] = vm.Costingmodel.Data.Affectedn12price;
            TempData["Unaffectedn12cases"] = vm.Costingmodel.Data.Unaffectedn12cases;
            TempData["Unaffectedn12price"] = vm.Costingmodel.Data.Unaffectedn12price;
            //ReturnValue rv = new ReturnValue();
            //try
            //{
            //    rv = new ERP.CostingModel().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(grouppost.ModelID), ref cmpayload);
            //    TempData["errorcode"] = rv.Number;
            //    if (rv.Number == 0 && cmpayload != null)
            //    {
            //        var x = grouppost.Data.Affectedl12cases;
            //        var x = grouppost.Data.Affectedl12price;
            //        var x = grouppost.Data.Unaffectedl12cases
            //        var x = grouppost.Data.Unaffectedl12price;
            //    }
            //}
            //catch { }
            CMViewModel nvm = vm;
            var cmid = nvm.Costingmodel.ModelID;
            return Json(new { url = Url.Action("CostingModel", "Home", new { id = cmid, step = 2 }) });
        }
        [HttpPost]
        public ActionResult SubmitCostingModel(string cmid, string brand, string vendor, string zone, int type, string name, string comment, string cmd)
        {

            CostingModelPayload payload = new CostingModelPayload
            {
                ID = int.Parse(cmid),
                brand = brand,
                vendor = vendor,
                name = name,
                comment = comment,
                rejectedComment = ""
            };
            if (zone == "Zone1")
            {
                payload.priceZone = Pricing.PriceZone.Zone1;
            }
            else if (zone == "Zone3")
            {
                payload.priceZone = Pricing.PriceZone.Zone3;
            }
            else
            {
                payload.priceZone = Pricing.PriceZone.ZoneAll;
            }
            switch (type)
            {
                case 1:
                    payload.type = ERP.CostingModel.Type.Planning;
                    break;
                case 2:
                    payload.type = ERP.CostingModel.Type.PriceChange;
                    break;
                default:
                    payload.type = ERP.CostingModel.Type.Baseline;
                    break;
            }
            ReturnValue rv = new ReturnValue();
            if (cmd == "update")
            {
                
                try
                {
                    rv = new ERP.CostingModel().Update(Request.Cookies["SecToken"]["SecurityKey"], payload);
                    TempData["errorcode"] = rv.Number;
                    if (rv.Number != 0)
                    {
                        return Json(new { url = Url.Action("CostingModel", "Home", new { id = payload.ID, step = 3 }) });
                    }
                }
                catch (InvalidOperationException e)
                {
                }
            }
            else if (cmd == "submit")
            {
                try
                {
                    rv = new ERP.CostingModel().Update(Request.Cookies["SecToken"]["SecurityKey"], payload);
                    TempData["errorcode"] = rv.Number;
                    if (rv.Number != 0)
                    {
                        return Json(new { url = Url.Action("CostingModel", "Home", new { id = payload.ID, step = 3 }) });
                    }
                    else
                    {
                        try
                        {
                            rv = new ERP.CostingModel().Submit(Request.Cookies["SecToken"]["SecurityKey"], payload.ID);
                            TempData["errorcode"] = rv.Number;
                            if (rv.Number != 0)
                            {
                                
                                return Json(new { url = Url.Action("CostingModel", "Home", new { id = payload.ID, step = 3 }) });
                            }
                            else
                            {
                                BuildEmail(1, payload.ID.ToString());
                                return Json(new { url = Url.Action("PCMMain", "Home") });
                            }
                        }
                        catch (InvalidOperationException e)
                        {
                        }
                    }
                }
                catch (InvalidOperationException e)
                {
                }
            }

            return Json(new { url = Url.Action("PCMMain", "Home") });
        }
        
        [HttpPost]
        public ActionResult ApproveCostingModel(string modelid)
        {
            try
            {
                int.TryParse(modelid, out int cmid);
                ReturnValue rv = new ERP.CostingModel().Approve(Request.Cookies["SecToken"]["SecurityKey"], cmid);
                if (rv.Number != 0)
                {
                    TempData["errorcode"] = rv.Number;
                    return Json(new { url = Url.Action("CostingModelReview", "Home", new { id = cmid}) });
                }
                else
                {
                    BuildEmail(2, modelid);
                    return Json(new { url = Url.Action("PCMMain", "Home") });
                }
            }
            catch (InvalidOperationException e)
            {
            }
            return RedirectToAction("PCMMain", "Home");
        }
        [HttpPost]
        public ActionResult RejectCostingModel(string modelid,string rejectcomment)
        {
            try
            {
                int.TryParse(modelid, out int cmid);
                ReturnValue rv = new ERP.CostingModel().Reject(Request.Cookies["SecToken"]["SecurityKey"], cmid, rejectcomment);
                if (rv.Number != 0)
                {
                    TempData["errorcode"] = rv.Number;
                    TempData["errormsg"] = rv.Message;
                    TempData["errorsrc"] = rv.Source;
                    return Json(new { status = "error", errorcode = rv.Number.ToString(), errormsg = rv.Message.ToString(), errorsrc = "Reject" });
                }
                else
                {
                    BuildEmail(3, modelid);
                    return Json(new { url = Url.Action("PCMMain", "Home") });
                }
            }
            catch (InvalidOperationException e)
            {
            }
            return RedirectToAction("PCMMain", "Home");
        }
        private void UpdateCostingModelGroup(CostingModelGroupPayload grouppayload)
        {
            ReturnValue rv = new ReturnValue();
            try
            {
                rv = new ERP.CostingModelGroup().Update(Request.Cookies["SecToken"]["SecurityKey"], grouppayload);
                TempData["errorcode"] = rv.Number;
            }
            catch (InvalidOperationException e)
            {
            }
        }

        private void CreateCostingModelGroup(CostingModelGroupPayload group)
        {
            
            ReturnValue rv = new ReturnValue();
            var grouppayload = group;
            if (grouppayload.warehouseCategory == null)
            {
                grouppayload.warehouseCategory = " ";
            }
            //var test = JsonConvert.SerializeObject(grouppayload);
            try
            {
                rv = new ERP.CostingModelGroup().Create(Request.Cookies["SecToken"]["SecurityKey"], ref grouppayload);
                TempData["errorcode"] = rv.Number;
            }
            catch (InvalidOperationException e)
            {
            }
        }
        private void DeleteCostingModelGroup(string gid)
        {
            ReturnValue rv = new ReturnValue();
            try
            {
                rv = new CostingModelGroup().Remove(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(gid));
                TempData["errorcode"] = rv.Number;
            }
            catch (InvalidOperationException e) { }
        }

        //Step 1 Costing Model Update
        private void UpdateCostingModel(CMViewModel vm, string id)
        {
            var cmpost = vm.Costingmodel;
            CostingModelPayload cmpayload = new CostingModelPayload
            {
                ID = Int32.Parse(id),
                name = cmpost.Modelname
            };
            ReturnValue rv = new ReturnValue();
            try
            {
                rv = new ERP.CostingModel().Update(Request.Cookies["SecToken"]["SecurityKey"], cmpayload);
                TempData["errorcode"] = rv.Number;
            }
            catch (InvalidOperationException e)
            {
            }
            cmpayload = null;
        }

        private int CreateCostingModel(CMViewModel vm)
        {
            var cmpost = vm.Costingmodel;
            CostingModelPayload cmpayload = new CostingModelPayload
            {
                brand = cmpost.Brand,
                vendor = cmpost.Vendor,
                priceZone = Pricing.PriceZone.Zone1,
                name = cmpost.Modelname
            };
            ReturnValue rv = new ReturnValue();
            try
            {
                rv = new ERP.CostingModel().Create(Request.Cookies["SecToken"]["SecurityKey"], ref cmpayload);
                TempData["errorcode"] = rv.Number;
            }
            catch(InvalidOperationException e)
            {
            }
            return cmpayload.ID;
        }
        private void DeleteCostingModel(string id)
        {
            ReturnValue rv = new ReturnValue();
            try
            {
                rv = new ERP.CostingModel().Remove(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(id));
                TempData["errorcode"] = rv.Number;
            }
            catch(InvalidOperationException e) { }
        }
        [HttpGet]
        private ActionResult GetItemMAC(string brand)
        {
            DataSet DBSetBranditems = null;
            ReturnValue rv = new ReturnValue();
            try
            {
                rv = new ERP.Item().ListbyBrand(Request.Cookies["SecToken"]["SecurityKey"], brand, ref DBSetBranditems);
                if (rv.Number != 0)
                {
                    TempData["errorcode"] = rv.Number;
                }
            }
            catch (InvalidOperationException e){ }

            return null;
        }
        
        private IEnumerable<Branditem> GetBrandItems(string id, string brand, string vendor, string zone)
        {
            var existingbranditems = false;
            List<Branditem> cmGroupItemList = new List<Branditem>();
            if (HttpContext.Cache["model"] != null)
            {
                var modelparam = (List<String>)HttpContext.Cache["model"];
                if (HttpContext.Cache["branditemlist"] != null && modelparam[0] == brand && modelparam[1] == vendor && modelparam[2] == zone)
                {
                    cmGroupItemList = (List<Branditem>)HttpContext.Cache["branditemlist"];
                    existingbranditems = true;
                }
            }
            
            if (!existingbranditems)
            {
                //use brand string to query list of items

                List<ItemPayload> itemidlist = new List<ItemPayload>();
                DataSet DBSetBranditems = null;
                ReturnValue rv = new ReturnValue();
                TempData["errorcode"] = "0";
                try
                {
                    rv = new ERP.Item().ListbyBrand(Request.Cookies["SecToken"]["SecurityKey"], brand, ref DBSetBranditems);
                    if (rv.Number != 0)
                    {
                        TempData["errorcode"] = rv.Number;
                    }

                    if (DBSetBranditems != null)
                    {
                        foreach (DataTable table in DBSetBranditems.Tables)
                        {
                            foreach (DataRow row in table.Rows)
                            {
                                Branditem item = new Branditem
                                {
                                    //refer to rowid in akitm minus 1
                                    itemID = row["ITEM"].ToString(),
                                    itemdescription = row["ITEM_DESCRIPTION"].ToString(),
                                    unitsize = row["ITEM_SIZE"].ToString(),
                                    unitspercase = Int32.Parse(row["UNIT_PER_CASE"].ToString()),
                                    movingaveragecost = Decimal.Parse(row["MAC"].ToString()),
                                    warehousecat = row["WAREHOUSE_CATEGORY"].ToString(),
                                    shelflife = Int32.Parse(row["SHELF_LIFE"].ToString()),
                                    purchaseprice = 0.00m,
                                    wholesaleprice = 0.00m,
                                    dutypct = Decimal.Parse(row["DUTY_PCT"].ToString())
                                };
                                ItemPayload i = new ItemPayload
                                {
                                    ItemNumber = row["ITEM"].ToString()
                                };
                                cmGroupItemList.Add(item);
                                itemidlist.Add(i);
                            }
                        }
                    }
                    //Retrive Avg purchase price for each item
                    rv = new ERP.Item().RetrieveAveragePurchasePricePerCase(Request.Cookies["SecToken"]["SecurityKey"], ref itemidlist, vendor);
                    if ((string)TempData["errorcode"] == "0" && rv.Number != 0)
                    {
                        TempData["errorcode"] = rv.Number;
                    }
                    //Retrive Wholesale, DSD and Retail price based on Viewmodel Zone value
                    rv = new ERP.Item().RetrievePrice(Request.Cookies["SecToken"]["SecurityKey"], ref itemidlist);
                    if ((string)TempData["errorcode"] == "0" && rv.Number != 0)
                    {
                        TempData["errorcode"] = rv.Number;
                    }

                    rv = new ERP.Item().RetrieveSalesHistory(Request.Cookies["SecToken"]["SecurityKey"], ref itemidlist);
                    if ((string)TempData["errorcode"] == "0" && rv.Number != 0)
                    {
                        TempData["errorcode"] = rv.Number;
                    }
                    for (int i = 0; i < cmGroupItemList.Count; i++)
                    {
                        cmGroupItemList[i].purchaseprice = Math.Round(itemidlist[i].AveragePurchasePricePerCase, 2);
                        //Add wholesale and DSD price based on zone value
                        if (zone == "3")
                        {
                            cmGroupItemList[i].wholesaleprice = Math.Round(itemidlist[i].Zone3WholesalePrice, 2);
                            cmGroupItemList[i].dsdprice = Math.Round(itemidlist[i].Zone3DSDPrice, 2);
                            cmGroupItemList[i].retailprice = Math.Round(itemidlist[i].Zone3RetailPrice, 2);

                        }
                        else
                        {
                            cmGroupItemList[i].wholesaleprice = Math.Round(itemidlist[i].Zone1WholesalePrice, 2);
                            cmGroupItemList[i].dsdprice = Math.Round(itemidlist[i].Zone1DSDPrice, 2);
                            cmGroupItemList[i].retailprice = Math.Round(itemidlist[i].Zone1RetailPrice, 2);
                        }

                        
                        cmGroupItemList[i].totalsales = Math.Round(itemidlist[i].SalesHistoryGrossDollars, 2);
                        cmGroupItemList[i].unitssold = itemidlist[i].SalesHistoryCases;

                        //Warehouse Category Expanded
                        if (cmGroupItemList[i].warehousecat == "F")
                        {
                            cmGroupItemList[i].warehousecat = "Frozen";
                        }
                        else if (cmGroupItemList[i].warehousecat == "P")
                        {
                            cmGroupItemList[i].warehousecat = "Perishable";
                        }
                        else
                        {
                            cmGroupItemList[i].warehousecat = "Dry";
                        }
                    }
                    //cmGroupItemList.Add(new Branditem("10001", "MIDEL MAPLE GINGER CREMES GF", "255 GR", 12, 2.64m, 53.00m, DateTime.MinValue));
                }
                catch (InvalidOperationException e) { }
                finally
                {
                    List<String> modelparam = new List<String>(new string[] { brand, vendor, zone });
                    HttpContext.Cache["model"] = modelparam;
                    HttpContext.Cache["branditemlist"] = cmGroupItemList;
                }

            }

            return cmGroupItemList;
        }

        private IndexViewModel RetrieveUserData(string uname)
        {
            //Call ERP method to retrive data based on username

            IndexViewModel udata = new IndexViewModel
            {
                uname = uname,
                role = "lead"
            };
            DataSet DBSetUserdata = null;
            ReturnValue rv = new ReturnValue();
            rv = new ERP.CostingModel().List(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetUserdata);
            TempData["errorcode"] = rv.Number;
            List<CostingModel> cmlist = new List<CostingModel>();
            if (DBSetUserdata != null)
            {
                foreach (DataTable table in DBSetUserdata.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        CostingModel cm = new CostingModel()
                        {
                            ModelID = row["ROW_ID"].ToString(),
                            Modelname = row["NAME"].ToString(),
                            Brand = row["BRAND"].ToString(),
                            Bdm = row["BRAND_MANAGER"].ToString(),
                            Type = row["TYPE_ID"].ToString(),
                            Datecreated = DateTime.Parse(row["CREATE_TS"].ToString()),
                            Status = (int)row["STATUS_ID"],
                            Data = new CostingModelData()
                        };
                        cmlist.Add(cm);
                    }
                }
            }
            udata.allcostingmodel = cmlist as IEnumerable<CostingModel>;
            return udata;
        }

        [HttpGet]
        public ActionResult GetVendorsFromBrand(string brandid)
        {
            List<SelectListItem> vendorlist = new List<SelectListItem>();
            Vendor Vendor = new Vendor();
            DataSet DBSetVendors = null;
            ReturnValue rv = new ReturnValue();
            rv = Vendor.List(Request.Cookies["SecToken"]["SecurityKey"],brandid, ref DBSetVendors);

            if (DBSetVendors != null)
            {
                foreach (DataTable table in DBSetVendors.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var vendor = new SelectListItem
                        {
                            Value = row["VENDOR"].ToString(),
                            Text = row["VENDOR_NAME"].ToString() + "(" + row["VENDOR"].ToString() + ")"
                        };
                        vendorlist.Add(vendor);
                    }
                }
            }
            else
            {
                TempData["errorcode"] = 911;
            }
            return Json(vendorlist, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult GetCurrencyExchange(string vendorid)
        {
            VendorPayload vp = new VendorPayload();
            TempData["errorcode"] = 0;
            decimal exchangerate = 0;
            List<String> result = new List<String>();
            ReturnValue rv = new ReturnValue();
            rv = new Vendor().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], vendorid, ref vp);
            TempData["errorcode"] = rv.Number;
            if (rv.Number == 0)
            {
                result.Add(vp.currencyString);
                rv = new Currency().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], vp.currencyCode, ref exchangerate);
                if (TempData["errorcode"].ToString() == "0" && rv.Number != 0)
                {
                    TempData["errorcode"] = rv.Number;
                }
                else
                {
                    result.Add(exchangerate.ToString());
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        private IEnumerable<SelectListItem> PriceZone()
        {
            List<SelectListItem> zoneslist = new List<SelectListItem>();
            List<Pricing.PriceZone> zones = new List<Pricing.PriceZone>();
            ReturnValue rv = null;
            //Brands.List(HttpContext.Session["SecurityKey"].ToString(), ref readerPRN);
            rv = new Pricing().ListPriceZone(Request.Cookies["SecToken"]["SecurityKey"], ref zones);
            short index = 1;
            string zonetext = "";
            foreach(var i in zones)
            {
                switch (i.ToString())
                {
                    case "Zone1":
                        zonetext = "Zone 1 Only";
                        break;
                    case "Zone3":
                        zonetext = "Zone 3 Only";
                        break;
                    case "ZoneAll":
                        zonetext = "National";
                        break;
                }
                zoneslist.Add(new SelectListItem { Value = index.ToString(), Text = zonetext });
                index += 2;
            }
            return zoneslist;
        }
        private IEnumerable<SelectListItem> CMType()
        {
            List<SelectListItem> cmtypelist = new List<SelectListItem>();
            DataSet DBSetCMType = null;
            ReturnValue rv = null;
            rv = new CostingModelType().List(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetCMType);
            if (DBSetCMType != null)
            {
                foreach (DataTable table in DBSetCMType.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var cmtype = new SelectListItem
                        {
                            Value = row["ROWID"].ToString(),
                            Text = row["TEXT"].ToString()

                        };
                        cmtypelist.Add(cmtype);
                    }
                }
            }
            return cmtypelist;
        }
        private IEnumerable<SelectListItem> GetBrands()
        {
            List<SelectListItem> brandlist = new List<SelectListItem>();
            Brand Brands = new Brand();
            DataSet DBSetBrands = null;
            //Brands.List(HttpContext.Session["SecurityKey"].ToString(), ref DBSetBrands);
            Brands.ListbyBrandManager(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetBrands);

            if (DBSetBrands != null)
            {
                foreach (DataTable table in DBSetBrands.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var brand = new SelectListItem
                        {
                            Value = row["PRNPRN"].ToString(),
                            Text = row["PRNNAM"].ToString()
                        };
                        brandlist.Add(brand);
                    }
                }

            }
            else
            {
                TempData["errorcode"] = 911;
            }
            Brands = null;
            return brandlist;
        }
        public string GetBrandPillar(string brand)
        {
            string Brandpillar = "";
            Brand Brands = new Brand();
            DataSet DBSetBrands = null;
            Brands.ListbyBrandManager(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetBrands);

            if (DBSetBrands != null)
            {
                foreach (DataTable table in DBSetBrands.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row["PRNPRN"].ToString() == brand)
                        {
                            Brandpillar = row["PRNPMG"].ToString();
                            break;
                        }
                    }
                }

            }
            else
            {
                TempData["errorcode"] = 911;
            }
            return Brandpillar;
        }
        private IEnumerable<SelectListItem> GetAllVendors()
        {
            List<SelectListItem> vendorlist = new List<SelectListItem>();
            Vendor Vendors = new Vendor();
            DataSet DBSetVendors = null;
            Vendors.List(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetVendors);
            

            if (DBSetVendors != null)
            {
                foreach (DataTable table in DBSetVendors.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var vendor = new SelectListItem
                        {
                            Value = row["VENDOR"].ToString(),
                            Text = row["VENDOR_NAME"].ToString()
                        };
                        vendorlist.Add(vendor);
                    }
                }

            }
            else
            {
                TempData["errorcode"] = 911;
            }
            Vendors = null;
            return vendorlist;
        }
        [HttpGet]
        public ActionResult Step3Data(string[] arr) {
            TempData["step3data"] = arr;

            return Json(new { url = Url.Action("CostingModel", "Home", new { id = 100, step = 3 }) });
        }
        [HttpPost]
        public ActionResult Step3Data(string[] arr, string modelid)
        {

            TempData["step3data"] = arr;

            return Json(new { url = Url.Action("CostingModel", "Home", new { id = modelid, step = 3 }) });
        }


        //New AJAX Methods
        [HttpGet]
        public ActionResult ListBrandsbyBDM()
        {
            List<SelectListItem> brandlist = new List<SelectListItem>();
            Brand Brands = new Brand();
            DataSet DBSetBrands = null;
            Brands.ListbyBrandManager(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetBrands);

            if (DBSetBrands != null)
            {
                foreach (DataTable table in DBSetBrands.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var brand = new SelectListItem
                        {
                            Value = row["PRNPRN"].ToString(),
                            Text = row["PRNNAM"].ToString()
                        };
                        brandlist.Add(brand);
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(brandlist);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListPriceZones()
        {
            List<SelectListItem> zoneslist = new List<SelectListItem>();
            List<Pricing.PriceZone> zones = new List<Pricing.PriceZone>();
            ReturnValue rv = null;
            //Brands.List(HttpContext.Session["SecurityKey"].ToString(), ref readerPRN);
            rv = new Pricing().ListPriceZone(Request.Cookies["SecToken"]["SecurityKey"], ref zones);
            short index = 0;
            string txt = "";
            foreach (var i in zones)
            {
                switch (i.ToString())
                {
                    case "Zone1":
                        txt = "Zone 1 Only";
                        break;
                    case "Zone3":
                        txt = "Zone 3 Only";
                        break;
                    case "ZoneAll":
                        txt = "National";
                        break;
                }
                zoneslist.Add(new SelectListItem { Value = index.ToString(), Text = txt });
                index ++;
            }
            string returnstring = JsonConvert.SerializeObject(zoneslist);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListCMTypes()
        {
            List<SelectListItem> cmtypelist = new List<SelectListItem>();
            DataSet DBSetCMType = null;
            ReturnValue rv = null;
            rv = new CostingModelType().List(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetCMType);
            if (DBSetCMType != null)
            {
                foreach (DataTable table in DBSetCMType.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var cmtype = new SelectListItem
                        {
                            Value = row["ROWID"].ToString(),
                            Text = row["TEXT"].ToString()

                        };
                        cmtypelist.Add(cmtype);
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(cmtypelist);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPriceTypes()
        {
            DataSet DBSetPriceCategories = null;
            List<PriceType> PriceTypes = new List<PriceType>();
            ReturnValue rv = new PriceCategory().List(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetPriceCategories);
            if (rv.Number == 0)
            {
                foreach (DataTable table in DBSetPriceCategories.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row["ZPTZPT"].ToString().TrimEnd(' ') != "0" && row["ZPTZPT"].ToString().TrimEnd(' ') != "1" && row["ZPTZPT"].ToString().TrimEnd(' ') != "2")
                        {
                            var pcat = new PriceType
                            {
                                PriceTypeCode = row["ZPTZPT"].ToString().TrimEnd(' '),
                                PriceTypeName = row["ZPTNAM"].ToString().TrimEnd(' ')
                            };
                            //Price category
                            PriceTypes.Add(pcat);
                        }
                        
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(PriceTypes);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLoblawsEC()
        {
            List<SelectListItem> eclist = new List<SelectListItem>();
            FormulaComponent ec = new FormulaComponent();
            DataSet DBSetEC = null;
            ReturnValue rv = new ReturnValue();
            rv = ec.ListLoblawsEC(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetEC);

            if (DBSetEC != null)
            {
                foreach (DataTable table in DBSetEC.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var vendor = new SelectListItem
                        {
                            Value = row["Rate"].ToString(),
                            Text = row["NAME"].ToString() + " - (" + row["Rate"].ToString() + "%)"
                        };
                        eclist.Add(vendor);
                    }
                }
            }
            else
            {
                TempData["errorcode"] = 911;
            }
            string returnstring = JsonConvert.SerializeObject(eclist);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMetroEC()
        {
            List<SelectListItem> eclist = new List<SelectListItem>();
            FormulaComponent ec = new FormulaComponent();
            DataSet DBSetEC = null;
            ReturnValue rv = new ReturnValue();
            rv = ec.ListMetroEC(Request.Cookies["SecToken"]["SecurityKey"], ref DBSetEC);

            if (DBSetEC != null)
            {
                foreach (DataTable table in DBSetEC.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        var vendor = new SelectListItem
                        {
                            Value = row["Code"].ToString(),
                            Text = row["NAME"].ToString()
                        };
                        eclist.Add(vendor);
                    }
                }
            }
            else
            {
                TempData["errorcode"] = 911;
            }
            string returnstring = JsonConvert.SerializeObject(eclist);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFormulaResults(string jsonpricetypes, string cmid, string gid)
        {
            var pricetypes = jsonpricetypes;
            List<FormulaResult> results = new List<FormulaResult>();
            List<ComponentRow> componentrow = new List<ComponentRow>();
            CostingModelPayload cm = null;
            DataSet dbset = new DataSet();
            CostingModelGroupPayload grouppayload = null;
            CostingModelGroupPricePayload wholesalepl = null;
            CostingModelGroupPricePayload dsdpl = null;
            DataSet result = new DataSet();
            ReturnValue rv = new ERP.CostingModel().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(cmid), ref cm);
            var zone = "";
            switch (cm.priceZone)
            {
                case Pricing.PriceZone.Zone1:
                     zone = "1";
                    break;
                case Pricing.PriceZone.Zone3:
                    zone = "3";
                    break;
                default:
                    zone = "0";
                    break;
            }

            var brand = cm.brand;
            rv = new CostingModelGroupItem().List(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(gid), ref dbset);
            var unitspercase = 0;
            if (dbset != null && rv.Number == 0)
            {
                foreach (DataTable table in dbset.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        unitspercase = int.Parse(row["UNIT_PER_CASE"].ToString());
                        break;
                    }
                }
            }
            rv = new CostingModelGroup().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(gid), ref grouppayload);
            if (grouppayload != null)
            {
                rv = new CostingModelGroupPrice().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(gid), CostingModelGroupPrice.CostingModelGroupPriceType.Wholesale, ref wholesalepl);
                rv = new CostingModelGroupPrice().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(gid), CostingModelGroupPrice.CostingModelGroupPriceType.DSD, ref dsdpl);
            }
            //Unit per case = row["ITEM"][4]
            
            var shortshelflife = grouppayload.shortShelfLife.ToString();
            var currunit = grouppayload.currentPurchasePricePerCase;
            //Calculate prop unit
            var cadequiv = grouppayload.proposedPurchasePricePerCase * grouppayload.exchangeRate;
            var propunit = ((cadequiv + grouppayload.tariffPercent*cadequiv + grouppayload.freightPricePerCase)/unitspercase);
            propunit = Decimal.Round(propunit, 2);

            //Rest of values
            var currppc = grouppayload.currentPurchasePricePerCase;
            var propppc = grouppayload.proposedPurchasePricePerCase;
            var currwholesale = wholesalepl.currentPricePerCase;
            var propwholesale = wholesalepl.proposedPricePerCase;
            var currdsd = dsdpl.currentPricePerCase;
            var propdsd = dsdpl.proposedPricePerCase;
            var dealaccrual = grouppayload.dealAccrualPercent.ToString();
            var branddev = grouppayload.brandDevelopmentPercent.ToString();

            var temp = new List<ComponentRow>();

            rv = new Pricing().ListPriceFormulaResult(Request.Cookies["SecToken"]["SecurityKey"], pricetypes,brand, zone, grouppayload.warehouseCategory,shortshelflife, unitspercase,
                 propwholesale, propdsd, propunit, dealaccrual, branddev, ref result);
            if (rv.Number == 0)
            {
                FormulaResult fr = null; ;
                string pricetype = "";
                foreach (DataTable table in result.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        //
                        
                        if (row["PRC_TYPE"].ToString().Trim(' ') != pricetype)
                        {
                            pricetype = row["PRC_TYPE"].ToString().Trim(' ');
                            if (temp.Count != 0)
                            {
                                fr.Components = componentrow;
                                results.Add(fr);
                                temp = new List<ComponentRow>();
                                
                                componentrow = new List<ComponentRow>();
                            }
                            fr = new FormulaResult
                            {
                                PriceType = pricetype
                            };
                            

                        }
                        else
                        {
                            ComponentRow cr = new ComponentRow
                            {
                                ComponentCode = row["COMPONENT"].ToString().Trim(' '),
                                ComponentText = row["TEXT"].ToString().Trim(' '),
                                SortClass = row["CLASS"].ToString().Trim(' '),
                                Relation = row["RELATION"].ToString().Trim(' '),
                                Unit = row["UOM"].ToString().Trim(' ')

                            };

                            if (float.TryParse(row["RATIO"].ToString().TrimStart(' '), out float f)) { cr.Ratio = f*100; } else { cr.Ratio = -1; }
                            //Check whether current row is the last one of the price type
                            //Total or subtotal
                            if (row["AMOUNT"].ToString().Trim(' ') != "" && cr.SortClass.Trim(' ') == "")
                            {
                                cr.PercVal = -1;
                                cr.TotalIden = true;
                                cr.DollarVal = float.Parse(row["AMOUNT"].ToString().Trim(' '));
                                componentrow.Add(cr);
                                //Save and Reset
                                
                                temp = componentrow;
                                
                                

                            }
                            //Subtotal
                            else if (row["AMOUNT"].ToString().Trim(' ') == "")
                            {
                                cr.PercVal = -1;
                                cr.TotalIden = true;
                                cr.DollarVal = float.Parse(row["SUBTOTAL"].ToString().Trim(' '));
                                componentrow.Add(cr);
                            }
                            //Regualr Component
                            else
                            {
                                cr.PercVal = float.Parse(row["VALUE"].ToString().Trim(' '));
                                cr.TotalIden = false;
                                cr.DollarVal = float.Parse(row["AMOUNT"].ToString().Trim(' '));
                                componentrow.Add(cr);
                            }
                            //Add Component row after initiating all values
                            
                        }
                    }
                    if (fr != null)
                    {
                        fr.Components = componentrow;
                        results.Add(fr);
                        fr = null;
                    }
                }
            }
            string returnstring = JsonConvert.SerializeObject(results);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCurrentPrice(string jsonpricetypes, string cmid, string gid)
        {
            DataSet ds = new DataSet();
            DataSet itemset = new DataSet();
            List<List<string>> currprices = new List<List<string>>();
            string itemid = "";
            ReturnValue rv = new CostingModelGroupItem().List(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(gid), ref itemset);
            if (itemset != null)
            {
                itemid = itemset.Tables[0].Rows[0]["ITEM"].ToString();
                rv = new Pricing().ListCurrentPrice(Request.Cookies["SecToken"]["SecurityKey"], jsonpricetypes, itemid, ref ds);

                if (ds != null)
                {
                    foreach (DataTable table in ds.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            List<string> x = new List<string>();
                            if (decimal.Parse(row["CURRENT_PRICE_Z1"].ToString()) == 0 || decimal.Parse(row["CURRENT_PRICE_Z1"].ToString()) < decimal.Parse(row["CURRENT_PRICE_Z3"].ToString()))
                            {
                                x.Add(row["PRICE_TYPE"].ToString().Trim(' '));
                                x.Add(row["CURRENT_PRICE_Z3"].ToString());
                                currprices.Add(x);
                            }
                            else
                            {
                                x.Add(row["PRICE_TYPE"].ToString().Trim(' '));
                                x.Add(row["CURRENT_PRICE_Z1"].ToString());
                                currprices.Add(x);
                            }
                        }
                    }
                }
            }
            else
            {
                TempData["errorcode"] = 911;
            }
            string returnstring = JsonConvert.SerializeObject(currprices);
            return Json(returnstring, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetGroupItemCount(string gid)
        {
            DataSet ds = new DataSet();
            int count = 0;
            ReturnValue rv = new CostingModelGroupItem().List(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(gid), ref ds);
            TempData["errorcode"] = rv.Number;
            //Current Group
            if (ds != null && rv.Number == 0)
            {
                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        count++;
                    }
                }
            }

            return Json(count, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Test Emails
        /// </summary>
        public void SendEmailNotification(Email email)
        {
            //Create SMTP Client Connection
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Host = "smtp.kehe.com";
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;

            //Create Email Content
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(email.sender.emailAddress, email.sender.displayName);

            //msg.To.Add(new MailAddress("terry.huang@treeoflife.com", "Terry Huang"));
            msg.To.Add(new MailAddress(email.recipients.First().emailAddress, email.recipients.First().displayName));
            foreach (var addr in email.recipientsCC)
            {
                if (addr.emailAddress != null && addr.emailAddress != "" && addr.emailAddress != " ")
                {
                    msg.CC.Add(new MailAddress(addr.emailAddress, addr.displayName));
                }
            }
            msg.Subject = email.subject;
            msg.Body = email.body;
            msg.IsBodyHtml = email.isHTML;
            client.Send(msg);
        }
        /// <summary>
        /// User Team email retrieve test
        /// </summary>
        /// <returns></returns>
        public void BuildEmail(int template, string cmid)
        {
            BrandTeam bt = new BrandTeam();
            DataSet ds = null;
            Email email = new Email();
            //Email Config
            email.isHTML = true;
            email.sender = new EmailAddress
            {
                displayName = "TOL Web Notifications",
                emailAddress = "noreply@treeoflife.com"
            };
            //Get Costing Model values
            CostingModelPayload cmpayload = new CostingModelPayload();
            ReturnValue rv = new ERP.CostingModel().Retrieve(Request.Cookies["SecToken"]["SecurityKey"], Int32.Parse(cmid), ref cmpayload);
            //Bob's Red Mill - 131 - LCHPAD_BDM Test
             rv = new Team().ListUser(Request.Cookies["SecToken"]["SecurityKey"],cmpayload.brand, ref ds);
            if (rv.Number == 0)
            {
                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        EmailAddress addr = new EmailAddress
                        {
                            displayName = row[2].ToString(),
                            username = Regex.Replace(row[1].ToString(), @"\s+", " "),
                            role = row[4].ToString(),
                            emailAddress = Regex.Replace(row[5].ToString(), @"\s+", " ")
                        };
                        if (template == 1)
                        {
                            if (row[4].ToString() == "Brnd.Team Lead")
                            {
                                email.recipients.Add(addr);
                            }
                            else
                            {
                                email.recipientsCC.Add(addr);
                            }
                        }
                        else
                        {
                            if (row[4].ToString() == "BDM")
                            {
                                email.recipients.Add(addr);
                            }
                            else
                            {
                                email.recipientsCC.Add(addr);
                            }
                        }
                    }
                }
            }
            
            
            //Build Rest of the Email
            var url = "";
            if (template == 1)
            {
                email.subject = "Action Required! - Price Change Management";
                url = "http://apps.treeoflife.com/pcm?p=cmr&id=" + cmid.ToString();
                email.body ="<p>Hello "+ email.recipients.First().displayName +",</p><p>There is a new costing model waiting for your approval.<br /></p>" +
                "<span class='text-primary'> Brand Manager:" + email.recipientsCC.FirstOrDefault(x => x.role == "BDM").displayName + "<br /><br />" +
                "Costing Model Name: " + cmpayload.name + "<br /><br /></span>" +
                "<p>Please <b><a href='"+ url + "'>Click Here</a></b> to approve or reject, adding in your comments.</p><br />" +
                "Thank you, <br />Price Change Management Program";
            }
            else if (template == 2)
            {
                if (cmpayload.type == ERP.CostingModel.Type.PriceChange)
                {
                    email.subject = "Action Required! - Price Change Management";
                    url = "http://apps.treeoflife.com/pcm?p=pc&id=" + cmid.ToString();
                    email.body = "<p>Hello " + email.recipients.First().displayName + ",</p><p>Your Costing model have been approved to proceed with a price change proposal.<br /></p>" +
                    "Costing Model Name:" + cmpayload.name + "<br /><br /></span>" +
                    "<p>Please <b><a href='" + url + "'>Click Here</a></b> to create a price change proposal for your costing model.</p><br />" +
                    "Thank you, <br />Price Change Management Program";
                }
                else
                {
                    url = "http://apps.treeoflife.com/pcm?p=cmr&id=" + cmid.ToString();
                    email.subject = "Approval Update - Price Change Management";
                    email.body = "<p>Hello " + email.recipients.First().displayName + ",</p><p>Please be advised that the following costing model has been approved.<br /></p>" +
                    "<span class='text-primary'>Costing Model Name:" + cmpayload.name + "<br /><br /></span>" +
                    "<p><b><a href = '" + url + "' > Click Here </a></b > to view your approved costing model. No further action is required.</p><br />" +
                    "Thank you <br />Price Change Management Program";
                }
                
            }
            else
            {
                email.subject = "Action Required! - Price Change Management";
                url = "http://apps.treeoflife.com/pcm?p=cm&id=" + cmid.ToString();
                email.body = "<p>Hello " + email.recipients.First().displayName + ",</p><p>Please be advised that your costing model has been rejected.<br /></p>" +
                "<span class='text-primary'>Costing Model Name:" + cmpayload.name + "<br /><br />" +
                "Comment: " + cmpayload.comment + "</span>" +
                "<p><b><a href = '" + url + "' > Click Here </a></b > to review your costing model and resubmit with changes.</p><br />" +
                "Thank you <br />Price Change Management Program";
            }

            //For TESTING ONLY
            EmailAddress testaddr = new EmailAddress
            {
                displayName = "PCM Team",
                username = "",
                role = "",
                emailAddress = "canada.pricechangehelp@treeoflife.com"
            };
            email.recipientsCC.Add(testaddr);

            SendEmailNotification(email);
        }
    }
}