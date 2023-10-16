using DNNrocketAPI.Components;
using RocketEcommerceAPI.Components;
using RocketEcommerceAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketEcommerceAPI.RE_CartPriceShipping
{
    public class ShippingCalcInterface : ShippingInterface
    {
        private ShipData GetShipData()
        {
            var shipData = new ShipData(PortalUtils.GetPortalId());
            return shipData;
        }
        public override bool Active()
        {
            return GetShipData().Active;
        }

        public override int CalculateShippingCost(CartLimpet cartData)
        {
            return CalcCost(cartData.PortalId, cartData.SubTotalCents);
        }

        public override int CalculateShippingCost(OrderLimpet orderData)
        {
            return CalcCost(orderData.PortalId, orderData.SubTotalCents);
        }

        public override string Msg()
        {
            return GetShipData().Msg;
        }

        public override string SelectText()
        {
            return GetShipData().SelectText;
        }

        public override string ShipProvKey()
        {
            return GetShipData().InterfaceKey;
        }

        private int CalcCost(int portalId, int cartSubTotal)
        {
            var shipData = new ShipData(portalId);
            var cost = shipData.Info.GetXmlPropertyInt("genxml/textbox/defaultcost");
            var datalist = shipData.Info.GetList("range");
            foreach (var rangeInfo in datalist)
            {
                var lowrange = rangeInfo.GetXmlPropertyInt("genxml/textbox/lowrange");
                var highrange = rangeInfo.GetXmlPropertyInt("genxml/textbox/highrange");
                var rangecost = rangeInfo.GetXmlPropertyInt("genxml/textbox/cost");
                if (cartSubTotal >= lowrange && cartSubTotal < highrange) cost = rangecost;
            }
            return cost;
        }

    }
}
