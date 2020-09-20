using DNNrocketAPI.Componants;
using RocketEcommerce.Componants;
using RocketEcommerce.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketEcommerce.RE_CartPriceShipping
{
    public class ShippingCalcInterface : ShippingInterface
    {
        public override bool Active()
        {
            var systemData = new SystemLimpet("rocketecommerce");
            var rocketInterface = systemData.GetInterface("cartpriceship");
            if (rocketInterface != null)
            {
                var shipData = new ShipData(PortalUtils.SiteGuid());
                return shipData.Active;
            }
            return false;
        }

        public override decimal CalculateShippingCost(CartLimpet cartData)
        {
            return CalcCost(cartData.SubTotal);
        }

        public override decimal CalculateShippingCost(OrderLimpet orderData)
        {
            return CalcCost(orderData.SubTotal);
        }

        private decimal CalcCost(decimal cartSubTotal)
        {
            var shipData = new ShipData(PortalUtils.SiteGuid());
            var cost = shipData.Info.GetXmlPropertyDecimal("genxml/textbox/defaultcost");
            var datalist = shipData.Info.GetList("range");
            foreach (var rangeInfo in datalist)
            {
                var lowrange = rangeInfo.GetXmlPropertyDecimal("genxml/textbox/lowrange");
                var highrange = rangeInfo.GetXmlPropertyDecimal("genxml/textbox/highrange");
                var rangecost = rangeInfo.GetXmlPropertyDecimal("genxml/textbox/cost");
                if (cartSubTotal >= lowrange && cartSubTotal < highrange) cost = rangecost;
            }
            return cost;
        }

    }
}
