﻿using DNNrocketAPI;
using DNNrocketAPI.Components;
using DNNrocketAPI.Interfaces;
using RocketEcommerceAPI.Components;
using Simplisity;
using System;
using System.Collections.Generic;

namespace RocketEcommerceAPI.RE_CartPriceShipping
{
    public class StartConnect : IProcessCommand
    {
        private SimplisityInfo _postInfo;
        private SimplisityInfo _paramInfo;
        private RocketInterface _rocketInterface;
        private string _currentLang;
        private Dictionary<string, string> _passSettings;
        private SystemLimpet _systemData;
        private const string _systemkey = "RE_CartPriceShipping";
        private AppThemeSystemLimpet _appThemeSystem;

        public Dictionary<string, object> ProcessCommand(string paramCmd, SimplisityInfo systemInfo, SimplisityInfo interfaceInfo, SimplisityInfo postInfo, SimplisityInfo paramInfo, string langRequired = "")
        {
            var strOut = ""; // return ERROR if not matching commands.
            var rtnDic = new Dictionary<string, object>();

            paramCmd = paramCmd.ToLower();

            _systemData = new SystemLimpet(_systemkey);
            _rocketInterface = new RocketInterface(interfaceInfo);
            _appThemeSystem = new AppThemeSystemLimpet(PortalUtils.GetPortalId(), _systemkey);

            _postInfo = postInfo;
            _paramInfo = paramInfo;

            _currentLang = langRequired;
            if (_currentLang == "") _currentLang = DNNrocketUtils.GetCurrentCulture();

            var portalShop = new PortalShopLimpet(PortalUtils.GetPortalId(), DNNrocketUtils.GetEditCulture());
            var securityData = new SecurityLimpet(portalShop.PortalId, _systemData.SystemKey, _rocketInterface, -1, -1);
            // Add any extra command that the provider needs.
            securityData.AddCommand("cartpriceship_edit", true);
            securityData.AddCommand("cartpriceship_save", true);
            securityData.AddCommand("cartpriceship_delete", true);
            securityData.AddCommand("cartpriceship_addrange", true);

            paramCmd = securityData.HasSecurityAccess(paramCmd, "cartpriceship_login");

            switch (paramCmd)
            {
                case "cartpriceship_login":
                    strOut = UserUtils.LoginForm("rocketecommerceapi", postInfo, _rocketInterface.InterfaceKey, UserUtils.GetCurrentUserId());
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
            var shipData = new ShipData(PortalUtils.GetPortalId());
            var razorTempl = _appThemeSystem.GetTemplate("settings.cshtml");
            var pr = RenderRazorUtils.RazorProcessData(razorTempl, shipData.Info, null, _passSettings, new SessionParams(_paramInfo), true);
            if (pr.StatusCode != "00") return pr.ErrorMsg;
            return pr.RenderedText;
        }
        public void SaveData()
        {
            var shipData = new ShipData(PortalUtils.GetPortalId());
            shipData.Save(_postInfo);
        }
        public void DeleteData()
        {
            var shipData = new ShipData(PortalUtils.GetPortalId());
            shipData.Delete();
        }
        public void AddRange()
        {
            var shipData = new ShipData(PortalUtils.GetPortalId());
            shipData.Info.AddListItem("range");
            shipData.Update();
        }



    }
}
