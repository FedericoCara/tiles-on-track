using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI{
	
	public abstract class NavigationLayout : MonoBehaviour {

		[SerializeField]
		protected Button backBtn;

		[SerializeField]
		protected Button nextBtn;

        [SerializeField]
        protected Button finishBtn;

        [SerializeField]
        protected Button closeBtn;

        [SerializeField]
        protected bool circular = false;

        public delegate void IndexChangedHandler(NavigationLayout layoutChanging);
        public event IndexChangedHandler IndexChanged;

        [SerializeField]
		protected int index = 0;
		public int Index {
			get{ return index; }
			set{ 
                int prevIndex = index;
                index = System.Math.Max(0, System.Math.Min(value, Count - 1)); 
				Refresh();
                if(prevIndex != index) {
                    OnIndexChanged();
                }
			}
		}

        public virtual bool IsFirstPage {
            get { return index == 0; }
        }

        public virtual bool IsLastPage {
            get { return index == Count - 1; }
        }

        public abstract int Count {
            get;
        }

        protected virtual bool Active {
            set{ gameObject.SetActive(value); }
        }

		protected virtual void Start(){
            CheckButtonsVisibility();
		}

        protected virtual void OnEnable(){
            CheckButtonsVisibility();
		}

        protected virtual void OnIndexChanged() {
            IndexChanged?.Invoke(this);
        }

		protected virtual void CheckButtonsVisibility(){
            if (!circular) {
                backBtn.gameObject.SetActive(index > 0);
                nextBtn.gameObject.SetActive(index < Count - 1);
            }
		}

		public virtual void Next(){
            if (circular) {
                Index = (index + 1) % Count;
            } else if (index < Count - 1) {
				Index++;
            }
		}

		public virtual void Back(){
            if (circular) {
                Index = (index - 1) < 0 ? (index - 1 + Count) : (index - 1);
            } else if (index > 0) {
				Index--;
            }
		}

		protected virtual void Refresh(){
            if (!circular) {
                backBtn.gameObject.SetActive(index > 0);

                bool last = index >= Count - 1;
                nextBtn.gameObject.SetActive(!last);
                if(finishBtn != null) {
                    finishBtn.gameObject.SetActive(last);
                }
            }
        }

        public virtual void Close() {
            Active = false;
        }

    }

}
