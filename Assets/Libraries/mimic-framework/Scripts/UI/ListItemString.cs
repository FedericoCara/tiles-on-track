using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI
{
    public class ListItemString : ListItem<string>
    {

        [SerializeField]
		protected Text text;
        
        protected override  void Refresh() {
            text.text = Data;
        }

    } 
}
