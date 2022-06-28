using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Villeon.Components
{
    public class Option
    {
        public Option(string option, Keys interactionKey)
        {
            OptionString = option;
            Key = interactionKey;
        }

        public Option(string option, Keys interactionKey, string neededItem, int neededAmount, string buyItem, int buyAmount)
        {
            OptionString = option;
            Key = interactionKey;
            NeededItem = neededItem;
            NeededItemAmount = neededAmount;
            BuyItem = buyItem;
            BuyItemAmount = buyAmount;
        }

        public string OptionString { get; set; }

        public string NeededItem { get; set; } = string.Empty;

        public int NeededItemAmount { get; set; } = 0;

        public string BuyItem { get; set; } = string.Empty;

        public int BuyItemAmount { get; set; } = 0;

        public Keys Key { get; set; }
    }
}
