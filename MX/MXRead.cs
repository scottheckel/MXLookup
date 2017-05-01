// File: MXRead.cs
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
    class MXRead
    {
        //Fields for Error Handling
        protected bool _isError = Error.isError;

        //Socket
        private Socket _mxSocReceive;

        //Domain
        private List<byte> _readDomain;

        //Return Answer
        private int _rranswer;

        //Receive Byte Array
        private int _postion = 12;
        private byte[] _readArr;

        //Loop for first instance
        private bool _isFirstLoop = true;

        //Success MX Return Array
        private MXResult[] _successArray;


        public MXResult[] Results
        {
            get { return _successArray; }
        }

        public bool Errors
        {
            get { return _isError; }
        }

        public MXRead(MXSend mxsendobj)
        {
            _mxSocReceive = mxsendobj.MXSendSocket;
            _readArr = new byte[512];
            _mxSocReceive.Receive(_readArr);
            this.Read();

        }


        private void Read()
        {

            //Response - Query 
            checkFlags(); //Check Bit Arr for Flag Error
            _rranswer = _readArr[7]; //Return Answers (1x Byte)

            if (_rranswer <= 0) {

                Error.isError = true;
                Error._errorArr.Add(7);
                return;
            }

            _successArray = new MXResult[_rranswer];
            _readDomain = (List<byte>)readData(_postion); //Store Query Domain Name (incase referenced later on in answers)
            _postion += _readDomain.Count + 6; //Skip 0x00 (End Of Name), DNS Query Type (2x Bytes) & Class (2x Bytes)
            //End of Query

            int dataLength; // Data Length in Little Endian 
            int pref; // Preference in Little Endian 
            byte[] record;

            for (int i = 0; i < _rranswer; i++) //Loop's number of return answers (_rranswer)
            {

                dataLength = BitConverter.ToInt16(new byte[] { _readArr[_postion + 11], _readArr[_postion + 10] }, 0); // Data Length in Little Endian 
                pref = BitConverter.ToInt16(new byte[] { _readArr[_postion + 13], _readArr[_postion + 12] }, 0); // Data Length in Little Endian
                record = readData(_postion + 14).ToArray(); // Read MX Hostname Data
                
                //Create new MXResult Object
                _successArray[i] = new MXResult() { Preference = pref, Hostname = Encoding.ASCII.GetString(record) };

                if( i+1 < _rranswer) _postion += dataLength + 12;
                

            }
        }



        private IEnumerable<byte> readData(int pos, List<byte> _temp = null)
        {
            int posCount = (_readArr[pos] == 0xc0) ? _readArr[pos+1] : pos;
            int arrpos = _readArr[posCount];
           
            if (_temp == null) _temp = new List<byte>();

            for (int i = 0; i < arrpos; i++)
            {
                posCount += 1;
                _temp.Add(_readArr[posCount]);

            }

            switch(_readArr[posCount + 1])
            {
                case 0x00:
                    //End of Line
                    break;

                case 0xc0:
                    _temp.Add(0x2E);
                    readData(_readArr[posCount + 2], _temp);
                    break;

                default:
                    _temp.Add(0x2E);
                    readData(posCount + 1, _temp);
                    break;

            }

            _isFirstLoop = true;
            return _temp;

        }

        private void checkFlags()
        {
            BitArray bits = new BitArray(new byte[] { _readArr[2], _readArr[3] });

            if (bits[7] == true)
            {
                for (int i = 12; i >= 8; i--)
                {
                    if (bits[i] == true)
                    {
                        Error.isError = true;
                        Error._errorArr.Add(6);
                    }
                        
                }

            }
        }


    }
}
