using System.Collections.Generic;
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

        public bool initializeModals = true;
        public bool openPrimaryModal = true;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;

                if (initializeModals)
                {
                    InitializeModals();
                }

                // for this to work the object must be on root level in the hierarchy
                // TODO: this might cause bugs since i will have two/three stacks of menus, each for each scene

                // Uncomment this if you wish to keep modals between scenes
                //DontDestroyOnLoad(gameObject);
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

            // Uncomment this if you wish to keep modals between scenes
            // DontDestroyOnLoad(_modalParent.gameObject);

            foreach (Modal modal in Modals)
            {
                Modal modalInstance = Instantiate(modal, _modalParent);

                if (openPrimaryModal)
                {
                    if (modal != primaryModalPrefab)
                    {
                        modalInstance.gameObject.SetActive(false);
                    }
                    else
                    {
                        OpenModal(modalInstance);
                    }
                }
                else
                {
                    modalInstance.gameObject.SetActive(false);
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
            if (_modalStack.Count == 0)
            {
                Debug.LogWarning("MODALMANAGER CloseModal ERROR: No modals in stack!");
                return;
            }

            Modal topMenu = _modalStack.Pop();
            topMenu.gameObject.SetActive(false);

            if (_modalStack.Count > 0)
            {
                Modal nextMenu = _modalStack.Peek();
                nextMenu.gameObject.SetActive(true);
            }
        }
    }
}
