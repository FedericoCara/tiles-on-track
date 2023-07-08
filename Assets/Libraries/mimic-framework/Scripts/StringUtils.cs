/// <summary>
/// Tests an E-Mail address.
/// </summary>
using System.Text.RegularExpressions;
using UnityEngine;

namespace Mimic {
    public static class StringUtils {
        /// <summary>
        /// Regular expression, which is used to validate an E-Mail address.
        /// </summary>
        public const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";


        public static readonly Regex NewLineRegex = new Regex("\r\n|\r|\n", RegexOptions.Compiled);


        /// <summary>
        /// Checks whether the given Email-Parameter is a valid E-Mail address.
        /// </summary>
        /// <param name="email">Parameter-string that contains an E-Mail address.</param>
        /// <returns>True, when Parameter-string is not null and contains a valid E-Mail address;
        /// otherwise false.</returns>
        public static bool IsEmail(string email) {
            if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
            else return false;
        }

        public static string ParseTime(float gameTime) {
            string timeFormat = "";                                     // clear time string

            int sec = (int)(gameTime % 60.0f);						// get seconds
            int min = (int)(gameTime / 60.0f);						// get minutes
            int hrs = (int)Mathf.Clamp(gameTime / 3600.0f, 0, 99);  // get hours

            if (min >= 60) {
                min = min % 60;
                timeFormat = timeFormat + hrs.ToString("00") + ":";
            }
            timeFormat = timeFormat + min.ToString("00") + ":";
            timeFormat = timeFormat + sec.ToString("00");
            return timeFormat;
        }

        /// <summary>
        /// Parses time with a HH:mmtt format or a HH:mm
        /// </summary>
        /// <param name="time">Time string</param>
        /// <returns>Fortmatted time span</returns>
        public static System.TimeSpan ParseTimeHHmm(string time)
        {
            try
            {
                int ampmIndex = time.IndexOfAny(new char[]{ 'a', 'A', 'p', 'P' });
                string timeString = time;
                bool pm = false;

                if (ampmIndex >= 0)
                {
                    switch (time.Substring(ampmIndex))
                    {
                        case "am":
                        case "AM":
                            pm = false;
                            break;
                        case "pm":
                        case "PM":
                            pm = true;
                            break;
                        default:
                            throw new System.Exception();
                    }
                    timeString = time.Remove(ampmIndex);
                }

                string[] times = timeString.Split(':');

                int hours = int.Parse(times[0]);
                if (pm) hours += 12;

                int minutes = int.Parse(times[1]);

                return new System.TimeSpan(hours, minutes, 0);
            }
            catch
            {
                throw new System.Exception("Invalid time format: " + time);
            }
        }


        public static string[] GetLines(string input) {
            return NewLineRegex.Split(input);
        }


        public static string GetOrdinal(int number) {
            if (number <= 0) return number.ToString();

            switch (number % 100) {
                case 11:
                case 12:
                case 13:
                    return number + "th";
            }

            switch (number % 10) {
                case 1:
                    return number + "st";
                case 2:
                    return number + "nd";
                case 3:
                    return number + "rd";
                default:
                    return number + "th";
            }

        }

        public static string WordsToUpperCamelCase(string value) {
            char[] array = value.TrimStart().ToLower().ToCharArray();
            if (array.Length >= 1) {
                array[0] = char.ToUpper(array[0]);
            }
            for (int i = 1; i < array.Length; i++) {
                if (char.IsWhiteSpace(array[i - 1])) {
                    if (char.IsLower(array[i])) {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }

        public static string GetRichTextWithHiddenCharacters(string text, int hiddenCharsIndex, int hiddenCharsLength = 1) {
            if(hiddenCharsIndex == text.Length) {
                return text;
            } else {
                return text.Insert(hiddenCharsIndex+hiddenCharsLength, "</color>").Insert(hiddenCharsIndex, "<color=#00000000>");
            }
        }

        public static string[] Wrap(this string text, int maxLength) {
            if (text.Length < maxLength)
                return new string[] { text };
            else {
                string[] wrappedStrings = new string[Mathf.CeilToInt(text.Length / (float) maxLength)];
                for (int i = 0; i < wrappedStrings.Length; i++) {
                    wrappedStrings[i] = text.Substring(i * maxLength, i<wrappedStrings.Length-1? maxLength : (text.Length % maxLength) );
                }
                return wrappedStrings;
            }
        }
    }
}