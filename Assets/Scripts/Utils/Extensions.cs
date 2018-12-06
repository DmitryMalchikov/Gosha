using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Assets.Scripts.Managers;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class Extensions
    {
        public static string Sha1Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                Encoding Encoding1252 = Encoding.GetEncoding(1252);
                var hash = sha1.ComputeHash(Encoding1252.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static List<T> SubList<T>(this List<T> list, int startIndex, int amount)
        {
            List<T> res = new List<T>();

            T[] temp = new T[amount];
            list.CopyTo(startIndex, temp, 0, amount);

            res.AddRange(temp);

            return res;
        }

        public static void ShowGameObjects(List<GameObject> objects, bool show = true)
        {
            if (objects == null)
            {
                return;
            }
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].SetActive(show);
            }
        }

        public static string AddBrackets(this string input)
        {
            int index = input.LastIndexOf(" ");
            input = input.Insert(index + 1, "(");
            input += ")";
            input = input.Replace(" Card", "");
            input = input.Replace(" Suit", "");
            return input;
        }

        public static string TimeSpanToLocalizedString(this TimeSpan time)
        {
            time = time.Duration();
            StringBuilder timeLeft = new StringBuilder();

            if (time.Days > 0)
            {
                timeLeft.AppendFormat("{0:00} {1} ", time.Days, LocalizationManager.GetLocalizedValue("days"));
            }
            if (time.Hours > 0)
            {
                timeLeft.AppendFormat("{0:00} {1} ", time.Hours, LocalizationManager.GetLocalizedValue("hours"));
            }
            if (time.Minutes > 0)
            {
                timeLeft.AppendFormat("{0:00} {1}", time.Minutes, LocalizationManager.GetLocalizedValue("minutes"));
            }

            return timeLeft.ToString();
        }

        public static void ClearContent(this Transform transform)
        {
            foreach (Transform item in transform)
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }

        public static T GetEnumeratorResult<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext()) { }
            return enumerator.Current;
        }
    }
}
