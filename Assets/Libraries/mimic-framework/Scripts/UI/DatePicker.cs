using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mimic.UI{

	public class DatePicker : MonoBehaviour {
	    [SerializeField]
	    private Dropdown monthDropdown;
	    [SerializeField]
	    private Dropdown dayDropdown;
	    [SerializeField]
	    private Dropdown yearDropdown;

	    public string Date {
	        get {
	            return monthDropdown.captionText.text + "-" + dayDropdown.captionText.text + "-" + yearDropdown.captionText.text;
	        }
	    }

	    public void SetDate(int month,int day,int year)
	    {
	        SetOptions();
	        monthDropdown.value=month-1;
	        monthDropdown.RefreshShownValue();
	        //monthDropdown.captionText.text = month.ToString("00") ;
	        dayDropdown.value= day - 1;
	        dayDropdown.RefreshShownValue();
	        //dayDropdown.captionText.text = day.ToString("00");
	        yearDropdown.value = year- 2016;
	        yearDropdown.RefreshShownValue();
	        //yearDropdown.captionText.text = year.ToString("00");


	    }

        public void SetDate(DateTime date)
        {
	        SetOptions();
            SetDate(date.Month,date.Day,date.Year);
        }

        private void SetOptions()
	    {
	        monthDropdown.ClearOptions();
	        for (int m = 1; m <= 12; ++m)
	        {
	            monthDropdown.options.Add(new Dropdown.OptionData(m.ToString("00")));
	        }
	        dayDropdown.ClearOptions();
	        for (int m = 1; m <= 31; ++m)
	        {
	            dayDropdown.options.Add(new Dropdown.OptionData(m.ToString("00")));
	        }

	        yearDropdown.ClearOptions();
	        for (int m = 2016; m <= System.DateTime.Now.Year; ++m)
	        {
	            yearDropdown.options.Add(new Dropdown.OptionData(m.ToString("0000")));
	        }
	    }
	}
}
