using DNNrocketAPI;
using DNNrocketAPI.Components;
using RocketEcommerceAPI.Components;
using Simplisity;

namespace RocketEcommerceAPI.RE_CartPriceShipping
{
    public class ShipData
    {
        private const string _entityTypeCode = "CARTPRICESHIP";
        private const string _tableName = "RocketEcommerceAPI";
        private const string _systemKey = "rocketecommerceapi";
        private string _guidKey;
        private DNNrocketController _objCtrl;
        public ShipData(int portalid)
        {
            PortalShop = new PortalShopLimpet(portalid, DNNrocketUtils.GetCurrentCulture());
            _guidKey = portalid + "_" + _systemKey + "_" + _entityTypeCode;
            _objCtrl = new DNNrocketController();
            Info = _objCtrl.GetByGuidKey(portalid,-1, _entityTypeCode, _guidKey, "", _tableName);
            if (Info == null)
            {
                Info = new SimplisityInfo();
                Info.TypeCode = _entityTypeCode;
                Info.GUIDKey = _guidKey;
                Info.PortalId = portalid;
            }
        }
        public void Save(SimplisityInfo postInfo)
        {
            Info.XMLData = postInfo.XMLData;

            // we rewrite to the property, so we use the correct format for the culture.
            var pAmount = PortalShop.CurrencyConvertToCulture(Info.GetXmlPropertyRaw("genxml/textbox/defaultcost"));
            Info.SetXmlPropertyInt("genxml/textbox/defaultcost", PortalShop.CurrencyConvertCents(pAmount.ToString()).ToString());

            var lp = 1;
            foreach (var r in Info.GetList("range"))
            {
                var amt = PortalShop.CurrencyConvertToCulture(Info.GetXmlPropertyRaw("genxml/range/genxml[" + lp + "]/textbox/lowrange"));
                Info.SetXmlPropertyInt("genxml/range/genxml[" + lp + "]/textbox/lowrange", PortalShop.CurrencyConvertCents(amt.ToString()).ToString());
                var amt2 = PortalShop.CurrencyConvertToCulture(Info.GetXmlPropertyRaw("genxml/range/genxml[" + lp + "]/textbox/highrange"));
                Info.SetXmlPropertyInt("genxml/range/genxml[" + lp + "]/textbox/highrange", PortalShop.CurrencyConvertCents(amt2.ToString()).ToString());
                var amt3 = PortalShop.CurrencyConvertToCulture(Info.GetXmlPropertyRaw("genxml/range/genxml[" + lp + "]/textbox/cost"));
                Info.SetXmlPropertyInt("genxml/range/genxml[" + lp + "]/textbox/cost", PortalShop.CurrencyConvertCents(amt3.ToString()).ToString());
                lp += 1;
            }

            ValidateAndUpdate();
            LogUtils.LogTracking("Save - UserId: " + UserUtils.GetCurrentUserId() + " " + postInfo.XMLData, "CARTPRICESHIP");
        }
        public int ValidateAndUpdate()
        {
            Validate();
            return Update();
        }
        public void Validate()
        {
        }
        public int Update()
        {
            return _objCtrl.Update(Info, _tableName);
        }
        public void Delete()
        {
            if (Info.ItemID > 0)
            {
                _objCtrl.Delete(Info.ItemID);
                LogUtils.LogTracking("DELETE - UserId: " + UserUtils.GetCurrentUserId() + " " + Info.ItemID, "CARTPRICESHIP");
            }
        }
        public bool DebugMode
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/debugmode"); }
            set { Info.SetXmlProperty("genxml/checkbox/debugmode", value.ToString()); }
        }
        public bool Active
        {
            get { return Info.GetXmlPropertyBool("genxml/checkbox/active"); }
            set { Info.SetXmlProperty("genxml/checkbox/active", value.ToString()); }
        }
        public PortalShopLimpet PortalShop { get; set; }
        public SimplisityInfo Info { get; set; }
        public string SelectText
        {
            get { return Info.GetXmlProperty("genxml/lang/genxml/textbox/selecttext"); }
            set { Info.SetXmlProperty("genxml/lang/genxml/textbox/selecttext", value); }
        }
        public string Msg
        {
            get { return Info.GetXmlProperty("genxml/lang/genxml/textbox/msg"); }
            set { Info.SetXmlProperty("genxml/lang/genxml/textbox/msg", value); }
        }
        public string InterfaceKey { get { return "cartpriceship"; } }

    }
}
