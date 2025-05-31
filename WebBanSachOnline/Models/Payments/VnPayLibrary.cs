using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebBanSachOnline.Models.Payments
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> requestData = new SortedList<string, string>();


        public void AddRequestData(string key, string value)
        {
            requestData.Add(key, value);
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = requestData;
            var queryString = new StringBuilder();
            var rawData = new StringBuilder();

            foreach (var kv in data)
            {
                string key = kv.Key;
                string value = kv.Value;

                queryString.AppendFormat("{0}={1}&", WebUtility.UrlEncode(key), WebUtility.UrlEncode(value));
                rawData.AppendFormat("{0}={1}&", key, WebUtility.UrlEncode(value));
            }

            string rawHash = rawData.ToString().TrimEnd('&');
            string secureHash = HmacSHA512(hashSecret, rawHash);
            queryString.Append("vnp_SecureHash=" + secureHash);
            return baseUrl + "?" + queryString.ToString();
        }


        //public string CreateRequestUrl(string baseUrl, string hashSecret)
        //{
        //    var data = requestData;
        //    var queryString = new StringBuilder();
        //    var rawData = new StringBuilder();

        //    foreach (var kv in data)
        //    {
        //        queryString.AppendFormat("{0}={1}&", WebUtility.UrlEncode(kv.Key), WebUtility.UrlEncode(kv.Value));
        //        rawData.AppendFormat("{0}={1}&", kv.Key, kv.Value);
        //    }

        //    string rawHash = rawData.ToString().TrimEnd('&');
        //    string secureHash = HmacSHA512(hashSecret, rawHash);
        //    queryString.Append("vnp_SecureHash=" + secureHash);
        //    return baseUrl + "?" + queryString.ToString();
        //}

        public static string HmacSHA512(string key, string inputData)
        {
            var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
        }
    }
}