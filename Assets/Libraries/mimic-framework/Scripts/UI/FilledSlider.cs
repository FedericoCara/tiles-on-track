using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI
{
	public class FilledSlider : MonoBehaviour {

		[SerializeField]
		protected Image bar;
		public float Filled{
			get{ return bar.fillAmount; }
			set{
				value = Mathf.Clamp01 (value);
				bar.fillAmount = value;
				slider.value *= value;
			}
		}

		[SerializeField]
		protected Image barContainer;

		[SerializeField]
		private Slider slider;
		public Slider Slider{get{ return slider;}}
		private float realValue;
		protected float RealValue{
			get{
				return realValue;
			}
			set{
				realValue = value;
			}
		}
		public virtual float Value{
			get{ return realValue; }
			set{
				realValue = value;
				slider.value = Mathf.Clamp01 (value) * bar.fillAmount;
			}
		}
		protected float SliderValue{
			get{
				return slider.value;
			}
			set{
				slider.value = Mathf.Clamp01 (value) * bar.fillAmount;
			}
		}

		public bool LeftToRight{
			set{ 
				Slider.direction = value ? Slider.Direction.LeftToRight : Slider.Direction.RightToLeft;
				bar.fillOrigin = value ? 0 : 1;
			}
		}

		
	}
}