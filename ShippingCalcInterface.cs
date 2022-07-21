using DNNrocketAPI.Components;
using RocketEcommerce.Components;
using RocketEcommerce.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketEcommerce.RE_CartPriceShipping
{
    public class ShippingCalcInterface : ShippingInterface
    {
        private ShipData GetShipData()
        {
            var shipData = new ShipData("RE_CartPriceShipping_" + PortalUtils.GetPortalId());
            return shipData;
        }
        public override bool Active()
        {
            return GetShipData().Active;
        }

        public override int CalculateShippingCost(CartLimpet cartData)
        {
            return CalcCost(cartData.SubTotalCents);
        }

        public override int CalculateShippingCost(OrderLimpet orderData)
        {
            return CalcCost(orderData.SubTotalCents);
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

        private int CalcCost(int cartSubTotal)
        {
            var shipData = new ShipData("RE_CartPriceShipping_" + PortalUtils.GetPortalId());
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
