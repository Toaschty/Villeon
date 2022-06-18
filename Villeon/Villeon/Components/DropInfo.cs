using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villeon.Components
{
    public class DropInfo : IComponent
    {
        private int _dropCount;

        private string[] _itemNames;
        private float[] _dropRates;
        private int[] _minAmounts;
        private int[] _maxAmounts;

        public DropInfo(int dropCount, string[] itemNames, float[] dropRates, int[] minAmounts, int[] maxAmounts)
        {
            _dropCount = dropCount;
            _itemNames = itemNames;
            _dropRates = dropRates;
            _minAmounts = minAmounts;
            _maxAmounts = maxAmounts;
        }

        public int DropCount { get => _dropCount; }

        public string[] ItemNames { get => _itemNames; }

        public float[] DropRates { get => _dropRates; }

        public int[] MinAmounts { get => _minAmounts; }

        public int[] MaxAmounts { get => _maxAmounts; }
    }
}
