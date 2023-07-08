using UnityEngine;

//credits: https://forum.unity.com/threads/how-to-change-the-name-of-list-elements-in-the-inspector.448910/
namespace Mimic
{
    public class ArrayElementTitleAttribute : PropertyAttribute
    {
        private string varname;
        public string Varname => varname;
        public ArrayElementTitleAttribute(string ElementTitleVar) {
            varname = ElementTitleVar;
        }
    }
}