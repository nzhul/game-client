using UnityEngine;

namespace Assets.Scripts.UI.Modals
{
    public abstract class Modal<T> : Modal where T : Modal<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = (T)this;
            }
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroi()
        {
            _instance = null;
        }

        public virtual void Open()
        {
            if (ModalManager.Instance != null && Instance != null)
            {
                ModalManager.Instance.OpenModal(Instance);
            }
        }


    }

    [RequireComponent(typeof(Canvas))]
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
