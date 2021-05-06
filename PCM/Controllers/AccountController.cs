using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using PCM.Models.AccountViewModels;
using TOLC.ERP.Application;

namespace PCM.Controllers
{
    [Route("")]
    public class AccountController : Controller
    {
        public object Formathentication { get; private set; }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {

            LoginViewModel model = new LoginViewModel();
            TempData["errorcode"] = 0;
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string usrUserID, string usrPassword, string signon)
        {
            try
            {
                ReturnValue rv = new ReturnValue();
                Session session = null;
                if (usrUserID != null && usrPassword != null)
                {
                    rv = new Security().Logon(usrUserID, usrPassword, ref session,false);
                }
                else
                {
                    rv = new Security().Logon(model.Username, model.Password, ref session,false);
                }
                
                try
                {
                    if (rv.Number != 0)
                    {
                        TempData["errorcode"] = rv.Number;
                        TempData["errormsg"] = rv.Message;
                        TempData["errorsrc"] = rv.Source;
                        return View();
                    }
                    else
                    {
                        TempData["errorcode"] = 0;
                        HttpCookie userCookie = new HttpCookie("SecToken");
                        List<string> approvalbrands = new List<string>();
                        var brandrolelist = session.BrandUserRole;
                        var assistantbrands = brandrolelist.Where(x => (x.Value != Role.BrandRole.Assistant) && (x.Value != Role.BrandRole.None) && (x.Value != Role.BrandRole.Manager) && (x.Value != Role.BrandRole.Administrator));
                        foreach (var i in assistantbrands)
                        {
                            approvalbrands.Add(i.Key);
                        }
                        var approvalbrandsstr = string.Join(",", approvalbrands);
                        userCookie["ApprovalBrands"] = approvalbrandsstr;
                        userCookie["FullName"] = session.FullName;
                        userCookie["Email"] = session.EmailAddress;
                        userCookie["Username"] = session.Username;
                        userCookie["SecurityKey"] = session.securityIdentifier;

                        Response.SetCookie(userCookie);
                        FormsAuthentication.SetAuthCookie(userCookie["SecurityKey"], true);
                        
                        return RedirectToAction("PCMMain", "Home");
                    }
                }
                catch
                {
                    ViewBag.error = "Invalid Login";
                    return View();
                }
                
            }
            catch (InvalidCastException)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }
        }
        [HttpGet]
        public ActionResult LoginAJAX(string username, string password, string role, string page, string id)
        {
            ReturnValue rv = new ReturnValue();
            Session session = null;
            rv = new Security().Logon(username, password, ref session, false);
            if (session == null)
            {
                return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    if (rv.Number != 0)
                    {
                        TempData["errorcode"] = rv.Number;
                        TempData["errormsg"] = rv.Message;
                        TempData["errorsrc"] = rv.Source;
                        return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        TempData["errorcode"] = 0;
                        HttpCookie userCookie = new HttpCookie("SecToken");
                        List<string> approvalbrands = new List<string>();
                        var brandrolelist = session.BrandUserRole;
                        var assistantbrands = brandrolelist.Where(x => (x.Value != Role.BrandRole.Assistant) && (x.Value != Role.BrandRole.None) && (x.Value != Role.BrandRole.Manager) && (x.Value != Role.BrandRole.Administrator));
                        foreach (var i in assistantbrands)
                        {
                            approvalbrands.Add(i.Key);
                        }
                        var approvalbrandsstr = string.Join(",", approvalbrands);
                        userCookie["ApprovalBrands"] = approvalbrandsstr;
                        userCookie["FullName"] = session.FullName;
                        userCookie["Email"] = session.EmailAddress;
                        userCookie["Username"] = session.Username;
                        userCookie["SecurityKey"] = session.securityIdentifier;

                        Response.SetCookie(userCookie);
                        FormsAuthentication.SetAuthCookie(userCookie["SecurityKey"], true);
                        if (page == "" && id == "")
                        {
                            return Json(new { url = Url.Action("PCMMain", "Home") }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var view = "";
                            switch (page)
                            {
                                case "cm":
                                    view = "CostingModel";
                                    break;
                                case "cmr":
                                    view = "CostingModelReview";
                                    break;
                                case "pc":
                                    view = "PriceChange";
                                    break;
                            }
                            return Json(new { url = Url.Action(view, "Home" , new { id = id}) }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                catch
                {
                    ViewBag.error = "Invalid Login";
                    return Json(new { status = "error", errorcode = rv.Number, errormsg = (rv.Message ?? ""), errorsrc = (rv.Source ?? "") }, JsonRequestBehavior.AllowGet);
                }
                
            }
        }
        public ActionResult Logout()
        {
            if (Request.Cookies["SecToken"] != null)
            {
                Response.Cookies["SecToken"].Expires = DateTime.Now.AddDays(-1);
            }
            return RedirectToAction("Login", "Account", new { area = "" });
        }

    }
}