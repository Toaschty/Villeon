﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villeon.GUI;

namespace Villeon.Components
{
    public class GUIHandler : IComponent
    {
        private DungeonMenu _dungeonMenu;
        private EquipmentMenu _equipmentMenu;

        private IGUIMenu? _currentMenu;

        public GUIHandler()
        {
            _dungeonMenu = new DungeonMenu();
            _equipmentMenu = new EquipmentMenu();
            _currentMenu = null;
        }

        public DungeonMenu DungeonMenu => _dungeonMenu;

        public EquipmentMenu EquipmentMenu => _equipmentMenu;

        public IGUIMenu? CurrentMenu
        {
            get { return _currentMenu; }
            set { _currentMenu = value; }
        }
    }
}
