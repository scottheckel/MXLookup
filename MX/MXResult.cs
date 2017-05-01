// File: MXResult.cs
// Project: MXLookup for C#
// Author: Jamie Kitson
// Date: 01/05/2017
// Summary: A simple library for peforming MXLookup queries against a DNS server of your choice

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MXLookUp.MX
{
    public struct MXResult
    {
        private int _pref;
        private string _hostname;

        public int Preference { get { return _pref; }  set { _pref = value; } }
        public string Hostname { get { return _hostname; }  set { _hostname = value; } }
        
    }
}
