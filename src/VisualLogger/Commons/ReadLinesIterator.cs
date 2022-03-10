using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Commons
{
    //https://referencesource.microsoft.com/#mscorlib/system/io/ReadLinesIterator.cs,ad6ef3e539c0e86e
    // An iterator that returns a single line at-a-time from a given file.
    //
    // Known issues which cannot be changed to remain compatible with 4.0:
    //
    //  - The underlying StreamReader is allocated upfront for the IEnumerable<T> before 
    //    GetEnumerator has even been called. While this is good in that exceptions such as 
    //    DirectoryNotFoundException and FileNotFoundException are thrown directly by 
    //    File.ReadLines (which the user probably expects), it also means that the reader 
    //    will be leaked if the user never actually foreach's over the enumerable (and hence 
    //    calls Dispose on at least one IEnumerator<T> instance).
    //
    //  - Reading to the end of the IEnumerator<T> disposes it. This means that Dispose 
    //    is called twice in a normal foreach construct.
    //
    //  - IEnumerator<T> instances from the same IEnumerable<T> party on the same underlying 
    //    reader (Dev10 Bugs 904764).
    //
    internal class ReadLinesIterator : Iterator<string>
    {
        private readonly StreamReader _reader;

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        public ReadLinesIterator(StreamReader reader)
        {
            _reader = reader;
        }

        public override bool MoveNext()
        {
            if (this._reader != null)
            {
                var line = _reader.ReadLine();
                if (line != null)
                {
                    this.current = line;
                    return true;
                }
            }

            return false;
        }

        public string? ReadNext()
        {
            if (MoveNext())
            {
                return Current;
            }
            else
            {
                return null;
            }
        }

        protected override Iterator<string> Clone()
        {
            // NOTE: To maintain the same behavior with the previous yield-based
            // iterator in 4.0, we have all the IEnumerator<T> instances share the same 
            // underlying reader. If we have already been disposed, _reader will be null, 
            // which will cause CreateIterator to simply new up a new instance to start up
            // a new iteration. Dev10 Bugs 904764 has been filed to fix this in next side-
            // by-side release.
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (_reader != null)
                    {
                        _reader.Dispose();
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
