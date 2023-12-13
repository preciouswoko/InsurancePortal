using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InsuranceCore.Interfaces;
using InsuranceInfrastructure.Helpers;
using InsuranceInfrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static InsuranceCore.DTO.ReusableVariables;

namespace InsuranceManagement.Controllers
{
   // [TypeFilter(typeof(AuditFilterAttribute))]
    public class MenuController : Controller
    {
        private TemporaryVariables temporaryVariables;
        private GlobalVariables globalVariables;
        private List<SelectListItem> alrow = new List<SelectListItem>();
        private readonly GlobalVariables _globalVariables;
        private readonly TemporaryVariables _temporaryVariables;
        private readonly IUtilityService _utils;
        private readonly ISessionService _service;
        public MenuController(IUtilityService utils,  IHttpContextAccessor hcontext, ISessionService service)
        {
            _utils = utils;
            _service = service;
            _globalVariables = _service.Get<GlobalVariables>("GlobalVariables");
            _temporaryVariables = _service.Get<TemporaryVariables>("TemporaryVariables");
        }


        //int all_ctr = 1; string str1; string tr1;

        public ActionResult ItemRun(string id)
        {
            //// check he has accesss
            string str1;
            int pos1 = id.IndexOf("[]");
            string code1 = id.Substring(0, pos1);
            //int pos2 = id.IndexOf("[]", pos1 + 2);
            //string type1 = id.Substring(pos1 + 2, pos2 - pos1 - 2);
            //string optiony = id.Substring(pos2 + 2);


            // check for self services
            var menu = MenuItemRepository.Menus().Where(x => x.Code == code1).FirstOrDefault();
            if (menu == null)
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(menu.Controller))
                return RedirectToAction("Index", "home");

            if (!string.IsNullOrWhiteSpace(menu.Parameter))
            {
                string ancs = Ccheckg.convert_pass2("pc=" + menu.Parameter);
                return RedirectToAction(menu.Action.Trim(), menu.Controller.Trim(), new { anc = ancs });
            }
            else
            {
                string p1 = "pc=" + code1;
                string ancs = Ccheckg.convert_pass2(p1);
                return RedirectToAction(menu.Action.Trim(), menu.Controller.Trim(), new { anc = ancs });
            }
          
        }


    }
  

}
