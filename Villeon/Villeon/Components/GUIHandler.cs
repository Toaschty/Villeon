using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.GUI;

namespace Villeon.Components
{
    public class GUIHandler : IComponent
    {
        private static GUIHandler? _instance;

        private DungeonMenu _dungeonMenu;
        private EquipmentMenu _equipmentMenu;
        private InventoryMenu _inventoryMenu;

        private IGUIMenu? _currentMenu;

        private GUIHandler()
        {
            _dungeonMenu = new DungeonMenu();
            _equipmentMenu = new EquipmentMenu();
            _inventoryMenu = InventoryMenu.GetInstance();
            _currentMenu = null;
        }

        public DungeonMenu DungeonMenu => _dungeonMenu;

        public EquipmentMenu EquipmentMenu => _equipmentMenu;

        public InventoryMenu InventoryMenu => _inventoryMenu;

        public IGUIMenu? CurrentMenu
        {
            get { return _currentMenu; }
            set { _currentMenu = value; }
        }

        public static GUIHandler GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GUIHandler();
            }

            return _instance;
        }
    }
}
