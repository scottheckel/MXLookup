// File: MXSend.cs
// Project: MXLookup for C#
// Author: Jamie Kitson
// Date: 01/05/2017
// Summary: A simple library for peforming MXLookup queries against a DNS server of your choice

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MXLookUp
{
    class MXSend
    {
        //Fields for Error Handling
        protected bool _isError = MXLookUp.Error.isError;

        //Validate Obj
        private Validate _obj;

        //Socket
        private Socket _mxSocSend;

        //Transaction ID - Random Bytes
        private byte[] _transArr = new byte[2] { 0x00, 0x00 };
        private Random _transRand = new Random();

        //ByteArr DNS Header for DNS Send
        private byte[] _dnsArrHeader = new byte[] 
        {
           0x00, 0x00, //Transaction ID (2x bytes) - See TransArr Above
           0x01, 0x00, //Flags (2x bytes)
           0x00, 0x01, //Questions (2x bytes) - 1x Question
           0x00, 0x00, //Answers RRs (2x bytes)
           0x00, 0x00, //Authority RRs (2x bytes)
           0x00, 0x00  //Additional RRs (2x bytes)
        };

        //ByteList DNS Start Data for Hostname
        private List<byte> _dnsArrDataStart;

        //ByteArr DNS End Data
        private byte[] _dnsArrDataEnd = new byte[]
        {
            0x00, //End of Hostname
            0x00, 0x0f, //Type -> MX Lookup (15)
            0x00, 0x01, //Class -> INET
        };

        //Complete Byte Array for Socket Send
        private byte[] _dnsArrComplete;

        //***********Properties****************

        public byte[] TransactionID
        {
            get { return _transArr; }
        }

        public Socket MXSendSocket
        {
            get { if (_mxSocSend != null) return _mxSocSend; return null; }
        }

        public bool Errors
        {
            get { return _isError; }
        }

        public MXSend(Validate obj)
        {
            _obj = obj;
            //Set Transaction ID & inject into _dnsArrHeader
            _transRand.NextBytes(_transArr);
            _dnsArrHeader[0] = _transArr[0];
            _dnsArrHeader[1] = _transArr[1];

        }

        public bool Send()
        {
            convertDomain();
            dnsSend();
            return true;
        }
        

        private void convertDomain()
        {

            var domArr = new string[_obj.Domain.Split('.').Length];
            domArr = _obj.Domain.Split('.');

            _dnsArrDataStart = new List<byte>();

            for (int i = 0; i < domArr.Length; i++)
            {
                //Count Byte
               _dnsArrDataStart.Add((byte)domArr[i].Length);

                //Add Data
                foreach(var data in domArr[i])
                {
                    _dnsArrDataStart.Add((byte)data);
                }

            }

            //Inject Arr's into final Array
            int _dnsArrCount = _dnsArrHeader.Length + _dnsArrDataStart.Count + _dnsArrDataEnd.Length;
            _dnsArrComplete = new byte[_dnsArrCount];

            _dnsArrHeader.CopyTo(_dnsArrComplete, 0);
            _dnsArrDataStart.CopyTo(_dnsArrComplete, _dnsArrHeader.Length);
            _dnsArrDataEnd.CopyTo(_dnsArrComplete, _dnsArrHeader.Length + _dnsArrDataStart.Count);

        }

        private void dnsSend()
        {
            _mxSocSend = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _mxSocSend.Connect(_obj.DNSProvider, 53);

            if (!_mxSocSend.Connected)
            {
                Error.isError = true;
                Error._errorArr.Add(5);

            }

            _mxSocSend.Send(_dnsArrComplete, SocketFlags.None);

        }



    }
}
