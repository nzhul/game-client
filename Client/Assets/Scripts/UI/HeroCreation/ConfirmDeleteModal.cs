using Assets.Scripts.UI.Modals;
using UnityEngine;

namespace Assets.Scripts.UI.HeroCreation
{
    public class ConfirmDeleteModal : Modal<ConfirmDeleteModal>
    {
        public void OnConfirmDeleteBtnPressed()
        {
            Debug.Log("Login Pressed");
        }
    }
}
