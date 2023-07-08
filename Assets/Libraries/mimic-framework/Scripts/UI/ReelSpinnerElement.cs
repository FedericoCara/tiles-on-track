using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI {

    public class ReelSpinnerElement : MonoBehaviour {

        [SerializeField]
        protected Text valueTxt;

        protected int value;
        public virtual int Value {
            get { return value; }
            set { 
                this.value = value; 
                valueTxt.text = value.ToString();
            }
        }

        protected bool selected;
        public virtual bool Selected {
            get { return selected; }
            set { selected = value; }
        }       
        
    }

}