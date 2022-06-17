using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Villeon.Components;
using Villeon.EntityManagement;
using Villeon.GUI;
using Villeon.Helper;

namespace Villeon.Systems.Update
{
    public class DialogSystem : System, IUpdateSystem
    {
        private DialogBox? _dialogBox;

        public DialogSystem(string name)
            : base(name)
        {
            Signature.IncludeAND(typeof(Interactable), typeof(Dialog));
        }

        public void Update(float time)
        {
            foreach (IEntity entity in Entities)
            {
                Interactable interactable = entity.GetComponent<Interactable>();

                // Maybe close all gui menus?

                // Spawn the Dialogbox
                // If Player can Interact, has pressed T and there is no Dialogbox
                // Todo: has pressed T could be one of the options. ####
                if (interactable.CanInteract && Helper.KeyHandler.IsPressed(Keys.T) && _dialogBox is null)
                {
                    GUIHandler.GetInstance().ClearMenu();
                    StateManager.InDialog = true;
                    Dialog dialog = entity.GetComponent<Dialog>();
                    _dialogBox = new DialogBox(dialog.DialogString);
                }

                // Go to the next page in the current dialog box
                if (interactable.CanInteract && Helper.KeyHandler.IsPressed(Keys.N) && _dialogBox is not null)
                {
                    bool hasNextPage = _dialogBox.NextPage();

                    // There is no next page -> close the DialogBox
                    // Player can now move again
                    if (hasNextPage is false)
                    {
                        StateManager.InDialog = false;
                        _dialogBox.Close();
                        _dialogBox = null;
                    }
                }
            }

        }
    }
}
