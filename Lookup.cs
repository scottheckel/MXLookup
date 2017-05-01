// File: Lookup.cs
// Project: MXLookup for C#
// Author: Jamie Kitson
// Date: 01/05/2017
// Summary: A simple library for peforming MXLookup queries against a DNS server of your choice

using MXLookUp.MX;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MXLookUp
{
    public class Lookup
    {

        //Validate Obj
        private Validate _obj;

        //MXSend & MXReceive
        private MXSend _mxSend;
        private MXRead _mxRead;

        public Lookup(Validate obj)
        {
            _obj = MXLookUp.Error.isError ? null : obj;
            if (_obj == null) MXLookUp.Error.isError = true;
        }

        public IEnumerable<MXResult> Result
        {

            get
            {
                if (MXSendData() == false && MXReadData() == false && Error.isError != true)
                {
                    //Sort by Preference
                    var result = _mxRead.Results.OrderBy(o => o.Preference).ToList();
                    return result;

                }

                return new List<MXResult> { new MXResult { Preference = -1, Hostname = "See Error Object" } };
            }
        }

        private bool MXSendData()
        {
            _mxSend = new MXSend(_obj);
            _mxSend.Send();
            return Error.isError;
        }

        private bool MXReadData()
        {
            _mxRead = new MXRead(_mxSend);
            return Error.isError;

        }

    }
}
