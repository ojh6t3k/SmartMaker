using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HutongGames.PlayMaker;

namespace SmartMaker.PlayMaker
{
    [AddComponentMenu("SmartMaker/PlayMaker/UiListViewProxy")]
    public class UiListViewProxy : MonoBehaviour
    {
        public readonly string builtInOnChangedSelection = "UI LIST VIEW / ON CHANGED SELECTION";

        public string eventOnChangedSelection = "UI LIST VIEW / ON CHANGED SELECTION";

        private PlayMakerFSM _fsm;
        private UiListView _listView;
        private FsmEventTarget _fsmEventTarget;

        void Awake()
        {
            _fsm = FindObjectOfType<PlayMakerFSM>();
            if (_fsm == null)
                _fsm = gameObject.AddComponent<PlayMakerFSM>();

            _listView = GetComponent<UiListView>();
            if (_listView != null)
            {
                _listView.OnChangedSelection.AddListener(OnChangedSelection);
            }

            _fsmEventTarget = new FsmEventTarget();
            _fsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
            _fsmEventTarget.excludeSelf = false;
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnChangedSelection()
        {
            _fsm.Fsm.Event(_fsmEventTarget, eventOnChangedSelection);
        }
    }
}
