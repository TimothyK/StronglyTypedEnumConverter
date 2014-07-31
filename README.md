StronglyTypedEnumConverter
==========================

Converts a basic C# or VB enum to a strong typed enum class.  

A strongly typed enum class offers:
* Invalid integers cannot be assigned
* ToString can be overridden to yeild results different that the member names and survive obfuscators
* Solves the Primitive Obsession code smell
* Provides the same intellisense goodness as a standard enum, plus discoverability with our custom added methods


```C#
//A simple C# enum
enum CowboyType {
    Good,
    Bad,
    Ugly
}
```

This tool converts to the following:

```C#
class CowboyType
{

    private CowboyType() { }

    public static readonly CowboyType Good = new CowboyType();
    public static readonly CowboyType Bad = new CowboyType();
    public static readonly CowboyType Ugly = new CowboyType();

    public override string ToString()
    {
        var map = new Dictionary<CowboyType, string>
        {
            {Good, "Good"},
            {Bad, "Bad"},
            {Ugly, "Ugly"}
        };

        return map[this];
    }
    
    //And much, much more...
    //* An All method to iterate over the enum members
    //* A FromString static method
    //* Explicit operator methods to convert to/from an integer
    
    //Plus add your own...
    //* DefaultHatColor method
    //* To/From how this value is represented in a database or other external systems
}
```
Read more: http://geekswithblogs.net/TimothyK/archive/2014/07/31/strongly-typed-enum-pattern.aspx

The conversion routine is hosted by a simple WPF desktop application.  
Its also available as a DLL so that if you want you can
build a command line tool, Visual Studio or Resharper Add-in (please share).


License
=======

The MIT License (MIT)

Copyright (c) 2014 Timothy Klenke

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
