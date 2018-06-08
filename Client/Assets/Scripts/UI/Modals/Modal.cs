using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI.Modals
{
    public abstract class Modal<T> : Modal where T : Modal<T>
    {

    }

    public abstract class Modal : MonoBehaviour
    {
        public virtual void OnClosePressed()
        {
            if (ModalManager.Instance != null)
            {
                ModalManager.Instance.CloseModal();
            }
        }
    }
}
