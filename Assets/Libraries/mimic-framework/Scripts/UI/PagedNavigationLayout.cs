using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI{
	
	public class PagedNavigationLayout : NavigationLayout {
		
		[SerializeField]
		protected List<GameObject> pages;

        public override int Count {
            get{ return pages.Count; }
        }

        public GameObject CurrentPage {
            get {
                if (pages.Count > 0)
                    return pages[Index];
                else
                    return null;
            }
        }

		protected override void Refresh(){
            for (int i = 0; i < pages.Count; i++) {
				pages [i].SetActive (i == index);
			}
            base.Refresh();
        }

        public virtual void AddPage(GameObject newPage, bool setAsCurrentIndex = false) {
            pages.Add(newPage);
            if (setAsCurrentIndex) {
                Index = pages.Count - 1;
            }
        }

        public virtual void ClearPages() {
            pages.ForEach(page => Destroy(page));
            pages.Clear();
        }
    }

}
