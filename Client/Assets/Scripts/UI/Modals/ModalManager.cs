using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI.Modals
{
    public class ModalManager : MonoBehaviour
    {
        [SerializeField]
        private Modal primaryModalPrefab;

        [SerializeField]
        public Modal[] Modals;

        [SerializeField]
        private Transform _modalParent;

        private Stack<Modal> _modalStack = new Stack<Modal>();

        private static ModalManager _instance;

        public static ModalManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                InitializeModals();

                // for this to work the object must be on root level in the hierarchy
                // TODO: this might cause bugs since i will have two/three stacks of menus, each for each scene
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void InitializeModals()
        {
            if (_modalParent == null)
            {
                GameObject modalParentObject = new GameObject("Modals");
                _modalParent = modalParentObject.transform;
            }

            DontDestroyOnLoad(_modalParent.gameObject);

            foreach (Modal modal in Modals)
            {
                Modal modalInstance = Instantiate(modal, _modalParent);
                if (modal != primaryModalPrefab)
                {
                    modalInstance.gameObject.SetActive(false);
                }
                else
                {
                    OpenModal(modalInstance);
                }
            }
        }

        public void OpenModal(Modal modalInstance)
        {
            if (modalInstance == null)
            {
                Debug.LogWarning("MODALMANAGER: OpenModal ERROR: invalid menu");
                return;
            }

            if (_modalStack.Count > 0)
            {
                foreach (Modal modal in _modalStack)
                {
                    modal.gameObject.SetActive(false);
                }
            }

            modalInstance.gameObject.SetActive(true);
            _modalStack.Push(modalInstance);
        }

        public void CloseModal()
        {

        }
    }
}
