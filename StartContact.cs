using DNNrocketAPI;
using DNNrocketAPI.Componants;
using RocketEcommerce.Componants;
using Simplisity;
using System;
using System.Collections.Generic;

namespace RocketEcommerce.RE_CartPriceShipping
{
    public class StartConnect : APInterface
    {
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private RocketInterface _rocketInterface;
        private string _currentLang;
        private Dictionary<string, string> _passSettings;
        private SystemLimpet _systemData;

        public override Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return ERROR if not matching commands.
            var rtnDic = new Dictionary<string, object>();

            paramCmd = paramCmd.ToLower();

            _systemData = new SystemLimpet(systemInfo);
            _rocketInterface = new RocketInterface(interfaceInfo);

            _postInfo = postInfo;
            _paramInfo = paramInfo;

            _currentLang = langRequired;
            if (_currentLang == "") _currentLang = DNNrocketUtils.GetCurrentCulture();

            var portalShop = new PortalShopLimpet(PortalUtils.GetPortalId(), DNNrocketUtils.GetEditCulture());
            var securityData = new SecurityLimet(portalShop.PortalId, _systemData.SystemKey, _rocketInterface, -1, -1);
            // Add any extra command that the provider needs.
            securityData.AddCommand("cartpriceship_edit", true);
            securityData.AddCommand("cartpriceship_save", true);
            securityData.AddCommand("cartpriceship_delete", true);
            securityData.AddCommand("cartpriceship_addrange", true);

            paramCmd = securityData.HasSecurityAccess(paramCmd, "cartpriceship_login");

            switch (paramCmd)
            {
                case "cartpriceship_login":
                    strOut = UserUtils.LoginForm(systemInfo, postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
                    break;

                case "cartpriceship_edit":
                    strOut = EditData();
                    break;
                case "cartpriceship_save":
                    SaveData();
                    strOut = EditData();
                    break;
                case "cartpriceship_delete":
                    DeleteData();
                    strOut = EditData();
                    break;
                case "cartpriceship_addrange":
                    AddRange();
                    strOut = EditData();
                    break;
            }

            if (!rtnDic.ContainsKey("outputjson")) rtnDic.Add("outputhtml", strOut);
            return rtnDic;
        }

        public String EditData()
        {
            var payData = new ShipData(PortalUtils.SiteGuid());

            var razorTempl = RenderRazorUtils.GetRazorTemplateData(_rocketInterface.DefaultTemplate, _rocketInterface.TemplateRelPath, _rocketInterface.DefaultTheme, _currentLang, _rocketInterface.ThemeVersion, true);
            var strOut = RenderRazorUtils.RazorDetail(razorTempl, payData.Info, _passSettings, new SessionParams(_paramInfo), true);
            return strOut;
        }
        public void SaveData()
        {
            var payData = new ShipData(PortalUtils.SiteGuid());
            payData.Save(_postInfo);
        }
        public void DeleteData()
        {
            var payData = new ShipData(PortalUtils.SiteGuid());
            payData.Delete();
        }
        public void AddRange()
        {
            var shipData = new ShipData(PortalUtils.SiteGuid());
            shipData.Info.AddListItem("range");
            shipData.Update();
        }



    }
}
