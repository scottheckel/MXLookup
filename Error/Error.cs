// File: Error.cs
// Project: MXLookup for C#
// Author: Jamie Kitson
// Date: 01/05/2017
// Summary: A simple library for peforming MXLookup queries against a DNS server of your choice

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MXLookUp
{
    public static class Error
    {
        //Object Error
        public static List<int> _errorArr =  new List<int> { 0 };

        //Object private isError
        private static bool _iserror = false;

        //Object isError
        public static bool isError { get { return _iserror; } set { _iserror = value; } }

        //Error Dict
        private static Dictionary<int, string> _errorList = new Dictionary<int, string>
        {

            {0, "No Errors!" },
            {1, "Error 1: Invalid Domain Name" },
            {2, "Error 2: Invalid DNS IP Address - Failed to parse to IPAddress Object" },
            {3, "Error 3: Invalid DNS IP Address - Using a Private IP" },
            {4, "Error 4: Issue with Validation Object - Please check errors on Validation Object" },
            {5, "Error 5: Unable to connect to host via UDP 53" },
            {6, "Error 6: Error Response from DNS Server" },
            {7, "Error 7: No valid answers returned from DNS Server" }

        };

        public static string[] GetErrors()
        {

            if (_errorArr.Count > 1) _errorArr.Remove(0);

            var returnErrors = new string[_errorArr.Count];

            for (int i = 0; i < _errorArr.Count; i++)
            {
                if(_errorList.ContainsKey(_errorArr[i]))
                {
                    returnErrors[i] = _errorList[_errorArr[i]];
                }

            }

            return returnErrors;

        }




    }
}
