# MXLookup Library for C#
A simple library which validates and returns a DNS MXLookup query using a DNS server of your choice. I'm pretty new to programming so I'm sure this can be made a million times better. 

## Requirements
1. C# 6.0
2. An internet connection which allows traffic on UDP port 53

## Usage
Reference the lib in VS and namespace in code (if you wish):

```c#

using MXLookUp; //If you want ref namespace

```

The `Validate` constructor takes a 2x strings in the order of domain & DNS server IP. 

The `Lookup` constructor takes a `Validate` object as its constructor.

`Error` is a static class which is tightly coupled with all classes part of MXLookup. In the example below, I've checked the `Error.isError` property to ensure validation and `Socket` connection has been succesful before returning the result from the `Lookup` object.


```c#
            var validateObj = new Validate("google.com", "8.8.8.8"); //Google for DNS Server
            var lookupObj = new Lookup(validateObj);
            var result = lookupObj.Result;

            if (Error.isError != true)
            {
                foreach(var i in result)
                {
                    Console.WriteLine($"{i.Preference} {i.Hostname}");
                }
            }
            else
            {
                foreach (var i in Error.GetErrors())
                {
                    Console.WriteLine(i);
                }
            }
```

Output:

```
10 aspmx.l.Google.com
20 alt1.aspmx.l.Google.com
30 alt2.aspmx.l.Google.com
40 alt3.aspmx.l.Google.com
50 alt4.aspmx.l.Google.com
```
## Detailed Output (Debug)

A result with a failed input:

```c#

            var validateObj = new Validate("Google.comasdasd", "208.67.222.222"); //OpenDNS for DNS Server
            var lookupObj = new Lookup(validateObj);
            var result = lookupObj.Result;

```

Output:

```
Error 6: Error Response from DNS Server
Error 7: No valid answers returned from DNS Server
```

The failed example passed hostname validation due to the TLD check (TLD's (Top Level Domains) are becoming longer year by year, so I've set the RegEx TLD validation to x10 characters) but failed when querying the DNS server as there were no valid results. Other error reporting built-in can be found below: 

```c#

            {0, "No Errors!" },
            {1, "Error 1: Invalid Domain Name" },
            {2, "Error 2: Invalid DNS IP Address - Failed to parse to IPAddress Object" },
            {3, "Error 3: Invalid DNS IP Address - Using a Private IP" },
            {4, "Error 4: Issue with Validation Object - Please check errors on Validation Object" },
            {5, "Error 5: Unable to connect to host via UDP 53" },
            {6, "Error 6: Error Response from DNS Server" },
            {7, "Error 7: No valid answers returned from DNS Server" }

```
