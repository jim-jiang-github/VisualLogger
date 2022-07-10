using System.Collections;
using System.Runtime.Versioning;

namespace VisualLogger.Reader
{
    abstract public class Iterator<TSource> : IEnumerable<TSource>, IEnumerator<TSource>
    {
        int threadId;
        internal int state;
        internal TSource current;

        public Iterator()
        {
            threadId = Thread.CurrentThread.ManagedThreadId;
        }

        public TSource Current
        {
            get { return current; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            current = default(TSource);
            state = -1;
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            return this;
        }

        public abstract bool MoveNext();

        object IEnumerator.Current
        {
            get { return Current; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }

    }

    public class ReadLinesIterator : Iterator<string>
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
