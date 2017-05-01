// File: Validate.cs
// Project: MXLookup for C#
// Author: Jamie Kitson
// Date: 01/05/2017
// Summary: A simple library for peforming MXLookup queries against a DNS server of your choice

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;

namespace MXLookUp
{


    public class Validate
    {

        //Fields for Domain Name
        protected bool _domIsValid;
        protected string _domain;

        //Fields for DNS IP
        protected bool _ipIsValid;
        protected IPAddress _ipAddress;

        //Returns NULL if IP fails validation
        public IPAddress DNSProvider
        {
            get { return _ipAddress; }
        }

        //Returns NULL if Domain Name fails validation
        public string Domain
        {
            get { return _domain;}
        }

        //Error Status
        public bool IsError
        {
            get
            {
                return MXLookUp.Error.isError;
            }
        }

        //Returns Errors
        public string[] Error
        {
            get{ return ErrorHandler(); }
        }

        public Validate(string Domain, string DNS)
        {
            _domain = regexDomain(Domain) ? Domain : null;
            _ipIsValid = validateIP(DNS) ? true : false;

        }


        //Validates Domain using RegEx Query. Format should be: block.(2-10).(2-10)
        private bool regexDomain(string domain)
        {
            bool regReturn = Regex.IsMatch(domain, "^(\\S*)\\.(\\w{2,10}|\\w{2,10}\\.\\w{2,10})$");
            if (regReturn == false) { MXLookUp.Error._errorArr.Add(1); MXLookUp.Error.isError = true; }
            return regReturn;
        }

        //Validates IP to ensure it parses to IPAddress obj and not private
        private bool validateIP(string ip)
        {
      
            IPAddress parseIP;

            if (IPAddress.TryParse(ip, out parseIP))
            {
                _ipAddress = parseIP;

                string[] arr = new string[] { "192 168", "172 16", "127 0", "169 254", "10 0" };
                string[] iparr = parseIP.ToString().Split('.');
                string twoOct = iparr[0] + " " + iparr[1];
                bool result = arr.Contains(twoOct);
                if (result != false) { MXLookUp.Error._errorArr.Add(3); MXLookUp.Error.isError = true; }

                return result;
            }

            MXLookUp.Error.isError = true;
            MXLookUp.Error._errorArr.Add(2);
            return false;
        }

        private string[] ErrorHandler()
        {
            return MXLookUp.Error.GetErrors();
        }

        


         
    }
}
