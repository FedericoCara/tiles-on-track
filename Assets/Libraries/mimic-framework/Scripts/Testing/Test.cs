using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Mimic.Testing {

	public abstract class Test : MonoBehaviour {

        public const string ACTIVE_PREFIX = "(Active) ";
        public const string FINISHED_PREFIX = "(Finished) ";

		[SerializeField]
		protected bool runAtStart = true;
		public bool RunAtStart{
			set{ runAtStart = value; }
		}

		[SerializeField]
		protected Color color = Color.blue;

        private string originalName;

		private bool finished = false;
		public bool Finished {
			get { return finished; }
		}

        protected virtual void Start() {
			if (transform.parent == null) DontDestroyOnLoad(gameObject);

			if (runAtStart) {
				StartCoroutine (InitAndRun ());
			}
		}

		public IEnumerator InitAndRun(){
			yield return BeforeInit ();
			yield return Init ();
			yield return AfterInit ();
			yield return BeforeRun ();
			yield return Run ();
			yield return AfterRun ();
		}

		public virtual IEnumerator BeforeInit(){
			yield return null;
		}

		public virtual IEnumerator Init(){
			yield return null;
		}

		public virtual IEnumerator AfterInit(){
			yield return null;
		}

		public virtual IEnumerator BeforeRun(){
			Log("-------------------- Running Test: "+name+" ----------------------");
            originalName = name;
            name = ACTIVE_PREFIX + name;
			yield return null;
		}

		public abstract IEnumerator Run();

		public virtual IEnumerator AfterRun(){
			finished = true;
            name = FINISHED_PREFIX + originalName;
			yield return null;
		}


        public static IEnumerator Press(GameObject button) {
            UnityEngine.Assertions.Assert.IsTrue(button != null && button.activeInHierarchy, "Button is null, destroyed or disabled.");

            ExecuteEvents.Execute(button, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            yield return null;
            ExecuteEvents.Execute(button, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            ExecuteEvents.Execute(button, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
            yield return null;
        }

        public static IEnumerator Press(GameObject button, int times) {
			for (int i = 0; i < times; i++) {
				yield return Press (button);
			}
		}


		protected IEnumerator DragAndDrop(GameObject gameObject, Vector2 position){

			ExecuteEvents.Execute (gameObject, new BaseEventData (EventSystem.current), ExecuteEvents.beginDragHandler);

//			ExecuteEvents.Execute (gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.beginDragHandler);
//			gameObject.transform.position = position;
//			ExecuteEvents.Execute (gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.endDragHandler);
			yield return null;
		}

		protected void Log(string message){
			Debug.Log("<color="+ColorUtility.ToHtmlStringRGB(color)+">"+message+"</color>", this);
		}

	}
}
